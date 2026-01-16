# FSI Test Migration Blockers

This document tracks FSI tests that **cannot be migrated** from `tests/fsharpqa/Source/CompilerOptions/fsi/` to the component test infrastructure due to technical limitations in `FsiEvaluationSession.Create`.

## Summary

| Folder | Test Count | Error ID | Reason |
|--------|------------|----------|--------|
| help | 4 | N/A (StopProcessingExn) | Help options (`-?`, `--help`, `/?`) exit before session creation |
| help (langversion) | 1 | N/A (StopProcessingExn) | `--langversion:?` help output exits before session creation |
| highentropyva | 1 | FS0243 | Unrecognized option crashes session |
| subsystemversion | 1 | FS0243 | Unrecognized option crashes session |
| **Total** | **7** | | |

> **Note**: The `--langversion:?` blocker is counted separately because while it's documented in the same help baseline files, it represents a distinct help feature with the same migration blocker behavior.

---

## Technical Root Cause

The `runFsi` function in the component test infrastructure uses `FsiEvaluationSession.Create` which throws `StopProcessingExn` in these scenarios:

1. **Help options** (`-?`, `--help`, `/?`): FSI processes the help request and throws `StopProcessingExn` to exit cleanly after printing help.

2. **Unrecognized options** (`--highentropyva+`, `--subsystemversion:X.XX`): These are valid `fsc.exe` options but not valid `fsi.exe` options. FSI reports FS0243 and throws `StopProcessingExn`.

The exception is thrown **before** the session is fully created, so there's no opportunity to capture the diagnostic output through the normal test infrastructure.

---

## Blocked Tests Detail

### 1. help (4 tests)

**Source**: `tests/fsharpqa/Source/CompilerOptions/fsi/help/env.lst`

| Test | Option | Baseline File |
|------|--------|---------------|
| `-?-40` | `-?` | help40.437.1033.bsl |
| `--help-40` | `--help` | help40.437.1033.bsl |
| `/?-40` | `/?` | help40.437.1033.bsl |
| `-? --nologo-40` | `--nologo -?` | help40-nologo.437.1033.bsl |

**Original Test Pattern**:
```perl
SOURCE=dummy.fsx COMPILE_ONLY=1 SCFLAGS="--nologo" FSIMODE=EXEC PRECMD="$FSI_PIPE >help.txt -? 2>&1" POSTCMD="$FSI_PIPE --nologo --quiet --exec ..\\..\\..\\comparer.fsx help.txt help40.437.1033.bsl"
```

**Why Blocked**: The legacy test runs `fsi.exe` as a separate process, captures stdout/stderr to a file, then compares against baseline. The component test infrastructure uses `FsiEvaluationSession.Create` which throws `StopProcessingExn` when help is requested, preventing session creation.

---

### 2. highentropyva (1 test)

**Source**: `tests/fsharpqa/Source/CompilerOptions/fsi/highentropyva/env.lst`

**File**: `E_highentropyva01.fsx`

**Original Test**:
```perl
SOURCE=E_highentropyva01.fsx COMPILE_ONLY=1 FSIMODE=PIPE SCFLAGS="--highentropyva+"
```

**Expected Error**:
```fsharp
//<Expects status="error" id="FS0243">Unrecognized option: '--highentropyva+'</Expects>
```

**Why Blocked**: `--highentropyva` is a valid `fsc.exe` compiler option but is not recognized by `fsi.exe`. When FSI encounters this unrecognized option, it reports FS0243 and throws `StopProcessingExn` before the session can be created.

---

### 3. subsystemversion (1 test)

**Source**: `tests/fsharpqa/Source/CompilerOptions/fsi/subsystemversion/env.lst`

**File**: `E_subsystemversion01.fsx`

**Original Test**:
```perl
SOURCE=E_subsystemversion01.fsx COMPILE_ONLY=1 FSIMODE=PIPE SCFLAGS="--subsystemversion:4.00"
```

**Expected Error**:
```fsharp
//<Expects status="error" id="FS0243">Unrecognized option: '--subsystemversion'</Expects>
```

**Why Blocked**: Same as highentropyva - `--subsystemversion` is a valid `fsc.exe` option but not recognized by `fsi.exe`. The session throws `StopProcessingExn` before creation completes.

---

### 4. langversion help (1 test)

**Source**: `tests/fsharpqa/Source/CompilerOptions/fsi/help/` (documented in baseline files)

**Option**: `--langversion:?`

**Behavior**: Displays the allowed values for language version and exits.

**Why Blocked**: The `--langversion:?` option causes FSI to display help output for available language versions and exit. Like other help options, this triggers `StopProcessingExn` before `FsiEvaluationSession.Create` completes, preventing the component test infrastructure from capturing the output.

**Evidence**: The help baseline file `help40.437.1033.bsl` documents this option at lines 66-67:
```
--langversion:?                          Display the allowed values for
                                         language version.
```

---

## Potential Solutions (Future Work)

### Option 1: Process Execution Helper
Add a test helper that runs `fsi.exe` as a separate process (like the legacy Perl harness) and captures stdout/stderr:

```fsharp
let runFsiProcess (args: string list) : (int * string * string) =
    // Returns (exitCode, stdout, stderr)
```

This would allow testing scenarios where FSI exits before session creation.

### Option 2: Modify FsiEvaluationSession
Change `FsiEvaluationSession.Create` to not throw `StopProcessingExn` for help/unrecognized options, instead returning the error through a different mechanism.

**Risk**: This could break existing consumers of the API.

### Option 3: Keep in Legacy Suite
Accept these 7 tests as permanent blockers and maintain them in the legacy fsharpqa infrastructure indefinitely.

---

## References

- **Original fsharpqa folders** (DO NOT DELETE):
  - `tests/fsharpqa/Source/CompilerOptions/fsi/help/`
  - `tests/fsharpqa/Source/CompilerOptions/fsi/highentropyva/`
  - `tests/fsharpqa/Source/CompilerOptions/fsi/subsystemversion/`

- **Migration Master Document**: `/FSHARPQA_MIGRATION.md`

- **Component Test FSI Infrastructure**: `tests/FSharp.Test.Utilities/Compiler.fs` (`runFsi` function)
