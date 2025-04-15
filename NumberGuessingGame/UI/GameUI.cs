using NumberGuessingGame.Core;
using NumberGuessingGame.Core.Enums;
using NumberGuessingGame.Data;
using Spectre.Console;

namespace NumberGuessingGame.UI;

public class GameUI : IGameUI
{
    private readonly IGameEngine _gameEngine;
    private readonly IHighScoreData _highScoreData;

    public GameUI(IGameEngine gameEngine, IHighScoreData highScoreData)
    {
        _gameEngine = gameEngine;
        _highScoreData = highScoreData;
    }

    public void LaunchGame()
    {
        AnsiConsole.Write(new FigletText("Number Guessing Game").Centered().Color(Color.SteelBlue));

        AnsiConsole.MarkupLine("Welcome to the [bold]Number Guessing Game[/]!");
        Console.WriteLine();

        var selectedOperation = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .AddChoices(["Play Game", "Exit"]));

        if (selectedOperation == "Exit")
        {
            AnsiConsole.MarkupLine("Thank you for playing! Goodbye!");
            return;
        }

        StartGameLoop();
    }

    private void StartGameLoop()
    {
        bool playAgain;

        do
        {
            PlayGame();
            playAgain = AskToPlayAgain();

            Console.WriteLine();
        }
        while (playAgain);
    }

    private bool AskToPlayAgain()
    {
        return AnsiConsole.Prompt(
            new TextPrompt<bool>("Do you want to play again?")
                .AddChoice(true)
                .AddChoice(false)
                .DefaultValue(true)
                .WithConverter(choice => choice ? "y" : "n"));
    }

    private void PlayGame()
    {
        DifficultyLevel difficulty = AskForDifficulty();

        _gameEngine.StartNewGame(difficulty);

        AnsiConsole.MarkupLine("Let's start the game!");
        Console.WriteLine();

        AnsiConsole.MarkupLine($"I'm thinking of a number between 1 and 100.");
        AnsiConsole.MarkupLine($"You have {_gameEngine.RemainingAttempts} chances to guess the correct number.");
        Console.WriteLine();

        ProcessGuesses();
    }

    private DifficultyLevel AskForDifficulty()
    {
        var selectedDifficulty = AnsiConsole.Prompt(
            new SelectionPrompt<string>()
                .Title("Please select the difficulty level:")
                .AddChoices(["1. Easy (10 chances)", "2. Medium (5 chances)", "3. Hard (3 chances)"]));

        return selectedDifficulty switch
        {
            "1. Easy (10 chances)" => DifficultyLevel.Easy,
            "2. Medium (5 chances)" => DifficultyLevel.Medium,
            "3. Hard (3 chances)" => DifficultyLevel.Hard,
            _ => throw new ArgumentOutOfRangeException(nameof(selectedDifficulty), selectedDifficulty, null)
        };
    }

    private void ProcessGuesses()
    {
        while (!_gameEngine.IsGameOver)
        {
            var input = AnsiConsole.Ask<string>("Enter your guess (or type [blue]'hint'[/]):");

            if (input.Equals("hint", StringComparison.OrdinalIgnoreCase))
            {
                var (low, high) = _gameEngine.GetHintRange();
                AnsiConsole.MarkupLine($"[yellow]Hint: The number is between {low} and {high}.[/]");
                continue;
            }

            if (!int.TryParse(input, out int guess) || guess < 1 || guess > 100)
            {
                AnsiConsole.MarkupLine("[red]Invalid input! Please enter a number between 1 and 100.[/]");
                continue;
            }

            var result = _gameEngine.MakeGuess(guess);

            switch (result)
            {
                case GuessResult.Correct:
                    HandleCorrectGuess();
                    break;
                case GuessResult.TooLow:
                    AnsiConsole.MarkupLine($"[red]Incorrect! The number is higher than {guess}.[/]");
                    break;
                case GuessResult.TooHigh:
                    AnsiConsole.MarkupLine($"[red]Incorrect! The number is lower than {guess}.[/]");
                    break;
                case GuessResult.OutOfAttempts:
                    HandleGameOver();
                    break;
            }
        }
    }

    private void HandleCorrectGuess()
    {
        var currentHighScore = _highScoreData.GetHighScore();

        if (currentHighScore is null || _gameEngine.Attempts < currentHighScore.Score)
        {
            AnsiConsole.MarkupLine($"[green]New High Score! You guessed the number in {_gameEngine.Attempts} attempts. Time taken: {_gameEngine.ElapsedTime:mm\\:ss\\.fff}[/]");

            _highScoreData.SaveHighScore(_gameEngine.Attempts, DateTime.Now);
        }
        else
        {
            AnsiConsole.MarkupLine($"[green]Congratulations! You have guessed the correct number in {_gameEngine.Attempts} attempts. Time taken: {_gameEngine.ElapsedTime:mm\\:ss\\.fff}. Well done![/]");
        }
    }

    private void HandleGameOver()
    {
        AnsiConsole.MarkupLine($"[red]Game Over! You used all your attempts. The number was {_gameEngine.TargetNumber}.[/]");
    }
}
