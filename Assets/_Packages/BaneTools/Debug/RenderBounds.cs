using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RenderBounds : MonoBehaviour
{
    private Renderer rend;

    void OnDrawGizmos()
    {
        rend = GetComponent<Renderer>();

        Gizmos.DrawCube(rend.bounds.center, rend.bounds.size);
    }
}
