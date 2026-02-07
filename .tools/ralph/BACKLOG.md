# BACKLOG

## Original Request
Move the logic so that compilation does not retrieve the inheritdoc cref target and does not replace it at .xml writing time, and ONLY makes the inherit a tooling time feature - e.g. when any user of the compiler service, including VS IDE, asks for a tooltip - then the inheritance is resolved. But it is not resolved at .xml writing time. Also, at tooling time, it is resolved recursively. Code must be tested, the PR description must reflect the new design.

## Analysis

### Branch & PR
- **Branch:** `copilot/support-xmldoc-inherit-element`
- **PR:** [dotnet/fsharp#19188](https://github.com/dotnet/fsharp/pull/19188)

### Current State (commit 1ce64c799 + uncommitted working tree changes)
The previous agent has already performed the architectural refactor:

1. **XmlDocInheritance.fs** — Simplified from 585→211 lines. Now a pure module taking `resolveCref: string -> string option`. Zero TypedTree/CCU dependencies.
2. **Symbols.fs** — Added `buildCrefResolver` using SymbolEnv. Expansion at FCS API time via `FSharpEntity.XmlDoc` / `FSharpMemberOrFunctionOrValue.XmlDoc`.
3. **XmlDocFileWriter.fs** — Writes `<inheritdoc>` as-is to `.xml` files (no expansion). `tcImports` parameter removed.
4. **SymbolHelpers.fs** — No expansion (passes through unchanged).
5. **fsc.fs** — Updated `WriteXmlDocFile` call without `tcImports`.
6. **ILVerify baselines** — Updated for moved functions.
7. **Recursive resolution** — Already works at tooling time (tested by chained inheritance test).

**Uncommitted working tree changes (from previous agent, correct but not committed):**
- `Symbols.fs`: `docHasInheritDoc` fast-path optimization (skips resolver construction when no `<inheritdoc>` present)
- `XmlDocTests.fs`: Refactoring from verbose `[<Fact>]` tests to parameterized `[<Theory>]` tests with shared helpers

### What's Still Missing

1. **Test: XML file output preserves `<inheritdoc>`** — No test verifies `.xml` files keep `<inheritdoc>` verbatim when compiling with `--doc:`.
2. **Test: Recursive resolution at tooling time** — The chained test exists but we should verify it deeply.
3. **PR description** — Still says "compile-time XML generation: WORKS" (meaning expansion). Must reflect new design.
4. **Release notes** — Missing entry in `docs/release-notes/.FSharp.Compiler.Service/11.0.0.md`.
5. **Build + test verification** — Must confirm code builds and all tests pass.

## Approach

Two sprints:
1. **Sprint 01**: Add test verifying XML file preserves `<inheritdoc>` as-is. Verify build + existing tests pass. Add a resolver-based unit test for recursive expansion.
2. **Sprint 02**: Update PR description, release notes, and SPEC-TODO.MD to reflect new tooling-only design.

## Sprint Overview (restructured after parse failure in Sprint 01)
Original Sprint 01 was 16KB with heavily nested code blocks that caused a parse error at offset 15341.
Replaced with concise sprint files that describe WHAT to do without embedding large code samples.

| # | Name | Purpose |
|---|------|---------|
| 10 | Add_Tests_And_Commit | Add resolver-based unit tests + integration test; commit uncommitted perf optimization and test refactoring |
| 11 | Update_Docs_And_ReleaseNotes | Update PR description, release notes, SPEC-TODO.MD to reflect tooling-only design |

