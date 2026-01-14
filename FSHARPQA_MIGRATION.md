# FSharpQA Migration - Master Instructions

## Overview

Migrate the legacy `tests/fsharpqa` Perl-based test suite to a new xUnit test project `tests/FSharpQaMigratedTests` following the coding style of FSharp.Compiler.ComponentTests

**Guiding Principles:**
- Each package must have verification before cleanup
- Source files (`.fs`/`.fsx`/`.fsi`) are git-moved unchanged (preserves line numbers, enables clean PR review)
- Delete fsharpqa source folders only after successful verification
- No duplication between old and new locations
- Agents run on macOS; some tests will skip there but must not error
- Make individual commits for each atomic 
---

## Part 1: New Project Setup

Create `tests/FSharpQaMigratedTests/FSharpQaMigratedTests.fsproj` following the pattern of `FSharp.Compiler.ComponentTests.fsproj`:
- Reference `FSharp.Test.Utilities` 
- Reference same test framework packages (xUnit, etc.)

- Use existing test helpers, compiler, assert features etc. from Test Utilities.
- Do NOT recreate another test framework
Create `tests/FSharpQaMigratedTests/TestHelpers.fs`:


**Verification:**
```bash
cd tests/FSharpQaMigratedTests
dotnet build   # Must succeed
dotnet test    # Should show 0 tests initially
```

---

## Part 2: Tracking Documents

Maintain these files in `tests/FSharpQaMigratedTests/`:

### TEST_FRAMEWORK_ADDITIONS.md
Track functionality gaps in TestUtilities that need implementation:
```markdown
# Test Framework Additions Needed

## Pending
- [ ] Negative Diagnostic Assertion - `<Expects status="notin">internal error</Expects>`
- [ ] VB Compilation Helper

```

### MIGRATION_BLOCKERS.md
Track tests that cannot be migrated after multiple attempts:
```markdown
# Migration Blockers

## [Package-ID]:  [Folder Path]
**File:** filename.fs
**Attempts:** 3
**Error:** Description of what fails
**Notes:** Why this can't be solved, potential future fix
```

### FEATURE_MAPPING.md
Living document mapping fsharpqa patterns to ComponentTests equivalents:
```markdown
# FSharpQA to ComponentTests Feature Mapping

## Compiler Invocation

| FsharpQA | ComponentTests |
|----------|----------------|
| `SOURCE=file.fs` | `[<FileInlineData("file.fs")>]` + `getCompilation` |
| `SCFLAGS="--opt"` | `withOptions ["--opt"]` |
| `SCFLAGS="-a"` | `asLibrary` |
| `COMPILE_ONLY=1` | Use `typecheck` (fastest) or `compile` if IL needed |
| `FSIMODE=EXEC` | `asFsx` |

## Expected Results

| FsharpQA | ComponentTests |
|----------|----------------|
| `<Expects status="success">` | `shouldSucceed` |
| `<Expects status="error" id="FS0001">` | `shouldFail` + `withErrorCode 1` |
| `<Expects status="warning">` | `shouldSucceed` + `withDiagnostics [Warning...]` |
| `span="(5,1-10,15)"` | `Line 5, Col 1, Line 10, Col 15` |

## Multi-File Compilation

| FsharpQA | ComponentTests |
|----------|----------------|
| `SOURCE="a.fsi a.fs"` | `withAdditionalSourceFile` |
| Reference C# project | `CSharp """.. ."""` + `withReferences [csLib]` |

## Platform

| Scenario | Approach |
|----------|----------|
| Self-contained F# code | CrossPlatform (default) |
| Uses desktop-only namespaces | `[<Trait("Category", "DesktopOnly")>]` + skip check |
| Uses P/Invoke or COM | `[<Trait("Category", "WindowsOnly")>]` + skip check |
| Uses 32-bit FSI | DesktopOnly |
```

---

## Part 3: FsharpQA Format Reference

### 3.1 Directory Structure
```
tests/fsharpqa/Source/
├── test.lst                    # Master list:  tags + paths to test folders
├── run.pl                      # Per-test execution logic  
├── CompilerOptions/fsc/        # Compiler option tests
├── CompilerOptions/fsi/        # FSI option tests
├── Conformance/                # Language conformance (largest)
├── Diagnostics/                # Error/warning tests
├── Libraries/                  # Core library tests
└── ... 
```

