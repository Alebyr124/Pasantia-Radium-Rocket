using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelMenuScript : MonoBehaviour
{
    public Button Level1Button;
    public Button Level2Button;
    public Button BackButton;
    public TextMeshProUGUI Level1DataText;
    public TextMeshProUGUI Level2DataText;
    public TextMeshProUGUI UsernameText; // <-- Nuevo: mostrar nombre

    private void Awake()
    {
        Level1Button.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(3);
        });

        Level2Button.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(4);
        });

        BackButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(0);
        });

        // ---------- Mostrar nombre de usuario ----------
        string playerName = PlayerPrefs.GetString("PlayerName", "JugadorDesconocido");
        UsernameText.text = $"Logueado como: {playerName}";

        // ---------- Cargar datos desde Firebase ----------
        var saveSystem = FindObjectOfType<SaveSystem>();

        // Nivel 1
        saveSystem.LoadFromFirebase(1, (data1) =>
        {
            if (data1 != null && data1.completed)
            {
                System.TimeSpan time = System.TimeSpan.FromSeconds(data1.timeTaken);
                Level1DataText.text = $"Completado\nTiempo: {time.Minutes:D2}:{time.Seconds:D2}";
            }
            else
            {
                Level1DataText.text = "No completado";
            }
        });

        // Nivel 2
        saveSystem.LoadFromFirebase(2, (data2) =>
        {
            if (data2 != null && data2.completed)
            {
                System.TimeSpan time = System.TimeSpan.FromSeconds(data2.timeTaken);
                Level2DataText.text = $"Completado\nTiempo: {time.Minutes:D2}:{time.Seconds:D2}";
            }
            else
            {
                Level2DataText.text = "No completado";
            }
        });
    }
}
