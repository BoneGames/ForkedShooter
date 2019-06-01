using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDrops : MonoBehaviour
{
  public List<GameObject> drops = new List<GameObject>();

  public void DropRandomItem()
  {
    print("Dropping a random item");
    Instantiate(drops[Random.Range(0, drops.Count - 1)], transform.position, Quaternion.identity);
  }

}