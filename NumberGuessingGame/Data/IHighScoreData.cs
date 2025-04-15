using NumberGuessingGame.Models;

namespace NumberGuessingGame.Data;

public interface IHighScoreData
{
    HighScoreModel? GetHighScore();
    void SaveHighScore(int score, DateTime dateAchieved);
}