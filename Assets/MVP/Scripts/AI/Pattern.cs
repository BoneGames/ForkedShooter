using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pattern : ScriptableObject
{
    public void OnEnable()
    {
        isRunning = false;
    }
    public bool isRunning = false;
    public Decider patternType = null;
    public virtual void StartPatternWith(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {
        isRunning = true;
    }

    public virtual void UpdatePattern(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {

    }

    public virtual void PatternHasEnded()
    {
        isRunning = false;
    }

    public virtual void PatternHasBeenInterrupted()
    {
        isRunning = false;
    }
}