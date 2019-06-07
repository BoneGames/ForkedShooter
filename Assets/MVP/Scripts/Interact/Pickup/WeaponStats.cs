using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponStats : MonoBehaviour
{
    public bool initialized;

    public int damage;
    public int maxReserves;
    public int currentReserves;
    public int magSize;
    public int currentMag;

    public float accuracy;
    public float loudness;
    public float bulletDetectionRadius;
    public float scopeZoom;
    public float reloadSpeed;
    public float rateOfFire;

    public Elements.Element weaponElement;

    public WeaponStats()
    {

    }
}
