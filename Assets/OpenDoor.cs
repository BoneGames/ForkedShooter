using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenDoor : MonoBehaviour
{
  public Animator doorAnim;

  private void Start()
  {
    doorAnim.SetTrigger("Enter");
  }

  public void TriggerDoor()
  {
    doorAnim.SetBool("Exit", true);
  }
}
