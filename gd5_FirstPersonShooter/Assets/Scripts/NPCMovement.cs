using UnityEngine;
using UnityEngine.AI;

public class NPCMovement : MonoBehaviour
{
    NavMeshAgent agent;
    Transform player;
    Vector3 currentDestination;
    [SerializeField] float followDistance = 10f;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        player = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void Update()
    {
        // agent.SetDestination(player.position);

        if (Vector3.Distance(transform.position, player.position) < followDistance)
        {
            Follow();
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
            agent.SetDestination(player.position);
        }
    }

    void Search()
    {
        if (Vector3.Distance(transform.position, currentDestination) < 5)
        {
            currentDestination = transform.position + new Vector3(Random.Range(-10, 10), 0, Random.Range(-10, 10));
            agent.SetDestination(currentDestination);
        }
    }
}
