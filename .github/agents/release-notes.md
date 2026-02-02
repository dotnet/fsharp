---
name: Authoring release notes
description: Write release notes for completed changes
---

# Release Notes

## Version
From GitHub repo variable `VNEXT` (e.g., `10.0.300`)
Language: `preview.md`
VisualStudio: `<VSMajorVersion>.vNext.md`

## Path
`docs/release-notes/.<Sink>/<Version>.md`

## Sink
LanguageFeatures.fsi: .Language
src/FSharp.Core/: .FSharp.Core
vsintegration/src/: .VisualStudio
src/Compiler/: .FSharp.Compiler.Service

## Sections (Keep A Changelog format)
```markdown
### Fixed
* Bug fix description. ([Issue #NNN](...), [PR #NNN](...))

### Added
* New feature description. ([PR #NNN](...))

### Changed
* Behavior change description. ([PR #NNN](...))

### Breaking Changes
* Breaking change description. ([PR #NNN](...))
```

## Entry format
`* Description. ([PR #NNNNN](https://github.com/dotnet/fsharp/pull/NNNNN))`
With issue: `* Description. ([Issue #NNNNN](...), [PR #NNNNN](...))`

## GH Action
PR fails if changes in tracked paths without release notes entry containing PR URL.
Add `NO_RELEASE_NOTES` label to skip.
