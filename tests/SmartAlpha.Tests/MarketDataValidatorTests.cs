using SmartAlpha.Analytics.Validation;
using SmartAlpha.Core.Models.Market;
using System.Reflection;

namespace SmartAlpha.Tests;

public class MarketDataValidatorTests
{
    private readonly MarketDataValidator _validator = new();

    // ---------------------------------------------------------------------------
    // Instrument validation
    // ---------------------------------------------------------------------------

    [Fact]
    public void ValidateInstrument_ValidInstrument_ReturnsValid()
    {
        var instrument = new Instrument("AAPL", "Apple Inc.", AssetType.Stock, "USD", "NASDAQ");

        var result = _validator.ValidateInstrument(instrument);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void ValidateInstrument_InvalidAssetType_ReturnsError()
    {
        var instrument = new Instrument("AAPL", "Apple Inc.", (AssetType)999, "USD", "NASDAQ");

        var result = _validator.ValidateInstrument(instrument);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("AssetType", result.Errors[0]);
        Assert.Contains("999", result.Errors[0]);
    }

    [Fact]
    public void ValidateInstrument_NullInstrument_ReturnsError()
    {
        var result = _validator.ValidateInstrument(null!);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("null", result.Errors[0]);
    }

    [Fact]
    public void ValidateInstrument_DefaultInstrument_ReturnsSymbolRequiredError()
    {
        var instrument = new Instrument();

        var result = _validator.ValidateInstrument(instrument);

        Assert.False(result.IsValid);
        Assert.Equal(4, result.Errors.Count);
        Assert.Contains(result.Errors, e => e.Contains("Symbol is required"));
        Assert.Contains("Instrument '<unknown>': Name is required and cannot be empty.", result.Errors);
        Assert.Contains("Instrument '<unknown>': Currency is required and cannot be empty.", result.Errors);
        Assert.Contains("Instrument '<unknown>': Exchange is required and cannot be empty.", result.Errors);
    }

    // ---------------------------------------------------------------------------
    // PriceBar validation
    // ---------------------------------------------------------------------------

    [Fact]
    public void ValidatePriceBar_ValidBar_ReturnsValid()
    {
        var bar = new PriceBar(new DateTime(2024, 1, 15), 150m, 155m, 149m, 153m, 1_000_000m);

        var result = _validator.ValidatePriceBar(bar);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void ValidatePriceBar_NullBar_ReturnsError()
    {
        var result = _validator.ValidatePriceBar(null!);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("null", result.Errors[0]);
    }

    [Fact]
    public void ValidatePriceBar_IncludesSymbolAndIndexInErrorContext()
    {
        var result = _validator.ValidatePriceBar(null!, index: 3, symbol: "TSLA");

        Assert.Contains("symbol='TSLA'", result.Errors[0]);
        Assert.Contains("index=3", result.Errors[0]);
    }

    [Fact]
    public void ValidatePriceBar_NegativeLow_ReturnsError()
    {
        var bar = CreateInvalidPriceBar(low: -1m);

        var result = _validator.ValidatePriceBar(bar, index: 2, symbol: "AAPL");

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Low must be non-negative"));
        Assert.Contains(result.Errors, e => e.Contains("symbol='AAPL'"));
        Assert.Contains(result.Errors, e => e.Contains("index=2"));
    }

    [Fact]
    public void ValidatePriceBar_HighLessThanMaxOpenClose_ReturnsError()
    {
        var bar = CreateInvalidPriceBar(high: 100m, open: 120m, close: 105m, low: 90m);

        var result = _validator.ValidatePriceBar(bar);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("High (100) must be >= max(Open=120, Close=105)"));
    }

    [Fact]
    public void ValidatePriceBar_LowGreaterThanMinOpenClose_ReturnsError()
    {
        var bar = CreateInvalidPriceBar(low: 110m, open: 100m, close: 115m, high: 120m);

        var result = _validator.ValidatePriceBar(bar);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("Low (110) must be <= min(Open=100, Close=115)"));
    }

    // ---------------------------------------------------------------------------
    // HistoricalSeries validation
    // ---------------------------------------------------------------------------

