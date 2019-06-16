using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using NaughtyAttributes;
using BT;
using UnityEngine.Events;

public class BehaviourAI : MonoBehaviour
{
    #region VARIABLES
    public bool ShowEvents;
    [ShowIf("ShowEvents")] [BoxGroup("Events")] public UnityEvent updateAi;
    //public float turnSpeed;

    public bool ShowAiStateData;
    [ShowIf("ShowAiStateData"), BoxGroup("Ai State Data")] public Pattern currentPattern;

    [SerializeField, ShowIf("ShowAiStateData"), BoxGroup("Ai State Data")] SenseMemoryFactory.SMData currentData;
    public SenseMemoryFactory.SMData CurrentData
    {
        get
        {
            return currentData;
        }
        set
        {
            currentData = value;
            targets = value.targets;
            inspectionPoints = value.inspectionPoints;
            targetLastSeen = value.targetLastSeen;
            distance = value.distance;
        }
    }

    // Sighted enemie's positions
    [ShowIf("ShowAiStateData"), BoxGroup("Ai State Data")] public List<Transform> targets;
    // Positions where strange activity detected
    [ShowIf("ShowAiStateData"), BoxGroup("Ai State Data")] public List<Vector3> inspectionPoints;
    // lst seen position of target
    [ShowIf("ShowAiStateData"), BoxGroup("Ai State Data")] public Vector3 targetLastSeen;
    // distance to closest Target
    [ShowIf("ShowAiStateData"), BoxGroup("Ai State Data")] public float distance;

    public bool debugBehaviour;
    public float hoverHeight;
    bool handCorrecting, startHover;

    public EnemyUIHealthBar healthBarRef;

    float finalModelHeight, startModelHeight, hoverTimer;

    // Control Flow Classes
    [HideInInspector] public PatternManager pM;
    [HideInInspector] public DecisionMachine dM;
    [HideInInspector] public SenseMemoryFactory sMF;

    // Behaviours and Modes
    [HideInInspector]
    public List<Decider> deciders;

    public bool ShowSpecs;
    [ShowIf("ShowSpecs"), BoxGroup("Enemy Specs")] public float aiUpdateRate = 1;
    [ShowIf("ShowSpecs"), BoxGroup("Enemy Specs")] float shootTimer;
    [SerializeField] [ShowIf("ShowSpecs")] [BoxGroup("Enemy Specs")] float shootDelay;
    [ShowIf("ShowSpecs"), BoxGroup("Enemy Specs"), AI_ScoutDrone_(new string[] { "Naive", "Suspicious", "Combat" })]
    public float[] moveSpeed = new float[3]; // Movement speeds for different states (up to you).
    [ShowIf("ShowSpecs"), BoxGroup("Enemy Specs"), AI_ScoutDrone_(new string[] { "Naive", "Suspicious", "Combat" })]
    public float[] turnSpeed = new float[3]; // Movement speeds for different states (up to you).
    [ShowIf("ShowSpecs"), BoxGroup("Enemy Specs")] public int maxBurstFire;
    Quaternion handStartRot;
    [ShowIf("ShowSpecs"), BoxGroup("Enemy Specs")] public bool lookAtTarget;


    //[AI_ScoutDrone_(new string[] { "0-Waypoint", "1-Seek Target", "2-Range Target", "3-Retreat" })]
    //public float[] stoppingDistance = new float[4]; // Stopping distance for different conditions.

    public bool ShowComponents;
    [ShowIf("ShowComponents")] [BoxGroup("Enemy Components")] public NavMeshAgent agent; // Unity component reference
    [ShowIf("ShowComponents")] [BoxGroup("Enemy Components")] public AI_FoV_Detection fov; // Reference FieldOfView Script (used for line of sight player detection).
    [ShowIf("ShowComponents")] [BoxGroup("Enemy Components")] public AI_Weapon gun;
    [ShowIf("ShowComponents")] [BoxGroup("Enemy Components")] public EnemyHealth healthRef;
    [ShowIf("ShowComponents")] [BoxGroup("Enemy Components")] public Transform hand, model;
    Collider col;

    [HideInInspector] public bool isGuard;


