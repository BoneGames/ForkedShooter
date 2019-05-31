using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public abstract class Projectile : MonoBehaviour
    {
        public float damage;
        public float speed = 5f;
        public float range;

        // Inherit bullet from weapon - pass detection radius down from relevant weapon
        public float detectionRadius;

        public Elements.Element bulletElement;
        public Vector3 scale;
        public Rigidbody rigid;
        public string firedBy;

        public GameObject impact;
        public Quaternion hitRotation;
        [HideInInspector]
        public Vector3 fireOrigin;

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
