using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class teleport : MonoBehaviour
{
  public Transform enter, exit;

  public void Warp()
  {
    enter.position = exit.position;
  }
}