### 3.2 env.lst Format
Each test folder has `env.lst` with test definitions:
```perl
SOURCE=test.fs SCFLAGS="--test: ErrorRanges"              # Simple test
SOURCE=dummy.fsx COMPILE_ONLY=1 PRECMD="..." POSTCMD="..." # With commands
SOURCE=script.fsx FSIMODE=EXEC                           # FSI test
SOURCE="file.fsi file.fs" SCFLAGS="-a"                   # Multi-file
ReqENU  SOURCE=test.fs                                   # English-only prefix
```

**Variables:**
| Variable | Meaning |
|----------|---------|
| `SOURCE` | Source file(s) |
| `SCFLAGS` | Compiler flags |
| `COMPILE_ONLY=1` | Don't run output |
| `FSIMODE` | `EXEC`, `PIPE`, or `FEED` |
| `PRECMD`/`POSTCMD` | Shell commands (manual migration) |

**Prefix Tags:**
| Tag | Meaning |
|-----|---------|
| `ReqENU` | English locale only |
| `NoMT` | Skip multi-targeting (obsolete) |

### 3.3 Source File Expects Format
```fsharp
//<Expects status="success"></Expects>
//<Expects status="error" id="FS0001" span="(5,1-5,10)">message</Expects>
//<Expects status="warning" id="FS0046">message</Expects>
//<Expects status="skip">reason</Expects>
//<Expects status="notin">should not appear</Expects>
```

### 3.4 Token Substitutions
`$FSC_PIPE`, `$FSI_PIPE`, `$CSC_PIPE`, `$CWD` get replaced at runtime in PRECMD/POSTCMD. 

---

## Part 4: Migration Rules

### 4.1 Source File Handling
**DO NOT modify source files. ** Git-move them unchanged: 
- Preserves line numbers (Expects spans remain valid)
- Clean PR review (shows as rename, not edit)
- Copy to:  `tests/FSharpQaMigratedTests/resources/[original-path]/`

### 4.2 Compilation Method Selection

| Scenario | Method | Reason |
|----------|--------|--------|
| Checking errors/warnings | `typecheck` | Fastest |
| Need IL verification | `compile` | Required for IL |
| Testing runtime output | `compileExeAndRun` | Required for execution |
| Default choice | `typecheck` | Prefer speed |

### 4.3 Platform Classification

Apply judgment based on source file content:

| Source File Uses | Classification |
|------------------|----------------|
| Pure F# code, standard library | CrossPlatform |
| `System.Windows.Forms`, WPF namespaces | WindowsOnly |
| Desktop-only BCL (e.g., `System.Runtime. Remoting`) | DesktopOnly |
| P/Invoke with Windows DLLs | WindowsOnly |
| 32-bit specific (`$FSI32_PIPE`) | DesktopOnly |
| COM interop | WindowsOnly |

**Default: CrossPlatform** unless source clearly requires otherwise.

### 4.4 Expects to Assertions Mapping

| Expects | Assertion |
|---------|-----------|
| `status="success"` | `shouldSucceed` |
| `status="error"` | `shouldFail` |
| `status="warning"` with no errors | `shouldSucceed` + `withDiagnostics [Warning...]` |
| `id="FS0001"` | Error/Warning number `1` |
| `span="(5,1-10,15)"` | `Line 5, Col 1, Line 10, Col 15` |
| Multiple `<Expects>` | All go in `withDiagnostics [...]` list |

---

## Part 5: Work Packages

### Package Definition
```yaml
Package ID: [CATEGORY]-[SUBFOLDER]
Source: tests/fsharpqa/Source/[path]
Target: tests/FSharpQaMigratedTests/[Category]/[Module]. fs
Resources: tests/FSharpQaMigratedTests/resources/[path]/
```

### Package List

