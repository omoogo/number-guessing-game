using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberGuessingGame.Core;

public interface IGameTimer
{
    void Start();
    void Stop();
    TimeSpan Elapsed { get; }
    bool IsRunning { get; }
    void Reset();
}
