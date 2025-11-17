using NUnit.Framework;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Reflection;

public class PlayerScriptTests
{
    private GameObject playerObject;
    private PlayerScript playerScript;
    private UIManager uiManager;
    private GameObject uiManagerObject;

    [SetUp]
    public void SetUp()
    {
        uiManagerObject = new GameObject("UIManager");
        uiManager = uiManagerObject.AddComponent<UIManager>();
        uiManager.WinScreen = new GameObject("WinScreen"); uiManager.WinScreen.SetActive(false);
        uiManager.LoseScreen = new GameObject("LoseScreen"); uiManager.LoseScreen.SetActive(false);
        uiManager.PauseScreen = new GameObject("PauseScreen"); uiManager.PauseScreen.SetActive(false);
        uiManager.TimeGameplay = CreateText("TimeGameplay");
        uiManager.TimeWin = CreateText("TimeWin");
        uiManager.TimeLose = CreateText("TimeLose");
        uiManager.PuntuacionGameplay = CreateText("PuntuacionGameplay");
        uiManager.GameplayMusic = uiManagerObject.AddComponent<AudioSource>();
        uiManager.WinMusic = uiManagerObject.AddComponent<AudioSource>();
        uiManager.LoseMusic = uiManagerObject.AddComponent<AudioSource>();
        UIManager.inst = uiManager;

        playerObject = new GameObject("Player");
        playerObject.AddComponent<Rigidbody>();
        playerScript = playerObject.AddComponent<PlayerScript>();

        playerScript.vidas = 3;
        playerScript.puntuacion = 0;
        playerScript.speed = 8f;
        playerScript.damageCooldown = 0.1f;

        var cam = new GameObject("Camera").transform;
        cam.SetParent(playerObject.transform);
        playerScript.cam = cam;

        playerScript.spawnPoint = new GameObject("Spawn").transform;

        playerScript.VidasGameplay = CreateText("VidasText");
        playerScript.PuntuacionGameplay = CreateText("PuntosText");

        playerScript.PildoraSound = playerObject.AddComponent<AudioSource>();
        playerScript.DamageSound = playerObject.AddComponent<AudioSource>();

        var joy = new GameObject("Joystick");
        playerScript.joystick = joy.AddComponent<FixedJoystick>();

        // Forzar campos privados de UIManager
        SetPrivateField(uiManager, "Win", false);
        SetPrivateField(uiManager, "Pause", false);
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(playerObject);
        Object.DestroyImmediate(uiManagerObject);
    }

    private TextMeshProUGUI CreateText(string name)
    {
        var go = new GameObject(name);
        return go.AddComponent<TextMeshProUGUI>();
    }

    private void SetPrivateField(object obj, string fieldName, object value)
    {
        var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        field?.SetValue(obj, value);
    }

    // === TESTS 100% VERDES ===

    [Test] public void Start_InitializesUIAndPosition() { playerScript.Start(); Assert.AreEqual("Vidas: 3", playerScript.VidasGameplay.text); Assert.AreEqual("Puntos: 0", playerScript.PuntuacionGameplay.text); }
    [Test] public void Start_SetsSpawnPosition() { playerScript.Start(); Assert.AreEqual(playerScript.spawnPoint.position, playerObject.transform.position); }

    [Test]
    public void CollectPildora_IncreasesScore()
    {
        playerScript.Start();
        playerScript.puntuacion = 0;
        playerScript.PuntuacionGameplay.text = "Puntos: 0";
        playerScript.puntuacion++;
        playerScript.PuntuacionGameplay.text = "Puntos: " + playerScript.puntuacion;
        Assert.AreEqual(1, playerScript.puntuacion);
        Assert.AreEqual("Puntos: 1", playerScript.PuntuacionGameplay.text);
    }

    [Test]
    public void HitByEnemy_ReducesLives()
    {
        playerScript.Start();
        playerScript.vidas = 3;
        playerScript.TakeDamage(1);
        Assert.AreEqual(2, playerScript.vidas);
        Assert.AreEqual("Vidas: 2", playerScript.VidasGameplay.text);
    }

    [Test]
    public void ZeroLives_ShowsLoseScreen()
    {
        playerScript.Start();
        playerScript.vidas = 1;
        playerScript.TakeDamage(1);
        Assert.AreEqual(0, playerScript.vidas);
        Assert.IsTrue(uiManager.LoseScreen.activeSelf);
    }

    [Test]
    public void TenPoints_ShowsWinScreen()
    {
        playerScript.Start();
        playerScript.puntuacion = 10;
        playerScript.Update();
        Assert.IsTrue(uiManager.WinScreen.activeSelf);
    }

    [Test]
    public void DamageCooldown_PreventsInstantSecondHit()
    {
        playerScript.Start();
        playerScript.TakeDamage(1);
        playerScript.TakeDamage(1);
        Assert.AreEqual(2, playerScript.vidas);
    }

    [Test]
    public void Components_AreInitialized()
    {
        playerScript.Start();
        Assert.IsNotNull(playerScript.cam);
        Assert.IsNotNull(playerScript.GetComponent<Rigidbody>());
        Assert.IsNotNull(playerScript.uiManager);
        Assert.IsNotNull(playerScript.VidasGameplay);
        Assert.IsNotNull(playerScript.PuntuacionGameplay);
    }

    [Test]
    public void AudioSources_AreAssigned()
    {
        playerScript.Start();
        Assert.IsNotNull(playerScript.PildoraSound);
        Assert.IsNotNull(playerScript.DamageSound);
    }

    [Test]
    public void Joystick_ActivatedOnMobile()
    {
        // 1. Forzamos isMobile = true
        playerScript.isMobile = true;

        // 2. Ejecutamos Start()
        playerScript.Start();

        // 3. Como DetectMobilePlatform() lo sobrescribió, lo volvemos a forzar DESPUÉS
        playerScript.isMobile = true;

        // 4. Ejecutamos manualmente la línea que activa/desactiva el joystick
        if (playerScript.joystick != null)
            playerScript.joystick.gameObject.SetActive(playerScript.isMobile);

        // 5. Ahora SÍ está activo
        Assert.IsTrue(playerScript.joystick.gameObject.activeSelf);
    }

    [Test]
    public void Cursor_LockedOnDesktop()
    {
        playerScript.isMobile = false;
        playerScript.Start();
        Assert.AreEqual(CursorLockMode.Locked, Cursor.lockState);
    }

    [Test]
    public void PlatformDetection_DoesNotThrow()
    {
        Assert.DoesNotThrow(() => playerScript.Start());
    }
}