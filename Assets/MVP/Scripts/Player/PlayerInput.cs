using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public RigidCharacterMovement player;
    public int weaponIndex = 0;

    // Use this for initialization
    void Start()
    {
        player = GetComponent<RigidCharacterMovement>();

        player.SelectWeapon();
    }

    

    // Update is called once per frame
    void Update()
    {
        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");
        player.Move(inputH, inputV);

        if (Input.GetKeyDown(KeyCode.Space))
        {
            player.Jump();
        }

        if (Input.GetButtonDown("Fire1"))
        {
            player.Attack();
        }

        if (Input.GetAxis("Mouse ScrollWheel") > 0f && weaponIndex > 0) // forward
        {
            weaponIndex -= 1;
            weaponSwitch();
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && weaponIndex < player.weapons.Length - 1) // backwards
        {
            weaponIndex += 1;
            weaponSwitch();
        }


        // weaponSwitch();
    }

    void weaponSwitch()
    {
        var currentIndex = weaponIndex;

        // if (Input.GetAxis("Mouse ScrollWheel") > 0f && weaponIndex > 0) // forward
        // {
        //     weaponIndex -= 1;
        // }
        // else if (Input.GetAxis("Mouse ScrollWheel") < 0f && weaponIndex < player.weapons.Length - 1) // backwards
        // {
        //     weaponIndex += 1;
        // }

        if (Input.GetKeyDown(KeyCode.E))
        {
            player.Interact();
        }

        //if (Input.GetKeyDown(KeyCode.Q) && weaponIndex > 0)
        //{
        //    weaponIndex -= 1;
        //}
        //if (Input.GetKeyDown(KeyCode.E) && weaponIndex < player.weapons.Length -1)
        //{
        //    weaponIndex += 1;
        //}

        if (currentIndex != weaponIndex)
        {
            return;
        }
        
        else
        {
            weaponIndex = currentIndex;
            //bool[] WeaponsBoolsCopy = new bool[3];
			for(int bo = 0;bo < player.WeaponsBools.Length; bo++)
			{
				if(bo == weaponIndex)
				{
					player.WeaponsBools[bo] = true;
				}
				else
				{
					player.WeaponsBools[bo] = false;
				}
			}
			//player.WeaponsBools = WeaponsBoolsCopy;
            player.GetComponent<PhotonView>().RPC("SelectWeapon", PhotonTargets.All);
            //player.WeaponsBools[weaponIndex] = true;
        }
    }
}
