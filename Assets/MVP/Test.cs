using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using GameSystems;

public class Test : MonoBehaviour
{
    public Weapon currentWeapon;

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.P))
        {
            currentWeapon.Reload();
        }
    }
}
