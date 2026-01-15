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
