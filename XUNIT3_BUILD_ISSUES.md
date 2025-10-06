# xUnit3 Migration - Build Issues Tracking

## Issues to Resolve

### 1. VisualFSharp.Salsa.fsproj - Missing OutputType ❌
**Error**: `xUnit.net v3 test projects must be executable (set project property 'OutputType')`
**Location**: `vsintegration/tests/Salsa/VisualFSharp.Salsa.fsproj`
**Fix Needed**: Add `<OutputType>Exe</OutputType>` property
**Status**: PENDING

### 2. FSharp.Editor.IntegrationTests.csproj - Missing Main Entry Point ❌
**Error**: `CS5001: Program does not contain a static 'Main' method suitable for an entry point`
**Location**: `vsintegration/tests/FSharp.Editor.IntegrationTests/FSharp.Editor.IntegrationTests.csproj`
**Fix Needed**: Add `<GenerateProgramFile>false</GenerateProgramFile>` for xUnit3 or investigate entry point requirement
**Status**: PENDING

### 3. FSharp.Test.Utilities - ValueTask.FromResult not available on net472 ❌
**Error**: `The type 'ValueTask' does not define the field, constructor or member 'FromResult'`
**Location**: Multiple files in `tests/FSharp.Test.Utilities/`
- DirectoryAttribute.fs (line 44)
- FileInlineDataAttribute.fs (line 172)
- XunitHelpers.fs (line 37)
**Fix Needed**: Use `new ValueTask<T>(result)` constructor instead of `ValueTask.FromResult()` for net472 compatibility
**Status**: PENDING

### 4. FSharp.Compiler.LanguageServer.Tests - Missing Module/Namespace Declaration ❌
**Error**: `FS0222: Files in libraries or multiple-file applications must begin with a namespace or module declaration`
**Location**: `tests/FSharp.Compiler.LanguageServer.Tests/Program.fs`
**Fix Needed**: Change OutputType to Exe doesn't work with current file structure; needs proper entry point or module
**Status**: PENDING

### 5. Test Execution - .NET 10 RC Not Found ⚠️
**Error**: Test process trying to use .NET 10.0.0-rc.1.25411.109 which is not installed
**Location**: Test execution on Linux/MacOS
**Note**: Tests target net10.0 but runtime not available in CI environment
**Fix Needed**: Investigate target framework configuration or runtime availability
**Status**: INVESTIGATION NEEDED

### 6. VSTest Adapter Showing in Output ⚠️
**Observation**: `xUnit.net VSTest Adapter v3.1.4` message in output
**Expected**: Should be using native xUnit3 runner, not VSTest adapter
**Fix Needed**: Review test execution configuration
**Status**: INVESTIGATION NEEDED

## Resolution Plan

1. Fix OutputType for Salsa project
2. Fix ValueTask.FromResult compatibility for net472
3. Address IntegrationTests entry point issue
4. Fix LanguageServer.Tests module declaration
5. Review .NET 10 targeting and runtime requirements
6. Verify xUnit3 runner configuration

## Notes

- Build script downloads latest .NET SDK to local `.dotnet` folder
- Some test suites skipped on Linux/MacOS which allowed build to progress further
- Need to ensure xUnit3 native runner is used, not VSTest adapter
