using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

[RequireComponent(typeof(AudioSource))]
public class AudioPlayable : MonoBehaviour
{
    public void Setup(AudioClip clip, Vector3 pos, float volume, AudioMixerGroup group){
        transform.position = pos;
        GetComponent<AudioSource>().outputAudioMixerGroup = group;
        GetComponent<AudioSource>().PlayOneShot(clip, volume);
        Destroy(gameObject, clip.length + 0.1f);
    }
}
