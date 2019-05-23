using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Naive : BehaviourAI
{
    private void OnEnable()
    {
        agent.speed = moveSpeed[0];
    }
    void Patrol()
    {
        // Transform(s) of the current waypoint in the waypoints array.
        Transform point = waypoints[waypointIndex];

        // Agent destination (move to current waypoint position).
        agent.SetDestination(point.position);

        // If we're close enough to the waypoint...
        if (DestinationReached(0.5f))
        {
            SetNewWaypoint();
        }
    }
    public override bool LookForPlayer()
    {
        return base.LookForPlayer();
    }

    void Update()
    {
        if (LookForPlayer())
        {
            ModeSwitch(true);
            return;
        }
        Patrol();
    }
}
