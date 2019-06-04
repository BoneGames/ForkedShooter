using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Linq.Expressions;
using UnityEditor;
using System;
using BT;

public class PatternManager
{
    Pattern currentPattern;
    BehaviourAI ai;

    // initialise, get relevant AI class
    public PatternManager(BehaviourAI ai)
    {
        this.ai = ai;
    }

    //Func<Vector3, Vector3> GetDestinationInTotem()
    //{
    //    Func<Vector3, Vector3> fn = (v) =>
    //    {
    //        if (!localTotem)
    //        {
    //            return v;
    //        }
    //        Vector3 totemPos = localTotem.transform.position;
    //        float destinationDist = Vector3.Distance(totemPos, v);
    //        if (destinationDist > localTotem.radius)
    //        {
    //            // position + direction.normalized * radius 0 shortens destination to point on totem radius
    //            return totemPos + (v - totemPos).normalized * localTotem.radius;
    //        }
    //        return v;
    //    };
    //    return fn;
    //}
    public void TryExecutePattern(Pattern incomingPattern, SenseMemoryFactory.SMData _data)
    {
        string debug = _data.inspectionPoints.Count > 0 ? string.Format(BaneTools.ColorString("iPs: " + _data.inspectionPoints.Count + ", tLS: " + _data.targetLastSeen + ", tars: " + _data.targets.Count, Color.yellow)): "iPs: " + _data.inspectionPoints.Count + ", tLS: " + _data.targetLastSeen + ", tars: " + _data.targets.Count;
        Debug.Log(debug);
        //remove currentPattern if it has stopped
        if (currentPattern && !currentPattern.isRunning)
        {
            currentPattern = null;
        }
        // if there is no pattern running
        if (!currentPattern)
        {
            Debug.Log(string.Format(BaneTools.ColorString("NEW Pattern: " + incomingPattern + ", OLD Pattern ended", Color.red)));
            incomingPattern.StartPatternWith(ai, _data);
            currentPattern = incomingPattern;
            return;
        }
        // if incoming pattern is new and current pattern is interuptable
        if (incomingPattern != currentPattern && currentPattern.interuptable)
        {
            Debug.Log(string.Format(BaneTools.ColorString("NEW Pattern: " + incomingPattern + ", OLD Pattern: " + currentPattern, Color.green)));
            currentPattern.PatternHasBeenInterrupted(ai);
            incomingPattern.StartPatternWith(ai, _data);
            currentPattern = incomingPattern;
        }
        else // continue running current pattern
        {
            Debug.Log("Update Pattern: " + currentPattern);
            currentPattern.UpdatePattern(ai, _data);
        }
    }
}