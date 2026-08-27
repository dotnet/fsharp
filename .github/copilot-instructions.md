# F# Compiler

## Build

Default (set `BUILDING_USING_DOTNET=true` system-wide):
```bash
dotnet build <changed>.fsproj -c Debug
```
Get target framework: `dotnet msbuild <proj> -getProperty:TargetFrameworks`
FSharp.Core + compiler composite: `./build.sh -c Release`
FSharp.Build changes: `./build.sh -c Release`

## No bullshit

Build fails → 99% YOUR previous change broke it. You ARE the compiler.
DON'T say "pre-existing", "infra issue", "unrelated".
DO `git clean -xfd artifacts` and rebuild.
Bootstrap contamination: early commits break compiler → later "fixes" still use broken bootstrap. Clean fully.
Triage a build failure → `binlog-analysis` skill fetches the binlog (local build or failed AzDo PR build) and analyzes it live via the `binlog-mcp` MCP (structured errors, root-cause diagnose, MSBuild perf X-ray).

## Test

Default: `-c Debug`

Use `-c Release` for: EmittedIL tests, Optimizer tests, full component runs

spot check: `dotnet test <proj> [--filter-method|--filter-class] "<glob_pattern>" -c Debug`

full component: `dotnet test tests/FSharp.Compiler.ComponentTests -c Release`

IDE/service: `tests/FSharp.Compiler.Service.Tests`

VS integration: `vsintegration/` (Windows only)

update baselines: `TEST_UPDATE_BSL=1 <test command>`

## Spotcheck tests

- find new tests for bugfix/feature
- find preexisting tests in same area
- run siblings/related

## Final validation (Copilot Coding Agent only)

Before submitting: `./build.sh -c Release --testcoreclr`

## Code

.fs: implementation
.fsi: declarations, API docs, context comments

Abbreviations (`ad`, `cenv`, `m`, `tcref`, `eenv`, `cgbuf`, `ncenv`, `tau`, …): see `docs/coding-standards.md` for the canonical glossary before guessing what a short identifier means.

## Rules

Public API change → update .fsi
New diagnostic → update `src/Compiler/FSComp.txt`
API surface change → `TEST_UPDATE_BSL=1 dotnet test tests/FSharp.Compiler.Service.Tests --filter "SurfaceAreaTest" -c Release`
After code changes → `dotnet fantomas .`
When fully done → write release notes (see skill)

## Pull requests

Push topic branches to `dotnet/fsharp` directly (maintainers have push access); never to a personal fork. Every PR's head repository must be `dotnet/fsharp`, not a fork.

"Stacked PRs" means GitHub's **native Stacks** feature, not a hand-built chain of PRs that merely target each other's branches:
1. Open each layer as an ordinary PR, bottom to top — the bottom targets `main`, each higher one targets the branch below it.
2. Then **register** them as a stack via the Stacks REST API (bottom to top):
   `gh api --method POST repos/dotnet/fsharp/stacks -F 'pull_requests[]=<bottom>' -F 'pull_requests[]=<top>'`
   Verify with `gh api repos/dotnet/fsharp/stacks/<n>`.

Native stacks require every member's head branch to live in `dotnet/fsharp` (no fork heads). Auto-merge is not supported on stacked PRs — the stack's cascading merge replaces it.
