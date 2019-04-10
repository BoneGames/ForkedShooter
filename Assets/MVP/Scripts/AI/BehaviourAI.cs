﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class BehaviourAI : MonoBehaviour
{
    #region VARIABLES
    // Declaration
    public enum State // The behaviour states of the enemy AI.
    {
        Patrol = 0,
        Seek = 1,
        Retreat = 2,
        Survey = 3,
        Totem = 4
    }

    [Header("Behaviours")]
    public State currentState = State.Patrol; // The default/start state set to Patrol.

    [AI_ScoutDrone_(new string[] { "Speed Patrol", "Speed Seek", "Speed Investigate" })]
    public float[] moveSpeed = new float[3]; // Movement speeds for different states (up to you).
    [AI_ScoutDrone_(new string[] { "Pause Patrol", "Pause Seek", "Pause Investigate" })]
    public float[] pauseDuration = new float[3]; // Time to wait before doing next thing.
    [HideInInspector]
    // [AI_ScoutDrone_(new string[] { "Timer Patrol", "Timer Seek", "Timer Investigate" })]
    public float[] holdStateTimer = new float[3]; // Used to count how much time has passed since...

    [AI_ScoutDrone_(new string[] { "Waypoint", "Seek Target", "Range Target", "Retreat" })]
    public float[] stoppingDistance = new float[4]; // Stopping distance for different conditions.

    [Header("Components")]
    public NavMeshAgent agent; // Unity component reference
    public Transform target; // Reference assigned target's Transform data (position/rotation/scale).
    public Transform waypointParent; // Reference one waypoint Parent (used to get children in array).
    public AI_FoV_Detection fov; // Reference FieldOfView Script (used for line of sight player detection).

    public float attackRange = 5f;
    public LayerMask obstacleMask;
    public float inaccuracy;
    Vector3 strafeDir = Vector3.up;

    float strafeTimer, strafeTimerMax;
    [HideInInspector]
    public Quaternion startRotation;
    private Vector3 totemPos;
    public EnemyHealth healthRef;

    // Creates a collection of Transforms
    [HideInInspector]
    public Transform[] waypoints; // Transform of (child) waypoints in array.
    [HideInInspector]
    public int currentIndex = 1; // Counts sequential waypoints of array index.
    //[HideInInspector]
    //public Quaternion startRotation;
    #endregion VARIABLES

    

    // private void OnDrawGizmos()
    // {
    //     GetAvoidanceWaypoint();
    // 
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawWireSphere(target.position, fov.viewRadius);
    // 
    //     Gizmos.color = Color.red;
    //     Gizmos.DrawLine(target.position, closestPoint);
    // 
    //     Gizmos.color = Color.blue;
    //     Gizmos.DrawSphere(foundPoint, .5f);
    // }

    // Returns closest collider to target
    public Collider ClosestObstacle()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, attackRange, obstacleMask);
        //Debug.Log("obstacles found: " + hits.Length);
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

    public Vector3 GetAvoidanceWaypoint()
    {
        Collider closest = ClosestObstacle();

        Vector3 start = target.position;
        Vector3 end = closest.transform.position;
        Vector3 direction = end - start;
        Vector3 point = closest.ClosestPoint(start + direction * 2f);
        return point;
    }

    #region STATES
    // The contained variables for the Patrol state (what rules the enemy AI follows when in 'Patrol').
    public virtual void Patrol()
    {
        // Transform(s) of the current waypoint in the waypoints array.
        Transform point = waypoints[currentIndex];
        // Agent navigation speed.
        agent.speed = moveSpeed[0];
        // Agent destination (move to current waypoint position).
        agent.SetDestination(point.position);
        
        // Get the distance between enemy and waypoint.
        float distance = agent.remainingDistance;

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
        if(healthRef.currentHealth < 25)
        {
            Debug.Log("Totem");
            currentState = State.Totem;
        }
        // Agent navigation speed.
        agent.speed = moveSpeed[1];

        #region If Target is Lost...
        // If we can't see any targets...
        if (fov.visibleTargets.Count < 1)
        {
            // Reset rotation of our aim.
            //aim.transform.localRotation = startRotation;
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
            if (fov.visibleTargets.Count > 1)
            {
                target = GetClosestTarget();
            }

            // Aim at the target.
            #region Rotations (AIM GUN)
            if (target)
            {
                // Reset the Seek timer.
                holdStateTimer[1] = pauseDuration[1];
                Vector3 accuracyOffset = new Vector3(Random.Range(0, inaccuracy), Random.Range(0, inaccuracy), Random.Range(0, inaccuracy));
                transform.LookAt(target.position + accuracyOffset);
            }
            #endregion

            // Get distance between enemy and player/target.
            float seekDistance = agent.remainingDistance;

            // Move to specified position under set conditions.
            #region Agent Destinations
            if (seekDistance > stoppingDistance[1])
            {
                agent.SetDestination(target.position);
                //print("Chase");
            }
            if (seekDistance >= stoppingDistance[2] - 0.5f && seekDistance <= stoppingDistance[2] + 0.5f)
            {
                Strafe();
                agent.ResetPath();
                //print("Hold");
            }
            if (seekDistance < stoppingDistance[3])
            {
                currentState = State.Retreat;
                //print("Retreat");
            }
            #endregion
        }
        #endregion

        #region Waypoint Timer (Fix)
        // NOTE: Copy-paste from Patrol() - This is to keep the waitTimer counting down during Seek().
        Transform point = waypoints[currentIndex];

        float distance = agent.remainingDistance;

        if (distance < stoppingDistance[0])
        {
            holdStateTimer[0] -= Time.deltaTime;
            if (holdStateTimer[0] <= 0)
            {
                holdStateTimer[0] = pauseDuration[0];
                currentIndex++;
                if (currentIndex >= waypoints.Length)
                {
                    currentIndex = 0;
                }
            }
        }
        #endregion
    }

    void Survey()
    {
        // spin to check surroundings
        transform.Rotate(Vector3.up * Time.deltaTime * 50);
        // after 1 full revolution
        if(transform.rotation.eulerAngles.y > startRotation.eulerAngles.y -5 && transform.rotation.eulerAngles.y < startRotation.eulerAngles.y +5)
        {
            currentState = State.Patrol;
        }
    }

    void Strafe()
    { 
        strafeTimer += Time.deltaTime;
        //Debug.Log("strafeTimer: " + strafeTimer);
        if(strafeTimer > strafeTimerMax)
        {
            strafeDir *= -1;
            strafeTimerMax = Random.Range(1f, 3f);
            strafeTimer = 0;
            //random strafeSpeed?
        }
        transform.RotateAround(target.position, strafeDir, 10 * Time.deltaTime);
    }

    public Transform GetClosestTarget()
    {
        float closestTargetDist = Mathf.Infinity;
        int transformIndex = 0;
        for(int index = 0; index < fov.visibleTargets.Count; index++)
        {
             if(Vector3.Distance(transform.position, fov.visibleTargets[index].position) < closestTargetDist)
             {
                 closestTargetDist = Vector3.Distance(transform.position, fov.visibleTargets[index].position);
                 transformIndex = index;
             }
        }
       
        return fov.visibleTargets[transformIndex];
    }

    public void Retreat()
    {
        Vector3 retreatPoint = GetAvoidanceWaypoint();
        agent.SetDestination(retreatPoint);
        if(agent.remainingDistance < 0.5f)
        {
            // Wait for a period of time
            holdStateTimer[2] -= Time.deltaTime;
            // when wait timer expires
            if(holdStateTimer[2] <= 0)
            {
                // get initial rotation
                startRotation = transform.rotation;

                currentState = State.Survey; 
                // reset timer
                holdStateTimer[2] = pauseDuration[2];
            }
        }
    }

    public void Totem()
    {
        agent.SetDestination(totemPos);
        if(agent.remainingDistance < 5)
        {
            agent.ResetPath();
            startRotation = transform.rotation;
            currentState = State.Survey;
        }
    }
   
    #endregion

    #region Start
    // Use this for initialization
    public virtual void Start()
    {
        GetNearestTotem();
        healthRef = GetComponent<EnemyHealth>();

        // Set thisTimer to pauseDuration.
        holdStateTimer[0] = pauseDuration[0];
        holdStateTimer[1] = pauseDuration[1];
        holdStateTimer[2] = pauseDuration[2];

        // Get children of waypointParent.
        waypoints = waypointParent.GetComponentsInChildren<Transform>();

        // Get NavMeshAgent (failsafe).
        agent = GetComponent<NavMeshAgent>();
    }

    void GetNearestTotem()
    {
        InvulTotem[] totemPoles = FindObjectsOfType<InvulTotem>();
        float shortestDist = Mathf.Infinity;

        foreach(InvulTotem tp in totemPoles)
        {
            float thisDist = Vector3.Distance(transform.position, tp.transform.position);
            if(thisDist < shortestDist)
            {
                shortestDist = thisDist;
                totemPos = tp.transform.position;
            }
        }
    }
    #endregion Start

    #region Update
    void Update()
    {
        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Seek:
                Seek();
                break;
            case State.Retreat:
                Retreat();
                break;
            case State.Survey:
                Survey();
                break;
            case State.Totem:
                Totem();
                break;
            default:
                Patrol();
                break;
        }
    }
    #endregion Update
}
