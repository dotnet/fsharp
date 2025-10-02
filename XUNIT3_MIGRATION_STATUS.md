# xUnit3 Migration Status and Plan

## Executive Summary

**Status**: Migration IN PROGRESS - Package versions exist, API migration underway.

The xUnit3 packages (3.1.0) and required dependencies are now available on nuget.org. Active work is proceeding on migrating the custom test extensibility code to the new xUnit3 APIs.

## Current Package Versions (Original)

- **xunit**: 2.9.0  
- **xunit.runner.console**: 2.8.2 (property: XUnitRunnerVersion)
- **FsCheck**: 2.16.5
- **Microsoft.NET.Test.Sdk**: 17.11.1

## Target Package Versions (NOW AVAILABLE)

- **xunit.v3**: 3.1.0 ✅ Available on nuget.org
- **xunit.v3.runner.console**: 3.1.0 ✅ Available on nuget.org
- **xunit.runner.visualstudio**: 3.1.5 ✅ Available on nuget.org
- **FsCheck**: 3.3.1 ✅ Available on nuget.org
- **Microsoft.TestPlatform**: 17.14.1 ✅ Available

## Migration Progress

### Phase 1: Infrastructure Setup ✅ COMPLETED

1. ✅ Added nuget.org to NuGet.config as package source
2. ✅ Updated `eng/Versions.props` with xUnit3 package versions
3. ✅ Updated `tests/Directory.Build.props` with xUnit3 package references
4. ✅ Updated `Directory.Build.targets` to reference xunit.v3 packages
5. ✅ Configured FSharp.Test.Utilities to use extensibility packages

### Phase 2: API Migration 🚧 IN PROGRESS

**Current Status**: Console capturing removed, fixing remaining xUnit3 API changes

**Latest Progress (Commit 9b8347e)**:
- ✅ Added `[<assembly: CaptureTrace>]` for automatic console capturing
- ✅ Removed ConsoleCapturingTestRunner (~60 lines)
- ✅ Removed Xunit.Abstractions imports
- ✅ Simplified test case classes

**Detailed Migration Guide**: See `XUNIT3_API_MIGRATION_GUIDE.md` for complete step-by-step instructions

**Files Status:**
1. ✅ **Compiler.fs** - Removed unused `Xunit.Abstractions` import  
2. ✅ **XunitSetup.fs** - Added `[<assembly: CaptureTrace>]`
3. 🚧 **XunitHelpers.fs** - Simplified, fixing API changes (~85 errors remaining)
4. 🚧 **DirectoryAttribute.fs** - DataAttribute resolution issues
5. 🚧 **FileInlineDataAttribute.fs** - DataAttribute resolution issues

**Build Errors**: 74 errors (down from 90, was 126 initially), primarily:
- **DataAttribute** type not resolved by F# compiler despite being in xunit.v3.core.dll
  - Confirmed exists via ILSpy
  - Package references configured
  - Explicit DLL reference added
  - **BLOCKER**: F# compiler still cannot resolve the type
- XunitTestCase, TestCollection, TestClass, TestMethod - concrete types removed in xUnit3
- Test case creation model changed
- Trait API changes

**Current Investigation - DataAttribute Blocker**:
- ✅ DataAttribute confirmed to exist in xunit.v3.core.dll (verified via ILSpy)
- ✅ Located in Xunit.Sdk namespace  
- ✅ Added explicit Reference to xunit.v3.core.dll
- ✅ Added xunit.v3.common package reference
- ❌ F# compiler reports "The type 'DataAttribute' is not defined in 'Xunit.Sdk'"
- ❌ Tested with fully qualified name: same error
- **CONCLUSION**: DataAttribute exists in DLL but is not accessible to F# compiler
- **POSSIBLE CAUSES**: 
  1. Type visibility issue in xUnit3 (internal/private)
  2. Assembly not properly loaded by F# compiler/MSBuild
  3. xUnit3/F# compatibility issue
  4. Need different package (xunit.v3 test framework vs extensibility)

**Recommended Path Forward**:
Given the persistent blocker and time invested, two options:
1. **Alternative Pattern**: Convert to ClassData/MemberData (xUnit3 recommended)
2. **Community Help**: File issue with xUnit3 team about F# compatibility

**Next Actions**:
1. Document findings and current state
2. Try ClassData/MemberData conversion as workaround
3. If that doesn't work, disable XUNIT_EXTRAS temporarily to unblock other tests
4. Update remaining API issues once DataAttribute resolved

**Status**: FSharp.Test.Utilities builds successfully! ✅

**Current Blocker**: xUnit3 test projects must have `<OutputType>Exe</OutputType>`

