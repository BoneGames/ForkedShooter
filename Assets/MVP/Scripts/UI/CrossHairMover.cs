using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossHairMover : MonoBehaviour
{
    public GameObject[] cHairs = new GameObject[4];
    public Vector3[] directions = new Vector3[4];
    public float crossHairSpreadAmount;
    void Start()
    {
        for (int i = 0; i < cHairs.Length; i++)
        {
            cHairs[i].transform.position += (directions[i] * 6);
        }
    }

    public void MoveHairs(float recoil, float recoilMulti)
    {
        for (int i = 0; i < cHairs.Length; i++)
        {
            cHairs[i].transform.position += (directions[i] * recoil * crossHairSpreadAmount);
        }
        StopAllCoroutines();
        for (int i = 0; i < cHairs.Length; i++)
        {
            StartCoroutine(CorrectCrossHairs(cHairs[i].transform, directions[i], recoilMulti));
        }
    }

    IEnumerator CorrectCrossHairs(Transform crossHair, Vector3 direction, float recoilMulti)
    {
        yield return new WaitForEndOfFrame();
       
        Vector3 start = crossHair.position;
        Vector3 finish = new Vector3(Screen.width / 2, Screen.height / 2, 0) + (direction * 6);

        float timer = 0;
        while (timer < 1)
        {
            timer += Time.deltaTime * recoilMulti;
            crossHair.position = Vector3.Lerp(start, finish, timer);
            yield return null;
        }
    }
}
