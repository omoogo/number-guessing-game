using NumberGuessingGame.Models;
using System.IO.Abstractions;
using System.Text.Json;

namespace NumberGuessingGame.Data;

public class HighScoreData : IHighScoreData
{
    private readonly string _filePath;
    private readonly IFileSystem _fileSystem;

    public HighScoreData(IFileSystem fileSystem)
    {
        _fileSystem = fileSystem;
        _filePath = _fileSystem.Path.Combine(
            _fileSystem.Directory.GetCurrentDirectory(),
            "Data",
            "HighScoreData.json");
    }

    public HighScoreModel? GetHighScore()
    {
        if (!_fileSystem.File.Exists(_filePath))
        {
            return null;
        }

        string json = _fileSystem.File.ReadAllText(_filePath);

        return JsonSerializer.Deserialize<HighScoreModel>(json);
    }

    public void SaveHighScore(int score, DateTime dateAchieved)
    {
        var highScoreModel = new HighScoreModel
        {
            Score = score,
            DateAchieved = dateAchieved
        };

        string json = JsonSerializer.Serialize(highScoreModel, new JsonSerializerOptions
        {
            WriteIndented = true
        });

        string? dir = _fileSystem.Path.GetDirectoryName(_filePath);
        if (!_fileSystem.Directory.Exists(dir))
        {
            _fileSystem.Directory.CreateDirectory(dir!);
        }

        _fileSystem.File.WriteAllText(_filePath, json);
    }
}
