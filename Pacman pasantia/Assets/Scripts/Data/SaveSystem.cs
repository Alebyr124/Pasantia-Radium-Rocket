using UnityEngine;
using System.IO;
using System.Collections.Generic;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine.SceneManagement;

public class SaveSystem : MonoBehaviour
{
    private string savePath;
    public GameData gameData;
    public int totalLevels = 2; // Ajustá según tu juego

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
        savePath = Application.persistentDataPath + "/save.json";
        LoadGame();
    }

    // ------------------------------
    // GUARDADO LOCAL
    // ------------------------------
    public void SaveGame()
    {
        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText(savePath, json);
        Debug.Log("💾 Guardado local en: " + savePath);
    }

    public void LoadGame()
    {
        if (File.Exists(savePath))
        {
            string json = File.ReadAllText(savePath);
            gameData = JsonUtility.FromJson<GameData>(json);
            Debug.Log("📂 Cargado localmente");
        }
        else
        {
            gameData = new GameData(totalLevels);
            Debug.Log("🆕 No se encontró guardado, creando nuevo");
        }
    }

    // ------------------------------
    // GUARDADO DE NIVEL
    // ------------------------------
    public void CompleteLevel(int levelNumber, float timeTaken, int livesLeft)
    {
        LevelData level = gameData.GetLevelData(levelNumber);
        if (level != null)
        {
            // Si mejora el tiempo, actualizamos
            if (level.timeTaken == 0 || timeTaken < level.timeTaken)
            {
                level.completed = true;
                level.timeTaken = timeTaken;
                level.livesLeft = livesLeft;

                SaveGame();  // Guardado local
                SaveToFirebase(levelNumber, timeTaken, livesLeft);  // Guardado online
            }
        }
        else
        {
            Debug.LogWarning("Nivel no encontrado: " + levelNumber);
        }
    }

    public void CompleteCurrentLevel(float timeTaken, int livesLeft)
    {
        int nivelActual = SceneManager.GetActiveScene().buildIndex - 2;
        CompleteLevel(nivelActual, timeTaken, livesLeft);
    }

    public LevelData GetLevelData(int levelNumber)
    {
        return gameData.GetLevelData(levelNumber);
    }

    // ------------------------------
    // GUARDADO EN FIREBASE
    // ------------------------------
    private void SaveToFirebase(int levelNumber, float timeTaken, int livesLeft)
    {
        if (FirebaseInit.DBreference == null)
        {
            Debug.LogWarning("⚠️ Firebase no inicializado. Solo se guardó localmente.");
            return;
        }

        string playerId = PlayerPrefs.GetString("PlayerName", "JugadorDesconocido");
        string levelName = "Level" + levelNumber;

        Dictionary<string, object> data = new Dictionary<string, object>();
        data["levelNumber"] = levelNumber;
        data["timeTaken"] = timeTaken;
        data["livesLeft"] = livesLeft;
        data["timestamp"] = System.DateTime.UtcNow.ToString("o");

        FirebaseInit.DBreference
            .Child("saves")
            .Child(playerId)
            .Child(levelName)
            .SetValueAsync(data)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                {
                    Debug.Log($"☁️ Guardado en Firebase: {levelName} ({timeTaken}s, {livesLeft} vidas)");
                }
                else
                {
                    Debug.LogError($"❌ Error al guardar en Firebase: {task.Exception}");
                }
            });
    }

    // ------------------------------
    // CARGA DESDE FIREBASE
    // ------------------------------
    public void LoadFromFirebase(int levelNumber, System.Action<LevelData> onLoaded)
    {
        if (FirebaseInit.DBreference == null)
        {
            Debug.LogWarning("⚠️ Firebase no inicializado.");
            onLoaded?.Invoke(null);
            return;
        }

        string playerId = PlayerPrefs.GetString("PlayerName", "JugadorDesconocido");
        string levelName = "Level" + levelNumber;

        FirebaseInit.DBreference
            .Child("saves")
            .Child(playerId)
            .Child(levelName)
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsFaulted)
                {
                    Debug.LogError("Error al cargar desde Firebase: " + task.Exception);
                    onLoaded?.Invoke(null);
                    return;
                }

                DataSnapshot snap = task.Result;
                if (snap.Exists)
                {
                    LevelData data = new LevelData(levelNumber);
                    data.completed = true;
                    data.timeTaken = float.Parse(snap.Child("timeTaken").Value.ToString());
                    data.livesLeft = int.Parse(snap.Child("livesLeft").Value.ToString());

                    Debug.Log($"✅ Cargado de Firebase: Nivel {levelNumber} - {data.timeTaken}s - {data.livesLeft} vidas");
                    onLoaded?.Invoke(data);
                }
                else
                {
                    Debug.Log($"⚠️ No se encontró nivel {levelNumber} en Firebase");
                    onLoaded?.Invoke(null);
                }
            });
    }

    public void LoadScoreboard(int levelNumber, System.Action<List<(string playerName, float time)>> onLoaded)
    {
        FirebaseInit.DBreference
            .Child("saves")
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                List<(string, float)> scores = new();

                if (task.IsCompleted)
                {
                    foreach (var playerSnap in task.Result.Children)
                    {
                        var levelSnap = playerSnap.Child("Level" + levelNumber);
                        if (levelSnap.Exists && levelSnap.Child("timeTaken").Value != null)
                        {
                            string playerName = playerSnap.Key;
                            float time = float.Parse(levelSnap.Child("timeTaken").Value.ToString());
                            scores.Add((playerName, time));
                        }
                    }

                    // Ordenar de menor a mayor tiempo
                    scores.Sort((a, b) => a.Item2.CompareTo(b.Item2));
                    onLoaded?.Invoke(scores);
                }
            });
    }

}
