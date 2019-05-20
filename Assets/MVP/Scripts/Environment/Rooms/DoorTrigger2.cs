using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class DoorTrigger2 : OnTriggerEvent
{
  public UnityEvent onResetRoom;

  public void ResetRoom()
  {
    onResetRoom.Invoke();
  }
}
