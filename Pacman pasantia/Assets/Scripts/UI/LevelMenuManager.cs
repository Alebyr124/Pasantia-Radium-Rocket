using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class LevelMenuScript : MonoBehaviour
{
    public Button Leve1Button;
    public Button Leve2Button;
    public Button BackButton;
    public TextMeshProUGUI Level1DataText;
    public TextMeshProUGUI Level2DataText;

    private void Awake()
    {
        Leve1Button.onClick.AddListener(() => SceneManager.LoadScene(2));
        Leve2Button.onClick.AddListener(() => SceneManager.LoadScene(3));
        BackButton.onClick.AddListener(() => SceneManager.LoadScene(0));
    }

    private IEnumerator Start()
    {
        // Esperar hasta que FirebaseSaveSystem haya cargado los datos
        while (FirebaseSaveSystem.Instance == null || !FirebaseSaveSystem.Instance.isLoaded)
        {
            yield return null;
        }

        var data = FirebaseSaveSystem.Instance.gameData;
        if (data == null)
        {
            Debug.LogWarning("GameData es null incluso después de cargar Firebase");
            yield break;
        }

        var data1 = data.GetLevelData(1);
        var data2 = data.GetLevelData(2);

        if (data1 != null && data1.completed)
        {
            System.TimeSpan time = System.TimeSpan.FromSeconds(data1.timeTaken);
            Level1DataText.text = $"Completado\nTiempo: {time.Minutes:D2}:{time.Seconds:D2}";
        }
        else
        {
            Level1DataText.text = "No completado";
        }

        if (data2 != null && data2.completed)
        {
            System.TimeSpan time = System.TimeSpan.FromSeconds(data2.timeTaken);
            Level2DataText.text = $"Completado\nTiempo: {time.Minutes:D2}:{time.Seconds:D2}";
        }
        else
        {
            Level2DataText.text = "No completado";
        }
    }
}
