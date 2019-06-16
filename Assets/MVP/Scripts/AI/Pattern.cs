using UnityEngine;

public class Pattern : ScriptableObject
{
    public bool isInteruptable;
    public bool notePrecedence;
    int precedence;
    public bool isRunning = false;
    public Decider patternType = null;
    public virtual void OnEnable()
    {
        isRunning = false;
    }
    
    public virtual void StartPatternWith(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {
        isRunning = true;
        ai.SetSpeed(patternType.precedence);
        ai.currentPattern = this;
        ai.CurrentData = data;
    }

    public virtual void UpdatePattern(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {

    }
    public virtual void KillPattern(BehaviourAI ai)
    {
        isRunning = false;
        ai.playerTarget = null;
        ai.agent.updateRotation = true;
        ai.lookAtTarget = false;
    }
}