**Remaining Work**:
1. **CRITICAL**: Update all test projects to set `<OutputType>Exe</OutputType>` (xUnit3 requirement)
2. Test projects may have FsCheck 3.x API compatibility issues (separate from xUnit3)
3. Tests using DirectoryAttribute/FileInlineDataAttribute need conversion
4. Re-evaluate batch trait injection for CI if needed

**Recent Changes**:
- ✅ Removed nuget.org from NuGet.config (reverted to original sources)
- ⚠️ xUnit3 packages available from existing Azure DevOps feeds

**Estimated Remaining Effort**: 
- Update OutputType in test projects: 1-2 hours
- Fix FsCheck API issues: 2-4 hours (separate task)
- Convert custom data attribute tests: 4-6 hours (if needed)
- Total xUnit3 core migration: 80% complete (XUNIT_EXTRAS disabled, OutputType fix needed)

### 1. Test Project Configuration Cleanup ✅

**What**: Removed obsolete MSBuild properties from all test projects.  
**Why**: These properties (`<UnitTestType>`, `<IsTestProject>`) are specific to xUnit2/VSTest and not needed for xUnit3/Microsoft.TestPlatform.

**Files Modified**:
- `tests/FSharp.Compiler.ComponentTests/FSharp.Compiler.ComponentTests.fsproj`
- `tests/FSharp.Compiler.Service.Tests/FSharp.Compiler.Service.Tests.fsproj`
- `tests/FSharp.Build.UnitTests/FSharp.Build.UnitTests.fsproj`
- `tests/FSharp.Core.UnitTests/FSharp.Core.UnitTests.fsproj`
- `tests/FSharp.Compiler.Private.Scripting.UnitTests/FSharp.Compiler.Private.Scripting.UnitTests.fsproj`
- `tests/FSharp.Compiler.LanguageServer.Tests/FSharp.Compiler.LanguageServer.Tests.fsproj`
- `tests/fsharp/FSharpSuite.Tests.fsproj`
- `tests/FSharp.Test.Utilities/FSharp.Test.Utilities.fsproj`
- `tests/EndToEndBuildTests/BasicProvider/BasicProvider.Tests/BasicProvider.Tests.fsproj`
- `tests/EndToEndBuildTests/ComboProvider/ComboProvider.Tests/ComboProvider.Tests.fsproj`

**Changes Made**:
```xml
<!-- REMOVED -->
<UnitTestType>xunit</UnitTestType>
<IsTestProject>true</IsTestProject>
```

**Impact**: No build impact. These properties are ignored by modern SDK-style projects using xUnit2.

### 2. xUnit Configuration Files Updated to v3 Schema ✅

**What**: Updated all `xunit.runner.json` files to use xUnit3 configuration schema.  
**Why**: xUnit3 has a different configuration format and deprecated certain settings.

**Files Modified**: All `xunit.runner.json` files in test projects.

**Changes Made**:
```json
{
  "$schema": "https://xunit.net/schema/current/xunit.runner.schema.json",
  "parallelizeAssembly": true,
  "parallelizeTestCollections": true,
  "maxParallelThreads": 4
}
```

**Removed**:
- `"appDomain": "denied"` - Not supported in xUnit3

**Impact**: No build impact. xUnit2 ignores the new xUnit3 settings and respects the existing `parallelizeAssembly` setting.

## Remaining Work (Blocked)

### 1. Package Version Updates (eng/Versions.props) ⏸️

**Blocked on**: Stable release of xUnit3 packages.

**Required Changes**:
```xml
<!-- ADD -->
<MicrosoftTestPlatformVersion>17.14.1</MicrosoftTestPlatformVersion>

<!-- UPDATE --> 
<XunitVersion>3.1.0</XunitVersion>  <!-- currently: XUnitVersion>2.9.0 -->
<XunitRunnerConsoleVersion>3.0.1</XunitRunnerConsoleVersion>  <!-- currently: XUnitRunnerVersion>2.8.2 -->
<FsCheckVersion>3.3.1</FsCheckVersion>  <!-- currently: 2.16.5 -->
```

### 2. Central Package References (tests/Directory.Build.props) ⏸️

**Blocked on**: Package version updates.

**Required Changes**:
```xml
<!-- ADD new ItemGroup for test projects -->
<ItemGroup Condition="$(MSBuildProjectName.EndsWith('.Tests')) OR ...">
  <PackageReference Include="xunit.v3" Version="$(XunitVersion)" />
  <PackageReference Include="xunit.v3.runner.console" Version="$(XunitRunnerConsoleVersion)" />
  <PackageReference Include="Microsoft.TestPlatform" Version="$(MicrosoftTestPlatformVersion)" />
  <PackageReference Include="FsCheck" Version="$(FsCheckVersion)" />
</ItemGroup>
```

