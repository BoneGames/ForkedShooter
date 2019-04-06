using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [Header("Animation")]
    public Animator doorFront;
    public Animator doorBack;

    [Header("Variables")]
    public GameObject drone;
    public GameObject enemy;
    public Transform enemyParent;
    public bool enemySpawned = false;
    public bool roomCleared = false;
    public Transform droneSpawnPoint;
    public Transform enemySpawnPoint;

    public Transform waypointParent;

    public int spawnDrone = 2;
    public int spawnEnemy = 2;

    void Update()
    {
        if(enemyParent.transform.childCount <= 0 && enemySpawned == true && roomCleared == false)
        {
            roomCleared = true;
            doorFront.SetBool("Exit", true);
            doorBack.SetBool("Exit", true);
            print("Fuck");
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
                Debug.Log("Collision detected");
                //Set the trigger for the animator
                doorFront.SetTrigger("Enter");
                doorBack.SetTrigger("Enter");
                StartCoroutine(SpawnTimer());

            }
        }
    }
    IEnumerator SpawnTimer()
    {

        for (int i = 0; i < spawnDrone; i++)
        {
            GameObject clone = Instantiate(drone, droneSpawnPoint.position, droneSpawnPoint.rotation);
            clone.transform.parent = enemyParent;
            clone.GetComponent<AI_ScoutDrone>().waypointParent = waypointParent;
            yield return new WaitForSeconds(1);
        }
        for (int i = 0; i < spawnEnemy; i++)
        {
            GameObject clone = Instantiate(enemy, enemySpawnPoint.position, enemySpawnPoint.rotation);
            clone.transform.parent = enemyParent;
            clone.GetComponent<BehaviourAI>().waypointParent = waypointParent;
            yield return new WaitForSeconds(1);
        }

        enemySpawned = true;

    }

 
}
