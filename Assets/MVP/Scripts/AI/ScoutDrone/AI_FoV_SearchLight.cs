using System.Collections;
using System.Collections.Generic; // Used to get public List<Transform> visibleTargets.
using UnityEngine;

public class AI_FoV_SearchLight : AI_FoV_Detection
{
    #region Variables
    // How far (viewRadius) can the AI see, and how much (viewAngle) can they see (clamped to 0°-360°).
    [Header("View Attributes")]
    // Spotlight component (to control the 'Spot Angle' and 'Range' with the script's viewAngle and Radius).
    public Light fovLight;

    // (advanced)
    // Used to visualize the Field of View arc.
    //public MeshFilter viewMeshFilter;
    //Mesh viewMesh;
    #endregion Variables

    void Start()
    {
        // Get Light component from child SpotLight and assign values.
        fovLight = GetComponentInChildren<Light>();
        fovLight.spotAngle = viewAngle;
        fovLight.range = viewRadius * 1.5f;

        // Where the MeshFilter is initialized.
        //viewMesh = new Mesh();
        //viewMesh.name = "View Mesh";
        //viewMeshFilter.mesh = viewMesh;

        // Start running the Coroutine that calls upon 'FindTargetsWithDelay' Method, with a call rate of 0.2f (five times a second).
        StartCoroutine("FindTargetsWithDelay", .2f);
    }

