namespace ScaledTimer;

public class ElapsedEventArgs : EventArgs
{
    public DateTime Timestamp { get; }
    public TimeSpan ScaledElapsed { get; }
    public double ScalePercent { get; }
    public TimeSpan RealDelta { get; }

    public ElapsedEventArgs(DateTime timestamp, TimeSpan scaledElapsed, double scalePercent, TimeSpan realDelta)
    {
        Timestamp = timestamp;
        ScaledElapsed = scaledElapsed;
        ScalePercent = scalePercent;
        RealDelta = realDelta;
    }
}