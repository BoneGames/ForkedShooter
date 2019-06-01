using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using System.Linq;
using NaughtyAttributes;


public class BehaviourAI : MonoBehaviour
{
    #region VARIABLES
    // Control Flow Classes
    [HideInInspector] public PatternManager pM;
    [HideInInspector] public DecisionMachine dM;
    [HideInInspector] public SenseMemoryFactory sMF;

    // Behaviours and Modes
    [HideInInspector]
    public List<Pattern> patterns;
    public List<Decider> deciders;

    public bool ShowSpecs;
    [ShowIf("ShowSpecs")] [BoxGroup("Enemy Specs")] public float updateRate = 1;
    [ShowIf("ShowSpecs")] [BoxGroup("Enemy Specs")] public float shootTimer;
    [ShowIf("ShowSpecs")] [BoxGroup("Enemy Specs")] public float shootDelay;
    [ShowIf("ShowSpecs")]
    [BoxGroup("Enemy Specs")]
    [AI_ScoutDrone_(new string[] { "Naive", "Suspicious", "Combat" })]
    public float[] moveSpeed = new float[3]; // Movement speeds for different states (up to you).


    //[AI_ScoutDrone_(new string[] { "0-Waypoint", "1-Seek Target", "2-Range Target", "3-Retreat" })]
    //public float[] stoppingDistance = new float[4]; // Stopping distance for different conditions.

    public bool ShowComponents;
    [ShowIf("ShowComponents")] [BoxGroup("Enemy Components")] public NavMeshAgent agent; // Unity component reference
    [ShowIf("ShowComponents")] [BoxGroup("Enemy Components")] public AI_FoV_Detection fov; // Reference FieldOfView Script (used for line of sight player detection).
    [ShowIf("ShowComponents")] [BoxGroup("Enemy Components")] public AI_Weapon gun;
    [ShowIf("ShowComponents")] [BoxGroup("Enemy Components")] public EnemyHealth healthRef;


    public bool ShowMarkers;
    [ShowIf("ShowMarkers")] [BoxGroup("Navigation Markers")] public Transform playerTarget; // Reference assigned target's Transform data (position/rotation/scale).
    [ShowIf("ShowMarkers")] [BoxGroup("Navigation Markers")] public Transform waypointParent; // Reference one waypoint Parent (used to get children in array).
    [ShowIf("ShowMarkers")] [BoxGroup("Navigation Markers")] public LayerMask obstacleMask; // Mask on all obstacles
    [ShowIf("ShowMarkers")] [BoxGroup("Navigation Markers")] public Vector3 investigatePoint;
    [ShowIf("ShowMarkers")] [BoxGroup("Navigation Markers")] public InvulTotem totem; // enemie's local totem
    #endregion VARIABLES


    #region HELPER FUNCTIONS
    // Returns closest obstacle collider to target
    public Collider GetClosestObstacle()
    {
        Collider[] obstacles = Physics.OverlapSphere(transform.position, 100, obstacleMask);
        // Set closest to null
        Collider closest = null;
        // Init closest as max value
        float closestDist = float.MaxValue;
        // Loop through all entities
        foreach (var obstacle in obstacles)
        {
            // Set distance to entity distance
            float distance = Vector3.Distance(transform.position, obstacle.transform.position);
            // If distance < minValue
            if (distance < closestDist)
            {
                // Set minValue to distance
                closestDist = distance;
                // Set closest to entity
                closest = obstacle;
            }
        }
        return closest;
    }

