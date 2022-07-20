using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

namespace JZ.AUDIO
{
    /// <summary>
    /// <para>Audio clips that are to be played on their own audio sources</para>
    /// <para>Typically played by sound players</para>
    /// </summary>
    [System.Serializable]
    public class Sound
    {
        #region //Sound properties
        [Header("Sound Properties")]
        [SerializeField] public string title = "";
        [SerializeField] public AudioClip clip = null;
        [SerializeField] private VolumeType volumeType = VolumeType.sfx;
        [HideInInspector] public AudioSource source = null;
        #endregion

        #region //Source properties
        [Header("Source Properties")]
        [Range(0f, 1f)] [SerializeField] private float volume = 1f;
        [Range(0.1f, 3f)] [SerializeField] private float pitch = 1f;
        [SerializeField] private bool isBackwards = false;
        [SerializeField] private bool loop = false;
        [SerializeField] private bool playOnStart = false;
        #endregion


        #region //Constructor
        public Sound(string title, AudioClip clip, VolumeType volumeType)
        {
            this.title = title;
            this.volumeType = volumeType;
            this.clip = clip;
        }
        #endregion

        #region //Getters
        public VolumeType GetVolumeType() { return volumeType; }
        public float GetBaseVolume()
        {
            return volume * JZAudioSettings.GetAdjustedVolume(volumeType);
        }
        #endregion

        #region //Set Up
        public void SetUpSource()
        {
            if(source == null) return;
            SetUpSource(source);
        }
        public void SetUpSource(AudioSource source)
        {
            source.clip = clip;
            source.pitch = pitch;
            source.loop = loop;
            source.playOnAwake = false;
            if(isBackwards)
            {
                source.pitch *= -1;
                source.timeSamples = source.clip.samples - 1;
            }
            
            this.source = source;
            SetSourceVolume();
            if(playOnStart && !source.isPlaying) Play();
        }
        #endregion
    
        #region //Playing and Stopping
        public void Play()
        {
            SetSourceVolume();
            source.Play();
        }

        public void Stop()
        {
            source.Stop();
        }
        #endregion

        #region //Modification
        public void SetSourceVolume()
        {
            source.volume = volume * JZAudioSettings.GetAdjustedVolume(volumeType);
        }

        public void ChangeClip(AudioClip clip)
        {
            if(this.clip == clip)
            {
                Debug.LogWarning("Tried to change to the currently active clip");
                return;
            }

            Stop();
            this.clip = clip;
            source.clip = clip;
        }
        #endregion
    }

    #if UNITY_EDITOR
    [CustomPropertyDrawer(typeof(Sound))]
    public class SoundDrawer : PropertyDrawer
    {
        public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
        {
            if(property.isExpanded)
                return base.GetPropertyHeight(property, label) * 14.5f;
            else
                return base.GetPropertyHeight(property, label);
        }

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            var rect = position;
            rect.position += new Vector2(0, 235);
            rect.height = 20;
            EditorGUI.PropertyField(position, property, label, true);

            if(!property.isExpanded) return;
            if (GUI.Button(rect, "Play"))
            {
                var player = property.serializedObject.targetObject as SoundPlayer;
                player.Play(property.displayName);
            }
        }
    }
    #endif
} 