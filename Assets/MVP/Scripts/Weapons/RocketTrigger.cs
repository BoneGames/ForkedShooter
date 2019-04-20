using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RocketTrigger : RocketLauncher {


    private void OnTriggerEnter(Collider other)
    {
        if(other.name == "TerrainLP")
        {
            isBuried = true;
            Debug.Log(isBuried);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.name == "TerrainLP")
        {
            isBuried = false;
            Debug.Log(isBuried);
        }
    }
}
