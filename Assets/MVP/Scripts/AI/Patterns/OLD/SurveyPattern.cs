using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[CreateAssetMenu(fileName = "Survey Pattern", menuName = "Patterns/Survey")]
public class SurveyPattern : Pattern
{
    float spotsToCheck = 3;
    public float spotChecks = 3, surveyLength;
    public override void StartPatternWith(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {
        base.StartPatternWith(ai, data);
        // reset spotcheck count
        spotsToCheck = spotChecks;

        Vector3 surveyPoint = GetSurveyPoint(ai);

        ai.agent.SetDestination(surveyPoint);
    }

    public override void UpdatePattern(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {
        base.UpdatePattern(ai, data);

        if (ai.DestinationReached(0.1f))
        {
            if (spotsToCheck > 0)
            {
                spotsToCheck--;
                Vector3 point = GetSurveyPoint(ai);
                ai.agent.SetDestination(point);
            }
            else
            {
                // remove inspection point
                ai.sMF.inspectionPoints.RemoveAt(0);

                // exit behaviour if no more inspection points
                if (ai.sMF.inspectionPoints.Count < 1)
                {
                    KillPattern(ai);
                    return;
                }
            }
        }
    }

    Vector3 GetSurveyPoint(BehaviourAI ai)
    {
        Vector3 seekPoint = ai.transform.position + (Random.onUnitSphere * surveyLength);
        NavMeshHit hit;

        if (NavMesh.SamplePosition(seekPoint, out hit, 10, NavMesh.AllAreas))
        {
            seekPoint = hit.position;
        }
        return seekPoint;
    }

    public override void KillPattern(BehaviourAI ai)
    {
        base.KillPattern(ai);
    }
}



// OLD CODE
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