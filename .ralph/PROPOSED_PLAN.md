# LangVersion 8.0+ Migration - Comprehensive Analysis and Plan

## Executive Summary

The F# compiler now enforces a minimum language version of 8.0. This analysis identifies **ALL** test files that reference language versions below 8.0 and categorizes them for atomic migration (per REPLAN.md feedback).

## Discovery Results (Full Search 2026-01-14)

### Test Project Inventory (Complete)

| Directory | Project Files | Files with Old LangVersion |
|-----------|---------------|---------------------------|
| tests/FSharp.Compiler.ComponentTests | 1 | 38 files |
| tests/fsharp | 1 | 18 files |
| tests/fsharpqa | 1 | 13 env.lst files |
| tests/FSharp.Compiler.Service.Tests | 1 | 2 files |
| tests/FSharp.Test.Utilities | 1 | 2 (infrastructure) |
| tests/FSharp.Compiler.Private.Scripting.UnitTests | 1 | 1 file |
| vsintegration/tests | 5 | 0 files |
| **Total** | **11 projects** | **74+ files** |

### Test Frameworks in Use

1. **xUnit** - Primary framework for ComponentTests, Service.Tests, FSharp.Editor.Tests
2. **FSharp.Test.Utilities** - Custom test infrastructure with `Compiler.fs` and `ScriptHelpers.fs` helpers
3. **fsharpqa** - Legacy Perl-based test harness with `env.lst` configuration files
4. **FSharp Suite** - `tests/fsharp/` with `single-test.fs` and `tests.fs`

---

## Detailed File Counts by Helper Type

### withLangVersionXX helper function usage (from Compiler.fs)

| Helper | File Count | Total Uses |
|--------|------------|------------|
| `withLangVersion46` | 4 files | 11 uses |
| `withLangVersion47` | 2 files | 5 uses |
| `withLangVersion50` | 11 files | 77 uses |
| `withLangVersion60` | 8 files | 20 uses |
| `withLangVersion70` | 24 files | 100 uses |
| **Total** | **38 unique files** | **213 uses** |

### LangVersion.VXX enum usage (from ScriptHelpers.fs)

| Usage | File Count | Uses |
|-------|------------|------|
| `LangVersion.V47` | 2 files | 2 uses |
| `LangVersion.V50` | 3 files | 6 uses |
| `LangVersion.V60` | 2 files | 4 uses |
| `LangVersion.V70` | 2 files | 3 uses |

Files:
- `tests/FSharp.Compiler.Private.Scripting.UnitTests/DependencyManagerInteractiveTests.fs` (V47)
- `tests/FSharp.Compiler.ComponentTests/Miscellaneous/MigratedTypeCheckTests.fs` (V50, V60, V70)
- `tests/FSharp.Compiler.ComponentTests/Miscellaneous/FsharpSuiteMigrated.fs` (V47, V50, V60, V70 - helper function)
- `tests/FSharp.Compiler.ComponentTests/Miscellaneous/MigratedCoreTests.fs` (V50, V70)

### Raw `--langversion:X.X` string usage

| Pattern | File Count | Uses |
|---------|------------|------|
| `--langversion:4.6` | 4 files | 22 uses |
| `--langversion:4.7` | 2 files | 3 uses |
| `--langversion:5.0` | 14 files | 93 uses |
| `--langversion:6.0` | 2 files | 3 uses |
| `--langversion:7.0` | 5 files | 12 uses |

---

## Large Files (>50 langversion refs)

| File | Lines | Total Refs | Notes |
|------|-------|------------|-------|
| `tests/fsharp/Compiler/Language/DefaultInterfaceMemberTests.fs` | 4953 | 76 uses of 4.6/5.0 | Tests Default Interface Member consumption |
| `tests/fsharp/Compiler/Language/OpenTypeDeclarationTests.fs` | 2445 | 80 uses (59 withLangVersion50, 5 withLangVersion46, 21 raw) | Tests `open type` feature |

---

## fsharpqa env.lst Files (13 total)

