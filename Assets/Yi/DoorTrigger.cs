using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [Header("Animation")]
    public Animator anim;

    [Header("Variables")]
    public GameObject enemy;

    void Start()
    {
        
    }
    void Update()
    {
        if(enemy.transform.childCount <= 0)
        {
            anim.SetBool("Exit", true);
        }
    }


    //When player enters triggerbox
    void OnTriggerEnter(Collider other)
    {
        //Only trigger is tag is player
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("Collision detected");
            //Set the trigger for the animator
            anim.SetTrigger("Enter");
        }
    }
}
