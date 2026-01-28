# FSI Stdin Test Failures: xUnit v3 / MTP Migration Debugging Summary

## Overview

During migration of the F# compiler repository from **xUnit v2 + vstest** to **xUnit v3 + Microsoft Testing Platform (MTP)**, 26 FSI stdin-based tests began failing on Windows Desktop (net472) CI while passing locally.

This document summarizes the investigation, root cause analysis, and fix for future reference.

---

## Symptoms

### Failing Tests
- `fsi-reference`
- `fsiAndModifiers`
- `libtest-FSI_NETFX_STDIN`
- `load-script` (tests 3-17)
- Multiple `printing` variants
- `TypeProviderTests.helloWorld`

### Observed Behavior
- Tests passed locally on developer machines
- Tests failed only on Windows Desktop (net472) CI legs
- FSI output showed just `"> >"` (two prompts) instead of expected script execution results
- Exit code was 0 (success), suggesting FSI ran but received no input

---

## Investigation Timeline

### Phase 1: Parallelization Hypothesis
**Hypothesis**: xUnit v3 has different parallelization defaults causing test interference.

**Actions Taken**:
1. Added `[<Collection(nameof NotThreadSafeResourceCollection)>]` to TypeProviderTests.fs
2. Created `testconfig.json` with parallelization settings
3. Fixed MTP config naming (must be `[AssemblyName].testconfig.json`)

**Result**: ❌ Tests still failed with same errors

### Phase 2: Stdin Writing Analysis
**Hypothesis**: Something about stdin redirection changed in xUnit v3 execution model.

**Actions Taken**:
1. Changed async stdin write to synchronous in `scriptlib.fsx`
2. Added `eprintfn` diagnostic logging

**Result**: ❌ Diagnostics didn't appear in CI logs

### Phase 3: Console Capture Investigation
**Discovery**: TestConsole.fs uses `AsyncLocal<TextWriter>` to redirect Console.Out. When no `ExecutionCapture` is set, writes go to `TextWriter.Null`.

**Key Insight**: xUnit v3's `[assembly: CaptureConsole]` was intentionally disabled in the repo because it conflicts with the custom `TestConsole.ExecutionCapture` system.

**Actions Taken**:
1. Switched to `printfn` logging
2. Still no output visible

**Result**: ❌ Console-based diagnostics are invisible in this test framework

### Phase 4: File-Based Logging
**Approach**: Bypass TestConsole entirely using `File.AppendAllText()`.

**Implementation**:
```fsharp
let logFile = Path.Combine(Path.GetTempPath(), "fsi_stdin_diag.log")
File.AppendAllText(logFile, sprintf "[STDIN] Read %d bytes...\n" contentBytes.Length)
```

**Result**: ✅ Diagnostics appeared in test failure messages

### Phase 5: Root Cause Identified
**Findings from CI Logs**:
```
[STDIN] Read 26 bytes from file, writing to stdin
[STDIN] Successfully wrote and flushed to stdin
[PROC] Process exited with code 0
```

**Conclusion**: Stdin WAS being written correctly. The problem was **output capture timing**.

---

## Root Cause

### The Race Condition

When using .NET's asynchronous process output capture:
```fsharp
proc.BeginOutputReadLine()
proc.BeginErrorReadLine()
proc.WaitForExit()
// File writers disposed here via `use` statements
```

The `OutputDataReceived` events can fire **after** `WaitForExit()` returns. In `scriptlib.fsx`, the output file writers were being disposed before all output data was captured.

### Why It Worked in xUnit v2

xUnit v2 + vstest ran tests in-process with different timing characteristics. The race condition existed but rarely manifested due to:
- Different process scheduling
- In-process test execution with shared state
- Possible implicit synchronization from vstest harness

### Why It Fails in xUnit v3/MTP

xUnit v3 + MTP runs tests as standalone executables (out-of-process), which:
- Changes process scheduling timing
- Removes any implicit synchronization from shared in-process state
- Makes the race condition much more likely to occur

