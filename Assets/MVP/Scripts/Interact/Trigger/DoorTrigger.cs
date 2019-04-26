using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [Header("Animation")]
    public Animator doorFront;
    public Animator doorBack;

    [Header("Enemies")]
    public GameObject drone;
    public int droneCount = 2;
    public GameObject enemy;
    public int enemyCount = 2;
    public Transform droneSpawnPoint;
    public Transform enemySpawnPoint;

    [Header("Variables")]
    public Transform enemyParent;
    public bool enemySpawned = false;
    public bool roomCleared = false;

    public Transform waypointParent;

    void Update()
    {
        if(enemyParent.transform.childCount <= 0 && enemySpawned == true && roomCleared == false)
        {
            roomCleared = true;
            doorFront.SetBool("Exit", true);
            doorBack.SetBool("Exit", true);
            print("Doors Activated");
        }
    }

    //When player enters triggerbox
    void OnTriggerEnter(Collider other)
    {
        if (enemySpawned == false)
        {
            //Only trigger is tag is player
            if (other.gameObject.CompareTag("Player"))
            {
                Debug.Log("DoorTrigger PLayer Enter Collision detected");
                //Set the trigger for the animator
                doorFront.SetTrigger("Enter");
                doorBack.SetTrigger("Enter");
                StartCoroutine(SpawnTimer());
            }
        }
    }
    IEnumerator SpawnTimer()
    {
        for (int i = 0; i < droneCount; i++)
        {
            GameObject clone = Instantiate(drone, droneSpawnPoint.position, droneSpawnPoint.rotation, droneSpawnPoint);
            clone.transform.name += i.ToString(); 
            clone.transform.parent = enemyParent;
            clone.GetComponent<AI_ScoutDrone>().waypointParent = waypointParent;
            yield return new WaitForSeconds(.1f);
        }
        for (int i = 0; i < enemyCount; i++)
        {
            GameObject clone = Instantiate(enemy, enemySpawnPoint.position, enemySpawnPoint.rotation, enemySpawnPoint);
            clone.transform.name += i.ToString(); 
            clone.transform.parent = enemyParent;
            clone.GetComponent<BehaviourAI>().waypointParent = waypointParent;
            yield return new WaitForSeconds(.1f);
        }

        enemySpawned = true;
    }
}
