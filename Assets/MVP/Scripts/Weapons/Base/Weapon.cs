using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using System.Linq;
using BT;
using NaughtyAttributes;
using System;


public abstract class Weapon : MonoBehaviour
{
    public bool ShowWeaponStats;
    [BoxGroup("Weapon Stats"), ShowIf("ShowWeaponStats")]
    public int damage = 100, maxReserves = 30, currentReserves, magSize, currentMag;
    [BoxGroup("Weapon Stats"), Slider(0, 10), ShowIf("ShowWeaponStats")] public float accuracy = 1f, loudness, bulletDetectionRadius, recoil, recoilRecoverMulti;
    [HideInInspector] public float baseAccuracy;
    [BoxGroup("Weapon Stats"), ShowIf("ShowWeaponStats")] public float scopeZoom = 75f, aimSpeed = 5f, reloadSpeed, rateOfFire = 5f;
    [BoxGroup("Weapon Stats"), HideInInspector, ShowIf("ShowWeaponStats")] public float startScopeZoom = 75f;
    [BoxGroup("Weapon Stats"), ShowIf("ShowWeaponStats")]
    public Elements.Element weaponElement;
    [BoxGroup("Weapon Stats"), ShowIf("ShowWeaponStats")]
    public AmmoType.AmmoTypes ammoType;
    //public float range = 10f
    public bool ShowReferences;
    [BoxGroup("References"), ShowIf("ShowReferences")]
    public GameObject projectile, muzzle, lineRendPrefab;
    [BoxGroup("References"), ShowIf("ShowReferences")]
    public Transform shootPoint, aimShootPos, hipShootPos;
    [BoxGroup("References"), ShowIf("ShowReferences")]
    public UIHandler UI;
    [BoxGroup("References"), ShowIf("ShowReferences")]
    public SfxPitchShifter pitchShifter;
    [BoxGroup("References"), ShowIf("ShowReferences")]
    public AudioSource audioWep;
    [BoxGroup("References"), ShowIf("ShowReferences")]
    public AudioClip[] sfx;
    [BoxGroup("References"), ShowIf("ShowReferences")]
    public Transform hand;

    public bool ShowEvents;
    [BoxGroup("Events"), ShowIf("ShowEvents")]
    public UnityEvent onFire;

    public bool ShowElementColors;
    [BoxGroup("Element Colours")]
    [ShowIf("ShowElementColors")]
    public Color normal, fire, water, grass;

    float startingAccuracy;
    public LayerMask enemy;
    Quaternion hitRotation;
    [HideInInspector] public GradientAlphaKey[] startingAlphaKeys;

    public bool ShowShootRules;
    [BoxGroup("Shoot Rules"), ShowIf("ShowShootRules")]
    public float attackTimer;
    [BoxGroup("Shoot Rules"), ShowIf("ShowShootRules")]
    public bool canShoot, isEquipped;
    public bool autoReload;
    public bool enemiesHearBullets;

    public bool ShowUniqueStats;
    [BoxGroup("Unique Stats"), ShowIf("ShowUniqueStats")]
    public bool spawnWithUniqueStats;
    [BoxGroup("Unique Stats"), ShowIf("ShowUniqueStats")]
    public UniqueWeaponStats uniqueStats;
    [BoxGroup("Unique Stats"), ShowIf("ShowUniqueStats"), Slider(0.1f, 0.9f)]
    public float statVariation = 0.2f;
    public int indexID;

    int tempMag;
    Quaternion camStartRotation;

    [HideInInspector] public InsideCollider internalCheck;

    public virtual void Awake()
    {
        if (spawnWithUniqueStats)
        {
            if (uniqueStats == null)
            {
                UniqueWeaponStats setup = ScriptableObject.CreateInstance<UniqueWeaponStats>();
                setup.Init(statVariation);
                uniqueStats = setup;
                ApplyUniqueWeaponStats(uniqueStats);
            }
        }
        internalCheck = shootPoint.GetComponent<InsideCollider>();
        baseAccuracy = accuracy;
        camStartRotation = Camera.main.transform.localRotation;
        currentReserves = maxReserves;
        currentMag = magSize;
        startScopeZoom = Camera.main.fieldOfView;
        Debug.Log(Camera.main.fieldOfView);
        DefaultReload();

        startingAccuracy = accuracy;
    }

