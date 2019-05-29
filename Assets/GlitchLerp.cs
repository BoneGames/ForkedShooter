using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

public class GlitchLerp : MonoBehaviour
{
  public AnalogueGlitch glitch;

  public float lerpTime;
  [Slider(0,1)]
  public float startLevel;
  [Slider(0, 1)]
  public float endLevel;

  public float lerpTimer;
  private void Start()
  {
    glitch = GetComponent<AnalogueGlitch>();
    lerpTimer = 1;
  }

  private void Update()
  {
    if (lerpTimer >= 0)
    {
      lerpTimer -= (Time.deltaTime * lerpTime);

      glitch._HorizontalShake = Mathf.Lerp(endLevel, startLevel, lerpTimer);
      glitch._ScanLineJitter = Mathf.Lerp(endLevel, startLevel, lerpTimer);
      glitch._ColourDrift = Mathf.Lerp(endLevel, startLevel, lerpTimer);
    }
  }
}
