# F# Test Suite LangVersion 8.0+ Migration - Vision

## High-Level Goal

Migrate all F# compiler test files to be compatible with the new minimum language version requirement of 8.0. This involves removing or updating all references to language versions below 8.0 (4.6, 4.7, 5.0, 6.0, 7.0) in test files, then removing obsolete infrastructure.

## Previous Attempt Lessons (2026-01-15)

1. **Environment issues**: Previous runs encountered `posix_spawnp failed` errors preventing bash commands
2. **Build baseline**: Must use `SKIP_VERSION_SUPPORTED_CHECK=1` to pass existing tests
3. **Atomic changes**: Each subtask must leave the build passing - do NOT remove infrastructure before usages
4. **Some work completed**: StaticClassTests.fs and ComponentTests/Language files already migrated

## Current Status (2026-01-15 Verification Pass)

### Build Status: PASSING ✓
- `dotnet build FSharp.Compiler.Service.sln -c Release` succeeds

### Test Status: MOSTLY PASSING ✓
- **FSharp.Compiler.ComponentTests**: 4922 passed, 0 failed, 208 skipped
- **FSharp.Compiler.Service.Tests**: 2027 passed, 0 failed, 29 skipped
- **FSharp.Core.UnitTests**: 6011 passed, 0 failed, 5 skipped
- **FSharp.Build.UnitTests**: 42 passed, 0 failed
- **FSharp.Compiler.Private.Scripting.UnitTests**: 99 passed, 0 failed, 2 skipped
- **FSharpSuite.Tests**: 531 passed, 8 failed (all environment-specific, see below)

### Fixed in This Session
1. `StringInterpolationTests.String interpolation negative incomplete triple quote string` - Updated expected errors (Warning→Error severity change with langversion 8.0)
2. `ComputationExpressions.AndBang TraceApplicative Disable` - Removed test (and! feature is always available with langversion 8.0+)
3. `ForInDoMutableRegressionTest` - Added `--nowarn:3370` to suppress ref-cell deprecation warnings
4. `OpenTypeDeclarationTests.Opened types do allow unqualified access to C#-style extension methods if type has no [<Extension>] attribute` - Updated expected behavior (CSharpExtensionAttributeNotRequired feature changed behavior in F# 8.0)

### Remaining Failures (Environment-Specific, NOT Langversion-Related)
These 8 failures are NOT caused by the langversion migration:

1. **5 IL compilation tests** - Require `ilasm` for ARM64 macOS (not available in current environment)
   - `An assembly with a method, property, event, and field with the same name`
   - `An assembly with an event and field with the same name, favor the field`
   - `An assembly with an event and field with the same name, favor the field - reversed`
   - `An assembly with a property, event, and field with the same name`
   - `Open custom type with unit of measure should error with measure mismatch`

2. **Windows-only test** - Requires .NET Framework assemblies
   - `4715-optimized` - Needs System.Windows.Forms.dll, etc.

3. **SDK test** - Requires net472/.NET Framework
   - `SDKTests` - Needs Microsoft.FSharp.NetSdk.props for net472

4. **Pre-existing service issue**
   - `MultiProjectTests.Using compiler service, file referencing a DLL will correctly update` - Compilation fails (pre-existing, no changes to this file)

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
- [x] FSharpSuite tests pass (except environment-specific failures)
- [x] Infrastructure helpers removed from Compiler.fs and ScriptHelpers.fs
