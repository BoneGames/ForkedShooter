using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Charge Pattern", menuName = "Patterns/Charge")]
public class ChargePattern : Pattern
{
    public override void StartPatternWith(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {
        base.StartPatternWith(ai, data);
        // closest target (FoV script orders targets in list)
        Vector3 target = data.targets[0];
        // Go toward player target
        ai.agent.SetDestination(target);
        
        // aim gun at target
        if(ai.hand)
        {
            //ai.hand.LookAt(target);
        }
        else
        {
            Debug.Log("You need to asign the AI Hand Transform component to aim the gun");
        }
        // fire
        ai.ShootAt(target);
    }

}
