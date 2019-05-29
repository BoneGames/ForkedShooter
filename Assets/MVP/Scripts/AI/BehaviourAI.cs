using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;


public class BehaviourAI : MonoBehaviour
{
    // Monobehavious Lists (later to become Pattern Type Lists) 
     public List<MonoBehaviour> naivePatterns;
     public List<MonoBehaviour> suspiciousPatterns;
     public List<MonoBehaviour> combatPatterns;



    public PatternManager pM;
    public DecisionMachine dM;

    public float ai_Update_Rate = 1;

    public List<Pattern> patterns;

    public List<Decider> deciders;


    #region STATE ENUMs
    // Declaration
    public enum State // The behaviour states of the enemy AI.
    {
        Patrol,
        Seek,
        Retreat,
        Survey,
        Totem,
        Investigate,
        Hide,
        Fire,
    }

    public enum Mode
    {
        Naive,
        Suspicious,
        Combat
    }

    public SenseMemoryFactory sMF;

    #endregion

    #region VARIABLES

    [Header("Behaviours")]
    public State currentState = State.Patrol;
    //private Mode currentMode = Mode.Naive;

    InvulTotem totem;

    [AI_ScoutDrone_(new string[] { "Speed Patrol", "Speed Seek", "Speed Investigate" })]
    public float[] moveSpeed = new float[3]; // Movement speeds for different states (up to you).

    private float hideTimer;
    public float hideTime;

    public GameObject investigateTarget;

    [AI_ScoutDrone_(new string[] { "0-Waypoint", "1-Seek Target", "2-Range Target", "3-Retreat" })]
    public float[] stoppingDistance = new float[4]; // Stopping distance for different conditions.

    [Header("Components")]
    public NavMeshAgent agent; // Unity component reference
    public Transform playerTarget; // Reference assigned target's Transform data (position/rotation/scale).
    public Transform waypointParent; // Reference one waypoint Parent (used to get children in array).
    public AI_FoV_Detection fov; // Reference FieldOfView Script (used for line of sight player detection).
    public AI_Weapon gun;
    public BehaviourAI[] Modes;
    public int currentMode;



    public LayerMask obstacleMask;
    public bool initVar = true;

    public int intensity;



    [HideInInspector]
    public Quaternion startRotation;

    public EnemyHealth healthRef;

    //[HideInInspector]
    public Transform hand;
    public float shootTimer, shootDelay;


    public Vector3 investigatePoint;

    #endregion VARIABLES


