using UnityEngine;
using System.Collections;
//this script can be found in the Component section under the option Character Set Up 
//Mouse Look

[AddComponentMenu("FirstPerson/Camera - Mouse Look")]
public class SingleMouseLook : MonoBehaviour
{
    #region Variables
    [Header("Sensitivity")]
    public float xSensitivity = 10f;
    public float ySensitivity = 10f;
    [Header("Y Rotation Clamp")]
    public float minY = -60f;
    public float maxY = 60f;
    //we will have to invert our mouse position later to calculate our mouse look correctly

    public bool cursorVisible;

    private float rotationY;
    private GameObject player;
    private GameObject fpsCamera;

    #endregion
    #region Start
    void Start()
    {
        if (this.GetComponent<Rigidbody>())
        {
            this.GetComponent<Rigidbody>().freezeRotation = true;
        }
        player = GameObject.FindGameObjectWithTag("Player");
        fpsCamera = GameObject.FindGameObjectWithTag("MainCamera");
    }

    #endregion
    #region Update  
    void Update()
    {
        player.transform.Rotate(0, Input.GetAxis("Mouse X") * xSensitivity * Time.deltaTime, 0);

        rotationY += Input.GetAxis("Mouse Y") * ySensitivity * Time.deltaTime;
        rotationY = Mathf.Clamp(rotationY, minY, maxY);

        fpsCamera.transform.localEulerAngles = new Vector3(-rotationY, 0, 0);

        if (true)
        {
            ToggleCursor(cursorVisible);
        }
    }

    public void ToggleCursor(bool toggle)
    {
        if (toggle == true)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
    }
}
#endregion
