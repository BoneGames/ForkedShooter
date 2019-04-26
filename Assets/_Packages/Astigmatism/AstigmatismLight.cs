using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

[RequireComponent(typeof(Renderer))]
public class AstigmatismLight : MonoBehaviour
{
    [Header("References")]
    public GameObject rayPrefab;
    public GameObject rayOrigin;
    public GameObject myChild;

    public GameObject target;

    [Header("Stats")]
    public int lensDivision = 8;
    public float distortionSeverity = 1;
    public Vector2 lensVariation = new Vector2(0, 0);

    [Tooltip("X/Y/Z/W = Left/Right/Top/Bottom")]
    public Vector4 objectBounds = new Vector4(-1, 1, 1, -1);

    void Start()
    {
        //Spawns a lens ray at a rotation given by the amount of divisions provided
        for (int i = 1; i <= lensDivision; i++)
        {
            GameObject ray = Instantiate(rayPrefab, rayOrigin.transform);
            ray.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, (BaneMath.DivideInts(i, lensDivision) * 360) + Random.Range(lensVariation.x, lensVariation.y)));
        }

        BaneTools.SetWorldScale(rayOrigin.transform, rayOrigin.transform.parent, distortionSeverity);
    }

    void Update()
    {
        //Is the render visible to the camera? If so, set it to active in the scene, else deactivate it
        rayOrigin.SetActive(GetComponent<Renderer>().IsVisibleFrom(Camera.main) ? true : false);

        //Rotates the lens rays to the camera to mimic the light bouncing inside the abnormal lens
        rayOrigin.transform.localRotation = Camera.main.transform.localRotation;

        myChild.SetActive(BaneRays.VewNotObstructed(objectBounds, transform, target.transform, true) ? true : false);
    }
}
