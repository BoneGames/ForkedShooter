using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldControllerTest : MonoBehaviour
{
  Material shieldMat;
  public Color[] shieldColor;

  public int elementNumber;
  private void Start()
  {
    shieldMat = GetComponent<Renderer>().material;
  }
  private void Update()
  {
  }
}
