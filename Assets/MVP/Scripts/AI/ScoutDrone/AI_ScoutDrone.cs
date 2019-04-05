using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AI_ScoutDrone : BehaviourAI
{
    #region VARIABLES
    [Header("Animations")]
    public Animator anim;
    
    [Header("SearchLight")]
    public Light searchLight; // Reference Light (child 'SearchLight').
    // Colours! Switching searchlight colour during different states (names are self explanatory).
    public Color colorPatrol = Color.white;
    public Color colorSearch = new Color(0.8039216f - 0 / 100, 0.4019608f - 0 / 100, 0);
    public Color colorSeek = new Color(0.8039216f - 0 / 100, 0, 0);
    #endregion VARIABLES

    #region STATES
    // The contained variables for the Patrol state (what rules the enemy AI follows when in 'Patrol').
    public override void Patrol()
    {
        base.Patrol();

        // Current animation (Patrol) and SearchLight Color.
        anim.SetBool("hasTarget", false);
        anim.SetBool("isAlert", false);
        searchLight.color = colorPatrol;
    }

    // The contained variables for the Seek state (what rules the enemy AI follows when in 'Seek').
    public override void Seek()
    {
        base.Seek();

        if (fov.visibleTargets.Count < 1)
        {
            // Current animation (Search) and SearchLight Color.
            anim.SetBool("hasTarget", false);
            anim.SetBool("isAlert", true);
            searchLight.color = colorSearch;
        }

        if (fov.visibleTargets.Count > 0)
        {
            // Current animation (Seek) and SearchLight Color.
            anim.SetBool("hasTarget", true);
            anim.SetBool("isAlert", true);
            searchLight.color = colorSeek; 
        }
    }
    #endregion

    ///public void Investigate(Vector3 position)
    ///{
    ///    // Agent navigation speed.
    ///    agent.speed = moveSpeed[2];
    ///
    ///    // Current animation and SearchLight Color.
    ///    //anim.SetBool("hasTarget", true);
    ///    //anim.SetBool("isAlert", false);
    ///    searchLight.color = colorSearch;
    ///    
    ///    agent.SetDestination(position);
    ///    currentState = State.Investigate;
    ///}

    void Aim()
    {
        if (target)
        {
            // Direction of target (player) from the aim position.
            Vector3 aimDir = target.position - aim.position;

            // If our aim is even slightly offset (not pointing directly at the target)...
            if (aimDir.magnitude > 0)
            {
                // ... rotate our aim to point straight at the target.
                aim.transform.rotation = Quaternion.LookRotation(aimDir.normalized, Vector3.up);
            }
        }
        if (fov.visibleTargets.Count < 1)
        {
            // Reset rotation of SearchLight
            aim.transform.localRotation = startRotation;
        }
    }

    public override bool Retreat()
    {
        return base.Retreat();
    }

    #region Start
    // Use this for initialization
    public override void Start()
    {
        base.Start();

        // Get Light component from child in GameObject.
        searchLight = GetComponentInChildren<Light>();
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
                Aim();
                break;
            //case State.Investigate:
            //    // Run this code while in investigate state
            //    // If the agent gets close to the investigate position
            //    if (agent.remainingDistance < stoppingDistance[0])
            //    {
            //        // Current animation (Search).
            //        anim.SetBool("hasTarget", false);
            //        anim.SetBool("isAlert", true);
            //
            //        // Note(Manny): Why not wait for 5 seconds here (timer)
            //        holdStateTimer[2] -= Time.deltaTime;
            //        if (holdStateTimer[2] <= 0)
            //        {
            //            holdStateTimer[2] = pauseDuration[2];
            //            // Switch to Patrol
            //            currentState = State.Patrol;
            //        }
            //    }
            //    else
            //    {
            //        // Current animation (Investigate).
            //        anim.SetBool("hasTarget", true);
            //        anim.SetBool("isAlert", false);
            //    }
            //
            //    // If the agent sees the player
            //    if (fov.visibleTargets.Count > 0)
            //    {
            //        // Switch over to seek
            //        currentState = State.Seek;
            //        // Seek towards the visible target
            //        target = fov.visibleTargets[0];
            //    }
            //    break;
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
