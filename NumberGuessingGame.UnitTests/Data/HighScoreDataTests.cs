using NumberGuessingGame.Data;
using NumberGuessingGame.Models;
using System.IO.Abstractions.TestingHelpers;
using System.Text.Json;

namespace NumberGuessingGame.UnitTests.Data;

public class HighScoreDataTests
{
    private const string _basePath = @"C:\app";
    private const string _dataPath = @"C:\app\Data";
    private const string _filePath = @"C:\app\Data\HighScoreData.json";

    [Fact]
    public void GetHighScore_ReturnsNull_WhenFileDoesNotExist()
    {
        var fileSystem = new MockFileSystem();
        var data = new HighScoreData(fileSystem);

        var result = data.GetHighScore();

        Assert.Null(result);
    }

    [Fact]
    public void GetHighScore_ReturnsHighScore_WhenFileExists()
    {
        var highScore = new HighScoreModel
        {
            Score = 5,
            DateAchieved = new DateTime(2025, 4, 1, 12, 0, 0)
        };
        var json = JsonSerializer.Serialize(highScore);

        var fileSystem = new MockFileSystem(new Dictionary<string, MockFileData>
        {
            { _filePath, new MockFileData(json) }
        });

        fileSystem.Directory.SetCurrentDirectory(_basePath);

        var data = new HighScoreData(fileSystem);
        var result = data.GetHighScore();

        Assert.NotNull(result);
        Assert.Equal(highScore.Score, result.Score);
        Assert.Equal(highScore.DateAchieved, result.DateAchieved);
    }

    [Fact]
    public void SaveHighScore_CreateFileWithCorrectContent_WhenFileDoesNotExist()
    {
        var fileSystem = new MockFileSystem();
        fileSystem.Directory.SetCurrentDirectory(_basePath);
        var data = new HighScoreData(fileSystem);
        var highScore = 5;
        var expectedDate = new DateTime(2025, 4, 1, 12, 0, 0);

        data.SaveHighScore(highScore, expectedDate);

        var dataFilePath = _filePath;
        Assert.True(fileSystem.Directory.Exists(_dataPath));
        Assert.True(fileSystem.File.Exists(dataFilePath));

        var fileContent = fileSystem.File.ReadAllText(dataFilePath);
        var highScoreModel = JsonSerializer.Deserialize<HighScoreModel>(fileContent);

        Assert.NotNull(highScoreModel);
        Assert.Equal(highScore, highScoreModel.Score);
        Assert.Equal(expectedDate, highScoreModel.DateAchieved);
    }
}
