using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Retreat Pattern", menuName = "Patterns/Retreat")]
public class RetreatPattern : Pattern
{
    Vector3 retreatPoint;
    public bool rotated = false;
    public override void StartPatternWith(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {
        base.StartPatternWith(ai, data);
        retreatPoint = ai.GetAvoidanceWaypoint(data.targets[0].position);
        ai.agent.SetDestination(retreatPoint);
    }

    public override void UpdatePattern(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {
        if(data.targets.Count != 0)
        {
            retreatPoint = ai.GetAvoidanceWaypoint(data.targets[0].position);
            ai.agent.SetDestination(retreatPoint);
        }
        if (ai.DestinationReached(0.1f))
        {
            if(!rotated)
            {
                retreatPoint = ai.transform.position + (data.targetLastSeen - ai.transform.position).normalized;
                ai.agent.SetDestination(retreatPoint);
                rotated = true;
            }
            else
            {
                KillPattern(ai);
                rotated = false;
                return;
            }
        }
    }
}
