using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

namespace RumyooAudioManager
{
    public class AudioManager : MonoBehaviour
    {
        private static AudioManager _instance;
        public static AudioManager Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = FindObjectOfType<AudioManager>();
                    if (_instance == null)
                    {
                        GameObject go = new GameObject("AudioManager");
                        _instance = go.AddComponent<AudioManager>();
                    }
                }
                return _instance;
            }
        }

        public Sound[] backgroundSound, effectsSound;
        public AudioSource musicSource;
        public AudioMixer audioMixer;
        public int SFXPoolValue = 10; // amount of sfx that can be played at the same time, we instantiate them at the start of the game
        public AudioMixerGroup musicMixerGroup, sfxMixerGroup;

        private List<AudioSource> sfxPool = new List<AudioSource>();

        void Start()
        {
            PlayMusic("SlimeBG");

            SetMusicVolume(PlayerPrefs.GetFloat("MusicVolume", 1));
            SetSFXVolume(PlayerPrefs.GetFloat("SFXVolume", 1));
            MuteMusic(PlayerPrefs.GetInt("MusicMuted", 0) == 1);
            MuteSFX(PlayerPrefs.GetInt("SFXMuted", 0) == 1);
        }

        void Awake()
        {
            if (_instance != null && _instance != this)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
            DontDestroyOnLoad(gameObject);

            for (int i = 0; i < SFXPoolValue; i++)
            {
                GameObject go = new GameObject("SFXPlayer_" + i);
                go.transform.SetParent(transform);
                AudioSource src = go.AddComponent<AudioSource>();
                src.outputAudioMixerGroup = sfxMixerGroup;
                sfxPool.Add(src);
            }
        }

        public void PlayMusic(string musicName)
        {
            Sound s = Array.Find(backgroundSound, sound => sound.name == musicName);

            if (s == null)
            {
                Debug.LogWarning("Sound: " + musicName + " not found!");
            }
            else
            {
                musicSource.clip = s.clip;
                musicSource.Play();
            }
        }

        public void PlaySFX(string sfxName)
        {
            Sound s = Array.Find(effectsSound, sound => sound.name == sfxName);

            if (s == null)
            {
                Debug.LogWarning("Sound: " + sfxName + " not found!");
            }
            else
            {
                AudioSource freeSource = sfxPool.Find(src => !src.isPlaying);
                if (freeSource == null)
                {
                    freeSource = sfxPool[0];
                }
                freeSource.clip = s.clip;
                freeSource.pitch = UnityEngine.Random.Range(.85f, 1.15f);
                freeSource.Play();
                StartCoroutine(ClearClip(freeSource, s.clip.length));
            }
        }

        private IEnumerator ClearClip(AudioSource source, float duration)
        {
            yield return new WaitForSeconds(duration);
            source.clip = null;
        }

        // SFX stuff
        public void SetMusicVolume(float volume)
        {
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }

        public void SetSFXVolume(float volume)
        {
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("SFXVolume", volume);
        }

        public void MuteMusic(bool isMuted)
        {
            audioMixer.SetFloat("MusicVolume", isMuted ? -80 : Mathf.Log10(PlayerPrefs.GetFloat("MusicVolume", 1)) * 20);
            PlayerPrefs.SetInt("MusicMuted", isMuted ? 1 : 0);
        }

        public void MuteSFX(bool isMuted)
        {
            audioMixer.SetFloat("SFXVolume", isMuted ? -80 : Mathf.Log10(PlayerPrefs.GetFloat("SFXVolume", 1)) * 20);
            PlayerPrefs.SetInt("SFXMuted", isMuted ? 1 : 0);
        }
    }
}