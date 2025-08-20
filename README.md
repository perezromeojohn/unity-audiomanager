# Rumyoonomicon Audio Manager for Unity

## Overview

A robust, easy-to-use audio manager for Unity.  
Supports background music and sound effects with smooth crossfading, fade in/out, pooling for SFX, and volume/mute controls via AudioMixer.

## Features

- **Play, Pause, Resume, and Stop music with fade in/out**
- **Automatic crossfade between music tracks**
- **SFX pooling for efficient simultaneous playback**
- **Volume and mute controls for music and SFX (with PlayerPrefs persistence)**
- **Random pitch variation for SFX**
- **Easy integration and extensibility**

## Installation

1. Open Unity Package Manager (`Window > Package Manager`)
2. Click the plus icon and select **Add package from git URL...**
3. Paste the following URL and click **Add**:

   ```
   https://github.com/perezromeojohn/unity-audiomanager.git
   ```

## Usage

1. In the Unity toolbar, go to **Tools > Rumyoonomicon > Audio Manager** to create an AudioManager GameObject.
2. In the inspector, configure:
   - **Background Clips**: List of music tracks.
   - **Effect Clips**: List of sound effects.
   - **Music Source**: AudioSource for music.
   - **Audio Mixer**: Controls volume/mute for music and SFX.
   - **Music/SFX Mixer Groups**: Assign mixer groups for routing.
   - **SFX Pool Size**: Number of pooled AudioSources for SFX.

3. Add your audio clips to the lists and assign names for easy reference.

### Example Code

```csharp
// Play a sound effect by name
AudioManager.Instance.PlaySFX("Click");

// Play background music with crossfade
AudioManager.Instance.PlayMusic("MainTheme");

// Pause music with fade out
AudioManager.Instance.PauseMusic();

// Resume music with fade in
AudioManager.Instance.ResumeMusic();

// Stop music with fade out
AudioManager.Instance.StopMusic();

// Instant stop music
AudioManager.Instance.InstantStopMusic();

// Instant pause music
AudioManager.Instance.InstantPauseMusic();
```

### Volume and Mute Controls

```csharp
// Set music volume (0.0 to 1.0)
AudioManager.Instance.SetMusicVolume(0.8f);

// Mute/unmute music
AudioManager.Instance.MuteMusic(true);

// Set SFX volume
AudioManager.Instance.SetSFXVolume(0.5f);

// Mute/unmute SFX
AudioManager.Instance.MuteSFX(false);
```

## Customization

- Adjust fade durations and pool size in the inspector.
- Extend the AudioManager script for advanced features (playlists, spatial audio, etc.).

## Version History

* [1.0.0](CHANGELOG.md) - Initial release
* [1.0.1](CHANGELOG.md) - Documentation and bug fixes
* [1.0.2](CHANGELOG.md) - Removed default BGM/SFX
* [1.0.3](CHANGELOG.md) - Added crossfade and fade in/out for music

## Author

[@rumyoonomicon](https://twitter.com/rumyoonomicon)

## Acknowledgments

Special thanks to Gabrielle for her support and encouragement. 💕
