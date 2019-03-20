using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    public float speed;
    public float range;
    public Element element;
    public Vector3 scale;
    public Rigidbody rigid;
    public PlayerNetworkSetup pns;
    public int damage;

    public GameObject impact;
    public Quaternion hitRotation;

    void Start()
    {
        rigid.AddForce(transform.forward * speed, ForceMode.Impulse);
    }

    public virtual void Fire()
    {
        rigid.AddForce(transform.forward * speed, ForceMode.Impulse);
    }

    public virtual void OnCollisionEnter(Collision collision)
    {
        if(collision.transform.tag == "Player")
        {
            string playerID = collision.transform.name;
            collision.transform.GetComponent<PlayerNetworkSetup>().CmdPlayerShot(playerID, damage);
        }
    }

    public virtual void OnHit()
    {
        
    }

    public virtual void OnKill()
    {

    }

    public enum Element
    {
        Normal,
        Fire,
        Explosive
    }
}
