using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor; // Extends the Unity Editor.

[CustomEditor(typeof(AI_FoV_Detection))] // CustomEditor is running off 'BossFoV_SearchLight' script.

public class AI_FoV_Detection_Editor : Editor // Base class to derive custom Editors from. Used to create custom inspectors and editors for objects.
{
    // Method creates custom 3D GUI controls and drawing in the scene view.
    void OnSceneGUI()
    {
        // Script reference = object is custom editor of BossFoV_SearchLight script.
        AI_FoV_Detection fow = (AI_FoV_Detection)target;

        // 3D GUI drawing colour
        Handles.color = Color.white;

        // Draw a circular arc in 3D space(Vector3 center, Vector3 normal, Vector3 from, float angle, float radius) or...
        // (CENTER = Start from center point, NORMAL = rotate on direction, FROM = draw arc from direction, ANGLE = arc size, RADIUS = arc distance)
        Handles.DrawWireArc(fow.transform.position, Vector3.up, Vector3.forward, 360, fow.viewRadius);

        // Creates two Vector3 points using 'Direction From Angle' Method, where angleInDegrees is +/- the viewAngle, and it's NOT a globalAngle.
        // +/- in both Vector3s needed in Handles below to get them going in two different directions.
        // NOT a globalAngle = the line will rotate using Global angles.
        Vector3 viewAngleA = fow.DirFromAngle(-fow.viewAngle / 2, false);
        Vector3 viewAngleB = fow.DirFromAngle(fow.viewAngle / 2, false);

        // Draws a line from point 1 to point 2 (hence p1 & p2 respectively) using Vector3 transforms. To clarify:
        // p1 = the start position (once again, from the center)
        // p2 = the end position (the center + viewAngle(A | B) × viewRadius (set the length of the line)).
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleA * fow.viewRadius);
        Handles.DrawLine(fow.transform.position, fow.transform.position + viewAngleB * fow.viewRadius);

        // EXPUNGED - Now handled in main script (OnDrawGizmos).
        #region DrawLine(transform→Target)
        // See 'Color.white'.
//        Handles.color = Color.red;
        // foreach (for each instance of) 'visibleTarget' added to the list in the 'BossFoV_SearchLight' script...
//        foreach (Transform visibleTarget in fow.visibleTargets)
//        {
            // Draw a line from the script's transform position in 3D space to the target's position.
//            Handles.DrawLine(fow.transform.position, visibleTarget.position);
//        }
        #endregion
    }

}
