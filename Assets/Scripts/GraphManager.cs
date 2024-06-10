using System.Collections.Generic;
using UnityEngine;

public class GraphManager : MonoBehaviour
{
    public List<Transform> checkpoints;

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        foreach (Transform checkpoint in checkpoints)
        {
            Gizmos.DrawSphere(checkpoint.position, 0.5f);
            foreach (Transform neighbor in checkpoints)
            {
                if (checkpoint != neighbor)
                {
                    Gizmos.DrawLine(checkpoint.position, neighbor.position);
                }
            }
        }
    }
}
