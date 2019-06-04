using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : Projectile
{
  public float explosionRadius;
  public float explosionDelay;

  public GameObject explosionEffect;
  public override void Fire(Vector3 direction)
  {
    base.Fire(direction);
    //rigid.useGravity = true;
  }

  public override void OnCollisionEnter(Collision collision)
  {
    Debug.Log("ROCKET HIT: " + collision.transform.name);
    string tag = collision.collider.tag;
    //if(tag != "Player" && collision.transform.name != firedBy)
    //{
    //    collision.transform.GetComponent<PhotonView>().RPC("ChangeHealth", PhotonTargets.All, damage);
    //}
    if (tag == "Enemy")
    {
      collision.transform.GetComponent<Health>().ChangeHealth(damage, transform.position, bulletElement);
      Debug.Log(collision.transform.name + " just got hit by rocket. Now has: " + collision.transform.GetComponent<Health>().currentHealth + " health");
    }
    Explode();
    base.OnCollisionEnter(collision);
    //Effects();
  }

  void Explode()
  {
    // explosion damage does 1/2 the damage the impact does
    // NOTE: this will mean damage gets done twice for hit player... Maybe that's fine?
    int explosionDamage = (int)(damage * 0.5f);
    Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
    foreach (var hit in hits)
    {
      Health h = hit.GetComponent<Health>();
      if (h)
      {
        h.ChangeHealth(explosionDamage, transform.position, bulletElement);
        Debug.Log(h.transform.name + " just got hit by rocket explosion and took +" + explosionDamage + " damage. It now has: " + h.transform.GetComponent<Health>().currentHealth + " health");
      }
    }
    GameObject.Destroy(this.gameObject);
  }

  void Effects()
  {
    GameObject explosion = Instantiate(explosionEffect);
    explosion.transform.position = transform.position;
    explosion.transform.localRotation = hitRotation;
  }
}


