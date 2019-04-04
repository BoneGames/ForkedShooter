using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pickup : MonoBehaviour
{
    public float rotateSpeed = 45f;
    public GameObject beam;
    public Color32 beamColour;

    public virtual void Start()
    {
        beam.GetComponent<SpriteRenderer>().color = beamColour;
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }

    public virtual void Rotate()
    {
        this.gameObject.transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }
}