---

## The Fix

### Solution: ManualResetEvent Synchronization

Wait for EOF signals from async output readers before returning from `Process.exec`:

```fsharp
open System.Threading

// Create events (signaled if not redirecting)
let stdoutDone = new ManualResetEvent(not redirectOutput)
let stderrDone = new ManualResetEvent(not redirectError)

// Add EOF handlers
proc.OutputDataReceived.Add(fun e ->
    if isNull e.Data then stdoutDone.Set() |> ignore
    else outputWriter.WriteLine(e.Data))

proc.ErrorDataReceived.Add(fun e ->
    if isNull e.Data then stderrDone.Set() |> ignore
    else errorWriter.WriteLine(e.Data))

// After WaitForExit, wait for all output to be captured
proc.WaitForExit()
stdoutDone.WaitOne() |> ignore
stderrDone.WaitOne() |> ignore
```

### Files Modified
- `tests/scripts/scriptlib.fsx` - Added synchronization to `Process.exec`
- `tests/FSharp.Test.Utilities/TestFramework.fs` - Cleaned up diagnostic logging
- `tests/fsharp/tests.fs` - Removed diagnostic log reading

---

## Key Learnings for xUnit v3 Migration

### 1. Execution Model Differences
xUnit v3 + MTP runs tests out-of-process, which can expose race conditions that were hidden by in-process execution.

### 2. Console Capture Changes
- xUnit v3 requires `[assembly: CaptureConsole]` for console output in test results
- May conflict with custom console capture systems
- `CaptureTrace` can cause issues if the test framework has its own trace handling

### 3. Async Process Output
Any code using `BeginOutputReadLine()`/`BeginErrorReadLine()` should:
- Use ManualResetEvent or similar synchronization
- Wait for EOF signals (e.Data == null) before disposing writers
- Not assume WaitForExit() means all output has been delivered

### 4. Diagnostic Logging Challenges
- Console-based diagnostics may be invisible depending on capture configuration
- File-based logging (`File.AppendAllText`) reliably bypasses all redirections
- Include log file path in test failure messages for CI visibility

### 5. MTP Configuration
- Config files must be named `[AssemblyName].testconfig.json`
- Parallelization settings differ from xUnit v2
- Test isolation is stricter by default

---

## Builds Analyzed

| Build ID | Purpose | Result |
|----------|---------|--------|
| 1264374 | Initial failure analysis | 26 test failures |
| 1264623 | After parallelization fix | Same failures |
| 1268088 | With console diagnostics | Diagnostics invisible |
| 1269018 | With file-based diagnostics | Diagnostics visible |
| 1269179 | Confirmed stdin writing works | Root cause identified |
| 1269714 | Verification build | Prepared for fix |
| 1270325 | ManualResetEvent fix (reverted) | Same 26 failures |
| 1270558 | Double WaitForExit() pattern | Same 26 failures |

---

## Failed Fix Attempts

### Attempt 1: ManualResetEvent Synchronization (Build 1270325)
Added ManualResetEvent signals triggered on EOF (null data) from OutputDataReceived/ErrorDataReceived handlers. Waited for these events after WaitForExit().

**Result**: ❌ Same 26 test failures. Reverted in commit `0a470915a`.

### Attempt 2: Double WaitForExit() Pattern (Build 1270558)
Used the documented .NET pattern of calling `p.WaitForExit()` twice - the second call is supposed to wait for async output handlers to complete according to Microsoft documentation.

**Result**: ❌ Same 26 test failures.

---

## Current Status: INVESTIGATING

The root cause hypothesis (async output capture race condition) may be **incorrect or incomplete**. Both synchronization approaches that should address async output capture timing have failed to fix the issue.

### Evidence Against Output Capture Race Condition:
1. ManualResetEvent waiting for EOF signals didn't help
2. Double WaitForExit() (documented .NET pattern) didn't help
3. Both approaches would have fixed a true async output capture race

