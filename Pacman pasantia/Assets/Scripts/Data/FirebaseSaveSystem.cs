using Firebase;
using Firebase.Database;
using Firebase.Extensions;
using UnityEngine;

public class FirebaseSaveSystem : MonoBehaviour
{
    public static FirebaseSaveSystem Instance;
    public GameData gameData;
    public int totalLevels = 2;
    private DatabaseReference dbRef;
    public bool isLoaded = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWithOnMainThread(task =>
        {
            if (task.Result == DependencyStatus.Available)
            {
                dbRef = FirebaseDatabase.GetInstance(FirebaseApp.DefaultInstance,
                    "https://pacmanrr-43e78-default-rtdb.firebaseio.com/").RootReference;
                LoadGame();
            }
            else
            {
                Debug.LogError("No se pudieron resolver dependencias de Firebase: " + task.Result);
            }
        });
    }

    public void SaveGame()
    {
        if (gameData == null) return;
        string json = JsonUtility.ToJson(gameData);
        Debug.Log("Guardando JSON: " + json); // Depuración
        dbRef.Child("players").Child("player1").SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCompleted)
                    Debug.Log("Datos guardados en Firebase ✅");
                else
                    Debug.LogError("Error al guardar en Firebase: " + task.Exception);
            });
    }

    public void LoadGame()
    {
        dbRef.Child("players").Child("player1").GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCompleted)
            {
                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    string json = snapshot.GetRawJsonValue();
                    Debug.Log("JSON cargado desde Firebase: " + json); // Depuración
                    gameData = JsonUtility.FromJson<GameData>(json);
                    if (gameData == null)
                    {
                        Debug.LogError("Deserialización fallida, creando nuevos datos");
                        gameData = new GameData(totalLevels);
                    }
                }
                else
                {
                    Debug.Log("No hay datos en Firebase, creando nuevos");
                    gameData = new GameData(totalLevels);
                }
                SaveGame();
                isLoaded = true;
            }
            else
            {
                Debug.LogError("Error al cargar datos desde Firebase: " + task.Exception);
            }
        });
    }

    public void CompleteLevel(int levelNumber, float timeTaken, int livesLeft)
    {
        if (gameData == null) return;
        LevelData level = gameData.GetLevelData(levelNumber);
        if (level != null)
        {
            level.completed = true;
            level.timeTaken = timeTaken;
            level.livesLeft = livesLeft;
            SaveGame();
        }
    }

    public void CompleteCurrentLevel(float timeTaken, int livesLeft)
    {
        if (gameData == null) return;
        int nivelActual = UnityEngine.SceneManagement.SceneManager.GetActiveScene().buildIndex - 1;
        LevelData level = gameData.GetLevelData(nivelActual);
        if (level != null)
        {
            if (!level.completed || level.timeTaken > timeTaken)
            {
                CompleteLevel(nivelActual, timeTaken, livesLeft);
            }
        }
        else
        {
            Debug.LogWarning("Nivel no encontrado: " + nivelActual);
        }
    }
}