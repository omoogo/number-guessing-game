using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NumberGuessingGame.Core;

public class SystemRandomNumberGenerator : IRandomNumberGenerator
{
    public int Next(int minValue, int maxValue) => Random.Shared.Next(minValue, maxValue);
}
