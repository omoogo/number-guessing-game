using NumberGuessingGame.Core.Enums;

namespace NumberGuessingGame.Core;

public class GameEngine : IGameEngine
{
    private readonly IRandomNumberGenerator _rng;
    private readonly IGameTimer _gameTimer;
    private int _target;
    private int _maxAttempts;

    public int Attempts { get; private set; }
    public bool IsGameOver { get; private set; }
    public bool IsCorrect { get; private set; }
    public int RemainingAttempts => _maxAttempts - Attempts;
    public int TargetNumber => _target;
    public TimeSpan ElapsedTime => _gameTimer.Elapsed;

    public GameEngine(IRandomNumberGenerator rng, IGameTimer gameTimer)
    {
        _rng = rng;
        _gameTimer = gameTimer;
    }

    public void StartNewGame(DifficultyLevel difficulty)
    {
        _target = _rng.Next(1, 101);
        _maxAttempts = difficulty switch
        {
            DifficultyLevel.Easy => 10,
            DifficultyLevel.Medium => 5,
            DifficultyLevel.Hard => 3,
            _ => throw new ArgumentOutOfRangeException(nameof(difficulty), difficulty, null)
        };

        Attempts = 0;
        IsGameOver = false;
        IsCorrect = false;
        
        _gameTimer.Reset();
        _gameTimer.Start();
    }

    public GuessResult MakeGuess(int guess)
    {
        if (IsGameOver)
        {
            throw new InvalidOperationException("Game is already over.");
        }

        Attempts++;

        if (guess == _target)
        {
            IsCorrect = true;
            IsGameOver = true;
            _gameTimer.Stop();
            return GuessResult.Correct;
        }

        if (Attempts >= _maxAttempts)
        {
            IsGameOver = true;
            _gameTimer.Stop();
            return GuessResult.OutOfAttempts;
        }

        return guess < _target ? GuessResult.TooLow : GuessResult.TooHigh;
    }

    public (int lowerBound, int upperBound) GetHintRange()
    {
        if (TargetNumber == 0) throw new InvalidOperationException("Game has not started yet.");
        if (IsGameOver) throw new InvalidOperationException("Game is already over.");

        int buffer = _rng.Next(5, 16);
        int lower = Math.Max(1, TargetNumber - buffer);
        int upper = Math.Min(100, TargetNumber + buffer);
        return (lower, upper);
    }
}
