using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using BT;

public class FadeToQuit : MonoBehaviour
{
    public KeyCode quitKey;

    public float holdTime;
    float startTime;
    float timer;

    public Image dark;

    // Use this for initialization
    void Start()
    {
        startTime = 0;

        if (gameObject.name == "QuitFadeObject")
        {
            dark = GetComponent<Image>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(quitKey))
        {
            timer = 0;
        }

        if (Input.GetKey(quitKey))
        {
            timer += Time.deltaTime;

            var tempColor = dark.color;
            tempColor.a = (timer/holdTime);
            dark.color = tempColor;

            if (timer > holdTime)
            {
            	print(BaneTools.ColorString("Quit Successfully", BaneTools.Color255(255,0,0)));
                Application.Quit();
            }
        }

        if (Input.GetKeyUp(quitKey))
        {
            var tempColor = dark.color;
            tempColor.a = 0;
            dark.color = tempColor;
        }
    }
}
