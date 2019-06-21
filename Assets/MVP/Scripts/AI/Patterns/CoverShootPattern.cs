using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New CoverShoot Pattern", menuName = "Patterns/CoverShoot")]
public class CoverShootPattern : Pattern
{
    Vector3 coverPoint;
    public int playerChecks = 0;
    public override void StartPatternWith(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {
        base.StartPatternWith(ai, data);

        //ai.Crouch(true);

        // look at player while moving
        ai.playerTarget = data.targets[0];
        ai.agent.updateRotation = false;
        ai.lookAtTarget = true;

        // Covershoot sequence
        CoverShootCycle(ai, data);
    }

    void CoverShootCycle(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {
        Transform enemyTarget = data.targets[0];

        // get cover point
        coverPoint = ai.GetAvoidanceWaypoint(data.targets[0].position);

        // move to cover point
        ai.agent.SetDestination(coverPoint);

        // fire while moving
        ai.ShootAt(enemyTarget);
    }

    public override void UpdatePattern(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {
        base.UpdatePattern(ai, data);
        if(data.targets.Count == 0)
        {
            switch(playerChecks)
            {
                                                                    // 1st check:
                case 0:
                    ai.agent.SetDestination(data.targetLastSeen);   // look for target
                    Debug.Log("CS check: " + playerChecks);
                    break;
                                                                    // 2nd check:
                case 1:
                    ai.agent.SetDestination(coverPoint);            // return to cover
                    Debug.Log("CS check: " + playerChecks);
                    break;
                                                                    // 3rd check:
                case 2:
                    ai.agent.SetDestination(data.targetLastSeen);   // look for target
                    Debug.Log("CS check: " + playerChecks);
                    break;
                                                                    // 4th/Final check:
                case 3:
                    KillPattern(ai);                                // End Behaviour
                    Debug.Log("CS check: " + playerChecks);
                    return;
                default:
                    KillPattern(ai);
                    Debug.Log("CS check: " + playerChecks);
                    return;
            }
             playerChecks++;
        }
        else
        {
            playerChecks = 0;
            CoverShootCycle(ai, data);
        }
    }

    public override void KillPattern(BehaviourAI ai)
    {
        base.KillPattern(ai);
        playerChecks = 0;
    }
}
