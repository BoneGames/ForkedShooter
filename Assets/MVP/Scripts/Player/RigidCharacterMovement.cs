using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystems;
using Interactions;

public class RigidCharacterMovement : MonoBehaviour
{
    [Header("Player Stats")]
    public float playerSpeed = 5f;
    public float jumpHeight = 10f;
    public float crouchMultiplier = .8f;
    public float sprintMultiplier = 1.5f;

    [Header("Player States")]
    public bool isCrouching = false;
    public bool isSprinting = false;
    private bool isJumping = false;
    public bool isDead = false;
    public Transform lastCheckpoint;

    [Header("Important Stuff")]
    public Rigidbody rigid;
    public float rayDistance = 1f;
    public GameObject myCamera;
    public Transform myHand;
    public Health myHealth;

    public Weapon[] weapons;

    public Weapon currentWeapon;

    GameObject shootPoint;
    public bool rotateToMainCamera = false;
    bool weaponRotationThing = false;
    private int currentWeaponIndex;

    private Vector3 moveDirection;
    

    private Interactable interactObject;

    [Header("Photon Networking")]
    public PhotonView pView;

        // PHOTON SYNC WEAPON IN_PROGRESS
    public bool[] WeaponsBools = new bool[3];

    //private bool isGrounded = true;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        myHealth = GetComponent<PlayerHealth>();
        pView = GetComponent<PhotonView>();
    }

    void OnTriggerEnter(Collider other)
    {
        interactObject = other.GetComponent<Interactable>();

        if (interactObject)
        {
            print("Should be able to open");
        }

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
        interactObject = null;
        print("Should not be able to open");

    }

    void Update()
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
    #region Photon Weapon Sync
        // PHOTON SYNC WEAPON IN_PROGRESS
    [PunRPC]
    public void SelectWeaponRPC()
    {
        //Debug.Log(this.name + ": WeaponRPC called");
        DisableAllWeapons();
        for(int weaponIndex = 0; weaponIndex < weapons.Length; weaponIndex++)
        {
            weapons[weaponIndex].gameObject.SetActive(WeaponsBools[weaponIndex]);
            if(weapons[weaponIndex].isActiveAndEnabled)
            {
                currentWeapon = weapons[weaponIndex];
            }
        }
    }
        // PHOTON SYNC WEAPON IN_PROGRESS
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //Debug.Log(this.name + ": OSV Called");
        if(stream.isReading)
        {
            WeaponsBools = (bool[])stream.ReceiveNext();
        }
        if(stream.isWriting)
        {
            stream.SendNext(WeaponsBools);
        }
    }
    #endregion

    #region Weapons
    public void Attack()
    {
        currentWeapon.Attack();
    }
    public void Reload()
    {
        currentWeapon.Reload();
    }
    public void Aim(bool isAiming)
    {
        myHand.localPosition = isAiming ? new Vector3(0, myHand.localPosition.y + .05f, myHand.localPosition.z) : myHand.localPosition = new Vector3(0.5f, myHand.localPosition.y - .05f, myHand.localPosition.z);

        //if (isAiming)
        //{
        //    myHand.localPosition = new Vector3(0, myHand.localPosition.y, myHand.localPosition.z);
        //}
        //else
        //{
        //    myHand.localPosition = new Vector3(0, myHand.localPosition.y, myHand.localPosition.z);
        //}
    }

    public void DisableAllWeapons()
    {
        foreach (var weapon in weapons)
        {
            weapon.gameObject.SetActive(false);
        }
    }

    public void SelectWeapon(int direction)
    {
        //if (!inBounds(index, weapons))
        //{
        //    return;
        //}

        currentWeaponIndex += direction;
        
        if (currentWeaponIndex < 0)
        {
            currentWeaponIndex = weapons.Length - 1;
        }
        if(currentWeaponIndex >= weapons.Length)
        {
            currentWeaponIndex = 0;
        }

        DisableAllWeapons();

        currentWeapon = weapons[currentWeaponIndex];
        currentWeapon.gameObject.SetActive(true);
    }

    private bool inBounds(int index, Weapon[] array)
    {
        return (index >= 0) && (index < array.Length);
    }
    #endregion

    #region Motion
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

    bool IsGrounded()
    {
        Ray groundRay = new Ray(transform.position, Vector3.down);
        RaycastHit hit;
        if (Physics.Raycast(groundRay, out hit, rayDistance))
        {
            return true;
        }
        return false;
    }
    #endregion

    #region Actions
    public void Interact()
    {
        if (interactObject)
        {
            interactObject.Interact();
        }
    }

    public void Respawn()
    {
        isDead = false;

        transform.position = lastCheckpoint.position;

        myHealth.currentHealth = myHealth.maxHealth;
    }
    #endregion
}
