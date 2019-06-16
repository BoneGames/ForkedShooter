using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;
using NaughtyAttributes;

public class EnemySpawner : MonoBehaviour
{
  public UnityEvent onChildrenDead;

  public Transform waypointParent, droneSpawnPoint, guardSpawnPoint;

  public GameObject drone, guard;

  bool dronesSpawned, guardsSpawned;
  public bool roomCleared;

  void Update()
  {
    if (transform.childCount <= 0 && !roomCleared && (dronesSpawned || guardsSpawned))
    {
      print("Children of room died");
      ChildrenDied();
    }
  }

  void ChildrenDied()
  {
    roomCleared = true;
    onChildrenDead.Invoke();
  }

  public void SpawnDrone(int _count)
  {
    if (!dronesSpawned)
    {
      for (int i = 0; i < _count; i++)
      {
        //print("I was called!");
        GameObject clone = Instantiate(drone, droneSpawnPoint.position, droneSpawnPoint.rotation, droneSpawnPoint);
        clone.transform.name += i.ToString();
        clone.transform.parent = transform;
        clone.transform.localScale = new Vector3(1, 1, 1);

        print(string.Format("The spawned enemy's name is {0}, they are parented to {1}, and their scale is {2}", clone.transform.name, clone.transform.parent, clone.transform.localScale));
        
        clone.GetComponent<BehaviourAI>().waypointParent = waypointParent;
      }
      dronesSpawned = true;
    }
  }
  public void SpawnGuard(int _count)
  {
    if (!guardsSpawned)
    {
      for (int i = 0; i < _count; i++)
      {
        //print("I was called!");
        GameObject clone = Instantiate(guard, guardSpawnPoint.position, guardSpawnPoint.rotation, guardSpawnPoint);
        clone.transform.name += i.ToString();
        clone.transform.parent = transform;
        clone.transform.localScale = new Vector3(1, 1, 1);

        print(string.Format("The spawned enemy's name is {0}, they are parented to {1}, and their scale is {2}", clone.transform.name, clone.transform.parent, clone.transform.localScale));

        clone.GetComponent<BehaviourAI>().waypointParent = waypointParent;
      }
      guardsSpawned = true;
    }
  }

  [Button]
  public void ResetSpawner()
  {
    if (transform.childCount > 0)
    {
      while (transform.childCount > 0)
      {
        Transform child = transform.GetChild(0);
        child.parent = null;
        Destroy(child.gameObject);
      }
    }

    dronesSpawned = false;
    guardsSpawned = false;
    roomCleared = false;
  }
}
