using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Laser_Mesh : AI_Weapon
{
    public GameObject laserMeshPrefab;
    public List<GameObject> laserMeshes = new List<GameObject>();
    Renderer rend;
    public float laserChunkLength;

    public int bits;

    float distance;
    void Start()
    {
        //rend = GetComponent<Renderer>();
        // maybe use bounds.size if this doesnt work
        laserChunkLength = laserMeshPrefab.GetComponent<Renderer>().bounds.size.y * 2;
    }

    //public override void AiShoot(int _shots, Transform target)
    //{
    //    StopAllCoroutines();
    //    StartCoroutine(TrackingLaser());
    //}
    //IEnumerator HoldLaser()
    //{
    //    while(holdShoot)
    //    {
    //        yield return null;

    //    }
    //    List<Transform> beams = this.GetComponentsInChildren<Transform>().ToList();
    //    beams.RemoveAt(0);
     
    //    for(int i = 0; i < beams.Count; i++)
    //    {
            
    //        Destroy(beams[i].gameObject);
    //    }
    //    bits = 0;
    //}

    public override void Attack()
    {
        //Debug.Log("shootLasewr");
        RaycastHit hit;
        Vector3 direction = transform.forward;

        if (Physics.Raycast(transform.position, direction, out hit))
        {
            distance = Vector3.Distance(transform.position, hit.point);
            Quaternion angle = Quaternion.LookRotation(direction);
            if (bits < 1)
            {
                //StopAllCoroutines();
                //StartCoroutine(HoldLaser());
                for (int i = 0; i < (int)distance / laserChunkLength; i++)
                {
                    NewLaserChunk(transform.position + (direction * (laserChunkLength * i)), angle * Quaternion.Euler(90, 0, 0));
                }
            }
            else
            {
                //StopAllCoroutines();
                //StartCoroutine(HoldLaser());
                if (distance > ((bits * laserChunkLength) - laserChunkLength))
                { 
                    AddLaserChunk(direction * laserChunkLength, angle * Quaternion.Euler(90, 0, 0));
                    Debug.Log("distance: " + distance + ", bits:" + bits);
                    
                }
                else
                {
                    Debug.Log("Kill distance: " + distance + ", count * length:" + bits);
                    bits--;
                    List<Transform> beams = this.GetComponentsInChildren<Transform>().ToList();
                    beams.RemoveAt(0); 
                    Destroy(beams[beams.Count - 1].gameObject);
                }
            }
        }
    }

    void AddLaserChunk(Vector3 direction, Quaternion rotation)
    {
        bits++;
        GameObject laserBit = Instantiate(laserMeshPrefab, laserMeshes[laserMeshes.Count-1].transform.position + direction, rotation);
       // laserMeshes.Add(laserBit);
        laserBit.transform.SetParent(this.transform);
        laserBit.transform.name += "add";
        
    }

    void NewLaserChunk(Vector3 position, Quaternion rotation)
    {
        bits++;
        GameObject laserBit = Instantiate(laserMeshPrefab, position, rotation);
        //laserMeshes.Add(laserBit);
        laserBit.transform.SetParent(this.transform);
        laserBit.transform.name += "new";
        
    }

}
