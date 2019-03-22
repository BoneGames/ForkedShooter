using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetwork : MonoBehaviour {

	[SerializeField] private GameObject playerCam;
	[SerializeField] private MonoBehaviour[] playerControlScripts;
	private PhotonView photonView;
    public int health = 100;
	string ID;

	private void Start()
	{
		FindObjectOfType<PhotonHealthMoniter>().Register(gameObject);
		photonView = GetComponent<PhotonView>();
		Initialise();
		ID = GetComponent<PhotonView>().viewID.ToString();
	}

	private void Update()
	{
		if(!photonView.isMine)
		{
			return;
		}
		if(Input.GetKeyDown(KeyCode.E))
		{
			Debug.Log("you just reduced health by 5 by pressing 'E'"); 
			health -= 5;
		}
	}

	[PunRPC]
	public void ChangeHealth(int Damage)
	{
		health -= Damage; 
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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
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

	// void OnGUI()
	// {
	// 	GUI.Box(new Rect(0, 0, Screen.width/3, Screen.height/6), "Player" + ID +  " health: " + health);
	// 	GUI.Box(new Rect(0, 100, Screen.width/3, Screen.height/6), "Player" + ID +  " health: " + health);
	// }
}
