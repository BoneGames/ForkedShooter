using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Linq.Expressions;
using System;
public class DecisionMachine
{
    //InvulTotem localTotem;
    List<Decider> deciders;
    PatternManager pM;

    // Constructor (initialise values)
    public DecisionMachine(InvulTotem _totem, List<Decider> _deciders, PatternManager _patternManager)
    {
        //this.localTotem = _totem;
        this.deciders = _deciders;
        this.pM = _patternManager;
    }

    // this lambda needs to be passed to pattern being executed (to constrain position to totem radius)
    //Func<Vector3, Vector3> MakeDestinationPointReduction()
    //{
    //    Func<Vector3, Vector3> fn = (v) =>
    //    {
    //        if(localTotem == null)
    //        {
    //            return v;
    //        }
    //        Vector3 tPosition = localTotem.transform.position;
    //        float destinationDist = Vector3.Distance(tPosition, v);
    //        if (destinationDist > localTotem.radius)
    //        {
    //            return tPosition + (v - tPosition).normalized * localTotem.radius;
    //        }
    //        return v;
    //    };
    //    return fn;
    //}
   
    // Wrapper Method that allocates data to each of the AI Modes - Conditions for
    // execution are defined at the top of each respective Mode class
    public void MakeDecisionFrom(SenseMemoryFactory.SMData data)
    {

        // SelectMany() makes 1 list out of all the pattern lists (derived/transformed from deciders) that pass the condition (based on SMData)
        List<Pattern> relevantPatterns = deciders.SelectMany(d => d.PatternsBasedOn(data)).ToList();
        // currently SelectPattern() just chooses the first pattern in the list
        Pattern pattern = pM.SelectPattern(relevantPatterns);
        //pM.ExecutePattern(pattern,data);

        
        Debug.Log(relevantPatterns.Count);
        //Pattern pattern = pM.SelectPattern(relevantPatterns);
        pM.ExecutePattern(pattern,data);
        //TurnOffDecidersBasedOn(pattern);
    }

    //public void TurnOffDecidersBasedOn(Pattern pattern)
    //{
    //    foreach (var decider in deciders)
    //    {
    //        if(decider.importanceScore < pattern.patternType.importanceScore)
    //        {
    //            decider.isEnabled = false;
    //        }
    //    }
    //}

    //public void TurnAllDecidersOn()
    //{
    //    foreach (var decider in deciders)
    //    {
    //        decider.isEnabled = true;
    //    }
    //}
}

public abstract class Decider
{
    public List<Pattern> patterns;
    // Constructor tells each instance to get it's patterns from it's Mode List
    public Decider(List<Pattern> _patterns)
    {
        this.patterns = _patterns;

        // tell each pattern what Mode it is (Naive, Suspicious, Combat)
        foreach (var p in this.patterns)
        {
            p.patternType = this;
        }
    }
    public bool isEnabled = true;
    public int precedence;
    public abstract List<Pattern> PatternsBasedOn(SenseMemoryFactory.SMData data);
}

public class NaiveDecider: Decider
{
     // Constructor - get patterns from list (in inspector)
     public NaiveDecider(List<Pattern> patterns): base(patterns)
     {
        precedence = 0;
     } 

    override public List<Pattern> PatternsBasedOn(SenseMemoryFactory.SMData data)
    {
        // if no direct/indirect evidence of adversaries
        bool condition = (data.inspectionPoints.Count == 0 && data.targets.Count == 0);
        if (condition)
        {
            Debug.Log(this.GetType().Name + ", passed");
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
    // Constructor - get patterns from list (in inspector)
    public SuspiciousDecider(List<Pattern> _patterns) : base(_patterns)
    {
        precedence = 1;
    }
    override public List<Pattern> PatternsBasedOn(SenseMemoryFactory.SMData _data)
    {
        // if no direct evidence but some indirect evidence of enemies
        bool condition = (_data.targets.Count == 0 && _data.inspectionPoints.Count > 0);
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
    // Constructor - get patterns from list (in inspector)
    public CombatDecider(List<Pattern> _patterns) : base(_patterns)
    {
        precedence = 2;
    }

    override public List<Pattern> PatternsBasedOn(SenseMemoryFactory.SMData _data)
    {
        // if direct evidence of enemies
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



