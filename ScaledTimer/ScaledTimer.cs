using System.Diagnostics;

namespace ScaledTimer;

public delegate void ElapsedEventHandler(object sender, ElapsedEventArgs e);

public class ScaledTimer
{
    private readonly double _interval;
    private readonly double _startScale;
    private readonly bool _autoReset;

    private double _accumulatedTimeMilliseconds;
    private double _currentFactor;

    private readonly Stopwatch _stopwatch = new();
    private long? _lastTicks;
    private static readonly double TickFrequency = 1000.0 / Stopwatch.Frequency;

    private CancellationTokenSource? _cts;
    private Task? _tickTask;

    public event ElapsedEventHandler? Elapsed;

    public bool Running { get; private set; }

    public ScaledTimer(double intervalMs, double startScale, bool autoReset = false)
    {
        _interval = intervalMs;
        _startScale = startScale;
        _autoReset = autoReset;
    }

    public void Start()
    {
        _currentFactor = Math.Clamp(_startScale / 100.0, 0, 1);
        _accumulatedTimeMilliseconds = 0;
        _lastTicks = null;
        Running = true;

        _cts = new CancellationTokenSource();
        _stopwatch.Restart();
        _tickTask = Task.Run(() => TickLoop(_cts.Token));
    }

    public void Stop()
    {
        Running = false;
        _cts?.Cancel();
        _stopwatch.Stop();
    }

    public void SetScale(double percent)
    {
        _currentFactor = Math.Clamp(percent / 100.0, 0, 1);
    }

    public TimeSpan GetTime()
    {
        Update();
        return TimeSpan.FromMilliseconds(_accumulatedTimeMilliseconds);
    }

    public void Reset()
    {
        _accumulatedTimeMilliseconds = 0;
        _lastTicks = null;
    }

    private async Task TickLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            Update();
            await Task.Delay(1, token);
        }
    }

    private void Update()
    {
        if (!Running) return;

        long nowTicks = Stopwatch.GetTimestamp();

        if (_lastTicks.HasValue)
        {
            double deltaMs = (nowTicks - _lastTicks.Value) * TickFrequency;
            _accumulatedTimeMilliseconds += deltaMs * _currentFactor;

            if (_accumulatedTimeMilliseconds >= _interval)
            {
                Elapsed?.Invoke(this, new ElapsedEventArgs(
                    DateTime.UtcNow,
                    TimeSpan.FromMilliseconds(_accumulatedTimeMilliseconds),
                    _currentFactor * 100.0,
                    TimeSpan.FromMilliseconds(deltaMs)
                ));

                if (_autoReset)
                {
                    _accumulatedTimeMilliseconds = 0;
                }
                else
                {
                    Stop();
                }
            }
        }

        _lastTicks = nowTicks;
    }
}
