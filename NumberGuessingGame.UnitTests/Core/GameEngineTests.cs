using NSubstitute;
using NumberGuessingGame.Core;
using NumberGuessingGame.Core.Enums;

namespace NumberGuessingGame.UnitTests.Core;

public class GameEngineTests
{
    [Fact]
    public void StartNewGame_SetsTargetNumberAndAttemptsBasedOnDifficulty()
    {
        var rng = Substitute.For<IRandomNumberGenerator>();
        var gameTimer = Substitute.For<IGameTimer>();

        rng.Next(1, 101).Returns(50);

        var engine = new GameEngine(rng, gameTimer);

        engine.StartNewGame(DifficultyLevel.Easy);

        Assert.Equal(50, engine.TargetNumber);
        Assert.Equal(10, engine.RemainingAttempts);
        Assert.False(engine.IsGameOver);
        Assert.False(engine.IsCorrect);
        Assert.Equal(TimeSpan.Zero, engine.ElapsedTime);
        gameTimer.Received(1).Reset();
        gameTimer.Received(1).Start();
    }

    [Fact]
    public void MakeGuess_ReturnsCorrect_WhenGuessIsEqualToTarget()
    {
        var rng = Substitute.For<IRandomNumberGenerator>();
        var gameTimer = Substitute.For<IGameTimer>();
        rng.Next(1, 101).Returns(50);

        var engine = new GameEngine(rng, gameTimer);
        engine.StartNewGame(DifficultyLevel.Easy);
        var result = engine.MakeGuess(50);

        Assert.Equal(GuessResult.Correct, result);
        Assert.True(engine.IsGameOver);
        Assert.True(engine.IsCorrect);
        Assert.Equal(1, engine.Attempts);
    }

    [Fact]
    public void MakeGuess_ReturnsTooLow_WhenGuessIsLowerThanTarget()
    {
        var rng = Substitute.For<IRandomNumberGenerator>();
        var gameTimer = Substitute.For<IGameTimer>();
        rng.Next(1, 101).Returns(50);

        var engine = new GameEngine(rng, gameTimer);
        engine.StartNewGame(DifficultyLevel.Easy);
        var result = engine.MakeGuess(30);

        Assert.Equal(GuessResult.TooLow, result);
        Assert.False(engine.IsGameOver);
        Assert.False(engine.IsCorrect);
        Assert.Equal(1, engine.Attempts);
    }

    [Fact]
    public void MakeGuess_ReturnsTooHigh_WhenGuessIsHigherThanTarget()
    {
        var rng = Substitute.For<IRandomNumberGenerator>();
        var gameTimer = Substitute.For<IGameTimer>();
        rng.Next(1, 101).Returns(50);

        var engine = new GameEngine(rng, gameTimer);
        engine.StartNewGame(DifficultyLevel.Easy);
        var result = engine.MakeGuess(70);

        Assert.Equal(GuessResult.TooHigh, result);
        Assert.False(engine.IsGameOver);
        Assert.False(engine.IsCorrect);
        Assert.Equal(1, engine.Attempts);
    }

    [Fact]
    public void MakeGuess_StopsTimer_WhenGameOver()
    {
        var rng = Substitute.For<IRandomNumberGenerator>();
        var gameTimer = Substitute.For<IGameTimer>();
        rng.Next(1, 101).Returns(50);

        var engine = new GameEngine(rng, gameTimer);
        engine.StartNewGame(DifficultyLevel.Easy);
        var result = engine.MakeGuess(50);

        gameTimer.Received(1).Stop();
    }

    [Fact]
    public void MakeGuess_ReturnsOutOfAttempts_WhenMaxAttemptsReached()
    {
        var rng = Substitute.For<IRandomNumberGenerator>();
        var gameTimer = Substitute.For<IGameTimer>();
        rng.Next(1, 101).Returns(50);

        var engine = new GameEngine(rng, gameTimer);
        engine.StartNewGame(DifficultyLevel.Hard);
        engine.MakeGuess(30);
        engine.MakeGuess(40);
        var result = engine.MakeGuess(60);

        Assert.Equal(GuessResult.OutOfAttempts, result);
        Assert.True(engine.IsGameOver);
        Assert.False(engine.IsCorrect);
        gameTimer.Received(1).Stop();
    }

    [Fact]
    public void MakeGuess_ThrowsException_WhenGameIsAlreadyOver()
    {
        var rng = Substitute.For<IRandomNumberGenerator>();
        var gameTimer = Substitute.For<IGameTimer>();
        rng.Next(1, 101).Returns(50);

        var engine = new GameEngine(rng, gameTimer);
        engine.StartNewGame(DifficultyLevel.Easy);
        engine.MakeGuess(50);

        Assert.Throws<InvalidOperationException>(() => engine.MakeGuess(30));
    }

    [Theory]
    [InlineData(50, 7, 43, 57)]
    [InlineData(1, 5, 1, 6)]
    [InlineData(100, 8, 92, 100)]
    public void GetHintRange_ReturnsCorrectRange_WhenGameIsInProgress(
        int targetNumber,
        int randomBuffer,
        int expectedLower,
        int expectedUpper)
    {
        var rng = Substitute.For<IRandomNumberGenerator>();
        var gameTimer = Substitute.For<IGameTimer>();

        rng.Next(1, 101).Returns(targetNumber);
        rng.Next(5, 16).Returns(randomBuffer);

        var engine = new GameEngine(rng, gameTimer);
        engine.StartNewGame(DifficultyLevel.Easy);

        (int, int) hintRange = engine.GetHintRange();

        Assert.Equal((expectedLower, expectedUpper), hintRange);
    }

    [Fact]
    public void GetHintRange_ThrowsInvalidOperation_WhenGameHasNotStarted()
    {
        var rng = Substitute.For<IRandomNumberGenerator>();
        var gameTimer = Substitute.For<IGameTimer>();
        var engine = new GameEngine(rng, gameTimer);

        Assert.Throws<InvalidOperationException>(() => engine.GetHintRange());
    }

    [Fact]
    public void GetHintRange_ThrowsInvalidOperation_WhenGameIsOver()
    {
        var rng = Substitute.For<IRandomNumberGenerator>();
        var gameTimer = Substitute.For<IGameTimer>();
        rng.Next(1, 101).Returns(50);

        var engine = new GameEngine(rng, gameTimer);
        engine.StartNewGame(DifficultyLevel.Easy);
        engine.MakeGuess(50);

        Assert.Throws<InvalidOperationException>(() => engine.GetHintRange());
    }
}
