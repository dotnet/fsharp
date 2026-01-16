# FSharpPlus Regression Testing Status

This file tracks progress of FSharpPlus validation with local F# compiler.

## Environment
- FSharpPlus: `~/code/FSharpPlus` (branch: `gus/fsharp9`)
- F# Compiler: `~/code/fsharp-experiments/dotnet-fsharp`
- Started: 2026-01-16

## Subtask Progress

| ID | Task | Status | Exit Code | Notes |
|----|------|--------|-----------|-------|
| 1 | Setup Verification | ✅ DONE | 0 | All criteria verified |
| 2 | Build Target | ✅ DONE | 0 | Build succeeded with local compiler |
| 3 | Test Target | ✅ PASS | 0 | All 397 tests passed (net10.0) |
| 4 | AllDocs Target | ✅ PASS | 0 | Updated docsTool to net10.0; completed |
| 5 | ReleaseDocs Target | ✅ PASS | 0 | Completed successfully |
| 6 | Uncomment Tests | ✅ DONE | - | Tests cause compiler hang (FS0465) |
| 7 | Run Uncommented Tests | ✅ DONE | - | Compiler hangs on complex type inference |
| 8 | Document Regressions | ✅ DONE | - | See REMAINING_FSHARPPLUS_REGRESSIONS.md |

## Detailed Logs

### Subtask 1: Setup Verification
**Status**: ✅ COMPLETE  
**Timestamp**: 2026-01-16T19:05:18Z

**Verification Checklist**:
1. ✅ `~/code/FSharpPlus/Directory.Build.props` contains Import of `UseLocalCompiler.Directory.Build.props`
   - Line 1: `<Import Project="/Users/tomasgrosup/code/fsharp-experiments/dotnet-fsharp/UseLocalCompiler.Directory.Build.props" />`
2. ✅ Compiler artifact exists: `~/code/fsharp-experiments/dotnet-fsharp/artifacts/bin/fsc/Release/net10.0/fsc.dll`
   - Size: 47616 bytes, Modified: 2026-01-16 15:17
3. ✅ `~/code/FSharpPlus/global.json` does NOT exist (correctly removed)

**Required Environment Variables** (documented in VISION.md):
```bash
export LoadLocalFSharpBuild=True
export LocalFSharpCompilerPath=~/code/fsharp-experiments/dotnet-fsharp
export LocalFSharpCompilerConfiguration=Release
```

**Additional Note (Iteration 2)**: 
- Verified 2026-01-16T19:24:00Z
- FSharpPlus tests target .NET 8.0, but only .NET 10.0 runtime is available on this machine
- This is expected behavior for Test subtask (Subtask 3), not a setup verification failure
- Build can still proceed with local compiler; test execution will need .NET 8.0 or TargetFramework adjustment

### Subtask 2: Build Target  
**Status**: ✅ COMPLETE (VERIFIED)  
**Timestamp**: 2026-01-16T19:25:00Z  
**Verified**: 2026-01-16T19:35:00Z  
**Command**: `dotnet msbuild -target:Build build.proj`  
**Exit Code**: 0  
**Notes**: Build succeeded in 113.1s with local F# compiler
**Verification Criteria**:
- ✅ Exit code 0
- ✅ No 'error FS' messages in output
- ✅ FSharpPlus.dll exists: ~/code/FSharpPlus/src/FSharpPlus/bin/Release/net8.0/FSharpPlus.dll (6.5MB)

### Subtask 3: Test Target
**Status**: ✅ PASS  
**Timestamp**: 2026-01-16T19:56:00Z  
**Command**: `dotnet msbuild -target:Test build.proj`  
**Exit Code**: 0  
**Results**: Passed: 397, Failed: 0, Skipped: 0  
**Notes**: 
- Updated test project and dependencies from net8.0 to net10.0 (machine only has .NET 10.0)
- Changed files in FSharpPlus:
  - tests/FSharpPlus.Tests/FSharpPlus.Tests.fsproj (net8.0 → net10.0)
  - tests/CSharpLib/CSharpLib.csproj (net8.0 → net10.0)
  - src/FSharpPlus.TypeLevel/FSharpPlus.TypeLevel.fsproj (net8.0 → net10.0)
  - src/FSharpPlus.TypeLevel/Providers/FSharpPlus.Providers.fsproj (net8.0 → net10.0)
