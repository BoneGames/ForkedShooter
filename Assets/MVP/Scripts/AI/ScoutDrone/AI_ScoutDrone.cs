//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.AI;

//public class AI_ScoutDrone : BehaviourAI
//{
//    #region VARIABLES
//    [Header("Animations")]
//    public Animator anim;
    
//    [Header("SearchLight")]
//    public Light searchLight; // Reference Light (child 'SearchLight').
//    // Colours! Switching searchlight colour during different states (names are self explanatory).
//    public Color colorPatrol = Color.white;
//    public Color colorSearch = new Color(0.8039216f - 0 / 100, 0.4019608f - 0 / 100, 0);
//    public Color colorSeek = new Color(0.8039216f - 0 / 100, 0, 0);
//    public Transform aim; // Transform of aim position.

//    #endregion VARIABLES

//    #region STATES
//    // The contained variables for the Patrol state (what rules the enemy AI follows when in 'Patrol').
//    public override void Patrol()
//    {
//        // Everything within 'BehaviourAI.Patrol()'.
//        base.Patrol();

//        // Current animation (Patrol) and SearchLight Color.
//        anim.SetBool("hasTarget", false);
//        anim.SetBool("isAlert", false);
//        searchLight.color = colorPatrol;
//    }
    
//    // The contained variables for the Seek state (what rules the enemy AI follows when in 'Seek').
//    void Seek()
//    {
//        // Agent navigation speed.
//        agent.speed = moveSpeed[1];

//        float seekDistance = Vector3.Distance(transform.position, playerTarget.position);

//        #region If Target is Lost...
//        if (fov.visibleTargets.Count < 1)
//        {

//            // Current animation (Search) and SearchLight Color.
//            anim.SetBool("hasTarget", false);
//            anim.SetBool("isAlert", true);
//            searchLight.color = colorSearch;

//            // Reset rotation of SearchLight
//            aim.transform.localRotation = startRotation;

//            //holdStateTimer[1] -= Time.deltaTime;

//            //if (holdStateTimer[1] <= 0)
//            //{
//            //    holdStateTimer[1] = pauseDuration[1];
//            //    currentState = State.Patrol;
//            //}
//        }
       
//        #endregion
//        #region If Target is Seen...
//        if (fov.visibleTargets.Count > 0)
//        {
//            if(fov.visibleTargets.Count > 1)
//            {
//                playerTarget = GetClosestTarget();
//            } else {
//                playerTarget = fov.visibleTargets[0];
//            }
            

//            // Switch to relative animations and aim
//            #region Anims, Rotations (AIM GUN)
//            if (playerTarget)
//            {
//                // Current animation (Seek) and SearchLight Color.
//                anim.SetBool("hasTarget", true);
//                anim.SetBool("isAlert", true);
//                searchLight.color = colorSeek;

//                //holdStateTimer[1] = pauseDuration[1];

//                #region Aim at Player Position
//                // Direction of target (player) from the aim position.
//                Vector3 aimDir = playerTarget.position - aim.position;
                
//                if (aimDir.magnitude > 0)
//                {
//                    aim.transform.rotation = Quaternion.LookRotation(aimDir.normalized, Vector3.up);
//                }
//                #endregion
//            }
//            #endregion

//            // Move to specified point under set conditions.
//            #region Agent Destinations
//            Vector3 targetDir = transform.position - playerTarget.position;
//            if (seekDistance > stoppingDistance[1])
//            {
//                agent.SetDestination(playerTarget.position);
//                //print("Chase");
//            }
//            if (seekDistance >= stoppingDistance[2] - 0.5f && seekDistance <= stoppingDistance[2] + 0.5f)
//            {
//                agent.SetDestination(transform.position);
//                //print("Hold");
//            }
//            if (seekDistance < stoppingDistance[3])
//            {
//                agent.SetDestination(targetDir.normalized * stoppingDistance[2]);
//                //print("Retreat");
//            } 
//            #endregion
//        }
//        #endregion

//        #region Waypoint Timer (Fix)
//        // NOTE: Copy-paste from Patrol() - This is to keep the waitTimer counting down during Seek().
//        Transform point = waypoints[waypointIndex];

//        float distance = Vector3.Distance(transform.position, point.position);

//        if (distance < stoppingDistance[0])
//        {
//            //holdStateTimer[0] -= Time.deltaTime;
//            //if (holdStateTimer[0] <= 0)
//            //{
//            //    holdStateTimer[0] = pauseDuration[0];
//            //    currentIndex++;


//            //}
//            if (waypointIndex >= waypoints.Length)
//                {
//                    waypointIndex = 1;
//                }
//        }
//        #endregion
//    }
//    #endregion



//    #region Start
//    // Use this for initialization
//    public override void Start()
//    {
//        base.Start();
        
//        // Get Light component from child in GameObject.
//        searchLight = GetComponentInChildren<Light>();

//        startRotation = aim.transform.localRotation;
//    }
//    #endregion Start

//    #region Update
//    // Update is called once per frame
//    void Update()
//    {
//        // Switch current state
//        switch (currentState)
//        {
//            case State.Patrol:
//                // Patrol state
//                Patrol();
//                break;
//            case State.Seek:
//                // Seek state
//                Seek();
//                break;
//            /// case State.Investigate:
//            ///     // Run this code while in investigate state
//            ///     // If the agent gets close to the investigate position
//            ///     if (agent.remainingDistance < stoppingDistance[0])
//            ///     {
//            ///         // Current animation (Search).
//            ///         anim.SetBool("hasTarget", false);
//            ///         anim.SetBool("isAlert", true);
//            /// 
//            ///         // Note(Manny): Why not wait for 5 seconds here (timer)
//            ///         holdStateTimer[2] -= Time.deltaTime;
//            ///         if (holdStateTimer[2] <= 0)
//            ///         {
//            ///             holdStateTimer[2] = pauseDuration[2];
//            ///             // Switch to Patrol
//            ///             currentState = State.Patrol;
//            ///         }
//            ///     }
//            ///     else
//            ///     {
//            ///         // Current animation (Investigate).
//            ///         anim.SetBool("hasTarget", true);
//            ///         anim.SetBool("isAlert", false);
//            ///     }
//            /// 
//            ///     // If the agent sees the player
//            ///     if (fov.visibleTargets.Count > 0)
//            ///     {
//            ///         // Switch over to seek
//            ///         currentState = State.Seek;
//            ///         // Seek towards the visible target
//            ///         target = fov.visibleTargets[0];
//            ///     }
//            ///     break;
//            case State.Retreat:
//                Retreat();
//                break;
//            default:
//                break;
//        }
//        // If we are in Patrol State...
//        // Call Patrol()
//        // If we are in Seek State...
//        // Call Seek()
//    }
//    #endregion Update
//}
