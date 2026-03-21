using SmartAlpha.Core.Models;
using System.Text;

namespace SmartAlpha.Reporting.Formatters;

public class PortfolioSummaryFormatter
{
    public string Format(PortfolioSummary summary)
    {
        var sb = new StringBuilder();

        sb.AppendLine("Portfolio Summary");
        sb.AppendLine($"- Equity exposure: {summary.EquityExposure:P2}");
        sb.AppendLine($"- Cash allocation: {summary.CashAllocation:P2}");
        sb.AppendLine($"- Tech concentration: {summary.TechConcentration}");
        sb.AppendLine($"- Single stock concentration: {summary.SingleStockConcentration}");
        sb.AppendLine();
        sb.AppendLine("Risk Note");
        sb.AppendLine(summary.RiskNote);

        return sb.ToString();
    }
}