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
