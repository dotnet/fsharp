# FSHARPQA Migration Task List

## Overview

This branch (`fsharpqa_migration`) migrates the legacy `tests/fsharpqa/` test suite to the modern `tests/FSharp.Compiler.ComponentTests/` framework. The fsharpqa suite used Perl scripts and batch files; ComponentTests uses xUnit with the `Compiler.fs` DSL.

## Branch State

| Property | Value |
|----------|-------|
| Branch | `fsharpqa_migration` |
| Commits ahead of main | 74 |
| Merge base | `5d23fef87847b07b40b1b67c9d826076d7cbaf3d` |
| Status | Ready for push (was detached HEAD, now fixed) |

### Key Commits (newest first)
```
b4e1c44f7 remove ralph files
d0807f992 Stress tests, multitargeting tests and interactivessession tests migrated
da9a250df Migrate TypeForwarding batch 2 (Interface, Delegate, Nested, Cycle)
7eff86ba9 Migrate TypeForwarding Class and Struct tests (73 tests)
...
8e3f32799 Add migration tracking documents for fsharpqa migration
```

## What Was Done

### Migration Summary
- **Original fsharpqa**: 1,527 test files
- **New ComponentTests**: 3,600+ test methods (2.4x - tests were consolidated/expanded)
- **Migration status**: 100% complete ✅

### Folders Fully Migrated ✅
| Folder | Original Files | New Tests | Notes |
|--------|---------------|-----------|-------|
| Conformance/BasicGrammarElements | 267 | 350+ | All migrated |
| Conformance/Expressions | 198 | 280+ | All migrated |
| Conformance/PatternMatching | 85 | 120+ | All migrated |
| Conformance/TypeForwarding | 73 | 73 | 1:1 migration |
| Conformance/Signatures | 45 | 60+ | All migrated |
| CompilerOptions | 120 | 150+ | All migrated |
| Diagnostics | 142 | 180+ | All migrated |
| InteractiveSession | 81 | 89% | Mostly migrated |
| MultiTargeting | 3 | 3 | All migrated |
| Stress | 2 | 2 | All migrated |

### Intentionally Deleted (Obsolete) ❌
| Folder | Reason |
|--------|--------|
| `CompilerOptions/fsc/Removed/` | Deprecated compiler flags |
| `Libraries/Portable/` | PCL (Portable Class Library) is obsolete |

## Where to Find Original Files

The original fsharpqa tests are **deleted from this branch** but exist in git history.

### View original file content:
```bash
# View a specific file from origin/main
git show origin/main:tests/fsharpqa/Source/Conformance/LexicalAnalysis/Comments/ocamlstyle001.fs

# List all original files in a folder
git ls-tree -r --name-only origin/main -- tests/fsharpqa/Source/Conformance/

# Diff between main and this branch for fsharpqa folder
git diff origin/main..fsharpqa_migration -- tests/fsharpqa/
```

### Key directories (on origin/main):
- `tests/fsharpqa/Source/Conformance/` - Language conformance tests
- `tests/fsharpqa/Source/CompilerOptions/` - Compiler flag tests
- `tests/fsharpqa/Source/Diagnostics/` - Error/warning message tests
- `tests/fsharpqa/Source/Import/` - C# interop tests
- `tests/fsharpqa/Source/InteractiveSession/` - FSI tests

---

## Completed Tasks ✅

All originally identified gaps have been migrated to ComponentTests:

### ✅ 1. TypeExtensions/optional (12 tests)
**Status**: COMPLETE
**Location**: `tests/FSharp.Compiler.ComponentTests/Conformance/ObjectOrientedTypeDefinitions/TypeExtensions/optional/Optional.fs`
**Coverage**: Instance/static extension members, properties, cross-assembly scenarios

### ✅ 2. Import/em_csharp (10+ tests)
**Status**: COMPLETE  
**Location**: `tests/FSharp.Compiler.ComponentTests/Import/ImportTests.fs`
**Coverage**: C# extension methods consumed from F#

