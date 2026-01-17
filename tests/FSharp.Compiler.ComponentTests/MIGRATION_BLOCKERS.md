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

## Resolved Blockers

_Record resolved blockers here for reference._
