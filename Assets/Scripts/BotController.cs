using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BotController : MonoBehaviour
{
    public List<Transform> checkpoints;
    public float speed = 3.5f;
    private int currentCheckpointIndex = 0;
    private NavMeshAgent agent;

    public bool followPlayer;
    public Transform player;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        agent.speed = speed;
        MoveToNextCheckpoint();
    }

    void Update()
    {
        if (player != null)
        {
            if (Vector3.Distance(player.position, transform.position) < 10f && followPlayer)
            {
                agent.SetDestination(player.position);
            }
        }
    }

    public void MoveToNextCheckpoint()
    {
        if (checkpoints.Count == 0) return;
        agent.SetDestination(checkpoints[currentCheckpointIndex].position);
        currentCheckpointIndex = (currentCheckpointIndex + 1) % checkpoints.Count;
    }
}
