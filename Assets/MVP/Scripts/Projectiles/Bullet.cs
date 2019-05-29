using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : Projectile
{
    public int damage = 50;
    public float speed = 5f;
    public Rigidbody rigid;
    // Inherit bullet from weapon - pass detection radius down from relevant weapon
    public float detectionRadius;
    public LayerMask enemy;

    public GameObject sourceAgent;
    Vector3 fireOrigin;

  void Start()
  {
    firePoint = transform.position;
  }

    void Start()
    {
        fireOrigin = transform.position;
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

        AlertCloseEnemies(other.contacts[0].point);

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