| File | Old Version Uses | Purpose |
|------|------------------|---------|
| `CompilerOptions/fsi/langversion/env.lst` | 4.5, 4,7, 4.70000... | Tests **invalid** langversion handling |
| `Conformance/ObjectOrientedTypeDefinitions/InterfaceTypes/env.lst` | 4.7, 5.0 (15 uses) | Version-gated interface tests |
| `Conformance/Expressions/DataExpressions/SequenceExpressions/env.lst` | 4.6 (3 uses) | version46/W_IfThenElse tests |
| `Diagnostics/NONTERM/env.lst` | 5.0 (5 uses) | Quote/brace expression tests |
| `Conformance/Expressions/DataExpressions/NameOf/env.lst` | 5.0 (10 uses) | nameof tests |
| `Conformance/TypesAndTypeConstraints/CheckingSyntacticTypes/env.lst` | 5.0 (6 uses) | Type constraint tests |
| `Conformance/ImplementationFilesAndSignatureFiles/.../global/env.lst` | 5.0 (1 use) | Namespace tests |
| `Conformance/DeclarationElements/ObjectConstructors/env.lst` | 5.0 (1 use) | Constructor tests |
| `Conformance/Expressions/ExpressionQuotations/Regressions/env.lst` | 5.0 (1 use) | Quotation tests |
| `Conformance/Expressions/DataExpressions/ObjectExpressions/env.lst` | 5.0 (2 uses) | Object expression tests |
| `Conformance/Expressions/DataExpressions/ComputationExpressions/env.lst` | 5.0 (3 uses) | Computation expression tests |
| `Conformance/Expressions/Type-relatedExpressions/env.lst` | 5.0 (1 use) | Type expression tests |
| `Conformance/InferenceProcedures/NameResolution/RequireQualifiedAccess/env.lst` | 5.0 (1 use) | Name resolution tests |

---

## tests/fsharp/tests.fs langversion references

| Line | Version | Purpose |
|------|---------|---------|
| 62 | 4.6 | `test-langversion-46.exe` in subtype test |
| 618 | 4.7 | `output.47` in printing test |
| 926 | 4.6 | `test-langversion-46.exe` in subtype test |
| 1888 | 6.0 | `pos40.exe` in fsc test |

---

## FSharp.Compiler.Service.Tests

| File | Uses | Notes |
|------|------|-------|
| `Common.fs` | 2 funcs | `parseAndCheckScript50`, `parseAndCheckScript70` |
| `ExprTests.fs` | 5 uses | All `--langversion:7.0` for witness tests |

---

## Migration Strategy (Option A from REPLAN.md)

Per REPLAN.md: **Atomic migrations** - each subtask must leave the build in a passing state. This means:

1. **Do NOT remove infrastructure first** - that causes 100+ compile errors
2. **Migrate test files first**, updating them to use 8.0+ or deleting obsolete tests
3. **Remove infrastructure helpers last**, after all usages are removed

### Migration Hierarchy

```
1. Simple updates (7.0 → 8.0, 5.0/6.0 → default) - one file at a time
2. Delete negative version gate tests (tests that verify failure on old versions)
3. Handle large files (DefaultInterfaceMemberTests, OpenTypeDeclarationTests) - one test block at a time
4. Update fsharpqa env.lst files
5. Update tests/fsharp/tests.fs
6. Update FSharp.Compiler.Service.Tests
7. Update ScriptHelpers.fs LangVersion enum users
8. Finally: Remove infrastructure (withLangVersion46..70, LangVersion.V47..V70)
```

---

## Categorized Files for Migration

### Category 1: Simple withLangVersion70 → withLangVersion80 (24 files, ~100 uses)

These files use `withLangVersion70` which can be directly updated to `withLangVersion80`:

