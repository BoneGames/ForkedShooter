using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameSystems;
using System.Linq;

public class Pistol : Weapon
{
    public float spread;
    Vector3 lastPos;
    public float resolution;
    
    public List<Vector3> positions = new List<Vector3>();
    
    


    public override void Attack()
    {
        
        if (currentMag > 0)
        {
            RaycastHit hit;
            Ray ray = new Ray(spawnPoint.position, spawnPoint.transform.forward);

            SpawnMuzzleFlash();
            UpdateAmmoDisplay();

            if (Physics.Raycast(ray, out hit))
            {
                // For reference to see where bullets hit;
                GameObject bullet = Instantiate(GameObject.CreatePrimitive(PrimitiveType.Cube), hit.point, Quaternion.identity);
                bullet.GetComponent<Renderer>().material.color = Color.red;
                bullet.transform.localScale = new Vector3(.15f, .15f, .15f);

                GeneratePositions(spawnPoint.position, hit.point, hit.distance * resolution);
                StartCoroutine(BulletGradient(hit.distance * resolution));

                if (GameManager.isOnline)
                {
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        hit.transform.GetComponent<PhotonView>().RPC("ChangeHealth", PhotonTargets.All, damage);
                    }
                }
                else
                {
                    //print("I'm firing!");
                    //Debug.DrawRay(spawnPoint.position, spawnPoint.forward, Color.red);

                    if (hit.collider.tag == "Enemy")
                    {
                        hit.transform.GetComponent<Health>().ChangeHealth(damage, transform.position);
                        print("I hit an enemy");
                    }

                }
            }
            currentMag--;
        }
        if(currentMag <= 0 )
        {
            Reload();
        }
    }

    void GeneratePositions(Vector3 start, Vector3 end, float distance)
    {
        positions.Clear();
        lastPos = start;
        positions.Add(lastPos);
        Vector3 dir = (end - start)/distance;
        int intervals = (int)distance + 1;
        for(int position = 0; position < intervals; position++)
        {
            Vector3 nextPos = lastPos + dir;
            positions.Add(nextPos);
            lastPos = nextPos;
        }
        bulletPathRend.positionCount = intervals;
        bulletPathRend.SetPositions(positions.ToArray());
    }

    IEnumerator BulletGradient(float distance)
    {
        // Reset alpha settings on Gradient
        bulletGrad.alphaKeys = startingAlphaKeys;

        // New array of alphakeys to apply later
        GradientAlphaKey[] alphaKeys = bulletGrad.alphaKeys;

        float timer = 0;
        // alphaKey timer setting goes from 0 - 1
        while(timer < 1)
        {
            // speed equates to distance units/second
            timer += Time.deltaTime * bulletPathSpeed;

            // add timer value to alphaKey time position
            for (int AlphaKey = 0; AlphaKey < alphaKeys.Length; AlphaKey++)
            {
                alphaKeys[AlphaKey].time += Time.deltaTime * bulletPathSpeed;
            }

            // Re-apply array values to lineRenderer Gradient
            bulletGrad.alphaKeys = alphaKeys;
            bulletPathRend.colorGradient = bulletGrad;
            yield return null;
        }
        Debug.Log("finished1");

        // GENERAL LINE FADE OUT
        timer = 0;
        // while larget alpha value (bullet point) is bigger than 0
        while(timer < 1)
        {
            // subtract reduced timer
            timer += Time.deltaTime * 2;
            for (int AlphaKey = 0; AlphaKey < alphaKeys.Length; AlphaKey++)
            {
                alphaKeys[AlphaKey].alpha -= timer;
            }

            // Re-apply array values to lineRenderer Gradient
            bulletGrad.alphaKeys = alphaKeys;
            bulletPathRend.colorGradient = bulletGrad;
            yield return null;
        }
        Debug.Log("finished2");
    }
}
