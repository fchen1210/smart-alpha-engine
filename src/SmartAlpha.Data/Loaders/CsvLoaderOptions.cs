using SmartAlpha.Core.Models.Market;

namespace SmartAlpha.Data.Loaders;

/// <summary>
/// Configuration options for <see cref="MarketDataCsvLoader"/>.
/// </summary>
public class CsvLoaderOptions
{
    /// <summary>
    /// The ticker symbol to assign to the loaded <see cref="HistoricalSeries"/>.
    /// </summary>
    public string Symbol { get; init; } = string.Empty;

    /// <summary>
    /// The asset type of the instrument (e.g. Stock, ETF).
    /// </summary>
    public AssetType AssetType { get; init; } = AssetType.Stock;

    /// <summary>
    /// Date/time format string used to parse the Date column.
    /// Defaults to <c>yyyy-MM-dd</c>.
    /// </summary>
    public string DateFormat { get; init; } = "yyyy-MM-dd";
}
