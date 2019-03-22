using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetwork : MonoBehaviour {

	[SerializeField] private GameObject playerCam;
	[SerializeField] private MonoBehaviour[] playerControlScripts;
	private PhotonView photonView;

    public int health = 100;

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

    private void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //Send health data to network
        if(stream.isWriting)
        {
            stream.SendNext(health);
        }
        // recieve health data from network (other player)
        else if(stream.isReading)
        {
            health = (int)stream.ReceiveNext();
        }
    }
}
