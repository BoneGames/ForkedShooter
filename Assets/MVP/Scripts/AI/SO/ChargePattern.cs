using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Charge Pattern", menuName = "Pattern/Charge")]
public class ChargePattern : Pattern
{
    public override void StartPatternWith(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {
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
    }

}