    // Method to call upon FindVisibleTargets Method with a delay (0.2f from Coroutine argument).
    IEnumerator FindTargetsWithDelay(float delay)
    {
        // while running...
        while (true)
        {
            // Stop/Wait (delay) seconds, then run 'FindVisibleTargets' Method, and update information drawn from it.
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }

    void LateUpdate()
    {
        // Method called at LateUpdate (makes it look much less jittery).
        DrawFieldOfView();
    }

    #region void METHOD - Find Visible Targets (Player)
    // Method to find target/player in a list (called upon in 'IEnumberator FindTargetsWithDelay' Method).
    void FindVisibleTargets()
    {
        // Each time it's run, clear targets from visibleTargets list to avoid duplicates.
        visibleTargets.Clear();

        // Returns an array of colliders overlapping (touching or inside) a sphere. Basically, it works like a spherical raycast using:
        // transform.position = Vector3 position (where is the center of the sphere (where is it cast from)?).
        // viewRadius = float radius (how big is the sphere?).
        // targetMask = int layerMask (out of all the layers in the scene, which layer will colliders selectively RETURN on when casting a ray?).
        Collider[] targetInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

        // 'for' loop to get array of Colliders found inside of Physics.OverlapSphere into an index.
        for (int i = 0; i < targetInViewRadius.Length; i++)
        {
            // Transform component = array containing the transform component of every 'targetInViewRadius'.
            Transform target = targetInViewRadius[i].transform;
            // Find the target's Vector3 position normalized (magnitude of 1) (used in if statement below).
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            // 'if' the angle between the current (forward) view and the target's position is less than half of viewAngle (target in view angle)...
            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                // return the Distance (dstToTarget) to the target from point A (start), and point B (target's position).
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                // Long story short: 'if' the Physics.Raycast to find the player/target does NOT hit an obstruction (LayerMask obstacleMask)...
                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    // There are no obstructions in the way, so add the target/player to the array's index!
                    visibleTargets.Add(target);
                }
            }
        }
    }
    #endregion

    #region METHOD (advanced) - Draw Field of View
    // Method uses all of the information made from multiple structs and functions to control how the Field of View arc is visualized.
    void DrawFieldOfView()
    {
        // stepCount = number of rays cast from meshResolution per ° (degree).
        // stepAngleSize = How many ° are in each stepCount.
        // MATH → 
        int stepCount = Mathf.RoundToInt(viewAngle * meshResolution);
        float stepAngleSize = viewAngle / stepCount;

        // List of all points view cast hits (to construct mesh/array of triangles from hit points).
        List<Vector3> viewPoints = new List<Vector3>();

        // A copy of 'ViewCastInfo' struct values.
        ViewCastInfo oldViewCast = new ViewCastInfo();

        // loop through each of the steps.
        for (int i = 0; i <= stepCount; i++)
        {
            // Current angle = object's current rotation - half of viewAngle (rotates back to left-most view angle) + stepAngleSize × i (step count).
            float angle = transform.eulerAngles.y - viewAngle / 2 + stepAngleSize * i;

            // Using all of the variables stored in the ViewCastInfo struct as they appear after being used in the ViewCast Method.
            ViewCastInfo newViewCast = ViewCast(angle);

            // Uses 'FindEdge' Method to optimize smooth appearance of Field of View arc when one Raycast of a triangle hits/misses an obstacle.
            if (i > 0)
            {
                bool edgeDstThresholdExceeded = Mathf.Abs(oldViewCast.dst - newViewCast.dst) > edgeDstThreshold;
                if (oldViewCast.hit != newViewCast.hit || (oldViewCast.hit && newViewCast.hit && edgeDstThresholdExceeded))
                {
                    EdgeInfo edge = FindEdge(oldViewCast, newViewCast);
                    if (edge.pointA != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointA);
                    }
                    if (edge.pointB != Vector3.zero)
                    {
                        viewPoints.Add(edge.pointB);
                    }
                }
            }

            // Add points hit to 'List<Vector3> viewPoints' from contact points from loop. This is getting difficult to describe.
            viewPoints.Add(newViewCast.point);

            // Values of 'oldViewCast' equal the values of 'newViewCast'
            oldViewCast = newViewCast;
        }

        // Counts all of the viewPoints from Vector3 List to in an array ('viewPoints + 1' (+ 1 is origin vertex)).
        int vertexCount = viewPoints.Count + 1;
        // Vector3 array created from vertexCount value (which already contained Vector3 data with each count).
        Vector3[] vertices = new Vector3[vertexCount];
        // Integer array of 'traingles' created from '(vertexCount - 2) × 3' (this keeps the integer array as triangles (?)).
        int[] triangles = new int[(vertexCount - 2) * 3];

        // Vertex 0 = Vector3.zero (the center/cast point for the arc).
        vertices[0] = Vector3.zero;
        // loop through the rest of the vertices and set them equal to the points in the viewPoint List.
        for (int i = 0; i < vertexCount - 1; i++)
        {
            // 'i + 1' prevents overwriting first vertex set, and 'InverseTransformPoint' will transform position from WORLD space to LOCAL space.
            vertices[i + 1] = transform.InverseTransformPoint(viewPoints[i]);

            // if the elements in vertexCount array is greater than 'vertexCount - 2' (prevents from going out of bounds of the array)...
            if (i < vertexCount - 2)
            {
                // Reads like this:
                triangles[i * 3] = 0;           // When 'i' = '0', triangle vertexPoints = [0,1,2]
                triangles[i * 3 + 1] = i + 1;   // When 'i' = '1', triangle vertexPoints = [0,2,3]
                triangles[i * 3 + 2] = i + 2;   // When 'i' = '2', triangle vertexPoints = [0,3,4]
            }
        }

        //viewMesh.Clear();                   // Reset everything in the viewMesh to start with.
        //viewMesh.vertices = vertices;       // Use vertice array-loop... thingy to RETURN vertex positions to draw the viewMesh with.
        //viewMesh.triangles = triangles;     // Use array for all the triangles in the mesh.
        //viewMesh.RecalculateNormals();      // Recalculates the mesh normals from the triangles and vertices.
    }
    #endregion

    #region METHOD (advanced) - Find Edge
    // At this point, I just have to say 'forget it', and get back to work.
    // This is a thing that finds edges of objects hit by Raycast, and if a ray in a triangle misses, it will redraw a new Vector3.
    EdgeInfo FindEdge(ViewCastInfo minViewCast, ViewCastInfo maxViewCast)
    {
        float minAngle = minViewCast.angle;
        float maxAngle = maxViewCast.angle;
        Vector3 minPoint = Vector3.zero;
        Vector3 maxPoint = Vector3.zero;

        for (int i = 0; i < edgeResolveIterations; i++)
        {
            float angle = (minAngle + maxAngle) / 2;
            ViewCastInfo newViewCast = ViewCast(angle);

            bool edgeDstThresholdExceeded = Mathf.Abs(minViewCast.dst - newViewCast.dst) > edgeDstThreshold;
            if (newViewCast.hit == minViewCast.hit && !edgeDstThresholdExceeded)
            {
                minAngle = angle;
                minPoint = newViewCast.point;
            }
            else
            {
                maxAngle = angle;
                maxPoint = newViewCast.point;
            }
        }
        return new EdgeInfo(minPoint, maxPoint);
    }
    #endregion

    #region METHOD (advanced) - View Cast
    // Method 'ViewCast' ('ViewCast' takes in an angle) using a copy of all of the variables in ViewCastInfo struct.
    ViewCastInfo ViewCast(float globalAngle)
    {
        // 'Vector3 dir' = 'public Vector3 DirFromAngle' (globalAngle uses angleInDegrees) (see 'METHOD - Direction From Angle').
        Vector3 dir = DirFromAngle(globalAngle, true);
        RaycastHit hit; // RaycastHit variable called 'hit'.

        // This 'if' statement uses a Raycast to set the variables found in the struct 'ViewCastInfo'.
        // if (Physics.Raycast(Vector3 origin, Vector3 dir, out RaycastHitInfo, float maxDistance, int layerMask))... ('if I hit any OBSTACLE'...)
        if (Physics.Raycast(transform.position, dir, out hit, viewRadius, obstacleMask))
        {
            // Set each of the four variables in 'public struct ViewCastInfo' (hit, point, dst, angle) to the information returned from the hit.
            return new ViewCastInfo(true, hit.point, hit.distance, globalAngle);
            // Stops the visualized Raycast from going through walls.
        }
        // In case I don't hit anything...
        else
        {
            // Visualized Raycast stops at the end of the Field of View arc.
            return new ViewCastInfo(false, transform.position + dir * viewRadius, viewRadius, globalAngle);
        }
    }
    #endregion
}