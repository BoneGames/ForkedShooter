using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Patrol Pattern", menuName = "Patterns/Patrol")]
public class PatrolPattern : Pattern
{
    public List<Transform> wayPoints; // Transform of (child) waypoints in array.
    public int waypointIndex = 1; // Counts sequential waypoints of array index.
    

    //public UnityEvent wayPointReached;

 
    void GetWaypointsFrom(BehaviourAI ai)
    {
        Transform [] waypoints = ai.waypointParent.GetComponentsInChildren<Transform>();
        foreach  (Transform t in waypoints)
        {
            Debug.Log("waypoint");
            if (t != ai.waypointParent)
            {
                wayPoints.Add(t);
            }
        }
    }

    // Initialisation - runs once when pattern begins
    public override void StartPatternWith(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {
        base.StartPatternWith(ai,data);

        if (wayPoints.Count < 1)
        GetWaypointsFrom(ai);

        // Transform(s) of the current waypoint in the waypoints array.
        Transform point = wayPoints[waypointIndex];

        // Agent destination (move to current waypoint position).
        ai.agent.SetDestination(point.position);
    }

    // Gets called when pattern is re-called (is cheaper due to if check)
    public override void UpdatePattern(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {
        base.UpdatePattern(ai, data);
        // If we're close enough to the waypoint...
        if (ai.DestinationReached(0.1f))
        {
            waypointIndex = Random.Range(0, wayPoints.Count);

            // Transform(s) of the current waypoint in the waypoints array.
            Transform point = wayPoints[waypointIndex];

            // Agent destination (move to current waypoint position).
            ai.agent.SetDestination(point.position);
        }
    }


   
}