    [Fact]
    public void ValidateSeries_ValidSeries_ReturnsValid()
    {
        var bars = new List<PriceBar>
        {
            new(new DateTime(2024, 1, 1), 100m, 110m, 98m, 105m, 200_000m),
            new(new DateTime(2024, 1, 2), 105m, 112m, 103m, 109m, 180_000m),
            new(new DateTime(2024, 1, 3), 109m, 115m, 107m, 112m, 210_000m)
        };
        var series = new HistoricalSeries("MSFT", bars);

        var result = _validator.ValidateSeries(series);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    [Fact]
    public void ValidateSeries_NullSeries_ReturnsError()
    {
        var result = _validator.ValidateSeries(null!);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("null", result.Errors[0]);
    }

    [Fact]
    public void ValidateSeries_EmptyBars_ReturnsError()
    {
        var series = new HistoricalSeries("AAPL", new List<PriceBar>());

        var result = _validator.ValidateSeries(series);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("at least one bar", result.Errors[0]);
        Assert.Contains("AAPL", result.Errors[0]);
    }

    [Fact]
    public void ValidateSeries_UnsortedTimestamps_ReturnsError()
    {
        var bars = new List<PriceBar>
        {
            new(new DateTime(2024, 1, 3), 100m, 110m, 98m, 105m, 200_000m),
            new(new DateTime(2024, 1, 1), 105m, 112m, 103m, 109m, 180_000m)
        };
        var series = new HistoricalSeries("GOOG", bars);

        var result = _validator.ValidateSeries(series);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("out of order", result.Errors[0]);
        Assert.Contains("GOOG", result.Errors[0]);
        Assert.Contains("index 1", result.Errors[0]);
    }

    [Fact]
    public void ValidateSeries_DuplicateTimestamps_ReturnsError()
    {
        var bars = new List<PriceBar>
        {
            new(new DateTime(2024, 1, 1), 100m, 110m, 98m, 105m, 200_000m),
            new(new DateTime(2024, 1, 1), 105m, 112m, 103m, 109m, 180_000m)
        };
        var series = new HistoricalSeries("NVDA", bars);

        var result = _validator.ValidateSeries(series);

        Assert.False(result.IsValid);
        Assert.Single(result.Errors);
        Assert.Contains("duplicate timestamp", result.Errors[0]);
        Assert.Contains("NVDA", result.Errors[0]);
        Assert.Contains("2024-01-01", result.Errors[0]);
    }

    [Fact]
    public void ValidateSeries_NonAdjacentDuplicateTimestamps_ReturnsError()
    {
        var bars = new List<PriceBar>
        {
            new(new DateTime(2024, 1, 1), 100m, 110m, 98m, 105m, 200_000m),
            new(new DateTime(2024, 1, 3), 105m, 112m, 103m, 109m, 180_000m),
            new(new DateTime(2024, 1, 2), 106m, 113m, 104m, 108m, 170_000m),
            new(new DateTime(2024, 1, 1), 101m, 111m, 99m, 104m, 160_000m)
        };
        var series = new HistoricalSeries("NFLX", bars);

        var result = _validator.ValidateSeries(series);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("duplicate timestamp 2024-01-01"));
    }

    [Fact]
    public void ValidateSeries_NullBarEntry_ReturnsErrorWithoutThrowing()
    {
        var bars = new List<PriceBar>
        {
            new(new DateTime(2024, 1, 1), 100m, 110m, 98m, 105m, 200_000m),
            null!,
            new(new DateTime(2024, 1, 3), 105m, 112m, 103m, 109m, 180_000m)
        };
        var series = new HistoricalSeries("META", bars);

        var result = _validator.ValidateSeries(series);

        Assert.False(result.IsValid);
        Assert.Contains(result.Errors, e => e.Contains("PriceBar cannot be null."));
        Assert.Contains(result.Errors, e => e.Contains("index=1"));
    }

    [Fact]
    public void ValidateSeries_MultipleIssues_ReturnsAllErrors()
    {
        // Unsorted AND duplicate in same series
        var bars = new List<PriceBar>
        {
            new(new DateTime(2024, 1, 5), 100m, 110m, 98m, 105m, 200_000m),
            new(new DateTime(2024, 1, 3), 105m, 112m, 103m, 109m, 180_000m),
            new(new DateTime(2024, 1, 3), 109m, 115m, 107m, 112m, 210_000m)
        };
        var series = new HistoricalSeries("AMZN", bars);

        var result = _validator.ValidateSeries(series);

        Assert.False(result.IsValid);
        Assert.Equal(2, result.Errors.Count);
        Assert.Contains(result.Errors, e => e.Contains("out of order"));
        Assert.Contains(result.Errors, e => e.Contains("duplicate timestamp"));
    }

    [Fact]
    public void ValidateSeries_SingleBar_ReturnsValid()
    {
        var bars = new List<PriceBar>
        {
            new(new DateTime(2024, 6, 1), 200m, 210m, 195m, 205m, 500_000m)
        };
        var series = new HistoricalSeries("SPY", bars);

        var result = _validator.ValidateSeries(series);

        Assert.True(result.IsValid);
        Assert.Empty(result.Errors);
    }

    private static PriceBar CreateInvalidPriceBar(
        decimal open = 100m,
        decimal high = 110m,
        decimal low = 90m,
        decimal close = 105m,
        decimal volume = 1_000m,
        DateTime? date = null)
    {
        var validSeedDate = new DateTime(2024, 1, 1);
        var bar = new PriceBar(validSeedDate, 100m, 110m, 90m, 105m, 1_000m);
        var type = typeof(PriceBar);

        SetProperty(type, bar, "Date", date ?? new DateTime(2024, 1, 1));
        SetProperty(type, bar, "Open", open);
        SetProperty(type, bar, "High", high);
        SetProperty(type, bar, "Low", low);
        SetProperty(type, bar, "Close", close);
        SetProperty(type, bar, "Volume", volume);

        return bar;
    }

    private static void SetProperty(Type type, object instance, string propertyName, object value)
    {
        var property = type.GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic);
        if (property is null)
            throw new InvalidOperationException($"Property '{propertyName}' not found on type {type.Name}.");

        var setMethod = property.GetSetMethod(nonPublic: true);
        if (setMethod is null)
            throw new InvalidOperationException(
                $"Property '{propertyName}' on type {type.Name} does not have an accessible setter (including non-public).");

        setMethod.Invoke(instance, new[] { value });
    }
}
