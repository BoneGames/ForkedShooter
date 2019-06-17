using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(LineRenderer))]
public class Laser_rend : AI_Weapon
{
    LineRenderer lineRend;
    public float distance;

    public float width, error, laserDuration, chaseSpeed;
    public Transform target;

    public float timer, damageTimer, laserDamageRatePs;

    // Start is called before the first frame update
    [System.Obsolete]
    void Start()
    {
        lineRend = GetComponent<LineRenderer>();
        lineRend.enabled = false;
        lineRend.startWidth = width;
        lineRend.alignment = LineAlignment.View;
    }

    public override void Attack()
    {
        canShoot = false;
        attackTimer = 0;
        lineRend.SetPosition(0, transform.position);
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit))
        {
            if (hit.collider)
            {
                //StopAllCoroutines();
                StartCoroutine(TrackingLaser(hit.point));
            }
        }
        else
        {
            lineRend.SetPosition(1, transform.forward * 5000);
        }
    }



    public override void AiShoot(int _shots, Transform target)
    {
        this.Attack();
    }

    IEnumerator TrackingLaser(Vector3 hitPoint)
    {
        lineRend.enabled = true;
        timer = 0;
        damageTimer = 0;


        Vector3 newPosition = hitPoint + (Random.onUnitSphere * (1 / accuracy * 5));
        lineRend.SetPosition(1, newPosition);

        while (timer < laserDuration)
        {
            timer += Time.deltaTime;

            if (damageTimer > 0)
                damageTimer -= Time.deltaTime;

            Vector3 dir = (lineRend.GetPosition(1) - transform.position).normalized;

            if (Physics.Raycast(transform.position, dir, out RaycastHit hit1, 500))
            {
                Debug.Log("raycast hit: " + hit1.transform.name);
                if (hit1.transform.name == target.transform.name)
                {
                    Debug.Log("laser hit");
                    if (hit1.transform.GetComponent<Health>() && damageTimer <= 0)
                    {
                        //hit1.transform.GetComponent<Health>().ChangeHealth(damage, transform.position, weaponElement);
                        Debug.Log("laser dealt damage");
                        damageTimer = 1 / laserDamageRatePs;
                    }
                }
            }
            Vector3 laserTipToTargetDir = (target.position - lineRend.GetPosition(1)).normalized;
            newPosition = Vector3.Lerp(lineRend.GetPosition(1), target.position + (laserTipToTargetDir * chaseSpeed), Time.deltaTime);

            lineRend.SetPosition(1, newPosition);
            lineRend.SetPosition(0, transform.position);
            yield return null;
        }
        lineRend.enabled = false;
    }
}






//IEnumerator TrackingLaser(Vector3 hitPoint)
//{
//    Debug.Log("track");
//    lineRend.enabled = true;
//    timer = 0;

//    // Init first pos
//    Vector3 newPosition = hitPoint + (Random.insideUnitSphere * error);
//    NavMeshHit hit;
//    // get pos in mesh
//    if (NavMesh.SamplePosition(newPosition, out hit, 10, NavMesh.AllAreas))
//    {
//        newPosition = hit.position;
//    }

//    //newPosition.y = target.position.y;

//    lineRend.SetPosition(1, newPosition);

//    while (timer < duration)
//    {
//        timer += Time.deltaTime;
//        Vector3 dir = (lineRend.GetPosition(1) - transform.position).normalized * 100;
//        ray = dir;
//        RaycastHit hit1;
//        if (Physics.Raycast(transform.position, dir, out hit1))
//        {
//            Debug.Log("raycast hit: " + hit1.transform.name);
//            if (hit1.collider == target.GetComponent<Collider>())
//            {
//                // change health
//                Debug.Log("hit target");
//                continue;
//            }
//        }
//        // while new position is further away from target than old one

//        // Debug.Log("currentDist: " + Vector3.Distance(target.position, lineRend.GetPosition(1)) + ", newDist: " + Vector3.Distance(trackPos, target.position));

//        float currentDistance = Vector3.Distance(target.position, lineRend.GetPosition(1));
//        float newDistance = Vector3.Distance(newPosition, target.position);
//        while (currentDistance <= newDistance)
//        {
//            if (currentDistance < 4)
//            {
//                newPosition = Vector3.Lerp(lineRend.GetPosition(1), target.position, Time.deltaTime);
//                Debug.Log("lerping from close");
//                break;
//            }
//            Debug.Log("finding target, current dist: " + currentDistance);

//            newPosition = lineRend.GetPosition(1) + (Random.insideUnitSphere * error);

//            newPosition.y = 0;

//            currentDistance = Vector3.Distance(target.position, lineRend.GetPosition(1));
//            newDistance = Vector3.Distance(newPosition, target.position);
//            yield return null;
//        }
//        Debug.Log("set pos end");
//        //Vector3 lerpedPos = Vector3.Lerp(lineRend.GetPosition(1), trackPos, Time.deltaTime);
//        lineRend.SetPosition(1, newPosition);
//        yield return null;
//    }
//    lineRend.enabled = false;
//}