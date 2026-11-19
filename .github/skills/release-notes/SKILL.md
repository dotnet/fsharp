---
name: release-notes
description: Write per-PR changelog entries for completed F# changes, or draft and review F# release announcements and articles for .NET preview, RC, and stable milestones.
---

# Release Notes

## Choose the task

- **Per-PR changelog entry:** Use the rules below when a PR changes tracked paths.
- **Milestone announcement:** Read [F# release announcements](references/release-announcements.md) before drafting or reviewing a .NET/F# release article.

The announcement workflow does not replace the per-PR helper or CI check.

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
