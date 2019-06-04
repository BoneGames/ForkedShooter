using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatePrefab : MonoBehaviour
{
  public GameObject[] prefabs;
  Transform caster;

  public void InstantiateObject(bool followRotation)
  {
    caster = this.transform;

    for (int i = 0; i < prefabs.Length; i++)
    {
      GameObject p = Instantiate(prefabs[i]);

      p.transform.position = transform.position;
      p.transform.parent = null;
      p.transform.localScale = new Vector3(1, 1, 1);
      if (followRotation)
      {
        p.transform.localRotation = caster.rotation;
      }
    }
  }

  public void InstantiateObjectAtPoint(Transform origin)
  {
    caster = this.transform;

    for (int i = 0; i < prefabs.Length; i++)
    {
      GameObject p = Instantiate(prefabs[i], origin);

      p.transform.position = origin.position;
      p.transform.parent = null;
      p.transform.localScale = new Vector3(1, 1, 1);
    }
  }
}
