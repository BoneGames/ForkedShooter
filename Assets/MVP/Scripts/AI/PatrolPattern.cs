using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolPattern : MonoBehaviour,  Pattern
{
    public Transform[] waypoints; // Transform of (child) waypoints in array.
    public int waypointIndex = 1; // Counts sequential waypoints of array index.
                                  //[HideInInspector]
                                  //public Quaternion startRotation;
    public PatrolPattern()
    {

    }


    public void StartPatternWith(BehaviourAI ai)
    {
        if(waypoints.Length == 0)
        {
            waypoints = ai.waypointParent.GetComponentsInChildren<Transform>();
        }
        // Transform(s) of the current waypoint in the waypoints array.
        Transform point = waypoints[waypointIndex];

        // Agent destination (move to current waypoint position).
        ai.agent.SetDestination(point.position);

        // If we're close enough to the waypoint...
        if (AI_Helper.DestinationReached(ai, 0.5f))
        {
            waypointIndex = Random.Range(0, waypoints.Length);
        }
    }
  
}
