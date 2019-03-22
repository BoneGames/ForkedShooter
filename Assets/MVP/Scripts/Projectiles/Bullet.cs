using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 50;
    public float speed = 5f;
    public Rigidbody rigid;

    public GameObject sourceAgent;

    public void Fire(Vector3 direction)
    {
        rigid.AddForce(direction * speed, ForceMode.Impulse);
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject != sourceAgent)
        {
            if (other.transform.GetComponent<Health>())
            {
                Health target = other.transform.GetComponent<Health>();
                target.ChangeHealth(damage);
            }
            
            Destroy(gameObject);
        }
    }

    public enum SourceAgent
    {
        Player,
        Enemy1
    }
}
