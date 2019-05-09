using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystems;
using Interactions;

// Note (Manny): To eliminate getting components, use PunBehaviour (I did it in PlayerInput as well)
public class RigidCharacterMovement : Photon.PunBehaviour
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
    public static bool isAiming = false;
    public Transform lastCheckpoint;

    [Header("Important Stuff")]
    public Rigidbody rigid;
    public float rayDistance = 1f;
    public Camera myCamera;
    public Transform myHand;
    public Health myHealth;
    public Weapon[] weapons;
    public Weapon currentWeapon;
    public bool rotateToMainCamera = false;
    public int currentWeaponIndex;

    private GameObject shootPoint;
    private bool weaponRotationThing = false;
    private Vector3 moveDirection;
    private Interactable interactObject;
    private float timeTillRespawn = 5;

    #region Unity Events
    void Awake()
    {
        // Note (Manny): Get into the habbit of getting components in Awake instead!
        rigid = GetComponent<Rigidbody>();
        myHealth = GetComponent<PlayerHealth>();
    }
    void Start()
    {
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
        interactObject = null;
        print("Should not be able to open");

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

        myHealth.healthBar.UpdateBar();
    }
    public void FreeAmmo()
    {
        currentWeapon.currentReserves = 300;
        currentWeapon.UpdateAmmoDisplay();
    }

    void OnGUI()
    {
        if (isDead)
        {
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            style.fontSize = 35;
            GUI.Box(new Rect(0, 0, Screen.width, Screen.height), "You died and will respawn in \n" + timeTillRespawn + " seconds", style);
        }
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

    public void Aim(bool _isAiming)
    {
        isAiming = _isAiming;
        myHand.localPosition = _isAiming ? new Vector3(0, myHand.localPosition.y + .05f, myHand.localPosition.z) : myHand.localPosition = new Vector3(0.5f, myHand.localPosition.y - .05f, myHand.localPosition.z);

        myCamera.fieldOfView = _isAiming ? currentWeapon.scopeZoom : 75;
    }
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

        SelectWeapon(currentWeaponIndex);
        currentWeapon.UpdateAmmoDisplay();

        // Note (Manny): Send the index to every client
        if (photonView)
        {
            photonView.RPC("SelectWeaponRPC", PhotonTargets.All, currentWeaponIndex);
        }
    }
    #endregion
}
