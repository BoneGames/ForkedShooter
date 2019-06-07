using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
public class DoorController : MonoBehaviour
{
  public bool doorStartsOpen = false;
  public Animator doorAnim;

  public bool doorTriggeredInitially, doorTriggeredClear;

  private void Awake()
  {
    if (doorStartsOpen)
    {
      DoorOpen(true);
    }
  }

  public void DoorOpen(bool _openState)
  {
    if (!doorTriggeredInitially || !doorTriggeredClear)
    {
      print("Triggering door!");
      doorAnim.SetBool("IsOpen", _openState);
    }
  }

  public void InitialTrigger()
  {
    doorTriggeredInitially = true;
  }

  public void ClearTrigger()
  {
    doorTriggeredClear = true;
  }

  [Button]
  public void ToggleDoor()
  {
    print("I am toggling the door!");
    doorAnim.SetBool("IsOpen", !doorAnim.GetBool("IsOpen"));
  }

  [Button]
  public void ResetDoor()
  {
    print("Resetting door!");
    doorAnim.SetBool("IsOpen", doorStartsOpen);
    doorTriggeredInitially = false;
    doorTriggeredClear = false;
  }
}
