using System.Collections;
using System.Collections.Generic; // Used to get public List<Transform> visibleTargets.
using UnityEngine;

public class AI_FoV_SearchLight : AI_FoV_Detection
{
    #region Variables
    // Spotlight component (to control the 'Spot Angle' and 'Range' with the script's viewAngle and Radius).
    public Light fovLight;

    // (advanced)
    // Used to visualize the Field of View arc.
    //public MeshFilter viewMeshFilter;
    //Mesh viewMesh;
    #endregion Variables

    public override void Start()
    {
        // Get Light component from child SpotLight and assign values.
        fovLight = GetComponentInChildren<Light>();
        fovLight.spotAngle = viewAngle;
        fovLight.range = viewRadius * 1.5f;

        base.Start();
    }
}