using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Linq.Expressions;
using System;
public class DecisionMachine
{
    InvulTotem localTotem;
    List<Decider> deciders;
    PatternManager pM;

    // Constructor (initialise values)
    public DecisionMachine(InvulTotem _totem, List<Decider> _deciders, PatternManager _patternManager)
    {
        this.localTotem = _totem;
        this.deciders = _deciders;
        this.pM = _patternManager;
    }

    // this lambda needs to be passed to pattern being executed (to constrain position to totem radius)
    Func<Vector3, Vector3> MakeDestinationPointReduction()
    {
        Func<Vector3, Vector3> fn = (v) =>
        {
            if(localTotem == null)
            {
                return v;
            }
            Vector3 tPosition = localTotem.transform.position;
            float destinationDist = Vector3.Distance(tPosition, v);
            if (destinationDist > localTotem.radius)
            {
                return tPosition + (v - tPosition).normalized * localTotem.radius;
            }
            return v;
        };
        return fn;
    }
   
    // Wrapper Method that allocates data to each of the AI Modes - Conditions for
    // execution are defined at the top of each respective Mode class
    public void MakeDecisionFrom(SenseMemoryFactory.SMData data)
    {
        // 1st d is paramaters of lambda func, 2nd d is paramater used in lambda
        // e.g. in the lambda (a => a + 5), first a is the parameter and second a in a + 5 is the parameter being used in lambda
        List<Pattern> relevantPatterns = deciders.SelectMany(d => d.PatternsBasedOn(data)).ToList();
        Pattern pattern = pM.SelectPattern(relevantPatterns);
        pM.ExecutePattern(pattern);
    }
}
public class NaiveDecider: Decider
{
    List<Pattern> patterns;

    public NaiveDecider(List<Pattern> _patterns)
    {
        this.patterns = _patterns;
    }

    public List<Pattern> PatternsBasedOn(SenseMemoryFactory.SMData data)
    {
        bool condition = (data.inspectionPoints.Count == 0 && data.targets.Count == 0);
        if (condition)
        {
            Debug.Log(this.GetType().Name + ", passed");
            // return set of naive patterns
            return patterns;
        }
        else
        {
            // return empty set of patterns (basically null state)
            return new List<Pattern>();
        }
    }
}
public class SuspiciousDecider: Decider
{

    List<Pattern> patterns;

    public SuspiciousDecider(List<Pattern> _patterns)
    {
        this.patterns = _patterns;
    }
    public List<Pattern> PatternsBasedOn(SenseMemoryFactory.SMData _data)
    {
        bool condition = (_data.inspectionPoints.Count > 0 && _data.targets.Count == 0);
        if (condition)
        {
            Debug.Log(this.GetType().Name + ", passed");
            // return set of suspicious patterns
            return patterns;
        }
        else
        {
            // return empty set of patterns (basically null state)
            return new List<Pattern>();
        }
    }
}
public class CombatDecider: Decider
{
    List<Pattern> patterns;

    // Add Combat Patterns to Combat Decider
    public CombatDecider(List<Pattern> _patterns)
    {
        this.patterns = _patterns;
    }

    public List<Pattern> PatternsBasedOn(SenseMemoryFactory.SMData _data)
    {
        bool condition = (_data.targets.Count > 0);
        if (condition)
        {
            Debug.Log(this.GetType().Name + ", passed");
            // return set of combat patterns
            return patterns;
        }
        else
        {
            // return empty set of patterns (basically null state)
            return new List<Pattern>();
        }
    }
}


public interface Decider
{
    List<Pattern> PatternsBasedOn(SenseMemoryFactory.SMData data);
}