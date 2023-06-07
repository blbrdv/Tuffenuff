# Changelog
[![Common Changelog](https://common-changelog.org/badge.svg)](https://common-changelog.org)

## [0.0.3-beta] - 2023-06-04

### Added

- Added `fromFile()` function for reading raw Dockerfiles
- Added `MAINTAINER` instruction and marked as deprecated

### Changed

- Changed building function of `RUN` instruction to CE
- Changed project license from `WTFPL` to `MIT`

## [0.0.2-alpha] - 2023-05-31

### Added

- Added flags support for `RUN` instruction
- Added `br` function for line breaks

### Removed

- Removed line break auto insert for `FROM` instruction

## [0.0.1-alpha] - 2023-05-30

Initial release.

[0.0.3-beta]: https://github.com/blbrdv/Tuffenuff/releases/tag/v0.0.3-alpha
[0.0.2-alpha]: https://github.com/blbrdv/Tuffenuff/releases/tag/v0.0.2-alpha
[0.0.1-alpha]: https://www.nuget.org/packages/Tuffenuff/0.0.1-alpha
