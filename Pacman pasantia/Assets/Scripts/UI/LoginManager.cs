using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Firebase.Database;
using Firebase.Extensions;

public class LoginManager : MonoBehaviour
{
    [Header("Referencias UI")]
    public InputField playerNameInput;
    public Button loginButton;
    public Text feedbackText;

    private DatabaseReference dbRef;

    void Start()
    {
        // Si ya hay un nombre guardado, saltamos el login
        if (PlayerPrefs.HasKey("PlayerName"))
        {
            string savedName = PlayerPrefs.GetString("PlayerName");
            Debug.Log($"Jugador ya logueado como {savedName}");
            SceneManager.LoadScene("LevelMenu");
            return;
        }

        // Esperamos que Firebase se haya inicializado
        dbRef = FirebaseInit.DBreference;
        if (dbRef == null)
            Debug.LogWarning("FirebaseInit.DBreference aún no está listo, se inicializará luego.");

        loginButton.onClick.AddListener(OnLoginClicked);
    }

    void OnLoginClicked()
    {
        string playerName = playerNameInput.text.Trim();

        if (string.IsNullOrEmpty(playerName))
        {
            feedbackText.text = "Por favor, ingresá un nombre.";
            return;
        }

        feedbackText.text = "Iniciando sesión...";

        // Guardamos el nombre localmente
        PlayerPrefs.SetString("PlayerName", playerName);
        PlayerPrefs.Save();

        // Si Firebase ya está listo, registramos el usuario
        if (FirebaseInit.DBreference != null)
        {
            dbRef = FirebaseInit.DBreference;
            RegisterPlayerInFirebase(playerName);
        }

        // Pasamos al menú de niveles
        SceneManager.LoadScene("LevelMenu");
    }

    private void RegisterPlayerInFirebase(string playerName)
    {
        dbRef.Child("saves").Child(playerName).GetValueAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Error al verificar jugador: " + task.Exception);
                return;
            }

            if (!task.Result.Exists)
            {
                // Crear nodo del jugador si no existe
                dbRef.Child("saves").Child(playerName).Child("info").SetValueAsync("Jugador nuevo");
                Debug.Log("Jugador registrado en Firebase: " + playerName);
            }
            else
            {
                Debug.Log("Jugador existente: " + playerName);
            }
        });
    }
}
