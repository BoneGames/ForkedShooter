using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Investigate Pattern", menuName = "Patterns/Investigate")]
public class InvestigatePattern : Pattern
{
    Vector3 investigationPoint;
    public override void StartPatternWith(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {
        base.StartPatternWith(ai, data);

        // get target
        investigationPoint = data.targetLastSeen != null ? data.targetLastSeen : data.inspectionPoints[0];

        // move to position & start coroutine to rotate to player
        ai.agent.SetDestination(investigationPoint);
    }

    public override void UpdatePattern(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {
        if (ai.DestinationReached(0.5f))
        {
            if (investigationPoint == ai.sMF.targetLastSeen)
            {
                Debug.Log("TLS set to Vector3.zero");
                ai.sMF.targetLastSeen = Vector3.zero;
                PatternHasEnded();
                return;
            }
            else if (investigationPoint == data.inspectionPoints[0])
            {
                Debug.Log("TLS set to Vector3.zero");
                ai.sMF.inspectionPoints.RemoveAt(0);
                if (ai.sMF.inspectionPoints.Count < 1)
                {
                    PatternHasEnded();
                    return;
                }
            }
        }
    }

    public override void PatternHasEnded()
    {
        base.PatternHasEnded();
    }

    public override void PatternHasBeenInterrupted(BehaviourAI ai)
    {
        base.PatternHasBeenInterrupted(ai);

    }
}
