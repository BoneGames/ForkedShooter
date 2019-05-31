using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile
{
    public LayerMask enemy;

    public GameObject sourceAgent;

    void Start()
    {
        fireOrigin = transform.position;
    }

    public override void OnCollisionEnter(Collision other)
    {
        if (other.gameObject != sourceAgent)
        {
            Health target = other.transform.GetComponent<Health>();
            if (target)
            {
                print("AAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAAA");
                target.ChangeHealth(damage, transform.position, bulletElement);
            }
        }

        AlertCloseEnemies();

        Destroy(gameObject);
    }

    void AlertCloseEnemies()
    {
        Collider[] cols = Physics.OverlapSphere(transform.position, detectionRadius, enemy);
        if (cols.Length > 0)
        {
            foreach (Collider col in cols)
            {
                col.GetComponent<BehaviourAI>().BulletAlert(fireOrigin);
            }
        }
    }

    public enum SourceAgent
    {
        Player,
        Enemy1
    }
}
