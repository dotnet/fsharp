## Subtask 1 - Review iteration 1 (2026-01-19 16:17:16)
-    VERIFY_FAILED

   The first subtask "Add runFsiProcess and runFscProcess CLI helpers" is implemented and its tests
    pass (7/7).

   However, the full test suite has 294 failures. These failures are in tests like:
   - StaticLet tests (24+ failures)
   - LexicalFiltering/OffsideExceptions tests
   - langversion tests


## Subtask 2 - Implement iteration 1 (2026-01-19 16:31:21)
-    **VERIFY_FAILED** - The task is incomplete per VISION.md criteria.

   ### What Was Completed:
   1. ✅ **runFsiProcess and runFscProcess CLI helpers** - Added to Compiler.fs
   2. ✅ **FSI CLI tests migrated** - 7 tests in FsiCliTests.fs (help, langversion, highentropyva,
   subsystemversion)
   3. ✅ **Build succeeds** - No build errors

   ### What Remains (per VISION.md and status.txt):
   - ❌ **[3] Migrate E_MissingSourceFile CLI tests** - Todo

## Subtask 3 - Review iteration 1 (2026-01-19 17:15:26)
-    **VERIFY_FAILED**

   The task is **partially complete**. The infrastructure additions (`runFsiProcess`,
   `runFscProcess`) and initial CLI test migrations are done and working (10 tests pass). However,
   according to VISION.md and status.txt, subtasks 4-8 are still "Todo":
   - FSIMODE=PIPE InteractiveSession tests
   - PRECMD dependency tests
   - TypeForwardingHelpers module
   - TypeForwarding tests (303 tests total)


## Subtask 3 - Review iteration 2 (2026-01-19 17:25:40)
-    VERIFY_FAILED: TypeForwarding tests
   (`tests/FSharp.Compiler.ComponentTests/Conformance/TypeForwarding/TypeForwardingTests.fs` and
   `NegativeCases.fs`) exist as files but are NOT included in
   `FSharp.Compiler.ComponentTests.fsproj`. These tests will not compile or run. The project file
   must include `<Compile Include="Conformance\TypeForwarding\TypeForwardingTests.fs" />` and
   `<Compile Include="Conformance\TypeForwarding\NegativeCases.fs" />`.



## Subtask 4 - Implement iteration 1 (2026-01-19 18:02:00)
-    **VERIFY_FAILED**: Task is still in progress. Subtasks 4-8 are not complete per status.txt. The
   work completed so far (runFsiProcess, runFscProcess helpers, CLI tests, E_MissingSourceFile
   tests, 37 InteractiveSession tests) is verified working, but the full FSharpQA migration
   remediation is not yet finished per VISION.md scope.



## Subtask 4 - Implement iteration 2 (2026-01-19 18:31:13)
-    **VERIFY_FAILED**

   The FSharpQA migration task cannot be verified as complete because:

   1. **Build/test run failed with 125 errors** - The core command `./build.sh -c Release
   --testcoreclr` exited with code 1
   2. **Language version deprecation** - Tests using deprecated language versions (7.0) are now
   failing due to new "out of support" errors being emitted
   3. **Missing completion** - According to `.ralph/status.txt`, subtasks 4-8 are still incomplete:
      - [4] FSIMODE=PIPE InteractiveSession tests: Still running

## Subtask 4 - Implement iteration 3 (2026-01-19 18:59:10)
-    **VERIFY_FAILED**

   The FSharpQA migration task cannot be verified as complete because:

   1. **Build/test run failed with 125 errors** - The core command `./build.sh -c Release
   --testcoreclr` exited with code 1
   2. **Language version deprecation** - Tests using deprecated language versions (7.0) are now
   failing due to new "out of support" errors being emitted
   3. **Missing completion** - According to `.ralph/status.txt`, subtasks 4-8 are still incomplete:
      - [4] FSIMODE=PIPE InteractiveSession tests: Still running
- Did not output SUBTASK_COMPLETE

## Subtask 4 - Review iteration 1 (2026-01-19 19:31:00)
-    **VERIFY_FAILED**

   The migration task is incomplete. Per the status tracker:
   - Only 3 of 8 subtasks are marked Done
   - Subtask 4 (FSIMODE=PIPE InteractiveSession tests) is still running after 114 minutes
   - Subtasks 5-8 (PRECMD tests, TypeForwardingHelpers, TypeForwarding test batches) are still
   "Todo"
   - Per VISION.md, the goal was to migrate ~500 deleted tests; only infrastructure and ~20 CLI
   tests have been migrated so far


## Subtask 4 - Review->Implement iteration 2 (2026-01-19 19:39:39)
- **Test host crashes on ~50% of tests** - Many tests in `Misc.fs` cause the test host process
- `ExnOnNonUIThread` - Explicitly throws unhandled async exception (`Async.Start (async {
- `TryWithFinally`, `CustomException`, `AbstractClass`, `ArraySlicing`, `StringInterpolation`,
- **Verification criteria not met:**
- ✅ Build succeeds: `dotnet build tests/FSharp.Compiler.ComponentTests` passes
- ❌ Tests pass: Only 5-6 tests pass before crash; running `--filter InteractiveSession`
- ✅ 113 [<Fact>] tests added (exceeds 30 requirement)
- ✅ CommandLineArgs tests exist and pass (BLOCKER 6 resolved)
- **Root cause:** The tests were not properly verified after migration. The `ExnOnNonUIThread`

## Subtask 5 - Implement iteration 2 (2026-01-19 20:55:46)
-    **VERIFY_FAILED**: 300 test failures in OffsideExceptions baseline tests. While the migration
   infrastructure (runFsiProcess, runFscProcess, TypeForwarding tests) appears complete, the build
   fails with baseline drift errors in Warning 58 indentation tests. Investigation needed to
   determine if this is a pre-existing issue or caused by the branch changes.



