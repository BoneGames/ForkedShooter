using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class FPSSound : MonoBehaviour
{
  public AudioMixer mainMix;
  public GameObject player;
  public GameObject enemy;
  public float calmDistance = 80;
  public float alertDistance = 30;
  public List<AudioClip> track01; // ← Make this NOT a List when done (maybe?).
  public List<AudioClip> track02; // List if you want to use variations of clips. 
  public List<AudioClip> track03; // If using samples, make sure they have the same tempo to loop seamlessly.
  public List<AudioClip> track04; // This whole system is intended for playing different loops (i.e. drum loops, melody variations).

  private bool inCalmZone = true;
  private bool inAlertZone = false;
  private bool inDangerZone = false;

  void Start()
  {
    // Add tracks(audio sources), fade in, and start playing music.
    SoundManager.AddTracks(2, gameObject);
    SoundManager.TrackSettings(0, mainMix, "Track01", 0.5f, true);
    SoundManager.TrackSettings(1, mainMix, "Track02", 0.5f, true);

    SoundManager.PlayMusic(0, track01[0]);
    SoundManager.FadeInCaller (0, 0.01f, SoundManager.trackList[0].trackVolume);

    SoundManager.PlayMusic(1, null, track02, -1, 2); // Some randomness; -1 doesn't play; anything else plays.
    SoundManager.FadeInCaller (1, 0.01f, SoundManager.trackList[1].trackVolume);
  }

  void Update()
  {
    // CalmZone
    if(Vector3.Distance(enemy.transform.position, player.transform.position) > calmDistance && !inCalmZone) // ← SIMPLIFY THIS WHEN DONE; THIS IS AWFUL.
    {
      Debug.Log("CalmZone...");
      inCalmZone = true;
      inAlertZone = false;
      inDangerZone = false;

      SoundManager.ChangeMusicCaller(0, 0.5f, track01[0]);

      SoundManager.ChangeMusicCaller(0, 0.5f, null, track02, 0, 2);
    }

    // AlertZone
    if (Vector3.Distance(enemy.transform.position, player.transform.position) <= calmDistance &&
        Vector3.Distance(enemy.transform.position, player.transform.position) > alertDistance && !inAlertZone)
    {
      Debug.Log("AlertZone...");
      inCalmZone = false;
      inAlertZone = true;
      inDangerZone = false;

      SoundManager.ChangeMusicCaller(0, 0.5f, track01[1]);

      SoundManager.ChangeMusicCaller(0, 0.5f, null, track02, 0, 2);
    }

    // DangerZone
    if (Vector3.Distance(enemy.transform.position, player.transform.position) <= alertDistance && !inDangerZone)
    {
      Debug.Log("DangerZone...");
      inCalmZone = false;
      inAlertZone = false;
      inDangerZone = true;

      SoundManager.ChangeMusicCaller(0, 0.5f, track01[2]);

      SoundManager.ChangeMusicCaller(0, 0.5f,track02[2]);
    }
  }
}
