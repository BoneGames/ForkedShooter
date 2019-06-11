﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Interactions;

using BT;
using NaughtyAttributes;

// Note (Manny): To eliminate getting components, use PunBehaviour (I did it in PlayerInput as well)
public class RigidCharacterMovement : Photon.PunBehaviour
{
    public bool showPlayerStats;
    [ShowIf("showPlayerStats")] [BoxGroup("Player Stats")] public float playerSpeed = 5f, jumpHeight = 10f, crouchMultiplier = .8f, sprintMultiplier = 1.5f, aimLerpSpeed;

    public bool showPlayerStates;
    [ShowIf("showPlayerStates")] [BoxGroup("Player States")] public bool isCrouching = false, isSprinting = false, isJumping = false, isDead = false, isAiming = false;

    [BoxGroup("Checkpoints")] public Transform lastCheckpoint;

    public bool showImportantStuff;
    [ShowIf("showImportantStuff")] [BoxGroup("Important Stuff")] public Rigidbody rigid;
    [ShowIf("showImportantStuff")] [BoxGroup("Important Stuff")] public float rayDistance = 1f;
    [ShowIf("showImportantStuff")] [BoxGroup("Important Stuff")] public Camera myCamera;
    [ShowIf("showImportantStuff")] [BoxGroup("Important Stuff")] public Transform myHand;
    [ShowIf("showImportantStuff")] [BoxGroup("Important Stuff")] public Health myHealth;
    [ShowIf("showImportantStuff")] [BoxGroup("Important Stuff")] public Weapon[] weapons;
    [ShowIf("showImportantStuff")] [BoxGroup("Important Stuff")] public Weapon currentWeapon;
    [ShowIf("showImportantStuff")] [BoxGroup("Important Stuff")] public GameObject[] pickups;
    [ShowIf("showImportantStuff")] [BoxGroup("Important Stuff")] public bool rotateToMainCamera = false;
    [ShowIf("showImportantStuff")] [BoxGroup("Important Stuff")] public int currentWeaponIndex;

    private GameObject shootPoint;
    private bool weaponRotationThing = false;
    private Vector3 moveDirection;
    private Interactable interactObject;
    private float timeTillRespawn = 5;
    private Vector3 handStartPos;

    public List<Weapon> weaponIds = new List<Weapon>();


    #region Unity Events
    void Awake()
    {
        // Note (Manny): Get into the habbit of getting components in Awake instead!
        rigid = GetComponent<Rigidbody>();
        myHealth = GetComponent<PlayerHealth>();

    }
    void Start()
    {
        handStartPos = myHand.localPosition;
        // Note (Manny): Since it's an internal function, call it on start internally
        SelectWeapon(currentWeaponIndex);

        currentWeapon.UpdateAmmoDisplay();
    }
    void OnTriggerEnter(Collider other)
    {
        interactObject = other.GetComponent<Interactable>();

        if (other.tag == "OOB")
        {
            //Respawn();
        }
        if (other.tag == "CheckPoint")
        {
            lastCheckpoint = other.gameObject.transform;
        }
    }
    void OnTriggerExit(Collider other)
    {
        if (interactObject)
        {
            interactObject = null;
            print("Should not be able to open");
        }
    }
    void Update()
    {
        if (photonView != null)
        {
            // Only control this player if it owns it on the network
            if (photonView.isMine)
            {
                PerformMotion();
            }
        }
        else //we nust not have the Photon stuff in the scene, so we don't care about networking
        {
            PerformMotion();
        }
    }

    public void Aim(bool _isAiming)
    {
        Debug.Log("AIM: " + _isAiming);
        // set script bool value from PLayerInput
        isAiming = _isAiming;
        // change weapon accuracy value
        currentWeapon.OnAim(isAiming);
        StopAllCoroutines();
        // move hand to correct position
        StartCoroutine(HandAimPos());
    }

    public IEnumerator HandAimPos()
    {
        // Set Lerp destination
        Vector3 end = isAiming ? currentWeapon.aimShootPos.localPosition : currentWeapon.hipShootPos.localPosition;
        Vector3 start = myHand.transform.localPosition;
        // Initiate Lerp
        float timer = 0;
        while (myHand.transform.localPosition != end)
        {
            timer += Time.deltaTime;
            myHand.transform.localPosition = Vector3.Lerp(start, end, timer * aimLerpSpeed);
            yield return null;
        }
    }

    #endregion

    #region Photon
    // Note (Manny): Created an RPC call that sets the weapon index only.
    [PunRPC]
    public void SelectWeaponRPC(int index)
    {
        SelectWeapon(index);
    }
    #endregion

