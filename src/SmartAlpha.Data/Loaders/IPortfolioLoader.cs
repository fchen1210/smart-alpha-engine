using SmartAlpha.Core.Models;

namespace SmartAlpha.Data;

public interface IPortfolioLoader
{
    Portfolio LoadFromFile(string filePath);
}