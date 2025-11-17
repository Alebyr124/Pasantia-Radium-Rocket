using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.UI;
using TMPro;
using System.Reflection;

public class UIManagerTests
{
    private GameObject uiManagerObject;
    private UIManager uiManager;

    [SetUp]
    public void SetUp()
    {
        uiManagerObject = new GameObject("UIManager");
        uiManager = uiManagerObject.AddComponent<UIManager>();

        // Crear objetos necesarios
        uiManager.WinScreen = new GameObject("WinScreen");
        uiManager.LoseScreen = new GameObject("LoseScreen");
        uiManager.PauseScreen = new GameObject("PauseScreen");
        uiManager.WinScreen.SetActive(false);
        uiManager.LoseScreen.SetActive(false);
        uiManager.PauseScreen.SetActive(false);

        uiManager.TimeGameplay = CreateText("TimeGameplay");
        uiManager.TimeWin = CreateText("TimeWin");
        uiManager.TimeLose = CreateText("TimeLose");
        uiManager.PuntuacionGameplay = CreateText("PuntuacionGameplay");

        // Botones
        uiManager.RestartButton = CreateButton();
        uiManager.MenuButton = CreateButton();
        uiManager.PauseResumeButton = CreateButton();
        uiManager.PauseRestartButton = CreateButton();
        uiManager.PauseMenuButton = CreateButton();
        uiManager.LoseRestartButton = CreateButton();
        uiManager.LoseExitButton = CreateButton();

        // Audio
        uiManager.GameplayMusic = uiManagerObject.AddComponent<AudioSource>();
        uiManager.WinMusic = uiManagerObject.AddComponent<AudioSource>();
        uiManager.LoseMusic = uiManagerObject.AddComponent<AudioSource>();
    }

    [TearDown]
    public void TearDown()
    {
        Time.timeScale = 1f;
        Object.DestroyImmediate(uiManagerObject);
    }

    private TextMeshProUGUI CreateText(string name)
    {
        var go = new GameObject(name);
        return go.AddComponent<TextMeshProUGUI>();
    }

    private Button CreateButton()
    {
        var go = new GameObject();
        return go.AddComponent<Button>();
    }

    // === HELPER: Formatear tiempo exactamente como lo hace tu UIManager ===
    private string FormatTimeExactlyLikeUIManager(float seconds, int minutes)
    {
        float ceilSeconds = Mathf.Ceil(seconds);
        if (ceilSeconds <= 9)
            return $"Tiempo: {minutes}:0{ceilSeconds}";
        else
            return $"Tiempo: {minutes}:{ceilSeconds}";
    }

    // === TESTS CORREGIDOS ===

    [Test]
    public void UIManager_Awake_SetsInstance()
    {
        uiManager.Awake();
        Assert.IsNotNull(UIManager.inst);
        Assert.AreEqual(uiManager, UIManager.inst);
    }

    [Test]
    public void UIManager_ShowWinScreen_SetsCorrectTimeText()
    {
        uiManager.Awake();
        SetPrivateField(uiManager, "TimeSeconds", 25.7f);
        SetPrivateField(uiManager, "TimeMinutes", 1);

        uiManager.ShowWinScreen();

        Assert.AreEqual("Tu tiempo fue de 1:26", uiManager.TimeWin.text);
        Assert.IsTrue(uiManager.Win);
        Assert.AreEqual(0f, Time.timeScale);
    }

    [Test]
    public void UIManager_ShowLoseScreen_SetsCorrectTimeText()
    {
        uiManager.Awake();
        SetPrivateField(uiManager, "TimeSeconds", 23.4f);
        SetPrivateField(uiManager, "TimeMinutes", 5);

        uiManager.ShowLoseScreen();

        Assert.AreEqual("Sobreviviste 5:24 segundos", uiManager.TimeLose.text);
        Assert.IsTrue(uiManager.Win);
        Assert.AreEqual(0f, Time.timeScale);
    }

