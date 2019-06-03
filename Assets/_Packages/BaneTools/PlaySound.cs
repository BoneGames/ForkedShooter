using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

using NaughtyAttributes;

public class PlaySound : MonoBehaviour
{
  public AudioClip[] clips;
  public AudioSource source;

  [Tooltip("Can be null, however if you have no defined Audio Source, this should be filled.")]
  public GameObject temporarySource;
  public AudioMixerGroup mixer;

  public void PlaySoundFromArrayAtPoint()
  {
    CheckSources();

    AudioClip clip = clips[Random.Range(0, clips.Length - 1)];
    AudioSource.PlayClipAtPoint(clip, transform.position);
  }

  public void PlaySoundFromArrayAtSource()
  {
    CheckSources();

    AudioClip clip = clips[Random.Range(0, clips.Length - 1)];
    source.PlayOneShot(clip);
  }

  void CheckSources()
  {
    if (source == null)
    {
      if (temporarySource)
      {
        Instantiate(temporarySource, transform);
        temporarySource.GetComponent<AudioSource>().outputAudioMixerGroup = mixer;
        source = temporarySource.GetComponent<AudioSource>();
      }
      else
      {
        print("Temporary Source not found. Did you forget to add it in the Inspector?");
      }
    }
  }
}