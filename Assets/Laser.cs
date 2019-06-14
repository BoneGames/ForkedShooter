using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(LineRenderer))]
public class Laser : AI_Weapon
{
    LineRenderer lineRend;
    List<Vector3> positions = new List<Vector3>();
    public float distance, laserDuration;
    

    public float width;

    bool entry = true;

    public Transform target;
    // Start is called before the first frame update
    void Start()
    {

        lineRend = GetComponent<LineRenderer>();
        lineRend.enabled = false;
        lineRend.startWidth = width;

        positions.Clear();
        positions.TrimExcess();

      
        positions.Add(target.position);

        lineRend.SetPosition(0, transform.position);
    }


    public override void AiShoot(int _shots)
    {
        StopAllCoroutines();
        StartCoroutine(TrackingLaser());
    }


    IEnumerator TrackingLaser()
    {
        lineRend.enabled = true;


        Vector3 trackLaser = target.position;
        lineRend.SetPosition(1, trackLaser);

        yield return new WaitForSeconds(laserDuration);
        lineRend.enabled = true;
    }
}

