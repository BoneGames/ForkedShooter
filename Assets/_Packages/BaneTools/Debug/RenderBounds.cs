using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderBounds : MonoBehaviour
{
    private Renderer rend;

    public Color boundColor;

    void OnDrawGizmos()
    {
        rend = GetComponent<Renderer>();

        Gizmos.color = boundColor;
        Gizmos.DrawCube(rend.bounds.center, rend.bounds.size);
    }
}
