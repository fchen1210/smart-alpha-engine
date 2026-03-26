using System.Globalization;
using SmartAlpha.Core.Models.Market;

namespace SmartAlpha.Data.Loaders;

/// <summary>
/// Loads historical market price data from a CSV file into a <see cref="HistoricalSeries"/>.
///
/// <para><b>Supported CSV schema</b> (header row required, case-insensitive):</para>
/// <list type="table">
///   <listheader><term>Column</term><description>Required?</description></listheader>
///   <item><term>Date</term><description>Required – parsed with the configured date format (default <c>yyyy-MM-dd</c>).</description></item>
///   <item><term>Open</term><description>Required – decimal opening price.</description></item>
///   <item><term>High</term><description>Required – decimal high price.</description></item>
///   <item><term>Low</term><description>Required – decimal low price.</description></item>
///   <item><term>Close</term><description>Required – decimal closing price.</description></item>
///   <item><term>Volume</term><description>Optional – decimal volume; defaults to 0 when absent.</description></item>
/// </list>
///
/// <para>Bars are returned in ascending date order regardless of input order.</para>
/// <para><b>Limitation:</b> field values that contain commas must not be present (RFC 4180 quoting
/// is not supported in this MVP version).</para>
/// </summary>
public class MarketDataCsvLoader : IMarketDataCsvLoader
{
    private static readonly string[] RequiredColumns = ["Date", "Open", "High", "Low", "Close"];

    /// <inheritdoc/>
    public HistoricalSeries Load(string filePath, CsvLoaderOptions options)
    {
        ArgumentNullException.ThrowIfNull(options);

        if (string.IsNullOrWhiteSpace(filePath))
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));

        if (!File.Exists(filePath))
            throw new FileNotFoundException($"Market data CSV file not found: {filePath}", filePath);

        if (string.IsNullOrWhiteSpace(options.Symbol))
            throw new ArgumentException("Symbol must be provided in CsvLoaderOptions.", nameof(options));

        if (string.IsNullOrWhiteSpace(options.DateFormat))
            throw new ArgumentException("DateFormat must be a non-empty string in CsvLoaderOptions.", nameof(options));

        // Stream lines to avoid loading the entire file into memory at once.
        using var enumerator = File.ReadLines(filePath).GetEnumerator();

        if (!enumerator.MoveNext())
            throw new InvalidOperationException("CSV file is empty.");

        // --- Parse header (line 1) ---
        var headers = enumerator.Current.Split(',');
        var columnIndex = BuildColumnIndex(headers);
        ValidateRequiredColumns(columnIndex);

        // --- Parse data rows (lines 2+) ---
        var bars = new List<PriceBar>();
        var humanLineNumber = 2;

        while (enumerator.MoveNext())
        {
            var line = enumerator.Current;

            // Skip blank lines
            if (!string.IsNullOrWhiteSpace(line))
            {
                var fields = line.Split(',');
                var bar = ParseRow(fields, columnIndex, options, humanLineNumber);
                bars.Add(bar);
            }

            humanLineNumber++;
        }

        bars.Sort((a, b) => a.Date.CompareTo(b.Date));

        return new HistoricalSeries(options.Symbol, bars);
    }

    // -------------------------------------------------------------------------
    // Helpers
    // -------------------------------------------------------------------------

    private static Dictionary<string, int> BuildColumnIndex(string[] headers)
    {
        var index = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        for (var i = 0; i < headers.Length; i++)
        {
            var name = headers[i].Trim();
            if (!string.IsNullOrEmpty(name))
                index[name] = i;
        }
        return index;
    }

    private static void ValidateRequiredColumns(Dictionary<string, int> columnIndex)
    {
        var missing = RequiredColumns
            .Where(col => !columnIndex.ContainsKey(col))
            .ToList();

        if (missing.Count > 0)
            throw new InvalidOperationException(
                $"CSV is missing required column(s): {string.Join(", ", missing)}.");
    }

    private static PriceBar ParseRow(
        string[] fields,
        Dictionary<string, int> columnIndex,
        CsvLoaderOptions options,
        int humanLineNumber)
    {
        var date = ParseDate(fields, columnIndex, options.DateFormat, humanLineNumber);
        var open = ParseDecimal(fields, columnIndex, "Open", humanLineNumber);
        var high = ParseDecimal(fields, columnIndex, "High", humanLineNumber);
        var low = ParseDecimal(fields, columnIndex, "Low", humanLineNumber);
        var close = ParseDecimal(fields, columnIndex, "Close", humanLineNumber);
        var volume = columnIndex.ContainsKey("Volume")
            ? ParseDecimal(fields, columnIndex, "Volume", humanLineNumber)
            : 0m;

        try
        {
            return new PriceBar(date, open, high, low, close, volume);
        }
        catch (ArgumentException ex)
        {
            throw new FormatException(
                $"Line {humanLineNumber}: invalid price bar – {ex.Message}", ex);
        }
    }

    private static DateTime ParseDate(
        string[] fields,
        Dictionary<string, int> columnIndex,
        string dateFormat,
        int humanLineNumber)
    {
        var raw = GetField(fields, columnIndex, "Date", humanLineNumber);

        if (!DateTime.TryParseExact(raw, dateFormat,
                CultureInfo.InvariantCulture, DateTimeStyles.None, out var result))
        {
            throw new FormatException(
                $"Line {humanLineNumber}: cannot parse Date value '{raw}' with format '{dateFormat}'.");
        }

        return result;
    }

    private static decimal ParseDecimal(
        string[] fields,
        Dictionary<string, int> columnIndex,
        string column,
        int humanLineNumber)
    {
        var raw = GetField(fields, columnIndex, column, humanLineNumber);

        if (!decimal.TryParse(raw, NumberStyles.Any, CultureInfo.InvariantCulture, out var result))
        {
            throw new FormatException(
                $"Line {humanLineNumber}: cannot parse {column} value '{raw}' as a decimal number.");
        }

        return result;
    }

    private static string GetField(
        string[] fields,
        Dictionary<string, int> columnIndex,
        string column,
        int humanLineNumber)
    {
        var idx = columnIndex[column];
        if (idx >= fields.Length)
            throw new FormatException(
                $"Line {humanLineNumber}: column '{column}' is missing (row has fewer fields than header).");

        return fields[idx].Trim();
    }
}
