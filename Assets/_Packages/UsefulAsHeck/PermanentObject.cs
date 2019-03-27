using UnityEngine;
using UnityEngine.SceneManagement;

[AddComponentMenu("Useful As Heck/Object Permanence")]
public class PermanentObject : MonoBehaviour
{
    void Start()
    {
        Keep(); //Keeps on startup
    }

    public void Keep()
    {
        DontDestroyOnLoad(this); //Don't destroy this object on loading new scenes

        //If there's already one though...
        if (FindObjectsOfType(GetType()).Length > 1)
        {
            Destroy(gameObject); //...Destroy it. We don't need more of them.
        }
    }
}