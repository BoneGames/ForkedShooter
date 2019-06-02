using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{
  public static List<TrackClass> trackList = new List<TrackClass>();
  public static SoundManager instance;

  private static float clipLength;
  private static bool keepFadingIn, keepFadingOut;

  void Awake()
  {
    instance = this;
  }

  // Loop through tracks and add new tracks to trackList.
  static public void AddTracks (int numberofTracks, GameObject gameObj)
  {
    if(numberofTracks != 0)
    {
      for (int i = 0; i < numberofTracks; i++)
      {
        TrackClass track = new TrackClass{id = 1, audioSource = gameObj.AddComponent<AudioSource>()};
        trackList.Add(track);
      }
    }
  }

  // Set track output/playback settings.
  static public void TrackSettings(int track, AudioMixer mainMix, string audioGroup, float trackVolume, bool loop = false)
  {
    trackList[track].audioSource.outputAudioMixerGroup = mainMix.FindMatchingGroups(audioGroup)[0];
    trackList[track].trackVolume = trackVolume;
    trackList[track].loop = loop;
  }

  // Play audioSource using audioClip from trackList with trackList's volume. null makes it optional (we don't have to say 'if list is null').
  static public void PlayMusic(int track, AudioClip audioClip = null, List<AudioClip> listAudioClip = null, int min = -2, int max = -2)
  {
    // Play a clip one time, or looped
    if (audioClip != null && listAudioClip == null && trackList[track].audioSource.isPlaying == false)
    {
      trackList[track].audioSource.PlayOneShot(audioClip, trackList[track].trackVolume);

      if(trackList[track].loop)
      {
        // Loop.
        clipLength = audioClip.length;
        LoopCaller(track, clipLength, audioClip, null, min, max);
      }
    }

    // Play from List Randomly, looped or not
    if (audioClip == null && listAudioClip != null && trackList[track].audioSource.isPlaying == false)
    {
      int index = Random.Range(min, max);

      if(index == -1)
      {
        Debug.Log("noSound intentional (index == -1)");
      }
      else
      {
        Debug.Log("Playing: " + listAudioClip[index].name);
        trackList[track].audioSource.PlayOneShot(listAudioClip[index], trackList[track].trackVolume);
        clipLength = listAudioClip[index].length;
      }

      if(trackList[track].loop)
      {
        LoopCaller(track, clipLength, audioClip, listAudioClip, min, max);
      }
    }
  }

  // CALLERS
  public static void FadeInCaller(int track, float speed, float maxVolume)
  {
    instance.StartCoroutine(FadeIn(track, speed, maxVolume));
  }

  public static void FadeOutCaller(int track, float speed)
  {
    instance.StartCoroutine(FadeOut(track, speed));
  }

  public static void LoopCaller(int track, float clipLength, AudioClip audioClip, List<AudioClip> listAudioClip, int min, int max)
  {
    instance.StartCoroutine(Loop(track, clipLength, audioClip, listAudioClip, min, max));
  }

  public static void ChangeMusicCaller(int track, float speed, AudioClip audioClip, List<AudioClip> listAudioClip = null, int min = -2, int max = -2)
  {
    instance.StartCoroutine(ChangeMusic(track, speed, audioClip, listAudioClip, min, max));
  }

  // COROUTINES
  static IEnumerator FadeIn(int track, float speed, float maxVolume)
  {
    keepFadingIn = true;
    keepFadingOut = false;

    trackList[track].audioSource.volume = 0;
    float audioVolume = trackList[track].audioSource.volume;

    while(trackList[track].audioSource.volume < maxVolume && keepFadingIn)
    {
      audioVolume += speed;
      trackList[track].audioSource.volume = audioVolume;
      yield return new WaitForSeconds(0.1f);
    }
  }

  static IEnumerator FadeOut(int track, float speed)
  {
    keepFadingIn = false;
    keepFadingOut = true;

    trackList[track].audioSource.volume = 0;
    float audioVolume = trackList[track].audioSource.volume;

    while (trackList[track].audioSource.volume >= speed && keepFadingOut)
    {
      audioVolume -= speed;
      trackList[track].audioSource.volume = audioVolume;
      yield return new WaitForSeconds(0.1f);
    }
  }

  static IEnumerator Loop (int track, float clipLength, AudioClip audioClip = null, List<AudioClip> listAudioClip = null, int min = -2, int max = -2)
  {
    yield return new WaitForSeconds(clipLength + 0.05f); // 0.05f failsafe (if it's still playing, it won't loop when it's called).
    // yield return new WaitForSeconds(Mathf.Round(clipLength)); // Alternate solution (sometimes exported clip timecodes will be off slightly).

    PlayMusic(track, audioClip, listAudioClip, min, max);
  }

  static IEnumerator ChangeMusic (int track, float speed, AudioClip audioClip = null, List<AudioClip> listAudioClip = null, int min = -2, int max = -2)
  {
    FadeOutCaller(track, speed);
    while(trackList[track].audioSource.volume >= speed)
    {
      yield return new WaitForSeconds(0.01f);
    }

    trackList[track].audioSource.Stop(); // Ensure we stop audioSource when we fade out (or it will still be playing).

    // Play a clip one time, or looped
    if (audioClip != null)
    {
      trackList[track].audioSource.PlayOneShot(audioClip, trackList[track].trackVolume);

      /// if (trackList[track].loop)
      /// {
      ///   // Loop.
      ///   clipLength = audioClip.length;
      ///   LoopCaller(track, clipLength, audioClip, null, min, max);
      /// }
      
        clipLength = audioClip.length;

      FadeInCaller(track, speed, trackList[track].trackVolume);

        LoopCaller(track, clipLength, audioClip, null, -2, -2);
    }

    if (listAudioClip != null)
    {
      int index = Random.Range(min, max);

      if (index == -1)
      {
        Debug.Log("noSound intentional (index == -1)");
      }
      else
      {
        Debug.Log("Playing: " + listAudioClip[index].name);
        trackList[track].audioSource.PlayOneShot(listAudioClip[index], trackList[track].trackVolume);
        clipLength = trackList[track].trackVolume;

        ///   clipLength = listAudioClip[index].length;
        /// }
        /// 
        /// if (trackList[track].loop)
        /// {
        ///   LoopCaller(track, clipLength, audioClip, listAudioClip, min, max);

        FadeInCaller(track, speed, trackList[track].trackVolume);

          LoopCaller(track, clipLength, audioClip, listAudioClip, min, max);
      }
    }
  }
}
