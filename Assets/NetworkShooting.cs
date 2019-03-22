// using System.Collections;
// using System.Collections.Generic;
// using UnityEngine;

// public class NetworkShooting : MonoBehaviour {

// 	[SerializeField] private Transform shootPoint;
// 	[SerializeField] private int damage = 5;
// 	void Start () {
		
// 	}
	
// 	// Update is called once per frame
// 	void Update () {
// 		if(Input.GetKeyDown(KeyCode.Space))
// 		{
// 			Fire();
// 		}
// 	}

// 	private void Fire()
// 	{
// 		RaycastHit hit;
// 		if(Physics.Raycast(shootPoint.position, shootPoint.forward, out hit))
// 		{
// 			if(hit.transform.CompareTag("Player"))
// 			{
// 				PhotonView pView = hit.transform.GetComponent<PhotonView>();

// 				if(pView)
// 				{
// 					pView.RPC("ChangeHealth", PhotonTargets.All, damage);
// 				}
// 			}
// 		}
// 	}
// }
