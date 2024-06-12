using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Bot : MonoBehaviour
{
    private NavMeshAgent agent;
    public GameObject target;
    private Drive ds;

    [Header("Settings")]
    public float wanderRadius = 10f;
    public float wanderDistance = 20f;
    public float wanderJitter = 1f;
    public float lookAheadMultiplier = 1.5f;
    public float fieldOfViewAngle = 60f;
    public float behaviourCooldownTime = 5f;

    private bool coolDown = false;
    private Vector3 wanderTarget = Vector3.zero;

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        ds = target.GetComponent<Drive>();
    }

    void Seek(Vector3 location)
    {
        agent.SetDestination(location);
    }

    void Flee(Vector3 location)
    {
        Vector3 fleeVector = location - transform.position;
        agent.SetDestination(transform.position - fleeVector);
    }

    void Pursue()
    {
        Vector3 targetDir = target.transform.position - transform.position;
        float relativeHeading = Vector3.Angle(transform.forward, transform.TransformVector(target.transform.forward));
        float toTarget = Vector3.Angle(transform.forward, transform.TransformVector(targetDir));

        if ((toTarget > fieldOfViewAngle && relativeHeading < 20) || ds.currentSpeed < 0.01f)
        {
            Seek(target.transform.position);
            return;
        }

        float lookAhead = targetDir.magnitude / (agent.speed + ds.currentSpeed) * lookAheadMultiplier;
        Seek(target.transform.position + target.transform.forward * lookAhead);
    }

    void Evade()
    {
        Vector3 targetDir = target.transform.position - transform.position;
        float lookAhead = targetDir.magnitude / (agent.speed + ds.currentSpeed) * lookAheadMultiplier;
        Flee(target.transform.position + target.transform.forward * lookAhead);
    }

    void Wander()
    {
        wanderTarget += new Vector3(Random.Range(-1.0f, 1.0f) * wanderJitter, 0, Random.Range(-1.0f, 1.0f) * wanderJitter);
        wanderTarget.Normalize();
        wanderTarget *= wanderRadius;

        Vector3 targetLocal = wanderTarget + new Vector3(0, 0, wanderDistance);
        Vector3 targetWorld = transform.TransformPoint(targetLocal);

        Seek(targetWorld);
    }

    void Hide()
    {
        Vector3 chosenSpot = Vector3.zero;
        float minDistance = Mathf.Infinity;

        foreach (var hideSpot in World.Instance.GetHidingSpots())
        {
            Vector3 hideDir = hideSpot.transform.position - target.transform.position;
            Vector3 hidePos = hideSpot.transform.position + hideDir.normalized * 10;

            float distance = Vector3.Distance(transform.position, hidePos);
            if (distance < minDistance)
            {
                chosenSpot = hidePos;
                minDistance = distance;
            }
        }

        Seek(chosenSpot);
    }

    void CleverHide()
    {
        Vector3 chosenSpot = Vector3.zero;
        Vector3 chosenDir = Vector3.zero;
        GameObject chosenGO = null;
        float minDistance = Mathf.Infinity;

        foreach (var hideSpot in World.Instance.GetHidingSpots())
        {
            Vector3 hideDir = hideSpot.transform.position - target.transform.position;
            Vector3 hidePos = hideSpot.transform.position + hideDir.normalized * 10;

            float distance = Vector3.Distance(transform.position, hidePos);
            if (distance < minDistance)
            {
                chosenSpot = hidePos;
                chosenDir = hideDir;
                chosenGO = hideSpot;
                minDistance = distance;
            }
        }

        if (chosenGO != null)
        {
            Collider hideCol = chosenGO.GetComponent<Collider>();
            Ray backRay = new Ray(chosenSpot, -chosenDir.normalized);
            if (hideCol.Raycast(backRay, out RaycastHit info, 100f))
            {
                Seek(info.point + chosenDir.normalized * 2);
            }
        }
    }

    bool CanSeeTarget()
    {
        Vector3 rayToTarget = target.transform.position - transform.position;
        if (Vector3.Angle(transform.forward, rayToTarget) < fieldOfViewAngle)
        {
            if (Physics.Raycast(transform.position, rayToTarget, out RaycastHit raycastInfo))
            {
                return raycastInfo.transform.gameObject.CompareTag("cop");
            }
        }
        return false;
    }

    bool CanSeeMe()
    {
        Vector3 rayToTarget = transform.position - target.transform.position;
        return Vector3.Angle(target.transform.forward, rayToTarget) < fieldOfViewAngle;
    }

    void BehaviourCooldown()
    {
        coolDown = false;
    }

    void Update()
    {
        if (!coolDown)
        {
            Debug.Log("Bot is evaluating situation");

            if (CanSeeTarget())
            {
                Debug.Log("Bot can see the cop, hiding...");
                HideMove();

            }
            else
            {
                if (CanSeeMe())
                {
                    Debug.Log("Bot cannot see the cop but cop can see the bot, hiding...");
                    HideMove();
                }
                else
                {
                    Debug.Log("Bot cannot see the cop and cop cannot see the bot, pursuing...");
                    Pursue();
                }
            }
        }
    }

    private void HideMove()
    {
        CleverHide();
        coolDown = true;
        Invoke(nameof(BehaviourCooldown), behaviourCooldownTime);
    }


    void OnDrawGizmos()
    {
        if (agent == null) return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, agent.stoppingDistance);

        Vector3 fovLine1 = Quaternion.AngleAxis(fieldOfViewAngle, transform.up) * transform.forward * agent.stoppingDistance;
        Vector3 fovLine2 = Quaternion.AngleAxis(-fieldOfViewAngle, transform.up) * transform.forward * agent.stoppingDistance;

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(transform.position, transform.position + fovLine1);
        Gizmos.DrawLine(transform.position, transform.position + fovLine2);

        if (target != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawLine(transform.position, target.transform.position);
        }
    }
}
