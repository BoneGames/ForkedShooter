﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;

using BT;
using NaughtyAttributes;


public abstract class Weapon : MonoBehaviour
{
  [BoxGroup("Weapon Stats")]
  public int damage = 100, maxReserves = 30, currentReserves, magSize, currentMag;
  [BoxGroup("Weapon Stats")]
  public float accuracy = 1f, scopeZoom = 75f, reloadSpeed, rateOfFire = 5f;
  [BoxGroup("Weapon Stats")]
  public Elements.Element weaponElement;
  //public float range = 10f
  [BoxGroup("References")]
  public GameObject projectile, muzzle, lineRendPrefab;
  [BoxGroup("References")]
  public Transform spawnPoint, aimPoint;
  [BoxGroup("References")]
  public Text ammoDisplay;
  [BoxGroup("References")]
  public SfxPitchShifter pitchShifter;
  [BoxGroup("References")]
  public AudioSource audioWep;
  [BoxGroup("References")]
  public AudioClip[] sfx;

  [BoxGroup("Events")]
  public UnityEvent onFire;

  public Vector3 hitPoint;

  public LayerMask enemy;
  public float bulletDetectionRadius;

  Quaternion hitRotation;

  public GradientAlphaKey[] startingAlphaKeys;

  public bool canShoot;

  int tempMag;

  public virtual void Start()
  {
    currentReserves = maxReserves;
    currentMag = magSize;
    DefaultReload();
  }

  public abstract void Attack();

  public virtual Quaternion AimAtCrosshair()
  {
    Ray crossHairRay = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
    RaycastHit hit;
    if (Physics.Raycast(crossHairRay, out hit, Mathf.Infinity))
    {
      Vector3 direction = hit.point - spawnPoint.position;
      spawnPoint.rotation = Quaternion.LookRotation(direction);
    }
    else
    {
      spawnPoint.localRotation = Quaternion.Euler(0, 90, 0);
    }
    return spawnPoint.rotation;
  }

  public Quaternion GetTargetNormal()
  {
    RaycastHit hit;
    //Ray ray = Camera.main.ScreenPointToRay(Screen);
    Vector3 screenCentre = new Vector3(Screen.width / 2, Screen.height / 2, 0);
    Ray ray = Camera.main.ScreenPointToRay(screenCentre);

    if (Physics.Raycast(ray, out hit))
    {
      hitPoint = hit.point;
      hitRotation = hit.transform.rotation;
    }
    return hitRotation;
  }

  public virtual void Reload()
  {
    DefaultReload();

    UpdateAmmoDisplay();
  }

  public void OnFire()
  {
    onFire.Invoke();
  }

  public virtual void SpawnMuzzleFlash()
  {
    if (muzzle)
    {
      GameObject _flash = Instantiate(muzzle, spawnPoint.transform);
      _flash.transform.SetParent(null);

      Destroy(_flash, 3);
    }
  }

  public virtual void UpdateAmmoDisplay()
  {
    if (ammoDisplay)
      ammoDisplay.text = string.Format("{0}/{1} // {2}/{3}", currentMag, magSize, currentReserves, maxReserves);
  }
  public IEnumerator ReloadTimed()
  {
    yield return new WaitForSeconds(reloadSpeed);
    DefaultReload();
  }

  void DefaultReload()
  {
    //print(BaneTools.ColorString(gameObject.name + " is reloading!", BaneTools.Color255(0, 255, 0)));
    if (currentReserves > 0)
    {
      if (currentMag >= 0)
      {
        currentReserves += currentMag;
        currentMag = 0;

        if (currentReserves >= magSize)
        {
          currentReserves -= magSize - currentMag;

          currentMag = magSize;
        }
        else if (currentReserves < magSize)
        {
          tempMag = currentReserves;
          currentMag = tempMag;
          currentReserves -= tempMag;
        }
      }
    }

    UpdateAmmoDisplay();
  }


  public void AlertCloseEnemies(Vector3 origin, Vector3 _hitPoint)
  {
    //float distance = Vector3.Distance(origin, _hitPoint);
    //Vector3 dir = (origin - _hitPoint).normalized;
    Vector3 dir = (origin - _hitPoint) * 0.7f;

    Vector3 _newInspectionPoint = origin - dir;


    //GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
    //cube.transform.position = _newInspectionPoint;

    Collider[] cols = Physics.OverlapSphere(transform.position, bulletDetectionRadius, enemy);
    if (cols.Length > 0)
    {
      foreach (Collider col in cols)
      {
        col.GetComponent<BehaviourAI>().BulletAlert(_newInspectionPoint);
      }
    }
  }

}

