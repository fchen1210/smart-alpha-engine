using SmartAlpha.Analytics.Validation;
using SmartAlpha.Core.Models.Market;

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
}
