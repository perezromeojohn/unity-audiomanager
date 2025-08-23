# Changelog
All notable changes to the Rumyoonomicon AudioManager will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

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
