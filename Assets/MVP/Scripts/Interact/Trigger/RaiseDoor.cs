using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaiseDoor : MonoBehaviour
{
    //Code for closing the door
    [Header("Door Variables")]
    public Vector3 doorDown;
    public Vector3 doorUp;
    public GameObject door;
    public float doorSpeed;

    //Animations for closing door
    [Header("Door animation")]
    public Animator anim;

    bool enteredRoom;

    void Start()
    {
        door.transform.position = doorDown;
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if(enteredRoom)
        {
            door.transform.position += new Vector3(0, 2 * Time.deltaTime, 0);
        }
        else
        {
            door.transform.position -= new Vector3(0, 2 * Time.deltaTime, 0);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))   
        {
            
        }
    }

    
}