    #region Internal
    private bool IsGrounded()
    {
        Ray groundRay = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(groundRay, out hit, rayDistance))
        {
            if (hit.collider.tag == "OOB")
            {
                return false;
            }
            return true;
        }
        return false;
    }
    private void PerformMotion()
    {
        Vector3 camEuler = Camera.main.transform.eulerAngles;

        if (rotateToMainCamera)
        {
            moveDirection = Quaternion.AngleAxis(camEuler.y, Vector3.up) * moveDirection;
            if (isSprinting && !isCrouching)
            {
                moveDirection *= sprintMultiplier;
            }
            if (isCrouching)
            {
                moveDirection *= crouchMultiplier;
            }
            if (isAiming)
            {
                moveDirection *= crouchMultiplier;
            }
        }

        Vector3 force = new Vector3(moveDirection.x, rigid.velocity.y, moveDirection.z);

        if (isJumping && IsGrounded())
        {
            force.y = jumpHeight;
            isJumping = false;
        }

        rigid.velocity = force;

        Quaternion playerRotation = Quaternion.AngleAxis(camEuler.y, Vector3.up);
        transform.rotation = playerRotation;

        if (weaponRotationThing)
        {
            Quaternion weaponRotation = Quaternion.AngleAxis(camEuler.x, Vector3.right);
            currentWeapon.transform.localRotation = weaponRotation;
        }
    }

    public void DropWeapon()
    {
        if(currentWeapon)
        {
            currentWeapon.isEquipped = false;
            
            UniqueWeaponStats statsToDrop = currentWeapon.uniqueStats;
            currentWeapon.ResetValues(statsToDrop.baseStats);
            GameObject droppedWeapon = Instantiate(pickups[currentWeaponIndex], transform.position + (transform.forward*2), Quaternion.identity);
            droppedWeapon.GetComponent<WeaponPickup>().stats = statsToDrop;
            SelectWeapon(1);
        }
    }
    private void DisableAllWeapons()
    {
        foreach (var weapon in weapons)
        {
            weapon.gameObject.SetActive(false);
        }
    }
    private void SelectWeapon(int index)
    {
        DisableAllWeapons();

        // Note (Manny): Use the incoming 'index' instead of the changed 'currentWeaponIndex' this time.
        while(!weapons[index].isEquipped)
        {
            index++;
            if(index >= weapons.Length)
            {
                Debug.Log(BaneTools.ColorString("You Have Not Equipped Any Of The Weapons! (is Equipped bool)", Color.red));
                return;
            }
        }
        currentWeapon = weapons[index];
        currentWeapon.gameObject.SetActive(true);

        // Note (Manny): Update it here for observers
        currentWeaponIndex = index;
        currentWeapon.UpdateAmmoDisplay();
    }
    #endregion

    #region External
    // Controls
    public void Move(float inputH, float inputV)
    {
        moveDirection = new Vector3(inputH, 0f, inputV);
        moveDirection *= playerSpeed;
    }
    public void Jump()
    {
        isJumping = true;
    }
    public void Crouch()
    {
        isCrouching = !isCrouching;
        if (isCrouching)
        {
            myCamera.transform.localPosition = new Vector3(0, 0f, 0);
        }
        else
        {
            myCamera.transform.localPosition = new Vector3(0, 0.5f, 0);
        }
    }
    // Actions
    public void Interact()
    {
        if (interactObject)
        {
            interactObject.Interact();
        }
    }
    public IEnumerator Respawn()
    {
        isDead = true;
        Aim(!isDead);

        float fuck = timeTillRespawn;

        for (int respawnTime = (int)timeTillRespawn; respawnTime > 0; respawnTime--)
        {
            yield return new WaitForSeconds(1);
            timeTillRespawn--;
        }
        timeTillRespawn = fuck;

        isDead = false;

        if (lastCheckpoint)
        {
            transform.position = lastCheckpoint.position;
        }
        else
        {
            // gives sense of falling back into scene
            transform.position += new Vector3(0, 5, 0);
        }

        Debug.Log("Player has died and respawned");
        myHealth.currentHealth = myHealth.maxHealth;

        //myHealth.healthBar.UpdateBar();
        myHealth.updateHealthBarEvent.Invoke(myHealth.currentHealth, myHealth.maxHealth);
    }
    public void FreeAmmo()
    {
        currentWeapon.currentReserves = 300;
        currentWeapon.UpdateAmmoDisplay();
    }


    // Combat
    public void Attack()
    {
        currentWeapon.Attack();

        // if (photonView)
        // {
        //     currentWeapon.isOnline = true;
        // }
    }
    public void Reload()
    {
        currentWeapon.Reload();
    }

    //public void Aim(bool _isAiming)
    //{
    //    isAiming = _isAiming;
    //    if (_isAiming)
    //    {
    //        //myHand.localPosition = currentWeapon.aimPoint.localPosition;
    //        currentWeapon.OnAim(isAiming);
    //    }
    //    else
    //    {
    //       // myHand.localPosition = handStartPos;
    //        currentWeapon.OnAim(isAiming);
    //    }

    //    myCamera.fieldOfView = _isAiming ? currentWeapon.scopeZoom : 75;
    //}
    public void SwitchWeapon(int direction)
    {
        currentWeaponIndex += direction;
        if (currentWeaponIndex < 0)
        {
            currentWeaponIndex = weapons.Length - 1;
        }
        if (currentWeaponIndex >= weapons.Length)
        {
            currentWeaponIndex = 0;
        }
        while (!weapons[currentWeaponIndex].isEquipped)
        {
            currentWeaponIndex += direction;
            if (currentWeaponIndex < 0)
            {
                currentWeaponIndex = weapons.Length - 1;
            }
            if (currentWeaponIndex >= weapons.Length)
            {
                currentWeaponIndex = 0;
            }
        }

        SelectWeapon(currentWeaponIndex);
        currentWeapon.UpdateAmmoDisplay();

        // Note (Manny): Send the index to every client
        if (photonView)
        {
            photonView.RPC("SelectWeaponRPC", PhotonTargets.All, currentWeaponIndex);
        }
    }

    public void DeathActions()
    {
        if (lastCheckpoint)
        {
            lastCheckpoint.GetComponent<CheckpointHandler>().ResetMyRoom();
        }
        else
        {
            //Start at the beginning again
        }
    }
    #endregion
}
