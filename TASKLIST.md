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
- **New ComponentTests**: 3,511+ test methods (2.3x - tests were consolidated/expanded)
- **Migration status**: ~95% complete

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

## Remaining Tasks

### ⚡ HIGH PRIORITY

#### 1. TypeExtensions/optional (~22 tests)
**Gap**: Optional extension members on types from external DLLs

**Original files** (view with `git show origin/main:tests/fsharpqa/Source/Conformance/ObjectOrientedTypeDefinitions/TypeExtensions/optional/`):
- `typeext_opt001.fs` through `typeext_opt008.fs`
- `E_typeext_opt005.fs`
- `GenericExtensions.fs`
- `SignatureIssue01.fs`
- `E_CrossModule01.fs`, `E_ModuleNotOpen.fs`, `E_NotInModule.fs`, `E_PrivateFields.fs`
- `ShortNamesAllowed.fs`, `FSUsingExtendedTypes.fs`

**Where to add**: `tests/FSharp.Compiler.ComponentTests/Conformance/ObjectOrientedTypeDefinitions/TypeExtensions/`

**How to implement**:
```fsharp
// Use FSharp helper lib for cross-assembly extensions
let helperLib = 
    FSharp """
namespace NS
type Lib() = 
    member val instanceField = 0 with get, set
"""
    |> withName "HelperLib"

FSharp """
namespace NS
module M = 
    type Lib with
        member x.ExtensionMember() = 1
        static member StaticExtensionMember() = 1
"""
|> withReferences [helperLib]
|> compile
|> shouldSucceed
```

**Acceptance**: All extension scenarios covered - instance, static, properties, indexers, cross-assembly

---

### 🔶 MEDIUM PRIORITY

#### 2. Import/em_csharp (~9 tests)
**Gap**: C# extension methods consumed from F#

**Original files**: `tests/fsharpqa/Source/Import/em_csharp_*.fs`

**Where to add**: `tests/FSharp.Compiler.ComponentTests/Import/ImportTests.fs`

**How to implement**:
```fsharp
let csExtensions = 
    CSharp """
using System;
public static class MyExtensions {
    public static int ExtMethod(this string s) => s.Length;
}
"""
    |> withName "CsExtensions"

FSharp """
open MyExtensions
let result = "hello".ExtMethod()
"""
|> withReferences [csExtensions]
|> compile
|> shouldSucceed
```

---

#### 3. Import/FamAndAssembly (~4 tests)
**Gap**: Protected internal accessibility from C#

**Original files**: `FamAndAssembly.fs`, `FamAndAssembly_NoIVT.fs`, `FamOrAssembly.fs`, `FamOrAssembly_NoIVT.fs`

**How to implement**: Need C# class with protected internal members + InternalsVisibleTo setup

---

#### 4. SymbolicOperators/QMark (~14 tests)
**Gap**: `?.` nullable operator precedence and nesting

**Original files**: `tests/fsharpqa/Source/Conformance/LexicalAnalysis/SymbolicOperators/QMark*.fs`

**Where to add**: `tests/FSharp.Compiler.ComponentTests/Conformance/LexicalAnalysis/SymbolicOperators.fs`

**Scenarios to cover**:
- `QMarkSimple.fs` - Basic `?.` usage
- `QMarkNested.fs` - Nested nullable access
- `QMarkPrecedence*.fs` - Precedence with arrays, method calls, currying
- `E_QMarkGeneric.fs` - Error case

---

### 🔵 LOW PRIORITY

#### 5. NumericLiterals/casing (~10 tests)
**Gap**: Literal casing variants and overflow errors

**Original files**: `casingBin.fs`, `casingHex.fs`, `casingOct.fs`, `casingIEEE-lf-LF*.fs`, `E_MaxLiterals*.fs`

**Where to add**: `tests/FSharp.Compiler.ComponentTests/Conformance/LexicalAnalysis/NumericLiterals.fs`

**Example**:
```fsharp
[<Fact>]
let ``Binary literal casing - 0b and 0B both work`` () =
    FSharp """
let a = 0b1010
let b = 0B1010
let equal = (a = b)
"""
    |> compile
    |> shouldSucceed
```

---

#### 6. Comments/ocamlstyle (~15 tests)
**Gap**: Legacy `(* *)` OCaml-style comments

**Original files**: `ocamlstyle001.fs`, `ocamlstyle_nested001-005.fs`, `embeddedString001-004.fs`, etc.

**Priority**: LOW - This syntax rarely regresses and is legacy

**Where to add**: `tests/FSharp.Compiler.ComponentTests/Conformance/LexicalAnalysis/Comments.fs`

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
