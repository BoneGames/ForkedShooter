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

    }

    public override void UpdatePattern(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {
        Debug.Log("retreat - UpdatePattern");
        if (ai.DestinationReached(0.1f))
        {
            Debug.Log("co-routine rotate");
            ai.RotateToward(data.targetLastSeen);
            //PatternHasEnded();
        }
    }
}
