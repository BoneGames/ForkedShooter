using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternManager
{
    BehaviourAI ai;

    // initialise, get relevant AI class
    public PatternManager(BehaviourAI ai)
    {
        this.ai = ai;
    }

    public Pattern SelectPattern(List<Pattern> _patterns)
    {
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

    public void ExecutePattern(Pattern _pattern)
    {
        if(_pattern == null)
        {
            return;
        }
        _pattern.StartPatternWith(ai);
    }
}

public interface Pattern
{
    void StartPatternWith(BehaviourAI ai);
}
