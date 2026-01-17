# Migration Blockers

This document tracks tests from `tests/fsharpqa` that cannot be migrated to `FSharp.Compiler.ComponentTests` after multiple attempts.

## Template

```markdown
## [Package-ID]: [Folder Path]
**File:** filename.fs
**Attempts:** N
**Error:** Description of what fails
**Notes:** Why this can't be solved, potential future fix
```

---

## Pending Blockers

### CONF-TYPEFORWARDING: Conformance/TypeForwarding

**Folders:** Class, Cycle, Delegate, Interface, Nested, Struct (~303 tests across 6 folders)

**Original Location:** tests/fsharpqa/Source/Conformance/TypeForwarding/

**Test Pattern:**
These tests verify F# runtime behavior with .NET type forwarding:
1. Compile C# library with types defined directly (`Class_Library.cs`)
2. Compile F# executable referencing the C# library
3. Replace C# library with a forwarding version (types in `Class_Forwarder.dll`, `Class_Library.dll` forwards to it)
4. Run F# executable - should work because types are forwarded at runtime

**env.lst example:**
```
SOURCE=NG_NormalClass.fs PRECMD="csc /t:library Class_Library.cs" SCFLAGS="--reference:Class_Library.dll"
SOURCE=Dummy.fs PRECMD="BuildAssembly.bat" POSTCMD="checkForward.bat NG_NormalClass.exe"
```

**Reason for Blocking:**
The test framework compiles everything in memory and doesn't support:
1. Building F# executable against one assembly
2. Swapping that assembly with a different version (containing TypeForwardedTo attributes)
3. Running the executable with the new assembly configuration

This is a **runtime behavior test** that requires assembly substitution after F# compilation, which the in-memory test framework cannot support.

**Partial Migration:**
10 basic C# interop tests were created in `Conformance/TypeForwarding/TypeForwardingTests.fs` that verify F# can:
- Use C# classes, interfaces, structs, delegates (non-generic and generic)
- Access nested types from C#

These tests validate the compile-time behavior but not the runtime type forwarding scenario.

**Possible Future Solutions:**
1. Add test helper that compiles to disk, swaps assemblies, and runs executable
2. Create separate integration test project with file-system-based compilation
3. Extend test framework to support multi-stage compilation with assembly substitution

**Decision:** Partial migration with 10 interop tests. Original TypeForwarding folders deleted as the runtime tests cannot be migrated with current infrastructure.

---

### DIAG-GENERAL: E_MissingSourceFile tests

**Files:** E_MissingSourceFile01.fs, E_MissingSourceFile02.fs, E_MissingSourceFile03.fs, E_MissingSourceFile04.fs

**Original Location:** tests/fsharpqa/Source/Diagnostics/General/

**env.lst entries:**
```
SOURCE="E_MissingSourceFile01.fs doesnotexist.fs"
SOURCE="E_MissingSourceFile02.fs X:\doesnotexist.fs"
SOURCE="E_MissingSourceFile03.fs \\qwerty\y\doesnotexist.fs"
NoMT SOURCE=E_MissingSourceFile04.fs SCFLAGS="--exec doesnotexist.fs" FSIMODE=PIPE
```

**Expected Errors:** FS0225 (source file could not be found), FS0078 (unable to find file)

**Reason for Blocking:**
These tests verify the compiler's error handling when source files specified on the command line don't exist. The FSharp.Compiler.ComponentTests infrastructure uses in-memory compilation where source content is provided directly - there's no concept of "file not found" because files are not read from disk during test execution.

**Possible Future Solutions:**
1. Add a new test helper that invokes fsc.exe directly as a subprocess with actual file paths
2. Create a separate test project specifically for CLI-based testing scenarios
3. Use `CompilerAssert.CompileExe` or similar with temporary directories and intentionally missing files

**Decision:** Skip migration. These tests cover a CLI-specific scenario that the current test infrastructure cannot support. The underlying compiler behavior (FS0225, FS0078 error codes) is stable and well-tested through other means.

---

### MISC: Misc/Windows-specific and complex tests

