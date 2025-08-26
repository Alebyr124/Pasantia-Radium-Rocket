using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class LevelMenuUI : MonoBehaviour
{
    public Button Level1Button;
    public Button Level2Button;
    public Button BackButton;

    private void Awake()
    {
        Level1Button.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(2);
        });

        Level2Button.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(3);
        });

        BackButton.onClick.AddListener(() =>
        {
            SceneManager.LoadScene(0);
        });
    }
}
