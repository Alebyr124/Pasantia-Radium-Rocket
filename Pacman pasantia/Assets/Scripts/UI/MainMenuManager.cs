using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class MainMenuScript : MonoBehaviour
{
    public Button PlayButton;
    public Button ExitButton;
    public Button logoutButton;

    private void Awake()
    {
        PlayButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(1);
        });

        ExitButton.onClick.AddListener(() =>
        {
            Application.Quit();
        });

        logoutButton.onClick.AddListener(() =>
        {
            PlayerPrefs.DeleteKey("PlayerName");
            PlayerPrefs.Save();
            SceneManager.LoadScene(1);
        });
    }
}