using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SenseMemoryFactory
{
    // Class that holds AI world data
    public class SMData
    {
        // Sighted enemie's positions
        public List<Transform> targets;
        // Positions where strange activity detected
        public List<Vector3> inspectionPoints;
        // lst seen position of target
        public Vector3 targetLastSeen;
        // distance to closest Target
        public float distance;

        // Constructor
        public SMData(List<Transform> _targets, List<Vector3> _inspectionPoints, float _distance, Vector3 _targetLastSeen)
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
    public List<Vector3> inspectionPoints;
    // stores last seen position of target
    public Vector3 targetLastSeen;

    // initialise variables
    public SenseMemoryFactory(AI_FoV_Detection sight)
    {
        this.targetLastSeen = new Vector3();
        this.inspectionPoints = new List<Vector3>();
        this.sight = sight;
    }

    public SMData GetSMData()
    {
        // get visual targets list and assign Vector3 from Transform conversion (linq)
        List<Transform> _targets = sight.visibleTargets.ToList();
        // return Sense Memory Data
        float _distance = sight.distance2Target;
        if(_targets.Count > 0)
        {
            targetLastSeen = _targets[0].position;
            // clear the inspectionPoints list
            inspectionPoints.Clear();
        }
        
        return new SMData(_targets, inspectionPoints, _distance, targetLastSeen);
    }
}