    #region HELPER FUNCTIONS
    // Returns closest obstacle collider to target
    public Collider GetClosestObstacle()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, 100, obstacleMask);
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

    public void ModeSwitch(bool up)
    {
        // set new Mode Index
        currentMode = up ? currentMode++ : currentMode--;
        // disable all Mode Scripts
        foreach (BehaviourAI mode in Modes)
        {
            mode.enabled = false;
        }
        // enable new mode
        Modes[currentMode].enabled = true;
    }

    public Vector3 GetAvoidanceWaypoint()
    {
        Collider closest = GetClosestObstacle();
        Vector3 start = playerTarget.position;
        Vector3 end = closest.transform.position;
        Vector3 direction = end - start;
        Vector3 point = closest.ClosestPoint(start + direction * 2f);
        return point;
    }

    public Transform GetClosestTarget()
    {
        float closestTargetDist = Mathf.Infinity;
        int transformIndex = 0;
        for (int index = 0; index < fov.visibleTargets.Count; index++)
        {
            if (Vector3.Distance(transform.position, fov.visibleTargets[index].position) < closestTargetDist)
            {
                closestTargetDist = Vector3.Distance(transform.position, fov.visibleTargets[index].position);
                transformIndex = index;
            }
        }
        return fov.visibleTargets[transformIndex];
    }

    public void GetNearestTotem()
    {
        InvulTotem[] totemPoles = FindObjectsOfType<InvulTotem>();
        float shortestDist = float.MaxValue;

        foreach (InvulTotem tp in totemPoles)
        {
            float thisDist = Vector3.Distance(transform.position, tp.transform.position);
            if (thisDist < shortestDist)
            {
                shortestDist = thisDist;
                totem = tp;
            }
        }
    }

    public void SetNewWaypoint()
    {
        //waypointIndex = Random.Range(0, waypoints.Length);
    }

    // This is accessed from the bullet - if it hits near the enemy
    #endregion

    #region STATES
    public virtual void Patrol()
    {
        //// Transform(s) of the current waypoint in the waypoints array.
        //Transform point = waypoints[waypointIndex];

        //// Agent destination (move to current waypoint position).
        //agent.SetDestination(point.position);

        //// If we're close enough to the waypoint...
        //if (DestinationReached(0.5f))
        //{
        //    SetNewWaypoint();
        //}
        //// If we spot a player...
        //if (LookForPlayer())
        //{
        //    // ... switch to Seek behaviour, and set target to the first visible target.
        //    currentState = State.Seek;
        //}
    }



    void Hide()
    {
        Crouch(true);
        // set pauseTmer on entry only
        if (initVar)
        {
            hideTimer = hideTime;
        }

        // count down timer
        hideTimer -= Time.deltaTime;

        // enter survey state
        if (hideTimer <= 0)
        {
            currentState = State.Survey;
            return;
        }
        initVar = false;
    }

    // The contained variables for the Seek state (what rules the enemy AI follows when in 'Seek').
    void Charge()
    {
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
            hand.LookAt(playerTarget.position);

            //Debug.Log("innacuarcy: "+accuracyOffset);

            // Get distance between enemy and player/target.
            float seekDistance = agent.remainingDistance;

            // Move to specified position under set conditions.
            #region Agent Destinations
            agent.SetDestination(playerTarget.position);
            if (seekDistance >= stoppingDistance[2] - 0.5f && seekDistance <= stoppingDistance[2] + 0.5f)
            {
                Debug.Log("strafe");
                //Strafe();
                if (agent.hasPath)
                {
                    agent.ResetPath();
                }
            }
            //else if (DestinationReached(1))
            //{
            //    Debug.Log("Melee");
            //}

            #endregion
        }
        #endregion
    }

    public void Survey()
    {
        if (initVar)
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
        if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
        {
            seeingDist = Vector3.Distance(transform.position, hit.point) / 4;
        }
        // spin to check surroundings
        transform.Rotate(Vector3.up * Time.deltaTime * 300 / seeingDist);
        // after 1 full revolution
        if (transform.rotation.eulerAngles.y > startRotation.eulerAngles.y - 5 && transform.rotation.eulerAngles.y < startRotation.eulerAngles.y)
        {
            currentState = State.Patrol;
            return;
        }
        initVar = false;
    }

    //public void Strafe()
    //{
    //    strafeTimer += Time.deltaTime;
    //    //Debug.Log("strafeTimer: " + strafeTimer);
    //    if(strafeTimer > strafeTime)
    //    {
    //        // Change strafe Direction
    //        strafeDir *= -1;
    //        // Set Time to strafe in current direction
    //        strafeTime = Random.Range(1f, 3f);
    //        // Set Strafe speed
    //        strafeSpeed = Random.Range(6f, 14f);

    //        strafeTimer = 0;
    //    }
    //    if(healthRef.currentHealth > 50)
    //    {
    //        transform.RotateAround(playerTarget.position, strafeDir, strafeSpeed * Time.deltaTime);
    //    }
    //    else
    //    {
    //        agent.updateRotation = false;
    //        Vector3 waypoint = GetAvoidanceWaypoint();
    //        agent.SetDestination(waypoint);
    //        transform.LookAt(playerTarget.position);
    //    }
    //    agent.updateRotation = true;
    //}

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
        if (agent.remainingDistance < 0.5f)
        {
            currentState = State.Hide;
        }
    }

    public void Investigate()
    {
        agent.SetDestination(investigatePoint);

        //if (DestinationReached(0.5f))
        //{
        //    currentState = State.Survey;
        //}
    }


    public void SeekTotem()
    {
        agent.SetDestination(totem.transform.position);
        if (agent.remainingDistance < 5)
        {
            agent.ResetPath();
            currentState = State.Survey;
        }
    }
    #endregion

    #region ACTIONS
    public void MeleeAttack()
    {
        if (LookForPlayer())
        {
            agent.SetDestination(playerTarget.position);
        }
        else
        {
            currentState = State.Investigate;
        }

        //if (DestinationReached(1))
        //{
        //    // MELEE ATTACK
        //}
    }

    // shoots gun a specific number of times
    public void Shoot(int _shots)
    {
        shootTimer -= Time.deltaTime;

        if (shootTimer < 0)
        {
            gun.Shoot(_shots);
            shootTimer = shootDelay;
        }
    }

    public void Crouch(bool _crouch)
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

    public virtual bool LookForPlayer()
    {
        if (fov.visibleTargets.Count > 0)
        {
            if (fov.visibleTargets.Count > 1)
            {
                GetClosestTarget();
            }

            playerTarget = fov.visibleTargets[0];

            // Store investigate point in case target is lost (last seen target point) 
            investigatePoint = playerTarget.position;
            investigateTarget.transform.position = investigatePoint;

            return true;
        }
        return false;
    }

  
    public void BulletAlert(Vector3 shotOrigin)
    {
        // reset Hide timer (if currently hiding)
        hideTimer = hideTime;
        if (currentState != State.Hide)
        {
            investigatePoint = shotOrigin;
            currentState = State.Investigate;
        }
    }
    #endregion

    #region START
    public virtual void Start()
    {
        GetNearestTotem();
        healthRef = GetComponent<EnemyHealth>();

        // Get children of waypointParent.
        // in patterns
        //waypoints = waypointParent.GetComponentsInChildren<Transform>();

        // Get NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        // Get weapon
        gun = GetComponentInChildren<AI_Weapon>();

        // Initiate List of Decider Interfaces (Modes: Naive, Suspicious, Combat)
        this.deciders = new List<Decider>();
        // Create Pattern Lists, add the Monobehaviours (re-cast as patterns)
        List<Pattern> _naivePatterns = new List<Pattern>(naivePatterns.Cast<Pattern>());
        List<Pattern> _suspiciousPatterns = new List<Pattern>(suspiciousPatterns.Cast<Pattern>());
        List<Pattern> _combatPatterns = new List<Pattern>(combatPatterns.Cast<Pattern>());
        // Add converted (Monobehaviour => Pattern) lists to Deciders List
        this.deciders.Add(new NaiveDecider(_naivePatterns));
        this.deciders.Add(new SuspiciousDecider(_suspiciousPatterns));
        this.deciders.Add(new CombatDecider(_combatPatterns));

        // Pattern Manager Instance
        pM = new PatternManager(this);
        // Decision Machine Instance
        dM = new DecisionMachine(totem, this.deciders, pM);
        // Sense Memory factory Instance
        sMF = new SenseMemoryFactory(fov);

        // repeating method that gets world info and decides actions
        InvokeRepeating("MakeDecisionBasedOnSenses", 0, ai_Update_Rate);
    }

    //
    private void MakeDecisionBasedOnSenses()
    {
        // Decision Maker class, "Makes Decision From" Sense Memory Factory, which returns SMData class (basically a struct with
        // target positions, and inspection Point positions)
        dM.MakeDecisionFrom(sMF.GetSMData());
    }
    #endregion Start

    #region Update
    void Update()
    {
        shootTimer -= Time.deltaTime;
        
       

       

        switch (currentState)
        {
            case State.Patrol:
                Patrol();
                break;
            case State.Seek:
                Charge();
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
        //SetSpeed();
    }

    
    //void SetSpeed()
    //{
    //    switch (currentState)
    //    {
    //        case State.Seek:
    //            agent.speed = moveSpeed[1];
    //            break;
    //        case State.Retreat:
    //            agent.speed = moveSpeed[1];
    //            break;
    //        case State.Totem:
    //            agent.speed = moveSpeed[1];
    //            break;
    //        case State.Investigate:
    //            agent.speed = moveSpeed[2];
    //            break;
    //        // patrol, survey
    //        default:
    //            agent.speed = moveSpeed[0];
    //            break;
    //    }
    //}
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