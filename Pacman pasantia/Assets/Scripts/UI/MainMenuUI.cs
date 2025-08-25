using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;


public class MainMenuUI : MonoBehaviour
{
    public Button PlayButton;
    public Button ExitButton;

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
    }
}
