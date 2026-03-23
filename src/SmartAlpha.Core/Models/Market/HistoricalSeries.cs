namespace SmartAlpha.Core.Models.Market;

public class HistoricalSeries
{
    private string _symbol = string.Empty;
    private IReadOnlyList<PriceBar> _bars = [];

    public string Symbol
    {
        get => _symbol;
        init
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Symbol cannot be null or whitespace.", nameof(Symbol));
            _symbol = value;
        }
    }

    public IReadOnlyList<PriceBar> Bars
    {
        get => _bars;
        init => _bars = (value ?? Array.Empty<PriceBar>()).ToList().AsReadOnly();
    }

    public HistoricalSeries() { }

    public HistoricalSeries(string symbol, IReadOnlyList<PriceBar> bars)
    {
        if (string.IsNullOrWhiteSpace(symbol))
            throw new ArgumentException("Symbol cannot be null or whitespace.", nameof(symbol));

        if (bars is null)
            throw new ArgumentNullException(nameof(bars));

        Symbol = symbol;
        Bars = bars;
    }
}
