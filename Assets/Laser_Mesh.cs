using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser_Mesh : AI_Weapon
{
    public GameObject laserMeshPrefab;
    public List<GameObject> laserMeshes = new List<GameObject>();
    Renderer rend;
    public float laserChunkLength;

    float distance;
    void Start()
    {
        rend = GetComponent<Renderer>();
        // maybe use bounds.size if this doesnt work
        laserChunkLength = rend.bounds.extents.y;
    }

    public override void AiShoot(int _shots, Transform target)
    {
        StopAllCoroutines();
        StartCoroutine(TrackingLaser());
    }

    public override void Attack()
    {
        RaycastHit hit;

        Vector3 direction = transform.forward;

        direction.x += Random.Range(-accuracy, accuracy);
        direction.y += Random.Range(-accuracy, accuracy);

        if (Physics.Raycast(transform.position, direction, out hit))
        {
            distance = Vector3.Distance(transform.position, hit.point);
            Vector3 impact = hit.point;

            Quaternion angle = Quaternion.LookRotation(direction);

            for (int i = 0; i <= distance/laserChunkLength; i++)
            {
                NewLaserChunk(transform.position + (direction * i), angle);
            }
        }
    }

    void NewLaserChunk(Vector3 position, Quaternion rotation)
    {
        GameObject laserBit = Instantiate(laserMeshPrefab, position, rotation);
    }

    IEnumerator TrackingLaser()
    {
        



        return null;
    }
}
