using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public virtual void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }
}
