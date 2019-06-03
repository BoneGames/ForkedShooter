﻿using System.Collections;
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
    public int maxBurstFire;
    Quaternion handStartRot;


    //[AI_ScoutDrone_(new string[] { "0-Waypoint", "1-Seek Target", "2-Range Target", "3-Retreat" })]
    //public float[] stoppingDistance = new float[4]; // Stopping distance for different conditions.

    public bool ShowComponents;
    [ShowIf("ShowComponents")] [BoxGroup("Enemy Components")] public NavMeshAgent agent; // Unity component reference
    [ShowIf("ShowComponents")] [BoxGroup("Enemy Components")] public AI_FoV_Detection fov; // Reference FieldOfView Script (used for line of sight player detection).
    [ShowIf("ShowComponents")] [BoxGroup("Enemy Components")] public AI_Weapon gun;
    [ShowIf("ShowComponents")] [BoxGroup("Enemy Components")] public EnemyHealth healthRef;
    [ShowIf("ShowComponents")] [BoxGroup("Enemy Components")] public Transform hand;


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
        if (agent.remainingDistance <= desiredDistance)
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
       

        //if (DestinationReached(1))
        //{
        //    // MELEE ATTACK
        //}
    }

    public void OnDrawGizmos()
    {
        Debug.DrawRay(gun.transform.position, gun.transform.forward * 5, Color.red);
    }

    public void ResetAI()
    {
        hand.transform.localRotation = handStartRot;
        agent.updateRotation = true;
    }

    // shoots gun a specific number of times
    public void Shoot(Vector3 target)
    {
        if (shootTimer <= 0)
        {
            int shots = Random.Range(1, maxBurstFire);
            hand.LookAt(target);
            gun.Shoot(shots);
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

   
   
    public void RotateToward(Vector3 target)
    {
        StopAllCoroutines();
        StartCoroutine(FaceTargetRotation(target));
    }
    public IEnumerator FaceTargetRotation(Vector3 target)
    {
        
        Vector3 targetDir;
        float angle = 10;
        // while AI isn't looking at target
        while (angle > 1)
        {
            Debug.Log("rotating");
            // get target direction
            targetDir = target - transform.position;
            // get angle still to turn
            angle = Vector3.Angle(targetDir, transform.forward);
            // get look rotation
            Quaternion lookRot = Quaternion.LookRotation(targetDir);
            // Slerp angle from current to lookRotation
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * angle/2);
            yield return null;
        }
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

    private void Update()
    {
        if(shootTimer > 0)
        shootTimer -= Time.deltaTime;
    }

    #region START

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
    public virtual void Start()
    {
        // Get Components
        GetReferences();
        PopulateLists();
        InitialiseSystem();
        handStartRot = hand.transform.localRotation;

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