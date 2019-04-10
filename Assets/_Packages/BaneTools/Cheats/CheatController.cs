using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheatController : MonoBehaviour
{
    public KeyCode[] sequence;
    private int sequenceIndex;

    private void Update()
    {
        if (Input.GetKeyDown(sequence[sequenceIndex]))
        {
            if (++sequenceIndex == sequence.Length)
            {
                sequenceIndex = 0;
                Debug.Log("Yay");
                // sequence typed
            }
        }
        else if (Input.anyKeyDown) sequenceIndex = 0;
    }
}
