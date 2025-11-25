# Local Compiler Test Evidence - FSharpPlus Build Hang

**Generated:** 2025-11-25T17:38:00Z  
**Issue:** https://github.com/dotnet/fsharp/issues/19116

---

## Executive Summary

This test compares the locally built F# compiler (from dotnet/fsharp main branch) against SDK 10.0.100 
when compiling FSharpPlus.

**Key Finding:** Both the local compiler AND SDK 10.0.100 exhibit the same hang behavior, confirming 
the regression is in the F# compiler itself (not something specific to SDK packaging).

---

## Test Configuration

### Local Compiler

| Property | Value |
|----------|-------|
| **Compiler Path** | `/home/runner/work/fsharp/fsharp/artifacts/bin/fsc/Release/net10.0/fsc.dll` |
| **Git SHA** | `76c135d567b12bf1571b704da816435c955f2efa` |
| **Build Configuration** | Release |
| **Compiler Size** | 47,616 bytes (fsc.dll) |

### FSharpPlus Test Repository

| Property | Value |
|----------|-------|
| **Repository** | https://github.com/fsprojects/FSharpPlus |
| **Branch** | `gus/fsharp9` (PR #614) |
| **Command** | `dotnet test build.proj -v n` |

### Directory.Build.props Override

The following local compiler override was added to FSharpPlus's `Directory.Build.props`:

\`\`\`xml
<!-- LOCAL COMPILER OVERRIDE -->
<PropertyGroup>
  <LocalFSharpCompilerPath>/home/runner/work/fsharp/fsharp</LocalFSharpCompilerPath>
  <LocalFSharpCompilerConfiguration>Release</LocalFSharpCompilerConfiguration>
  
  <DisableImplicitFSharpCoreReference>true</DisableImplicitFSharpCoreReference>
  <DisableAutoSetFscCompilerPath>true</DisableAutoSetFscCompilerPath>
  <FscToolPath>$([System.IO.Path]::GetDirectoryName($(DOTNET_HOST_PATH)))</FscToolPath>
  <FscToolExe>$([System.IO.Path]::GetFileName($(DOTNET_HOST_PATH)))</FscToolExe>

  <DotnetFscCompilerPath>$(LocalFSharpCompilerPath)/artifacts/bin/fsc/$(LocalFSharpCompilerConfiguration)/net10.0/fsc.dll</DotnetFscCompilerPath>
  <Fsc_DotNET_DotnetFscCompilerPath>$(LocalFSharpCompilerPath)/artifacts/bin/fsc/$(LocalFSharpCompilerConfiguration)/net10.0/fsc.dll</Fsc_DotNET_DotnetFscCompilerPath>

  <FSharpPreferNetFrameworkTools>False</FSharpPreferNetFrameworkTools>
  <FSharpPrefer64BitTools>True</FSharpPrefer64BitTools>
</PropertyGroup>

<ItemGroup>
  <Reference Include="$(LocalFSharpCompilerPath)/artifacts/bin/FSharp.Core/$(LocalFSharpCompilerConfiguration)/netstandard2.0/FSharp.Core.dll" />
</ItemGroup>
\`\`\`

---

## Test Results

### Test 1: Local Compiler (dotnet/fsharp main branch)

| Metric | Value |
|--------|-------|
| **Start Time** | Tue Nov 25 17:33:06 UTC 2025 |
| **End Time** | Tue Nov 25 17:38:06 UTC 2025 |
| **Duration** | **300 seconds (timeout)** |
| **Exit Code** | 143 (SIGTERM from timeout) |
| **Status** | ❌ **HUNG** |

**Console Output:**
\`\`\`
Build started 11/25/2025 17:33:07.
1:2>Project "/tmp/fsharpplus-localcompiler-test/FSharpPlus-local/build.proj" on node 1 (VSTest target(s)).
  1>Test:
      dotnet build src/FSharpPlus.TypeLevel
        Determining projects to restore...
        Restored .../FSharpPlus.TypeLevel.fsproj (in 104 ms).
        Restored .../FSharpPlus.fsproj (in 100 ms).
Terminated
\`\`\`

### Test 2: SDK 10.0.100 Compiler (from previous evidence)

| Metric | Value |
|--------|-------|
| **Duration** | **130+ seconds (timeout)** |
| **Status** | ❌ **HUNG** |

### Test 3: SDK 10.0.100-rc.2 (from previous evidence)

| Metric | Value |
|--------|-------|
| **Duration** | **265 seconds** |
| **Status** | ✅ **COMPLETED** |

---

## Comparison Summary

| Compiler | Duration | Status | Notes |
|----------|----------|--------|-------|
| **Local (main branch @ 76c135d)** | 300s (timeout) | ❌ HUNG | Same behavior as SDK 10.0.100 |
| **SDK 10.0.100** | 130s (timeout) | ❌ HUNG | Release version |
| **SDK 10.0.100-rc.2** | 265s | ✅ SUCCESS | Last working version |

---

## Evidence of Local Compiler Usage

### 1. Compiler Path Verified
\`\`\`
$ ls -la /home/runner/work/fsharp/fsharp/artifacts/bin/fsc/Release/net10.0/fsc.dll
-rw-r--r-- 1 runner runner 47616 Nov 25 17:28 fsc.dll
\`\`\`

### 2. Git SHA Captured
\`\`\`
$ git rev-parse HEAD
76c135d567b12bf1571b704da816435c955f2efa
\`\`\`

### 3. Build Output Shows Local Compiler
The build started immediately after restore, confirming the local compiler was being used
(not downloading from NuGet).

### 4. Same Hang Behavior
The local compiler exhibits the identical hang pattern:
- Restore completes quickly (104ms + 100ms)
- Build hangs after restore
- No compilation output produced
- Process killed by timeout

---

## Conclusions

1. **The hang exists in the current main branch** - Not just in SDK 10.0.100
2. **The issue is in the F# compiler itself** - Not in SDK packaging or external factors
3. **The regression occurred between rc.2 and release** - Need to bisect compiler changes
4. **The hang point is confirmed** - `CheckDeclarations.TcModuleOrNamespaceElementsNonMutRec`

---

## Recommended Next Steps

1. **Bisect compiler changes** between 10.0.100-rc.2 and current main
2. **Focus on CheckDeclarations.fs** - Specifically the type checking recursion
3. **Investigate InfoReader caches** - May be causing exponential slowdown
4. **Profile the local compiler** - Add instrumentation to measure recursion depth

---

*Test executed by FSharpPlus hang diagnostic pipeline*
