using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

public class PushOOB : MonoBehaviour
{
    public float pushForce = 5000;
    public void OnCollisionEnter(Collision other)
    {
        // If the object we hit is the enemy
        if (other.gameObject.tag == "OOB")
        {
            print(BaneTools.ColorString("I am hitting the wall!", "red"));

            // calculate force vector
            var force = transform.position - other.transform.position;
            // normalize force vector to get direction only and trim magnitude
            force.Normalize();
            gameObject.GetComponent<Rigidbody>().AddForce(force * pushForce, ForceMode.Acceleration);

            //// Calculate Angle Between the collision point and the player
            //Vector3 dir = other.contacts[0].point - transform.position;
            //// We then get the opposite (-Vector3) and normalize it
            //dir = -dir.normalized;
            //// And finally we add force in the direction of dir and multiply it by force. 
            //// This will push back the player
            //GetComponent<Rigidbody>().AddForce(dir * pushForce);
        }
    }
}