    public Vector3 GetAvoidanceWaypoint(Vector3 playerPosition)
    {
        Collider closest = GetClosestObstacle();
        Vector3 start = playerPosition;
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

    public InvulTotem GetNearestTotem()
    {
        InvulTotem[] totemPoles = FindObjectsOfType<InvulTotem>();
        float shortestDist = float.MaxValue;
        InvulTotem _totem = null; ;
        foreach (InvulTotem tp in totemPoles)
        {
            float thisDist = Vector3.Distance(transform.position, tp.transform.position);
            if (thisDist < shortestDist)
            {
                shortestDist = thisDist;
                _totem = tp;
            }
        }
        return _totem;
    }

    public bool DestinationReached(float desiredDistance)
    {
        if (agent.remainingDistance < desiredDistance)
        {
            return true;
        }
        return false;
    }
    #endregion

    #region STATES




    //void Hide()
    //{
    //    Crouch(true);
    //    // set pauseTmer on entry only
    //    if (initVar)
    //    {
    //        hideTimer = hideTime;
    //    }

    //    // count down timer
    //    hideTimer -= Time.deltaTime;

    //    // enter survey state
    //    if (hideTimer <= 0)
    //    {
    //        currentState = State.Survey;
    //        return;
    //    }
    //    initVar = false;
    //}

    // The contained variables for the Seek state (what rules the enemy AI follows when in 'Seek').
    //void Charge()
    //{
    //    // Retreat to totem if health is lower than 25
    //    if (healthRef.currentHealth < 30)
    //    {
    //        currentState = State.Totem;
    //    }

    //    // If we can't see any targets...
    //    if (!LookForPlayer())
    //    {
    //        currentState = State.Investigate;
    //    }

    //    #region If Target is Seen...
    //    // If we see a target...
    //    if (LookForPlayer())
    //    {
    //        // Aim gun at the target.
    //        hand.LookAt(playerTarget.position);

    //        //Debug.Log("innacuarcy: "+accuracyOffset);

    //        // Get distance between enemy and player/target.
    //        float seekDistance = agent.remainingDistance;

    //        // Move to specified position under set conditions.
    //        #region Agent Destinations
    //        agent.SetDestination(playerTarget.position);
    //        if (seekDistance >= stoppingDistance[2] - 0.5f && seekDistance <= stoppingDistance[2] + 0.5f)
    //        {
    //            Debug.Log("strafe");
    //            //Strafe();
    //            if (agent.hasPath)
    //            {
    //                agent.ResetPath();
    //            }
    //        }
    //        //else if (DestinationReached(1))
    //        //{
    //        //    Debug.Log("Melee");
    //        //}

    //        #endregion
    //    }
    //    #endregion
    //}

    //public void Survey()
    //{
    //    if (initVar)
    //    {
    //        startRotation = transform.rotation;
    //        // clear path
    //        if (agent.hasPath)
    //        {
    //            agent.ResetPath();
    //        }
    //    }
    //    RaycastHit hit;
    //    float seeingDist = 1;

    //    // spin speed is relative to length of sightLine
    //    if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
    //    {
    //        seeingDist = Vector3.Distance(transform.position, hit.point) / 4;
    //    }
    //    // spin to check surroundings
    //    transform.Rotate(Vector3.up * Time.deltaTime * 300 / seeingDist);
    //    // after 1 full revolution
    //    if (transform.rotation.eulerAngles.y > startRotation.eulerAngles.y - 5 && transform.rotation.eulerAngles.y < startRotation.eulerAngles.y)
    //    {
    //        currentState = State.Patrol;
    //        return;
    //    }
    //    initVar = false;
    //}

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
            //currentState = State.Survey;
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
            // currentState = State.Investigate;
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

            return true;
        }
        return false;
    }


    public void BulletAlert(Vector3 shotOrigin)
    {
        // reset Hide timer (if currently hiding)
        //hideTimer = hideTime;
        //if (currentState != State.Hide)
        //{
        //    investigatePoint = shotOrigin;
        //    currentState = State.Investigate;
        //}
    }
    #endregion

    #region START

    void GetReferences()
    {
        totem = GetNearestTotem();
        healthRef = GetComponent<EnemyHealth>();
        fov = GetComponentInChildren<AI_FoV_Detection>();
        // Get NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        // Get weapon
        gun = GetComponentInChildren<AI_Weapon>();
    }
    void PopulateLists()
    {
        // Initiate List of Decider Classes (Modes: Naive, Suspicious, Combat)
        this.deciders = new List<Decider>();

        // Add converted each decider class (that comes with it's list of behaviours)
        this.deciders.Add(new NaiveDecider(GetComponent<NaivePatterns>().patterns));
        this.deciders.Add(new SuspiciousDecider(GetComponent<SuspiciousPatterns>().patterns));
        this.deciders.Add(new CombatDecider(GetComponent<CombatPatterns>().patterns));
    }
    void InitialiseSystem()
    {
        // Pattern Manager Instance
        pM = new PatternManager(this);
        // Decision Machine Instance
        dM = new DecisionMachine(totem, this.deciders, pM);
        // Sense Memory factory Instance
        sMF = new SenseMemoryFactory(fov);
    }
    public virtual void Start()
    {
        // Get Components
        GetReferences();
        PopulateLists();
        InitialiseSystem();

        // repeating method that gets world info and decides actions
        InvokeRepeating("MakeDecisionBasedOnSenses", 0, updateRate);
    }

    // Wrapper Function
    private void MakeDecisionBasedOnSenses()
    {
        // Decision Maker class, "Makes Decision From" Sense Memory Factory, which returns SMData class (basically a struct with
        // target positions, and inspection Point positions - the basic inputs for ai behaviours)
        dM.MakeDecisionFrom(sMF.GetSMData());
    }
    #endregion Start
}