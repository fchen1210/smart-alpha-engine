using SmartAlpha.Core.Models;

namespace SmartAlpha.Analytics.Validation;

public class PortfolioValidator
{
    private const decimal TargetTotalWeight = 1.0m;
    private const decimal Tolerance = 0.0001m;

    public ValidationResult Validate(Portfolio portfolio)
    {
        var result = new ValidationResult();

        if (portfolio == null)
        {
            result.Errors.Add("Portfolio cannot be null.");
            result.IsValid = false;
            return result;
        }

        if (portfolio.Holdings == null || portfolio.Holdings.Count == 0)
        {
            result.Errors.Add("Portfolio must contain at least one holding.");
            result.IsValid = false;
            return result;
        }

        foreach (var holding in portfolio.Holdings)
        {
            if (string.IsNullOrWhiteSpace(holding.Ticker))
            {
                result.Errors.Add("Holding ticker cannot be null or empty.");
            }

            if (holding.Weight < 0)
            {
                result.Errors.Add($"Holding '{holding.Ticker}' has a negative weight.");
            }
        }

        var totalWeight = portfolio.Holdings.Sum(h => h.Weight);

        if (Math.Abs(totalWeight - TargetTotalWeight) > Tolerance)
        {
            result.Errors.Add(
                $"Portfolio total weight must equal 1.0. Current total: {totalWeight:F4}");
        }

        result.IsValid = result.Errors.Count == 0;
        return result;
    }
}