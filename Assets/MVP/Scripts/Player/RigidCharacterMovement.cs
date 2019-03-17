using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using GameSystems;
using Interactions;

public class RigidCharacterMovement : NetworkBehaviour
{
    public float playerSpeed = 5f;
    public float jumpHeight = 10f;
    public Rigidbody rigid;
    public float rayDistance = 1f;

    public Weapon[] weapons;

    public Weapon currentWeapon;

    GameObject shootPoint;
    public bool rotateToMainCamera = false;
    public bool weaponRotationThing = false;

    private Vector3 moveDirection;
    private bool isJumping = false;

    private Interactable interactObject;
    public PlayerNetworkSetup playerNetworkSetup;

    //private bool isGrounded = true;

    void Start()
    {
        rigid = GetComponent<Rigidbody>();
        //currentWeapon = weapons[0].GetComponent<Weapon>();
        //currentWeapon.gameObject.SetActive(true);
        //shootPoint = weapons[0].transform.GetChild(0).GetComponent<Transform>();
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
        #region oldCode
        //if (Input.GetKey(KeyCode.W))
        //{
        //    rigid.AddForce(Vector3.forward * playerSpeed);
        //}
        //if (Input.GetKey(KeyCode.S))
        //{
        //    rigid.AddForce(Vector3.back);
        //}
        //if (Input.GetKey(KeyCode.A))
        //{
        //    rigid.AddForce(Vector3.left * playerSpeed);
        //}
        //if (Input.GetKey(KeyCode.D))
        //{
        //    rigid.AddForce(Vector3.right * playerSpeed);
        //}

        //if (Input.GetKey(KeyCode.Space) && isGrounded == true)
        //{
        //    rigid.AddForce(Vector3.up * jumpHeight, ForceMode.Impulse);
        //    isGrounded = false;
        //}
        #endregion

        Vector3 camEuler = Camera.main.transform.eulerAngles;

        if (rotateToMainCamera)
        {
            moveDirection = Quaternion.AngleAxis(camEuler.y, Vector3.up) * moveDirection;
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

        //if(moveDirection.magnitude > 0)
        //{
        //    transform.rotation = Quaternion.LookRotation(moveDirection);
        //}

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
