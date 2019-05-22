using UnityEngine;

using Pada1.BBCore;           // Code attributes
using Pada1.BBCore.Tasks;     // TaskStatus
using Pada1.BBCore.Framework; // BasePrimitiveAction

[Action("BEN/ResetWaypoint")]
[Help("Resets Waypoint destination Vector")]
public class RestWaypoint : BasePrimitiveAction
{
    // Array of waypoints the enemy will move between when patrolling
    [InParam("waypoints")]
    public Vector3[] waypoints;

    [InParam("Current Waypoint")]
    public Vector3 currWaypoint;

    [OutParam("New Waypoint")]
    public Vector3 newWaypoint;


    // Main class method, invoked by the execution engine.
    public override TaskStatus OnUpdate()
    {
        // Temporary waypoint variable
        Vector3 tempNewWaypoint = currWaypoint;
        int newIndex = 0;
        
        // Get new waypoint
        while(tempNewWaypoint == currWaypoint)
        {
            newIndex = Random.Range(0, waypoints.Length);
            tempNewWaypoint = waypoints[newIndex];
        }

        // Set new waypoint
        newWaypoint = waypoints[newIndex];

        // The action is completed. We must inform the execution engine.
        return TaskStatus.COMPLETED;

    } // OnUpdate

} // class ShootOnce