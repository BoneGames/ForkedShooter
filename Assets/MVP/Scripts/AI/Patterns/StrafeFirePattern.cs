using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

[CreateAssetMenu(fileName = "StrafeFire Pattern", menuName = "Patterns/StrafeFire")]
public class StrafeFirePattern : Pattern
{
    public float strafeLength;
    public float moveHeight;
    float startHeight = 3;

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
        // get destination
        Vector3 moveTarget = ai.transform.position + Random.insideUnitSphere * strafeLength;
        // choose closer strafe target each time
        while (Vector3.Distance(moveTarget, data.targets[0].position) > Vector3.Distance(ai.transform.position, data.targets[0].position))
        {
            moveTarget = ai.transform.position + Random.insideUnitSphere * strafeLength;
            Debug.Log("strafeMoveTargetChanged");
        }
        // set destination.y
        moveTarget.y = moveHeight;
        // start height lerp method
        ai.HoverHeight(moveTarget.y);

        //Debug.Log("Strafing");
        //GameObject sphere = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //sphere.transform.position = moveTarget;
        //sphere.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);


        ai.agent.SetDestination(moveTarget);
        // shoot
        ai.ShootAt(data.targets[0]);
    }

    public override void UpdatePattern(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {
        StrafeCycle(ai, data);
    }

    public override void KillPattern(BehaviourAI ai)
    {
        base.KillPattern(ai);
        ai.HoverHeight(startHeight);
    }
}


//cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
//        cube.transform.position = moveTarget;