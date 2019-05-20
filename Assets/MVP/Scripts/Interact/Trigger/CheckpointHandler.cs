using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

public class CheckpointHandler : MonoBehaviour
{
  public UnityEvent onResetRoom;

  public void ResetMyRoom()
  {
    onResetRoom.Invoke();
  }
}
