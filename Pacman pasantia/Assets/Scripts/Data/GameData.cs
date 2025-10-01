using System;
using System.Collections.Generic;

[Serializable]
public class LevelData
{
    public int levelNumber;
    public bool completed;
    public float timeTaken;
    public int livesLeft;

    public LevelData(int number)
    {
        levelNumber = number;
        completed = false;
        timeTaken = 0f;
        livesLeft = 0;
    }
}

[Serializable]
public class GameData
{
    public List<LevelData> levels;

    public GameData(int totalLevels)
    {
        levels = new List<LevelData>();
        for (int i = 0; i < totalLevels; i++)
        {
            levels.Add(new LevelData(i));
        }
    }

    public LevelData GetLevelData(int levelNumber)
    {
        return levels.Find(l => l.levelNumber == levelNumber);
    }
}
