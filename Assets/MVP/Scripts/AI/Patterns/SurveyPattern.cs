using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Survey Pattern", menuName = "Patterns/Survey")]
public class SurveyPattern : Pattern
{
    public override void StartPatternWith(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {
        base.StartPatternWith(ai, data);
        //public void Survey()
        //{
        //    if (initVar)
        //    {
        //        startRotation = transform.rotation;
        //        // clear path
        //        if (agent.hasPath)
        //        {
        //            agent.ResetPath();
        //        }
        //    }
        //    RaycastHit hit;
        //    float seeingDist = 1;

        //    // spin speed is relative to length of sightLine
        //    if (Physics.Raycast(transform.position, transform.forward, out hit, Mathf.Infinity))
        //    {
        //        seeingDist = Vector3.Distance(transform.position, hit.point) / 4;
        //    }
        //    // spin to check surroundings
        //    transform.Rotate(Vector3.up * Time.deltaTime * 300 / seeingDist);
        //    // after 1 full revolution
        //    if (transform.rotation.eulerAngles.y > startRotation.eulerAngles.y - 5 && transform.rotation.eulerAngles.y < startRotation.eulerAngles.y)
        //    {
        //        currentState = State.Patrol;
        //        return;
        //    }
        //    initVar = false;
        //}
    }

}
