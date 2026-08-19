# Changelog
All notable changes to the Rumyoonomicon AudioManager will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.1.0] - 2026-08-19

### Added
- **Per-Entry Audio Settings**: `AudioClip[]` arrays replaced with `AudioEntry[] backgroundClips`/`effectClips`. Each entry has its own:
  - `loop` — BGM loops, SFX doesn't
  - `volume` — per-track gain (0–1)
  - `pitch` + `pitchVariation` — base pitch and random +/- range per sound
  - `maxConcurrent` — SFX simultaneous-play limit; oldest instance is cut when exceeded

### Changed
- **PlaySFX concurrency**: Each SFX now tracks its own active instances. Exceeding `maxConcurrent` stops the oldest instance of that sound and plays the new one.
- **Music volume**: `PlayMusic` now applies the entry's `volume` as the crossfade target instead of always fading to 1.

### Added
- **Inspector Preview**: Right-click any entry's `clip` field → **Preview** to audition the sound in the editor. Plays through the SFX mixer group when the manager exists, else a one-shot.

### Fixed
- **ResumeMusic volume**: `FadeInMusic` now fades to the current track's `volume` instead of hardcoded `1f`, so resuming after a pause restores the correct level.

## [1.0.4] - 2026-07-21

### Added
- **UI-Ready Properties**: `MusicVolume`, `MusicMuted`, `SFXVolume`, `SFXMuted` — public properties you can drag sliders/toggles onto. No glue code needed.

### Fixed
- **CrossfadeMusic**: Fade-in target no longer snaps to 0 when crossfading while muted/paused. Source volume always fades to 1f.
- **ClearClip Leak**: Removed coroutine that could null a reused AudioSource's clip mid-playback. Pool checks `isPlaying`, not `clip`.
- **Volume Tracking**: Linear volume cached in fields — mute/unmute no longer reads back from PlayerPrefs, so volume survives mute cycles correctly even if PlayerPrefs is stale.
- **FadeInMusic**: Now uses cached `_musicVolume` as target instead of hardcoded `1f` (consistent with the rest of the system).

## [1.0.3] - 2025-08-20

### Enhanced Music Management
- **Music Fade Support**: Added crossfade duration parameter to `PlayMusic()` method for smooth transitions
- **Music Control Methods**: New methods for comprehensive music management
  - `PauseMusic()` - Pause currently playing background music
  - `ResumeMusic()` - Resume paused music playback
  - `StopMusic()` - Stop music with optional fade out

### Technical Improvements
- **Simplified Audio Data Structure**: Replaced custom Sound class with AudioClip arrays for easier management and better Unity integration
- **Code Cleanup**: Removed unused code and comments for better maintainability
- **Performance Optimization**: Streamlined audio source pooling system

### Developer Experience
- **Easier Setup**: Simplified workflow for adding and managing audio clips
- **Better Integration**: More intuitive Unity Inspector integration
- **Cleaner API**: Reduced complexity while maintaining full functionality

## [1.0.2] - 2025-05-04

### Cleanup Release
- **Removed Default Audio**: Eliminated default BGM and SFX files from AudioManager script for cleaner package distribution
- **Reduced Package Size**: Lighter package without unnecessary default audio assets

## [1.0.1] - 2025-04-04

### Documentation & Polish
- **Documentation Added**: Comprehensive README with usage examples and setup instructions
- **Changelog Introduction**: Added this changelog for version tracking
- **Bug Fixes**: Resolved minor issues in AudioManager script for improved stability

## [1.0.0] - 2025-04-04

### Initial Release
- **Core Audio Management**: Full-featured audio management system for Unity
- **Audio Source Pooling**: Efficient pooling system for optimal performance
- **BGM & SFX Support**: Separate handling for background music and sound effects
- **Unity Integration**: Seamless integration with Unity's audio system
- **Singleton Pattern**: Easy global access to audio functionality

### Features Included
- Play music with loop support
- Play sound effects with randomization
- Audio source pooling for performance
- Volume control for different audio categories
- Simple API for quick integration

---

**Repository**: [unity-audiomanager](https://github.com/perezromeojohn/unity-audiomanager)  
**Author**: [@rumyoonomicon](https://rumyoonomicon.itch.io)
