using SmartAlpha.API.Services;

namespace SmartAlpha.Tests;

public class DailyPortfolioRiskWorkflowTests
{
    private static string ValidPortfolioPath =>
        Path.Combine(
            Path.GetDirectoryName(typeof(DailyPortfolioRiskWorkflowTests).Assembly.Location)!,
            "TestData",
            "portfolio.sample.json");

    [Fact]
    public async Task RunAsync_WithValidPortfolio_ReturnsSuccessResult()
    {
        var workflow = new DailyPortfolioRiskWorkflow(ValidPortfolioPath);

        var result = await workflow.RunAsync();

        Assert.Equal("Daily Portfolio Risk Workflow completed (partial: market data and AI analysis not yet implemented).", result);
    }

    [Fact]
    public async Task RunAsync_WithMissingPortfolioFile_ThrowsFileNotFoundException()
    {
        var workflow = new DailyPortfolioRiskWorkflow("nonexistent/path/portfolio.json");

        await Assert.ThrowsAsync<FileNotFoundException>(() => workflow.RunAsync());
    }

    [Fact]
    public async Task RunAsync_WithInvalidPortfolio_ReturnsFailureResult()
    {
        var emptyPortfolioPath = Path.Combine(Path.GetTempPath(), $"empty_portfolio_{Guid.NewGuid()}.json");
        await File.WriteAllTextAsync(emptyPortfolioPath, """{ "holdings": [] }""");

        try
        {
            var workflow = new DailyPortfolioRiskWorkflow(emptyPortfolioPath);

            var result = await workflow.RunAsync();

            Assert.Contains("failed", result, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            File.Delete(emptyPortfolioPath);
        }
    }

    [Fact]
    public async Task RunAsync_WithCashOnlyPortfolio_ReturnsFailureResult()
    {
        var cashOnlyPath = Path.Combine(Path.GetTempPath(), $"cash_only_{Guid.NewGuid()}.json");
        await File.WriteAllTextAsync(cashOnlyPath, """{ "holdings": [{ "ticker": "CASH", "weight": 1.0, "assetClass": "Cash" }] }""");

        try
        {
            var workflow = new DailyPortfolioRiskWorkflow(cashOnlyPath);

            var result = await workflow.RunAsync();

            Assert.Contains("failed", result, StringComparison.OrdinalIgnoreCase);
            Assert.Contains("no equity holdings", result, StringComparison.OrdinalIgnoreCase);
        }
        finally
        {
            File.Delete(cashOnlyPath);
        }
    }

    [Fact]
    public async Task RunAsync_WhenCancelled_ThrowsOperationCanceledException()
    {
        using var cts = new CancellationTokenSource();
        cts.Cancel();

        var workflow = new DailyPortfolioRiskWorkflow(ValidPortfolioPath);

        await Assert.ThrowsAsync<OperationCanceledException>(() => workflow.RunAsync(cts.Token));
    }
}