| File | Uses |
|------|------|
| `tests/FSharp.Compiler.ComponentTests/Language/StaticClassTests.fs` | 23 |
| `tests/FSharp.Compiler.ComponentTests/Interop/RequiredAndInitOnlyProperties.fs` | 15 |
| `tests/FSharp.Compiler.ComponentTests/Interop/StaticsInInterfaces.fs` | 8 |
| `tests/FSharp.Compiler.ComponentTests/ErrorMessages/ClassesTests.fs` | 6 |
| `tests/FSharp.Compiler.ComponentTests/ErrorMessages/UnionCasePatternMatchingErrors.fs` | 6 |
| `tests/FSharp.Compiler.ComponentTests/ErrorMessages/NameResolutionTests.fs` | 5 |
| `tests/FSharp.Compiler.ComponentTests/Conformance/Types/UnionTypes/UnionStructTypes.fs` | 5 |
| `tests/FSharp.Compiler.ComponentTests/Interop/VisibilityTests.fs` | 4 |
| `tests/FSharp.Compiler.ComponentTests/Conformance/Types/UnionTypes/UnionTypes.fs` | 2 |
| `tests/FSharp.Compiler.ComponentTests/ErrorMessages/IndexingSyntax.fs` | 2 |
| `tests/FSharp.Compiler.ComponentTests/Conformance/BasicGrammarElements/StaticLet/StaticLetInUnionsAndRecords.fs` | 2 |
| `tests/FSharp.Compiler.ComponentTests/Language/ExtensionMethodTests.fs` | 2 |
| `tests/FSharp.Compiler.ComponentTests/Diagnostics/Records.fs` | 2 |
| `tests/FSharp.Compiler.ComponentTests/Conformance/Types/TypeConstraints/IWSAMsAndSRTPs/IWSAMsAndSRTPsTests.fs` | 20 |
| `tests/FSharp.Compiler.ComponentTests/Miscellaneous/ListLiterals.fs` | 1 |
| `tests/FSharp.Compiler.ComponentTests/Language/InterfaceTests.fs` | 1 |
| `tests/FSharp.Compiler.ComponentTests/Language/DotLambdaTests.fs` | 1 |
| `tests/FSharp.Compiler.ComponentTests/Language/CopyAndUpdateTests.fs` | 1 |
| `tests/FSharp.Compiler.ComponentTests/Language/PrintfFormatTests.fs` | 1 |
| `tests/FSharp.Compiler.ComponentTests/Language/AttributeCheckingTests.fs` | 1 |
| `tests/FSharp.Compiler.ComponentTests/EmittedIL/MethodImplAttribute/MethodImplAttribute.fs` | 1 |
| `tests/FSharp.Compiler.ComponentTests/EmittedIL/Literals.fs` | 1 |
| `tests/FSharp.Compiler.ComponentTests/EmittedIL/TestFunctions/TestFunctions.fs` | 1 |
| `tests/FSharp.Compiler.ComponentTests/Conformance/BasicGrammarElements/MemberDefinitions/MethodsAndProperties/MethodsAndProperties.fs` | 1 |

### Category 2: Simple withLangVersion60 → withLangVersion80 (8 files, ~20 uses)

| File | Uses |
|------|------|
| `tests/FSharp.Compiler.ComponentTests/Conformance/LexicalFiltering/OffsideExceptions/OffsideExceptions.fs` | 8 |
| `tests/FSharp.Compiler.ComponentTests/Conformance/BasicGrammarElements/UseBindings/UseBindings.fs` | 2 |
| `tests/FSharp.Compiler.ComponentTests/EmittedIL/SerializableAttribute/SerializableAttribute.fs` | 2 |
| `tests/FSharp.Compiler.ComponentTests/Conformance/Types/TypeConstraints/IWSAMsAndSRTPs/IWSAMsAndSRTPsTests.fs` | 2 |
| `tests/FSharp.Compiler.ComponentTests/Conformance/Types/UnionTypes/UnionTypes.fs` | 2 |
| `tests/FSharp.Compiler.ComponentTests/Conformance/PatternMatching/As/As.fs` | 1 |
| `tests/FSharp.Compiler.ComponentTests/Miscellaneous/ListLiterals.fs` | 1 |

### Category 3: withLangVersion50 → withLangVersion80 or default (11 files, ~77 uses)

