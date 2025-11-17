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
    public GameObject LoseScreen;
    public GameObject PauseScreen;

    public bool Pause;

    public AudioSource GameplayMusic;
    public AudioSource WinMusic;
    public AudioSource LoseMusic;

    public TextMeshProUGUI TimeGameplay;
    public TextMeshProUGUI TimeWin;
    public TextMeshProUGUI PuntuacionGameplay;
    public TextMeshProUGUI TimeLose;

    public Button RestartButton;
    public Button MenuButton;
    public Button PauseResumeButton;
    public Button PauseRestartButton;
    public Button PauseMenuButton;
    public Button LoseRestartButton;
    public Button LoseExitButton;

    public bool Win = false;

    private float TimeSeconds;
    private int TimeMinutes;

    // Referencias guardadas para evitar buscarlas múltiples veces
    private PlayerScript playerScript;
  
    public void Awake()
    {
        inst = this;

        // Buscar referencias al inicio
        playerScript = FindObjectOfType<PlayerScript>();
    

        RestartButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });

        MenuButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(1);
        });

        PauseRestartButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });

        PauseMenuButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(1);
        });

        PauseResumeButton.onClick.AddListener(() =>
        {
            Pause = false;
            Time.timeScale = 1;
            PauseScreen.SetActive(false);
            TimeGameplay.gameObject.SetActive(true);
            PuntuacionGameplay.gameObject.SetActive(true);
            GameplayMusic.Play();
            Cursor.lockState = CursorLockMode.Locked;
            Cursor.visible = false;
        });

        LoseRestartButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        });

        LoseExitButton.onClick.AddListener(() =>
        {
            Time.timeScale = 1;
            SceneManager.LoadScene(1);
        });
    }

    private void Update()
    {
        if (!Win && !Pause)
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
        Time.timeScale = 0;
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

        // Guardar con verificación de nulos
        if (playerScript != null)
        {
            // Asumiendo que el nivel actual es 1, ajusta según tu lógica
            int nivelActual = 1; 
            FirebaseSaveSystem.Instance.CompleteLevel(
                nivelActual,
                (TimeMinutes * 60) + Mathf.Ceil(TimeSeconds),
                playerScript.vidas
            );
        }

        else
        {
            Debug.LogWarning("No se pudo guardar el progreso: " +
                (playerScript == null ? "PlayerScript no encontrado." : ""));
        }
    }

    public void ShowLoseScreen()
    {
        Win = true;
        Time.timeScale = 0;
        LoseScreen.SetActive(true);

        if (TimeSeconds <= 9)
            TimeLose.text = "Sobreviviste " + TimeMinutes + ":0" + Mathf.Ceil(TimeSeconds) + " segundos";
        else
            TimeLose.text = "Sobreviviste " + TimeMinutes + ":" + Mathf.Ceil(TimeSeconds) + " segundos";

        TimeGameplay.gameObject.SetActive(false);
        PuntuacionGameplay.gameObject.SetActive(false);
        GameplayMusic.Pause();
        LoseMusic.Play();

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void ShowPauseScreen()
    {
        Pause = true;
        Time.timeScale = 0;
        PauseScreen.SetActive(true);
        TimeGameplay.gameObject.SetActive(false);
        PuntuacionGameplay.gameObject.SetActive(false);
        GameplayMusic.Pause();
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }
}