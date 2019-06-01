//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class AI_Combat : BehaviourAI  
//{

//    Vector3 strafeDir = Vector3.up;
//    float strafeTimer, strafeTime, strafeSpeed;
//    void OnEnable()
//    {
//        agent.speed = moveSpeed[2];
//        shootTimer = shootDelay;
//        intensity = 0;
//    }

//    void Charge()
//    {
//        // Retreat to totem if health is lower than 25
//        if (healthRef.currentHealth < 30)
//        {
//            currentState = State.Totem;
//        }

     

       
//            // Aim gun at the target.
//            hand.LookAt(playerTarget.position);

//            //Debug.Log("innacuarcy: "+accuracyOffset);

//            // Get distance between enemy and player/target.
//            float seekDistance = agent.remainingDistance;

//            // Move to specified position under set conditions.
//            #region Agent Destinations
//            agent.SetDestination(playerTarget.position);
//            if (seekDistance >= stoppingDistance[2] - 0.5f && seekDistance <= stoppingDistance[2] + 0.5f)
//            {
//                Debug.Log("strafe");
//                Strafe();
//                if (agent.hasPath)
//                {
//                    agent.ResetPath();
//                }
//            }
//            //else if (DestinationReached(1))
//            //{
//            //    Debug.Log("Melee");
//            //}

//            #endregion
        
     
//    }

//    public void Strafe()
//    {
//        strafeTimer += Time.deltaTime;
//        //Debug.Log("strafeTimer: " + strafeTimer);
//        if (strafeTimer > strafeTime)
//        {
//            // Change strafe Direction
//            strafeDir *= -1;
//            // Set Time to strafe in current direction
//            strafeTime = Random.Range(1f, 3f);
//            // Set Strafe speed
//            strafeSpeed = Random.Range(6f, 14f);

//            strafeTimer = 0;
//        }
//        if (healthRef.currentHealth > 50)
//        {
//            transform.RotateAround(playerTarget.position, strafeDir, strafeSpeed * Time.deltaTime);
//        }
//        else
//        {
//            agent.updateRotation = false;
//            //Vector3 waypoint = GetAvoidanceWaypoint();
//            agent.SetDestination(waypoint);
//            transform.LookAt(playerTarget.position);
//        }
//        agent.updateRotation = true;
//    }

//    void CoverShoot()
//    {
//        // after retreat - go to hide (for pause) - then go to shoot - return to hide

//        // move to side (beyond obstacle)

//        // turn to look at last player position (investigatePoint)

//        // if player there, Shoot()

//        // if enemy hit - return to cover (retreat?)

//        // repeat
//    }

//    //public void Retreat()
//    //{
//    //    Vector3 retreatPoint = GetAvoidanceWaypoint();
//    //    agent.SetDestination(retreatPoint);
//    //    if (agent.remainingDistance < 0.5f)
//    //    {
//    //        currentState = State.Hide;
//    //    }
//    //}

//    public override bool LookForPlayer()
//    {
//        return base.LookForPlayer();
//    }

//    // Update is called once per frame
//    void Update()
//    {
//        if (!LookForPlayer())
//        {
//            ModeSwitch(false);
//            return;
//        }
//        shootTimer -= Time.deltaTime;

//        switch(intensity)
//        {
//            case 0:

//                break;
//            case 1:

//                break;
//            case 2:

//                break;
//            case 3:

//                break;
//            case 4:

//                break;
//            case 5:

//                break;
//            case 6:

//                break;
//            case 7:

//                break;
//        }
//    }
//}
