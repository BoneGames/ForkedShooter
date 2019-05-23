using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public int damage = 50;
    public float speed = 5f;
    public Rigidbody rigid;
    public float detectionRadius;
    public LayerMask enemy;

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
                target.ChangeHealth(damage, transform.position);
            }
        }

        AlertCloseEnemies();


        Destroy(gameObject);
    }

    void AlertCloseEnemies()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, detectionRadius, enemy);
        if(cols.Length > 0)
        {
            foreach (Collider col in cols)
            {
                col.GetComponent<BehaviourAI>().BulletAlert(transform.position);
            }
        }
    }

    public enum SourceAgent
    {
        Player,
        Enemy1
    }
}
