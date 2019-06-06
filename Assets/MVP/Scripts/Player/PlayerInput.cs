using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using BT;
using NaughtyAttributes;

public class PlayerInput : Photon.PunBehaviour
{
    public RigidCharacterMovement player;

    public bool showMotionKeys;

    [ShowIf("showMotionKeys")]
    [BoxGroup("Motion Keys")]
    public KeyCode sprintKey = KeyCode.LeftShift, crouchKey = KeyCode.LeftControl, jumpKey = KeyCode.Space;

    public bool showGunKeys;
    [ShowIf("showGunKeys")]
    [BoxGroup("Gun Keys")]
    public KeyCode shootKey = KeyCode.Mouse0, aimKey = KeyCode.Mouse1, reloadKey = KeyCode.R;

    public bool showActionKeys;
    [ShowIf("showActionKeys")]
    [BoxGroup("Action Keys")]
    public KeyCode interactKey = KeyCode.E, ammoKey = KeyCode.G;

    private int weaponIndex;
    private int currentIndex;

    #region Unity Events
    // Use this for initialization
    void Start()
    {
        player = GetComponent<RigidCharacterMovement>();
    }
    // Update is called once per frame
    void Update()
    {
        if (photonView != null)
        {
            if (photonView.isMine)
            {
                ProcessInputs();
            }
        }
        else //we nust not have the Photon stuff in the scene, so we don't care about networking
        {
            ProcessInputs();
        }
    }
    #endregion
  
    private void ProcessInputs()
    {
        if (!player.isDead)
        {
            float inputH = Input.GetAxis("Horizontal");
            float inputV = Input.GetAxis("Vertical");
            player.Move(inputH, inputV);

            if (Input.GetKeyDown(jumpKey)) player.Jump();
            if (Input.GetKeyDown(crouchKey)) player.Crouch();
            if (Input.GetKey(sprintKey)) player.isSprinting = true;
            if (Input.GetKeyUp(sprintKey)) player.isSprinting = false;

            if (player.currentWeapon.rateOfFire == 0 && Input.GetKeyDown(shootKey))
                player.Attack();

            //if (player.currentWeapon.rateOfFire != 0 && Input.GetKey(shootKey))
            //{
            //    fire1 = true;
            //    if (fire1)
            //    {
            //        player.Attack();
            //    }
            //}
            bool fire1 = Input.GetButton("Fire1");
            if (fire1 && player.currentWeapon.currentMag > 0 && player.currentWeapon.canShoot == true)
            {
                player.Attack();
            }
            if (Input.GetKeyUp(aimKey)) player.Aim(false); 
            if (Input.GetKeyDown(aimKey)) player.Aim(true);
            if (Input.GetKeyDown(reloadKey)) player.Reload();

            float inputScroll = Input.GetAxisRaw("Mouse ScrollWheel");
            if (inputScroll != 0)
            {
                inputScroll = inputScroll < 0 ? -1 : 1;

                // Note (Manny): Just changed this a bit.
                int direction = (int)inputScroll;
                player.SwitchWeapon(direction);
            }
        }

        if (Input.GetKeyDown(interactKey)) player.Interact();
        if (Input.GetKeyDown(ammoKey)) player.FreeAmmo();
    }
}
