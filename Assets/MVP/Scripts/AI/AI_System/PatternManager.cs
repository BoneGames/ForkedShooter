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

    //public Pattern SelectPattern(List<Pattern> _patterns)
    //public Pattern SelectPattern(Decider d)
    //{
    //    //// currently just selects first pattern in group (decider)
    //    //if(_patterns.Count == 0)
    //    //{
    //    //    return null;
    //    //}

    //    // put probabilistic pattern selection in later

    //    Pattern patternToReturn;
    //    // get first pattern in list
    //    //patternToReturn = _patterns[0];
    //    return patternToReturn;
    //}

    // Includes interupt logic
    public void TryExecutePattern(Pattern incomingPattern, SenseMemoryFactory.SMData _data)
    {
        //remove currentPattern if it has stopped
        if (currentPattern && !currentPattern.isRunning)
        {
            currentPattern = null;
        }


        // if there is no pattern running
        if (!currentPattern)
        {
            Debug.Log("NEW Pattern: " + incomingPattern + ", OLD Pattern: " + currentPattern);
            incomingPattern.StartPatternWith(ai, _data);
            currentPattern = incomingPattern;
            return;
        }
        // if incoming pattern can interupt the current pattern(higher precedence)
        if (incomingPattern.patternType.precedence >= currentPattern.patternType.precedence && currentPattern != incomingPattern)
        {
            ai.ResetAI();
            Debug.Log("NEW Pattern: " + incomingPattern + ", OLD Pattern: " + currentPattern);
            currentPattern.PatternHasBeenInterrupted();
            incomingPattern.StartPatternWith(ai, _data);
            currentPattern = incomingPattern;
        }
        else
        {
            //Debug.Log("pattern not important enough to run");
        }

        

        // once pattern has started (i.e. currentPattern is not null), keep calling UpdatePattern until currentPattern is not running 
        //if (incomingPattern == currentPattern)
        //{
            currentPattern.UpdatePattern(ai, _data);
            return;
       // }

     
        
    }
}