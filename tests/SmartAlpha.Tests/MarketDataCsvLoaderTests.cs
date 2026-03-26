using SmartAlpha.Core.Models.Market;
using SmartAlpha.Data.Loaders;

namespace SmartAlpha.Tests;

public class MarketDataCsvLoaderTests
{
    private static readonly string TestDataDir =
        Path.Combine(AppContext.BaseDirectory, "TestData");

    private static string CsvPath(string fileName) =>
        Path.Combine(TestDataDir, fileName);

    private static CsvLoaderOptions DefaultOptions(string symbol = "AAPL") =>
        new() { Symbol = symbol, AssetType = AssetType.Stock };

    // -------------------------------------------------------------------------
    // Happy path
    // -------------------------------------------------------------------------

    [Fact]
    public void Load_ValidCsvWithVolume_ReturnsPopulatedHistoricalSeries()
    {
        var loader = new MarketDataCsvLoader();
        var series = loader.Load(CsvPath("aapl_2024.csv"), DefaultOptions());

        Assert.Equal("AAPL", series.Symbol);
        Assert.Equal(5, series.Bars.Count);

        var first = series.Bars[0];
        Assert.Equal(new DateTime(2024, 1, 2), first.Date);
        Assert.Equal(148.00m, first.Open);
        Assert.Equal(152.50m, first.High);
        Assert.Equal(147.00m, first.Low);
        Assert.Equal(151.00m, first.Close);
        Assert.Equal(75_000_000m, first.Volume);
    }

    [Fact]
    public void Load_ValidCsvWithoutVolume_DefaultsVolumeToZero()
    {
        var loader = new MarketDataCsvLoader();
        var series = loader.Load(CsvPath("no_volume.csv"), DefaultOptions());

        Assert.Equal(2, series.Bars.Count);
        Assert.All(series.Bars, bar => Assert.Equal(0m, bar.Volume));
    }

    [Fact]
    public void Load_UnsortedCsv_ReturnsBarsInAscendingDateOrder()
    {
        var loader = new MarketDataCsvLoader();
        var series = loader.Load(CsvPath("unsorted.csv"), DefaultOptions());

        Assert.Equal(3, series.Bars.Count);
        Assert.Equal(new DateTime(2024, 1, 2), series.Bars[0].Date);
        Assert.Equal(new DateTime(2024, 1, 3), series.Bars[1].Date);
        Assert.Equal(new DateTime(2024, 1, 5), series.Bars[2].Date);
    }

    [Fact]
    public void Load_CustomDateFormat_ParsesSuccessfully()
    {
        // Write a temp CSV with MM/dd/yyyy dates
        var csvContent = """
            Date,Open,High,Low,Close
            01/02/2024,148.00,152.50,147.00,151.00
            01/03/2024,151.00,153.00,149.50,150.25
            """;
        var tmpFile = Path.GetTempFileName();
        try
        {
            File.WriteAllText(tmpFile, csvContent);
            var options = new CsvLoaderOptions
            {
                Symbol = "TEST",
                AssetType = AssetType.ETF,
                DateFormat = "MM/dd/yyyy"
            };
            var loader = new MarketDataCsvLoader();
            var series = loader.Load(tmpFile, options);

            Assert.Equal(2, series.Bars.Count);
            Assert.Equal(new DateTime(2024, 1, 2), series.Bars[0].Date);
        }
        finally
        {
            File.Delete(tmpFile);
        }
    }

    // -------------------------------------------------------------------------
    // Error: file not found / unreadable
    // -------------------------------------------------------------------------

    [Fact]
    public void Load_FileNotFound_ThrowsFileNotFoundException()
    {
        var loader = new MarketDataCsvLoader();
        var ex = Assert.Throws<FileNotFoundException>(() =>
            loader.Load("/nonexistent/path/data.csv", DefaultOptions()));

        Assert.Contains("data.csv", ex.Message);
    }

    [Fact]
    public void Load_NullFilePath_ThrowsArgumentException()
    {
        var loader = new MarketDataCsvLoader();
        Assert.Throws<ArgumentException>(() =>
            loader.Load(null!, DefaultOptions()));
    }

    [Fact]
    public void Load_EmptyFilePath_ThrowsArgumentException()
    {
        var loader = new MarketDataCsvLoader();
        Assert.Throws<ArgumentException>(() =>
            loader.Load("   ", DefaultOptions()));
    }

    // -------------------------------------------------------------------------
    // Error: missing required columns
    // -------------------------------------------------------------------------

    [Fact]
    public void Load_MissingRequiredColumn_ThrowsWithMissingColumnName()
    {
        var loader = new MarketDataCsvLoader();
        var ex = Assert.Throws<InvalidOperationException>(() =>
            loader.Load(CsvPath("missing_columns.csv"), DefaultOptions()));

        Assert.Contains("Low", ex.Message);
    }

    // -------------------------------------------------------------------------
    // Error: parse failures
    // -------------------------------------------------------------------------

    [Fact]
    public void Load_UnparseableNumeric_ThrowsFormatExceptionWithLineInfo()
    {
        var loader = new MarketDataCsvLoader();
        var ex = Assert.Throws<FormatException>(() =>
            loader.Load(CsvPath("bad_numeric.csv"), DefaultOptions()));

        Assert.Contains("Line 2", ex.Message);
        Assert.Contains("Open", ex.Message);
    }

    [Fact]
    public void Load_UnparseableDate_ThrowsFormatExceptionWithLineInfo()
    {
        var loader = new MarketDataCsvLoader();
        var ex = Assert.Throws<FormatException>(() =>
            loader.Load(CsvPath("bad_date.csv"), DefaultOptions()));

        Assert.Contains("Line 2", ex.Message);
        Assert.Contains("Date", ex.Message);
    }

    // -------------------------------------------------------------------------
    // Error: missing symbol in options
    // -------------------------------------------------------------------------

    [Fact]
    public void Load_EmptySymbolInOptions_ThrowsArgumentException()
    {
        var loader = new MarketDataCsvLoader();
        var options = new CsvLoaderOptions { Symbol = "" };
        Assert.Throws<ArgumentException>(() =>
            loader.Load(CsvPath("aapl_2024.csv"), options));
    }

    // -------------------------------------------------------------------------
    // Interoperability: output is a valid HistoricalSeries
    // -------------------------------------------------------------------------

    [Fact]
    public void Load_Result_IsCompatibleWithHistoricalSeriesModel()
    {
        var loader = new MarketDataCsvLoader();
        var series = loader.Load(CsvPath("aapl_2024.csv"), DefaultOptions());

        // HistoricalSeries requires a non-empty symbol and read-only bars
        Assert.False(string.IsNullOrWhiteSpace(series.Symbol));
        Assert.NotEmpty(series.Bars);
        Assert.IsAssignableFrom<IReadOnlyList<PriceBar>>(series.Bars);
    }
}
