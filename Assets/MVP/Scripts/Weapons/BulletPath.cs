using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletPath : MonoBehaviour
{
   
    List<Vector3> positions = new List<Vector3>();
    Vector3 lastPos;
    Gradient bulletGrad = new Gradient();
    LineRenderer bulletPathRend;
    GradientAlphaKey[] startingAlphaKeys = new GradientAlphaKey[5];
    GradientColorKey[] startingColorKeys = new GradientColorKey[2];
    [HideInInspector]
    public Vector3 target;
    [HideInInspector]
    public float distance;
    
    
    public float resolution;
    [Tooltip("meters/second")]
    public float bulletPathSpeed;
    public float trailDuration;
    

    void Start()
    {
        bulletPathRend = GetComponent<LineRenderer>();
        bulletPathRend.startWidth = 0.02f;
        CreateGradient();
        GeneratePositions();
    }

    void CreateGradient()
    {
        // gradient has to be hardcoded due to instantiation at runtime;
        bulletGrad = new Gradient();

        startingAlphaKeys[0].alpha = 0.1f;
        startingAlphaKeys[0].time = 0f;

        startingAlphaKeys[1].alpha = 0.1f;
        startingAlphaKeys[1].time = 0.01f;

        startingAlphaKeys[2].alpha = 1f;
        startingAlphaKeys[2].time = 0.03f;

        startingAlphaKeys[3].alpha = 0f;
        startingAlphaKeys[3].time = 0.05f;

        startingAlphaKeys[4].alpha = 0f;
        startingAlphaKeys[4].time = 1f;

        startingColorKeys[0].color = startingColorKeys[1].color = Color.white;
        startingColorKeys[0].time = 0f;
        startingColorKeys[1].time = 1f;

        bulletGrad.SetKeys(startingColorKeys, startingAlphaKeys);
    }
    

	public void GeneratePositions()
    {
        // defines how many vertices the line has
        // distance is provided by weapon raycast
        distance *= resolution;
        // set first position
        lastPos = transform.position;
        // add first pos
        positions.Add(lastPos);
        // get direction and divide by distance for magnitude
        Vector3 dir = (target - transform.position)/distance;
        // distance plus 1 for a clean interval count
        int intervals = (int)distance + 1;

        // add direction to lastPos, add to list, then update values
        for(int position = 0; position < intervals; position++)
        {
            Vector3 nextPos = lastPos + dir;
            positions.Add(nextPos);
            lastPos = nextPos;
        }

        // set line vertice positions array to correct length
        bulletPathRend.positionCount = intervals;
        // assign positions
        bulletPathRend.SetPositions(positions.ToArray());
        // start moving line
        StartCoroutine(BulletGradient(distance));
    }

    IEnumerator BulletGradient(float distance)
    {
        // Reset alpha settings on Gradient
        //bulletGrad.alphaKeys = startingAlphaKeys;

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

        // GENERAL LINE FADE OUT
        timer = 0;
        // while larget alpha value (bullet point) is bigger than 0
        while(timer < 1)
        {
            // subtract reduced timer
            timer += Time.deltaTime;
            for (int AlphaKey = 0; AlphaKey < alphaKeys.Length; AlphaKey++)
            {
                alphaKeys[AlphaKey].alpha -= Time.deltaTime/trailDuration;
            }

            // Re-apply array values to lineRenderer Gradient
            bulletGrad.alphaKeys = alphaKeys;
            bulletPathRend.colorGradient = bulletGrad;
            yield return null;
        }
        Destroy(gameObject);
    }
}