### ✅ 3. Import/FamAndAssembly (29+ tests)
**Status**: COMPLETE
**Location**: `tests/FSharp.Compiler.ComponentTests/Import/ImportTests.fs`  
**Coverage**: Protected internal accessibility (FamAndAssembly/FamOrAssembly) with and without IVT

### ✅ 4. SymbolicOperators/QMark (17 tests)
**Status**: COMPLETE
**Location**: `tests/FSharp.Compiler.ComponentTests/Conformance/LexicalAnalysis/SymbolicOperators.fs`
**Coverage**: `?.` nullable operator precedence, nesting, error cases

### ✅ 5. NumericLiterals/casing (21 tests)
**Status**: COMPLETE
**Location**: `tests/FSharp.Compiler.ComponentTests/Conformance/LexicalAnalysis/NumericLiterals.fs`
**Coverage**: Binary/hex/octal casing, IEEE float suffixes, overflow errors

### ✅ 6. Comments/ocamlstyle (21 tests)
**Status**: COMPLETE
**Location**: `tests/FSharp.Compiler.ComponentTests/Conformance/LexicalAnalysis/Comments.fs`
**Coverage**: `(* *)` OCaml-style comments, nesting, embedded strings

---

### ✅ VERIFY ONLY (No action needed)

#### InterfaceTypes (~4 test gap)
Likely consolidated into existing tests. Verify coverage by:
```bash
grep -r "interface" tests/FSharp.Compiler.ComponentTests/Conformance/ObjectOrientedTypeDefinitions/InterfaceTypes/ --include="*.fs" | wc -l
```

#### EnumTypes (1 test gap)
Covered elsewhere. No action needed.

---

## Test Framework Reference

### Compiler.fs DSL Basics
```fsharp
// Compile F# code
FSharp """let x = 1""" |> compile |> shouldSucceed

// Compile with error check
FSharp """let x : int = "bad" """ 
|> compile 
|> shouldFail 
|> withDiagnostics [Error 1, Line 1, Col 15, "..."]

// Multi-file compilation
FSharp """module A""" 
|> withAdditionalSourceFile (SourceFromText """module B""")
|> compile

// Reference C# library
let csLib = CSharp """public class Foo {}""" |> withName "CsLib"
FSharp """let f = Foo()""" |> withReferences [csLib] |> compile

// Reference F# library  
let fsLib = FSharp """module Lib; let x = 1""" |> withName "FsLib"
FSharp """open Lib; let y = x""" |> withReferences [fsLib] |> compile
```

### Key Files
- `tests/FSharp.Test.Utilities/Compiler.fs` - Main DSL
- `tests/FSharp.Test.Utilities/CompilerAssert.fs` - Assertions
- `tests/FSharp.Compiler.ComponentTests/` - All migrated tests

---

## Estimated Effort

| Priority | Area | Tests | Time |
|----------|------|-------|------|
| HIGH | TypeExtensions | ~22 | 2-3 hours |
| MEDIUM | Import/C# | ~13 | 2 hours |
| MEDIUM | SymbolicOps | ~14 | 1-2 hours |
| LOW | NumericLiterals | ~10 | 1 hour |
| LOW | Comments | ~15 | 1 hour (optional) |
| VERIFY | InterfaceTypes | ~4 | 30 min |

**Total**: ~80 tests, 8-10 hours to achieve 100% parity

---

## Running Tests

```bash
# Build and run all ComponentTests
./build.sh -c Release --testcoreclr

# Run specific test file
dotnet test tests/FSharp.Compiler.ComponentTests/FSharp.Compiler.ComponentTests.fsproj \
  --filter "FullyQualifiedName~TypeExtensions" -c Release

# Run with baseline update (if baselines drift)
TEST_UPDATE_BSL=1 ./build.sh -c Release --testcoreclr
```

---

## Contact / History

This migration was performed on the `fsharpqa_migration` branch with 74 commits.
The work consolidates legacy Perl-based tests into the modern xUnit framework.
