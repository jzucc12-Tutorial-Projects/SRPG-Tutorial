using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
[CreateAssetMenu(fileName = "New SfxContainer", menuName = "ScriptableObjects/SfxContainer", order = 1)] //Allows you to make instances of this from the create asset menu.
public class SfxContainer : ScriptableObject
{
    [SerializeField]
    private List<AudioClip> _clips;
    public List<AudioClip> clips {
        get {
            return _clips;
        }
        private set {
            _clips = value;
        }
    }
    [SerializeField] [Range(min:0f, max:1f)] 
    private float _volume = 1f;
    public float volume {
        get {
            return _volume;
        }
        private set {
            _volume = value;
        }
    }
    [SerializeField]
    private AudioMixerGroup _audioMixerGroup;
    public AudioMixerGroup audioMixerGroup {
        get {
            return _audioMixerGroup;
        }
        private set {
            _audioMixerGroup = value;
        }
    }
    public int NumberOfClips() => _clips.ToArray().Length;

    public AudioClip GetClip(string clipName)
    {
        foreach(var clip in _clips)
        {
            if(clip.name != clipName) continue;
            return clip;
        }
        Debug.LogError($"Could not find clip '{clipName}' in '{name}'");
        return null;
    }
}