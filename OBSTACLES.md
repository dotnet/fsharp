# Obstacles Encountered

This document tracks any obstacles encountered during the implementation of the Azure DevOps regression testing template.

## Current Obstacles

None at this time.

## Resolved Obstacles

### 1. FSharpPlus Build Failure: `_GetRestoreSettingsPerFramework` Target Missing

**Date:** 2025-11-28

**Symptom:**
```
D:\a\_work\1\TestRepo\src\FSharpPlus.TypeLevel\Providers\FSharpPlus.Providers.fsproj : error MSB4057: The target "_GetRestoreSettingsPerFramework" does not exist in the project. [TargetFramework=net8.0]
```

**Root Cause:**
The artifacts were being downloaded to an incorrect path structure. The UseLocalCompiler.Directory.Build.props file expects:
```xml
<LocalFSharpBuildBinPath>$(LocalFSharpCompilerPath)/artifacts/bin/fsc/$(LocalFSharpCompilerConfiguration)/net10.0</LocalFSharpBuildBinPath>
```

But the artifacts were being downloaded to:
```
$(Pipeline.Workspace)/FSharpCompiler/bin/fsc  (missing /artifacts/)
```

This caused the FSharpTargetsShim property to point to a non-existent path, which prevented Microsoft.FSharp.Targets from being imported. Without Microsoft.FSharp.Targets, Microsoft.Common.targets was not imported, which in turn prevented NuGet.targets from being imported - and NuGet.targets is where `_GetRestoreSettingsPerFramework` is defined.

**Investigation Steps:**
1. Used `dotnet msbuild -pp` to compare preprocessed output with and without local compiler
2. Found NuGet.targets was referenced 9 times without local compiler, but only 2 times with local compiler
3. Traced the import chain: FSharpTargetsShim -> Microsoft.FSharp.NetSdk.targets -> Microsoft.FSharp.Targets -> Microsoft.Common.targets -> NuGet.targets
4. Discovered FSharpTargetsShim was pointing to path with doubled `/artifacts/artifacts/`
5. Realized artifact download path didn't match UseLocalCompiler.Directory.Build.props expectations

**Solution:**
Changed the artifact download paths to include `/artifacts/bin/`:
```yaml
downloadPath: '$(Pipeline.Workspace)/FSharpCompiler/artifacts/bin/fsc'
downloadPath: '$(Pipeline.Workspace)/FSharpCompiler/artifacts/bin/FSharp.Core'
```

This ensures the directory structure matches what UseLocalCompiler.Directory.Build.props expects.

**Lessons Learned:**
- UseLocalCompiler.Directory.Build.props has specific path expectations that must be matched exactly
- Use `dotnet msbuild -pp` to debug MSBuild import issues
- The `_GetRestoreSettingsPerFramework` error often indicates broken MSBuild import chain

### 2. Git Submodule Initialization

**Date:** 2025-11-28

**Symptom:**
Type provider source files were missing.

**Root Cause:**
FSharpPlus uses git submodules for `FSharp.TypeProviders.SDK`.

**Solution:**
1. Changed `git clone` to `git clone --recursive` to clone with submodules
2. Added explicit `git submodule update --init --recursive` after `git checkout`

### 3. No Binary Log Files Generated

**Date:** 2025-11-28

**Symptom:**
No `.binlog` files were being collected or published.

**Root Cause:**
The build was failing early before any MSBuild commands could generate binlog files.

**Solution:**
Fixing the path issues above allows the build to proceed, enabling binlog generation.
