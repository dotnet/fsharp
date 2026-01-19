# Shared Context

This file is updated after each subtask completes. Use it to understand what was done.

---

## Subtask 1: Add runFsiProcess and runFscProcess CLI helpers

**Summary:** Completed in 3 iterations

**Files touched:** Check git log for details.

---

## Subtask 2: Migrate FSI CLI tests

**Summary:** Migrated 7 FSI CLI tests that require subprocess execution

**Files touched:**
- `tests/FSharp.Compiler.ComponentTests/CompilerOptions/fsi/FsiCliTests.fs` (new)
- `tests/FSharp.Compiler.ComponentTests/FSharp.Compiler.ComponentTests.fsproj`

**Tests migrated:**
1. `-?` (shorthand help) - from help/env.lst
2. `--help` (long form) - from help/env.lst
3. `/?` (Windows-style) - from help/env.lst
4. `--nologo -?` (nologo variant) - from help/env.lst
5. `--langversion:?` (language version list) - from help baseline
6. `--highentropyva+` (unrecognized option) - from highentropyva/env.lst
7. `--subsystemversion:4.00` (unrecognized option) - from subsystemversion/env.lst

**Original sources:** git show eb1873ff3:tests/fsharpqa/Source/CompilerOptions/fsi/

---
