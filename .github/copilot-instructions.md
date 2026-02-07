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

## Test

Default: `-c Debug`
Use `-c Release` for: EmittedIL tests, Optimizer tests, full component runs
spot check: `dotnet test <proj> --filter "Name~X" -c Debug`
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

## Rules

Public API change → update .fsi
New diagnostic → update `src/Compiler/FSComp.txt`
API surface change → `TEST_UPDATE_BSL=1 dotnet test tests/FSharp.Compiler.Service.Tests --filter "SurfaceAreaTest" -c Release`
After code changes → `dotnet fantomas .`
When fully done → write release notes (see skill)
