using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotateConstantly : MonoBehaviour
{
    public Vector3 rotation;
    public float rotateSpeed;

    // Update is called once per frame
    void Update()
    {
        this.transform.Rotate(rotation * Time.deltaTime);
    }
}
