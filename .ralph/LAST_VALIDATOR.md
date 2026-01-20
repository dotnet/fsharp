# Last Validator Output - Subtask 1

_Updated: 2026-01-20 10:17:57_

● This is a coding task for fixing nullness flow in match expressions. Let me verify the task is
   complete by:
   1. Checking if the fix was implemented
   2. Running the build and tests

● The fix is implemented. Let me check for release notes in the correct location and look for
   tests related to issue #18488:

● Good, tests were added for issue #18488. Let me check for release notes and the correct
   location:

● Good, tests are comprehensive. Let me check for release notes in the .FSharp.Compiler.Service
   folder:

● Release notes are missing for issue #18488. Let me also run the build and tests to verify the
   fix works:

● Build failed with 158 errors. Let me check the actual errors:

● The build failures are in `prim-types.fs` and `prim-types.fsi` - with "Unexpected keyword 'or'"
   errors at line 1750 and 657. This looks like a pre-existing issue with the FSharp.Core files,
   likely due to the compiler being used to build them. Let me check these lines:

● These errors appear to be a pre-existing issue with the build infrastructure - the `(or)`
   operator definition is valid F# and has been in the codebase for a long time. The build is using
    an older/incompatible compiler. Let me check if there's a proto build needed first:

● Only the `.ralph/` directory is untracked. Let me check if there are any uncommitted changes
   related to the fix:

● Let me see more commits to understand what was changed:

● I see there are more files changed. Let me check if the core fix is present and verify the build
    issue is a pre-existing environment problem. Let me try a cleaner build approach:

● The compiler builds successfully! The previous failure was an FSharp.Core build issue (likely
   needs a bootstrap compiler). Let me run just the nullness tests:

● 57 tests passed, 1 failed (an unrelated IL baseline issue). Let me run just the tests for the
   specific issue:

● The test is failing - the fix is not working correctly for the type alias case. The test "Match
   null branch should refine variable to non-null in subsequent branches - type alias" is failing
   because `x.ToString()` still gets nullness warnings. Let me examine the fix implementation more
   closely:

● The issue is that the current fix for type abbreviations (`TType_app (tcref, tinst,
   KnownWithoutNull)`) only updates the outer nullness annotation but the `tinst` (type
   instantiation) may contain nullable types. For `type objnull = obj | null`, the type
   abbreviation carries the nullable information within `tinst`. Let me look at the test more
   carefully:

● I see the test is checking a type alias `type objnull = obj | null`. The problem is that the
   test is using `x.ToString()` which calls a method. The error message in the test output shows
   the warning about line 12 which corresponds to `x.ToString()`.

   The test is failing because in the type alias case `type objnull = obj | null`, the fix doesn't
   work correctly. Let me look at how `replaceNullnessOfTy` works:

● I see the issue now. The current fix in `removeNull` for type abbreviations just changes the
   outer nullness but doesn't fully resolve the type abbreviation. Let me check what `stripTyEqns`
   does versus what we need:

● The issue is that when we have `type objnull = obj | null`, the type abbreviation representation
    might expand to `obj | null` and we need to correctly handle this. Let me check if there were
   any changes to TypedTreeOps:

● I see that the previous implementation included a `tryRefineToNonNullPreservingAbbrev` function
   that was removed. This function had more sophisticated logic. The current implementation in
   CheckExpressions.fs is simpler but appears to be incomplete.

   The core issue is:
   1. For `type objnull = obj | null`, the type abbreviation IS the nullable type
   2. The current fix just replaces the outer nullness marker but doesn't handle the case where the
    abbreviation expands to a nullable type

   Let me look at the actual failing test output more closely - the error is for `x.ToString()`
   call. This suggests that even after the null pattern match, the variable `x` is still considered
    nullable.

   Let me verify whether the issue is the fix is incomplete, or there's a release notes missing.
   Based on the verification:

   **Summary of verification:**

   1. **Implementation exists** - The fix is in CheckExpressions.fs at line 10735-10744 with a
   `removeNull` function that handles type abbreviations.

   2. **Tests exist** - 3 regression tests for issue #18488 were added at lines 1558-1608 in
   NullableReferenceTypesTests.fs.

   3. **Tests are failing** - The "type alias" test case is failing with nullness warnings,
   indicating the fix is incomplete.

   4. **Release notes are missing** - No release notes were added to
   `docs/release-notes/.FSharp.Compiler.Service/`.

   5. **Build status** - The compiler itself builds successfully but the FSharp.Core cannot be
   built (pre-existing infrastructure issue).

   VERIFY_FAILED

   **Issues found:**

   1. **Fix is incomplete** - The test `Match null branch should refine variable to non-null in
   subsequent branches - type alias` is failing. The type alias case (`type objnull = obj | null`)
   still produces nullness warnings after the null branch is matched. The current fix only updates
   the outer nullness annotation on type abbreviations but doesn't properly resolve the underlying
   nullable type.

   2. **Missing release notes** - No release notes were added to
   `docs/release-notes/.FSharp.Compiler.Service/` as required by acceptance criteria.

