using UnityEngine;
using UnityEngine.Networking;

public class PlayerSetup : NetworkBehaviour {
	[SerializeField]
	Behaviour[] componenetsToDisable;
	GameObject sceneCam;

	void Start()
	{
		if(!isLocalPlayer)
		{
			for(int i = 0; i < componenetsToDisable.Length; i++)
			{
				componenetsToDisable[i].enabled = false;
			}
		} else {
			// disable Scene Cam (not the fps cam)
			sceneCam = GameObject.FindGameObjectWithTag("SceneCam");
			sceneCam.SetActive(false);
		}
	}	

	void OnDisable()
	{
		if(sceneCam)
		{
			sceneCam.gameObject.SetActive(true);
		}
	}
}