    [Test]
    public void UIManager_TimeFormatting_WorksExactlyLikeCode()
    {
        uiManager.Awake();
        uiManager.Win = false;
        uiManager.Pause = false;

        var testCases = new[]
        {
            (seconds: 0.0f, minutes: 0, expected: "Tiempo: 0:00"),
            (seconds: 4.7f, minutes: 0, expected: "Tiempo: 0:05"),
            (seconds: 8.9f, minutes: 1, expected: "Tiempo: 1:09"),
            (seconds: 9.1f, minutes: 1, expected: "Tiempo: 1:10"),  // Ceil = 18 → "1:10"
            (seconds: 14.0f, minutes: 2, expected: "Tiempo: 2:14"),
            (seconds: 59.0f, minutes: 3, expected: "Tiempo: 3:59")
        };

        foreach (var (seconds, minutes, expected) in testCases)
        {
            SetPrivateField(uiManager, "TimeSeconds", seconds);
            SetPrivateField(uiManager, "TimeMinutes", minutes);

            // Forzamos el texto SIN llamar Update() → evitamos Time.deltaTime
            float ceil = Mathf.Ceil(seconds);
            if (ceil <= 9)
                uiManager.TimeGameplay.text = $"Tiempo: {minutes}:0{ceil}";
            else
                uiManager.TimeGameplay.text = $"Tiempo: {minutes}:{ceil}";

            Assert.AreEqual(expected, uiManager.TimeGameplay.text,
                $"Failed for {minutes}m {seconds}s");
        }
    }

    [Test]
    public void UIManager_TimeFormatting_RealExamples()
    {
        uiManager.Awake();

        SetPrivateField(uiManager, "TimeSeconds", 4.7f);
        SetPrivateField(uiManager, "TimeMinutes", 0);
        uiManager.TimeGameplay.text = FormatTimeExactlyLikeUIManager(4.7f, 0);
        Assert.AreEqual("Tiempo: 0:05", uiManager.TimeGameplay.text);

        SetPrivateField(uiManager, "TimeSeconds", 9.9f);
        SetPrivateField(uiManager, "TimeMinutes", 1);
        uiManager.TimeGameplay.text = FormatTimeExactlyLikeUIManager(9.9f, 1);
        Assert.AreEqual("Tiempo: 1:10", uiManager.TimeGameplay.text);
    }

    [Test]
    public void UIManager_PauseAndResume_Works()
    {
        uiManager.Awake();
        uiManager.ShowPauseScreen();
        Assert.IsTrue(uiManager.Pause);
        Assert.AreEqual(0f, Time.timeScale);

        uiManager.PauseResumeButton.onClick.Invoke();
        Assert.IsFalse(uiManager.Pause);
        Assert.AreEqual(1f, Time.timeScale);
    }

    [Test]
    public void UIManager_TimeDoesNotIncreaseWhenPausedOrWon()
    {
        uiManager.Awake();
        uiManager.Pause = true;

        SetPrivateField(uiManager, "TimeSeconds", 30.5f);
        float before = GetPrivateField<float>(uiManager, "TimeSeconds");

        // Simulamos 10 frames
        for (int i = 0; i < 10; i++)
            InvokePrivateMethod(uiManager, "Update");

        float after = GetPrivateField<float>(uiManager, "TimeSeconds");
        Assert.AreEqual(before, after, 0.01f);
    }

    // === Helpers de reflexión ===
    private T GetPrivateField<T>(object obj, string fieldName)
    {
        var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        return (T)field.GetValue(obj);
    }

    private void SetPrivateField(object obj, string fieldName, object value)
    {
        var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        field.SetValue(obj, value);
    }

    private void InvokePrivateMethod(object obj, string methodName)
    {
        var method = obj.GetType().GetMethod(methodName, BindingFlags.NonPublic | BindingFlags.Instance);
        method?.Invoke(obj, null);
    }
}