    public void ApplyUniqueWeaponStats(UniqueWeaponStats _uniqueStats)
    {
        // Get Array of all variable names in class
        var weaponVariableNames = this.GetType()
                     .GetFields()
                     .Select(field => field.Name);

        // Get Array of all stat multipliers in uniqueStats (paramater)
        var uniqueVariableNames = _uniqueStats.GetType()
                     .GetFields()
                     .Select(field => field.Name);
        _uniqueStats.baseStats.Clear();
        // Match variav\ble pairs with same name
        foreach (var multi in uniqueVariableNames)
        {
            if (weaponVariableNames.Contains(multi))
            {
                // Get field on weapon(as FieldInfo)
                var statField = this.GetType().GetField(multi);
                // Get matching field on UniqueWeaponStats class
                var statMultiplier = _uniqueStats.GetType().GetField(multi);

                try
                {
                    // Collect weapon's base stats to reapply to it later (if weapon dropped)
                    _uniqueStats.baseStats.Add(multi, Convert.ToSingle(statField.GetValue(this)));
                }
                catch
                {
                    Debug.Log("catch: " + multi);
                    _uniqueStats.baseStats.Add(multi, 0);
                }


                // if statfield is an int - cast it as such when applying value
                if (statField.GetValue(this) is int)
                {
                    int newValue = Mathf.RoundToInt(((int)statField.GetValue(this) * (float)statMultiplier.GetValue(_uniqueStats)));
                    statField.SetValue(this, newValue);
                }
                else if (statField.GetValue(this) is float)// if statfield is an float - cast it as such when applying value
                {
                    float newValue = (float)statField.GetValue(this) * (float)statMultiplier.GetValue(_uniqueStats);
                    newValue = (float)Math.Round((double)newValue, 2);
                    statField.SetValue(this, newValue);
                }
                else
                {
                    Debug.Log("SETTING ENUM");
                    Elements.Element newValue = (Elements.Element)statMultiplier.GetValue(this);
                    statField.SetValue(this, newValue);
                }
            }
            else
            {
                //Debug.Log("UniqueWeaponStats variable: " + multi + ", does not have a counterpart to mutate in weapon script");
            }
        }
        uniqueStats = _uniqueStats;
    }

    public Quaternion GetRecoil()
    {
        float jump = UnityEngine.Random.Range(2, recoil * 4);
        float swing = UnityEngine.Random.Range(-recoil/4, recoil/4);
        Quaternion recoilAmount = Quaternion.Euler(-jump, swing, 0);
        return recoilAmount;
    }

    public void ResetBaseWeaponStats(Dictionary<string, float> baseValues)
    {
        // Get Array of all variable names in class
        var weaponVariableNames = this.GetType()
                     .GetFields()
                     .Select(field => field.Name);

        foreach (KeyValuePair<string, float> field in baseValues)
        {
            // check if all weapon variable names have a match for current reset Value name
            if (weaponVariableNames.Contains(field.Key))
            {
                // Get field on weapon(as FieldInfo)
                var statField = this.GetType().GetField(field.Key);

                try // apply base value as float
                {
                    statField.SetValue(this, field.Value);
                }
                catch // apply base value as int
                {
                    statField.SetValue(this, (int)field.Value);
                }
            }
        }
    }

    //public abstract void AiShoot(int _shots);
    public virtual void Attack()
    {
        if (currentMag > 0)
        {
            // reset shot rate controller
            attackTimer = 0;
            canShoot = false;
            // start aim from centre of crosshair (not directly forward due to holding offset)
            shootPoint.transform.rotation = AimAtCrosshair();
            // muzzle flash & sound FX
            onFire.Invoke();

            bool insideMesh = internalCheck.InsideMesh(Camera.main.transform, shootPoint);
            // if spawnPoint is inside mesh
           

            // raycast with (in)accuracy
            Ray ray = new Ray(shootPoint.position, shootPoint.transform.forward + AccuracyOffset(accuracy));
            if (Physics.Raycast(ray, out RaycastHit hit))
            {
                if(!insideMesh)
                BulletTrail(hit.point, hit.distance, weaponElement);

                if(enemiesHearBullets)
                {
                    BulletAlert(transform.position, hit.point, loudness);
                }

                if (GameManager.isOnline)
                {
                    if (hit.collider.CompareTag("Enemy"))
                    {
                        hit.transform.GetComponent<PhotonView>().RPC("ChangeHealth", PhotonTargets.All, damage);
                    }
                }
                else
                {
                    if (hit.collider.tag == "Enemy")
                    {
                        // disable drone light on impact (also reduces drone look length)
                        if (hit.collider.GetComponent<AI_FoV_SearchLight>())
                        {
                            Debug.Log("hit Drone light - it should be off now");
                            hit.collider.GetComponent<AI_FoV_SearchLight>().fovLight.enabled = false;
                            hit.collider.enabled = false;
                            hit.collider.GetComponent<AI_FoV_SearchLight>().viewRadius = 10;
                        }
                        // Deal Damage
                        hit.transform.GetComponent<Health>().ChangeHealth(damage, transform.position, weaponElement);
                        print("I hit an enemy");
                    }

                }
            }
            else
            {
                // bullet trail into sky
                if(!insideMesh)
                BulletTrail(shootPoint.transform.position + (shootPoint.transform.forward + AccuracyOffset(accuracy)) * 200, 200, weaponElement);
            }
            RecoilMethod();
            // Reduce ammo
            currentMag--;
            UpdateAmmoDisplay();
        }
        if (currentMag <= 0 && autoReload)
        {
            StartCoroutine(ReloadTimed());
        }
    }

    public virtual Quaternion AimAtCrosshair()
    {
        Ray crossHairRay = Camera.main.ScreenPointToRay(new Vector2(Screen.width / 2, Screen.height / 2));
        RaycastHit hit;
        if (Physics.Raycast(crossHairRay, out hit, Mathf.Infinity))
        {
            Vector3 direction = hit.point - shootPoint.position;
            shootPoint.rotation = Quaternion.LookRotation(direction);
        }
        else
        {
            shootPoint.localRotation = Quaternion.Euler(0, 90, 0);
        }
        return shootPoint.rotation;
    }

