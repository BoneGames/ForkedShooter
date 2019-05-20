using UnityEngine;
using System.Collections;

public class AI_State_Machine : MonoBehaviour
{
    //Player Transform
    protected Transform target;

    //List of points for patrolling
    protected GameObject[] wayPoints;

    //Bullet shooting rate
    protected float shootRate;
    protected float elapsedTime;

    public Transform bulletSpawnPoint { get; set; }

    protected virtual void Initialize() { }
    protected virtual void FSMUpdate() { }
    protected virtual void FSMFixedUpdate() { }

    // Use this for initialization
    void Start()
    {
        Initialize();
    }

    // Update is called once per frame
    void Update()
    {
        FSMUpdate();
    }

    void FixedUpdate()
    {
        FSMFixedUpdate();
    }
}
