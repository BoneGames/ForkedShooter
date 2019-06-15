using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldController : MonoBehaviour
{
  public Material shieldMat;
  public Color[] shieldColor;

  private void Start()
  {
    shieldMat = GetComponent<Renderer>().material;
  }

  public void SetShieldElement(Elements.Element _shieldElement)
  {
        if(!shieldMat)
        {
            shieldMat = GetComponent<Renderer>().material;
        }
    int elementIndex = (int)_shieldElement;
    shieldMat.SetColor("_ShieldPatternColor", shieldColor[elementIndex]);
  }
}
