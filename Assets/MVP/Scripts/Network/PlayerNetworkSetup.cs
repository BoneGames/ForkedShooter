using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerNetworkSetup : Photon.PunBehaviour {

	private Camera attachedCamera;
    private Rigidbody rigid;
	
	private void Awake()
	{
        GetReferences();
		Initialise();
	}

    private void GetReferences()
    {
        attachedCamera = GetComponentInChildren<Camera>();
        rigid = GetComponent<Rigidbody>();
    }
    private void Initialise()
	{
		if(!photonView.isMine)
		{
			// disable other player's FP cam
			attachedCamera.enabled = false;
            rigid.isKinematic = true;

            // Note (Manny): You don't have to do this.
            //// disable other player's control scripts
            //foreach(MonoBehaviour m in playerControlScripts)
            //{
            //	m.enabled = false;
            //}
        }
	}
}
