using UnityEngine;

public class InsideCollider : MonoBehaviour
{
    public bool printResult;
    void Update()
    {
        InsideMesh(Camera.main.transform, transform);
    }

    public bool InsideMesh(Transform _checkOrigin, Transform _objectInside)
    {
        //Stores raycast hit data
        RaycastHit hit;

        //Gets the direction from the transform being checked to the origin being checked from (usually camera and shootpoint)
        Vector3 dir = _checkOrigin.position - _objectInside.position;
        //The distance between the two objects so we don't raycast too far
        float dist = dir.magnitude;


        Debug.DrawRay(_checkOrigin.position, -dir);
        //If we raycast...
        if (Physics.Raycast(_checkOrigin.position, -dir * dist, out hit))
        {
            //...and hit an object tagged as DoNotShootInside...
            if (hit.collider.CompareTag("DoNotShootInside")) //NOTE: This should be reconsidered somehow - <Bane>
            {
                ///...we are inside that object!
                if(printResult)
                print("Inside mesh!");
                return true;
            }
        }
        //Otherwise, we're not inside an object and can do what we wish!
        if (printResult)
        print("Outside mesh!");
        return false;
    }
}
