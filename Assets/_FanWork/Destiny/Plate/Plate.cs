using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using BT;
using NaughtyAttributes;

public class Plate : OnTriggerEvent
{
  [BoxGroup("Stats")]
  public float currentProgress = 0f;
  [BoxGroup("Stats")]
  public float targetProgress = 100f;
  [BoxGroup("Stats")]
  public Vector3 targetScale;
  [BoxGroup("Stats")]
  public Vector3 oldScale;

  [BoxGroup("References")]
  public GameObject scaleUpThing;

  [BoxGroup("Output")]
  public UnityEvent result;

  [BoxGroup("Debug")]
  [ReadOnly]
  public float percentComplete;
  [BoxGroup("Debug")]
  public bool targetReached = false;
  [BoxGroup("Debug")]
  public bool isPressed;

  void Start()
  {
    targetReached = false;
    isPressed = false;
    scaleUpThing.transform.localScale = oldScale;
  }

  public override void OnTriggerEnter(Collider other)
  {
    isPressed = true;
  }

  public override void OnTriggerExit(Collider other)
  {
    isPressed = false;
  }

  public override void OnTriggerStay(Collider other)
  {
    if (!targetReached)
    {
      base.OnTriggerStay(other);
    }
  }

  public void PlateProgression(float _mult)
  {
    if (currentProgress <= targetProgress)
    {
      ProgressUp(_mult);
    }
    else
    {
      TargetReached();
    }
  }

  void ProgressUp(float _mult)
  {
    if (!isPressed)
    {
      isPressed = true;
    }

    currentProgress += Time.deltaTime * _mult;
    percentComplete = (currentProgress / targetProgress) * 100;

    //Do stuff here
    scaleUpThing.transform.localScale = Vector3.Lerp(oldScale, targetScale, percentComplete / 100);
  }

  void ProgressDown(float _mult)
  {
    currentProgress -= Time.deltaTime * _mult;
    percentComplete = (currentProgress / targetProgress) * 100;

    //Do stuff here
    scaleUpThing.transform.localScale = Vector3.Lerp(oldScale, targetScale, percentComplete / 100);
  }

  void TargetReached()
  {
    targetReached = true;
    print(string.Format(BaneTools.ColorString("Plate Target Reached!", BaneTools.Color255(255, 100, 100))));

    //Do stuff here
    result.Invoke();
  }

  [Button]
  void ResetPlate()
  {
    targetReached = false;
    isPressed = false;
    percentComplete = 0;
    currentProgress = 0;
    scaleUpThing.transform.localScale = oldScale;
  }

  public void TestSuccess()
  {
    print(string.Format(BaneTools.ColorString("Called successfully!", BaneTools.Color255(100, 255, 100))));
  }
}
