namespace SmartAlpha.Core.Models.Market;

public class HistoricalSeries
{
    public string Symbol { get; init; } = string.Empty;

    public IReadOnlyList<PriceBar> Bars { get; init; } = [];

    public HistoricalSeries() { }

    public HistoricalSeries(string symbol, IReadOnlyList<PriceBar> bars)
    {
        Symbol = symbol;
        Bars = bars.ToList().AsReadOnly();
    }
}
