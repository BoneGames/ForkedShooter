using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GameSystems;
using Interactions;

public class RigidCharacterMovement : NetworkBehaviour
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

    [Header("Important Stuff")]
    public Rigidbody rigid;
    public float rayDistance = 1f;
    public GameObject myCamera;

    public Weapon[] weapons;

    public Weapon currentWeapon;

    GameObject shootPoint;
    public bool rotateToMainCamera = false;
    bool weaponRotationThing = false;

    private Vector3 moveDirection;

    private Interactable interactObject;
    public PlayerNetworkSetup playerNetworkSetup;

    //private bool isGrounded = true;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
    }

    void OnTriggerEnter(Collider other)
    {
        interactObject = other.GetComponent<Interactable>();
        print("Should be able to open");
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

    public void Attack()
    {
        currentWeapon.Attack();
    }

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

    private void OnDrawGizmos()
    {
        Ray groundRay = new Ray(transform.position, Vector3.down);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(groundRay.origin, groundRay.origin + groundRay.direction * rayDistance);
    }

    public void DisableAllWeapons()
    {
        foreach (var weapon in weapons)
        {
            weapon.gameObject.SetActive(false);
        }
    }

    public void SelectWeapon(int index)
    {
        if (!inBounds(index, weapons))
        {
            return;
        }

        CmdEquipWeaponLocal(index);
    }

    [Command]
    void CmdEquipWeaponLocal(int _index)
    {
        DisableAllWeapons();

        this.currentWeapon = weapons[_index];
        this.currentWeapon.gameObject.SetActive(true);

        RpcEquipWeaponGlobal(_index);
    }

    [ClientRpc]
    void RpcEquipWeaponGlobal(int _index)
    {
        DisableAllWeapons();

        this.currentWeapon = weapons[_index];
        this.currentWeapon.gameObject.SetActive(true);
    }


    private bool inBounds(int index, Weapon[] array)
    {
        return (index >= 0) && (index < array.Length);
    }

    [Command]
    public void CmdInteract()
    {
        if (interactObject)
        {
            Debug.Log("Cmd Door Opened by: " + this.gameObject.name );
            interactObject.Interact();
            RpcInteract();
        }
    }
    [ClientRpc]
    public void RpcInteract()
    {
        Debug.Log("Rpc Door Opened by: " + this.gameObject.name );
        interactObject.Interact();
    }
}
