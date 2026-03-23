using SmartAlpha.Core.Models.Market;

namespace SmartAlpha.Tests;

public class MarketModelsTests
{
    [Fact]
    public void Instrument_Constructor_SetsAllProperties()
    {
        var instrument = new Instrument("AAPL", "Apple Inc.", AssetType.Stock, "USD", "NASDAQ");

        Assert.Equal("AAPL", instrument.Symbol);
        Assert.Equal("Apple Inc.", instrument.Name);
        Assert.Equal(AssetType.Stock, instrument.AssetType);
        Assert.Equal("USD", instrument.Currency);
        Assert.Equal("NASDAQ", instrument.Exchange);
    }

    [Fact]
    public void Instrument_InitProperties_SetsAllProperties()
    {
        var instrument = new Instrument
        {
            Symbol = "BTC",
            Name = "Bitcoin",
            AssetType = AssetType.Crypto,
            Currency = "USD",
            Exchange = "Binance"
        };

        Assert.Equal("BTC", instrument.Symbol);
        Assert.Equal("Bitcoin", instrument.Name);
        Assert.Equal(AssetType.Crypto, instrument.AssetType);
        Assert.Equal("USD", instrument.Currency);
        Assert.Equal("Binance", instrument.Exchange);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Instrument_Constructor_ThrowsWhenSymbolIsEmpty(string symbol)
    {
        Assert.Throws<ArgumentException>(() =>
            new Instrument(symbol, "Apple Inc.", AssetType.Stock, "USD", "NASDAQ"));
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void Instrument_InitSetter_ThrowsWhenSymbolIsEmpty(string symbol)
    {
        Assert.Throws<ArgumentException>(() => new Instrument { Symbol = symbol });
    }

    [Fact]
    public void PriceBar_Constructor_SetsAllProperties()
    {
        var date = new DateTime(2024, 1, 15);
        var bar = new PriceBar(date, 150.00m, 155.00m, 149.00m, 153.00m, 1_000_000m);

        Assert.Equal(date, bar.Date);
        Assert.Equal(150.00m, bar.Open);
        Assert.Equal(155.00m, bar.High);
        Assert.Equal(149.00m, bar.Low);
        Assert.Equal(153.00m, bar.Close);
        Assert.Equal(1_000_000m, bar.Volume);
    }

    [Fact]
    public void PriceBar_Constructor_ThrowsWhenHighLessThanLow()
    {
        Assert.Throws<ArgumentException>(() =>
            new PriceBar(DateTime.Today, 100m, 90m, 95m, 100m, 1000m));
    }

    [Fact]
    public void PriceBar_Constructor_ThrowsWhenOpenOutsideHighLowRange()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new PriceBar(DateTime.Today, 120m, 110m, 100m, 105m, 1000m));
    }

    [Fact]
    public void PriceBar_Constructor_ThrowsWhenNegativeVolume()
    {
        Assert.Throws<ArgumentOutOfRangeException>(() =>
            new PriceBar(DateTime.Today, 105m, 110m, 100m, 105m, -1m));
    }

    [Fact]
    public void HistoricalSeries_Constructor_SetsSymbolAndBars()
    {
        var bars = new List<PriceBar>
        {
            new(new DateTime(2024, 1, 1), 100m, 110m, 98m, 105m, 200_000m),
            new(new DateTime(2024, 1, 2), 105m, 112m, 103m, 109m, 180_000m)
        };

        var series = new HistoricalSeries("MSFT", bars);

        Assert.Equal("MSFT", series.Symbol);
        Assert.Equal(2, series.Bars.Count);
        Assert.Equal(100m, series.Bars[0].Open);
        Assert.Equal(109m, series.Bars[1].Close);
    }

    [Fact]
    public void HistoricalSeries_DefaultConstructor_HasEmptyBars()
    {
        var series = new HistoricalSeries();

        Assert.Equal(string.Empty, series.Symbol);
        Assert.Empty(series.Bars);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void HistoricalSeries_Constructor_ThrowsWhenSymbolIsEmpty(string symbol)
    {
        var bars = new List<PriceBar>();
        Assert.Throws<ArgumentException>(() => new HistoricalSeries(symbol, bars));
    }

    [Fact]
    public void HistoricalSeries_Constructor_ThrowsWhenBarsIsNull()
    {
        Assert.Throws<ArgumentNullException>(() => new HistoricalSeries("AAPL", null!));
    }

    [Fact]
    public void HistoricalSeries_Constructor_IsMutationSafe()
    {
        var mutableList = new List<PriceBar>
        {
            new(new DateTime(2024, 1, 1), 100m, 110m, 98m, 105m, 200_000m)
        };

        var series = new HistoricalSeries("AAPL", mutableList);
        mutableList.Add(new PriceBar(new DateTime(2024, 1, 2), 105m, 112m, 103m, 109m, 180_000m));

        Assert.Single(series.Bars);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    public void HistoricalSeries_InitSetter_ThrowsWhenSymbolIsEmpty(string symbol)
    {
        Assert.Throws<ArgumentException>(() => new HistoricalSeries { Symbol = symbol });
    }

    [Fact]
    public void HistoricalSeries_InitSetter_IsMutationSafe()
    {
        var mutableList = new List<PriceBar>
        {
            new(new DateTime(2024, 1, 1), 100m, 110m, 98m, 105m, 200_000m)
        };

        var series = new HistoricalSeries { Symbol = "GOOG", Bars = mutableList };
        mutableList.Add(new PriceBar(new DateTime(2024, 1, 2), 105m, 112m, 103m, 109m, 180_000m));

        Assert.Single(series.Bars);
    }

    [Theory]
    [InlineData(AssetType.Stock)]
    [InlineData(AssetType.ETF)]
    [InlineData(AssetType.Bond)]
    [InlineData(AssetType.Crypto)]
    [InlineData(AssetType.Commodity)]
    [InlineData(AssetType.Forex)]
    public void AssetType_AllValuesAreDefined(AssetType assetType)
    {
        Assert.True(Enum.IsDefined(typeof(AssetType), assetType));
    }
}

