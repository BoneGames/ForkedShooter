using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorController : MonoBehaviour
{
  public bool doorStartsOpen = false;

  public Animator doorAnim;

  private void Awake()
  {
    if (doorStartsOpen)
    {
      DoorOpen(true);
    }
  }

  public void DoorOpen(bool _openState)
  {
    doorAnim.SetBool("IsOpen", _openState);
  }
}
