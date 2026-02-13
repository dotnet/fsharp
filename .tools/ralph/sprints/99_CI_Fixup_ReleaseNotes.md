---
---
# Sprint: Fix CI - Add Missing Release Notes Entry

## Context

PR #19267 (branch `feature/langserver-skill`) previously failed ALL 25 CI jobs due to a PerfTests.fs tuple mismatch and formatting issues. Those were fixed in commit `a7d181527`. The new CI runs (AzDo builds 1293493/1293495) show **0 build errors and 0 test failures** — all AzDo jobs pass.

However, the GitHub Actions `check_release_notes` check still FAILS because the PR modifies files under `src/Compiler/` but does not include a corresponding release notes entry in `docs/release-notes/.FSharp.Compiler.Service/10.0.300.md`.

The check requires:
1. The release notes file is modified in the PR diff
2. The file contains the PR URL: `https://github.com/dotnet/fsharp/pull/19267`

## Description

### Files to Modify
- `docs/release-notes/.FSharp.Compiler.Service/10.0.300.md` — add a release notes entry for PR #19267

### Implementation Steps

1. Open `docs/release-notes/.FSharp.Compiler.Service/10.0.300.md`
2. Under the `### Added` section, add the following bullet:

```
* Added internal `CompileFromCheckedProject` API to `FSharpChecker` for emitting DLLs directly from typecheck cache, enabling fast dev-loop builds. ([PR #19267](https://github.com/dotnet/fsharp/pull/19267))
```

3. Commit with message: `Add release notes entry for PR #19267`
4. Push to `origin/feature/langserver-skill`

### What to Avoid
- Do NOT modify any other files
- Do NOT change the existing release notes entries
- The entry MUST contain the exact URL `https://github.com/dotnet/fsharp/pull/19267` (the check greps for it)

### Verification
After pushing, the `check_release_notes` GitHub Action should pass. You can verify locally by confirming the file contains the PR URL:
```bash
grep "https://github.com/dotnet/fsharp/pull/19267" docs/release-notes/.FSharp.Compiler.Service/10.0.300.md
```

## Definition of Done
- `docs/release-notes/.FSharp.Compiler.Service/10.0.300.md` contains a bullet with `https://github.com/dotnet/fsharp/pull/19267`
- The entry is under `### Added` section (since this adds a new internal API)
- Changes committed and pushed
- `check_release_notes` CI check passes on next run
