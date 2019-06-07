using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ShotDirection : UIHandler
{
    Image shotDirImage;
    public float shotIndicatorDelay;
    Transform playerPos;
    public override void Awake()
    {
        shotDirImage = GetComponentInChildren<Image>();
        shotDirImage.enabled = false;
        playerPos = FindObjectOfType<RigidCharacterMovement>().transform;
    }

    public void ShotIndicator(Vector3 incoming, Elements.Element ammoType)
    {
        StopAllCoroutines();
        StartCoroutine(ShotDirectionActive(incoming, ammoType));
    }

    IEnumerator ShotDirectionActive(Vector3 incoming, Elements.Element ammoType)
    {
        //Set shotDirArm color
        switch (ammoType)
        {
            case Elements.Element.Normal:
                shotDirImage.color = normal;
                Debug.Log("normal");
                break;
            case Elements.Element.Fire:
                shotDirImage.color = fire;
                Debug.Log("fire");
                break;
            case Elements.Element.Water:
                shotDirImage.color = water;
                Debug.Log("water");
                break;
            case Elements.Element.Grass:
                shotDirImage.color = grass;
                Debug.Log("grass");
                break;
            default:
                Debug.Log("You Need to assign a new Color/Element to ShotDirection");
                break;
        }
        shotDirImage.enabled = true;
        float timer = 0;
        while (timer < shotIndicatorDelay)
        {
            timer += Time.deltaTime;
            // Angle between other pos vs player
            Vector3 incomingDir = (playerPos.position - incoming).normalized;
            // Flatten to plane
            Vector3 otherDir = new Vector3(-incomingDir.x, 0f, -incomingDir.z);
            var playerFwd = Vector3.ProjectOnPlane(playerPos.forward, Vector3.up);
            // Direction between player fwd and incoming object
            var angle = Vector3.SignedAngle(playerFwd, otherDir, Vector3.up);
            transform.rotation = Quaternion.Euler(0, 0, -angle);
            yield return null;
        }
        shotDirImage.enabled = false;
    }
}
