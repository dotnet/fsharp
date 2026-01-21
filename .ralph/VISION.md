# FSHARPQA Migration Completion Vision

## High-Level Goal
Complete the migration of remaining legacy `tests/fsharpqa/` tests to the modern `tests/FSharp.Compiler.ComponentTests/` framework using xUnit with the `Compiler.fs` DSL.

## Current State
- **Original fsharpqa**: 1,527 test files
- **Already migrated**: ~95% complete (3,511+ test methods)
- **Remaining**: ~80 tests across 6 gap areas

## Remaining Gaps (from TASKLIST.md)

| Priority | Area | Tests | Target File |
|----------|------|-------|-------------|
| HIGH | TypeExtensions/optional | ~22 | TypeExtensions/optional/Optional.fs (new) |
| MEDIUM | Import/em_csharp | ~9 | Import/ImportTests.fs (extend) |
| MEDIUM | Import/FamAndAssembly | ~4 | Import/ImportTests.fs (extend) |
| MEDIUM | SymbolicOperators/QMark | ~13 | LexicalAnalysis/SymbolicOperators.fs (extend) |
| LOW | NumericLiterals/casing | ~10 | LexicalAnalysis/NumericLiterals.fs (extend) |
| LOW | Comments/ocamlstyle | ~15 | LexicalAnalysis/Comments.fs (extend) |

## Key Design Decisions

1. **Use existing patterns**: Follow the DSL patterns already established in ComponentTests:
   - `FSharp """...""" |> compile |> shouldSucceed`
   - `CSharp """...""" |> withName "name" |> withReferences [...]`
   - Cross-assembly scenarios use `withReferences`

2. **Test structure**: Add tests to existing test files where possible (extend, don't scatter)

3. **Error tests**: Use `shouldFail |> withErrorCode NNNN |> withDiagnosticMessageMatches`

4. **Cross-assembly tests**: TypeExtensions/optional requires separate helper library compilations

## Constraints & Gotchas

1. **Optional extensions require separate assembly**: Cannot test cross-assembly extension methods with inline code only
2. **QMark tests**: The `?.` nullable operator is a language feature needing actual nullable reference types
3. **OCaml comments**: Legacy `(* *)` syntax - low priority as rarely regresses
4. **Build command**: `./build.sh -c Release --testcoreclr`
5. **Filter tests**: `dotnet test ... --filter "FullyQualifiedName~AreaName"`
6. **Test execution crashes**: Using `compileAndRun` with inline source or `FsFromPath` + `withReferences` causes test host crashes. Workaround: use `compile` only (no run) or add tests to existing working files rather than creating new test files.
7. **Optional tests partial migration**: Due to issue #6, only 4 of ~22 optional extension tests were migrated as compile-only tests in Basic.fs. Full migration requires investigation of test framework execution issues.

## Success Criteria
- All 6 gap areas have migrated tests
- All tests pass (`./build.sh -c Release --testcoreclr`)
- No regressions in existing tests
