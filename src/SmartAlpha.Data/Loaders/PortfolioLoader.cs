using System.Text.Json;
using SmartAlpha.Core.Models;

namespace SmartAlpha.Data;

public class PortfolioLoader : IPortfolioLoader
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        PropertyNameCaseInsensitive = true
    };

    public Portfolio LoadFromFile(string filePath)
    {
        if (string.IsNullOrWhiteSpace(filePath))
        {
            throw new ArgumentException("File path cannot be null or empty.", nameof(filePath));
        }

        if (!File.Exists(filePath))
        {
            throw new FileNotFoundException($"Portfolio file not found: {filePath}", filePath);
        }

        var json = File.ReadAllText(filePath);

        var portfolio = JsonSerializer.Deserialize<Portfolio>(json, JsonOptions);

        if (portfolio is null)
        {
            throw new InvalidOperationException("Failed to deserialize portfolio JSON.");
        }

        if (portfolio.Holdings is null)
        {
            throw new InvalidOperationException("Portfolio holdings cannot be null.");
        }

        return portfolio;
    }
}