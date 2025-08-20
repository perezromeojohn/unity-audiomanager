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

        // public Sound[] backgroundSound, effectsSound;
        public AudioClip[] backgroundClips, effectClips;
        public AudioSource musicSource;
        public AudioMixer audioMixer;
        public int SFXPoolValue = 10; // amount of sfx that can be played at the same time, we instantiate them at the start of the game
        public AudioMixerGroup musicMixerGroup, sfxMixerGroup;

        // music
        private Coroutine musicFadeCoroutine;

        private List<AudioSource> sfxPool = new List<AudioSource>();

        void Start()
        {
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

        // Music

        public void PlayMusic(string musicName, float musicFadeDuration = 1.5f)
        {
            AudioClip clip = Array.Find(backgroundClips, c => c.name == musicName);

            if (clip == null)
            {
                Debug.LogWarning("Sound: " + musicName + " not found!");
            }
            else
            {
                if (musicFadeCoroutine != null)
                    StopCoroutine(musicFadeCoroutine);

                musicFadeCoroutine = StartCoroutine(CrossfadeMusic(clip, musicFadeDuration));
            }
        }

        private IEnumerator CrossfadeMusic(AudioClip newClip, float duration)
        {
            if (musicSource.clip == newClip)
                yield break;

            float startVolume = musicSource.volume;
            for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
            {
                musicSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
                yield return null;
            }
            musicSource.volume = 0;
            musicSource.clip = newClip;
            musicSource.Play();

            for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
            {
                musicSource.volume = Mathf.Lerp(0, startVolume, t / duration);
                yield return null;
            }
            musicSource.volume = startVolume;
        }

        public void PauseMusic(float fadeDuration = 0.5f)
        {
            if (musicSource.isPlaying)
            {
                if (musicFadeCoroutine != null)
                    StopCoroutine(musicFadeCoroutine);

                musicFadeCoroutine = StartCoroutine(FadeOutAndPause(fadeDuration));
            }
        }

        private IEnumerator FadeOutAndPause(float duration)
        {
            float startVolume = musicSource.volume;
            for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
            {
                musicSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
                yield return null;
            }
            musicSource.volume = 0;
            musicSource.Pause();
        }

        public void StopMusic(float fadeDuration = 0.5f)
        {
            if (musicSource.isPlaying)
            {
                if (musicFadeCoroutine != null)
                    StopCoroutine(musicFadeCoroutine);

                musicFadeCoroutine = StartCoroutine(FadeOutAndStop(fadeDuration));
            }
            else
            {
                musicSource.clip = null;
            }
        }

        private IEnumerator FadeOutAndStop(float duration)
        {
            float startVolume = musicSource.volume;
            for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
            {
                musicSource.volume = Mathf.Lerp(startVolume, 0, t / duration);
                yield return null;
            }
            musicSource.volume = 0;
            musicSource.Stop();
            musicSource.clip = null;
        }

        public void ResumeMusic(float fadeDuration = 0.5f)
        {
            if (musicSource.clip != null)
            {
                if (musicFadeCoroutine != null)
                    StopCoroutine(musicFadeCoroutine);

                musicSource.Play();
                musicFadeCoroutine = StartCoroutine(FadeInMusic(fadeDuration));
            }
        }

        private IEnumerator FadeInMusic(float duration)
        {
            float targetVolume = 1f;
            musicSource.volume = 0;
            for (float t = 0; t < duration; t += Time.unscaledDeltaTime)
            {
                musicSource.volume = Mathf.Lerp(0, targetVolume, t / duration);
                yield return null;
            }
            musicSource.volume = targetVolume;
        }

        public void InstantStopMusic()
        {
            if (musicSource.isPlaying)
            {
                musicSource.Stop();
                musicSource.clip = null;
            }
        }

        public void InstantPauseMusic()
        {
            if (musicSource.isPlaying)
            {
                musicSource.Pause();
            }
        }

        // SFX

        public void PlaySFX(string sfxName)
        {
            AudioClip clip = Array.Find(effectClips, c => c.name == sfxName);

            if (clip == null)
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
                freeSource.clip = clip;
                freeSource.pitch = UnityEngine.Random.Range(.85f, 1.15f);
                freeSource.Play();
                StartCoroutine(ClearClip(freeSource, clip.length));
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