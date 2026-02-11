# Recover orphaned test files

A repo-wide scan found test files (containing `[<Fact>]`, `[<Theory>]`, etc.) that exist on disk but are not included in any `.fsproj`, meaning they are never compiled or run.
This PR adds them to the build, fixes issues that prevented inclusion, and cleans up dead duplicates.

## Summary

| | Count | Detail |
|---|---|---|
| **Files added to `.fsproj`** | 111 | 110 in `FSharp.Compiler.ComponentTests`, 1 in `FSharp.Core.UnitTests` |
| **Duplicate / dead files deleted** | 9 | See [Duplicate cleanup](#duplicate-cleanup-9-files-deleted) below |
| **Files requiring fixes** | 6 | Module renames, NUnit→xUnit, broken API calls, dead code removal |
| **New infrastructure** | 3 | `FactForWINDOWSAttribute`, `ProcessResult`/`runFsiProcess`/`runFscProcess`, `TypeForwardingHelpers` |
| **Build** | ✅ | 0 errors, 0 warnings |

---

## Infrastructure created

- **`FactForWINDOWSAttribute`** in `tests/FSharp.Test.Utilities/Utilities.fs` — conditional attribute for Windows-only CLI tests
- **`ProcessResult` + `runFsiProcess` + `runFscProcess`** in `tests/FSharp.Test.Utilities/Compiler.fs` — minimal subprocess helpers for tests that genuinely need CLI behavior (help flags, missing source file errors, unrecognized options that cause exit before session creation)
- **`TypeForwardingHelpers`** module in `tests/FSharp.Compiler.ComponentTests/Conformance/TypeForwarding/TypeForwardingHelpers.fs` — compiles C# original → F# exe → C# target → C# forwarder → swaps DLLs → runs via reflection

---

## Files added to `.fsproj`

### Wave 1: CompilerOptions (19 files)
- [x] `CompilerOptions\Fsc\FscCliTests.fs` — 5 CLI subprocess tests (FS0225 missing source file)
- [x] `CompilerOptions\Fsc\Removed.fs`
- [x] `CompilerOptions\Fsc\responsefile\responsefile.fs`
- [x] `CompilerOptions\Fsc\standalone\standalone.fs`
- [x] `CompilerOptions\Fsc\dumpAllCommandLineOptions\dumpAllCommandLineOptions.fs`
- [x] `CompilerOptions\Fsc\gccerrors\gccerrors.fs`
- [x] `CompilerOptions\Fsc\nologo\nologo.fs`
- [x] `CompilerOptions\Fsc\staticlink\staticlink.fs`
- [x] `CompilerOptions\Fsc\subsystemversion\Subsystemversion.fs`
- [x] `CompilerOptions\Fsc\target\target.fs`
- [x] `CompilerOptions\Fsc\tokenize\tokenize.fs`
- [x] `CompilerOptions\Fsc\lib\lib.fs`
- [x] `CompilerOptions\Fsc\Optimize.fs`
- [x] `CompilerOptions\Fsc\out\out.fs`
- [x] `CompilerOptions\Fsc\pdb\Pdb.fs`
- [x] `CompilerOptions\Fsc\tailcalls\tailcalls.fs`
- [x] `CompilerOptions\fsi\FsiCliTests.fs` — 7 CLI subprocess tests (FS0243 unrecognized option, help flags)
- [x] `CompilerOptions\fsi\Nologo.fs`
- [x] `CompilerOptions\fsi\Langversion.fs`

### Wave 2: Conformance Expressions & Lexical (23 files)
- [x] `Conformance\Expressions\ApplicationExpressions\Assertion\Assertion.fs`
- [x] `Conformance\Expressions\ApplicationExpressions\ObjectConstruction\ObjectConstruction.fs`
- [x] `Conformance\Expressions\ConstantExpressions\ConstantExpressions.fs`
- [x] `Conformance\Expressions\ControlFlowExpressions\SimpleFor\SimpleFor.fs`
- [x] `Conformance\Expressions\ControlFlowExpressions\TryFinally\TryFinally.fs`
- [x] `Conformance\Expressions\ControlFlowExpressions\TryWith\TryWith.fs`
- [x] `Conformance\Expressions\DataExpressions\AddressOf\AddressOf.fs`
- [x] `Conformance\Expressions\DataExpressions\ComputationExpressions\ComputationExpressions.fs`
- [x] `Conformance\Expressions\DataExpressions\ObjectExpressions\ObjectExpressions.fs`
- [x] `Conformance\Expressions\DataExpressions\QueryExpressions.fs`
- [x] `Conformance\Expressions\DataExpressions\RangeExpressions\RangeExpressions.fs`
- [x] `Conformance\Expressions\DataExpressions\SequenceExpressions\SequenceExpressions.fs`
- [x] `Conformance\Expressions\DataExpressions\Simple\Simple.fs`
- [x] `Conformance\Expressions\DataExpressions\TupleExpressions\TupleExpressions.fs`
- [x] `Conformance\Expressions\EvaluationOfElaboratedForms\EvaluationOfElaboratedForms.fs`
- [x] `Conformance\Expressions\ExpressionQuotations\Baselines.fs`
- [x] `Conformance\Expressions\ExpressionQuotations\Regressions.fs`
- [x] `Conformance\Expressions\SyntacticSugar\SyntacticSugar.fs`
- [x] `Conformance\Expressions\SyntacticSugarAndAmbiguities\SyntacticSugarAndAmbiguities.fs`
- [x] `Conformance\LexicalAnalysis\Directives.fs`
- [x] `Conformance\LexicalAnalysis\StringsAndCharacters.fs`
- [x] `Conformance\LexicalFiltering\Basic\Basic.fs`
- [x] `Conformance\LexicalFiltering\LexicalAnalysisOfTypeApplications\LexicalAnalysisOfTypeApplications.fs`

### Wave 3: Conformance InferenceProcedures (12 files)
- [x] `Conformance\InferenceProcedures\ConstraintSolving\ConstraintSolving.fs`
- [x] `Conformance\InferenceProcedures\DispatchSlotChecking\DispatchSlotChecking.fs`
- [x] `Conformance\InferenceProcedures\DispatchSlotInference\DispatchSlotInference.fs`
- [x] `Conformance\InferenceProcedures\FunctionApplicationResolution\FunctionApplicationResolution.fs`
- [x] `Conformance\InferenceProcedures\Generalization\Generalization.fs`
- [x] `Conformance\InferenceProcedures\MethodApplicationResolution\MethodApplicationResolution.fs`
- [x] `Conformance\InferenceProcedures\NameResolution\AutoOpen\AutoOpen.fs`
- [x] `Conformance\InferenceProcedures\NameResolution\Misc\NameResolutionMisc.fs`
- [x] `Conformance\InferenceProcedures\NameResolution\RequireQualifiedAccess\RequireQualifiedAccess.fs`
- [x] `Conformance\InferenceProcedures\ResolvingApplicationExpressions\ResolvingApplicationExpressions.fs`
- [x] `Conformance\InferenceProcedures\TypeInference\TypeInference.fs`
- [x] `Conformance\InferenceProcedures\WellFormednessChecking\WellFormednessChecking.fs`

### Wave 4: Conformance OO Types (17 files)
- [x] `Conformance\ObjectOrientedTypeDefinitions\AbstractMembers\AbstractMembers.fs`
- [x] `Conformance\ObjectOrientedTypeDefinitions\ClassTypes\AsDeclarations\AsDeclarations.fs`
- [x] `Conformance\ObjectOrientedTypeDefinitions\ClassTypes\AutoProperties\AutoProperties.fs`
- [x] `Conformance\ObjectOrientedTypeDefinitions\ClassTypes\ImplicitObjectConstructors\ImplicitObjectConstructors.fs`
- [x] `Conformance\ObjectOrientedTypeDefinitions\ClassTypes\InheritsDeclarations\InheritsDeclarations.fs`
- [x] `Conformance\ObjectOrientedTypeDefinitions\ClassTypes\MemberDeclarations\MemberDeclarations.fs`
- [x] `Conformance\ObjectOrientedTypeDefinitions\ClassTypes\Misc\Misc.fs`
- [x] `Conformance\ObjectOrientedTypeDefinitions\ClassTypes\StaticLetDoDeclarations\StaticLetDoDeclarations.fs`
- [x] `Conformance\ObjectOrientedTypeDefinitions\ClassTypes\ValueRestriction\ValueRestriction.fs`
- [x] `Conformance\ObjectOrientedTypeDefinitions\DelegateTypes\DelegateTypes.fs`
- [x] `Conformance\ObjectOrientedTypeDefinitions\EnumTypes\EnumTypes.fs`
- [x] `Conformance\ObjectOrientedTypeDefinitions\InterfaceTypes\InterfaceTypes.fs`
- [x] `Conformance\ObjectOrientedTypeDefinitions\StructTypes\StructTypes.fs`
- [x] `Conformance\ObjectOrientedTypeDefinitions\TypeExtensions\basic\Basic.fs`
- [x] `Conformance\ObjectOrientedTypeDefinitions\TypeExtensions\intrinsic\Intrinsic.fs`
- [x] `Conformance\ObjectOrientedTypeDefinitions\TypeExtensions\optional\Optional.fs`
- [x] `Conformance\ObjectOrientedTypeDefinitions\TypeKindInference\TypeKindInference.fs`

### Wave 5: Conformance Remaining + TypeForwarding (20 files)
- [x] `Conformance\DeclarationElements\PInvokeDeclarations.fs`
- [x] `Conformance\GeneratedEqualityHashingComparison\Attributes\New\Attributes_New.fs`
- [x] `Conformance\ImplementationFilesAndSignatureFiles\CheckingOfImplementationFiles.fs`
- [x] `Conformance\ImplementationFilesAndSignatureFiles\NamespacesFragmentsAndImplementationFiles.fs`
- [x] `Conformance\ImplementationFilesAndSignatureFiles\SignatureFiles.fs`
- [x] `Conformance\Signatures\Signatures.fs`
- [x] `Conformance\SpecialAttributesAndTypes\CallerInfo.fs`
- [x] `Conformance\SpecialAttributesAndTypes\ThreadStatic.fs`
- [x] `Conformance\Stress\StressTests.fs`
- [x] `Conformance\TypeForwarding\TypeForwardingTests.fs`
- [x] `Conformance\TypesAndTypeConstraints\TypesAndTypeConstraints.fs`
- [x] `Conformance\TypeForwarding\TypeForwardingHelpers.fs` *(new infrastructure)*
- [x] `Conformance\TypeForwarding\TypeForwardingHelpersTests.fs`
- [x] `Conformance\TypeForwarding\NegativeCases.fs`
- [x] `Conformance\TypeForwarding\StructTests.fs`
- [x] `Conformance\TypeForwarding\ClassTests.fs`
- [x] `Conformance\TypeForwarding\DelegateTests.fs`
- [x] `Conformance\TypeForwarding\InterfaceTests.fs`
- [x] `Conformance\TypeForwarding\CycleTests.fs`
- [x] `Conformance\TypeForwarding\NestedTests.fs`

### Wave 6: Remaining orphans (20 files)
Found during exhaustive re-sweep (grep for `[<Fact>]`/`[<Theory>]` in all `.fs` not in any `.fsproj`).
- [x] `InteractiveSession\Misc.fs` — 116+ tests
- [x] `CompilerOptions\Fsc\Platform.fs`
- [x] `EmittedIL\RealInternalSignature\ClassTypeVisibility.fs`
- [x] `EmittedIL\RealInternalSignature\RealInternalSignature.fs`
- [x] `Conformance\LexicalAnalysis\Whitespace.fs` — 1 test
- [x] `Conformance\LexicalAnalysis\ShiftGenerics.fs` — 1 test
- [x] `Conformance\LexicalAnalysis\LineDirectives.fs` — 2 tests
- [x] `Conformance\LexicalAnalysis\IdentifiersAndKeywords.fs` — 15 tests
- [x] `Conformance\LexicalAnalysis\IdentifierReplacements.fs` — 3 tests
- [x] `Conformance\LexicalAnalysis\ConditionalCompilation.fs` — 13 tests
- [x] `Conformance\DeclarationElements\ObjectConstructors.fs` — 16 tests
- [x] `Conformance\MultiTargeting.fs` — 3 tests (`FactForDESKTOP`)
- [x] `Diagnostics\ParsingAtEOF.fs` — 11 tests
- [x] `Diagnostics\NONTERM.fs` — 36 tests
- [x] `Misc.fs` (root) — 2 unique inline IL tests (Array.Empty, DU cctor renaming)
- [x] `EmittedIL\StringEncoding\StringEncoding.fs` — 4 tests
- [x] `CompilerOptions\CliProcessTests.fs` — 7 tests (FSC/FSI help, version, invalid options)
- [x] `Miscellaneous\MigratedMisc.fs` — 13 tests
- [x] `Libraries\NativeInterop.fs` — 1 test
- [x] `FSharp.Core\Microsoft.FSharp.Linq\NullableOperators.fs` *(in FSharp.Core.UnitTests.fsproj)*

---

## Fixes applied to enable inclusion

| File | Issue | Fix |
|------|-------|-----|
| `CompilerOptions\Fsc\Platform.fs` | `module Platform` conflicted with existing `Platform.fs` in same namespace | Renamed module to `PlatformErrors` |
| `EmittedIL\RealInternalSignature\ClassTypeVisibility.fs` | `module ClassTypeVisibilityModuleRoot` conflicted with existing file | Renamed module to `ClassTypeVisibility` |
| `EmittedIL\RealInternalSignature\RealInternalSignature.fs` | `namespace EmittedIL` + `module RealInternalSignature` conflicted; also had broken `\|> asLibrary \|> compile \|> compileExeAndRun` pipeline (`compileExeAndRun` takes `CompilationUnit`, not `CompilationResult`) | Changed namespace to `EmittedIL.RealInternalSignature`, renamed module to `RealInternalSignatureTests`, removed redundant `\|> asLibrary \|> compile` before `compileExeAndRun` (2 instances) |
| `InteractiveSession\Misc.fs` | Lines 2140-2186 contained 3 duplicate tests using `withFsiArgs` which doesn't exist in the test framework | Removed the 48-line dead section; the working versions using `CompilerAssert.RunScriptWithOptionsAndReturnResult` at lines 2052-2111 were already present |
| `Conformance\MultiTargeting.fs` | Used `withRawOptions` which doesn't exist in the test framework | Changed to `withOptions` |
| `FSharp.Core\Microsoft.FSharp.Linq\NullableOperators.fs` | NUnit test (`[<TestFixture>]`, `Assert.AreEqual`) in an xUnit project | Removed `[<TestFixture>]`, `Assert.AreEqual` → `Assert.Equal`, added `open System` |

---

## Duplicate cleanup (9 files deleted)

During the sweep, 9 `.fs` files were found that contained `[<Fact>]`/`[<Theory>]` attributes but were **not** in any `.fsproj` and could not be meaningfully revived. Each was independently assessed by 3 models (Claude Opus 4.6, Gemini 3 Pro, GPT-5.2 Codex) — unanimous verdict: **delete**.

All 9 files were never compiled, never executed by any test runner, and had been dead since the Goodbye Perl migration.

### Exact duplicates (4 files)

Root-level copies whose tests are already present in the canonical EmittedIL subdirectory files:

| Deleted file | Canonical file (already in `.fsproj`) | Evidence |
|---|---|---|
| `TupleElimination.fs` (root, 5 tests) | `EmittedIL\TupleElimination.fs` (5 tests) | All 5 test names identical |
| `Literals.fs` (root, 1 test) | `EmittedIL\Literals.fs` (13 tests) | Root test is a strict subset |
| `TailCalls.fs` (root, 5 tests) | `EmittedIL\TailCalls.fs` (8 tests) | All 5 root tests present in canonical file |
| `StringFormatAndInterpolation.fs` (root, 1 test) | `EmittedIL\StringFormatAndInterpolation.fs` (4 tests) | Root test is a strict subset |

### Broken duplicates (5 files)

Files that reference non-existent directories or test data, AND whose tests are already covered by working canonical versions:

| Deleted file | Problem | Canonical file | Evidence |
|---|---|---|---|
| `Operators.fs` (root) | Uses `Directory(__SOURCE_DIRECTORY__ + "/../resources/tests/CodeGen/operators")` — that directory does not exist | `EmittedIL\operators\Operators.fs` | Same decimal comparison test; canonical uses `FileInlineData("decimal_comparison.fs")` which exists |
| `AttributeUsage.fs` (root, 23 tests) | Uses `Directory(__SOURCE_DIRECTORY__, ...)` at root, but the referenced source files (e.g. `AssemblyVersion01.fs`, `E_WithBitwiseAnd01.fsx`) are not at root | `Conformance\BasicGrammarElements\CustomAttributes\AttributeUsage\AttributeUsage.fs` (86 tests) | 22 of 23 tests are duplicates; the 1 "unique" test (`E_WithBitwiseAnd01.fsx`) references a file that does not exist anywhere in the repo — it was DOA |
| `OnOverridesAndIFaceImpl.fs` (root, 6 tests) | Uses `Directory(__SOURCE_DIRECTORY__, ...)` at root, but `E_InterfaceImpl01.fs` etc. are at `Conformance/BasicGrammarElements/AccessibilityAnnotations/OnOverridesAndIFaceImpl/` | `Conformance\BasicGrammarElements\AccessibilityAnnotations\OnOverridesAndIFaceImpl\OnOverridesAndIFaceImpl.fs` (6 tests) | All 6 tests are identical (names differ only `.fs` vs `_fs` suffix) |
| `EmittedIL\Structure\StructFieldEquality.fs` | Uses `FileInlineData("decimal_comparison.fs")` but that file is in `EmittedIL/operators/`, not `EmittedIL/Structure/` | `EmittedIL\operators\Operators.fs` | Same test, same source file, wrong directory |
| `TypeChecks\TypeExtensions\PropertyShadowingTests.fs` | References `__SOURCE_DIRECTORY__ + "/Shadowing"` but `TypeChecks/TypeExtensions/Shadowing/` does not exist | `TypeChecks\PropertyShadowingTests.fs` | Identical tests; canonical version correctly references `TypeChecks/Shadowing/` which exists |

**Zero test coverage was lost.** Every test in the deleted files is either an exact duplicate of a test in an already-included file, or references test data that never existed.
