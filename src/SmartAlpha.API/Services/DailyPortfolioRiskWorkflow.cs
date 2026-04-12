using SmartAlpha.Analytics.Analysis;
using SmartAlpha.Analytics.Validation;
using SmartAlpha.Data;
using SmartAlpha.Reporting.Formatters;

namespace SmartAlpha.API.Services;

public class DailyPortfolioRiskWorkflow
{
    private readonly string _portfolioFilePath;

    public DailyPortfolioRiskWorkflow(string portfolioFilePath)
    {
        _portfolioFilePath = portfolioFilePath;
    }

    public Task<string> RunAsync(CancellationToken cancellationToken = default)
    {
        Console.WriteLine("[DailyPortfolioRiskWorkflow] Starting Daily Portfolio Risk Workflow...");

        // Step 1: Portfolio loading
        cancellationToken.ThrowIfCancellationRequested();
        Console.WriteLine("[DailyPortfolioRiskWorkflow] Step 1: Loading portfolio...");
        var loader = new PortfolioLoader();
        var portfolio = loader.LoadFromFile(_portfolioFilePath);

        var validator = new PortfolioValidator();
        var validationResult = validator.Validate(portfolio);

        if (!validationResult.IsValid)
        {
            Console.WriteLine("[DailyPortfolioRiskWorkflow] Portfolio validation failed:");
            foreach (var error in validationResult.Errors)
            {
                Console.WriteLine($"  - {error}");
            }

            return Task.FromResult("Daily Portfolio Risk Workflow failed: portfolio validation errors.");
        }

        // Step 2: Market data retrieval
        cancellationToken.ThrowIfCancellationRequested();
        Console.WriteLine("[DailyPortfolioRiskWorkflow] Step 2: Retrieving market data...");
        // TODO: Integrate market data provider

        // Step 3: Risk analysis
        cancellationToken.ThrowIfCancellationRequested();
        Console.WriteLine("[DailyPortfolioRiskWorkflow] Step 3: Running risk analysis...");

        var nonCashHoldings = portfolio.Holdings
            .Where(h => !string.Equals(h.Ticker, "CASH", StringComparison.OrdinalIgnoreCase))
            .ToList();

        if (nonCashHoldings.Count == 0)
        {
            Console.WriteLine("[DailyPortfolioRiskWorkflow] No equity holdings found; cannot run risk analysis.");
            return Task.FromResult("Daily Portfolio Risk Workflow failed: portfolio contains no equity holdings for analysis.");
        }

        var analyzer = new PortfolioAnalyzer();
        var summary = analyzer.Analyze(portfolio);

        // Step 4: AI analysis
        cancellationToken.ThrowIfCancellationRequested();
        Console.WriteLine("[DailyPortfolioRiskWorkflow] Step 4: Running AI analysis...");
        // TODO: Integrate AI provider

        // Step 5: Report generation
        cancellationToken.ThrowIfCancellationRequested();
        Console.WriteLine("[DailyPortfolioRiskWorkflow] Step 5: Generating report...");
        var formatter = new PortfolioSummaryFormatter();
        var report = formatter.Format(summary);

        Console.WriteLine(report);

        Console.WriteLine("[DailyPortfolioRiskWorkflow] Workflow completed (market data and AI analysis pending).");

        return Task.FromResult("Daily Portfolio Risk Workflow completed (partial: market data and AI analysis not yet implemented).");
    }
}
