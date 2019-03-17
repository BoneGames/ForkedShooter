using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Incendiary : Projectile
{
    //Element element = Element.Fire;
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public override void Fire()
    {
        base.Fire();
    }

    //public IEnumerator Burn(Enemy enemy)
    //{
    //    return;
    //}

    public override void OnCollisionEnter(Collision collision)
    {
        base.OnCollisionEnter(collision);
    }
}
