namespace SmartAlpha.Core.Models.Market;

public class PriceBar
{
    public DateTime Date { get; init; }

    public decimal Open { get; init; }

    public decimal High { get; init; }

    public decimal Low { get; init; }

    public decimal Close { get; init; }

    public decimal Volume { get; init; }

    public PriceBar() { }

    public PriceBar(DateTime date, decimal open, decimal high, decimal low, decimal close, decimal volume)
    {
        if (high < low) throw new ArgumentException("High must be greater than or equal to Low.", nameof(high));
        if (open < low || open > high) throw new ArgumentOutOfRangeException(nameof(open), "Open must be between Low and High.");
        if (close < low || close > high) throw new ArgumentOutOfRangeException(nameof(close), "Close must be between Low and High.");
        if (volume < 0) throw new ArgumentOutOfRangeException(nameof(volume), "Volume must be non-negative.");

        Date = date;
        Open = open;
        High = high;
        Low = low;
        Close = close;
        Volume = volume;
    }
}
