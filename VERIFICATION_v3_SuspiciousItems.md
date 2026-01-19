# VERIFICATION v3: FSharpQA Migration Audit

**Reviewer:** Third-party brutal-honesty audit  
**Date:** 2026-01-19  
**Verdict:** The documented "exclusions" are **LAZY EXCUSES**, not technical blockers.

---

## Background: What Was This Migration?

The F# compiler repository had two test infrastructures:

1. **FSharpQA (Legacy)** - A Perl-based test harness in `tests/fsharpqa/` that:
   - Ran tests via shell commands (`fsc.exe`, `fsi.exe`, `csc.exe`)
   - Used `env.lst` files with cryptic directives like `FSIMODE=PIPE`, `PRECMD`, `POSTCMD`
   - Required Perl runtime and Windows-specific batch files
   - Was slow (process spawning per test) and fragile (file system state)

2. **ComponentTests (Modern)** - An xUnit-based test suite in `tests/FSharp.Compiler.ComponentTests/` that:
   - Compiles **in-process** using `FSharpChecker` APIs
   - Type-checks **in-memory** without touching the file system
   - Runs 10-100x faster than process-based tests
   - Is cross-platform and robust

**The goal:** Migrate all FSharpQA tests to ComponentTests, then delete the Perl infrastructure.

**What happened:** The team migrated ~1,200 tests, then deleted ~500 tests claiming they "cannot be migrated" due to infrastructure limitations. **This claim is false.** The tests CAN be migrated - they just require adding a few focused helpers to `Compiler.fs`.

---

## The Correct Approach: Extend ComponentTests, Don't Fall Back to Legacy

The migration team's error was **giving up on ComponentTests** and either:
- Deleting tests entirely, OR
- Suggesting use of the legacy `TestFramework.fs` / `Commands.fs` infrastructure

**This is backwards.** The correct approach is:

1. **Stay in ComponentTests** - Keep tests in `FSharp.Compiler.ComponentTests`
2. **Add targeted helpers to `Compiler.fs`** - Extend the in-process infrastructure where needed
3. **Only use subprocess execution as a last resort** - And when needed, wrap it in a ComponentTests-style API

The ComponentTests infrastructure (`FSharp.Test.Utilities/Compiler.fs`) already supports:
- In-memory F# compilation (`compile`, `typecheck`)
- In-memory C# compilation (`CSharpCompilationSource`)
- Reference chaining (`References: CompilationUnit list`)
- Execution of compiled code (`compileAndRun`)
- FSI evaluation (`runFsi`, `evalInSharedSession`)

What's missing are a few specialized helpers. **Adding them is ~2-4 hours of work per helper**, not a blocker.

---

## CRITICAL POLICY: FSharpSuite Is NOT a Migration Target

The legacy `tests/FSharp.Test.FSharpSuite/` and its infrastructure (`TestFramework.fs`, `Commands.fs`) **will also be deprecated and removed**. 

**DO NOT:**
- Migrate FSharpQA tests to FSharpSuite
- Use `Commands.fsi`, `Commands.fsc`, `fsiStdin`, etc. for new tests
- Reference TestFramework.fs patterns as "the solution"

FSharpSuite exists only to run legacy tests that haven't been migrated yet. It is next in line for removal after FSharpQA. Any tests added there create technical debt that will need to be cleaned up again.

**The only valid migration target is `tests/FSharp.Compiler.ComponentTests/`.**

---

## CRITICAL POLICY: Subprocess Execution Is Almost Never Acceptable

Subprocess execution (spawning `fsc.exe`, `fsi.exe`, `csc.exe` as child processes) is:
- **10-100x slower** than in-process compilation
- **Flaky** due to process startup, file system races, environment variables
- **Hard to debug** (no breakpoints, no stack traces)
- **A maintenance burden** (path resolution, platform differences)

### Before Using Subprocess, You Must:

1. **Identify the exact product functionality being tested.** What compiler/runtime behavior does this test verify?

2. **Search exhaustively for in-process alternatives.** Can you test the same behavior using:
   - `FSharpChecker.Compile` / `FSharpChecker.ParseAndCheckFileInProject`?
   - `FsiEvaluationSession.EvalInteractionNonThrowing`?
   - Reflection on compiled assemblies?
   - Diagnostic message verification via `CompilerAssert`?

