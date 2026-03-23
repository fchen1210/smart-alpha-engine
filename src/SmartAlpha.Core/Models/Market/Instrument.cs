namespace SmartAlpha.Core.Models.Market;

public class Instrument
{
    public string Symbol { get; init; } = string.Empty;

    public string Name { get; init; } = string.Empty;

    public AssetType AssetType { get; init; }

    public string Currency { get; init; } = string.Empty;

    public string Exchange { get; init; } = string.Empty;

    public Instrument() { }

    public Instrument(string symbol, string name, AssetType assetType, string currency, string exchange)
    {
        if (string.IsNullOrWhiteSpace(symbol)) throw new ArgumentException("Symbol is required.", nameof(symbol));
        if (string.IsNullOrWhiteSpace(name)) throw new ArgumentException("Name is required.", nameof(name));
        if (string.IsNullOrWhiteSpace(currency)) throw new ArgumentException("Currency is required.", nameof(currency));
        if (string.IsNullOrWhiteSpace(exchange)) throw new ArgumentException("Exchange is required.", nameof(exchange));

        Symbol = symbol;
        Name = name;
        AssetType = assetType;
        Currency = currency;
        Exchange = exchange;
    }
}
