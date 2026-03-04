---
name: F# Release Announcement
description: Craft F# release announcements for .NET preview/release milestones by tracing SDK→VMR→dotnet/fsharp commit ranges, analyzing release notes, PRs, and issues, then producing a markdown article.
---

# 0. Resolve SDK branch names

User provides only a target release name (e.g., ".NET 11 Preview 2"). Derive everything else.

```bash
# List all release branches in dotnet/sdk
gh api repos/dotnet/sdk/branches --paginate -q '.[].name' | grep '^release/' | sort -V
```

Branch naming pattern: `release/<major>.0.<band>xx-preview<N>` or `release/<major>.0.<band>xx`

From the listing, identify:
1. **Target branch**: match release name to branch (e.g., ".NET 11 Preview 2" → `release/11.0.1xx-preview2`)
2. **Previous branch**: prior preview or prior milestone (e.g., preview1 for preview2, or last stable for preview1)
3. **Parallel stable branch**: highest `<major-1>.0.Xxx` branch (for exclusivity check)

# 1. Fetch previous announcement — DO NOT REPEAT

```
gh api repos/dotnet/core/contents/release-notes -q '.[].name'
```

Navigate to latest published `fsharp.md` under `release-notes/<major>/preview/*/`. Fetch it. Every item in it is excluded from output.

# 2. Resolve F# commit range

**CRITICAL**: Use SDK, not VMR branch. VMR branch may have newer F# than what SDK locked in.

### Get F# end commit (target release)

```
# 1. SDK → VMR hash (find the Dependency for Microsoft.FSharp.Compiler, extract its Sha)
gh api repos/dotnet/sdk/contents/eng/Version.Details.xml?ref=release/11.0.1xx-preview2 | jq -r '.content' | base64 -d | grep -A2 'Microsoft.FSharp.Compiler' | grep Sha | sed 's/.*>\([a-f0-9]*\)<.*/\1/'
# Result: VMR_HASH

# 2. VMR → F# hash
gh api repos/dotnet/dotnet/contents/src/source-manifest.json?ref=VMR_HASH | jq -r '.content' | base64 -d | jq -r '.repositories[] | select(.path=="fsharp") | .commitSha'
# Result: FSHARP_END
```

### Get F# start commit (previous release)

Repeat above with previous SDK branch (e.g., `release/11.0.1xx-preview1`).
Result: `FSHARP_START`

### Cross-check with parallel SDK branch

Repeat for `release/10.0.3xx` (or current stable band). Compare F# hash. Determine if features are shared or exclusive.

# 3. Diff release notes

```bash
git diff FSHARP_START..FSHARP_END -- docs/release-notes/ | grep '^+' | grep -v '^+++'
```

Source files:
- `docs/release-notes/.FSharp.Compiler.Service/*.md`
- `docs/release-notes/.FSharp.Core/*.md`
- `docs/release-notes/.Language/*.md`
- `docs/release-notes/.VisualStudio/*.md`

### EXCLUDE

- Already in previous announcement
- CI, infra, test-only, repo maintenance, dependency updates, localization
- FCS-internal API (unless enables user scenario)
- xUnit migration, build scripts, baseline updates

# 4. Investigate each entry via sub-agents

For each user-facing release note entry, spawn a sub-agent (explore or general-purpose) to investigate it independently. Each sub-agent writes its findings to a temp file.

```
For each entry with PR #NUMBER:

  Launch sub-agent with prompt:
  """
  Investigate dotnet/fsharp PR #NUMBER.
  1. Get PR author, title, description, linked issues
  2. Get files changed — classify as: feature / bugfix / test-only / infra
  3. If feature or significant bugfix:
     - Find test files: git diff-tree --no-commit-id -r <MERGE_SHA> --name-only | grep -i test
     - Extract the most illustrative code sample from the tests (real F# code, not test harness)
     - Note benchmark data if PR description has any
  4. If author is NOT T-Gro, Copilot, or abonie → mark as community contributor
  5. Write findings to /tmp/release-notes/pr-NUMBER.md in this format:

  ## PR #NUMBER: <title>
  - Author: <login> (community: yes/no)
  - Type: feature | bugfix | api | perf | infra
  - One-line summary: <what changed for users>
  - Issue(s): #X, #Y
  - Code sample (if feature/api):
  ```fsharp
  <code from tests>
  ```
  - Benchmark (if perf):
  <table or numbers from PR>
  """
```

Run sub-agents in parallel (up to 5). Wait for all to complete. Read all `/tmp/release-notes/pr-*.md` files to assemble the full picture.

Skip entries where sub-agent classified as `infra` or `test-only`.

# 5. Write the article

## Attribution

- Community contributors: `Thanks to [**user**](https://github.com/user)` with link
- NEVER mention: T-Gro, Copilot, abonie (internal)
- Credit community even if their PR was superseded (e.g., "Inspired by [**user**]'s work in #XXXX")

## Structure

```markdown
# F# in .NET XX Preview Y - Release Notes

Here's a summary of what's new in F# in this Preview Y release:

- [Highlight 1](#anchor)
- [Highlight 2](#anchor)
- [Category 1](#anchor)
- [Category 2](#anchor)
- [Bug fixes](#bug-fixes-and-other-improvements)

## Highlight 1              ← 1-3 sections, CODE SAMPLES from tests, minimal prose
## Highlight 2
## Category: Query fixes    ← bullet list with issue links, no code
## Category: IDE fixes      ← bullet list
## Bug fixes                ← flat bullet list, prefixed by area (Nullness:, Scripts:, etc.)

F# updates:
- [F# release notes](https://fsharp.github.io/fsharp-compiler-docs/release-notes/About.html)
- [dotnet/fsharp repository](https://github.com/dotnet/fsharp)
```

## Highlight selection (pick 1-3)

Rank by: language feature > new API > perf win with numbers > significant bug fix area
Each MUST have code sample extracted from PR tests. No invented examples.

## Style

- One-sentence intro max, then code block or table
- No fluff, no "we are excited", no "this release brings"
- Bold key terms in bullet lists
- Link every issue and PR number

# SDK branch naming

| Release | Branch |
|---|---|
| .NET 11 Preview N | `release/11.0.1xx-previewN` |
| .NET 10.0.X00 | `release/10.0.Xxx` |

# Key files

| Repo | Path | Contains |
|---|---|---|
| dotnet/sdk | `eng/Version.Details.xml` | VMR commit SHA |
| dotnet/dotnet | `src/source-manifest.json` | F# commit SHA |
| dotnet/fsharp | `docs/release-notes/` | Per-version changelogs |
| dotnet/core | `release-notes/` | Published announcements |
