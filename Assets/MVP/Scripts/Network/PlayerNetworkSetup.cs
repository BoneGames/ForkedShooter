using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkSetup : MonoBehaviour {

	[SerializeField] private Camera playerCam;
	[SerializeField] private MonoBehaviour[] playerControlScripts;

	
	PhotonView photonView;

	private void Awake()
	{
		photonView = GetComponent<PhotonView>();
		Initialise();
	}

	private void Initialise()
	{
		if(!photonView.isMine)
		{
			// disable other player's FP cam
			playerCam.enabled = false;
			// disable other player's control scripts
			foreach(MonoBehaviour m in playerControlScripts)
			{
				m.enabled = false;
			}
		}
	}
}
