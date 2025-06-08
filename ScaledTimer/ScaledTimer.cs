using System.Diagnostics;

namespace ScaledTimer;

public delegate void ElapsedEventHandler(object sender, ElapsedEventArgs e);

public class ScaledTimer
{
    private readonly double _intervalMs;
    private readonly double _startScale;
    private readonly bool _autoReset;

    private double _currentFactor;
    private double _accumulatedTimeMilliseconds;
    private long? _prevTicks;

    private static readonly double TickFrequency = 1000.0 / Stopwatch.Frequency;

    private CancellationTokenSource? _cts;
    private Task _updateTask;

    public ScaledTimer(double intervalMs, double startScale, bool autoReset = false)
    {
        _intervalMs = intervalMs;
        _startScale = startScale;
        _autoReset = autoReset;
    }

    public event ElapsedEventHandler? Elapsed;

    public bool Running { get; private set; }

    public void Start()
    {
        if (Running) return;
        SetScale(_startScale);
        _accumulatedTimeMilliseconds = 0;
        _prevTicks = null;
        _cts = new CancellationTokenSource();
        _updateTask = Task.Run(() => UpdateLoop(_cts.Token));
        Running = true;
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

    private async Task UpdateLoop(CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            Update();
            await Task.Delay(1, token);
        }
    }
    
    private void Stop()
    {
        Running = false;
        _cts?.Cancel();
    }

    private void Update()
    {
        if (!Running) return;

        var nowTicks = Stopwatch.GetTimestamp();
        if (_prevTicks.HasValue)
        {
            var deltaMs = (nowTicks - _prevTicks.Value) * TickFrequency;
            _accumulatedTimeMilliseconds += deltaMs * _currentFactor;

            if (_accumulatedTimeMilliseconds >= _intervalMs)
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

        _prevTicks = nowTicks;
    }
}