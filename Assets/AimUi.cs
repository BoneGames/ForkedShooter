using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AimUi : MonoBehaviour
{
    public GameObject[] cHairs = new GameObject[4];
    public Vector3[] directions = new Vector3[4];
    public float crossHairSpreadAmount;
    public Image circleSight;
    public float recoilHeight;

    void Start()
    {
        for (int i = 0; i < cHairs.Length; i++)
        {
            cHairs[i].transform.position += (directions[i] * 6);
        }
    }

    public void SwitchSight(bool crosshairActive, bool rocket)
    {
        foreach (GameObject crosshair in cHairs)
        {
            if (rocket)
            {
                crosshair.SetActive(!crosshairActive);
            }
            else
            {
                crosshair.SetActive(crosshairActive);
            }
        }
       
        circleSight.enabled = !crosshairActive;

        
    }

    public void EnableSight(bool enabled)
    {

        foreach (GameObject crosshair in cHairs)
        {
            crosshair.SetActive(enabled);
        }
    }

    public void MoveHairs(float recoil, float recoilMulti, Weapon accuracyRef)
    {
        for (int i = 0; i < cHairs.Length; i++)
        {
            cHairs[i].transform.position += (directions[i] * recoil * crossHairSpreadAmount);
        }
        StopAllCoroutines();
        for (int i = 0; i < cHairs.Length; i++)
        {
            StartCoroutine(CorrectCrossHairs(cHairs[i].transform, directions[i], recoilMulti, accuracyRef));
        }
    }

    IEnumerator CorrectCrossHairs(Transform crossHair, Vector3 direction, float recoilMulti, Weapon accuracyRef)
    {
        yield return new WaitForEndOfFrame();

        Vector3 start = crossHair.position;
        Vector3 finish = new Vector3(Screen.width / 2, Screen.height / 2, 0) + (direction * 6);

        float accStart = accuracyRef.accuracy;
        float accFinish = accuracyRef.baseAccuracy;

        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * recoilMulti;
            // move crosshairs
            crossHair.position = Vector3.Lerp(start, finish, timer);
            // weapon change accuracy
            recoilHeight = crossHair.position.y - direction.y;
            accuracyRef.accuracy = Mathf.Lerp(accStart, accFinish, timer);
            yield return null;
        }
    }
}
