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
FSharpPlus uses git submodules for `FSharp.TypeProviders.SDK`. The regression test was using `git clone` without the `--recursive` flag, which means submodules were not initialized. The TypeProviders project references source files from the submodule (`external/FSharp.TypeProviders.SDK/src/ProvidedTypes.fs` and `ProvidedTypes.fsi`), which didn't exist.

**Investigation Steps:**
1. Cloned FSharpPlus locally with `git clone --depth 1` (same as template)
2. Found `external/FSharp.TypeProviders.SDK/` directory was empty
3. Identified `.gitmodules` file referencing the submodule
4. Ran `git submodule update --init --recursive` which populated the directory
5. Confirmed the TypeProviders source files were now present

**Solution:**
1. Changed `git clone` to `git clone --recursive` to clone with submodules
2. Added explicit `git submodule update --init --recursive` after `git checkout` (in case submodules change between commits)

**Lessons Learned:**
- Always use `--recursive` flag when cloning repositories that may have submodules
- The `_GetRestoreSettingsPerFramework` error is often a symptom of missing project files, not an actual SDK issue

### 2. No Binary Log Files Generated

**Date:** 2025-11-28

**Symptom:**
No `.binlog` files were being collected or published, even though `MSBUILDBINARYLOGGERENABLED=true` was set.

**Root Cause:**
The build was failing early (due to the submodule issue above) before any MSBuild commands could generate binlog files. The `dotnet pack` command was failing at the project load stage, not during the actual build.

**Solution:**
Fixing the submodule initialization issue above allows the build to proceed past project loading, which enables binlog generation.

**Lessons Learned:**
- If no binlog files are generated, the build may be failing before MSBuild even starts
- Check for project file loading issues (missing files, invalid references) first
