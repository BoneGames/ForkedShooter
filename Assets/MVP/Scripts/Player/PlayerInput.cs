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
    
    // Use this for initialization
    void Start()
    {
        player = GetComponent<RigidCharacterMovement>();
        player.SelectWeapon(0);
        // PHOTON SYNC WEAPON IN-PROGRESS
        //player.SelectWeapon();
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

        if (Input.GetKeyDown(KeyCode.E))
        {
            player.Interact();
        }


        float inputScroll = Input.GetAxisRaw("Mouse ScrollWheel");
        if (inputScroll != 0)
        {
            player.SelectWeapon((int)inputScroll);
        }

    }
    
    // PHOTON SYNC WEAPON IN_PROGRESS
    // for(int bo = 0;bo < player.WeaponsBools.Length; bo++)
    // {
    // 	if(bo == weaponIndex)
    // 	{
    // 		player.WeaponsBools[bo] = true;
    // 	}
    // 	else
    // 	{
    // 		player.WeaponsBools[bo] = false;
    // 	}
    // }
    // player.GetComponent<PhotonView>().RPC("SelectWeapon", PhotonTargets.All);

}
