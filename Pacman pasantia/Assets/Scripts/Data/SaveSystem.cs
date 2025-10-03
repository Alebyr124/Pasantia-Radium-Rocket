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
            SaveGame(); // Guarda el archivo inicial
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

        Debug.Log("Build Index: " + SceneManager.GetActiveScene().buildIndex);
        Debug.Log("Nivel calculado: " + nivelActual);

        LevelData levelData = GetLevelData(nivelActual);

        if (levelData != null)
        {
            // Si el nivel no está completado o si el nuevo tiempo es mejor
            if (!levelData.completed || levelData.timeTaken > timeTaken)
            {
                CompleteLevel(nivelActual, timeTaken, livesLeft);
            }
            else
            {
                Debug.Log("Ya existe un mejor tiempo para este nivel");
            }
        }
        else
        {
            Debug.LogError("No se encontró data para el nivel " + nivelActual);
        }
    }

    public LevelData GetLevelData(int levelNumber)
    {
        return gameData.GetLevelData(levelNumber);
    }
}