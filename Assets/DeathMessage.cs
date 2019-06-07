using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DeathMessage : UIHandler
{
    Text displaytext;
    public float textDuration;

    public override void Awake()
    {
        displaytext = GetComponent<Text>();
    }

    public void StartRespawnText()
    {
        StopAllCoroutines();
        StartCoroutine(RespawnText());
    }

    IEnumerator RespawnText()
    {
        float timeTillRespawn = textDuration;
        while(timeTillRespawn > 0)
        {
            timeTillRespawn -= Time.deltaTime;
            displaytext.text = "You died and will respawn in \n" + (int)timeTillRespawn + " seconds";
            yield return null;
        }
        displaytext.text = "";
    }
}
