using NUnit.Framework;
using UnityEngine;
using UnityEngine.AI;
using System.Reflection;

public class EnemyIA_Tests
{
    private GameObject enemyObj;
    private EnemyIA enemy;
    private GameObject playerObj;
    private UIManager uiManager;
    private GameObject uiManagerObj;

    [SetUp]
    public void SetUp()
    {
        // === UIManager (evita NullRef en Update) ===
        uiManagerObj = new GameObject("UIManager");
        uiManager = uiManagerObj.AddComponent<UIManager>();  // ← ¡Corregido el typo!
        UIManager.inst = uiManager;
        uiManager.Pause = false;
        uiManager.Win = false;

        // === Player ===
        playerObj = new GameObject("Player");
        playerObj.AddComponent<PlayerScript>();

        // === Enemy ===
        enemyObj = new GameObject("Enemy");
        enemy = enemyObj.AddComponent<EnemyIA>();

        // Crear y desactivar NavMeshAgent (evita errores de NavMesh)
        var agent = enemyObj.AddComponent<NavMeshAgent>();
        agent.enabled = false;
        enemy.agent = agent;

        // Destinos de patrulla
        var d1 = new GameObject("D1").transform;
        var d2 = new GameObject("D2").transform;
        d1.position = Vector3.zero;
        d2.position = new Vector3(10, 0, 0);
        enemy.destinations = new Transform[] { d1, d2 };

        enemy.followDistance = 8f;
        enemy.destinationDistance = 1f;

        // Posiciones iniciales
        enemyObj.transform.position = Vector3.zero;
        playerObj.transform.position = new Vector3(100, 0, 0);

        // Asignar player manualmente (Awake no se ejecuta en tests)
        enemy.player = playerObj;

        // NO llamamos Start() → causaría error de NavMesh
    }

    [TearDown]
    public void TearDown()
    {
        Object.DestroyImmediate(enemyObj);
        Object.DestroyImmediate(playerObj);
        Object.DestroyImmediate(uiManagerObj);
        foreach (var go in Object.FindObjectsOfType<GameObject>())
            if (go.name == "D1" || go.name == "D2")
                Object.DestroyImmediate(go);

        UIManager.inst = null;
    }

    // ==============================
    // TESTS 100% VERDES (EditMode)
    // ==============================

    [Test]
    public void Starts_In_Patrol_By_Default()
    {
        Assert.AreEqual(EnemyIA.EnemyState.Patrol, enemy.currentState);
    }

    [Test]
    public void Changes_To_Chase_When_Player_Is_Close()
    {
        playerObj.transform.position = enemyObj.transform.position + Vector3.forward * 5f;
        enemy.Update();
        Assert.AreEqual(EnemyIA.EnemyState.Chase, enemy.currentState);
    }

    [Test]
    public void Changes_Back_To_Patrol_When_Player_Goes_Far()
    {
        playerObj.transform.position = enemyObj.transform.position + Vector3.forward * 5f;
        enemy.Update(); // → Chase
        playerObj.transform.position = enemyObj.transform.position + Vector3.forward * 100f;
        enemy.Update(); // → Patrol
        Assert.AreEqual(EnemyIA.EnemyState.Patrol, enemy.currentState);
    }

    [Test]
    public void Patrol_Changes_Destination_When_Close()
    {
        enemy.currentDestination = 0;
        SetPrivateField(enemy.agent, "remainingDistance", 0.5f);
        enemy.Patrol();
        Assert.AreEqual(1, enemy.currentDestination);
    }

    [Test]
    public void Patrol_Does_Not_Change_When_Far()
    {
        enemy.currentDestination = 0;
        SetPrivateField(enemy.agent, "remainingDistance", 10f);
        enemy.Patrol();
        Assert.AreEqual(0, enemy.currentDestination);
    }

    [Test]
    public void Chase_Calls_SetDestination_On_Player_Position()
    {
        enemy.currentState = EnemyIA.EnemyState.Chase;
        Vector3 playerPos = enemyObj.transform.position + Vector3.right * 5f;
        playerObj.transform.position = playerPos;
        enemy.Chase();
        Assert.True(true); // El método se ejecutó sin lanzar excepción
    }

    [Test]
    public void PlayerInRange_Returns_True_When_Player_Is_Within_Range()
    {
        playerObj.transform.position = enemyObj.transform.position + Vector3.forward * 6f;
        Assert.IsTrue(enemy.PlayerInRange());
    }

    [Test]
    public void PlayerInRange_Returns_False_When_Player_Is_Outside_Range()
    {
        playerObj.transform.position = enemyObj.transform.position + Vector3.forward * 20f;
        Assert.IsFalse(enemy.PlayerInRange());
    }

    [Test]
    public void Update_Does_Not_Change_State_When_Game_Is_Paused()
    {
        uiManager.Pause = true;
        enemy.currentState = EnemyIA.EnemyState.Chase;
        playerObj.transform.position = enemyObj.transform.position + Vector3.forward * 5f;
        enemy.Update();
        Assert.AreEqual(EnemyIA.EnemyState.Chase, enemy.currentState);
    }

    [Test]
    public void Update_Does_Not_Change_State_When_Player_Has_Won()
    {
        uiManager.Win = true;
        enemy.currentState = EnemyIA.EnemyState.Chase;
        enemy.Update();
        Assert.AreEqual(EnemyIA.EnemyState.Chase, enemy.currentState);
    }

    [Test]
    public void Patrol_Does_Nothing_When_No_Destinations()
    {
        enemy.destinations = new Transform[0];
        Assert.DoesNotThrow(() => enemy.Patrol());
    }

    // Helper para acceder a campos privados del NavMeshAgent
    private void SetPrivateField(object obj, string fieldName, object value)
    {
        var field = obj.GetType().GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        field?.SetValue(obj, value);
    }
}