using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//Originally from Brackeys//
namespace JZ.AUDIO
{
    /// <summary>
    /// <para>Manages a group of the Sound class and gives each their own audio source</para>
    /// <para>Serves as the middle-man between sounds and other classes</para>
    /// </summary>
    public class SoundPlayer : MonoBehaviour
    {
        [SerializeField] private List<Sound> mySounds = new List<Sound>();

        #region //Monobehaviour
        private void OnValidate() 
        {
            foreach(Sound sound in mySounds)
                sound.SetUpSource();
        }
        private void Awake()
        {
            foreach(Sound sound in mySounds)
                AddSource(sound);
        }

        private void OnEnable() 
        {
            VolumeSetter.VolumeUpdated += UpdateVolumes;
            JZAudioSettings.VolumeInitialized += UpdateVolumes;
        }

        private void OnDisable() 
        {
            VolumeSetter.VolumeUpdated -= UpdateVolumes;
            JZAudioSettings.VolumeInitialized -= UpdateVolumes;
        }
        #endregion

        #region //Getters
        public Sound GetSound(string soundName)
        {
            Sound sound = mySounds.Find(x => x.title == soundName);

            if(sound == null)
            {
                Debug.LogWarning("Couldn't find clip");
                return null;
            }
            return sound;
        }
        #endregion

        #region //Set Up
        //Public
        public void AddSound(Sound sound)
        {
            mySounds.Add(sound);
            AddSource(sound);
        }

        //Private
        private void AddSource(Sound sound)
        {
            GameObject child = new GameObject();
            child.transform.parent = transform;
            child.name = sound.title + " sound";
            sound.SetUpSource(child.AddComponent<AudioSource>());
        }
        #endregion

        #region //Sound Checking
        public bool IsSoundPlaying(string soundName)
        {
            Sound sound = GetSound(soundName);
            if(sound == null) return false;
            return sound.source.isPlaying;
        }
        
        public bool HasClip(string soundName, AudioClip clip)
        {
            Sound sound = GetSound(soundName);
            if(sound == null) return true;
            return sound.clip == clip;
        }
        #endregion

        #region //Playing and Stopping
        public void PlayLastSound(string soundName)
        {
            transform.parent = null;
            Sound sound = GetSound(soundName);
            if(sound == null) Destroy(gameObject);
            else StartCoroutine(DestroyOnEnd(sound));
        }

        public void Play(string soundName)
        {
            Sound sound = GetSound(soundName);
            if(sound == null) return;
            sound.Play();
        }

        public void Stop(string soundName)
        {
            Sound sound = GetSound(soundName);
            if(sound == null) return;
            sound.Stop();
        }

        public void StopAll()
        {
            foreach(Sound sound in mySounds)
            {
                sound.Stop();
            }
        }

        public void StopAllSFX()
        {
            foreach(Sound sound in mySounds)
            {
                if(sound.GetVolumeType() != VolumeType.sfx) continue;
                sound.Stop();
            }
        }

        public void StopAllMusic()
        {
            foreach(Sound sound in mySounds)
            {
                if(sound.GetVolumeType() != VolumeType.music) continue;
                sound.Stop();
            }
        }
        
        private IEnumerator DestroyOnEnd(Sound lastSound)
        {
            lastSound.Play();
            yield return new WaitUntil(() => !lastSound.source.isPlaying);
            Destroy(gameObject);
        }
        #endregion

        #region //Modification
        public void UpdateVolumes()
        {
            foreach(Sound sound in mySounds)
                sound.SetSourceVolume();
        }
        
        public void ChangeClip(string soundName, AudioClip clip)
        {
            Sound sound = GetSound(soundName);
            sound.ChangeClip(clip);
        }
        #endregion
    }
}