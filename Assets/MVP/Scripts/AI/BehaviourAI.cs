using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BehaviourAI : MonoBehaviour
{
    #region VARIABLES
    // Declaration
    public enum State // The behaviour states of the enemy AI.
    {
        Patrol = 0,
        Seek = 1,
        Investigate = 2
    }

    [Header("Behaviours")]
    public State currentState = State.Patrol; // The default/start state set to Patrol.

    [AI_ScoutDrone_(new string[] { "Speed Patrol", "Speed Seek", "Speed Investigate" })]
    public float[] moveSpeed = new float[3]; // Movement speeds for different states (up to you).
    [AI_ScoutDrone_(new string[] { "Pause Patrol", "Pause Seek", "Pause Investigate" })]
    public float[] pauseDuration = new float[3]; // Time to wait before doing next thing.
    // [AI_ScoutDrone_(new string[] { "Timer Patrol", "Timer Seek", "Timer Investigate" })]
    private float[] holdStateTimer = new float[3]; // Used to count how much time has passed since...

    [AI_ScoutDrone_(new string[] { "Waypoint", "Seek Target", "Range Target", "Retreat" })]
    public float[] stoppingDistance = new float[4]; // Stopping distance for different conditions.

    [Header("Components")]
    public NavMeshAgent agent; // Unity component reference
    public Transform aim; // Transform of drone chasis/body position.
    public Transform target; // Reference assigned target's Transform data (position/rotation/scale).
    public Transform waypointParent; // Reference one waypoint Parent (used to get children in array).
    public AI_FoV_Detection fov; // Reference FieldOfView Script (used for line of sight player detection).

    // Creates a collection of Transforms
    private Transform[] waypoints; // Transform of (child) waypoints in array.
    private int currentIndex = 1; // Counts sequential waypoints of array index.
    private Quaternion startRotation;
    #endregion VARIABLES

    #region STATES
    // The contained variables for the Patrol state (what rules the enemy AI follows when in 'Patrol').
    void Patrol()
    {
        // Transform(s) of the current waypoint in the waypoints array.
        Transform point = waypoints[currentIndex];
        // Agent navigation speed.
        agent.speed = moveSpeed[0];
        // Agent destination (move to current waypoint position).
        agent.SetDestination(point.position);
        
        // Get the distance between enemy and waypoint.
        float distance = Vector3.Distance(transform.position, point.position);

        // Wait for a set time at each waypoint before moving to the next one.
        #region Hold (Wait) at Waypoint
        // If we're close enough to the waypoint...
        if (distance < stoppingDistance[0])
        {
            // ... start counting down the waypoint timer.
            holdStateTimer[0] -= Time.deltaTime;
            // When the timer reaches zero...
            if (holdStateTimer[0] <= 0)
            {
                // Reset the timer, and move to the next waypoint in the index.
                holdStateTimer[0] = pauseDuration[0];
                currentIndex++;
                // Set waypoint currentIndex back to the start if we complete the last waypoint.
                if (currentIndex >= waypoints.Length)
                {
                    currentIndex = 1;
                }
            }
        }
        #endregion
        
        // If we spot a player...
        if (fov.visibleTargets.Count > 0)
        {
            // ... switch to Seek behaviour, and set target to the first visible target.
            currentState = State.Seek;
            target = fov.visibleTargets[0];
        }
    }

    // The contained variables for the Seek state (what rules the enemy AI follows when in 'Seek').
    void Seek()
    {
        // Agent navigation speed.
        agent.speed = moveSpeed[1];
        // Get distance between enemy and player/target.
        float seekDistance = Vector3.Distance(transform.position, target.position);

        #region If Target is Lost...
        // If we can't see any targets...
        if (fov.visibleTargets.Count < 1)
        {
            // Reset rotation of our aim.
            aim.transform.localRotation = startRotation;
            // Start counting down the Seek timer.
            holdStateTimer[1] -= Time.deltaTime;
            // When the timer reaches zero...
            if (holdStateTimer[1] <= 0)
            {
                // Reset the timer, and go back to Patrol behaviour.
                holdStateTimer[1] = pauseDuration[1];
                currentState = State.Patrol;

                // If we spot a player... set target to the first visible target.
                if (fov.visibleTargets.Count > 0)
                {
                    target = fov.visibleTargets[0];
                }
            }
        }
        #endregion
        #region If Target is Seen...
        // If we see a target...
        if (fov.visibleTargets.Count > 0)
        {
            // Target the first target we see.
            target = fov.visibleTargets[0];

            // Aim at the target.
            #region Rotations (AIM GUN)
            if (target)
            {
                // Reset the Seek timer.
                holdStateTimer[1] = pauseDuration[1];

                #region Aim at Player Position
                // Direction of target (player) from the aim position.
                Vector3 aimDir = target.position - aim.position;
                
                // If our aim is even slightly offset (not pointing directly at the target)...
                if (aimDir.magnitude > 0)
                {
                    // ... rotate our aim to point straight at the target.
                    aim.transform.rotation = Quaternion.LookRotation(aimDir.normalized, Vector3.up);
                }
                #endregion
            }
            #endregion

            // Move to specified position under set conditions.
            #region Agent Destinations
            Vector3 targetDir = transform.position - target.position;
            if (seekDistance > stoppingDistance[1])
            {
                agent.SetDestination(target.position);
                print("Chase");
            }
            if (seekDistance >= stoppingDistance[2] - 0.5f && seekDistance <= stoppingDistance[2] + 0.5f)
            {
                agent.SetDestination(transform.position);
                print("Hold");
            }
            if (seekDistance < stoppingDistance[3])
            {
                agent.SetDestination(targetDir.normalized * stoppingDistance[2]);
                print("Retreat");
            }
            #endregion
        }
        #endregion

        #region Waypoint Timer (Fix)
        // NOTE: Copy-paste from Patrol() - This is to keep the waitTimer counting down during Seek().
        Transform point = waypoints[currentIndex];

        float distance = Vector3.Distance(transform.position, point.position);

        if (distance < stoppingDistance[0])
        {
            holdStateTimer[0] -= Time.deltaTime;
            if (holdStateTimer[0] <= 0)
            {
                holdStateTimer[0] = pauseDuration[0];
                currentIndex++;
                if (currentIndex >= waypoints.Length)
                {
                    currentIndex = 1;
                }
            }
        }
        #endregion
    }


    public void Investigate(Vector3 position)
    {
        // Agent navigation speed.
        agent.speed = moveSpeed[2];

        agent.SetDestination(position);
        currentState = State.Investigate;
    }
    #endregion

    #region Start
    // Use this for initialization
    void Start()
    {
        // Set thisTimer to pauseDuration.
        holdStateTimer[0] = pauseDuration[0];
        holdStateTimer[1] = pauseDuration[1];
        holdStateTimer[2] = pauseDuration[2];

        moveSpeed[0] = moveSpeed[0];
        moveSpeed[1] = moveSpeed[1];
        moveSpeed[2] = moveSpeed[2];

        // Get children of waypointParent.
        waypoints = waypointParent.GetComponentsInChildren<Transform>();

        // Get NavMeshAgent (failsafe).
        agent = GetComponent<NavMeshAgent>();

        startRotation = aim.transform.localRotation;
    }
    #endregion Start

    #region Update
    // Update is called once per frame
    void Update()
    {
        // Switch current state
        switch (currentState)
        {
            case State.Patrol:
                // Patrol state
                Patrol();
                break;
            case State.Seek:
                // Seek state
                Seek();
                break;
            case State.Investigate:
                // Run this code while in investigate state
                // If the agent gets close to the investigate position
                if (agent.remainingDistance < stoppingDistance[0])
                {
                    // Note(Manny): Why not wait for 5 seconds here (timer)
                    holdStateTimer[2] -= Time.deltaTime;
                    if (holdStateTimer[2] <= 0)
                    {
                        holdStateTimer[2] = pauseDuration[2];
                        // Switch to Patrol
                        currentState = State.Patrol;
                    }
                }

                // If the agent sees the player
                if (fov.visibleTargets.Count > 0)
                {
                    // Switch over to seek
                    currentState = State.Seek;
                    // Seek towards the visible target
                    target = fov.visibleTargets[0];
                }
                break;
            default:
                break;
        }
        // If we are in Patrol State...
        // Call Patrol()
        // If we are in Seek State...
        // Call Seek()
    }
    #endregion Update
}
