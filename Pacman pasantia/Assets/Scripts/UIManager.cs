using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    public static UIManager inst;
    public GameObject WinScreen;

    public AudioSource GameplayMusic;
    public AudioSource WinMusic;

    public TextMeshProUGUI TimeGameplay;
    public TextMeshProUGUI TimeWin;
    public TextMeshProUGUI PuntuacionGameplay;
    public Button RestartButton;

    public bool Win = false;

    private float TimeSeconds;
    private int TimeMinutes;

    void Awake()
    {
        inst = this;

        RestartButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });
    }

    private void Update()
    {
        if (!Win)
        {
            TimeSeconds += Time.deltaTime;
            if (TimeSeconds >= 59f)
            {
                TimeSeconds = 0f;
                TimeMinutes++;
            }

            if (TimeSeconds <= 9)
                TimeGameplay.text = "Tiempo: " + TimeMinutes + ":0" + Mathf.Ceil(TimeSeconds);
            else
                TimeGameplay.text = "Tiempo: " + TimeMinutes + ":" + Mathf.Ceil(TimeSeconds);
        }
    }

    public void ShowWinScreen()
    {
        Win = true;
        WinScreen.SetActive(true);
        if (TimeSeconds <= 9)
            TimeWin.text = "Tu tiempo fue de " + TimeMinutes + ":0" + Mathf.Ceil(TimeSeconds);
        else
            TimeWin.text = "Tu tiempo fue de " + TimeMinutes + ":" + Mathf.Ceil(TimeSeconds);
        TimeGameplay.gameObject.SetActive(false);
        PuntuacionGameplay.gameObject.SetActive(false);
        GameplayMusic.Pause();
        WinMusic.Play();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}
