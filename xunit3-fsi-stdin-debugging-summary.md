# FSI Stdin Test Failures: xUnit v3 / MTP Migration Debugging Summary

## Overview

During migration of the F# compiler repository from **xUnit v2 + vstest** to **xUnit v3 + Microsoft Testing Platform (MTP)**, 26 FSI stdin-based tests began failing on Windows Desktop (net472) CI while passing locally.

This document summarizes the investigation, the hypotheses tested, and the current status for future reference.

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

### Phase 5: Initial Hypothesis Formed (Later Disproven)
**Findings from CI Logs**:
```
[STDIN] Read 26 bytes from file, writing to stdin
[STDIN] Successfully wrote and flushed to stdin
[PROC] Process exited with code 0
```

**Conclusion at the time**: Stdin WAS being written correctly, so the investigation pivoted toward **process output capture timing**.

**Hypothesis**: A .NET `Process` async output capture race where `OutputDataReceived`/`ErrorDataReceived` events can arrive after `WaitForExit()` returns, and `use`-scoped file writers get disposed too early.

**Rationale**: With this pattern:
```fsharp
proc.BeginOutputReadLine()
proc.BeginErrorReadLine()
proc.WaitForExit()
// File writers disposed here via `use` statements
```
the output callbacks may still be running after `WaitForExit()`.

**Why it seemed plausible**: xUnit v3 + MTP runs tests out-of-process, which changes timing and can expose races that were unlikely under xUnit v2 + vstest.

**Result**: ❌ Disproven for this incident. Synchronization attempts (see “Failed Fix Attempts”) did not change the failures, and later diagnostics showed FSI wasn’t processing stdin (see “Hypothesis Test Results (Build 1270965)”).

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
- Consider explicit synchronization (e.g., waiting for EOF signals where `e.Data == null`) before disposing writers
- Not assume `WaitForExit()` means all async output has been delivered
- Treat this as a general `Process` I/O pitfall; in this incident, applying those patterns did **not** resolve the failures

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
| 1269179 | Confirmed stdin writing works | Led to initial (later disproven) hypothesis |
| 1269714 | Verification build | Prepared for fix |
| 1270325 | ManualResetEvent fix (reverted) | Same 26 failures |
| 1270558 | Double WaitForExit() pattern | Same 26 failures |

---

## Failed Fix Attempts

### Attempt 1: ManualResetEvent Synchronization (Build 1270325)
Added ManualResetEvent signals triggered on EOF (`e.Data == null`) from `OutputDataReceived`/`ErrorDataReceived` handlers, then waited for those events after `WaitForExit()`.

Patch sketch:
```fsharp
open System.Threading

let stdoutDone = new ManualResetEvent(not redirectOutput)
let stderrDone = new ManualResetEvent(not redirectError)

proc.OutputDataReceived.Add(fun e ->
    if isNull e.Data then stdoutDone.Set() |> ignore
    else outputWriter.WriteLine(e.Data))

proc.ErrorDataReceived.Add(fun e ->
    if isNull e.Data then stderrDone.Set() |> ignore
    else errorWriter.WriteLine(e.Data))

proc.WaitForExit()
stdoutDone.WaitOne() |> ignore
stderrDone.WaitOne() |> ignore
```

Files changed in that attempt:
- `tests/scripts/scriptlib.fsx`
- `tests/FSharp.Test.Utilities/TestFramework.fs`
- `tests/fsharp/tests.fs`

**Result**: ❌ Same 26 test failures. Reverted in commit `0a470915a`.

### Attempt 2: Double WaitForExit() Pattern (Build 1270558)
Used the documented .NET pattern of calling `p.WaitForExit()` twice - the second call is supposed to wait for async output handlers to complete according to Microsoft documentation.

**Result**: ❌ Same 26 test failures.

---

## Current Status: INVESTIGATING - Hypothesis Disproven

The "Async.Start fire-and-forget" hypothesis has been **DISPROVEN** by build 1270965 diagnostics.

---

