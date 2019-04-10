using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enterable : MonoBehaviour
{
    public virtual void OnTriggerEnter(Collider other)
    {
        //Resets to scene at index 0 in Build Settings
        SceneManager.LoadScene(0);
    }
}
