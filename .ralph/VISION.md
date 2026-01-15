# F# Test Suite LangVersion 8.0+ Migration - Vision

## High-Level Goal

Migrate all F# compiler test files to be compatible with the new minimum language version requirement of 8.0. This involves removing or updating all references to language versions below 8.0 (4.6, 4.7, 5.0, 6.0, 7.0) in test files, then removing obsolete infrastructure.

## Previous Attempt Lessons (2026-01-15)

1. **Environment issues**: Previous runs encountered `posix_spawnp failed` errors preventing bash commands
2. **Build baseline**: Must use `SKIP_VERSION_SUPPORTED_CHECK=1` to pass existing tests
3. **Atomic changes**: Each subtask must leave the build passing - do NOT remove infrastructure before usages
4. **Some work completed**: StaticClassTests.fs and ComponentTests/Language files already migrated

## Current Status (2026-01-15 Verification Pass - COMPLETE)

### Build Status: PASSING ✓
- `./build.sh -c Release --testcoreclr` succeeds

### Test Status: ALL PASSING ✓
- **FSharp.Compiler.ComponentTests**: 4872 passed, 0 failed, 208 skipped
- **FSharp.Compiler.Service.Tests**: 2027 passed, 0 failed, 29 skipped
- **FSharp.Core.UnitTests**: 6011 passed, 0 failed, 5 skipped
- **FSharp.Build.UnitTests**: 42 passed, 0 failed
- **FSharp.Compiler.Private.Scripting.UnitTests**: 99 passed, 0 failed, 2 skipped

### Fixed in This Session (2026-01-15)
1. `FsharpSuiteMigrated.fs` - Removed unreachable wildcard pattern in `adjustVersion` match
2. `Language/FixedBindings/FixedBindings.fs` - Removed entire Legacy module with 7.0 tests, converted tests to use `withLangVersionPreview`
3. `EmittedIL/FixedBindings/FixedBindings.fs` - Removed 7.0 test variants, converted to `withLangVersionPreview` 
4. `IWSAMsAndSRTPsTests.fs` - Updated `[<InlineData("6.0")>]` and `[<InlineData("7.0")>]` to 8.0/preview
5. `DynamicAssignmentOperatorTests.fs` - Updated `[<InlineData("6.0")>]` and `[<InlineData("7.0")>]` to 8.0/preview
6. `StaticClassTests.fs` - Removed `[<InlineData("7.0")>]` variant
7. `ComputationExpressionTests.fs` - Updated `[<InlineData("4.7")>]` to 8.0
8. `CompilerAssert.fs` - Updated runtime config version from 7.0 to 10.0.0

### Previous Fixes (Still Applied)
1. `StringInterpolationTests.String interpolation negative incomplete triple quote string` - Updated expected errors (Warning→Error severity change with langversion 8.0)
2. `ComputationExpressions.AndBang TraceApplicative Disable` - Removed test (and! feature is always available with langversion 8.0+)
3. `ForInDoMutableRegressionTest` - Added `--nowarn:3370` to suppress ref-cell deprecation warnings
4. `OpenTypeDeclarationTests.Opened types do allow unqualified access to C#-style extension methods if type has no [<Extension>] attribute` - Updated expected behavior (CSharpExtensionAttributeNotRequired feature changed behavior in F# 8.0)

## Migration Rules

- `withLangVersion70` → `withLangVersion80` (or `withLangVersionPreview` if testing latest features)
- `withLangVersion60` → `withLangVersion80`
- `withLangVersion50` → `withLangVersion80`
- `withLangVersion46/47` → **DELETE the test** (negative version gate tests are obsolete)
- `LangVersion.V70` → `LangVersion.V80`
- `--langversion:5.0` → `--langversion:8.0` (in env.lst)

## Key Constraint

Tests FAIL due to langversion check without env var. Use:
```bash
SKIP_VERSION_SUPPORTED_CHECK=1 dotnet test ...
```

## Success Criteria

- [x] Build passes
- [x] ComponentTests pass
- [x] FSharp.Compiler.Service.Tests pass
- [x] FSharp.Core.UnitTests pass
- [x] FSharp.Build.UnitTests pass
- [x] Scripting tests pass
- [x] No tests use unsupported langversions (4.6, 4.7, 5.0, 6.0, 7.0)
- [x] Formatting passes
- [x] Infrastructure helpers removed from Compiler.fs and ScriptHelpers.fs
