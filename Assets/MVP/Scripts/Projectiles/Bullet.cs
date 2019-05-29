using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
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
        fireOrigin = transform.position;
    }

    public void Fire(Vector3 direction)
    {
        rigid.AddForce(direction * speed, ForceMode.Impulse);
    }

    public void OnCollisionEnter(Collision other)
    {
        if (other.gameObject != sourceAgent)
        {
            if (other.transform.GetComponent<Health>())
            {
                Health target = other.transform.GetComponent<Health>();
                target.ChangeHealth(damage, transform.position);
            }
        }

        AlertCloseEnemies(other.contacts[0].point);


        Destroy(gameObject);
    }

    // NOTE: implement sound from gun alert, and impact of bullet alert, separately?
    void AlertCloseEnemies(Vector3 _hitPoint)
    {
        Debug.Log("hey");
        float distance = Vector3.Distance(fireOrigin, _hitPoint);
        Vector3 dir = (fireOrigin - _hitPoint).normalized;
        Vector3 _newInspectionPoint = dir * (distance * 0.7f);

        GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.position = _newInspectionPoint;


        Collider[] cols = Physics.OverlapSphere(transform.position, detectionRadius, enemy);
        if(cols.Length > 0)
        {
            foreach (Collider col in cols)
            {
                col.GetComponent<BehaviourAI>().BulletAlert(_newInspectionPoint);
            }
        }
    }

    public enum SourceAgent
    {
        Player,
        Enemy1
    }
}
