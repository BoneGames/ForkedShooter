using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ContinueToMenu : MonoBehaviour
{
    public int sceneLoad;
    //integer for the next scene/level to load
    public Animator animator;
    //references the animator used to fade in and fade out


    // Update is called once per frame
    void Update()
    {
        //if any input on the keyboard is pushed
        if (Input.anyKey)
        {
            //activates FadeToLevel function, which will fade out the scene and load the next scene
            FadeToLevel(sceneLoad);
        }

    }
    public void FadeToLevel(int sceneIndex)
    {
        //scene load is equal to our scene index
        sceneLoad = sceneIndex;
        //on trigger, the scene will begin the fade out animation
        animator.SetTrigger("FadeOut");
    }
    public void FadeComplete()
    {
        //the scene manager loads the next scene upon the fade out animation completing
        SceneManager.LoadScene(sceneLoad);
    }
}
