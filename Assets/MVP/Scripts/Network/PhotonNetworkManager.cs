using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PhotonNetworkManager : Photon.MonoBehaviour {
	[SerializeField] private Text connectText;
	[SerializeField] private GameObject player;
	[SerializeField] private GameObject lobbyCamera;
	[SerializeField] private Transform spawnPoint;

	void Start () {
		PhotonNetwork.ConnectUsingSettings("0.1");
	}

	public virtual void OnJoinedLobby()
	{
		Debug.Log("Lobby Joined.");
		PhotonNetwork.JoinOrCreateRoom("testLobby", null, null);
	}

	public virtual void OnJoinedRoom()
	{
		// spawn in player prefab
		PhotonNetwork.Instantiate(player.name, spawnPoint.position, spawnPoint.rotation, 0);
		// disable lobby cam
		lobbyCamera.SetActive(false);
	}
	
	void Update () {
		connectText.text = PhotonNetwork.connectionStateDetailed.ToString();
	}
}