## Hypothesis Test Results (Build 1270965)

### Hypothesis Tested: Async.Start Fire-and-Forget Race Condition

**Prediction:** If stdin writing was racing with process exit due to `Async.Start`, we would see:
- Incomplete stdin writes, or
- `HasExited=true` during/before stdin write

**Actual Results from CI Logs:**

```
[20:37:00.149] RedirectInput=true RedirectOutput=true RedirectError=true
[20:37:00.153] Process started: PID=5260
[20:37:00.153] Writing to stdin (synchronously)...
[20:37:00.153] Process.HasExited=false BEFORE stdin write
[20:37:00.155] [inputWriter] Content to write: [#load "3.fsx";;\r\n#quit;;\r\n]
[20:37:00.155] [inputWriter] writer.BaseStream.CanWrite=true
[20:37:00.155] [inputWriter] CopyToAsync completed
[20:37:00.499] [inputWriter] FlushAsync completed
[20:37:00.499] Stdin closed. Process.HasExited=false AFTER stdin write
[20:37:03.062] === Process.exec END ===
```

**Key Findings:**
1. ✅ All 26 bytes written successfully
2. ✅ Content is correct: `#load "3.fsx";;\r\n#quit;;\r\n`
3. ✅ `HasExited=false` both before AND after stdin write
4. ✅ Process runs for ~2.5 seconds after stdin closes
5. ❌ Yet output is just `"> >"` - FSI didn't process the input!

### Conclusion: Hypothesis DISPROVEN

The stdin write is completing correctly and synchronously. The process doesn't exit prematurely. **The problem is not with how we write stdin - it's with how FSI receives/processes stdin.**

---

## New Hypotheses to Investigate

### ~~Hypothesis A: FSI Not Reading from Stdin~~ 
### ~~Hypothesis B: Stdin Stream Not Signaling Data Available~~
### ~~Hypothesis C: FSI Startup Race (stdin closes too fast)~~

**Note:** Hypothesis C (timing race) does not fully explain the issue. If it were a pure timing problem, it should have been at least flaky in xUnit v2. The fact that it works reliably in xUnit v2 and fails consistently in xUnit v3/MTP suggests something **specific to xUnit v3/MTP** causes the behavior change.

### Hypothesis D: MTP Console/Process Inheritance Issue (Most Likely)

xUnit v3 + MTP runs tests as **standalone executables** rather than in-process with vstest. This changes how the test process's console handles are set up, which may be **inherited by child processes** like FSI.

**Possible mechanisms:**
1. **No attached console** - MTP test processes may have no console attached. FSI may inherit this and decide stdin is not "interactive"
2. **Process creation flags** - MTP may start processes with flags like `CREATE_NO_WINDOW` or `DETACHED_PROCESS` that propagate to FSI
3. **Console.IsInputRedirected behavior** - FSI may check this property to decide whether to read stdin. The answer may differ based on how the parent process was started.

**Evidence supporting this:**
- Tests fail specifically on **Windows Desktop (net472)** - console handling differs between .NET Framework and .NET Core
- FSI shows `"> >"` (two prompts) - it's running but not processing input, suggesting it decided not to read stdin
- The stdin write completes successfully - data is in the pipe, FSI just isn't reading it

### How to Test Hypothesis D

1. **Check FSI's perspective**: Add `--readline-` flag to FSI to disable readline and see if behavior changes
2. **Check Console.IsInputRedirected**: Log this value from within the test process before spawning FSI
3. **Force console allocation**: Try `AllocConsole()` before spawning FSI to ensure a console exists
4. **Compare process creation**: Check if MTP uses different `ProcessStartInfo` settings

---

## Next Steps

1. [ ] Add diagnostic to log `Console.IsInputRedirected` in test process before spawning FSI
2. [ ] Try adding `--readline-` or other FSI flags that affect stdin reading
3. [ ] Research how MTP starts test processes and what console settings it uses
4. [ ] Consider if we need to explicitly set up console handles before spawning FSI

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