#### CompilerOptions (23 packages)
| ID | Path | Tests | Platform |
|----|------|-------|----------|
| OPTS-FSC-DUMP | CompilerOptions/fsc/dumpAllCommandLineOptions | ~2 | CrossPlatform |
| OPTS-FSC-FLATERRORS | CompilerOptions/fsc/flaterrors | ~5 | CrossPlatform |
| OPTS-FSC-GCCERRORS | CompilerOptions/fsc/gccerrors | ~5 | CrossPlatform |
| OPTS-FSC-LIB | CompilerOptions/fsc/lib | ~5 | CrossPlatform |
| OPTS-FSC-NOFRAMEWORK | CompilerOptions/fsc/noframework | ~3 | CrossPlatform |
| OPTS-FSC-NOLOGO | CompilerOptions/fsc/nologo | ~2 | CrossPlatform |
| OPTS-FSC-OPTIMIZE | CompilerOptions/fsc/optimize | ~5 | CrossPlatform |
| OPTS-FSC-OUT | CompilerOptions/fsc/out | ~5 | CrossPlatform |
| OPTS-FSC-PDB | CompilerOptions/fsc/pdb | ~5 | WindowsOnly |
| OPTS-FSC-PLATFORM | CompilerOptions/fsc/platform | ~5 | CrossPlatform |
| OPTS-FSC-REMOVED | CompilerOptions/fsc/Removed | ~5 | CrossPlatform |
| OPTS-FSC-RESPONSEFILE | CompilerOptions/fsc/responsefile | ~5 | CrossPlatform |
| OPTS-FSC-STANDALONE | CompilerOptions/fsc/standalone | ~3 | DesktopOnly |
| OPTS-FSC-STATICLINK | CompilerOptions/fsc/staticlink | ~3 | DesktopOnly |
| OPTS-FSC-SUBSYSVER | CompilerOptions/fsc/subsystemversion | ~3 | DesktopOnly |
| OPTS-FSC-TAILCALLS | CompilerOptions/fsc/tailcalls | ~3 | CrossPlatform |
| OPTS-FSC-TARGET | CompilerOptions/fsc/target | ~5 | CrossPlatform |
| OPTS-FSC-TOKENIZE | CompilerOptions/fsc/tokenize | ~3 | CrossPlatform |
| OPTS-FSI-HELP | CompilerOptions/fsi/help | 4 | DesktopOnly |
| OPTS-FSI-HIGHENTROPY | CompilerOptions/fsi/highentropyva | ~2 | DesktopOnly |
| OPTS-FSI-LANGVER | CompilerOptions/fsi/langversion | ~3 | CrossPlatform |
| OPTS-FSI-NOLOGO | CompilerOptions/fsi/nologo | ~2 | DesktopOnly |
| OPTS-FSI-SUBSYSVER | CompilerOptions/fsi/subsystemversion | ~2 | DesktopOnly |

#### Diagnostics (4 packages)
| ID | Path | Tests | Platform |
|----|------|-------|----------|
| DIAG-GENERAL | Diagnostics/General | ~80 | CrossPlatform |
| DIAG-ASYNC | Diagnostics/async | ~10 | CrossPlatform |
| DIAG-NONTERM | Diagnostics/NONTERM | ~5 | CrossPlatform |
| DIAG-PARSINGEOF | Diagnostics/ParsingAtEOF | ~5 | CrossPlatform |

