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
    

    public State CurrentState
    {
        get
        {
            return currentState;
        }
        set
        {
            if(currentState != value)
            {
                target = null;
                currentState = value;
                Debug.Log("new state: " + currentState);
            }
        }
    }
    [Header("Behaviours")]
    private State currentState = State.Patrol;
    



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

    //public float shootTimer, shootDelay;

    public GameObject investigateTarget;

    [AI_ScoutDrone_(new string[] { "0-Waypoint", "1-Seek Target", "2-Range Target", "3-Retreat" })]
    public float[] stoppingDistance = new float[4]; // Stopping distance for different conditions.

    [Header("Components")]
    public NavMeshAgent agent; // Unity component reference
    public Transform target; // Reference assigned target's Transform data (position/rotation/scale).
    public Transform waypointParent; // Reference one waypoint Parent (used to get children in array).
    public AI_FoV_Detection fov; // Reference FieldOfView Script (used for line of sight player detection).
    public AI_Weapon gun;
    public Transform player;

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
    public Transform hand;
    public float shootTimer, shootTimerMax;


    public Vector3 investigatePoint;

    // Creates a collection of Transforms
    //[HideInInspector]
    public Transform[] waypoints; // Transform of (child) waypoints in array.
    [HideInInspector]
    public int currentIndex = 1; // Counts sequential waypoints of array index.
                                 //[HideInInspector]
                                 //public Quaternion startRotation;
    #endregion VARIABLES

    #region HELPER FUNCTIONS
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

    void SetNewWaypoint()
    {
        currentIndex = Random.Range(0, waypoints.Length);
    }

    // This is accessed from the bullet - if it hits near the enemy
    #endregion

    #region STATES
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
            CurrentState = State.Seek;
        }
    }


    void Shoot()
    {
        shootTimer -= Time.deltaTime;

        if(shootTimer < 0)
        {
            gun.Shoot();
            shootTimer = shootTimerMax;
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
            CurrentState = State.Survey;
            return;
        }
        initVar = false;
    }

    // The contained variables for the Seek state (what rules the enemy AI follows when in 'Seek').
    void SeekPlayer()
    {
        // Retreat to totem if health is lower than 25
        if (healthRef.currentHealth < 30)
        {
            CurrentState = State.Totem;
        }

        // If we can't see any targets...
        if (!LookForPlayer())
        {
            CurrentState = State.Investigate;
        }

        #region If Target is Seen...
        // If we see a target...
        if (LookForPlayer())
        {
            // Aim gun at the target.
            hand.LookAt(target.position);

            //Debug.Log("innacuarcy: "+accuracyOffset);

            // Get distance between enemy and player/target.
            float seekDistance = agent.remainingDistance;

            // Move to specified position under set conditions.
            #region Agent Destinations
            agent.SetDestination(target.position);
            if (seekDistance >= stoppingDistance[2] - 0.5f && seekDistance <= stoppingDistance[2] + 0.5f)
            {
                Debug.Log("strafe");
                Strafe();
                if(agent.hasPath)
                {
                    agent.ResetPath();
                }          
            }
            else if (DestinationReached(1))
            {
                Debug.Log("Melee");
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
            CurrentState = State.Patrol;
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
        if(healthRef.currentHealth > 50)
        {
            transform.RotateAround(target.position, strafeDir, strafeSpeed * Time.deltaTime);
        }
        else
        {
            agent.updateRotation = false;
            Vector3 waypoint = GetAvoidanceWaypoint();
            agent.SetDestination(waypoint);
            transform.LookAt(target.position);
        }
        agent.updateRotation = true;
    }

    void CoverShoot()
    {
        // after retreat - go to hide (for pause) - then go to shoot - return to hide

        // move to side (beyond obstacle)

        // turn to look at last player position (investigatePoint)

        // if player there, Shoot()

        // if enemy hit - return to cover (retreat?)

        // repeat
    }

    public void Retreat()
    {
        Vector3 retreatPoint = GetAvoidanceWaypoint();
        agent.SetDestination(retreatPoint);
        if(agent.remainingDistance < 0.5f)
        {
            CurrentState = State.Hide;
        }
    }

    public void Investigate()
    {
        agent.SetDestination(investigatePoint);
         
        if (DestinationReached(0.5f))
        {
            CurrentState = State.Survey;
        }
    }

    

    public void SeekTotem()
    {
        agent.SetDestination(totemPos);
        if(agent.remainingDistance < 5)
        {
            agent.ResetPath();
            CurrentState = State.Survey;
        }
    }
    #endregion

    #region ACTIONS
    void MeleeAttack()
    {
        if (LookForPlayer())
        {
            agent.SetDestination(target.position);
        }
        else
        {
            CurrentState = State.Investigate;
        }

        if (DestinationReached(1))
        {
            // MELEE ATTACK
        }
    }

    void Crouch(bool _crouch)
    {
        if (_crouch)
        {
            // IMPLEMENT CROUCH MECHANIC
            return;
        }
        else
        {
            // IMPLEMENT STAND MECHANIC
        }
    }
    #endregion

    #region SENSES

    bool LookForPlayer()
    {
        if (fov.visibleTargets.Count > 0)
        {
            if (fov.visibleTargets.Count > 1)
            {
                GetClosestTarget();
            }

            target = fov.visibleTargets[0];

            // Store investigate point in case target is lost (last seen target point) 
            investigatePoint = target.position;
            investigateTarget.transform.position = investigatePoint;

            return true;
        }
        return false;
    }

    bool DestinationReached(float desiredDistance)
    {
        if (agent.remainingDistance < desiredDistance)
        {
            return true;
        }
        return false;
    }

    public void BulletAlert(Vector3 impactPoint)
    {
        hideTimer = hideTime;
        if (CurrentState != State.Hide)
        {
            investigatePoint = impactPoint;
            CurrentState = State.Investigate;
        }
    }
    #endregion

    #region START
    public virtual void Start()
    {
        GetNearestTotem();
        healthRef = GetComponent<EnemyHealth>();

        // Get children of waypointParent.
        waypoints = waypointParent.GetComponentsInChildren<Transform>();

        // Get NavMeshAgent (failsafe).
        agent = GetComponent<NavMeshAgent>();

        // Get weapon
        gun = GetComponentInChildren<AI_Weapon>();

        player = FindObjectOfType<PlayerHealth>().transform;
    }
    #endregion Start

    #region Update
    void Update()
    {
        shootTimer -= Time.deltaTime;
        
        // if in "searching" states, && there is an inspectionn waypoint found
        //if((currentState == State.Patrol || currentState == State.Survey) && investigatePoint != null)
        //{
        //    currentState = State.Investigate;
        //}

        // Hide and survey count down a wait timer - this bool sets it's time on the first entry to state
        if (CurrentState != State.Hide && CurrentState != State.Survey)
        {
            initVar = true;
        }

        switch (CurrentState)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Seek:
                SeekPlayer();
                break;
            case State.Retreat:
                Retreat();
                break;
            case State.Survey:
                Survey();
                break;
            case State.Totem:
                SeekTotem();
                break;
            case State.Investigate:
                Investigate();
                break;
            default:
                Patrol();
                break;
        }

        // Set speed based on current state
        SetSpeed();
    }

    
    void SetSpeed()
    {
        switch (CurrentState)
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
    #endregion Update
}


//IEnumerator LookAround()
//{
//    Vector3 curLeft = -transform.right;
//    Vector3 curRight = transform.right;

//    bool canRight = !Physics.Raycast(transform.position, curRight, 5);
//    bool canLeft = !Physics.Raycast(transform.position, curLeft, 5);

//    if(canRight)
//    {
//        Debug.Log("Look Right");
//        // look here
//        yield return new WaitForSeconds(2);
//    }
//    if (canLeft)
//    {
//        Debug.Log("Look Left");
//        // look here
//        yield return new WaitForSeconds(2);
//    }



//}