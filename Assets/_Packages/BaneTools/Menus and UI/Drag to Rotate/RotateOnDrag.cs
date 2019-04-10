using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateOnDrag : MonoBehaviour
{
    public float torque = 1.0f;
    //private float baseAngle = 0.0f;
    public Rigidbody rb;
    public Collider col;

    public bool clickToStop;
    public bool rotateOnX;
    public bool rotateOnY;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            col.enabled = true;
        }
        else
        {
            col.enabled = false;
        }
    }

    private void OnMouseDown()
    {
        if (clickToStop)
        {
            rb.angularVelocity = new Vector3(0, 0, 0);
        }
    }

    void OnMouseDrag()
    {
        if (Input.GetKey(KeyCode.LeftControl))
        {
            if (rotateOnX)
            {
                rb.AddTorque(Vector3.up * torque * -Input.GetAxis("Mouse X"));
            }
            if (rotateOnY)
            {
                rb.AddTorque(Vector3.right * torque * Input.GetAxis("Mouse Y"));
            }
        }
    }
}