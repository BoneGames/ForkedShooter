﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SenseMemoryFactory
{
    // Class that holds AI world data
    public class SMData
    {
        // Sighted enemie's positions
        public List<Vector3> targets;
        // Positions where strange activity detected
        public List<Vector3> inspectionPoints;
        // lst seen position of target
        public Vector3 targetLastSeen;
        // distance to closest Target
        public float distance;

        // Constructor
        public SMData(List<Vector3> _targets, List<Vector3> _inspectionPoints, float _distance, Vector3 _targetLastSeen)
        {
            this.targets = _targets;
            this.inspectionPoints = _inspectionPoints;
            this.distance = _distance;
            this.targetLastSeen = _targetLastSeen;
        }
    }
    // AI sees with this class - sight.visibleTargets is the targets list
    AI_FoV_Detection sight;
    // Ai investigates points with this list
    List<Vector3> inspectionPoints = new List<Vector3>();
    // stores last seen position of target
    public Vector3 targetLastSeen;

    public SenseMemoryFactory(AI_FoV_Detection sight)
    {
        /*
        eg: this.hearing = hearing - make dedicated audio alerts class that checks for 
        sound radius of different world events - using inbuilt Unity sound
       */
        // -- the two lines below allow for adding multiple inspection 
        // -- points later if the AI can maintain a list for subsequant checks
        // inspectionPoints = new List<Vector3>();
        // this.inspectionPoints.Add(inspectionPoint);
        this.sight = sight;
    }

    public void BulletAlert(Vector3 inspectionPoint)
    {
        this.inspectionPoints[0] = inspectionPoint;
    }

    public SMData GetSMData()
    {
        // create temporary(copied )list to return
        List<Vector3> _inspectionPoints = new List<Vector3>(inspectionPoints);
        // clear the inspectionPoints list
        inspectionPoints.Clear();
        // get visual targets list and assign Vector3 from Transform conversion (linq)
        List<Vector3> _targets = sight.visibleTargets.Select(t => t.position).ToList();
        // return Sense Memory Data
        float _distance = sight.distance2Target;
        if(_targets.Count > 0)
        {
            targetLastSeen = _targets[0];
        }
        return new SMData(_targets, _inspectionPoints, _distance, targetLastSeen);
    }
}
