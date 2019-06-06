using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

public class Pickup : MonoBehaviour
{
    public bool ShowVisuals;
    [BoxGroup("Visuals"), ShowIf("ShowVisuals")] public float rotateSpeed = 45f;
    [BoxGroup("Visuals"), ShowIf("ShowVisuals")] public GameObject beam;
    [BoxGroup("Visuals"), ShowIf("ShowVisuals")] public Color32 beamColour;

    [HideInInspector] public AudioSource aS;

    public UnityEvent onPickup;

    public virtual void Awake()
    {
        beam.GetComponent<SpriteRenderer>().color = beamColour;
        aS = GetComponent<AudioSource>();
    }

    public virtual void OnTriggerEnter(Collider other)
    {
        Destroy(gameObject);
    }

    public virtual void Rotate()
    {
        this.gameObject.transform.Rotate(0, rotateSpeed * Time.deltaTime, 0);
    }

    public virtual void PlayClip()
    {
        aS.Play();
    }
}
