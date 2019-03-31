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
        string tag = collision.collider.tag;
        if(tag.Contains("Player") && collision.transform.name != firedBy)
        {
            collision.transform.GetComponent<PhotonView>().RPC("ChangeHealth", PhotonTargets.All, damage);
            Explode();
            Effects();
        }
    }

    void Explode()
    {
		//Collider[] hits = Physics.OverlapSphere(transform.position, explosionRadius);
		//foreach(var hit in hits)
		//{
		//	Enemy e = hit.GetComponent<Enemy>();
		//	if(e)
		//	{
		//		//e.TakeDamage(damage);
		//	}
		//}
        GameObject.Destroy(this.gameObject);
    }
	
	void Effects()
	{
        GameObject explosion = Instantiate(explosionEffect);
        explosion.transform.position = transform.position;
        explosion.transform.localRotation = hitRotation;
	}
}


