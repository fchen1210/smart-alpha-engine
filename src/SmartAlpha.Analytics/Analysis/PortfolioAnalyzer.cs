using SmartAlpha.Core.Models;

namespace SmartAlpha.Analytics.Analysis;

public class PortfolioAnalyzer
{
    private static readonly HashSet<string> TechTickers = new(StringComparer.OrdinalIgnoreCase)
    {
        "QQQ", "NVDA", "MSFT", "AAPL", "GOOG", "AMZN", "META",
        "AMD", "ASML", "TSM", "INTU", "FTNT", "LRCX", "U",
        "PYPL", "NFLX", "TTWO", "TSLA"
    };

    public PortfolioSummary Analyze(Portfolio portfolio)
    {
        var equityHoldings = portfolio.Holdings
            .Where(h => !string.Equals(h.Ticker, "CASH", StringComparison.OrdinalIgnoreCase))
            .ToList();

        var cashAllocation = portfolio.Holdings
            .Where(h => string.Equals(h.Ticker, "CASH", StringComparison.OrdinalIgnoreCase))
            .Sum(h => h.Weight);

        var equityExposure = equityHoldings.Sum(h => h.Weight);

        var techExposure = equityHoldings
            .Where(h => TechTickers.Contains(h.Ticker))
            .Sum(h => h.Weight);

        var topHolding = equityHoldings
            .OrderByDescending(h => h.Weight)
            .First();

        var techConcentration = ClassifyTechConcentration(techExposure);
        var singleStockConcentration = ClassifySingleStockConcentration(topHolding.Weight);
        var riskNote = BuildRiskNote(techConcentration, singleStockConcentration);

        return new PortfolioSummary
        {
            EquityExposure = equityExposure,
            CashAllocation = cashAllocation,
            TechExposure = techExposure,
            TechConcentration = techConcentration,
            TopHolding = topHolding.Ticker,
            TopHoldingWeight = topHolding.Weight,
            SingleStockConcentration = singleStockConcentration,
            RiskNote = riskNote
        };
    }

    private static string ClassifyTechConcentration(decimal techExposure)
    {
        if (techExposure < 0.25m)
            return "Low";

        if (techExposure <= 0.50m)
            return "Moderate";

        return "High";
    }

    private static string ClassifySingleStockConcentration(decimal topHoldingWeight)
    {
        if (topHoldingWeight < 0.10m)
            return "Low";

        if (topHoldingWeight <= 0.20m)
            return "Moderate";

        return "High";
    }

    private static string BuildRiskNote(string techConcentration, string singleStockConcentration)
    {
        if (techConcentration == "High" && singleStockConcentration == "High")
        {
            return "Portfolio is highly concentrated in technology and oversized single-stock exposure.";
        }

        if (techConcentration == "High" && singleStockConcentration == "Moderate")
        {
            return "Portfolio is moderately concentrated in growth and technology exposure.";
        }

        if (techConcentration == "Moderate" && singleStockConcentration == "High")
        {
            return "Portfolio has a moderate technology tilt but meaningful single-stock concentration risk.";
        }

        if (techConcentration == "Low" && singleStockConcentration == "Low")
        {
            return "Portfolio appears relatively diversified under this simplified risk model.";
        }

        return "Portfolio has some concentration risk under this simplified model.";
    }
}