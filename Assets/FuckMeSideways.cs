using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FuckMeSideways : MonoBehaviour
{
  public NavMeshAgent agent;
  public BehaviourAI ai;

  float onMeshThreshold = 5;

  public bool IsAgentOnNavMesh(Transform agentObject)
  {
    Vector3 agentPosition = agentObject.transform.position;
    NavMeshHit hit;

    // Check for nearest point on navmesh to agent, within onMeshThreshold
    if (NavMesh.SamplePosition(agentPosition, out hit, onMeshThreshold, NavMesh.AllAreas))
    {
      // Check if the positions are vertically aligned
      if (Mathf.Approximately(agentPosition.x, hit.position.x)
          && Mathf.Approximately(agentPosition.z, hit.position.z))
      {
        // Lastly, check if object is below navmesh
        return agentPosition.y >= hit.position.y;
      }
    }

    return false;
  }

  private void Start()
  {
    ai = GetComponent<BehaviourAI>();
    agent = GetComponent<NavMeshAgent>();

    ai.enabled = false;
  }
  private void Update()
  {
    if (IsAgentOnNavMesh(this.transform))
    {
      ai.enabled = true;
      print("On one now, boyo");

    }
    else
    {
      ai.enabled = false;
      print("Not on one yet, boyo");
    }
  }
}
