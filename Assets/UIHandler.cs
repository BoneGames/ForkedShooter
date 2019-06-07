using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;

public class UIHandler : MonoBehaviour
{
    public bool ShowElementColors;
    [BoxGroup("Element Colors"), ShowIf("ShowElementColors")]
    public Color normal, fire, water, grass;
    [HideInInspector]public ShotDirection shotDirection;
    [HideInInspector]public DeathMessage deathMessage;


    public virtual void Awake()
    {
        shotDirection = GetComponentInChildren<ShotDirection>();
        deathMessage = GetComponentInChildren<DeathMessage>();
    }

    public void SetColors()
    {
        //normal = normal;

    }

   

}
