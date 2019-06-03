using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class PatternManager
{
    Pattern currentPattern;
    BehaviourAI ai;

    // initialise, get relevant AI class
    public PatternManager(BehaviourAI ai)
    {
        this.ai = ai;
    }

    public Pattern SelectPattern(List<Pattern> _patterns)
    {
        // currently just selects first pattern in group (decider)
        if(_patterns.Count == 0)
        {
            return null;
        }
        // put probabilistic pattern selection in later

        Pattern patternToReturn;
        // get first pattern in list
        patternToReturn = _patterns[0];
        return patternToReturn;
    }

    // Includes interupt logic
    public void ExecutePattern(Pattern incomingPattern, SenseMemoryFactory.SMData _data)
    {
        Debug.Log("Execute Pattern");
        Debug.Log(incomingPattern);
        Debug.Log(currentPattern);

        //remove currentPattern if it is not running
        if (currentPattern != null && !currentPattern.isRunning)
        {
            currentPattern = null;
        }

        //if incoming _pattern exists and start the incoming _pattern
        if (incomingPattern != null)
        {
            // if there is no pattern running, or the incoming pattern can interupt the current pattern (higher importanceScore)
            if (currentPattern == null || incomingPattern.patternType.precedence > currentPattern.patternType.precedence)
            {
                //prepare to interrupt currentPattern
                if (currentPattern != null)
                {
                    currentPattern.PatternHasBeenInterrupted();
                }
                incomingPattern.StartPatternWith(ai, _data);
                currentPattern = incomingPattern;
                return;
            }
        }
        

        // once pattern has started (i.e. currentPattern is not null), keep calling UpdatePattern until currentPattern is not running 
        if (currentPattern != null && currentPattern.isRunning )
        {
            currentPattern.UpdatePattern(ai, _data);
            return;
        }

        //
    


     

     
        Debug.Log("this makes no sense - no patterns executed");
    }
}

public class Pattern : ScriptableObject
{
    public bool isRunning = false;
    public Decider patternType = null;
    public virtual void StartPatternWith(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {
        isRunning = true;
    }

    public virtual void UpdatePattern(BehaviourAI ai, SenseMemoryFactory.SMData data)
    {
        
    }

    protected void StopPattern()
    {
        PatternHasEnded();
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
