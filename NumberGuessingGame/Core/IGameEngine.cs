using NumberGuessingGame.Core.Enums;

namespace NumberGuessingGame.Core;
public interface IGameEngine
{
    int Attempts { get; }
    bool IsCorrect { get; }
    bool IsGameOver { get; }
    int RemainingAttempts { get; }
    int TargetNumber { get; }
    TimeSpan ElapsedTime { get; }

    (int lowerBound, int upperBound) GetHintRange();
    GuessResult MakeGuess(int guess);
    void StartNewGame(DifficultyLevel difficulty);
}