    public virtual void OnAim(bool _aiming)
    {
        if (_aiming)
        {
            accuracy = startingAccuracy * 1.5f;
        }
        else
        {
            accuracy = startingAccuracy;
        }
    }

    public virtual Vector3 AccuracyOffset(float _accuracy)
    {
        if (UI.aimUi.recoilHeight != 0)
            accuracy *= UI.aimUi.recoilHeight / 200;
        // increase in order to keep slider friendly: 0-10 range
        _accuracy *= 10;
        // generate random values within accuracy range
        float x = UnityEngine.Random.Range(-(1 / _accuracy), 1 / _accuracy);
        float y = UnityEngine.Random.Range(-(1 / _accuracy), 1 / _accuracy);
        float z = UnityEngine.Random.Range(-(1 / _accuracy), 1 / _accuracy);

        // return offset modifer
        return new Vector3(x, y, z);
    }

    //public bool canShoot;
    public void Update()
    {
        if (canShoot == false)
        {
            attackTimer += Time.deltaTime;

            if (attackTimer >= 1f / rateOfFire)
            {
                // Can shoot!
                canShoot = true;
            }
        }
    }

    public void RecoilMethod()
    {
        StopAllCoroutines();
        StartCoroutine(Recoil());
        UI.aimUi.MoveHairs(recoil, recoilRecoverMulti, this);
    }

    IEnumerator Recoil()
    {
        yield return new WaitForEndOfFrame();
        Quaternion finish = Camera.main.transform.localRotation;
        Camera.main.transform.localRotation *= GetRecoil();

        Quaternion start = Camera.main.transform.localRotation;
       
        float timer = 0;

        while (timer < 1)
        {
            timer += Time.deltaTime * recoilRecoverMulti;
            Camera.main.transform.localRotation = Quaternion.Slerp(start, finish, timer);
            yield return null;
        }
    }
    public Quaternion GetTargetNormal()
    {
        //Ray ray = Camera.main.ScreenPointToRay(Screen);
        Vector3 screenCentre = new Vector3(Screen.width / 2, Screen.height / 2, 0);
        Ray ray = Camera.main.ScreenPointToRay(screenCentre);

        if (Physics.Raycast(ray, out RaycastHit hit))
        {
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
            GameObject _flash = Instantiate(muzzle, shootPoint.transform);
            _flash.transform.SetParent(null);

            Destroy(_flash, 3);
        }
    }

    public virtual void UpdateAmmoDisplay()
    {
        if (UI)
            UI.UpdateAmmoDisplay(currentMag, magSize, currentReserves, maxReserves);
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

    public void BulletAlert(Vector3 origin, Vector3 _hitPoint, float loudness)
    {
        // point 70% of the way from bit point to origin along trajectory
        Vector3 dir = (origin - _hitPoint) * 0.7f;
        Vector3 _newInspectionPoint = origin - dir;

        // Find enemies within radius of bullet impact
        Collider[] impactSearch = Physics.OverlapSphere(_hitPoint, bulletDetectionRadius, enemy);
        AlertEnemies(impactSearch, _newInspectionPoint);

        // Find enemies withing radius of fire point
        Collider[] fireSearch = Physics.OverlapSphere(origin, loudness * 2, enemy);
        AlertEnemies(fireSearch, origin);
    }

    void AlertEnemies(Collider[] enemies, Vector3 inspectionPoint)
    {
        if (enemies.Length > 0)
        {
            foreach (Collider col in enemies)
            {
                if (col.GetComponent<BehaviourAI>())
                {
                    // send alert to enemy
                    col.GetComponent<BehaviourAI>().BulletAlert(inspectionPoint);
                }
            }
        }
    }

    public Color GetTrailColorBasedOn(Elements.Element ammoType)
    {
        // set shotDirArm color
        switch (ammoType)
        {
            case Elements.Element.Normal:
                Debug.Log("Element: " + ammoType);
                return normal;
            case Elements.Element.Fire:
                Debug.Log("Element: " + ammoType);
                return fire;
            case Elements.Element.Water:
                Debug.Log("Element: " + ammoType);
                return water;
            case Elements.Element.Grass:
                Debug.Log("Element: " + ammoType);
                return grass;
            default:
                Debug.Log("You Need to add a new Color/Element to Weapon to asign to bullet trail color");
                return normal;
        }

    }

    public void BulletTrail(Vector3 target, float distance, Elements.Element bulletType)
    {
        Debug.Log("b trail");
        GameObject bulletPath = Instantiate(lineRendPrefab, shootPoint.position, shootPoint.rotation);
        bulletPath.transform.SetParent(shootPoint);
        bulletPath.GetComponent<LineRenderer>().materials[0].SetColor("_TintColor", GetTrailColorBasedOn(bulletType));
        BulletPath script = bulletPath.GetComponent<BulletPath>();
        script.target = target;
        script.distance = distance;
    }
}

