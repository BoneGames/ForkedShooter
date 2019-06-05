using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "StrafeFire Pattern", menuName = "Patterns/StrafeFire")]
public class StrafeFirePattern : Pattern
{
    public float strafeLength;

    public override void StartPatternWith(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {
        base.StartPatternWith(ai, data);
        // stop navMesh from setting rotation
        ai.agent.updateRotation = false;
        // give target to AI
        ai.playerTarget = data.targets[0];
        // tell ai to look at target
        ai.lookAtTarget = true;

        StrafeCycle(ai, data);
    }

    public void StrafeCycle(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {
        // get target
        ai.playerTarget = data.targets[0];
        // get strafe move-to position
        Vector3 moveTarget = ai.transform.position + Random.insideUnitSphere * strafeLength;
        moveTarget.y = 0;
        // move to position & start coroutine to rotate to player
        ai.agent.SetDestination(moveTarget);
        // shoot
        ai.ShootAt(data.targets[0].position);
    }

    public override void UpdatePattern(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {
        StrafeCycle(ai, data);
    }

    public override void KillPattern(BehaviourAI ai)
    {
        base.KillPattern(ai);
    }
}


//cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//        cube.transform.position = moveTarget;