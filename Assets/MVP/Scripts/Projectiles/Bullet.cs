using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile
{
  public float detectionRadius;
  public LayerMask enemy;

  public GameObject sourceAgent;
  Vector3 firePoint;

  void Start()
  {
    firePoint = transform.position;
  }

  public override void Fire(Vector3 direction)
  {
    base.Fire(direction);
  }

  public override void OnCollisionEnter(Collision other)
  {
    if (other.gameObject != sourceAgent)
    {
      if (other.transform.GetComponent<Health>())
      {
        Health target = other.transform.GetComponent<Health>();
        target.ChangeHealth(damage, transform.position, Elements.Element.Normal);
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
        col.GetComponent<BehaviourAI>().BulletAlert(firePoint);
      }
    }
  }

  public enum SourceAgent
  {
    Player,
    Enemy1
  }
}
