# GitHub Copilot Instructions for F# Compiler

## STRUCTURE YOUR CHANGE (BEFORE EDITING)
Keep scope tight.  
General guide:
- Use F#
- Target .NET Standard 2.0 for compatibility
- Avoid external dependencies – the codebase is self-contained (do NOT add new NuGet packages)
- Follow docs/coding-standards.md and docs/overview.md

**Test‑First** (bugs / regressions): Add/adjust a minimal test that fails on current main → confirm it fails → implement fix → run core command and ensure test passes → only then continue.

Plan your task:
1. Write a 1–2 sentence intent (bug fix / API add / language tweak).  
2. Identify domain: Language (`LanguageFeature.fsi` touched) vs `src/FSharp.Core/` vs `vsintegration/` vs compiler/service.  
3. Public API? Edit matching `.fsi` simultaneously.  
4. New/changed diagnostics? Update FSComp.txt.  
5. IL shape change expected? Plan ILVerify baseline update.  
6. Expect baseline diffs? Plan `TEST_UPDATE_BSL=1` run.  
7. Add/adjust tests in existing projects.  
8. Decide release-notes sink now (Section 8).  
9. Run formatting only at the end.

---

# AFTER CHANGING CODE ( Agent-only. Ubuntu only )

Always run the core command. Always verify exit codes. No assumptions.

## 1. Core Command
```
./build.sh -c Release --testcoreclr
```
Non‑zero → classify & stop.

## 2. Bootstrap (Failure Detection Only)
Two-phase build. No separate bootstrap command.  
Early proto/tool errors (e.g. "Error building tools") → `BootstrapFailure` (capture key lines). Stop.

## 3. Build Failure
Proto ok but solution build fails → `BuildFailure`.  
Capture exit code, ≤15 error lines (`error FS`, `error F#`, `error MSB`), binlog path: `artifacts/log/Release/Build.*.binlog`.  
Do not proceed to tests.

## 4. Tests
Core command runs CoreCLR tests:
- FSharp.Test.Utilities
- FSharp.Compiler.ComponentTests
- FSharp.Compiler.Service.Tests
- FSharp.Compiler.Private.Scripting.UnitTests
- FSharp.Build.UnitTests
- FSharp.Core.UnitTests
Failures → `TestFailure` (projects + failing lines + baseline hints).

## 5. Baselines
Drift → update then re-run.

General/component:
```
TEST_UPDATE_BSL=1
./build.sh -c Release --testcoreclr
```
Surface area:
```
TEST_UPDATE_BSL=1 
dotnet test tests/FSharp.Compiler.Service.Tests/FSharp.Compiler.Service.Tests.fsproj --filter "SurfaceAreaTest" -c Release /p:BUILDING_USING_DOTNET=true
```
ILVerify:
```
TEST_UPDATE_BSL=1 
pwsh tests/ILVerify/ilverify.ps1
```
Classify: `BaselineDrift(SurfaceArea|ILVerify|GeneralBSL)` + changed files.

## 6. Formatting
```
dotnet fantomas . --check
```
If fail:
```
dotnet fantomas .
dotnet fantomas . --check
```
Still failing → `FormattingFailure`.

## 7. Public API / IL
If new/changed public symbol (`.fsi` touched or public addition):
1. Update `.fsi`.
2. Surface area baseline flow.
3. ILVerify if IL shape changed.
4. Release notes (Section 8).
Missed baseline update → `BaselineDrift`.

## 8. Release Notes (Sink Rules – Compact)
Most fixes → FSharp.Compiler.Service.

| Condition | Sink |
|-----------|------|
| `LanguageFeature.fsi` changed | Language |
| Public API/behavior/perf change under `src/FSharp.Core/` | FSharp.Core |
| Only `vsintegration/` impacted | VisualStudio |
| Otherwise | FSharp.Compiler.Service |

Action each needed sink:
- Append bullet in latest version file under `docs/release-notes/<Sink>/`
- Format: `* Description. ([PR #NNNNN](https://github.com/dotnet/fsharp/pull/NNNNN))`
- Optional issue link before PR.
Missing required entry → `ReleaseNotesMissing`.

## 9. Classifications
Use one or more exactly:
- `BootstrapFailure`
- `BuildFailure`
- `TestFailure`
- `FormattingFailure`
- `BaselineDrift(SurfaceArea|ILVerify|GeneralBSL)`
- `ReleaseNotesMissing`

Schema:
```
Classification:
Command:
ExitCode:
KeySnippets:
ActionTaken:
Result:
OutstandingIssues:
```

## 10. Decision Flow
1. Format check  
2. Core command  
3. If fail classify & stop  
4. Tests → `TestFailure` if any  
5. Baseline drift? update → re-run → classify if persists  
6. Public surface/IL? Section 7  
7. Release notes sink (Section 8)  
8. If no unresolved classifications → success summary

## 11. Success Example
```
AllChecksPassed:
  Formatting: OK
  Bootstrap: OK
  Build: OK
  Tests: Passed
  Baselines: Clean
  ReleaseNotes: FSharp.Compiler.Service
```

## 12. Failure Example
```
BootstrapFailure:
  Command: ./build.sh -c Release --testcoreclr
  ExitCode: 1
  KeySnippets:
    - "Error building tools"
  ActionTaken: None
  Result: Stopped
  OutstandingIssues: Bootstrap must be fixed
```
(eof)
