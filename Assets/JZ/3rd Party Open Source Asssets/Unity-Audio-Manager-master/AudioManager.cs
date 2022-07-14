using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AudioManager {
    private static AudioManager _instance;

    public static AudioManager Instance () {
        if (_instance == null) {
            _instance = new AudioManager();
        }
        return _instance;
    }

    //[SerializeField]
    public static SfxContainer musicTracks; //The main music tracks to be played.

    private void Awake() 
    {
        if (_instance != null && _instance != this) {
            Debug.Log("Two Singletons active. May cause issues.");
        }
    }

    /// <summary>
    /// Play a clip from an audio container. Leave clip no at -1 to choose a random clip
    /// </summary>
    /// <param name="container"></param>
    /// <param name="playPosition"></param>
    /// <param name="clipNo"></param>
    public void PlayOneShot(SfxContainer container, Vector3 playPosition, int clipNo = -1) 
    { 
        if(!CheckValid(container)) return;
        if(clipNo == -1) clipNo = Random.Range(0, container.NumberOfClips());
        if (clipNo < 0 || clipNo >= container.NumberOfClips()) {
            Debug.LogError($"'{container.name}' tried to access index {clipNo} when there are only {container.NumberOfClips()} clips.");
            return;
        }
        GameObject obj;
        obj = new GameObject("Audio Instance");
        obj.AddComponent<AudioPlayable>();
        obj.GetComponent<AudioPlayable>().Setup(container.clips[clipNo], playPosition, container.volume, container.audioMixerGroup);
    }

    /// <summary>
    /// Play a clip from an audio container.
    /// </summary>
    /// <param name="container"></param>
    /// <param name="playPosition"></param>
    /// <param name="clipNo"></param>
    public void PlayOneShot(SfxContainer container, Vector3 playPosition, string clipName) 
    { 
        if(!CheckValid(container)) return;
        var clip = container.GetClip(clipName);
        if(clip == null) return;
        GameObject obj;
        obj = new GameObject("Audio Instance");
        obj.AddComponent<AudioPlayable>();
        obj.GetComponent<AudioPlayable>().Setup(clip, playPosition, container.volume, container.audioMixerGroup);
    }

    private bool CheckValid(SfxContainer container)
    {
        if (container == null) {
            Debug.LogError("Sfx Container is nonexistent or null.");
            return false;
        }
        if (container.NumberOfClips() == 0) {
            Debug.LogError($"'{container.name}' Sfx Container is empty.");
            return false;
        }
        return true;
    }
}
