using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_ScoutDrone : MonoBehaviour
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


    [Header("Animations")]
    public Animator anim;

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
    
    [Header("SearchLight")]
    public Light searchLight; // Reference Light (child 'SearchLight').
    // Colours! Switching searchlight colour during different states (names are self explanatory).
    public Color colorPatrol = Color.white;
    public Color colorSearch = new Color(0.8039216f - 0 / 100, 0.4019608f - 0 / 100, 0);
    public Color colorSeek = new Color(0.8039216f - 0 / 100, 0, 0);
    #endregion VARIABLES

    #region STATES
    // The contained variables for the Patrol state (what rules the enemy AI follows when in 'Patrol').
    void Patrol()
    {
        // Transform(s) of each waypoint in the array.
        Transform point = waypoints[currentIndex];

        // Agent navigation speed.
        agent.speed = moveSpeed[0];

        // Current animation (Patrol) and SearchLight Color.
        anim.SetBool("hasTarget", false);
        anim.SetBool("isAlert", false);
        searchLight.color = colorPatrol;

        // Gets the distance between enemy and waypoint.
        float distance = Vector3.Distance(transform.position, point.position);

        #region Hold (Wait) at Waypoint
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

        //transform.position = Vector3.MoveTowards(transform.position, point.position, 0.1f);
        agent.SetDestination(point.position); // (NavMeshAgent) agent: move to the Transform position of current waypoint.

        if (fov.visibleTargets.Count > 0)
        {
            currentState = State.Seek;
            target = fov.visibleTargets[0];
        }
        #region // DEFUNCT - Old Hold (Wait) at Waypoint
        /// NOTE: Never use Time.time
        /// This method breaks with Time.deltaTime, due to it counting frame time (see current method - courtesy of Stephen)).
        #region if statement logic
        // if statement reads as:
        /*
         *  if the enemy AI's distance to the waypoint is less than 0.5...
         *      and (&& breaks in previous argument) if curTime's equality is 0...
         *          curTime = Time(using Unity Engine's time).time(get the time at beginning of this frame in seconds since the start of the game).
         *      
         *      if the time is greater than or equal to the pauseDuration...
         *          add +1 to currentIndex (move to next waypoint in array).
         *          reset curTime time to 0.
         *          
         *          if enemy AI clears the final waypoint in array...
         *              reset currentIndex to 1 (return/repeat cycle).
        */
        #endregion
        // if (distance < .5f)
        // {
        //     if (waitTime == 0)
        //         waitTime = Time.time;
        // 
        //     if ((Time.time - waitTime) >= pauseDuration)
        //     {
        //         currentIndex++;
        //         waitTime = 0;
        // 
        //         if (currentIndex >= waypoints.Length)
        //         {
        //             currentIndex = 1;
        //         }
        //     }
        // } 
        #endregion
        #region // DEFUNCT - RotateTowards waypoint
        // // Direction of point (waypoint) from current position.
        // Vector3 pointDir = point.position - transform.position;
        // 
        // float step = speedPatrol * Time.deltaTime;
        // 
        // // Rotate front face of ScoutDrone towards pointDir.
        // Vector3 newDir = Vector3.RotateTowards(transform.forward, pointDir, step, 0.0f);
        // 
        // //float angle = Vector3.Angle(Vector3.right, pointDir.normalized);
        // //Vector3 euler = transform.eulerAngles;
        // //euler.y = angle;
        // //transform.eulerAngles = euler;
        // 
        // if (pointDir.magnitude > 0)
        // {
        //     body.transform.rotation = Quaternion.LookRotation(pointDir.normalized, Vector3.up);
        //     body.transform.rotation *= Quaternion.Euler(90, 0, 0);
        // }
        // 
        // // Execute rotation using newDir.
        // //transform.rotation = Quaternion.LookRotation(newDir);
        #endregion
    }
    
    // The contained variables for the Seek state (what rules the enemy AI follows when in 'Seek').
    void Seek()
    {
        // Agent navigation speed.
        agent.speed = moveSpeed[1];

        float seekDistance = Vector3.Distance(transform.position, target.position);

        #region If Target is Lost...
        if (fov.visibleTargets.Count < 1)
        {

            // Current animation (Search) and SearchLight Color.
            anim.SetBool("hasTarget", false);
            anim.SetBool("isAlert", true);
            searchLight.color = colorSearch;

            // Reset rotation of SearchLight
            aim.transform.localRotation = startRotation;

            holdStateTimer[1] -= Time.deltaTime;

            if (holdStateTimer[1] <= 0)
            {
                holdStateTimer[1] = pauseDuration[1];
                currentState = State.Patrol;

                if (fov.visibleTargets.Count > 0)
                {
                    target = fov.visibleTargets[0];
                }
            }
        }
        #region // DEFUNCT - Old Look (Wait) at Player
        // // Makes AI wait after losing line of sight of the player. 'lookTime' instead of 'waitTime' to ensure AI still waits at next waypoint.
        // if (fov.visibleTargets.Count < 1)
        // {
        //     if (lookTime == 0)
        //         lookTime = Time.time;
        // 
        //     if ((Time.time - lookTime) >= pauseDuration)
        //     {
        //         lookTime = 0;
        //         currentState = State.Patrol;
        // 
        //         if (fov.visibleTargets.Count > 0)
        //             target = fov.visibleTargets[0];
        //     }
        // }
        #endregion
        #endregion
        #region If Target is Seen...
        if (fov.visibleTargets.Count > 0)
        {
            target = fov.visibleTargets[0];

            // Switch to relative animations and aim
            #region Anims, Rotations (AIM GUN)
            if (target)
            {
                // Current animation (Seek) and SearchLight Color.
                anim.SetBool("hasTarget", true);
                anim.SetBool("isAlert", true);
                searchLight.color = colorSeek;

                holdStateTimer[1] = pauseDuration[1];

                #region Aim at Player Position
                // Direction of target (player) from the aim position.
                Vector3 aimDir = target.position - aim.position;
                
                if (aimDir.magnitude > 0)
                {
                    aim.transform.rotation = Quaternion.LookRotation(aimDir.normalized, Vector3.up);
                }
                #endregion
            }
            #endregion

            // Move to specified point under set conditions.
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

        // Current animation and SearchLight Color.
        //anim.SetBool("hasTarget", true);
        //anim.SetBool("isAlert", false);
        searchLight.color = colorSearch;
        
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

        // Get Light component from child in GameObject.
        searchLight = GetComponentInChildren<Light>();

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
                    // Current animation (Search).
                    anim.SetBool("hasTarget", false);
                    anim.SetBool("isAlert", true);

                    // Note(Manny): Why not wait for 5 seconds here (timer)
                    holdStateTimer[2] -= Time.deltaTime;
                    if (holdStateTimer[2] <= 0)
                    {
                        holdStateTimer[2] = pauseDuration[2];
                        // Switch to Patrol
                        currentState = State.Patrol;
                    }
                }
                else
                {
                    // Current animation (Investigate).
                    anim.SetBool("hasTarget", true);
                    anim.SetBool("isAlert", false);
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
