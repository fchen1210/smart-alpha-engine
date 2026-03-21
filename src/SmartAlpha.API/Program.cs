using SmartAlpha.Analytics.Analysis;
using SmartAlpha.Data;
using SmartAlpha.Analytics.Validation;
using SmartAlpha.Reporting.Formatters;

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

var formatter = new PortfolioSummaryFormatter();
var output = formatter.Format(summary);

Console.WriteLine(output);