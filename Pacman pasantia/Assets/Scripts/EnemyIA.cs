using UnityEngine;
using UnityEngine.AI;

public class EnemyIA : MonoBehaviour
{
    public enum EnemyState { Patrol, Chase }
    private EnemyState currentState = EnemyState.Patrol;

    [Header("NavMesh")]
    public NavMeshAgent agent;
    public Transform[] destinations;
    private int currentDestination = 0;

    [Header("Player")]
    private GameObject player;
    public float followDistance = 8f;

    [Header("Patrulla")]
    public float destinationDistance = 3f;

    private void Awake()
    {
        player = FindObjectOfType<PlayerScript>().gameObject;
    }

    private void Start()
    {
        if (destinations.Length > 0)
            agent.SetDestination(destinations[0].position);
    }

    private void Update()
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

    private void Patrol()
    {
        if (Vector3.Distance(transform.position, agent.destination) < destinationDistance)
        {
            currentDestination = (currentDestination + 1) % destinations.Length;
            agent.SetDestination(destinations[currentDestination].position);
        }
    }

    private void Chase()
    {
        if (Vector3.Distance(agent.destination, player.transform.position) > 1f)
        {
            agent.SetDestination(player.transform.position);
        }
    }

    private bool PlayerInRange()
    {
        return Vector3.Distance(transform.position, player.transform.position) < followDistance;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, followDistance);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, destinationDistance);
    }
}
