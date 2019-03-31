using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonHealthMoniter : MonoBehaviour {

	public List<GameObject> players = new List<GameObject>();
	public Text player1;
	public Text player2;

	public void Register(GameObject go)
	{
		players.Add(go);
	}

	void Update()
	{
		if(players.Count > 1)
		{
			player1.text = players[0].name + ": " + players[0].GetComponent<PlayerHealth>().currentHealth.ToString() + " health";
			player2.text = players[1].name + ": " + players[1].GetComponent<PlayerHealth>().currentHealth.ToString() + " health";
		}
	}
}
