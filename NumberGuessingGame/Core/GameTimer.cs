using System.Diagnostics;

namespace NumberGuessingGame.Core;

public class GameTimer : IGameTimer
{
    private readonly Stopwatch _stopwatch;

    public GameTimer()
    {
        _stopwatch = new Stopwatch();
    }

    public void Start()
    {
        _stopwatch.Start();
    }

    public void Stop()
    {
        _stopwatch.Stop();
    }

    public TimeSpan Elapsed => _stopwatch.Elapsed;

    public bool IsRunning => _stopwatch.IsRunning;

    public void Reset()
    {
        _stopwatch.Reset();
    }
}
