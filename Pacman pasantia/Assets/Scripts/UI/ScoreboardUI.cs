using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ScoreboardUI : MonoBehaviour
{
    public TMP_Text scoreboardText;
    private int currentLevel = 1;

    private SaveSystem save;

    void Start()
    {
        save = FindObjectOfType<SaveSystem>();
        if (save == null)
        {
            Debug.LogError("No se encontró SaveSystem en la escena");
            return;
        }

        LoadLevel(currentLevel);
    }

    // Método público para cambiar de nivel
    public void LoadLevel(int level)
    {
        currentLevel = level;
        save.LoadScoreboard(currentLevel, OnScoreboardLoaded);
    }

    void OnScoreboardLoaded(List<(string playerName, float time)> scores)
    {
        if (scores.Count == 0)
        {
            scoreboardText.text = $"Nivel {currentLevel}\nAún no hay registros";
            return;
        }

        scoreboardText.text = $"Nivel {currentLevel}\nMejores Tiempos\n";

        int rank = 1;
        foreach (var score in scores)
        {
            System.TimeSpan time = System.TimeSpan.FromSeconds(score.time);
            scoreboardText.text += $"{rank}. {score.playerName} - {time.Minutes:D2}:{time.Seconds:D2}\n";
            rank++;
        }
    }
}
