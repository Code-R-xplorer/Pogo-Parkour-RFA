using System;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;
using Random = UnityEngine.Random;

namespace Managers
{
    public class AudioManager : MonoBehaviour
    {
        public Sound[] sounds;
        public static AudioManager Instance { get; private set; }

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
                return;
            }
            DontDestroyOnLoad(gameObject);
            foreach (Sound s in sounds)
            {
                s.source = gameObject.AddComponent<AudioSource>();
                s.source.clip = s.clip;
                s.source.volume = s.volume;
                s.source.pitch = s.pitch;
                s.source.loop = s.loop;
                s.source.outputAudioMixerGroup = s.audioMixerGroup;
            }

            SceneManager.sceneUnloaded += _ => { StopAllAudio(); };
        }

        private void Start()
        {
            Play("music");
        }

        public void Play(string name)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            if (s == null)
            {
                return;
            }
            s.source.Play();
        }
        public void Stop(string name)
        {
            Sound s = Array.Find(sounds, sound => sound.name == name);
            if (s == null)
            {
                return;
            }
            s.source.Stop();
        }
        public void PlaySoundWithRandomPitch(string name, float lowPitch, float highPitch)
        {   
            Sound sound = Array.Find(sounds, s => s.name == name);
            if (sound == null)
            {
                Debug.LogWarning("Sound: " + name + " not found!");
                return;
            }

            float pitch = Random.Range(lowPitch, highPitch);

            sound.source.pitch = pitch; // Set the pitch before playing
            sound.source.Play();
        }

        public void PlayRandomSound(string[] names)
        {
            int index = Random.Range(0, names.Length);
            Play(names[index]);
        }
        
        public void PlayRandomSound(string[] names, float lowPitch, float highPitch)
        {
            int index = Random.Range(0, names.Length);
            PlaySoundWithRandomPitch(names[index], lowPitch, highPitch);
        }

        public void StopAllAudio()
        {
            foreach (var sound in sounds)
            {
                if(sound.doNotStop) continue;
                sound.source.Stop();
            }
        }
    }
    
    [Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
        [Range(0f, 1f)]
        public float volume;
        [Range(0.1f, 3f)]
        public float pitch;
        public bool loop;
        public AudioMixerGroup audioMixerGroup;
        public bool doNotStop;

        [HideInInspector]
        public AudioSource source;
    }
}
