using UnityEngine;
using System.IO;
using UnityEngine.SceneManagement;

public class SaveSystem : MonoBehaviour
{
    private string savePath;
    public GameData gameData;
    public int totalLevels = 2; // Cambiar si agregas más niveles

    private void Awake()
    {
        savePath = Application.persistentDataPath + "/save.json";
        LoadGame();
    }


    public void SaveGame()
    {
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(savePath, json);
        Debug.Log("Juego guardado en: " + savePath);
    }

    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            gameData = JsonUtility.FromJson<GameData>(json);
            Debug.Log("Juego cargado correctamente");
        }
        else
        {
            gameData = new GameData(totalLevels);
            Debug.Log("No se encontró guardado, creando nuevo");
            SaveGame();
        }
    }

    // Guarda el progreso de un nivel específico
    public void CompleteLevel(int levelNumber, float timeTaken, int livesLeft)
    {
        LevelData level = gameData.GetLevelData(levelNumber);
        if (level != null)
        {
            level.completed = true;
            level.timeTaken = timeTaken;
            level.livesLeft = livesLeft;
            SaveGame();
        }
        else
        {
            Debug.LogWarning("Nivel no encontrado: " + levelNumber);
        }
    }

    // Guarda automáticamente usando la escena actual
    public void CompleteCurrentLevel(float timeTaken, int livesLeft)
    {
        int nivelActual = SceneManager.GetActiveScene().buildIndex - 1;
        if(GetLevelData(nivelActual).timeTaken > timeTaken)
        {
            CompleteLevel(nivelActual, timeTaken, livesLeft);
        }
    }

    public LevelData GetLevelData(int levelNumber)
    {
        return gameData.GetLevelData(levelNumber);
    }
}
