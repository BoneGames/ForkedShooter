using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using NaughtyAttributes;

public class EnemyHealth : Health
{
    // Set damage immunity on/off (handled from InvulTotem.cs).
    public bool ShowStates;
    [ShowIf("ShowStates")]
    [BoxGroup("Enemy States")]
    public bool isGod = false;
    public Transform viewPoint;

    [HideInInspector] public PhotonView photonView;
    [HideInInspector] BehaviourAI ai;

    void Awake()
    {
        photonView = GetComponent<PhotonView>();
    }

    public override void Start()
    {
        base.Start();
        UI.SpawnEnemyHealthBar(transform, viewPoint);

    }

    public void RegisterHealthBarEventDelegate(EnemyUIHealthBar enemyBar)
    {
        updateHealthBarEvent.AddListener(enemyBar.UpdateHealthBar);
    }

    // Takes damage from various bullet/projectile scripts and runs 'CheckDie()'.
    [PunRPC]
    public override void ChangeHealth(float _value, Vector3 shotOrigin, Elements.Element ammoType)
    {
        // tell AI it's been shot
        ai.BulletAlert(shotOrigin);

        if (!isGod)
        {
            if (currentShield > 0)
            {
                shield.gameObject.SetActive(true);

                _value = CheckWeakness(_value, ammoType);

                if (currentShield > _value)
                {
                    currentShield -= _value;
                }
                else if (_value >= currentShield)
                {
                    carryOnDmg = _value - currentShield;
                    currentShield -= _value;
                    currentHealth -= carryOnDmg;

                    currentShield = currentShield < 0 ? 0 : currentShield;

                    CheckDie();
                }
            }
            else if (currentShield <= 0)
            {
                currentShield = 0;
                shield.gameObject.SetActive(false);

                if (currentHealth > 0)
                {
                    currentHealth -= _value;

                    if (currentHealth > maxHealth)
                    {
                        currentHealth = maxHealth;
                    }

                    CheckDie();
                }
            }
        }
    }


    // Self explanatory.
    public override void CheckDie()
    {
        updateHealthBarEvent.Invoke(currentHealth, maxHealth);
        

        if (currentHealth <= 0)
        {
            base.CheckDie();
            Destroy(gameObject);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        //Send health data to network
        if (stream.isWriting)
        {
            stream.SendNext(currentHealth);
            //stream.SendNext()
        }
        // recieve health data from network (other player)
        else if (stream.isReading)
        {
            currentHealth = (int)stream.ReceiveNext();
        }
    }
}
