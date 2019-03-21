using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetwork : MonoBehaviour {

	[SerializeField] private GameObject playerCam;
	[SerializeField] private MonoBehaviour[] playerControlScripts;
	private PhotonView photonView;

	private void Start()
	{
		photonView = GetComponent<PhotonView>();
		Initialise();
	}

	private void Initialise()
	{
		if(!photonView.isMine)
		{
			// disable other player's FP cam
			playerCam.SetActive(false);
			// disable other player's control scripts
			foreach(MonoBehaviour m in playerControlScripts)
			{
				m.enabled = false;
			}
		}
	}
}
