namespace SmartAlpha.Core.Models;

public class PortfolioSummary
{
    public decimal EquityExposure { get; set; }

    public decimal CashAllocation { get; set; }

    public decimal TechExposure { get; set; }

    public string TechConcentration { get; set; } = string.Empty;

    public string TopHolding { get; set; } = string.Empty;

    public decimal TopHoldingWeight { get; set; }

    public string SingleStockConcentration { get; set; } = string.Empty;

    public string RiskNote { get; set; } = string.Empty;
}