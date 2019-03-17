using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(Player))]
public class PlayerNetworkSetup : NetworkBehaviour {
    
    public Behaviour[] componenetsToDisable;//
    string remoteLayerName = "RemotePlayer";
    GameObject sceneCam;  
    public GameObject _projectile;

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

    [Command]
    public void CmdSpawnRocket(Vector3 _spawnPoint, Quaternion _rotation, int _damage)
    {
        Debug.Log("Cmd rot: " + _rotation);
        GameObject clone = Instantiate(_projectile, _spawnPoint, _rotation);
        clone.GetComponent<Projectile>().damage = _damage;
        NetworkServer.Spawn(clone);

        //Projectile newBullet = clone.GetComponent<Projectile>();

        //newBullet.hitRotation = _hitRotation;

        // should this line not just be put on the projectile itself?
        //newBullet.Fire();
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
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
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
