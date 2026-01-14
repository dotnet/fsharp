# FSharpQA Migration - VISION

## High-Level Goal
Migrate 1680 tests from the legacy `tests/fsharpqa` Perl-based test suite to `tests/FSharp.Compiler.ComponentTests` using the existing test infrastructure (Compiler.fs, DirectoryAttribute, FileInlineDataAttribute).

## Key Design Decisions

### 1. Use Existing Infrastructure (Don't Reinvent)
- **Compiler.fs** provides all compilation helpers: `typecheck`, `compile`, `shouldFail`, `shouldSucceed`, `withDiagnostics`, `withErrorCode`, etc.
- **DirectoryAttribute** allows batch-testing all files in a directory with baselines
- **FileInlineDataAttribute** allows single-file tests with compile options
- **No new test framework needed** - everything exists in FSharp.Test.Utilities

### 2. Git-Move Source Files (Preserve History & Review)
- Source files (`.fs`, `.fsx`, `.fsi`) are **git-moved unchanged** to `resources/tests/[path]/`
- This preserves line numbers (Expects spans remain valid)
- Clean PR review shows renames, not edits
- `<Expects>` comments stay in files for reference

### 3. Test Generation Pattern
For each test in `env.lst`:
```fsharp
[<Theory; Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/Diagnostics/async", Includes=[|"file.fs"|])>]
let ``test name`` compilation =
    compilation
    |> asFsx  // if FSI mode
    |> withOptions ["--warnaserror+"; "--test:ErrorRanges"]
    |> typecheck  // fastest method
    |> shouldFail
    |> withErrorCode 0025
    |> ignore
```

### 4. env.lst Parsing
Each line in `env.lst` defines a test:
- `SOURCE=file.fs` → file to compile
- `SCFLAGS="..."` → compiler options → `withOptions [...]`
- `COMPILE_ONLY=1` → use `typecheck` (fastest)
- `FSIMODE=EXEC` → use `asFsx`

### 5. Expects Parsing
Comments in source files define expected diagnostics:
- `<Expects status="error" id="FS0025" span="(10,10-10,22)">message</Expects>`
- Maps to: `withDiagnostics [(Error 25, Line 10, Col 10, Line 10, Col 22, "message")]`
- Or simplified: `withErrorCode 25` + `withDiagnosticMessageMatches "pattern"`

## Important Context for Subtasks

### Files to Reference
1. **FSHARPQA_MIGRATION.md** - Master spec with all package definitions and patterns
2. **FEATURE_MAPPING.md** - Mapping from fsharpqa patterns to ComponentTests equivalents
3. **tests/FSharp.Test.Utilities/Compiler.fs** - All test helpers (2000+ lines)
4. **tests/FSharp.Test.Utilities/DirectoryAttribute.fs** - Batch test attribute
5. **tests/FSharp.Test.Utilities/FileInlineDataAttribute.fs** - Single file test attribute

### Current State
- 1680 tests remaining in fsharpqa
- Some async tests already partially migrated (4 tests in async.fs, 18 files in resources)
- Need to complete remaining tests and delete fsharpqa folders

### Workflow Per Package
1. Parse `env.lst` to get test definitions
2. `git mv` source files to `resources/tests/[path]/`
3. Create test `.fs` file with test cases
4. Add `<Compile Include="...">` to .fsproj
5. Build and verify tests run
6. Delete fsharpqa source folder after verification

## Constraints and Gotchas

### Platform Classification
- Most tests are CrossPlatform (default)
- Mark WindowsOnly/DesktopOnly tests with traits and skip checks
- Agent runs on macOS - tests should skip gracefully, not error

### Don't Break Existing Tests
- ComponentTests already have many tests
- Add to existing structure, don't duplicate
- Use existing resource paths: `resources/tests/[category]/[folder]/`

### Build System
- Run `dotnet build tests/FSharp.Compiler.ComponentTests` to verify
- Build currently broken on main (FSharp.Core version conflict) - pre-existing issue
- Focus on correctness, not fixing unrelated build issues

## Lessons from Previous Attempts

### What Worked
- Using DirectoryAttribute with Includes for selective file testing
- Pattern: `|> typecheck |> shouldFail |> withErrorCode N` for simple diagnostics
- Git-moving files preserves review quality

### What Didn't Work
- Trying to match exact message text (use regex patterns instead)
- Creating new test framework pieces (use existing Compiler.fs)
- Large monolithic migrations (split into small packages)

## Priority Order
1. Small packages first (DIAG-NONTERM, DIAG-PARSINGEOF)
2. CompilerOptions (straightforward, isolated)
3. Diagnostics (larger but well-structured)
4. Conformance (largest, save for later)
5. Complex packages last (INTERACTIVE, IMPORT)
