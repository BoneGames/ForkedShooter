using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InstantiatePrefab : MonoBehaviour
{
  public GameObject prefab;
  Transform caster;

  public void InstantiateObject(bool followRotation)
  {
    caster = this.transform;
    GameObject p = Instantiate(prefab);

    p.transform.position = transform.position;
    if (followRotation)
    {
      p.transform.localRotation = caster.rotation;
    }
  }
}
