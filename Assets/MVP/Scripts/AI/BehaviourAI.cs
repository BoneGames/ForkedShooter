using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;

public class BehaviourAI : MonoBehaviour
{
    #region STATE ENUMs
    // Declaration
    public enum State // The behaviour states of the enemy AI.
    {
        Patrol = 0,
        Seek = 1,
        Retreat = 2,
        Survey = 3,
        Totem = 4,
        Investigate = 5,
        Hide = 6,
        Snipe = 7,
        Melee = 8
    }
    #endregion

    #region VARIABLES
    [Header("Behaviours")]
    public State currentState = State.Patrol; // The default/start state set to Patrol.

    [AI_ScoutDrone_(new string[] { "Speed Patrol", "Speed Seek", "Speed Investigate" })]
    public float[] moveSpeed = new float[3]; // Movement speeds for different states (up to you).

    // -- trying to eliminate these arrays and replace with 1 simple pause state that can be fed a float variable --

    //[AI_ScoutDrone_(new string[] { "Pause Patrol", "Pause Seek", "Pause Investigate" })]
    //public float[] pauseDuration = new float[3]; // Time to wait before doing next thing.
    //[HideInInspector]
    //// [AI_ScoutDrone_(new string[] { "Timer Patrol", "Timer Seek", "Timer Investigate" })]
    //public float[] holdStateTimer = new float[3]; // Used to count how much time has passed since...

    private float hideTimer;
    public float hideTime;

    [AI_ScoutDrone_(new string[] { "0-Waypoint", "1-Seek Target", "2-Range Target", "3-Retreat" })]
    public float[] stoppingDistance = new float[4]; // Stopping distance for different conditions.

    [Header("Components")]
    public NavMeshAgent agent; // Unity component reference
    public Transform target; // Reference assigned target's Transform data (position/rotation/scale).
    public Transform waypointParent; // Reference one waypoint Parent (used to get children in array).
    public AI_FoV_Detection fov; // Reference FieldOfView Script (used for line of sight player detection).

    public float obstacleSearchRange = 5f;
    public LayerMask obstacleMask;
    public float inaccuracy;
    Vector3 strafeDir = Vector3.up;
    bool initVar = true;

    float strafeTimer, strafeTime, strafeSpeed;
    [HideInInspector]
    public Quaternion startRotation;
    private Vector3 totemPos;
    public EnemyHealth healthRef;

    //[HideInInspector]
    public Transform shootPoint;


    Vector3 investigatePoint;

    // Creates a collection of Transforms
    //[HideInInspector]
    public Transform[] waypoints; // Transform of (child) waypoints in array.
    [HideInInspector]
    public int currentIndex = 1; // Counts sequential waypoints of array index.
                                 //[HideInInspector]
                                 //public Quaternion startRotation;
    #endregion VARIABLES

    #region HELPER FUNCTIONS
    // Method to call upon FindVisibleTargets Method with a delay (0.2f from Coroutine argument).
   

