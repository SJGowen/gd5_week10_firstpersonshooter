using UnityEngine;
using UnityEngine.AI;

public class NPCMovement : MonoBehaviour
{
    NavMeshAgent agent;
    Transform player;
    WaypointManager waypointManager;
    Animator animator;
    Vector3 currentDestination;
    [SerializeField] float followDistance = 10f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
        waypointManager = FindFirstObjectByType<WaypointManager>();
        animator = GetComponent<Animator>();
        if (waypointManager?.waypoints?.Length > 0)
        {
            currentDestination = waypointManager.waypoints[Random.Range(0, waypointManager.waypoints.Length)].position;
        }
        else
        {             
            currentDestination = GetRandomPositionOnMap();
        }
    }

    void Update()
    {
        // agent.SetDestination(player.position);

        if (Vector3.Distance(transform.position, player.position) < followDistance)
        {
            if (Vector3.Distance(transform.position, player.position) < 2f)
            {
                SetIdle();
                Attack();
            }
            else
            {
                Follow();
            }
        }
        else
        {
            Search();
        }
    }

    void Follow()
    {
        if (agent.isOnNavMesh)
        {
            if (Vector3.Distance(transform.position, player.position) < 2f)
            {
                SetIdle();
                Attack();
            }
            else
            {
                agent.SetDestination(player.position);
            }
        }
    }

    void SetIdle()
    {
        if (agent.isOnNavMesh)
        {
            agent.SetDestination(transform.position);
        }
    }

    void Attack()
    {
        // Play attack animation
        if (agent.isOnNavMesh)
        {
            animator.SetTrigger("Attack");
        }
    }

    void Search()
    {
        if (Vector3.Distance(transform.position, currentDestination) < 5)
        {
            //currentDestination = transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
            if (waypointManager?.waypoints?.Length > 0)
            {
                // Randomly select a new waypoint
                currentDestination = waypointManager.waypoints[Random.Range(0, waypointManager.waypoints.Length)].position;
            }
            else
            {
                // If no waypoints exist, just move to a random position within a range
                currentDestination = GetRandomPositionOnMap();
            }

            agent.SetDestination(currentDestination);
        }
    }

    private Vector3 GetRandomPositionOnMap()
    {
        Vector3 randomPosition = transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
        while (!NavMesh.SamplePosition(randomPosition, out var hit, 12f, NavMesh.AllAreas))
        {
            randomPosition = transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
        }

        return randomPosition;
    }

    public void DealDamage(int damage)
    {
        if (Vector3.Distance(transform.position, player.position) < 2.5f)
        {
            player.GetComponent<FPSController>().TakeDamage(damage);
        }
    }
}