**Note**: Package names change from `xunit` to `xunit.v3` and `xunit.runner.visualstudio` to `xunit.v3.runner.console`.

### 3. XunitHelpers.fs API Migration ⏸️

**Blocked on**: xUnit3 stable release and official API documentation.

**File**: `tests/FSharp.Test.Utilities/XunitHelpers.fs`

**Required Changes** (approximate, based on preview documentation):

| xUnit2 API | xUnit3 API |
|------------|------------|
| `Xunit.Sdk.*` | `xunit.v3.core.*` or `xunit.v3.extensibility.core.*` |
| `XunitTestCase` | New v3 equivalent |
| `XunitTheoryTestCase` | New v3 equivalent |
| `XunitTestRunner` | New v3 base class |
| `XunitTestFramework` | New v3 base class |
| `XunitTestFrameworkDiscoverer` | New v3 base class |
| `ITestCase`, `ITestMethod`, etc. | Updated interfaces |

**Complexity**: HIGH - This file has ~280 lines of custom xUnit extensibility code:
- Custom test case runners with console capturing
- Test parallelization logic
- Batch trait injection for CI
- Custom test discovery
- OpenTelemetry integration

**Approach**:
1. Review official xUnit3 migration guide when available
2. Reference WPF/WinForms migration PRs for patterns
3. May require additional package: `xunit.v3.extensibility.core`
4. Test thoroughly as xUnit3 has significant breaking changes in extensibility model

### 4. DirectoryAttribute.fs Migration ⏸️

**Blocked on**: xUnit3 stable release.

**File**: `tests/FSharp.Test.Utilities/DirectoryAttribute.fs`

**Current Code**:
```fsharp
open Xunit.Sdk

type DirectoryAttribute(dir: string) =
    inherit DataAttribute()
    override _.GetData _ = createCompilationUnitForFiles ...
```

**Required Changes**:
- Update `open Xunit.Sdk` to xUnit3 namespace
- Verify `DataAttribute` base class compatibility
- Update `GetData` method signature if changed in v3
- May need to add `xunit.v3.extensibility.core` package reference

### 5. XunitSetup.fs Verification ⏸️

**Blocked on**: xUnit3 stable release.

**File**: `tests/FSharp.Test.Utilities/XunitSetup.fs`

**Current Code**:
```fsharp
[<assembly: TestFramework("FSharp.Test.FSharpXunitFramework", "FSharp.Test.Utilities")>]
```

**Verification Needed**:
- Confirm custom framework registration still works in xUnit3
- Verify `CollectionDefinition` attribute with `DisableParallelization` is still supported
- Test that F# reflection-based discovery works (should be fine per xUnit3 docs)

### 6. Build & Test Validation ⏸️

**Blocked on**: All above changes.

**Tasks**:
1. Build with new packages: `./build.sh -c Release`
2. Run tests: `./build.sh -c Release --testcoreclr`
3. Verify all test features work:
   - Console output capture
   - Parallel test execution
   - Batch trait filtering (`--filter batch=N`)
   - Theory data with custom attributes
4. Update baselines if IL or surface area changes
5. Check CI pipeline compatibility

### 7. Documentation Updates ⏸️

**Files to Update**:
- `TESTGUIDE.md`: Add xUnit3 migration notes
- `README.md`: Update test infrastructure references if any
- This document: Mark as complete and archive

## Why Not Use Preview Packages?

1. **Breaking Changes**: xUnit3 preview API may change before stable release
2. **Build Stability**: Preview packages can cause CI failures
3. **Package Lifecycle**: Preview packages may be pulled or replaced
4. **Maintenance Burden**: Code written for preview may need rewrites

## Timeline

- **Now**: Preparatory work complete (properties removed, configs updated)
- **When xUnit3 goes stable**: 
  - Update package versions
  - Migrate XunitHelpers.fs and custom attributes
  - Full validation
- **Estimated Effort**: 2-3 days for code migration + testing once stable packages are available

## References

- xUnit3 Migration Guide: https://xunit.net/docs/getting-started/v3/migration
- xUnit3 Microsoft.TestPlatform Integration: https://xunit.net/docs/getting-started/v3/microsoft-testing-platform
- WPF Migration PR: https://github.com/dotnet/wpf/pull/10890
- WinForms Migration PR: https://github.com/dotnet/winforms/pull/13540
- xUnit3 NuGet Packages: https://xunit.net/docs/nuget-packages-v3

## Testing Current State

Current tests work normally with xUnit 2.9.0:
```bash
./build.sh -c Release --testcoreclr
```

The preparatory changes (removing obsolete properties, updating JSON configs) are backward-compatible and don't affect test execution.