    // Returns closest obstacle collider to target
    public Collider GetClosestObstacle()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, obstacleSearchRange, obstacleMask);
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
        Collider closest = GetClosestObstacle();
        Vector3 start = target.position;
        Vector3 end = closest.transform.position;
        Vector3 direction = end - start;
        Vector3 point = closest.ClosestPoint(start + direction * 2f);
        return point;
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

    void GetNearestTotem()
    {
        InvulTotem[] totemPoles = FindObjectsOfType<InvulTotem>();
        float shortestDist = float.MaxValue;

        foreach (InvulTotem tp in totemPoles)
        {
            float thisDist = Vector3.Distance(transform.position, tp.transform.position);
            if (thisDist < shortestDist)
            {
                shortestDist = thisDist;
                totemPos = tp.transform.position;
            }
        }
    }

    #endregion

    #region STATES
    // The contained variables for the Patrol state (what rules the enemy AI follows when in 'Patrol').
    public virtual void Patrol()
    {
        // Transform(s) of the current waypoint in the waypoints array.
        Transform point = waypoints[currentIndex];

        // Agent destination (move to current waypoint position).
        agent.SetDestination(point.position);

        // If we're close enough to the waypoint...
        if (DestinationReached(0.5f))
        {
            SetNewWaypoint();
        }
        
        // If we spot a player...
        if (LookForPlayer())
        {
            // ... switch to Seek behaviour, and set target to the first visible target.
            currentState = State.Seek;
        }
    }

    bool DestinationReached(float desiredDistance)
    {
        if(agent.stoppingDistance < desiredDistance)
        {
            return true;
        }
        return false;
    }

    void MeleeAttack()
    {
        if(LookForPlayer())
        {
            agent.SetDestination(target.position);
        }
        else
        {
            currentState = State.Investigate;
        }

        if(DestinationReached(1))
        {
            // MELEE ATTACK
        }
    }

    bool LookForPlayer()
    {
        if(fov.visibleTargets.Count > 0)
        {
            if(fov.visibleTargets.Count > 1)
            {
                GetClosestTarget();
            }

            target = fov.visibleTargets[0];

            // Store investigate point in case target is lost (last seen target point) 
            investigatePoint = target.position;

            return true;
        }
        return false;
    }

    void SetSpeed()
    {
        switch (currentState)
        {
            case State.Seek:
                agent.speed = moveSpeed[1];
                break;
            case State.Retreat:
                agent.speed = moveSpeed[1];
                break;
            case State.Totem:
                agent.speed = moveSpeed[1];
                break;
            case State.Investigate:
                agent.speed = moveSpeed[2];
                break;
                // patrol, survey
            default:
                agent.speed = moveSpeed[0];
                break;
        }
    }

    void Crouch(bool _crouch)
    {
        if(_crouch)
        {
            // IMPLEMENT CROUCH MECHANIC
            return;
        }
        else
        {
            // IMPLEMENT STAND MECHANIC
        }
    }

    public void BulletAlert(Vector3 impactPoint)
    {
        hideTimer = hideTime;
        if(currentState != State.Hide)
        {
            investigatePoint = impactPoint;
            currentState = State.Investigate; 
        }
    }

    void Hide()
    {
        Crouch(true);
        // set pauseTmer on entry only
        if(initVar)
        {
            hideTimer = hideTime;
        }

        // count down timer
        hideTimer -= Time.deltaTime;

        // enter survey state
        if(hideTimer <= 0)
        {
            currentState = State.Survey;
            return;
        }

        initVar = false;
    }

    void SetNewWaypoint()
    {
        currentIndex = Random.Range(0, waypoints.Length);
    }

    // The contained variables for the Seek state (what rules the enemy AI follows when in 'Seek').
    void Seek()
    {
        agent.SetDestination(target.position);
        // Retreat to totem if health is lower than 25
        if (healthRef.currentHealth < 30)
        {
            currentState = State.Totem;
        }

        // If we can't see any targets...
        if (!LookForPlayer())
        {
            currentState = State.Investigate;
        }

        #region If Target is Seen...
        // If we see a target...
        if (LookForPlayer())
        {
            // Aim gun at the target.
            Vector3 accuracyOffset = new Vector3(Random.Range(0, inaccuracy), Random.Range(0, inaccuracy), 0);
            shootPoint.LookAt(target.position + accuracyOffset);

            // Get distance between enemy and player/target.
            float seekDistance = agent.remainingDistance;

            // Move to specified position under set conditions.
            #region Agent Destinations
         
            if (seekDistance >= stoppingDistance[2] - 0.5f && seekDistance <= stoppingDistance[2] + 0.5f)
            {
                Strafe();
                if(agent.hasPath)
                {
                    agent.ResetPath();
                }          
            }
            if (DestinationReached(3))
            {
                currentState = State.Melee;
            }
            #endregion
        }
        #endregion
    }

    void Survey()
    {
        if(initVar)
        {
            startRotation = transform.rotation;
            // clear path
            if (agent.hasPath)
            {
                agent.ResetPath();
            }
        }
        RaycastHit hit;
        float seeingDist = 1;

        // spin speed is relative to length of sightLine
        if(Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
        {
            seeingDist = Vector3.Distance(transform.position, hit.point)/4;
        }
        // spin to check surroundings
        transform.Rotate(Vector3.up * Time.deltaTime * 300/seeingDist);
        // after 1 full revolution
        if(transform.rotation.eulerAngles.y > startRotation.eulerAngles.y -5 && transform.rotation.eulerAngles.y < startRotation.eulerAngles.y)
        {
            currentState = State.Patrol;
            initVar = true;
            return;

        }
        initVar = false;
    }

    void Strafe()
    { 
        strafeTimer += Time.deltaTime;
        //Debug.Log("strafeTimer: " + strafeTimer);
        if(strafeTimer > strafeTime)
        {
            // Change strafe Direction
            strafeDir *= -1;
            // Set Time to strafe in current direction
            strafeTime = Random.Range(1f, 3f);
            // Set Strafe speed
            strafeSpeed = Random.Range(6f, 14f);

            strafeTimer = 0;
        }
        
        transform.RotateAround(target.position, strafeDir, strafeSpeed * Time.deltaTime);
    }

    public void Retreat()
    {
        Vector3 retreatPoint = GetAvoidanceWaypoint();
        agent.SetDestination(retreatPoint);
        if(agent.remainingDistance < 0.5f)
        {
            currentState = State.Survey;
        }
    }

    public void Investigate()
    {
        agent.SetDestination(investigatePoint);
         
        if (DestinationReached(0.5f))
        {
            currentState = State.Survey;
        }
    }

    public void Totem()
    {
        agent.SetDestination(totemPos);
        if(agent.remainingDistance < 5)
        {
            agent.ResetPath();
            currentState = State.Survey;
        }
    }

    #endregion

    #region Start
    public virtual void Start()
    {
        GetNearestTotem();
        healthRef = GetComponent<EnemyHealth>();

        // Get children of waypointParent.
        waypoints = waypointParent.GetComponentsInChildren<Transform>();

        // Get NavMeshAgent (failsafe).
        agent = GetComponent<NavMeshAgent>();
    }

    
    #endregion Start

    #region Update
    void Update()
    {
        // if in "searching" states, && there is an inspectionn waypoint found
        if((currentState == State.Patrol || currentState == State.Survey) && investigatePoint != null)
        {
            Investigate();
        }
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
            case State.Investigate:
                Investigate();
                break;
            default:
                Patrol();
                break;
        }

        SetSpeed();
    }
    #endregion Update
}
