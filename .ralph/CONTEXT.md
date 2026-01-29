# Product Increments

This file is updated after each sprint completes. Use it to understand what was delivered.

---

## Sprint 1: Foundation: Create TFM file and
  MSBuild properties

**Summary:** Completed in 2 iterations

**Files touched:** Check git log for details.

---

## Sprint 2: MSBuild consumers: Update props files

**Summary:** Updated MSBuild files to use $(FSharpNetCoreProductDefaultTargetFramework) instead of hardcoded 'net10.0'

**Files touched:**
- UseLocalCompiler.Directory.Build.props - replaced 3 occurrences of net10.0
- buildtools/checkpackages/Directory.Build.props - added TFM property reading
- buildtools/checkpackages/FSharp.Compiler.Service_notshipped.fsproj - uses TFM property
- buildtools/checkpackages/FSharp.Core_notshipped.fsproj - uses TFM property

**Verification:**
- dotnet msbuild -getProperty:FSharpNetCoreProductDefaultTargetFramework returns "net10.0"
- dotnet msbuild -getProperty:TargetFramework returns "net10.0"
- No hardcoded net10.0 remains in modified files

---

## Sprint 2: MSBuild consumers: Update props files

**Summary:** Completed in 2 iterations

**Files touched:** Check git log for details.

---

## Sprint 3: Scripts: Update PowerShell and Bash scripts

**Summary:** Updated build scripts to read TFM from eng/productTfm.txt instead of hardcoded values

**Files touched:**
- eng/Build.ps1 - reads TFM from productTfm.txt for $coreclrTargetFramework, $bootstrapTfm, $fsharpNetCoreProductTfm
- eng/build.sh - reads TFM using `cat "$scriptroot/productTfm.txt" | tr -d '[:space:]'`
- eng/build-utils.ps1 - reads TFM from productTfm.txt for $fsharpNetCoreProductTfm

**Verification:**
- `./build.sh --help` executes without errors (exit code 0)
- No hardcoded 'net10.0' remains in any of the three script files
- TFM file reading works correctly: `cat eng/productTfm.txt | tr -d '[:space:]'` returns "net10.0"

---

## Sprint 3: Scripts: Update
  PowerShell and Bash scripts

**Summary:** Completed in 2 iterations

**Files touched:** Check git log for details.

---

## Sprint 4: Product code: Update CompilerLocation.fs

**Summary:** Updated toolingCompatibleVersions in CompilerLocation.fs to dynamically generate TFM list from FSharp.BuildProperties.fsProductTfmMajorVersion instead of hardcoded values.

**Files touched:**
- src/Compiler/Facilities/CompilerLocation.fs - replaced hardcoded TFM list with computed list

**Changes Made:**
- The list now generates TFMs from `fsProductTfmMajorVersion` (e.g., 10) down to 5
- Legacy netcoreapp/netstandard versions are appended after the computed list
- Fallback to major version 10 if parsing fails

**Verification:**
- dotnet build src/Compiler/FSharp.Compiler.Service.fsproj succeeds
- No hardcoded 'net10.0' or 'net11.0' in CompilerLocation.fs
- Uses FSharp.BuildProperties.fsProductTfmMajorVersion at line 128
- When TFM changes in eng/productTfm.txt, the list will automatically update

---

## Sprint 4: Product code: Update CompilerLocation.fs

**Summary:** Completed in 2 iterations

**Files touched:** Check git log for details.

---

## Sprint 5: Test utilities: Add productTfm constant

**Summary:** Added productTfm constant to TestFramework.fs and updated all test utility files to use it instead of hardcoded 'net10.0'

**Files touched:**
- tests/FSharp.Test.Utilities/TestFramework.fs - added productTfm constant reading from eng/productTfm.txt, updated dotnetArchitecture
- tests/FSharp.Test.Utilities/CompilerAssert.fs - uses productTfm in runtimeconfig generation
- tests/FSharp.Test.Utilities/ProjectGeneration.fs - uses productTfm in fsproj template
- tests/FSharp.Test.Utilities/Utilities.fs - uses productTfm for target framework

**Verification:**
- dotnet build tests/FSharp.Test.Utilities/FSharp.Test.Utilities.fsproj succeeds
- No hardcoded 'net10.0' in any of the four files (except documentation comment)
- Test utilities compile successfully

---

## Sprint 5: Test utilities: Add
  productTfm constant

**Summary:** Completed in 2 iterations

**Files touched:** Check git log for details.

---

## Sprint 6: Test files: Update remaining test references

**Summary:** Updated test files to use centralized TFM via TestFramework.productTfm

**Files touched:**
- tests/fsharp/single-test.fs - replaced 3 occurrences of "net10.0" with productTfm
- tests/FSharp.Compiler.Private.Scripting.UnitTests/DependencyManagerInteractiveTests.fs - replaced 18 occurrences of "net10.0" with TestFramework.productTfm

**Verification:**
- dotnet build tests/fsharp/FSharpSuite.Tests.fsproj succeeds with 0 errors
- dotnet build tests/FSharp.Compiler.Private.Scripting.UnitTests/FSharp.Compiler.Private.Scripting.UnitTests.fsproj succeeds with 0 errors
- No hardcoded 'net10.0' in single-test.fs
- No hardcoded 'net10.0' in DependencyManagerInteractiveTests.fs
- Remaining net10.0 in tests/ are only in: utility scripts, MSBuild config files, comments, and notebooks (not functional test code)

---
