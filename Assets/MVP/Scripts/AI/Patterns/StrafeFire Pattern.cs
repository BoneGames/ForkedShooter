using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StrafeFire Pattern", menuName = "Patterns/StrafeFire")]
public class StrafeFirePattern : Pattern
{
    public float strafeLength;
    
    public override void StartPatternWith(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {
        base.StartPatternWith(ai, data);
        // get target
        Vector3 target = data.targets[0];
        // stop navMesh from automating rotation
        if(ai.agent.updateRotation)
        ai.agent.updateRotation = false;

        // get strafe move-to position
        Vector3 moveTarget = Random.insideUnitSphere * strafeLength;
        moveTarget.y = 0;

        // move to position & start coroutine to rotate to player
        ai.agent.SetDestination(moveTarget);
        ai.RotateToward(target);

        // shoot
        ai.ShootAt(target);
    }
}
