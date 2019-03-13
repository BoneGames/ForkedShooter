using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Normal : Projectile
{
    public override void OnCollisionEnter(Collision col)
    {
        if (col.gameObject.tag == this.gameObject.tag)
        {
            Physics.IgnoreCollision(this.GetComponent<Collider>(), col.gameObject.GetComponent<Collider>());
        }
    }
    public override void Fire(Vector3 direction)
    {
        base.Fire(direction);
    }
}
