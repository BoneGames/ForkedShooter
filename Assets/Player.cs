using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

	[SerializeField]
	private int maxHealth = 100;
	[SyncVar]
	public int currentHealth;

	void Start()
	{
		SetDefaults();
	}

	public void TakeDamage(int _damageAmount)
	{
		Debug.Log("Damage: " + _damageAmount);
		currentHealth -= _damageAmount;

		Debug.Log(transform.name + " now has " + currentHealth + " health.");
	}

	public void SetDefaults()
	{
		currentHealth = maxHealth;
	}
}
