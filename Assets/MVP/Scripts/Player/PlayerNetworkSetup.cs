using UnityEngine;
using UnityEngine.Networking;

public class PlayerNetworkSetup : NetworkBehaviour {
    [SerializeField]
    Behaviour[] componenetsToDisable;
    [SerializeField]
    string remoteLayername = "RemotePlayer";
    GameObject sceneCam;

    void Start()
    {
        if (!isLocalPlayer)
        {
            DisableComponents();
            AssignRemoteLayer();
        } else {
            // disable Scene Cam (not the fps cam)
            sceneCam = GameObject.FindGameObjectWithTag("SceneCam");
            sceneCam.SetActive(false);
        }
        RegisterPlayer();
    }

    void RegisterPlayer()
    {
        string _ID = "Player " + GetComponent<NetworkIdentity>().netId;
        transform.name = _ID;
    }

    void DisableComponents()
    {
        if(!isLocalPlayer)
		{
			for(int i = 0; i < componenetsToDisable.Length; i++)
			{
				componenetsToDisable[i].enabled = false;
			}
		} 
    }

    void AssignRemoteLayer()
    {
        gameObject.layer = LayerMask.NameToLayer(remoteLayername);
    }

	void OnDisable()
	{
		if(sceneCam)
		{
			sceneCam.gameObject.SetActive(true);
		}
	}
}
