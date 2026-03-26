using SmartAlpha.Core.Models;
using SmartAlpha.Core.Models.Market;

namespace SmartAlpha.Analytics.Validation;

public class MarketDataValidator
{
    public ValidationResult ValidateInstrument(Instrument instrument)
    {
        var result = new ValidationResult();

        if (instrument is null)
        {
            result.Errors.Add("Instrument cannot be null.");
            result.IsValid = false;
            return result;
        }

        if (string.IsNullOrWhiteSpace(instrument.Symbol))
            result.Errors.Add("Instrument: Symbol is required and cannot be empty.");

        if (!Enum.IsDefined(typeof(AssetType), instrument.AssetType))
            result.Errors.Add($"Instrument '{instrument.Symbol}': AssetType '{(int)instrument.AssetType}' is not a valid value.");

        result.IsValid = result.Errors.Count == 0;
        return result;
    }

    public ValidationResult ValidatePriceBar(PriceBar bar, int? index = null, string? symbol = null)
    {
        var result = new ValidationResult();

        if (bar is null)
        {
            result.Errors.Add(FormatBarContext(symbol, index) + "PriceBar cannot be null.");
            result.IsValid = false;
            return result;
        }

        var context = FormatBarContext(symbol, index, bar.Date);

        if (bar.Open < 0)
            result.Errors.Add($"{context}Open must be non-negative (got {bar.Open}).");
        if (bar.High < 0)
            result.Errors.Add($"{context}High must be non-negative (got {bar.High}).");
        if (bar.Low < 0)
            result.Errors.Add($"{context}Low must be non-negative (got {bar.Low}).");
        if (bar.Close < 0)
            result.Errors.Add($"{context}Close must be non-negative (got {bar.Close}).");
        if (bar.Volume < 0)
            result.Errors.Add($"{context}Volume must be non-negative (got {bar.Volume}).");
        if (bar.High < bar.Low)
            result.Errors.Add($"{context}High ({bar.High}) must be >= Low ({bar.Low}).");
        if (bar.High < bar.Open || bar.High < bar.Close)
            result.Errors.Add($"{context}High ({bar.High}) must be >= max(Open={bar.Open}, Close={bar.Close}).");
        if (bar.Low > bar.Open || bar.Low > bar.Close)
            result.Errors.Add($"{context}Low ({bar.Low}) must be <= min(Open={bar.Open}, Close={bar.Close}).");

        result.IsValid = result.Errors.Count == 0;
        return result;
    }

    public ValidationResult ValidateSeries(HistoricalSeries series)
    {
        var result = new ValidationResult();

        if (series is null)
        {
            result.Errors.Add("HistoricalSeries cannot be null.");
            result.IsValid = false;
            return result;
        }

        var symbol = series.Symbol;

        if (string.IsNullOrWhiteSpace(symbol))
            result.Errors.Add("Series symbol is required and cannot be empty.");

        if (series.Bars is null || series.Bars.Count == 0)
        {
            result.Errors.Add($"Series '{symbol}': must contain at least one bar.");
            result.IsValid = result.Errors.Count == 0;
            return result;
        }

        var seenDateIndices = new Dictionary<DateTime, int>();
        DateTime? previousDate = null;

        for (int i = 0; i < series.Bars.Count; i++)
        {
            var bar = series.Bars[i];
            var barResult = ValidatePriceBar(bar, i, symbol);
            result.Errors.AddRange(barResult.Errors);

            if (bar is null)
                continue;

            if (seenDateIndices.TryGetValue(bar.Date, out var firstIndex))
                result.Errors.Add(
                    $"Series '{symbol}': duplicate timestamp {bar.Date:yyyy-MM-dd} at indices {firstIndex} and {i}.");
            else
                seenDateIndices[bar.Date] = i;

            if (previousDate.HasValue && bar.Date < previousDate.Value)
                result.Errors.Add(
                    $"Series '{symbol}': bar at index {i} ({bar.Date:yyyy-MM-dd}) is out of order after {previousDate.Value:yyyy-MM-dd}.");

            previousDate = bar.Date;
        }

        result.IsValid = result.Errors.Count == 0;
        return result;
    }

    private static string FormatBarContext(string? symbol, int? index, DateTime? date = null)
    {
        var parts = new List<string>(3);
        if (!string.IsNullOrWhiteSpace(symbol))
            parts.Add($"symbol='{symbol}'");
        if (index.HasValue)
            parts.Add($"index={index}");
        if (date.HasValue)
            parts.Add($"date={date.Value:yyyy-MM-dd}");
        return parts.Count > 0 ? $"[{string.Join(", ", parts)}] " : string.Empty;
    }
}