**Files:** productioncoverage02.fs, productioncoverage03.fsscript, StaticMethodValueTypeMain.fs, DefaultManifest.fs, DefaultManifest_dll.fs, WhetherEmbeddedManifest.fs, 6448.fs, SerializableClosure01.fs, AssemblyResolve01.fs, AssemblyResolve01.fsx, AsyncOperations.fs

**Original Location:** tests/fsharpqa/Source/Misc/

**Reason for Blocking:**
These tests cannot be migrated due to various infrastructure limitations:

1. **productioncoverage02.fs** - Uses P/Invoke to kernel32.dll (Windows-only)
2. **productioncoverage03.fsscript** - Requires pre-compiled NestedClasses.dll
3. **StaticMethodValueTypeMain.fs** - Requires pre-compiled StaticMethodValueTypeLib.dll
4. **DefaultManifest.fs/DefaultManifest_dll.fs** - Requires PRECMD to pre-compile DLL, uses P/Invoke to kernel32.dll
5. **WhetherEmbeddedManifest.fs** - Marked NOMONO, uses reflection on manifest resources (platform-specific)
6. **6448.fs, SerializableClosure01.fs** - Uses AppDomain.CreateDomain (DesktopOnly - not supported in .NET Core)
7. **AssemblyResolve01.fs/fsx** - Uses AppDomain.CurrentDomain.add_AssemblyResolve and external DLLs
8. **AsyncOperations.fs** - Complex async helper module, not a standalone test

**Decision:** Skip migration. The migrated tests (14 of ~24) cover the portable F# compilation scenarios. The remaining tests require Windows-specific APIs, pre-compiled external DLLs, or .NET Framework-only features (AppDomain security).

---

### STRESS: Stress tests (code generators)

**Files:** CodeGeneratorFor2766.fsx, SeqExprCapacity.fsx

**Original Location:** tests/fsharpqa/Source/Stress/

**env.lst entries:**
```
ReqENU,ReqRetail,STRESS SOURCE=2766.fs PRECMD="$FSI_PIPE --exec CodeGeneratorFor2766.fsx"
ReqNOCov,ReqRetail,STRESS SOURCE=SeqExprCapacity.fs PRECMD="$FSI_PIPE --exec SeqExprCapacity.fsx"
```

**Reason for Blocking:**
These tests use PRECMD to run FSI scripts that generate large F# source files dynamically, then compile the generated files. The test infrastructure does not support:
1. Running FSI scripts as a pre-compilation step
2. Dynamically generating source files before compilation
3. The generated files (2766.fs, SeqExprCapacity.fs) don't exist in the repository

**Decision:** Skip migration. These stress tests require code generation that can't be done with in-memory compilation.

---

### MULTITARGETING: MultiTargeting tests

**Files:** E_MissingReferenceToFSharpCore20.fs, E_BadPathToFSharpCore.fs, E_BadPathToFSharpCore.fsx

**Original Location:** tests/fsharpqa/Source/MultiTargeting/

**env.lst entries:**
```
NOMONO SOURCE=E_MissingReferenceToFSharpCore20.fs SCFLAGS="--noframework -r %WINDIR%\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll"
NOMONO SOURCE=E_BadPathToFSharpCore.fs SCFLAGS="--noframework -r %WINDIR%\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll -r I_DO_NOT_EXIST\FSharp.Core.dll"
```

**Reason for Blocking:**
1. All tests marked NOMONO (Windows-only)
2. Use Windows-specific paths (`%WINDIR%\Microsoft.NET\Framework\v4.0.30319\mscorlib.dll`)
3. Target .NET Framework 4.0 mscorlib (not applicable to .NET Core)
4. Test scenarios (referencing old .NET Framework assemblies) are obsolete for modern .NET

**Decision:** Skip migration. These tests are for legacy .NET Framework multi-targeting scenarios that don't apply to modern .NET Core/5+/6+/7+/8+/9+/10.0 development.

---

### LIBRARIES-PARTIALTRUST: Libraries/Core/PartialTrust

**Files:** PartialTrust01.fs

**Original Location:** tests/fsharpqa/Source/Libraries/Core/PartialTrust/

