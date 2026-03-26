using SmartAlpha.Core.Models.Market;

namespace SmartAlpha.Data.Loaders;

/// <summary>
/// Loads historical market price data from a CSV file into a <see cref="HistoricalSeries"/>.
/// </summary>
public interface IMarketDataCsvLoader
{
    /// <summary>
    /// Reads the CSV file at <paramref name="filePath"/> and returns a populated
    /// <see cref="HistoricalSeries"/> sorted in ascending date order.
    /// </summary>
    /// <param name="filePath">Path to the CSV file.</param>
    /// <param name="options">Loader configuration (symbol, asset type, date format).</param>
    /// <returns>A <see cref="HistoricalSeries"/> with the parsed <see cref="PriceBar"/> entries.</returns>
    /// <exception cref="FileNotFoundException">The file does not exist or cannot be read.</exception>
    /// <exception cref="InvalidOperationException">Required columns are missing.</exception>
    /// <exception cref="FormatException">A row contains an unparseable date or numeric value.</exception>
    HistoricalSeries Load(string filePath, CsvLoaderOptions options);
}
