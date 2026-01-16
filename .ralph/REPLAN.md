# REPLAN: Subtask 6 - CompilerOptions/fsi Migration

## Current Status
- **Migrated**: 5 tests (3 langversion + 2 nologo) in `CompilerOptions/fsi/Langversion.fs` and `Nologo.fs`
- **Migration Blockers**: 7 tests (4 help + 1 highentropyva + 1 subsystemversion + 1 langversion help baseline)

## Issue: Criteria Cannot Be Fully Met

The subtask criteria specifies:
- "Migrate all 5 fsi folders"
- "File tests/FSharp.Compiler.ComponentTests/CompilerOptions/Fsi/Help.fs exists"

**This is not possible** due to infrastructure limitations:

### Root Cause
The component test infrastructure uses `FsiEvaluationSession.Create` to run FSI tests via the `runFsi` function. However:

1. **Help options (`-?`, `--help`, `/?`)** cause FSI to display help and immediately exit, throwing `StopProcessingExn` before the session is created. This crashes the test host.

2. **Unrecognized options (`--highentropyva+`, `--subsystemversion:`)** also throw `StopProcessingExn` during session creation, before any output can be captured.

3. **langversion help (`--langversion:?`)** is the same as above - it outputs help and exits.

### What Was Migrated
- `langversion/badlangversion.fsx` → Error test for invalid version → ✅ Works with `asFsx | compile`
- `langversion/badlangversion-culture.fsx` → Error test for comma format → ✅ Works
- `langversion/badlangversion-decimal.fsx` → Error test for long decimal → ✅ Works  
- `nologo/nologo01.fsx` → Test --nologo option → ✅ Works with `runFsi`
- `nologo/nologo02.fsx` → Test copyright banner → ✅ Works with `runFsi`

### What Cannot Be Migrated
| Test | Original Behavior | Why It Can't Be Migrated |
|------|-------------------|-------------------------|
| help `-?` | Compare stdout to baseline | Session crashes before output capture |
| help `--help` | Compare stdout to baseline | Same |
| help `/?` | Compare stdout to baseline | Same |
| help `--nologo -?` | Compare stdout to baseline | Same |
| highentropyva `--highentropyva+` | Check error output | Session crashes before output capture |
| subsystemversion `--subsystemversion:4.00` | Check error output | Session crashes before output capture |
| langversion `--langversion:?` | Compare stdout to baseline | Session crashes before output capture |

### Proposed Resolution

**Option A**: Accept partial migration (recommended)
- Mark 5 tests as migrated
- Document 7 tests as migration blockers in VISION.md
- Keep legacy folders for unmigrated tests (help, highentropyva, subsystemversion)
- Update criteria to reflect reality

**Option B**: External process infrastructure
- Create new test infrastructure that runs FSI as an external process
- Capture stdout/stderr from the process
- This is significant new work and out of scope for simple migration

**Option C**: Skip this subtask entirely
- These are DesktopOnly tests that may not need to be migrated
- The original FSharpQA suite was designed for Windows/.NET Framework

## Recommendation
Accept Option A - partial migration with documented blockers. The 5 successfully migrated tests provide good coverage of the core langversion and nologo functionality. The blocked tests are edge cases that test FSI's command-line argument parsing, which is already covered by integration tests elsewhere.
