using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

	public static bool isOnline = false;

	void Awake()
	{
		if(FindObjectOfType<PhotonNetworkManager>())
		{
			isOnline = true;
		}
	}
	
}
