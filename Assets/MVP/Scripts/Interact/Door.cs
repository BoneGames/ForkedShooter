using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Interactions;

public class Door : Interactable
{
    public Animator anim;

    // Use this for initialization
    void Start()
    {
        anim = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Interact()
    {
        bool isOpen = anim.GetBool("isOpen");
        anim.SetBool("isOpen", !isOpen);
    }
}
