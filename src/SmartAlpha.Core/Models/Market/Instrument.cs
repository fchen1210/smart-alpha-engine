namespace SmartAlpha.Core.Models.Market;

public class Instrument
{
    private string _symbol = string.Empty;
    private string _name = string.Empty;
    private string _currency = string.Empty;
    private string _exchange = string.Empty;

    public string Symbol
    {
        get => _symbol;
        init
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Symbol is required.", nameof(Symbol));
            _symbol = value;
        }
    }

    public string Name
    {
        get => _name;
        init
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Name is required.", nameof(Name));
            _name = value;
        }
    }

    public AssetType AssetType { get; init; }

    public string Currency
    {
        get => _currency;
        init
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Currency is required.", nameof(Currency));
            _currency = value;
        }
    }

    public string Exchange
    {
        get => _exchange;
        init
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Exchange is required.", nameof(Exchange));
            _exchange = value;
        }
    }

    public Instrument() { }

    public Instrument(string symbol, string name, AssetType assetType, string currency, string exchange)
    {
        Symbol = symbol;
        Name = name;
        AssetType = assetType;
        Currency = currency;
        Exchange = exchange;
    }
}
