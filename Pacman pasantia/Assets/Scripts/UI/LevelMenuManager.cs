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
    }
}
