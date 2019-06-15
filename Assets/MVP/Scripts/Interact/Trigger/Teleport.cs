using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Teleport : OnTriggerEvent
{
  public Transform pointA, pointB;

  public void Warp()
  {
    otherCollider.transform.position = pointB.position;
    //enter.position = exit.position;
  }
}
