using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "StrafeFire Pattern", menuName = "Patterns/StrafeFire")]
public class StrafeFirePattern : Pattern
{
    public float strafeLength;
    public bool looking = true;
    public Transform target;
    GameObject cube;
    
    public override void StartPatternWith(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {
        base.StartPatternWith(ai, data);
        // get target
        target = data.targets[0];

        // stop navMesh from automating rotation
        if(ai.agent.updateRotation)
        ai.agent.updateRotation = false;

        // get strafe move-to position
        Vector3 moveTarget = ai.transform.position + Random.insideUnitSphere * strafeLength;
        moveTarget.y = 0;

        // move to position & start coroutine to rotate to player
        ai.agent.SetDestination(moveTarget);
        ai.playerTarget = target;
        ai.lookAtTarget = true;

        // shoot
        ai.ShootAt(target.position);
    }

    

    public override void UpdatePattern(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {
        Vector3 moveTarget = ai.transform.position + Random.insideUnitSphere * strafeLength;
        moveTarget.y = 0;
        ai.agent.SetDestination(moveTarget);

        target = ai.playerTarget = data.targets[0];
        // shoot
        ai.ShootAt(target.position);
    }

    public override void PatternHasBeenInterrupted(BehaviourAI ai)
    {
        base.PatternHasBeenInterrupted(ai);
        ai.agent.updateRotation = true;
        ai.playerTarget = null;
        ai.lookAtTarget = false;
        looking = false;
        target = null;
    }
}


//cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//        cube.transform.position = moveTarget;