using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;
using System;

namespace RumyooAudioManager
{
    [System.Serializable]
    public class AudioEntry
    {
        public string name;                          // lookup key: PlayMusic("name") / PlaySFX("name")
        [ContextMenuItem("Preview", "PreviewClip")]
        public AudioClip clip;
        public bool loop;                            // BGM: true, SFX: false
        [Range(0f, 1f)] public float volume = 1f;    // per-track gain
        [Range(0.1f, 3f)] public float pitch = 1f;   // base pitch
        [Range(0f, 1f)] public float pitchVariation = 0f; // random +/- range applied to pitch (SFX)
        [Range(1, 32)] public int maxConcurrent = 10; // SFX: max simultaneous plays; oldest is cut when exceeded

        public void PreviewClip() => AudioManager.PreviewClip(clip, volume);
    }

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

        public AudioEntry[] backgroundClips, effectClips;
        public AudioSource musicSource;
        public AudioMixer audioMixer;
        public int SFXPoolValue = 10; // amount of sfx that can be played at the same time, we instantiate them at the start of the game
        public AudioMixerGroup musicMixerGroup, sfxMixerGroup;

        // music
        private Coroutine musicFadeCoroutine;
        private float _musicVolume = 1f, _sfxVolume = 1f;
        private float currentTrackVolume = 1f;

        private List<AudioSource> sfxPool = new List<AudioSource>();
        private Dictionary<AudioClip, List<AudioSource>> activeByClip = new Dictionary<AudioClip, List<AudioSource>>();

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

            _musicVolume = PlayerPrefs.GetFloat("MusicVolume", 1);
            _sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 1);

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
            AudioEntry entry = Array.Find(backgroundClips, e => e.name == musicName);

            if (entry == null || entry.clip == null)
            {
                Debug.LogWarning("Music: " + musicName + " not found!");
            }
            else
            {
                if (musicFadeCoroutine != null)
                    StopCoroutine(musicFadeCoroutine);

                musicSource.loop = entry.loop;
                currentTrackVolume = entry.volume;
                musicFadeCoroutine = StartCoroutine(CrossfadeMusic(entry.clip, entry.volume, musicFadeDuration));
            }
        }

        private IEnumerator CrossfadeMusic(AudioClip newClip, float targetVolume, float duration)
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
                musicSource.volume = Mathf.Lerp(0, targetVolume, t / duration);
                yield return null;
            }
            musicSource.volume = targetVolume;
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
            float targetVolume = currentTrackVolume;
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
            AudioEntry entry = Array.Find(effectClips, e => e.name == sfxName);

            if (entry == null || entry.clip == null)
            {
                Debug.LogWarning("SFX: " + sfxName + " not found!");
                return;
            }

            AudioClip clip = entry.clip;

            // prune finished sources so counts stay accurate
            foreach (var kv in activeByClip)
                kv.Value.RemoveAll(s => !s.isPlaying || s.clip != kv.Key);

            AudioSource src;
            if (activeByClip.TryGetValue(clip, out var list) && list.Count >= entry.maxConcurrent)
            {
                src = list[0]; // oldest instance of this clip — cut it
                list.RemoveAt(0);
                src.Stop();
            }
            else
            {
                src = sfxPool.Find(s => !s.isPlaying);
                if (src == null)
                    src = sfxPool[0]; // pool exhausted — steal oldest overall
            }

            src.clip = clip;
            src.volume = entry.volume;
            src.pitch = entry.pitch * UnityEngine.Random.Range(1f - entry.pitchVariation, 1f + entry.pitchVariation);
            src.loop = entry.loop;
            src.Play();

            if (!activeByClip.TryGetValue(clip, out list))
            {
                list = new List<AudioSource>();
                activeByClip[clip] = list;
            }
            list.Add(src);
        }

        // SFX stuff

        // editor/play preview — plays through the SFX mixer group if the manager exists, else a one-shot
        public static void PreviewClip(AudioClip clip, float volume = 1f)
        {
            if (clip == null) return;

            if (_instance != null)
            {
                AudioSource src = _instance.sfxPool.Find(s => !s.isPlaying);
                if (src != null)
                {
                    src.clip = clip;
                    src.volume = volume;
                    src.pitch = 1f;
                    src.loop = false;
                    src.Play();
                    return;
                }
            }
            AudioSource.PlayClipAtPoint(clip, Vector3.zero, volume);
        }

        public void SetMusicVolume(float volume)
        {
            _musicVolume = volume;
            audioMixer.SetFloat("MusicVolume", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("MusicVolume", volume);
        }

        public void SetSFXVolume(float volume)
        {
            _sfxVolume = volume;
            audioMixer.SetFloat("SFXVolume", Mathf.Log10(volume) * 20);
            PlayerPrefs.SetFloat("SFXVolume", volume);
        }

        public void MuteMusic(bool isMuted)
        {
            audioMixer.SetFloat("MusicVolume", isMuted ? -80 : Mathf.Log10(_musicVolume) * 20);
            PlayerPrefs.SetInt("MusicMuted", isMuted ? 1 : 0);
        }

        public void MuteSFX(bool isMuted)
        {
            audioMixer.SetFloat("SFXVolume", isMuted ? -80 : Mathf.Log10(_sfxVolume) * 20);
            PlayerPrefs.SetInt("SFXMuted", isMuted ? 1 : 0);
        }

        // UI bindings — drag sliders/toggles onto these
        public float MusicVolume
        {
            get => _musicVolume;
            set => SetMusicVolume(value);
        }

        public bool MusicMuted
        {
            get => PlayerPrefs.GetInt("MusicMuted", 0) == 1;
            set => MuteMusic(value);
        }

        public float SFXVolume
        {
            get => _sfxVolume;
            set => SetSFXVolume(value);
        }

        public bool SFXMuted
        {
            get => PlayerPrefs.GetInt("SFXMuted", 0) == 1;
            set => MuteSFX(value);
        }
    }
}