#### Conformance (~16 packages, split by major subfolder)
| ID | Path | Tests | Platform |
|----|------|-------|----------|
| CONF-DECL | Conformance/DeclarationElements/* | ~35 | Mixed |
| CONF-EXPR-APP | Conformance/Expressions/ApplicationExpressions/* | ~30 | CrossPlatform |
| CONF-EXPR-CONST | Conformance/Expressions/ConstantExpressions | ~20 | CrossPlatform |
| CONF-EXPR-CTRL | Conformance/Expressions/ControlFlowExpressions/* | ~50 | CrossPlatform |
| CONF-EXPR-DATA | Conformance/Expressions/DataExpressions/* | ~60 | CrossPlatform |
| CONF-EXPR-QUOTE | Conformance/Expressions/ExpressionQuotations/* | ~20 | CrossPlatform |
| CONF-EXPR-MISC | Conformance/Expressions/* (others) | ~30 | CrossPlatform |
| CONF-IMPL | Conformance/ImplementationFilesAndSignatureFiles/* | ~25 | CrossPlatform |
| CONF-INFER | Conformance/InferenceProcedures/* | ~60 | CrossPlatform |
| CONF-LEX | Conformance/LexicalAnalysis/* | ~50 | CrossPlatform |
| CONF-LEXFILT | Conformance/LexicalFiltering/* | ~20 | CrossPlatform |
| CONF-OO | Conformance/ObjectOrientedTypeDefinitions/* | ~80 | CrossPlatform |
| CONF-SIG | Conformance/Signatures/* | ~15 | CrossPlatform |
| CONF-SPECIAL | Conformance/SpecialAttributesAndTypes/* | ~10 | CrossPlatform |
| CONF-STRUCT | Conformance/StructFieldEqualityComparison | ~5 | CrossPlatform |
| CONF-TYPES | Conformance/TypesAndTypeConstraints/* | ~30 | CrossPlatform |

#### Other (8 packages)
| ID | Path | Tests | Platform |
|----|------|-------|----------|
| LIB-CONTROL | Libraries/Control | ~10 | CrossPlatform |
| LIB-CORE | Libraries/Core/* | ~50 | CrossPlatform |
| LIB-PORTABLE | Libraries/Portable | ~10 | CrossPlatform |
| INTERACTIVE | InteractiveSession/* | ~20 | DesktopOnly |
| IMPORT | Import | ~20 | WindowsOnly |
| ENTRYPOINT | EntryPoint | ~5 | CrossPlatform |
| MISC | Misc | ~15 | CrossPlatform |
| STRESS | Stress | ~10 | CrossPlatform |

---

## Part 6: Sub-Agent Task Instructions

### Task:  Migrate Package [PACKAGE-ID]

#### Input
- Source folder: `tests/fsharpqa/Source/[path]`
- env.lst: `tests/fsharpqa/Source/[path]/env.lst`

#### Step 1: Parse env.lst
For each non-comment line, extract: 
- Prefix tags (`ReqENU`, `NoMT`, etc.)
- `SOURCE` file(s)
- `SCFLAGS`
- `COMPILE_ONLY`, `FSIMODE` if present
- `PRECMD`/`POSTCMD` if present (flag for manual handling)

Record:  **N tests found**

#### Step 2: Parse Source Files
For each SOURCE file, extract all `<Expects>` tags:
- `status`: success/error/warning/skip
- `id`: FS#### error code
- `span`: (line,col-line,col)
- Message text

#### Step 3: Classify Platform
Scan source file content: 
- Desktop-only namespaces?  → DesktopOnly
- Windows-specific APIs? → WindowsOnly
- Otherwise → CrossPlatform

#### Step 4: Generate Test File

Create `tests/FSharpQaMigratedTests/[Category]/[Module].fs`:

```fsharp
// Migrated from: tests/fsharpqa/Source/[path]
// Test count: N

namespace FSharpQaMigratedTests.[Category]

open Xunit
open FSharp.Test
open FSharp. Test.Compiler

module [ModuleName] =

    /// Original:  SOURCE=[file] SCFLAGS="[flags]"
    [<Theory; FileInlineData("[file]")>]
    let ``[file] - [brief description]`` compilation =
        compilation
        |> getCompilation
        |> asExe  // or asLibrary if SCFLAGS contains "-a"
        |> withOptions ["[flags]"]
        |> typecheck  // Use typecheck unless IL or execution needed
        |> shouldFail  // or shouldSucceed based on Expects
        |> withDiagnostics [
            (Error 1, Line 5, Col 1, Line 5, Col 10, "message")
        ]
```

For DesktopOnly/WindowsOnly tests: 
```fsharp
    [<Theory; FileInlineData("[file]")>]
    [<Trait("Category", "DesktopOnly")>]
    let ``[file] - [description]`` compilation =
        if not TestHelpers.isNetFramework then 
            Assert.Skip("Requires .NET Framework")
        // ... rest of test
```

#### Step 5: Move Resource Files
Git-move (not copy) source files unchanged:
```bash
mkdir -p tests/FSharpQaMigratedTests/resources/[path]
git mv tests/fsharpqa/Source/[path]/*. fs tests/FSharpQaMigratedTests/resources/[path]/
git mv tests/fsharpqa/Source/[path]/*.fsx tests/FSharpQaMigratedTests/resources/[path]/
git mv tests/fsharpqa/Source/[path]/*.fsi tests/FSharpQaMigratedTests/resources/[path]/
```

#### Step 6: Update Project File
Add to `.fsproj`:
- New test `.fs` file in `<Compile>` items
- Resource files as `<Content>` with `CopyToOutputDirectory`

#### Step 7: Verify

**On macOS (agent):**
```bash
cd tests/FSharpQaMigratedTests
dotnet build                    # MUST succeed
dotnet test --filter "FullyQualifiedName~[Module]"  # Run, note skips
```

**Verification Checklist:**
- [ ] Build passes
- [ ] Test count matches env.lst line count:  N expected, N found
- [ ] CrossPlatform tests pass on macOS
- [ ] DesktopOnly/WindowsOnly tests skip gracefully (not error)

#### Step 8: Cleanup fsharpqa Source

After verification: 
```bash
# Delete remaining files (env.lst, . gitignore, etc.)
rm -rf tests/fsharpqa/Source/[path]

# Update test. lst - comment out or remove the line for this path
# (Only after ALL subfolders under a path are migrated)
```

#### Step 9: Report

```markdown
## Package:  [PACKAGE-ID]

### Summary
- Source: tests/fsharpqa/Source/[path]
- Tests in env.lst: N
- Tests generated: N
- Platform: X CrossPlatform, Y DesktopOnly, Z WindowsOnly

### Verification (macOS)
- Build: PASS
- Tests: N total, M passed, K skipped, 0 failed

### Files
- Created: tests/FSharpQaMigratedTests/[Category]/[Module]. fs
- Moved: N source files to resources/[path]/
- Deleted: tests/fsharpqa/Source/[path]/

### Issues
- [Any PRECMD/POSTCMD tests flagged for manual migration]
- [Any blockers added to MIGRATION_BLOCKERS. md]
- [Any new patterns added to FEATURE_MAPPING.md]
```

---

## Part 7: Handling Special Cases

### PRECMD/POSTCMD Tests
Those are shell commands in general, but individually it is trivial stuff.
Like output capture (handled in ComponenetTests), file writes (Typically not needed at all, since ComponentTests work in memory) etc.

### Baseline Comparison Tests (comparer.fsx pattern)
Require output capture and file comparison:
1. Check if TEST_FRAMEWORK_ADDITIONS.md has helper
2. If not, add request to TEST_FRAMEWORK_ADDITIONS.md
3.  Implement manually or mark as blocker

### Multi-File Tests (`SOURCE="a.fsi a.fs"`)
Use `withAdditionalSourceFile`:
```fsharp
|> withAdditionalSourceFile (loadSourceFromFile "a.fsi")
```
Add pattern to FEATURE_MAPPING.md once solved.

### FSI Mode Tests
Use `asFsx`:
```fsharp
compilation |> getCompilation |> asFsx |> typecheck
```

---

## Part 8: Priority Execution Order

### Phase 1: Setup & Validation
1. Create project structure
2. Create tracking documents
3. Migrate DIAG-ASYNC (small, validates approach)

### Phase 2: Quick Wins
4.  DIAG-NONTERM
5. DIAG-PARSINGEOF  
6. OPTS-FSC-TAILCALLS
7. OPTS-FSC-OPTIMIZE

### Phase 3: High-Value Diagnostics
8. DIAG-GENERAL (large but straightforward)

### Phase 4: Compiler Options
9. Remaining OPTS-FSC-* (cross-platform first)
10. OPTS-FSI-* packages

### Phase 5: Conformance (Largest)
11. CONF-LEX* (lexical, usually simple)
12. CONF-EXPR-* 
13. Remaining CONF-*

### Phase 6: Libraries & Other
14. LIB-*
15. ENTRYPOINT, MISC, STRESS
16. INTERACTIVE, IMPORT (complex, last)

---

## Part 9: Verification Levels

### Level 1: Build (macOS ✓)
```bash
dotnet build tests/FSharpQaMigratedTests
```
Must pass for every package.

### Level 2: Cross-Platform Tests (macOS ✓)
```bash
dotnet test --filter "Category!=DesktopOnly&Category!=WindowsOnly"
```
All non-platform-specific tests must pass.

### Level 3: Full Tests (Windows CI)
```bash
dotnet test tests/FSharpQaMigratedTests
```
Validates DesktopOnly and WindowsOnly tests. 

---

## Part 10: Completion Criteria

### Per Package
- [ ] All env.lst tests have corresponding test functions
- [ ] Build passes
- [ ] Tests pass or skip appropriately
- [ ] Source files moved to resources/
- [ ] fsharpqa source folder deleted

### Full Migration
- [ ] `tests/fsharpqa/Source/` contains only infrastructure (run. pl, test.lst, etc.)
- [ ] All test folders deleted
- [ ] `tests/FSharpQaMigratedTests/` contains all migrated tests
- [ ] All tests pass on Windows CI
- [ ] Tracking documents updated

### Final Cleanup (After Full Migration)
- [ ] Delete fsharpqa infrastructure files
- [ ] Remove fsharpqa from CI
- [ ] Optionally merge into ComponentTests

---

## Summary

**For each package, the agent must:**
1. Parse env.lst → extract test definitions
2. Parse source files → extract Expects
3. Generate test file using `typecheck` (prefer speed)
4. Git-move source files unchanged (preserves line numbers)
5. Verify:  build + test on macOS
6. Delete fsharpqa source folder
7. Report with verification checklist

**Track progress in:**
- TEST_FRAMEWORK_ADDITIONS.md (needed framework features)
- MIGRATION_BLOCKERS. md (stuck tests)
- FEATURE_MAPPING.md (pattern solutions for reuse)

**Success = fsharpqa/Source empty, FSharpQaMigratedTests complete, all tests green on Windows+Linux+MacOS CI.**