using UnityEngine;
using TMPro;

public class DropdownLevelSelector : MonoBehaviour
{
    public TMP_Dropdown dropdown;
    public ScoreboardUI scoreboard;

    void Start()
    {
        if (dropdown == null || scoreboard == null)
        {
            Debug.LogError("Dropdown o Scoreboard no asignado");
            return;
        }

        dropdown.onValueChanged.AddListener(OnDropdownChanged);

        // Inicializa al nivel seleccionado por defecto
        OnDropdownChanged(dropdown.value);
    }

    void OnDropdownChanged(int value)
    {
        int level = value + 1; // Si los niveles empiezan en 1
        scoreboard.LoadLevel(level);
    }
}
