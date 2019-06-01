using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[CreateAssetMenu(fileName = "Patrol Pattern", menuName = "Patterns/Patrol")]
public class PatrolPattern : Pattern
{
    public Transform[] waypoints; // Transform of (child) waypoints in array.
    public int waypointIndex = 1; // Counts sequential waypoints of array index.
    

    public UnityEvent wayPointReached;
   

    // Initialisation - runs once when pattern begins
    public override void StartPatternWith(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {
        base.StartPatternWith(ai,data);
        Debug.Log("What is the length - " + waypoints.Length);
        if(waypoints.Length == 0)
        {
            waypoints = ai.waypointParent.GetComponentsInChildren<Transform>();
        }
        // Transform(s) of the current waypoint in the waypoints array.
        Transform point = waypoints[waypointIndex];

        // Agent destination (move to current waypoint position).
        ai.agent.SetDestination(point.position);
    }

    // Gets called when pattern is re-called (is cheaper due to if check)
    public override void UpdatePattern(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {
        base.UpdatePattern(ai, data);
        // If we're close enough to the waypoint...
        if (AI_Helper.DestinationReached(ai, 0.5f))
        {
            waypointIndex = Random.Range(0, waypoints.Length);

            // Transform(s) of the current waypoint in the waypoints array.
            Transform point = waypoints[waypointIndex];

            // Agent destination (move to current waypoint position).
            ai.agent.SetDestination(point.position);
        }
    }


   
}
