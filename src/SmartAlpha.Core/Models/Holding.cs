namespace SmartAlpha.Core.Models;

public class Holding
{
    public string Ticker { get; set; } = string.Empty;

    public decimal Weight { get; set; }

    public string AssetClass { get; set; } = string.Empty;
}