    public bool ShowMarkers;
    [ShowIf("ShowMarkers")] [BoxGroup("Navigation Markers")] public Transform playerTarget; // Reference assigned target's Transform data (position/rotation/scale).
    [ShowIf("ShowMarkers")] [BoxGroup("Navigation Markers")] public Transform waypointParent; // Reference one waypoint Parent (used to get children in array).
    [ShowIf("ShowMarkers")] [BoxGroup("Navigation Markers")] public LayerMask obstacleMask; // Mask on all obstacles
    [ShowIf("ShowMarkers")] [BoxGroup("Navigation Markers")] public Vector3 investigatePoint; // 
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
    public Vector3 GetAvoidanceWaypoint(Vector3 _playerPosition)
    {
        Collider closest = GetClosestObstacle();
        Vector3 start = _playerPosition;
        Vector3 end = closest.transform.position;
        Vector3 direction = end - start;
        Vector3 point = closest.ClosestPoint(start + direction * 2f);
        return point;
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
    public void MeleeAttack()
    {
    }

    public void OnDrawGizmos()
    {
        if (debugBehaviour)
        {
            Debug.DrawRay(gun.transform.position, gun.transform.forward * 5, Color.red);
        }
    }



    public bool DestinationReached(float desiredDistance)
    {
        if (agent.remainingDistance <= desiredDistance)
        {
            return true;
        }
        return false;
    }
    #endregion

    #region ACTIONS
    // shoots at target random number of times (within range: maxBurstFire)
    public void ShootAt(Transform target)
    {
        if (shootTimer <= 0)
        {
            int shots = Random.Range(1, maxBurstFire);

            hand.LookAt(target.position);

            gun.AiShoot(shots, target);
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

    public void SetSpeed(int _moveSpeedIndex)
    {
        agent.speed = moveSpeed[_moveSpeedIndex];
        agent.angularSpeed = turnSpeed[_moveSpeedIndex];
    }

    #endregion

    #region SENSES

    public void BulletAlert(Vector3 shotOrigin)
    {
        Debug.Log(string.Format(BaneTools.ColorString("bullet allert ai", Color.magenta)));
        // sort new inspection point into ordered list (from closest to player onward)
        if (sMF.inspectionPoints.Count == 0)
        {
            sMF.inspectionPoints.Add(shotOrigin);
        }
        else
        {
            for (int i = 0; i < sMF.inspectionPoints.Count; i++)
            {
                if (Vector3.Distance(transform.position, shotOrigin) < Vector3.Distance(transform.position, sMF.inspectionPoints[i]))
                {
                    return;
                }
            }
        }
        // Re-reun refreshed Ai
        MakeDecisionBasedOnSenses();
    }
    #endregion



    #region START/UPDATE
    private void Awake()
    {
        isGuard = this.name.Contains("Guard") ? true : false;
    }

    void LookAtTarget()
    {
        //Debug.Log("looking");
        if (playerTarget)
        {
            // The step size is equal to speed times frame time.
            float rotSpeed = turnSpeed[turnSpeed.Length - 1] * Time.deltaTime;
            //Debug.Log("rot:" + rotSpeed);
            if (isGuard)
            {
                Vector3 targetDir = playerTarget.position - transform.position;
                Vector3 newDir = Vector3.RotateTowards(transform.forward, targetDir, rotSpeed, 0.0f);
                // Move our position a step closer to the target.
                transform.rotation = Quaternion.LookRotation(newDir);
                Debug.DrawRay(transform.position, newDir, Color.red);
                //Quaternion lookRot = Quaternion.LookRotation(playerTarget.position - transform.position);
                //transform.rotation = lookRot;
            }
            else
            {
                Vector3 targetDir = playerTarget.position - hand.transform.position;
                Vector3 newDir = Vector3.RotateTowards(hand.transform.forward, targetDir, 1, 0.0f);
                // Move our position a step closer to the target.
                hand.transform.rotation = Quaternion.LookRotation(newDir);

                //Debug.DrawRay(hand.transform.position, newDir, Color.red);
                //Quaternion lookRot = Quaternion.LookRotation(playerTarget.position - hand.transform.position);
                //hand.transform.rotation = lookRot;
            }
        }
        else
        {
            Debug.Log("Lost target to Look at, finding new behaviour");
            // re-run ai selection for new behaviour that will run without player target
            updateAi.Invoke();
        }
    }
    private void Update()
    {
        // Shoot Timer
        if (shootTimer > 0)
            shootTimer -= Time.deltaTime;

        // Look at target (strafing)
        if (lookAtTarget)
        {
            LookAtTarget();
        }
        else
        {
            if (hand.transform.localRotation != handStartRot)// && !handCorrecting)
            {
                //hand.transform.localRotation = Quaternion.Slerp(hand.transform.localRotation, handStartRot, Time.deltaTime);
                hand.transform.localRotation = handStartRot;
            }
        }


        // Update AI when destination reached
        if (agent.hasPath && DestinationReached(0.1f))
        {
            updateAi.Invoke();
        }


            if (hoverTimer <= 1 && startHover)
            {
                hoverTimer += Time.deltaTime;
                model.localPosition = Vector3.Lerp(new Vector3(model.localPosition.x, startModelHeight, model.localPosition.z),
                                              new Vector3(model.localPosition.x, finalModelHeight, model.localPosition.z),
                                              hoverTimer);
                ((SphereCollider)col).center = model.localPosition;
                healthBarRef.offset.y = model.localPosition.y + 2;
            }
    }

    public void HoverHeight(float finish)
    {
        // if already at desired height, return
        if (model.position.y == finish)
        {
            return;
        }

        // set lerp start and end
        finalModelHeight = finish;
        startModelHeight = model.localPosition.y;

        hoverTimer = 0;
        // tell update to start lerping height
        startHover = true;
    }


    void GetReferences()
    {
        totem = GetNearestTotem();
        // get health
        healthRef = GetComponent<EnemyHealth>();
        // Get sight detection
        fov = GetComponentInChildren<AI_FoV_Detection>();
        // Get NavMeshAgent
        agent = GetComponent<NavMeshAgent>();
        // Get weapon
        gun = GetComponentInChildren<AI_Weapon>();
        // Get Object model
        model = transform.GetChild(0).transform;

        if (!isGuard)
            col = GetComponent<SphereCollider>();

    }
    void PopulateLists()
    {
        // Initiate List of Decider Classes (Modes: Naive, Suspicious, Combat)
        this.deciders = new List<Decider>();

        // Add behaviours matching each decider type
        this.deciders.Add(new NaiveDecider(GetComponent<Naive>().behaviours, healthRef));
        this.deciders.Add(new SuspiciousDecider(GetComponent<Suspicious>().behaviours, healthRef));
        this.deciders.Add(new CombatDecider(GetComponent<Combat>().behaviours, healthRef));
    }
    void InitialiseSystem()
    {
        // Pattern Manager Instance
        pM = new PatternManager(this);
        // Decision Machine Instance
        dM = new DecisionMachine(totem, this.deciders, pM, healthRef);
        // Sense Memory factory Instance
        sMF = new SenseMemoryFactory(fov);
    }
    private void Start()
    {
        // Get Components
        GetReferences();
        // Create Deciders list give each decider it's behaviours
        PopulateLists();
        // Create instances of AI architecture
        InitialiseSystem();


        handStartRot = hand.transform.localRotation;

        // repeating method that gets world info and decides actions
        InvokeRepeating("MakeDecisionBasedOnSenses", 0, aiUpdateRate);
    }
    public void MakeDecisionBasedOnSenses() // Wrapper Function
    {
        // Decision Maker class, "Makes Decision From" Sense Memory Factory, which returns SMData class (basically a struct with
        // target positions, and inspection Point positions - the basic inputs for ai behaviours)
        dM.MakeDecisionFrom(sMF.GetSMData());
    }
    #endregion Start
}
// Maybe Obsolete Code
//public Transform GetClosestTarget()
//{
//    float closestTargetDist = Mathf.Infinity;
//    int transformIndex = 0;
//    for (int index = 0; index < fov.visibleTargets.Count; index++)
//    {
//        if (Vector3.Distance(transform.position, fov.visibleTargets[index].position) < closestTargetDist)
//        {
//            closestTargetDist = Vector3.Distance(transform.position, fov.visibleTargets[index].position);
//            transformIndex = index;
//        }
//    }
//    return fov.visibleTargets[transformIndex];
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

//public void SeekTotem()
//{
//    agent.SetDestination(totem.transform.position);
//    if (agent.remainingDistance < 5)
//    {
//        agent.ResetPath();
//        //currentState = State.Survey;
//    }
//}