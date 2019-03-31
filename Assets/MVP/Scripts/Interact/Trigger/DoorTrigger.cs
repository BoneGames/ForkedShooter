using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorTrigger : MonoBehaviour
{
    [Header("Animation")]
    public Animator anim;

    [Header("Variables")]
    public GameObject enemy;
    public Transform enemyParent;
    public bool enemySpawned = false;
    public bool roomCleared = false;
    public Transform spawnPoint;

    public Transform waypointParent;

    public int spawnEnemy = 2;

    void Update()
    {
        if(enemyParent.transform.childCount <= 0 && enemySpawned == true && roomCleared == false)
        {
            roomCleared = true;
            anim.SetBool("Exit", true);
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
                anim.SetTrigger("Enter");

                StartCoroutine(SpawnTimer());

            }
        }
    }
    IEnumerator SpawnTimer()
    {

        for (int i = 0; i < spawnEnemy; i++)
        {
            GameObject clone = Instantiate(enemy, spawnPoint.position, spawnPoint.rotation);
            clone.transform.parent = enemyParent;
            clone.GetComponent<AI_ScoutDrone>().waypointParent = waypointParent;
            yield return new WaitForSeconds(1);
        }
        enemySpawned = true;

    }

 
}
