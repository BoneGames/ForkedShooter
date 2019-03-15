using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
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
    }

    public override void OnStartClient()
    {
        base.OnStartClient();

        string netID = GetComponent<NetworkIdentity>().netId.ToString();
        Player player = GetComponent<Player>();
        GameManager.RegisterPlayer(netID,player);        
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

        GameManager.UnRegisterPlayer(transform.name);
	}

    [Command]
    public void CmdPlayerShot(string _playerID, int _damage)
    {
        Debug.Log(_playerID + " was hit by ");
        Player _player = GameManager.GetPlayer(_playerID);
        _player.TakeDamage(_damage);
    }
}
