namespace SmartAlpha.Core.Models.Market;

public class PriceBar
{
    public DateTime Date { get; private set; }

    public decimal Open { get; private set; }

    public decimal High { get; private set; }

    public decimal Low { get; private set; }

    public decimal Close { get; private set; }

    public decimal Volume { get; private set; }

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
