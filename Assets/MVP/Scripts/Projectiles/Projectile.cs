using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

using NaughtyAttributes;

public abstract class Projectile : MonoBehaviour
{
  [BoxGroup("Projectile Stats")]
  public float damage, speed = 5f, range;

  // Inherit bullet from weapon - pass detection radius down from relevant weapon
  [BoxGroup("Projectile Stats")]
  public float detectionRadius;

  [BoxGroup("Projectile References")]
  public Elements bulletElement;
  [BoxGroup("Projectile References")]
  public Vector3 scale;
  [BoxGroup("Projectile References")]
  public Rigidbody rigid;
  [BoxGroup("Projectile References")]
  public string firedBy;

  [BoxGroup("Projectile References")]
  [Label("Impact Prefab")]
  public GameObject impact;
  [BoxGroup("Projectile References")]
  public Quaternion hitRotation;

  [BoxGroup("Events")]
  public UnityEvent onCollisionEnter;

  [HideInInspector]
  public Vector3 fireOrigin;

  public virtual void Fire(Vector3 direction)
  {
    rigid.AddForce(direction * speed, ForceMode.Impulse);
  }

  public virtual void OnCollisionEnter(Collision collision)
  {
    onCollisionEnter.Invoke();
  }

  public virtual void OnHit()
  {

  }

  public virtual void OnKill()
  {

  }


}
