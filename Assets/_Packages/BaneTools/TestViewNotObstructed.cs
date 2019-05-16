using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using BT;

public class DrawRaysFromEdges : MonoBehaviour
{
    private Renderer rend;
    public Transform target;

    public GameObject myChild;

    public bool debugMode;

    [Tooltip("X/Y/Z/W = Left/Right/Top/Bottom")]
    public Vector4 objectBounds = new Vector4(-1, 1, 1, -1);

    void Update()
    {
        myChild.SetActive(BaneRays.VewNotObstructed(objectBounds, transform, target, true) ? true : false);
    }
}
