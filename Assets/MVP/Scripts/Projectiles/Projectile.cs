using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
  public float damage;
  public float speed = 5f;
  public float range;
  public Elements bulletElement;
  public Vector3 scale;
  public Rigidbody rigid;
  public string firedBy;

  public GameObject impact;
  public Quaternion hitRotation;

  // Use this for initialization
  void Start()
  {

  }

  // Update is called once per frame
  void Update()
  {

  }

  public virtual void Fire(Vector3 direction)
  {
    rigid.AddForce(direction * speed, ForceMode.Impulse);
  }

  public virtual void OnCollisionEnter(Collision collision)
  {

  }

  public virtual void OnHit()
  {

  }

  public virtual void OnKill()
  {

  }


}
