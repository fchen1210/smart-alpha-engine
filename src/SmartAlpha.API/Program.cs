using SmartAlpha.Analytics.Analysis;
using SmartAlpha.Data;
using SmartAlpha.Analytics.Validation;

var filePath = Path.GetFullPath(Path.Combine(
    Directory.GetCurrentDirectory(),
    "..",
    "..",
    "data",
    "portfolio.sample.json"));

Console.WriteLine($"Loading portfolio from: {filePath}");

var loader = new PortfolioLoader();
var portfolio = loader.LoadFromFile(filePath);

var validator = new PortfolioValidator();
var validationResult = validator.Validate(portfolio);

if (!validationResult.IsValid)
{
    Console.WriteLine("Portfolio validation failed:");

    foreach (var error in validationResult.Errors)
    {
        Console.WriteLine($"- {error}");
    }

    return;
}

var analyzer = new PortfolioAnalyzer();
var summary = analyzer.Analyze(portfolio);

Console.WriteLine();
Console.WriteLine("Portfolio Summary");
Console.WriteLine($"- Equity exposure: {summary.EquityExposure:P2}");
Console.WriteLine($"- Cash allocation: {summary.CashAllocation:P2}");
Console.WriteLine($"- Tech concentration: {summary.TechConcentration}");
Console.WriteLine($"- Single stock concentration: {summary.SingleStockConcentration}");
Console.WriteLine();
Console.WriteLine("Risk Note");
Console.WriteLine(summary.RiskNote);