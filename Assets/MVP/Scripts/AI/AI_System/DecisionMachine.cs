﻿using System.Collections;
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
    public EnemyHealth health;

    // Constructor (initialise values)
    public DecisionMachine(InvulTotem _totem, List<Decider> _deciders, PatternManager _patternManager, EnemyHealth _health)
    {
        this.localTotem = _totem;
        this.deciders = _deciders;
        this.pM = _patternManager;
        this.health = _health;
    }

    // this lambda needs to be passed to pattern being executed (to constrain position to totem radius)
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

    // Wrapper Method that allocates data to each of the AI Modes - Conditions for
    // execution are defined at the top of each respective Mode class
    public void MakeDecisionFrom(SenseMemoryFactory.SMData senseData)
    {

        // SelectMany() makes 1 list out of all the pattern lists (derived/transformed from deciders) that pass the condition (based on SMData)
        //List<Pattern> relevantPatterns = deciders.SelectMany(d => d.DeciderBasedOn(data)).ToList();

        Decider relevantDecider = null;
        Pattern pattern = null;
        foreach (Decider d in deciders)
        {
            if(d.DeciderBasedOn(senseData) != null)
            {
                relevantDecider = d;
                break;
            }
        }

        if (relevantDecider != null)
        {
            // currently SelectPattern() just chooses the first pattern in the list
            //Pattern pattern = pM.SelectPattern(relevantDecider);
            pattern = relevantDecider.ChoosePattern(senseData);
        }
        else
        {
            Debug.Log("No relevant Decider chosen! (MakeDecisionFrom() method)");
            return;
        }


        //Pattern pattern = pM.SelectPattern(relevantPatterns);
        if(pattern)
        pM.TryExecutePattern(pattern, senseData);
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
    //public Dictionary<string, Pattern> patterns;
    public List<Pattern> patterns; 
    protected EnemyHealth health;
    // Constructor tells each instance to get it's patterns from it's Mode List
    //public Decider(Dictionary<string, Pattern> _patterns, EnemyHealth _health)
    public Decider(List<Pattern> _patterns, EnemyHealth _health)
    {
        this.patterns = _patterns;
        this.health = _health;
        // tell each pattern what Mode it is (Naive, Suspicious, Combat)
        foreach (var p in this.patterns)
        {
            //p.patternType = this;
        }
    }
    public bool isEnabled = true;
    public int precedence;
    //public abstract List<Pattern> DeciderBasedOn(SenseMemoryFactory.SMData data);
    public abstract Decider DeciderBasedOn(SenseMemoryFactory.SMData senseData);
    public abstract Pattern ChoosePattern(SenseMemoryFactory.SMData senseData);
}

public class NaiveDecider: Decider
{
     // Constructor - get patterns from list (in inspector)
     //public NaiveDecider(Dictionary<string, Pattern> patterns, EnemyHealth health): base(patterns, health)
     public NaiveDecider(List<Pattern> patterns, EnemyHealth health): base(patterns, health)
     {
        precedence = 0;
     } 

    public override Decider DeciderBasedOn(SenseMemoryFactory.SMData data)
    {
        // if no direct/indirect evidence of adversaries
        bool condition = (data.inspectionPoints.Count == 0 && data.targets.Count == 0 && data.targetLastSeen == Vector3.zero);
        if (condition)
        {
            //Debug.Log(this.GetType().Name + ", passed");
            return this;
        }
        else
        {
            // return empty set of patterns (basically null state)
            return null;
        }
    }

    public override Pattern ChoosePattern(SenseMemoryFactory.SMData senseData)
    {
        Pattern p = null;
        // Return Patrol Pattern (Only Naive Pattern)
        p = patterns[0];
        return p;
    }
}
public class SuspiciousDecider: Decider
{
    // Constructor - get patterns from list (in inspector)
    //public SuspiciousDecider(Dictionary<string, Pattern> patterns, EnemyHealth health) : base(patterns, health)
    public SuspiciousDecider(List<Pattern> patterns, EnemyHealth health) : base(patterns, health)
    {
        precedence = 1;
    }

    override public Decider DeciderBasedOn(SenseMemoryFactory.SMData _data)
    {
        // if no direct evidence but some indirect evidence of enemies
        bool condition = (_data.targets.Count == 0 && (_data.inspectionPoints.Count > 0 || (_data.targetLastSeen != Vector3.zero || _data.targetLastSeen != Vector3.zero)));
        if (condition)
        {
            //Debug.Log(this.GetType().Name + ", passed");
            // return set of suspicious patterns
            return this;
        }
        else
        {
            // return empty set of patterns (basically null state)
            return null;
        }
    }

    public override Pattern ChoosePattern(SenseMemoryFactory.SMData senseData)
    {
        Pattern p = null;
        // Randomize investigate and Survey Pattern execution
        int random = UnityEngine.Random.Range(0, patterns.Count);
        p = patterns[random];
        return p;
    }
}
public class CombatDecider: Decider
{
    // Constructor - get patterns from list (in inspector)
    //public CombatDecider(Dictionary<string, Pattern> patterns, EnemyHealth health) : base(patterns, health)
    public CombatDecider(List<Pattern> patterns, EnemyHealth health) : base(patterns, health)
    {
        precedence = 2;
    }

    override public Decider DeciderBasedOn(SenseMemoryFactory.SMData _data)
    {
        // if direct evidence of enemies
        bool condition = (_data.targets.Count > 0);
        if (condition)
        {
            //Debug.Log(this.GetType().Name + ", passed");
            // return set of combat patterns
            return this;
        }
        else
        {
            // return empty set of patterns (basically null state)
            return null;
        }
    }

    public override Pattern ChoosePattern(SenseMemoryFactory.SMData senseData)
    {
        // NOTE: generate random number between 0 and currentHealth * 2;
        // this number replaces health.currentHealth in all ifs below
        // same for distance

        Pattern p = null;
        if (health.currentHealth > 85)
        {
            if(senseData.distance < 20)
            {
                // Strafe Fire
                p = patterns[1];
            }
            else
            {
                // Charge Fire
                p = patterns[0];
            }
            
        }
        else if (health.currentHealth > 30)
        {
            // Cover shoot
            p = patterns[2];
        }
        else
        {
            // Retreat
            p = patterns[3];
        }
        return p;
    }
}



