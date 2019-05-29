using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class SfxPitchShifter : MonoBehaviour
{
  public Vector2 pitchBounds = new Vector2();
  public Transform tweetSource;

  public AudioSource source;
  public AudioMixer mixer;

  // Use this for initialization
  void Start()
  {
    source = GetComponent<AudioSource>();
  }

  public void Tweet()
  {
    mixer.SetFloat("Pitch", Random.Range(pitchBounds.x, pitchBounds.y));
  }

  AudioClip RandomClip(AudioClip[] _clioArray)
  {
    //Gets random clip and returns it
    return _clioArray[Random.Range(0, _clioArray.Length)];
  }
}
