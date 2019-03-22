using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInput : MonoBehaviour
{
    public RigidCharacterMovement player;

    [Header("Motion Keys")]
    public KeyCode sprintKey = KeyCode.LeftShift;
    public KeyCode crouchKey = KeyCode.LeftControl;
    public KeyCode jumpKey = KeyCode.Space;

    [Header("Gun Keys")]
    public KeyCode shootKey = KeyCode.Mouse0;
    public KeyCode aimKey = KeyCode.Mouse1;
    public KeyCode reloadKey = KeyCode.R;

    public int weaponIndex = 0;

    // Use this for initialization
    void Start()
    {
        player = GameObject.Find("Player").GetComponent<RigidCharacterMovement>();
        player.SelectWeapon(weaponIndex);
    }

    // Update is called once per frame
    void Update()
    {
        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");
        player.Move(inputH, inputV);

        if (Input.GetKeyDown(jumpKey))
        {
            player.Jump();
        }

        if (Input.GetKeyDown(crouchKey))
        {
            player.Crouch();
        }

        if (Input.GetKey(sprintKey))
        {
            player.isSprinting = true;
        }
        if (Input.GetKeyUp(sprintKey))
        {
            player.isSprinting = false;
        }

        if (Input.GetKeyDown(shootKey))
        {
            player.Attack();
        }
        if (Input.GetKeyDown(aimKey))
        {
            player.Aim(true);
        }
        if (Input.GetKeyUp(aimKey))
        {
            player.Aim(false);
        }
        if (Input.GetKeyDown(reloadKey))
        {
            player.Reload();
        }


        weaponSwitch();
    }

    void weaponSwitch()
    {
        var currentIndex = weaponIndex;

        if (Input.GetAxis("Mouse ScrollWheel") > 0f && weaponIndex > 0) // forward
        {
            weaponIndex -= 1;
        }
        else if (Input.GetAxis("Mouse ScrollWheel") < 0f && weaponIndex < player.weapons.Length - 1) // backwards
        {
            weaponIndex += 1;
        }

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
            player.SelectWeapon(weaponIndex);
        }
    }
}
