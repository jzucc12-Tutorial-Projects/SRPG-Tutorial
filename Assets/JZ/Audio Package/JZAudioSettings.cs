using System;
using UnityEngine;

namespace JZ.AUDIO
{
    /// <summary>
    /// <para>JZ specific audio settings</para>
    /// </summary>
    public static class JZAudioSettings
    {
        public const string masterVolKey = "Master Volume";
        public const string musicVolKey = "Music Volume";
        public const string sfxVolKey = "SFX Volume";
        private static float[] volumes = { 0.5f, 0.5f, 0.5f };
        private static float[] defaultVolumes = { 0.5f, 0.5f, 0.5f };
        public static event Action VolumeInitialized;


        public static float GetVolume(VolumeType type) { return volumes[(int)type]; }
        public static float GetDeffaultVolume(VolumeType type) { return defaultVolumes[(int)type]; }
        public static void SetVolume(VolumeType type, float newVol) { volumes[(int)type] = newVol; }
        public static float GetAdjustedVolume(VolumeType type) 
        { 
            float master = volumes[(int)VolumeType.master];
            float specific = volumes[(int)type];
            return 2 * master * specific;
        }

        public static void Initialize()
        {
            if(!PlayerPrefs.HasKey(masterVolKey))
            {
                PlayerPrefs.SetFloat(masterVolKey, 0.5f);
                PlayerPrefs.SetFloat(musicVolKey, 0.5f);
                PlayerPrefs.SetFloat(sfxVolKey, 0.5f);
            }

            SetVolume(VolumeType.master, PlayerPrefs.GetFloat(masterVolKey));
            SetVolume(VolumeType.music, PlayerPrefs.GetFloat(musicVolKey));
            SetVolume(VolumeType.sfx, PlayerPrefs.GetFloat(sfxVolKey));
            VolumeInitialized?.Invoke();
        }
    }
}
