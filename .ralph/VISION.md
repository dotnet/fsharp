# FSharpQA Migration Remediation: VISION

## High-Level Goal

Complete the FSharpQA test migration by restoring or properly migrating the ~500 tests that were deleted with "infrastructure limitation" excuses. Per the third-party audit (VERIFICATION_v3_SuspiciousItems.md), 97% of these tests CAN be migrated in-process.

## Key Facts

1. **58 commits** on the branch - a massive migration already done
2. **~500 tests were deleted** claiming "can't be migrated" - this is largely FALSE
3. **TypeForwarding (~303 tests)** - Only 10 migrated, 293 deleted with excuse "runtime assembly substitution not supported" - FALSE, can use reflection-based execution
4. **InteractiveSession (~97 tests)** - Only 10 migrated, ~87 deleted with excuse "FSIMODE=PIPE not supported" - FALSE, `runFsi` already supports this
5. **PRECMD tests** - Deleted claiming "multi-stage compilation not supported" - FALSE, `References` mechanism already works
6. **FSI CLI tests (~7)** - Legitimate subprocess case for --help/exit codes
7. **Missing file tests (~4)** - Legitimate subprocess case for CLI errors

## What Already Works

The ComponentTests infrastructure (`Compiler.fs`) already supports:
- In-memory F# compilation: `compile`, `typecheck`
- In-memory C# compilation: `CSharp` source
- Reference chaining: `withReferences [dependency]`
- Execution of compiled code: `compileAndRun`
- FSI evaluation: `runFsi`, `Fsx`
- Traits for platform-specific tests: `FactForNETCOREAPP`, `FactForWindows`, etc.

## What Needs To Be Added

1. **`runFsiProcess`** - For CLI tests (--help, exit codes) - ~1 hour
2. **`runFscProcess`** - For CLI tests (missing file errors) - ~30 minutes
3. **TypeForwardingHelpers** - For runtime type forwarding (in-process via reflection) - ~4-6 hours

## Design Decisions

1. **Stay in ComponentTests** - All tests go to `tests/FSharp.Compiler.ComponentTests/`
2. **NO FSharpSuite** - Do not use deprecated TestFramework.fs/Commands.fs
3. **In-process preferred** - Only use subprocess for documented legitimate cases
4. **Git moves for human review** - Use `git mv` where files exist
5. **Restore from git history** - Use `git show` to recover deleted test sources

## Constraints

- macOS environment - cannot run Windows-only tests (use traits)
- .NET Core only - skip .NET Framework-only tests (use traits)
- No new NuGet packages
- Must pass existing build/tests

## Lessons from Previous Attempts

- Previous architect sessions marked migration as "complete" without addressing the audit concerns
- The HonestReport found structural completion but did not verify semantic test coverage
- Tests were deleted with excuses instead of extending infrastructure

## Sprint Strategy

Sprints are organized by blocker category from the audit:
1. Infrastructure additions (runFsiProcess, runFscProcess, TypeForwardingHelpers)
2. FSIMODE=PIPE tests (use existing runFsi)
3. PRECMD tests (use existing References)
4. TypeForwarding tests (303 tests, biggest batch)
5. FSI CLI tests (legitimate subprocess)
6. Stress tests and platform-specific tests
7. Remaining InteractiveSession tests