---

## Current Hypothesis (Build 1270XXX - Pending)

### Hypothesis: Async.Start Fire-and-Forget Race Condition

**Observation:** In `scriptlib.fsx`, the stdin writing is done with `Async.Start`:

```fsharp
cmdArgs.RedirectInput |> Option.iter (fun input -> 
   async {
    let inputWriter = p.StandardInput
    do! inputWriter.FlushAsync () |> Async.AwaitIAsyncResult |> Async.Ignore
    input inputWriter
    do! inputWriter.FlushAsync () |> Async.AwaitIAsyncResult |> Async.Ignore
    inputWriter.Dispose ()
   } 
   |> Async.Start)  // <-- FIRE AND FORGET!

p.WaitForExit()  // Called immediately, doesn't wait for async block
```

**Problem:** `Async.Start` is fire-and-forget. The code immediately proceeds to `WaitForExit()` without waiting for the async stdin write to complete. This could cause:
1. FSI starts and waits for stdin
2. Async stdin write is *scheduled* but not yet executed
3. `WaitForExit()` is called
4. FSI times out waiting for input, or receives partial/no input
5. FSI exits with just `"> >"` (prompts only)

**Why previous fixes didn't help:** The previous fixes (ManualResetEvent, double WaitForExit) addressed *output* capture timing, not *input* writing timing. The stdin write was still fire-and-forget.

### Validation Approach

**Changes made:**
1. Changed stdin writing from `Async.Start` (fire-and-forget) to synchronous execution
2. Added comprehensive timestamped logging to track:
   - Process start with PID
   - `HasExited` status before/after stdin write
   - `BaseStream.CanWrite` status
   - Content being written (first 200 chars)
   - Any exceptions during write
   - Exit code and captured output lengths

**Files modified:**
- `tests/scripts/scriptlib.fsx` - Synchronous stdin write + logging
- `tests/FSharp.Test.Utilities/TestFramework.fs` - Logging in inputWriter callback
- `tests/fsharp/tests.fs` - Include diagnostic log in failure message

**Expected outcomes:**

| If hypothesis is CORRECT | If hypothesis is WRONG |
|--------------------------|------------------------|
| Tests should PASS (or show different failure) | Tests still fail with same `"> >"` output |
| Logs show stdin write completes before process exits | Logs show stdin write completes but FSI still doesn't process it |
| N/A | Need to investigate FSI-side behavior |

### What to look for in CI logs:

1. **Timing:** Do timestamps show stdin write completing before `WaitForExit`?
2. **Content:** Is the correct content (`#load "3.fsx";;\n#quit;;\n`) being written?
3. **Exceptions:** Any `IOException` or other errors during write?
4. **Process state:** Does `HasExited` become true during stdin write?
5. **Output lengths:** Are stdout/stderr being captured (non-zero lengths)?

---

### Alternative Hypotheses (if current one fails):
1. **Stdin not being read at all** - FSI may not be reading from stdin in the CI environment
2. **Process environment differences** - Something in the CI process environment causes FSI to behave differently
3. **Test framework interference** - xUnit v3/MTP may be doing something to stdin/stdout streams before tests run
4. **File handle issues** - The stdin pipe or output files may have permission/locking issues in CI
5. **FSI startup timing** - FSI might not be ready to accept stdin when we write to it

---

## Conclusion

~~The FSI stdin test failures were caused by a **race condition in async process output capture** that existed in the codebase but was hidden by xUnit v2's in-process execution model.~~

**UPDATE**: The original hypothesis appears to be incorrect. Multiple synchronization fixes have failed. Further investigation with enhanced diagnostics is required to understand the actual root cause.

**Total Investigation Time**: Multiple CI cycles over several days, involving:
- Parallelization configuration attempts
- Console vs file-based diagnostic logging
- .NET Process async I/O research
- Race condition analysis and fix implementation (FAILED)
- Double WaitForExit pattern (FAILED)
