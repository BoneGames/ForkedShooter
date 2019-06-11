using UnityEngine;
using System.Collections.Generic;
using System.Reflection;
using System;

public class UniqueWeaponStats : MonoBehaviour
{

    public float damage;
    public float maxReserves;
    public float magSize;

    public float accuracy;
    public float loudness;
    public float scopeZoom;
    public float reloadSpeed;
    public float rateOfFire;

    public Dictionary<string, float> baseStats = new Dictionary<string, float>();

    public Elements.Element weaponElement;

    public UniqueWeaponStats(float variance)
    {
        FieldInfo[] fields = this.GetType().GetFields();

        for (int i = 0; i < fields.Length; i++)
        {
            //Debug.Log(this.GetType() + " UWS: " + i);
            object t = fields[i].GetValue(this);
            if (t is Dictionary<string, float>)
            {
                continue;
            }
            else if (t is float)
            {
                float multiplier = UnityEngine.Random.Range(1f + variance, 1f - variance);
                fields[i].SetValue(this, multiplier);
                continue;
            }
            else
            {
//                Debug.Log("SETTING ENUM");
                int randomElement = UnityEngine.Random.Range(0, 4);
                fields[i].SetValue(this, (Elements.Element.Water));
            }
        }
        Debug.Log("WE: " + weaponElement);
    }

}