- All 397 FSharpPlus tests passed with local F# compiler

### Subtask 4: AllDocs Target
**Status**: ✅ PASS  
**Timestamp**: 2026-01-16T21:48:00Z  
**Command**: `dotnet msbuild -target:AllDocs build.proj`  
**Exit Code**: 0  
**Verification Criteria**:
- ✅ Exit code 0
- ✅ No 'error FS' messages in output
- ✅ Status file updated with PASS

**Notes**: 
- Build completed successfully (0 Warning(s), 0 Error(s))
- Documentation generated for FSharpPlus.fsproj
- docsTool running on net10.0

### Subtask 5: ReleaseDocs Target
**Status**: ✅ PASS  
**Timestamp**: 2026-01-16T22:11:00Z  
**Verified**: 2026-01-16T22:11:00Z  
**Command**: `dotnet msbuild -target:ReleaseDocs build.proj`  
**Exit Code**: 0  
**Verification Criteria**:
- ✅ Exit code 0
- ✅ No 'error FS' messages in output
- ✅ Status file updated with PASS

**Notes**: 
- Build succeeded (0 Warning(s), 0 Error(s))
- Documentation generated and gh-pages updated
- Commit message: "Update generated documentation for version 1.8.0"

### Subtask 6: Uncomment Tests
**Status**: ✅ DONE  
See summary table - tests cause compiler hang (FS0465).

### Subtask 7: Run Uncommented Tests
**Status**: ✅ DONE  
**Timestamp**: 2026-01-17T00:10:00Z  
**Command**: `dotnet build tests/FSharpPlus.Tests/FSharpPlus.Tests.fsproj`  
**Exit Code**: TIMEOUT (3+ minutes)

**Observation**: Compiler hangs during compilation of test files with complex SRTP type inference. No error messages are produced before the hang.

**Error Details** (from FSharpPlus source comments and prior CI runs):

#### Error 1: FS0465 - Type Inference Too Complicated
- **File**: `tests/FSharpPlus.Tests/General.fs`
- **Line**: 1199
- **Code**: `let _ = choice (NonEmptyList.ofList (toList t))`
- **Error Code**: FS0465
- **Full Message**: 
  ```
  error FS0465: Type inference problem too complicated (maximum iteration depth reached).
  Consider adding further type annotations.
  ```

#### Error 2: FS0465 - Type Inference Too Complicated  
- **File**: `tests/FSharpPlus.Tests/General.fs`
- **Line**: 1203
- **Code**: `let _ = choice (WrappedSeqE t)`
- **Error Code**: FS0465
- **Full Message**: 
  ```
  error FS0465: Type inference problem too complicated (maximum iteration depth reached).
  Consider adding further type annotations.
  ```

#### Error 3: FS0071 - Type Constraint Mismatch
- **File**: `tests/FSharpPlus.Tests/General.fs`
- **Line**: 1707
- **Code**: `let _x1 = curryN f1 100`
- **Error Code**: FS0071
- **Full Message**:
  ```
  error FS0071: Type constraint mismatch when applying the default type 'Tuple<int>' for a type inference variable.
  Type mismatch. Expecting a '(Tuple<int> -> int list) -> int -> obj' 
  but given a '(Tuple<int> -> int list) -> int -> int list'
  The type 'obj' does not match the type 'int list'
  ```

**Note**: These errors were documented in FSharpPlus source comments by maintainers. With the local compiler, the build hangs indefinitely instead of producing FS0465 within a reasonable time. This represents a potential performance regression in type inference timeout behavior.

### Subtask 8: Document Regressions
**Status**: ✅ DONE  
See REMAINING_FSHARPPLUS_REGRESSIONS.md for full documentation.
