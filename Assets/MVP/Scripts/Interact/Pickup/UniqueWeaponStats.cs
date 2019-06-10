using UnityEngine;
using System.Reflection;

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

    //public Elements.Element weaponElement;

    public UniqueWeaponStats(float variance)
    {
        RandomElement();
        FieldInfo[] fields = this.GetType().GetFields();

        for (int i = 0; i < fields.Length; i++)
        {
            object t = fields[i].GetValue(this);
            if (t is float)
            {
                float multi = Random.Range(1 + variance, 1 - variance);
                fields[i].SetValue(this, multi);
                continue;
            }
            else
            {
                fields[i].SetValue(this, Elements.Element.Fire);
            }
        }
    }

    void RandomElement()
    {

    }
}
