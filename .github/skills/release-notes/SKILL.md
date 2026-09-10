---
name: release-notes
description: Write release notes for completed changes. Use when PR modifies tracked paths and needs release notes entry.
---

# Release Notes

## Version
From GitHub repo variable `VNEXT` (e.g., `10.0.300`)
- Language: `preview.md`
- VisualStudio: `<VSMajorVersion>.vNext.md`

## Path
`docs/release-notes/.<Sink>/<Version>.md`

## Sink Mapping
- LanguageFeatures.fsi → `.Language`
- src/FSharp.Core/ → `.FSharp.Core`
- vsintegration/src/ → `.VisualStudio`
- src/Compiler/ → `.FSharp.Compiler.Service`

## Format (Keep A Changelog)
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

## Entry Format
- Basic: `* Description. ([PR #NNNNN](https://github.com/dotnet/fsharp/pull/NNNNN))`
- With issue: `* Description. ([Issue #NNNNN](...), [PR #NNNNN](...))`

## Where to insert (avoid merge conflicts)
Do NOT prepend at the top of the section — every PR doing that guarantees a merge conflict.
Instead, insert at a random line within the target section using the helper, then edit there:

```
dotnet fsi .github/skills/release-notes/pick-insert-line.fsx --sink <Sink> --section <Section>
```
- `<Sink>`: `FSharp.Compiler.Service` | `FSharp.Core` | `Language` | `VisualStudio` (no leading dot).
- `<Section>`: `Fixed` | `Added` | `Changed` | `Breaking Changes` | etc.
- `--sink` auto-selects the newest version file on disk, which can differ from the
  `VNEXT`-targeted version (e.g. on servicing branches). When the version is known, prefer
  passing the explicit path: `--file docs/release-notes/.<Sink>/<Version>.md --section <Section>`.

It prints an anchor line (number + verbatim text) and whether to insert ABOVE or BELOW it.
Use the printed line as the `edit` anchor (`old_str` = the line; `new_str` places your bullet
on the indicated side). If it reports the section is missing, add the section first.

## CI Check
PR fails if changes in tracked paths without release notes entry containing PR URL.
Add `NO_RELEASE_NOTES` label to skip.
