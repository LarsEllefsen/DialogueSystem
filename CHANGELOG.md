# Changelog
All notable changes to this project will be documented in this file.

## [1.1.0] - 10.10.2021

### Added
- Added animated properties for scale and rotation to TextEffect.
- Added Offset property for all animated properties.
- Added Active toggle to text effect.
- Added pause functionality.
- Added Wait Command (%[time]/%).

### Changed
- Changed DialogueTheme and DialogueSettings to ScriptableObjects.
- Split OnLetterAppear animation curve into x and y curves.
- Reworked time scale for text effects to easily allow for one-shot effects.
- Changed event "wait for input" to use the new AwaitEventResponse status instead of Pause.

## [1.0.0] - 06.10.2021
Initial release