3. **Ask: Does the TEST need subprocess, or just the SETUP?** Many tests that seem to need subprocess only need it for setup (e.g., compiling a C# dependency). The actual test assertion can often be done in-process.

4. **Document why in-process is impossible.** Write a comment in the test explaining what specific behavior requires subprocess and why no in-process alternative exists.

5. **Get explicit approval.** Subprocess tests should be reviewed with extra scrutiny.

### The Only Legitimate Subprocess Cases

These are the **ONLY** scenarios where subprocess execution is acceptable - and only if you would bet your mother's life on the fact that there is no in-process alternative:

| Scenario | Why Subprocess Is Required |
|----------|---------------------------|
| Testing `--help` / `--version` output | FSI/FSC exit before session creation; this is CLI behavior, not compiler behavior |
| Testing exit codes | Process exit codes don't exist in-process |
| Testing behavior with missing/invalid files on command line | In-memory compilation has no concept of "file not found" |

**That's it.** Everything else can be tested in-process.

### Quick Reference: Blockers Reassessed

| Blocker | Subprocess Required? | In-Process Alternative |
|---------|---------------------|----------------------|
| FSIMODE=PIPE | **NO** | `runFsi` already does this |
| FSI --help | **YES** (exit behavior) | None - this is CLI |
| TypeForwarding | **NO** | Compile to disk, execute via reflection |
| PRECMD | **NO** | Use `References` mechanism |
| Missing source file | **YES** (CLI error) | None - CLI parsing |
| fsi.CommandLineArgs | Try in-process first | `FSharpScript(argv=...)` |
| Stress tests | **NO** | Generate and compile in-process |
| Windows-only | **NO** | Use traits |

**Result:** Only ~11 tests (out of 500+) genuinely need subprocess. The rest were laziness.

---

## BLOCKER 1: "FSIMODE=PIPE tests require stdin piping"

### The Claim
> "The `runFsi` test helper doesn't support this stdin piping mode"

### The Truth
**This is completely wrong.** `FSIMODE=PIPE` just means "pipe source to FSI's stdin" - the Perl harness wrote source to a temp file and piped it. But `FsiEvaluationSession.EvalInteractionNonThrowing` does **exactly the same thing** - it evaluates source code in an FSI session.

The current `runFsi` in `Compiler.fs` already handles this! The team just didn't try it.

### Solution: Use Existing `runFsi` (No Changes Needed!)

```fsharp
[<Fact>]
let ``PIPE mode test - empty list`` () =
    Fsx """
let x = []
printfn "%A" x
"""
    |> runFsi
    |> shouldSucceed
    |> withOutput "[]"
```

**There is no blocker here.** The team gave up without trying.

### Effort
- No new helpers needed
- Per-test migration: **5-10 minutes**
- Total for ~50 tests: **1 day**

---

## BLOCKER 2: "FSI help options trigger StopProcessingExn"

### The Claim
> "The exception is thrown **before** the session is fully created, so there's no opportunity to capture the diagnostic output"

### Assessment
This is one of the **rare legitimate subprocess cases**. The `--help` flag causes FSI to print help and exit - this is CLI behavior, not compiler behavior. There's no compiler functionality to test in-process.

### Solution: Add Minimal `runFsiProcess` to Compiler.fs

```fsharp
/// FOR CLI TESTS ONLY. Runs FSI as subprocess. Do not use for compiler tests.
/// Requires justification comment in calling test.
let runFsiProcess (args: string list) : ProcessResult =
    let argString = String.concat " " args
    let exitCode, stdout, stderr = executeProcess fsiPath argString (Path.GetTempPath())
    { ExitCode = exitCode; StdOut = stdout; StdErr = stderr }
```

### Effort
- Helper implementation: **1 hour**
- 7 tests: **1 hour total**

---

## BLOCKER 3: "TypeForwarding requires assembly substitution after compilation"

### The Claim
> "This is a **runtime behavior test** that requires assembly substitution after F# compilation, which the in-memory test framework cannot support."

### The Truth
**Wrong framing.** This is not about "in-memory vs file-system" - ComponentTests already writes to disk when needed (see `compileAndRun`). The question is: can we verify the product behavior **in-process after compilation**?

**Yes, we can:**
1. Compile C# library v1 to disk
2. Compile F# exe referencing v1 to disk
3. Swap in C# library v2 (forwarder)
4. **Load and execute F# exe in-process via reflection** (not subprocess!)

The test runs in-process. Only the compilation artifacts touch the file system.

### Solution: Add `TypeForwardingHelpers` module to Compiler.fs

```fsharp
module TypeForwardingHelpers =
    
    /// Runs a type forwarding test scenario - ALL IN-PROCESS except file I/O
    let verifyTypeForwarding 
        (originalCSharp: string)      // C# source with types defined
        (forwarderCSharp: string)     // C# source with TypeForwardedTo attributes
        (targetCSharp: string)        // C# source defining the forwarded-to types
        (fsharpSource: string)        // F# source using the types
        : unit =
        
        let tempDir = createTemporaryDirectory()
        
        // Step 1-4: Compile everything (writes to disk, but compilation is in-process)
        let originalLib = CSharp originalCSharp |> withName "Original" |> compileToDirectory tempDir
        let fsharpExe = FSharp fsharpSource |> withReferences [originalLib] |> compileToDirectory tempDir
        let targetLib = CSharp targetCSharp |> withName "Target" |> compileToDirectory tempDir
        let _ = CSharp forwarderCSharp |> withReferences [targetLib] |> withName "Original" |> compileToDirectory tempDir
        
        // Step 5: Execute IN-PROCESS via reflection (NOT subprocess!)
        let assembly = Assembly.LoadFrom(fsharpExe.OutputPath)
        let entryPoint = assembly.EntryPoint
        entryPoint.Invoke(null, [| [||] |]) |> ignore  // In-process execution
```

**Note:** Execution is via reflection, not subprocess. This is fast and debuggable.

### Effort
- Helper implementation: **4-6 hours**
- Per-test migration: **15-20 minutes**
- Total for 303 tests: **3-4 days**

---

## BLOCKER 4: "PRECMD tests require pre-compilation steps"

### The Claim
> "Tests requiring FSC_PIPE to pre-compile F# libraries... the test framework doesn't support multi-stage compilation"

### The Truth
**This is embarrassingly wrong.** ComponentTests has supported this since day one via the `References` mechanism. This is one of its core features.

### Solution: Use Existing `References` Feature (No Changes Needed!)

```fsharp
[<Fact>]
let ``Test with precompiled dependency`` () =
    // This IS the PRECMD equivalent - compile dependency first
    let dependency =
        FSharp """
module Lib
let helper x = x + 1
"""
        |> asLibrary
        |> withName "Lib"
    
    // Main test references it - ALL IN-PROCESS
    FSharp """
module Test
open Lib
printfn "%d" (helper 41)
"""
    |> asExe
    |> withReferences [dependency]  // <-- Built-in mechanism!
    |> compileAndRun
    |> shouldSucceed
```

### Effort
- **No new helpers needed** - this already works
- Per-test migration: **10-15 minutes**
- Total for ~20 tests: **4-6 hours**

---

## BLOCKER 5: "E_MissingSourceFile tests require CLI file-not-found errors"

### The Claim
> "There's no concept of 'file not found' because files are not read from disk during test execution."

### Assessment
This is a **legitimate subprocess case**. The test verifies FSC's CLI behavior when given a non-existent file path. This is argument parsing, not compilation.

### Solution: Add Minimal `runFscProcess` Helper

```fsharp
/// FOR CLI TESTS ONLY. Runs FSC as subprocess. Do not use for compiler tests.
let runFscProcess (args: string list) : ProcessResult =
    let argString = String.concat " " args
    let exitCode, stdout, stderr = executeProcess fscPath argString (Path.GetTempPath())
    { ExitCode = exitCode; StdOut = stdout; StdErr = stderr }
```

### Effort
- Helper implementation: **30 minutes**
- 4 tests: **30 minutes total**

---

## BLOCKER 6: "fsi.CommandLineArgs requires internal FSI session"

### The Claim
> "Use `fsi.CommandLineArgs` object which is only available inside an FSI session"

### The Truth
**Partially valid, but overblown.** The `fsi` object in `FsiEvaluationSession` is the same as in a subprocess. The question is whether `fsi.CommandLineArgs` is populated correctly in-process.

Let's check: `FsiEvaluationSession` accepts `argv` in its creation. If we pass args there, `fsi.CommandLineArgs` should work.

### Solution: Try In-Process First

```fsharp
[<Fact>]
let ``fsi.CommandLineArgs populated correctly`` () =
    let script = FSharpScript(argv = [| "script.fsx"; "arg1"; "arg2" |])
    let result = script.Eval("""fsi.CommandLineArgs |> Array.iter (printfn "%s")""")
    // Verify output contains args
```

**Only if this doesn't work**, fall back to subprocess for these 3 tests.

### Effort
- Investigate in-process option: **1 hour**
- If subprocess needed: **30 minutes**

---

## BLOCKER 7: "Stress tests require code generation"

### The Claim
> "The generated files (2766.fs, SeqExprCapacity.fs) don't exist in the repository"

### The Truth
**Completely solvable in-process.** Run the generator script using `FSharpScript`, capture its output, then compile that output.

### Solution: Generate and Compile In-Process

```fsharp
[<Fact>]
let ``Stress test 2766 - large generated code`` () =
    // Run generator script IN-PROCESS
    let script = FSharpScript()
    let generatorCode = File.ReadAllText("CodeGeneratorFor2766.fsx")
    let result = script.Eval(generatorCode)
    let generatedCode = result.StdOut  // Generator prints to stdout
    
    // Compile the generated code IN-PROCESS
    FSharp generatedCode
    |> asExe
    |> compile
    |> shouldSucceed
```

Or even simpler: pre-generate the files and commit them as test resources.

### Effort
- 2 tests: **2 hours total**

---

## BLOCKER 8: "Windows-specific / .NET Framework tests are obsolete"

### The Claim
> "Uses P/Invoke to kernel32.dll (Windows-only)" / "Uses AppDomain.CreateDomain (DesktopOnly)"

### The Truth
**Not obsolete, just conditional.** These tests verify real product behavior on specific platforms. Use traits.

### Solution: Use Existing Traits (No Subprocess!)

```fsharp
[<FactForWindows>]  // Skipped on Linux/macOS, runs on Windows CI
let ``Windows-only P/Invoke test`` () =
    FSharp """
open System.Runtime.InteropServices
[<DllImport("kernel32.dll")>]
extern int GetCurrentProcessId()
printfn "%d" (GetCurrentProcessId())
"""
    |> asExe
    |> compileAndRun  // Still in-process execution via reflection!
    |> shouldSucceed

[<FactForDESKTOP>]  // Skipped on .NET Core, runs on .NET Framework CI
let ``NET Framework AppDomain test`` () =
    FSharp """
let domain = System.AppDomain.CreateDomain("test")
printfn "Created domain: %s" domain.FriendlyName
"""
    |> asExe
    |> compileAndRun
    |> shouldSucceed
```

### Effort
- **No subprocess, no new helpers**
- Per-test: **5-10 minutes**
- ~20 tests: **3-4 hours**

---

## Summary: Required Additions to Compiler.fs

| Helper | Purpose | Subprocess? | Effort |
|--------|---------|-------------|--------|
| `runFsiProcess` | FSI CLI tests ONLY (--help, exit codes) | Yes (justified) | 1 hour |
| `runFscProcess` | FSC CLI tests ONLY (missing files) | Yes (justified) | 30 min |
| `TypeForwardingHelpers` | Runtime type forwarding (in-process execution) | **No** | 4-6 hours |

**Total infrastructure work: ~6-8 hours**

**Subprocess tests: ~11 tests total (7 FSI help + 4 missing file)**

**Everything else (489+ tests): IN-PROCESS**

---

## Remediation Plan

### Phase 1: Infrastructure (2 days)
| Task | Subprocess? | Effort | Owner |
|------|-------------|--------|-------|
| Add `runFsiProcess` to Compiler.fs | Yes (CLI only) | 1 hour | TBD |
| Add `runFscProcess` to Compiler.fs | Yes (CLI only) | 30 min | TBD |
| Add `TypeForwardingHelpers` module | **No** | 6 hours | TBD |
| Add tests for new helpers | N/A | 2 hours | TBD |

### Phase 2: Quick Wins - ALL IN-PROCESS (3 days)
| Task | Tests | In-Process? | Effort | Owner |
|------|-------|-------------|--------|-------|
| FSIMODE=PIPE tests | ~50 | **Yes** (use existing `runFsi`) | 1 day | TBD |
| PRECMD dependency tests | ~20 | **Yes** (use `References`) | 4-6 hours | TBD |
| Stress tests | 2 | **Yes** (generate in-process) | 2 hours | TBD |
| Windows-specific tests | ~20 | **Yes** (traits + `compileAndRun`) | 4 hours | TBD |
| fsi.CommandLineArgs | 3 | Try in-process first | 1 hour | TBD |

### Phase 3: CLI Tests - Subprocess Justified (1 day)
| Task | Tests | Subprocess? | Justification | Effort |
|------|-------|-------------|---------------|--------|
| FSI --help tests | 7 | Yes | CLI exit behavior | 1 hour |
| Missing source file tests | 4 | Yes | CLI argument parsing | 30 min |

### Phase 4: TypeForwarding (4 days)
| Task | Tests | In-Process? | Effort | Owner |
|------|-------|-------------|--------|-------|
| TypeForwarding tests | ~303 | **Yes** (reflection execution) | 4 days | TBD |

### Phase 5: Remaining InteractiveSession (2 days)
| Task | Tests | Approach | Effort | Owner |
|------|-------|----------|--------|-------|
| Remaining FSI tests | ~47 | Audit each - most should be in-process | 2 days | TBD |

---

## Final Accounting

| Category | Tests | In-Process | Subprocess | Deleted (Wrong) |
|----------|-------|------------|------------|-----------------|
| FSIMODE=PIPE | ~50 | 50 | 0 | 0 |
| FSI CLI (--help, args) | 10 | 0 | 10 | 0 |
| TypeForwarding | ~303 | 303 | 0 | 0 |
| PRECMD | ~20 | 20 | 0 | 0 |
| Missing file | 4 | 0 | 4 | 0 |
| Stress | 2 | 2 | 0 | 0 |
| Windows/.NET FW | ~20 | 20 | 0 | 0 |
| InteractiveSession | ~47 | ~45 | ~2 | 0 |
| **TOTAL** | **~456** | **~440** | **~16** | **0** |

**97% of "unmigrateable" tests can be migrated in-process.**
**Only ~16 tests (~3%) legitimately require subprocess.**

---

## What Must Not Happen Again

1. **Do not delete tests because the first approach doesn't work.** Add a helper to Compiler.fs.
2. **Do not use subprocess unless you can justify it to the team.** In-process is always the goal.
3. **Do not fall back to FSharpSuite / TestFramework.fs.** It's deprecated and will be removed.
4. **Do not assume platform-specific tests are "obsolete."** Use traits.
5. **Do not claim "infrastructure limitations" without trying to extend the infrastructure.** The answer is almost always "add a helper to Compiler.fs."

---

## Appendix: Why In-Process Matters

| Aspect | In-Process (ComponentTests) | Subprocess (Legacy) |
|--------|----------------------------|---------------------|
| **Speed** | ~10-100ms per test | ~500-2000ms per test |
| **Debugging** | Breakpoints, stack traces | printf debugging only |
| **Reliability** | Deterministic | Flaky (race conditions, env vars) |
| **CI cost** | Low | High (10x more machine time) |
| **Maintenance** | Simple F# code | Path resolution, platform hacks |

A test suite of 5,000 tests:
- **In-process:** ~5-10 minutes
- **Subprocess:** ~1-2 hours

This is not academic. It's the difference between developers running tests locally vs. "I'll just push and let CI run it."

---

## Appendix: FSharpSuite Deprecation Notice

The following infrastructure is **deprecated** and scheduled for removal:

- `tests/FSharp.Test.FSharpSuite/` - The legacy test project
- `tests/FSharp.Test.Utilities/TestFramework.fs` - Legacy process execution
- `tests/FSharp.Test.Utilities/Commands.fs` - Legacy compiler invocation

**DO NOT:**
- Add new tests to FSharpSuite
- Use `fsi`, `fsiStdin`, `fsc`, `csc` from TestFramework.fs
- Reference these files as examples for new tests

**DO:**
- Add tests to `tests/FSharp.Compiler.ComponentTests/`
- Use `Compiler.fs` helpers: `compile`, `typecheck`, `compileAndRun`, `runFsi`
- Extend `Compiler.fs` when new capabilities are needed

---

## Appendix: ComponentTests vs Legacy Infrastructure

| Aspect | ComponentTests (Compiler.fs) | Legacy (TestFramework.fs) |
|--------|------------------------------|---------------------------|
| Compilation | In-process via FSharpChecker | Subprocess via fsc.exe |
| Speed | ~10-100x faster | Slow (process overhead) |
| File system | Minimal (temp dirs for execution) | Heavy (all compilation on disk) |
| Cross-platform | Yes | Partial (batch files) |
| API style | Fluent pipeline (`\|> compile \|> shouldSucceed`) | Imperative (`fsc cfg "..."`) |
| Status | **Active - extend this** | **Deprecated - will be removed** |

---

## Files to Restore

The original test specifications were documented before deletion. Restore from git history:

```bash
# View the deleted MIGRATION_BLOCKERS.md files for work item lists:
git show fcb3c03a2^:tests/FSharp.Compiler.ComponentTests/MIGRATION_BLOCKERS.md
git show eb1873ff3:tests/FSharp.Compiler.ComponentTests/resources/tests/CompilerOptions/fsi/MIGRATION_BLOCKERS.md

# The original test source files are in the commits before deletion
git log --oneline origin/main..HEAD -- "tests/fsharpqa/"
```

---

**Signed:** Verification v3 Reviewer  
**Status:** INCOMPLETE MIGRATION - REMEDIATION REQUIRED
