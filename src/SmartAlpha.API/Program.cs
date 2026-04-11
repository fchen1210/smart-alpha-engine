using SmartAlpha.API.Services;

var filePath = Path.GetFullPath(Path.Combine(
    Directory.GetCurrentDirectory(),
    "..",
    "..",
    "data",
    "portfolio.sample.json"));

Console.WriteLine($"Loading portfolio from: {filePath}");

var workflow = new DailyPortfolioRiskWorkflow(filePath);
var result = await workflow.RunAsync(CancellationToken.None);

Console.WriteLine(result);