| File | Uses | Notes |
|------|------|-------|
| `tests/fsharp/Compiler/Language/OpenTypeDeclarationTests.fs` | 54 | LARGE FILE |
| `tests/FSharp.Compiler.ComponentTests/Conformance/LexicalFiltering/OffsideExceptions/OffsideExceptions.fs` | 7 | |
| `tests/FSharp.Compiler.ComponentTests/Language/NameofTests.fs` | 4 | |
| `tests/FSharp.Compiler.ComponentTests/Interop/VisibilityTests.fs` | 4 | |
| `tests/FSharp.Compiler.ComponentTests/ErrorMessages/ModuleTests.fs` | 2 | |
| `tests/FSharp.Compiler.ComponentTests/Language/CodeQuotationTests.fs` | 1 | |
| `tests/FSharp.Compiler.ComponentTests/ErrorMessages/ConstructorTests.fs` | 1 | |
| `tests/fsharp/Compiler/Language/WitnessTests.fs` | 1 | |
| `tests/fsharp/Compiler/Regressions/NullableOptionalRegressionTests.fs` | 1 | |
| `tests/fsharp/Compiler/Service/SignatureGenerationTests.fs` | 1 | |

### Category 4: withLangVersion46/47 - Review and likely DELETE (4 files)

These test "feature fails on version 4.6/4.7" - no longer relevant:

| File | Uses | Action |
|------|------|--------|
| `tests/FSharp.Compiler.ComponentTests/ErrorMessages/WarnExpressionTests.fs` | 3 | DELETE tests or update |
| `tests/FSharp.Compiler.ComponentTests/ErrorMessages/WarnIfDiscardedInList.fs` | 6 (3+3) | DELETE tests or update |
| `tests/fsharp/Compiler/Language/OpenTypeDeclarationTests.fs` | 5 | DELETE 4.6 tests |

### Category 5: LangVersion.VXX enum users (4 files)

| File | Versions | Action |
|------|----------|--------|
| `tests/FSharp.Compiler.Private.Scripting.UnitTests/DependencyManagerInteractiveTests.fs` | V47 | Update to V80 |
| `tests/FSharp.Compiler.ComponentTests/Miscellaneous/MigratedTypeCheckTests.fs` | V50, V60, V70 | Update to V80 |
| `tests/FSharp.Compiler.ComponentTests/Miscellaneous/FsharpSuiteMigrated.fs` | V47-V70 (helper) | Update helper |
| `tests/FSharp.Compiler.ComponentTests/Miscellaneous/MigratedCoreTests.fs` | V50, V70 | Update to V80 |

### Category 6: Large Files (2 files, ~156 refs)

| File | Refs | Strategy |
|------|------|----------|
| `tests/fsharp/Compiler/Language/DefaultInterfaceMemberTests.fs` | 76 | Split into multiple subtasks |
| `tests/fsharp/Compiler/Language/OpenTypeDeclarationTests.fs` | 80 | Split into multiple subtasks |

### Category 7: fsharpqa env.lst files (13 files)

These need sed/text replacement of `--langversion:X.X`:

| File | Action |
|------|--------|
| `CompilerOptions/fsi/langversion/env.lst` | KEEP (tests invalid versions) |
| `Conformance/ObjectOrientedTypeDefinitions/InterfaceTypes/env.lst` | Update 4.7→8.0, 5.0→8.0 |
| `Conformance/Expressions/DataExpressions/SequenceExpressions/env.lst` | DELETE version46 tests |
| `Diagnostics/NONTERM/env.lst` | Update 5.0→8.0 |
| `Conformance/Expressions/DataExpressions/NameOf/env.lst` | Update 5.0→8.0 |
| All others | Update 5.0→8.0 |

### Category 8: tests/fsharp/tests.fs (4 refs)

| Line | Version | Action |
|------|---------|--------|
| 62, 926 | 4.6 | DELETE or update subtype tests |
| 618 | 4.7 | DELETE or update printing test |
| 1888 | 6.0 | Update to 8.0 |

### Category 9: FSharp.Compiler.Service.Tests (2 files, 7 refs)

| File | Action |
|------|--------|
| `Common.fs` | Rename parseAndCheckScript50→80, parseAndCheckScript70→80 |
| `ExprTests.fs` | Update 7.0→8.0 |

### Category 10: Infrastructure Removal (LAST)

After all usages are removed:

| File | Action |
|------|--------|
| `tests/FSharp.Test.Utilities/Compiler.fs` | Remove `withLangVersion46`, `withLangVersion47`, `withLangVersion50`, `withLangVersion60`, `withLangVersion70` (lines 612-625) |
| `tests/FSharp.Test.Utilities/ScriptHelpers.fs` | Remove `V47`, `V50`, `V60`, `V70` from LangVersion enum and match arms |

---
