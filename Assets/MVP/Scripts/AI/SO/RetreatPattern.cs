using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "Retreat Pattern", menuName = "Patterns/Retreat")]
public class RetreatPattern : Pattern
{


    public override void StartPatternWith(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {
        base.StartPatternWith(ai, data);
           if(data.targets.Count == 0)
        {
            Debug.Log("not enemey to hide from;");
            return;
        }
            Vector3 retreatPoint = ai.GetAvoidanceWaypoint(data.targets[0]);
            ai.agent.SetDestination(retreatPoint);
        Debug.Log("retreat pattern has started");

    }

    public override void UpdatePattern(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {
        Debug.Log("retreat - UpdatePattern");
        base.UpdatePattern(ai, data);
        if (ai.DestinationReached(0.2f))
        {
            StopPattern();
            Debug.Log("retreat pattern is stopped");
        }
    }


}