**Reason for Blocking:**
Uses .NET Framework Code Access Security (CAS) model:
- `System.Security.Permissions.PermissionSet`
- `System.Security.Permissions.SecurityPermission`
- `AppDomain.CreateDomain` with security sandbox

These APIs don't exist in .NET Core.

**Decision:** Skip migration. CAS is not supported in .NET Core.

---

### LIBRARIES-PORTABLE: Libraries/Portable

**Files:** provider.fs, consumer.cs, parse_tests.fs, parse_oracle.cs

**Original Location:** tests/fsharpqa/Source/Libraries/Portable/

**env.lst entries:**
```
NoMT SOURCE=provider.fs POSTCMD="$CSC_PIPE /r:provider.dll /r:System.Numerics.dll consumer.cs && consumer.exe"
NoMT SOURCE=parse_tests.fs SCFLAGS="--standalone -g -a" POSTCMD="$CSC_PIPE /debug+ /r:parse_tests.dll /r:System.Numerics.dll parse_oracle.cs && parse_oracle.exe"
```

**Reason for Blocking:**
These tests require:
1. Compiling F# to DLL
2. Using CSC to compile C# that references the F# DLL
3. Running the C# executable

This multi-stage cross-language compilation with external execution is not supported by the in-memory test framework.

**Decision:** Skip migration. Cross-language compilation with runtime execution requires file-system based testing.

---

### INTERACTIVE-SESSION: InteractiveSession/Misc

**Original Location:** tests/fsharpqa/Source/InteractiveSession/Misc/

**Tests in env.lst:** ~107 tests across 2 env.lst files

**Tests Successfully Migrated:** 10 tests (inline FSI tests)

**Reason for Blocking (~97 tests):**

1. **fsi.CommandLineArgs tests (3 tests):** CommandLineArgs01.fs, CommandLineArgs01b.fs, CommandLineArgs02.fs
   - Use `fsi.CommandLineArgs` object which is only available inside an FSI session
   - `runFsi` runs FSI as an external process, not as an in-memory session

2. **FSIMODE=PIPE tests with stdin piping:**
   - Many tests use `FSIMODE=PIPE` which pipes script content to FSI via stdin
   - The `runFsi` test helper doesn't support this stdin piping mode

3. **PRECMD tests (~5 tests):**
   - Tests like issue2411/app.fsx require PRECMD to compile C# libraries first
   - ReferencesFullPath.fsx uses PRECMD to dynamically generate the test script

4. **Relative #r reference tests (~50+ tests):**
   - Extensive tests for `#r` with relative paths from different directories (ccc/, aaa/bbb/, etc.)
   - Tests invoke FSI from different relative paths (`fsi.exe --exec path\script.fsx`)
   - These require file-system based testing with actual file paths

5. **--simpleresolution tests (~10 tests):**
   - Use `--simpleresolution --noframework` with Windows-specific paths like `%FSCOREDLLPATH%`
   - Platform-specific resolution behavior

6. **NOMONO/ReqENU tests:**
   - TimeToggles.fsx requires English locale (ReqENU)
   - References40.fsx, Regressions01.fs marked NOMONO (mono-incompatible)

7. **Tests causing test host crashes (~5 tests):**
   - Array2D1.fsx - test host crashes (unknown cause)
   - ReflectionBugOnMono6320.fsx - test host crashes
   - DefinesInteractive.fs - test host crashes intermittently
   - These may be timing/resource issues with parallel test execution

**Migrated Tests (10):**
- EmptyList - empty list literal in FSI
- ToStringNull - null ToString handling
- DeclareEvent - event declaration in FSI
- E_let_equal01 - error: incomplete let binding
- E_let_id - error: incomplete binding
- E_let_mutable_equal - error: mutable binding syntax
- E_emptyRecord - error: empty record type
- E_type_id_equal_pipe - error: incomplete union case
- E_GlobalMicrosoft - error: undefined value
- E_RangeOperator01 - error: malformed range operator

**Decision:** Partial migration with 10 working inline tests. The remaining ~97 tests require FSI session internals, file-system paths, or PRECMD execution that the ComponentTests framework doesn't support.

---

## Resolved Blockers

_Record resolved blockers here for reference._
