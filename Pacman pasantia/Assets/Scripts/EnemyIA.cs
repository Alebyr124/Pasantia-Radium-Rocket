using UnityEngine;
using UnityEngine.AI;

public class EnemyIA : MonoBehaviour
{
    public enum EnemyState { Patrol, Chase }
    public EnemyState currentState = EnemyState.Patrol;

    [Header("NavMesh")]
    public NavMeshAgent agent;
    public Transform[] destinations;
    public int currentDestination = 0;

    [Header("Player")]
    public GameObject player;
    public float followDistance = 8f;

    [Header("Patrulla")]
    public float destinationDistance = 3f;

    public void Awake()
    {
        player = FindObjectOfType<PlayerScript>().gameObject;
    }

    public void Start()
    {
        if (destinations.Length > 0)
            agent.SetDestination(destinations[0].position);
    }

    public void Update()
    {
        if (!UIManager.inst.Pause && !UIManager.inst.Win)
        {
            switch (currentState)
            {
                case EnemyState.Patrol:
                    Patrol();
                    if (PlayerInRange())
                        currentState = EnemyState.Chase;
                    break;

                case EnemyState.Chase:
                    Chase();
                    if (!PlayerInRange())
                        currentState = EnemyState.Patrol;
                    break;
            }
        }
    }

    public void Patrol()
    {
        if (destinations.Length == 0) return;

        if (!agent.pathPending && agent.remainingDistance < destinationDistance)
        {
            currentDestination = (currentDestination + 1) % destinations.Length;
            agent.SetDestination(destinations[currentDestination].position);
        }
    }

    public void Chase()
    {
        if (Vector3.Distance(agent.destination, player.transform.position) > 1f)
        {
            agent.SetDestination(player.transform.position);
        }
    }

    public bool PlayerInRange()
    {
        return Vector3.Distance(agent.transform.position, player.transform.position) < followDistance;
    }

    public void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, followDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, destinationDistance);
    }
}
