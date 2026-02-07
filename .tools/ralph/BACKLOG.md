# BACKLOG

## Overview
Check the PR you are at , 19188 in dotnet/fsharp. It has tests, description, pointers to csharp. I want you t
o change it so that inherit attr and reference is not resolved at compile time into the xml, but only works at tooling time by resolving the possibly recursive chain of cref pointers. i.e. a tooling o
nly feature, but make sure it is tested. You might need to change testing approach somewhere, maybe. inheritdoc can made its way into the .xml, this is fine. Make use of this simplification and try to
 simplify the feature architecture (like, can you perharps pass a cref -> doc resolved from tooling/service layer to this feature? You can also move the file up if that helps - asses what helps the mo
st

**Approach:** ### Original User Request
> Check the PR you are at, 19188 in dotnet/fsharp. It has tests, description, pointers to csharp. I want you to change it so that inherit attr and reference is not resolved at compile time into the xml, but only works at tooling time by resolving the possibly recursive chain of cref pointers. i.e. a tooling only feature, but make sure it is tested. You might need to change testing approach somewhere, maybe. inheritdoc can made its way into the .xml, this is fine. Make use of this simplification and try to simplify the feature architecture (like, can you perhaps pass a cref -> doc resolved from tooling/service layer to this feature? You can also move the file up if that helps - assess what helps the most.

### Analysis

**PR #19188** implements `<inheritdoc>` XML documentation support for F#. The PR branch is `copilot/support-xmldoc-inherit-element` in dotnet/fsharp. The working tree already contains **uncommitted changes** that partially implement the requested simplification. Here's what we're working with:

#### Current State (committed on branch)
The committed version has `<inheritdoc>` resolution in THREE places:
1. **`XmlDocFileWriter.fs`** — Expands `<inheritdoc>` at compile time when writing `.xml` files (takes `TcImports`, does CCU traversal)
2. **`SymbolHelpers.fs`** — Expands `<inheritdoc>` for tooltips (partial, no TcImports available)
3. **`Symbols.fs`** — Expands via `FSharpEntity.XmlDoc` / `FSharpMemberOrFunctionOrValue.XmlDoc` properties

The committed `XmlDocInheritance.fs` is 585 lines with a complex signature:
```fsharp
val expandInheritDoc:
    allCcusOpt: CcuThunk list option ->
    tryFindXmlDocBySignature: (string -> string -> XmlDoc option) option ->
    ccuOpt: CcuThunk option ->
    currentModuleTypeOpt: ModuleOrNamespaceType option ->
    implicitTargetCrefOpt: string option ->
    m: range -> visited: Set<string> -> doc: XmlDoc -> XmlDoc
```

All the CCU/TypedTree resolution logic was embedded in XmlDocInheritance.fs itself.

#### Uncommitted Working Tree Changes (partially complete)
Someone already started the refactoring:
1. **`XmlDocFileWriter.fs`** — DONE: Reverted to original signature `(g, assemblyName, generatedCcu, xmlFile)`, writes `<inheritdoc>` as-is to XML
2. **`fsc.fs`** — DONE: Reverted to original `WriteXmlDocFile` call without `tcImports`
3. **`SymbolHelpers.fs`** — DONE: Removed `XmlDocInheritance` import, passes `xmlDoc` through unchanged
4. **`XmlDocInheritance.fs`** — DONE: Simplified to 211 lines, pure XML processing with `resolveCref: string -> string option`
5. **`XmlDocInheritance.fsi`** — DONE: Simplified signature
6. **`Symbols.fs`** — DONE: Contains `buildCrefResolver` that creates the `resolveCref` function using SymbolEnv (CCUs, typed tree, external XML files), plus cref parsing helpers moved from XmlDocInheritance
7. **`ILVerify baselines`** — DONE: Updated to reflect functions moving from XmlDocInheritance to Symbols.Impl
8. **Component tests** — DONE: Updated to use new `expandInheritDoc` signature with `resolveCref` function

**What's NOT done yet:**
- The changes are **uncommitted** — need to be verified via build and test
- The test approach needs verification: committed `XmlDocTests.fs` (662 lines added, 57 tests) tests expansion through `FSharpEntity.XmlDoc` etc. — these should still work since `Symbols.fs` still expands. But some tests may have tested XML file output and need adjustment.
- Surface area baseline may need updating (currently reflects committed state, not working tree)
- Need to assess whether `XmlDocSigParser.fs` is still needed or if its logic overlaps with the cref parsers now in `Symbols.fs`

### Approach

The architecture simplification is already largely done in the working tree. The key design:

1. **`XmlDocInheritance.fs`** (pure XML) — Only does XML parsing/expansion. Takes `resolveCref: string -> string option`. No TypedTree/CCU dependencies.
2. **`Symbols.fs`** (tooling layer) — Provides the `resolveCref` implementation via `buildCrefResolver` using SymbolEnv. Calls `expandInheritDoc` when `FSharpEntity.XmlDoc` or `FSharpMemberOrFunctionOrValue.XmlDoc` are accessed.
3. **`XmlDocFileWriter.fs`** (compiler) — Writes `<inheritdoc>` as-is to `.xml` files. No resolution at compile time.
4. **`SymbolHelpers.fs`** — Passes XML docs through unchanged. Resolution happens via Symbols.fs path.

**What remains:**
- Build and run tests to verify everything works
- Fix any test failures (some XmlDocTests may test XML file output expansion)
- Update surface area baseline if needed
- Assess if `XmlDocSigParser.fs` is redundant given the cref parsers in `Symbols.fs`
- Commit all changes

### Key Design Decisions
- `<inheritdoc>` is a **tooling-only feature**: it appears as-is in `.xml` files, resolution happens only when accessed through FCS Symbols API (IDE tooltips, code analysis)
- The `resolveCref` function pattern decouples XML processing from TypedTree/CCU concerns — XmlDocInheritance has zero compiler infrastructure dependencies
- Implicit target resolution (for `<inheritdoc/>` without `cref`) is computed in `Symbols.fs` using slot signatures and type hierarchy
- `XmlDocSigParser.fs` remains as a public API type (exposed in surface area) for parsing doc comment IDs — it's used by other consumers potentially

### Codebase Context
- **File ordering in fsproj**: XmlDocSigParser (475) → XmlDocInheritance (477) → XmlDocFileWriter (479) → ... → SymbolHelpers (491) → Symbols (493)
- XmlDocInheritance CANNOT reference Symbols.fs (compilation order), which is correct for the dependency direction
- `SymbolEnv` provides access to `thisCcu`, `thisCcuTy`, `tcImports`, `amap` — all needed for cref resolution
- Tests are in two places: `tests/FSharp.Compiler.Service.Tests/XmlDocTests.fs` (57 integration tests) and `tests/FSharp.Compiler.ComponentTests/Miscellaneous/XmlDoc.fs` (unit tests)
- Build command: `./build.sh -c Release --testcoreclr`
- Targeted test: `dotnet test tests/FSharp.Compiler.Service.Tests/FSharp.Compiler.Service.Tests.fsproj --filter "FullyQualifiedName~InheritDoc" -c Release /p:BUILDING_USING_DOTNET=true`

## Tasks

| ID | Task | Status |
| --- | --- | --- |
| 1 | Simplify `XmlDocInheritance.fs` to pure XML processing with `resolveCref` parameter | ✅ DONE |
| 2 | Move cref parsing/resolution logic to `Symbols.fs` (`buildCrefResolver`) | ✅ DONE |
| 3 | Remove compile-time `<inheritdoc>` expansion from `XmlDocFileWriter.fs` | ✅ DONE |
| 4 | Update `fsc.fs` call site to simplified `WriteXmlDocFile` signature | ✅ DONE |
| 5 | Remove partial expansion from `SymbolHelpers.fs` (leave for Symbols.fs path) | ✅ DONE |
| 6 | Update `XmlDocInheritance.fsi` to match simplified signature | ✅ DONE |
| 7 | Update `XmlDocFileWriter.fsi` to match simplified signature | ✅ DONE |
| 8 | Update ILVerify baselines (functions moved from XmlDocInheritance to Symbols.Impl) | ✅ DONE |
| 9 | Consolidate & simplify component tests (`XmlDocSigParserTests`, `XmlDocInheritanceTests`) | ✅ DONE |
| 10 | Build verification (0 errors, 0 warnings) | ✅ DONE |
| 11 | Run InheritDoc tests via FCS Symbols API (23 passed) | ✅ DONE |
| 12 | Run XmlDoc component tests (21 passed) | ✅ DONE |
| 13 | Run all XmlDoc tests including integration (84 passed, 5 pre-existing skips) | ✅ DONE |
| 14 | Surface area baseline test (1 passed) | ✅ DONE |

## Summary

All tasks completed. The architecture simplification is verified:

- **`XmlDocInheritance.fs`** (211 lines, down from 585): Pure XML processing. Takes `resolveCref: string -> string option`. Zero TypedTree/CCU dependencies.
- **`Symbols.fs`**: Provides `buildCrefResolver` using `SymbolEnv` (CCUs, typed tree, external XML files). Calls `expandInheritDoc` when `FSharpEntity.XmlDoc` / `FSharpMemberOrFunctionOrValue.XmlDoc` are accessed.
- **`XmlDocFileWriter.fs`**: Writes `<inheritdoc>` as-is to `.xml` files. No resolution at compile time.
- **`SymbolHelpers.fs`**: Passes XML docs through unchanged. Resolution happens only through Symbols.fs path.

Net change: -640 lines removed, +329 lines added = **-311 lines net reduction**.

