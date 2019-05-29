using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AI_Suspicious : BehaviourAI
{
    // Start is called before the first frame update
    void OnEnable()
    {
        intensity = 1;

        agent.speed = moveSpeed[1];
    }

    public override bool LookForPlayer()
    {
        return base.LookForPlayer();
    }

    // Update is called once per frame
    void Update()
    {
        if(LookForPlayer())
        {
            ModeSwitch(true);
            return;
        }

        switch(intensity)
        {
            case 1:
                Investigate();
                break;
            case 0:
                Survey();
                break;
            default:
                investigatePoint = Vector3.zero;
                ModeSwitch(false);
                break;
        }
    }

    public void Investigate()
    {
        agent.SetDestination(investigatePoint);

        //if (DestinationReached(0.5f))
        //{
        //    intensity--;
        //}
    }

    void Survey()
    {
        if (initVar)
        {
            startRotation = transform.rotation;
            // clear path
            if (agent.hasPath)
            {
                agent.ResetPath();
            }
        }
        RaycastHit hit;
        float seeingDist = 1;

        // spin speed is relative to length of sightLine
        if (Physics.Raycast(transform.position, transform.forward, out hit, 100))
        {
            seeingDist = Vector3.Distance(transform.position, hit.point) / 4;
        }
        // spin to check surroundings
        transform.Rotate(Vector3.up * Time.deltaTime * 300 / seeingDist);
        // after 1 full revolution
        if (transform.rotation.eulerAngles.y > startRotation.eulerAngles.y - 5 && transform.rotation.eulerAngles.y < startRotation.eulerAngles.y)
        {
            initVar = true;
            intensity--;
            return;
        }
        initVar = false;
    }
}
