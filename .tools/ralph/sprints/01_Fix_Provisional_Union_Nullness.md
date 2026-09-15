---
---

# Sprint: Fix false FS3261 for provisional union declarations

## Context

Implement [dotnet/fsharp issue #20211](https://github.com/dotnet/fsharp/issues/20211) using RED-first TDD. This file contains the complete task. No other sprint or backlog is required.

Work in `Q:\fsharp-worktrees\issue-876`. The directory name does not identify the issue being fixed. Planning found branch `fix/issue-20211` at commit `b5c530ed6bc42937de6363e3dcc104ebb833893d`, with no tracked changes.

Recursive declaration checking evaluates `not null` constraints before union-case tables are complete. `IsUnionTypeWithNullAsTrueValue` currently accepts every empty table. It therefore reports false FS3261 for ordinary unfinished unions, including structs.

The user reports 25 isolated compilations and six RED regressions on that commit. Planning reconfirmed the source and declaration ordering. The candidate is not yet implemented or proven GREEN. Reproduce RED locally before editing the compiler.

The issue's `Value` member, payload nullness, and imported Dictionary metadata are not the cause. The shared predicate is also used outside nullness checking. Preserve completed-union representation behavior.

## Description

### Scope, instructions, and files

Make the smallest correct, complete change. This sprint includes its own tests, validation, release note, review, and commit. Do not push, open a PR, post comments, or modify remote state.

Read these repository instructions before editing:

- `.github\copilot-instructions.md`
- `.github\instructions\ExpertReview.instructions.md`
- `.github\instructions\NoBloat.instructions.md`
- `.github\instructions\ComponentTests.instructions.md`

Expected product changes:

| File | Change |
|---|---|
| `src\Compiler\TypedTree\TypedTreeOps.Attributes.fs` | Move the attribute guard in `IsUnionTypeWithNullAsTrueValue`, near lines 1319-1327. |
| `tests\FSharp.Compiler.ComponentTests\Language\Nullness\NullableRegressionTests.fs` | Add one compact parameterized positive regression test and only missing negative controls. |
| `docs\release-notes\.FSharp.Compiler.Service\11.0.100.md` | Add one concise entry under `### Fixed`; reconfirm `VNEXT` before editing. |

Do not change public APIs, `.fsi` declarations, diagnostics, payload constraints, declaration passes, or project files. No new traversal, broad refactor, or unrelated baseline update is expected.

### Source map and required invariant

Read these existing functions before changing the predicate:

| Location at the starting commit | Relevant behavior |
|---|---|
| `src\Compiler\TypedTree\TypedTreeOps.Attributes.fs:1307-1329` | `TyconHasUseNullAsTrueValueAttribute`, `CanHaveUseNullAsTrueValueAttribute`, and `IsUnionTypeWithNullAsTrueValue`. |
| `src\Compiler\Checking\CheckDeclarations.fs:2950-3036` | `TcTyconDefnCore_Phase1B_EstablishBasicKind` creates `Construct.MakeUnionRepr []`. |
| `src\Compiler\Checking\CheckDeclarations.fs:3301-3408` | The second abbreviation pass uses `CheckCxs`; the first supertype pass publishes `entity_attribs`. |
| `src\Compiler\Checking\CheckDeclarations.fs:4413-4530` | Basic kind, unchecked aliases, attribute publication, checked aliases, then Phase1G representations. |
| `src\Compiler\Checking\ConstraintSolver.fs:2940-2965` | `SolveTypeUseNotSupportsNull` emits representation FS3261 through `TypeNullIsTrueValue`. |
| `src\Compiler\TypedTree\TypedTreeOps.Transforms.fs:496-501` | `TypeNullIsTrueValue` invokes the predicate and separately recognizes `unit`. |

The candidate is this rearrangement, using the existing formatting:

```fsharp
let IsUnionTypeWithNullAsTrueValue (g: TcGlobals) (tycon: Tycon) =
    (tycon.IsUnionTycon
     && TyconHasUseNullAsTrueValueAttribute g tycon
     && let ucs = tycon.UnionCasesArray in

        (ucs.Length = 0
         || (ucs |> Array.existsOne (fun uc -> uc.IsNullary)
             && ucs |> Array.exists (fun uc -> not uc.IsNullary))))
```

Required truth table:

| Union state | Result |
|---|---|
| No representation attribute, empty provisional cases | `false`, correcting the bug. |
| Representation attribute, empty provisional cases | `true`, preserving early genuine warnings. |
| No representation attribute, completed cases | `false`, unchanged. |
| Representation attribute, completed cases | Existing condition: exactly one nullary case and at least one non-nullary case. |
| Not a union | `false`, unchanged. |

Leave `CanHaveUseNullAsTrueValueAttribute` and the `unit` special case unchanged. Do not suppress warnings, special-case structs or Dictionary, inspect payload nullness, or defer constraints. Reconfirm attribute-publication timing for any counterexample before expanding the fix.

### Local prerequisites and durable evidence

Start with `git --no-pager status --short` and `git rev-parse HEAD`. Preserve unrelated work. Do not reset, clean the worktree, amend commits, or modify the planning files.

Planning found that the pinned SDK is missing. `global.json` requires `11.0.100-rc.1.26420.103` and selects Microsoft.Testing.Platform. Available system SDKs stopped at .NET 10. The default product target is `net11.0`.

Run these commands from the worktree in PowerShell:

```powershell
$env:BUILDING_USING_DOTNET = 'true'
dotnet --version
```

If SDK discovery fails, use the existing bootstrap script, then verify discovery again:

```powershell
& .\eng\common\dotnet.ps1 --version
$env:BUILDING_USING_DOTNET = 'true'
dotnet --version
dotnet msbuild tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj -getProperty:TargetFrameworks
```

Repeat the environment assignment in each fresh shell. Do not edit `global.json`, lower target frameworks, or replace the local compiler with an installed compiler. Read `eng\common\AGENTS.md` if investigating bootstrap scripts; do not edit Arcade-owned files.

Use the implementing agent's persistent session artifact directory for logs and checkpoints. Record the starting SHA, test commands, exit codes, test counts, and full diagnostics. Keep RED evidence before changing the compiler. Record the final SHA and review result. Do not commit logs or add a new test harness.

If build or bootstrap fails, capture the exact failure and resolve it before claiming RED or GREEN. Use `binlog-analysis` for MSBuild failures. Use `hypothesis-driven-debugging` for compiler or test failures. Clean only identified generated artifacts when a clean rebuild is needed. Never discard source changes or assume missing SDK output is a regression-test failure.

### RED: parameterized regressions using existing helpers

Add tests in module `Language.NullableRegressions`, in `NullableRegressionTests.fs`. Reuse `withVersionAndCheckNulls`. It enables FS3261, promotes warnings with `--warnaserror+`, and conditionally enables nullness.

Use a single `[<Theory>]` with source-string data for the valid cases. Prefer `[<InlineData>]`, or one small existing-style data binding if that is clearer. Do not create six copied test functions or a source-generation framework.

Give new test method names the prefix `Issue 20211` so one filter selects the complete new group. Each row must execute the compiler and assert both success and an empty diagnostic list:

```fsharp
// Attach source-string InlineData rows to this theory.
let ``Issue 20211 - ordinary union constraints during declaration checking`` source checknulls =
    FSharp source
    |> asLibrary
    |> withVersionAndCheckNulls ("preview", checknulls)
    |> (if checknulls then id else withOptions ["--checknulls-"])
    |> typecheck
    |> shouldSucceed
    |> withDiagnostics []
```

`typecheck` is a real compiler action and is sufficient for these diagnostics. Do not pipe a `CompilationUnit` directly into assertions. Do not use `ignoreWarnings`, `withNoWarn 3261`, or a warning-only success check.

Include all six RED sources below. Use `checknulls = true` for cases 1-5 and explicitly disable it for case 6. Each case must independently fail because of false representation FS3261 before the compiler change, not because of syntax or missing references.

**1. Exact issue source.** Preserve the original member and declaration order:

```fsharp
module rec M

open System.Collections.Generic

[<Struct>]
type Hole = Hole of string with
    member this.Value =
        let (Hole value) = this in value

type Substitution = Dictionary<Hole,obj>
```

**2. Exact follow-up source.** Preserve `module M` and `and Substitution`:

```fsharp
module M

open System.Collections.Generic

[<Struct>]
type Hole = Hole of string with
    member this.Value =
        let (Hole value) = this in value

and Substitution = Dictionary<Hole,obj>
```

**3. Ordinary reference union, no member.** This prevents a struct-only fix:

```fsharp
module rec M
open System.Collections.Generic
type Hole = Hole of string
type Substitution = Dictionary<Hole,obj>
```

**4. Generic struct union with nullable string contents.** Do not add `'T : not null`:

```fsharp
module rec M
open System.Collections.Generic
[<Struct>]
type Hole<'T> = Hole of 'T
type Substitution = Dictionary<Hole<string | null>,obj>
```

**5. Record representation checked before the struct union.** Keep the record first:

```fsharp
module rec M
open System.Collections.Generic
type Container = { Values: Dictionary<Hole,obj> }
[<Struct>]
type Hole = Hole of string
```

**6. Local F# constraint with `--checknulls-`.** Do not use Dictionary:

```fsharp
module rec M
type Keyed<'T when 'T : not null>() = class end
[<Struct>]
type Hole = Hole of string
type Substitution = Keyed<Hole>
```

Case 6 must still enable warning 3261 through the helper. Representation warnings are independent of the general nullness switch. If a proposed source does not reproduce, inspect the actual declaration path. Preserve the scenario instead of weakening the assertion.

### Controls: preserve valid inputs and genuine diagnostics

Run controls before and after the fix. Controls do not need to fail in the RED run.

**7. Non-recursive and completed unions remain valid.** Reuse case 1 with `module M` and separate `type Substitution`. Reuse completed-type controls instead of adding another assembly-building helper.

Existing `NullableReferenceTypesTests.fs` tests `WithNull on a DU` and `Nullness support for F# types` cover completed local unions. Their full diagnostic lists exclude warnings for valid ordinary-union uses. For the referenced-assembly boundary, a compact positive row using `Dictionary<Choice<string,int>,obj>` reuses the completed ordinary union in the existing FSharp.Core reference. Prefer an equivalent existing constrained completed-union test if one is present when implementing. Record the exact selected test. No synthetic C# library or duplicated multi-project setup is needed.

**8. Attributed provisional union still warns.** Add a negative control unless an equivalent same-group alias-before-union test exists:

```fsharp
module M
open System.Collections.Generic
type Substitution = Dictionary<Maybe,obj>
and [<CompilationRepresentation(CompilationRepresentationFlags.UseNullAsTrueValue)>] Maybe =
    | Missing
    | Present of string
```

Keep the constrained alias before the attributed union, within the same type group. This case must retain genuine representation FS3261. A completed option alone cannot replace it.

**9. Option as constrained key still warns.** Use `type Substitution = Dictionary<string option,obj>` in a small non-recursive module. Expect the genuine representation message:

```text
Nullness warning: The type 'string option' uses 'null' as a representation value but a non-null type is expected.
```

**10. Nullable string as constrained key still warns.** Use `type Substitution = Dictionary<(string | null),obj>`. Enable nullness and expect the supports-null message:

```text
Nullness warning: The type 'string | null' supports 'null' but a non-null type is expected.
```

For new negative cases, reuse the same configuration helper, then `typecheck |> shouldFail |> withDiagnostics [...]`. Capture the complete baseline diagnostic lists, including severity, ranges, order, and messages. Warning promotion means FS3261 is asserted as `Error 3261` with this helper. Do not mix in `Warning 3261` expectations from tests that do not promote warnings. Do not guess diagnostic counts or accept only an error code.

Prior art is in `NullableReferenceTypesTests.fs`: `Invalid usages of WithNull syntax`, `Strict func handling of obj type`, and `Notnull constraint and inline annotated value`. The last test already checks both representation and supports-null FS3261 through an explicit `not null` constraint. Reuse existing negatives when they cover the same requested path. Otherwise add only the missing key-control rows, preferably in one small negative theory. Do not rewrite existing baselines.

### Execute RED, apply the fix, and obtain GREEN

Use the component project, which references the local `FSharp.Compiler.Service` project. Start with the targeted Debug tests after adding tests but before editing the compiler:

```powershell
$env:BUILDING_USING_DOTNET = 'true'
dotnet test --project tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj -c Debug -- --filter-method "*Issue 20211*"
```

Save output showing each of the six required false-FS3261 failures. Do not count build errors or zero discovered tests as RED. Keep success assertions in the test source while recording the expected failing run.

Apply only the predicate guard change after RED is established. Immediately invoke the `fsharp-diagnostics` skill. Its local script supports these checks:

```powershell
& .\.github\skills\fsharp-diagnostics\scripts\get-fsharp-errors.ps1 -ParseOnly src\Compiler\TypedTree\TypedTreeOps.Attributes.fs
& .\.github\skills\fsharp-diagnostics\scripts\get-fsharp-errors.ps1 src\Compiler\TypedTree\TypedTreeOps.Attributes.fs
```

Resolve new parse or typecheck errors. Re-run the same targeted Debug command, allowing it to rebuild the compiler. Require all new tests to pass. Never use `--no-build` against a compiler built before the edit.

Then run the neighboring nullness modules:

```powershell
$env:BUILDING_USING_DOTNET = 'true'
dotnet test --project tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj -c Debug -- --filter-class "Language.NullableRegressions" --filter-class "Language.NullableReferenceTypes" --filter-class "Language.NullableCSharpImport"
```

Confirm discovered class names from the runner if they have changed. Keep the regression, neighboring constraint, and C# import coverage. Record nonzero test counts.

### Representation compatibility validation

Run these existing tests in Release because the group includes EmittedIL tests:

```powershell
$env:BUILDING_USING_DOTNET = 'true'
dotnet test --project tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj -c Release -- --filter-method "*UseNullAsTrueValue*" --filter-method "*Nullable attr for Option clones*"
```

Repeated inclusion filters select their union. If the installed runner requires different syntax, inspect its help and preserve the same five tests. Do not treat an empty selection as a pass.

The starting commit has these five selected tests:

| Test | File under `tests\FSharp.Compiler.ComponentTests` | Protects |
|---|---|---|
| `Is* discriminated union properties work with UseNullAsTrueValue` | `Language\DiscriminatedUnionTests.fs` | Compile-and-run behavior for a nullary attributed union case. |
| `NullAsTrueUnion01 - UseNullAsTrueValue with signature` | `Conformance\ImplementationFilesAndSignatureFiles\CheckingOfImplementationFiles.fs` | Signature/implementation representation agreement. |
| `E_UseNullAsTrueValue01_fs` | `Conformance\BasicGrammarElements\CustomAttributes\Basic\Basic.fs` | Invalid attribute shapes retain all FS1196 diagnostics. |
| `Csharp understands option like type using UseNullAsTrueValue` | `EmittedIL\Nullness\NullnessMetadata.fs` | C# observes existing nullable metadata correctly. |
| `Nullable attr for Option clones` | `EmittedIL\Nullness\NullnessMetadata.fs` | Existing IL baseline is unchanged. |

The user reports all five passed before this change. Require them to pass locally after the change without baseline updates. If final edits affect tested code, rebuild and repeat the affected runs. Escalate to broader suites only if targeted evidence requires it.

### Format, release note, expert review, and commit

Format only changed F# files, not the repository:

```powershell
dotnet fantomas src\Compiler\TypedTree\TypedTreeOps.Attributes.fs tests\FSharp.Compiler.ComponentTests\Language\Nullness\NullableRegressionTests.fs
```

If the tool is missing, restore the repository tool manifest and retry. Inspect the resulting diff. Exclude unrelated existing-file formatting while retaining the formatter output for changed code. Do not remove another contributor's changes.

Invoke the `release-notes` skill when the fix is complete. Recheck `gh variable get VNEXT --repo dotnet/fsharp`. Planning resolved version `11.0.100`.

Use the existing insertion helper with the explicit version file:

```powershell
dotnet fsi .github\skills\release-notes\pick-insert-line.fsx --file docs\release-notes\.FSharp.Compiler.Service\11.0.100.md --section Fixed
```

Insert one bullet at the suggested position, for example:

```markdown
* Fix false FS3261 nullness warnings for union types in recursive declaration groups. ([Issue #20211](https://github.com/dotnet/fsharp/issues/20211))
```

Do not invent a PR number or create a PR to obtain one. This is commit-only work.

Invoke the `reviewing-compiler-prs` skill for the final implementation and follow its `expert-reviewer` dispatch. This fulfills the requested expert review. Provide the final diff and RED/GREEN evidence. Request local findings only, with no remote comments or other publishing.

The review must check the attribute-publication timing, both provisional-union cases, completed-union compatibility, diagnostic severity, real test execution, and the local `--checknulls-` constraint. Resolve concrete correctness findings and repeat affected validation. Remove noisy comments, duplicate setup, and unnecessary helpers. Use `code-compaction` if the implementation becomes bloated.

Run `git diff --check` and inspect the exact staged diff. Stage only the compiler file, regression test file, and release note unless a proven counterexample required another directly related file. Do not stage runner files, logs, SDK files, or planning changes.

Commit the validated fix with a descriptive message and required trailers:

```text
Fix false nullness warnings for recursive union declarations

Require UseNullAsTrueValue before accepting provisional union cases.
Fixes #20211.

Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>
Copilot-Session: <implementing agent's actual session ID>
```

Use the session ID supplied to the implementing agent, not the literal placeholder. Verify the commit contains the intended files. Record the commit SHA and local validation evidence. Leave no task-owned uncommitted source changes. Do not push.

## Definition of Done

- Cases 1-6 each have recorded local RED failures caused by false representation FS3261 before the predicate change.
- Cases 1-6 run through one compact parameterized test and pass with an empty diagnostic list after the fix.
- The exact issue and comment sources retain the original `Value` member and their distinct declaration-group forms.
- The ordinary reference union, generic struct with nullable contents, and record-before-union paths pass without special cases or new payload constraints.
- The explicit local F# `not null` constraint passes with `--checknulls-` and warning 3261 still enabled.
- Case 7 covers the non-recursive original and a completed union from a preceding file or referenced assembly without duplicate assembly setup.
- Case 8 retains full expected representation FS3261 diagnostics for an attributed same-group union declared after its constrained alias.
- Cases 9-10 retain full expected representation and supports-null FS3261 diagnostics, with warning promotion handled consistently.
- `CanHaveUseNullAsTrueValueAttribute`, the `unit` special case, completed-union semantics, public APIs, and diagnostic definitions remain unchanged.
- The local compiler builds without new warnings, and `fsharp-diagnostics` reports no new parse or typecheck errors.
- Targeted regressions and neighboring nullness tests pass locally with nonzero discovered test counts.
- All five named runtime, signature, invalid-attribute, C# metadata, and IL-baseline controls pass in Release without baseline changes.
- Only changed F# files are formatted, and `git diff --check` passes without unrelated diff churn.
- One concise release-note bullet exists in the correct FSharp.Compiler.Service version file.
- Final expert review has completed, concrete correctness findings are resolved, and affected tests were rerun after subsequent changes.
- Persistent session artifacts contain commands, revisions, exit codes, full RED diagnostics, GREEN test counts, and review results.
- The validated fix is committed with the required trailers, with no task-owned source changes left uncommitted and no push or remote publication.
