using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Helper : MonoBehaviour
{

    public static bool DestinationReached(BehaviourAI ai, float desiredDistance)
    {
        if (ai.agent.remainingDistance < desiredDistance)
        {
            return true;
        }
        return false;
    }

    public Collider GetClosestObstacle()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 100,  LayerMask.NameToLayer("Obstacle"));
        // Set closest to null
        Collider closest = null;
        // Set minValue to max value
        float minValue = float.MaxValue;
        // Loop through all entities
        foreach (var hit in hits)
        {
            // Set distance to entity distance
            float distance = Vector3.Distance(transform.position, hit.transform.position);
            // If distance < minValue
            if (distance < minValue)
            {
                // Set minValue to distance
                minValue = distance;
                // Set closest to entity
                closest = hit;
            }
        }
        // Return closest
        return closest;
    }

    public Vector3 GetAvoidanceWaypoint(Vector3 playerTarget)
    {
        Collider closest = GetClosestObstacle();
        Vector3 start = playerTarget;
        Vector3 end = closest.transform.position;
        Vector3 direction = end - start;
        Vector3 point = closest.ClosestPoint(start + direction * 2f);
        return point;
    }

    public Transform GetClosestTarget(List<Transform> targets)
    {
        float closestTargetDist = Mathf.Infinity;
        int transformIndex = 0;
        for (int index = 0; index < targets.Count; index++)
        {
            if (Vector3.Distance(transform.position, targets[index].position) < closestTargetDist)
            {
                closestTargetDist = Vector3.Distance(transform.position, targets[index].position);
                transformIndex = index;
            }
        }
        return targets[transformIndex];
    }

    //public void GetNearestTotem()
    //{
    //    InvulTotem[] totemPoles = FindObjectsOfType<InvulTotem>();
    //    float shortestDist = float.MaxValue;

    //    foreach (InvulTotem tp in totemPoles)
    //    {
    //        float thisDist = Vector3.Distance(transform.position, tp.transform.position);
    //        if (thisDist < shortestDist)
    //        {
    //            shortestDist = thisDist;
    //            totemPos = tp.transform.position;
    //        }
    //    }
    //}

}
