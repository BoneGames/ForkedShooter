using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InvulTotem : MonoBehaviour
{
    #region Variables
    public float radius = 10f;
    public List<Collider> invulnerable = new List<Collider>();
    public SphereCollider col;
    #endregion

    // Start is called just before any of the Update methods is called the first time
    void Start()
    {
        col = gameObject.GetComponent<SphereCollider>();
        col.isTrigger = true;
        col.radius = radius;
    }

    void MakeInvulnerable()
    {
        if (invulnerable.Count > 0)
        {
            print("isGod");
        }
    }

    // OnTriggerStay is called once per frame for every Collider other that is touching the trigger
    void OnTriggerStay(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            invulnerable.Add(other);
            MakeInvulnerable();
        }
    }
    
    // OnTriggerExit is called when the Collider other has stopped touching the trigger
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Enemy")
        {
            invulnerable.Remove(other);
            print("!isGod");
        }
    }
}
