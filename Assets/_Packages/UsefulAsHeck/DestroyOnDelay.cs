using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestroyOnDelay : MonoBehaviour
{
    public float destroyDelay = 3f;
    // Use this for initialization
    void Start()
    {
        GameObject.Destroy(this.gameObject, destroyDelay);
    }
}
