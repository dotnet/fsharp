# Deletion Explanation for Core Test Files

This document audits the 6 deleted files in `tests/fsharp/core/` as part of the LangVersion 8.0+ migration.

## Categorization Reference

- **Category A**: Safe to Delete - tests "Feature X not available in version Y" error messages
- **Category B**: Safe to Delete - superseded by retained counterpart at higher langversion
- **Category C**: NEEDS INVESTIGATION - behavior tests at old versions
- **Category D**: POTENTIALLY PROBLEMATIC - unique test with no counterpart

## Core Test Files Audit

| File | Category | Justification | Risk |
|------|----------|---------------|------|
| `tests/fsharp/core/indent/version46/test.fsx` | **B** | Tests FS0058 indentation warnings at langversion 4.6. The `version47` counterpart tested the same scenarios with updated warning behavior. Since indentation rules are now standardized at 8.0+ and both version-specific directories were deleted, the behavior is covered by `tests/FSharp.Compiler.ComponentTests/Conformance/LexicalFiltering/OffsideExceptions/` tests. The deleted tests were checking version-specific offside rule differences that no longer exist. | **OK** |
| `tests/fsharp/core/indent/version47/test.fsx` | **B** | Tests FS0058 indentation warnings at langversion 4.7. This was the "success" counterpart to the 4.6 tests, verifying that certain indentation patterns that warned in 4.6 were OK in 4.7. With 8.0+ as minimum, the 4.7 behavior is the baseline, and offside exceptions are now tested in ComponentTests. | **OK** |
| `tests/fsharp/core/members/self-identifier/version46/test.fs` | **A** | Tests that `member _.Method()` syntax produces FS0010 errors in langversion 4.6. The `//<Expects id="FS0010" status="error">` annotations confirm this is purely testing "feature not available in old version" - the underscore self-identifier was introduced in F# 4.7. | **OK** |
| `tests/fsharp/core/members/self-identifier/version47/test.fs` | **B** | Tests that `member _.Method()` syntax works correctly in langversion 4.7. This was the "success" counterpart to version46. The underscore self-identifier feature is now standard and widely used throughout the codebase (80+ files use `member _.`). No dedicated feature test needed as it's implicitly tested everywhere. | **OK** |
| `tests/fsharp/core/nameof/version46/test.fsx` | **A** | Tests that `nameof` produces FS0039 "not defined" errors in langversion 4.6 (shows ~40+ error expects for "nameof is not defined"). Also includes FS3350 "Feature 'X' is not available in F# 4.6" error. This is purely testing unavailability of `nameof` in old versions. The counterpart `nameof/preview/test.fsx` tests actual nameof functionality and is retained. ComponentTests also has `Language/NameofTests.fs`. | **OK** |
| `tests/fsharp/core/fsfromfsviacs/compilation.langversion.old.output.bsl` | **A** | Baseline file for error output when compiling with old langversion. This was the expected error output for a test that used an older langversion. Since there's no test targeting old langversions anymore, this baseline is orphaned. The main `compilation.errors.output.bsl` baseline remains for the actual test. | **OK** |

## Summary

All 6 deleted core test files fall into safe deletion categories:

- **4 files in Category A**: Testing "feature not available in version X" errors - pointless once version X is no longer supported
- **2 files in Category B**: Superseded by newer counterparts or covered by ComponentTests

**No Category C or D issues found** - there are no unique behavior tests that were lost without coverage elsewhere.

## Coverage Verification

| Deleted Feature Test | Now Covered By |
|---------------------|----------------|
| Indentation warnings | `FSharp.Compiler.ComponentTests/Conformance/LexicalFiltering/OffsideExceptions/` |
| Underscore self-identifier | Implicitly tested in 80+ files using `member _.` syntax |
| `nameof` operator | `nameof/preview/test.fsx` + `FSharp.Compiler.ComponentTests/Language/NameofTests.fs` |
| Old langversion compilation | No longer applicable - 8.0 is minimum |
