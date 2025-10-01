using System.Collections;
using System.Collections.Generic;
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
        Leve1Button.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(2);
        });

        Leve2Button.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(3);
        });

        BackButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(0);
        });

        //Data Partidas
        var saveSystem = FindObjectOfType<SaveSystem>();
        var data1 = saveSystem.GetLevelData(1);
        var data2 = saveSystem.GetLevelData(2);

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
            Level1DataText.text = $"Completado\nTiempo: {time.Minutes:D2}:{time.Seconds:D2}";
        }
        else
        {
            Level2DataText.text = "No completado";
        }
    }
}
