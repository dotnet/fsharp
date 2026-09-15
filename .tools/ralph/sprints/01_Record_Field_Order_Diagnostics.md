---
---
# Sprint: Correct record field-order diagnostics with RED-first coverage

## Context - WHY this sprint exists

Fix [dotnet/fsharp issue #20410](https://github.com/dotnet/fsharp/issues/20410) in `Q:\fsharp-worktrees\issue-878`.
This sprint is the complete implementation unit. It has no dependency on another sprint or on `BACKLOG.md`.
Use minimal, surgical changes. Validate locally, then commit. Do not push or publish anything.

The starting compiler commit is `b5c530ed6bc42937de6363e3dcc104ebb833893d`, on branch `fix/issue-20410`.
Planning documents can be committed above that source revision. Inspect the actual worktree before editing.
Preserve other people's changes and all useful execution evidence.

`checkRecordFields` in `src\Compiler\Checking\SignatureConformance.fs` first compares record fields by name in both directions.
It then calls the diagnostic-emitting `checkField` positionally through `List.forall2`.
The first displaced field produces misleading FS0193 before the correct FS0312 on the implementation type.
`checkField` also copies XML documentation and updates paired source ranges before comparing names.
The positional call can therefore overwrite the correct same-name association.

The issue incorrectly calls the order diagnostic FS0313. The existing order diagnostic is **FS0312**.
FS0313 means a required field is missing. FS0311 means an implementation field is absent from the signature.
Do not change these numbers or their messages.
Record constructor parameter order depends on declaration order. A permutation must still fail compilation.

The request reports eighteen starting-main matrix compilations: seven expected RED assertions and eleven passing controls.
It also reports a separate original `ResolvedConfig` compilation with only positional FS0193 and type-level FS0312.
Those historical logs are not prerequisites. Capture fresh RED and GREEN evidence for the tests in this sprint.
The guard was not applied or proven GREEN during planning.

## Description - WHAT to implement with DETAILED guidance

### Files and repository rules

| Path, relative to the worktree | Purpose |
|---|---|
| `src\Compiler\Checking\SignatureConformance.fs` | Change only the final positional predicate in `checkRecordFields`, initially around lines 640-642. |
| `tests\FSharp.Compiler.ComponentTests\Conformance\Signatures\Signatures.fs` | Add one data-driven regression and separately named controls in `Conformance.Signatures.SignatureConformance`. |
| `docs\release-notes\.FSharp.Compiler.Service\11.0.100.md` | Add one concise `Fixed` entry after successful implementation. `VNEXT` was `11.0.100` during planning. |
| `tests\FSharp.Test.Utilities\Compiler.fs` | Read existing source-pair, compilation, and diagnostic helpers. Do not change this shared harness. |
| `src\Compiler\FSComp.txt` | Read diagnostic identities around lines 149-151. Do not edit diagnostic resources. |
| `.tools\ralph\evidence\issue-20410` | Preserve commands, source revision, fixtures, exit codes, selected test counts, and RED/GREEN logs. Do not commit generated logs. |

Read these instruction files before editing:

- `.github\instructions\ExpertReview.instructions.md`
- `.github\instructions\ComponentTests.instructions.md`
- `.github\instructions\NoBloat.instructions.md`

Read `docs\coding-standards.md` before interpreting compiler abbreviations.
Use existing helpers and F# formatting. Add no API, project, package, diagnostic, feature flag, or new name map.
The existing test file is already included in `tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj`.

### 1. Establish a usable local baseline

Run commands from `Q:\fsharp-worktrees\issue-878` in PowerShell.
Set `$env:BUILDING_USING_DOTNET = 'true'` in each fresh command process.
Do not change system-wide environment variables on a shared machine.

The planning probe `dotnet --version` failed because the pinned SDK was unavailable.
`global.json` requests SDK `11.0.100-rc.1.26420.103` and `Microsoft.Testing.Platform`.
After confirming the missing SDK, use the repository bootstrap:

```powershell
.\eng\common\dotnet.ps1 --version
```

This script installs the repository SDK and invokes it.
Use that SDK for all subsequent commands. If necessary, invoke each command through `.\eng\common\dotnet.ps1`.
Do not change `global.json`, dependency versions, or `eng\common` files to obtain a build.

Query target frameworks rather than assuming `net472` or another framework:

```powershell
$env:BUILDING_USING_DOTNET = 'true'
dotnet msbuild tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj -getProperty:TargetFrameworks
dotnet msbuild src\Compiler\FSharp.Compiler.Service.fsproj -getProperty:TargetFrameworks
```

Confirm the test-runner syntax with the installed SDK's help.
The command patterns below follow the component-test instructions.
If the runner requires a different placement of `--filter-method`, adjust only command syntax.
Confirm nonzero discovery and the intended test names. A zero-test success is not validation.

### 2. Write complete RED-first paired-source tests

Follow `Issue 11331 - Public constructor taking internal type should report FS0410 in signature` in `Signatures.fs`.
Share one small source-pair constructor, for example:

```fsharp
let private recordSignaturePair signature implementation =
    Fsi signature
    |> withAdditionalSourceFile (FsSource implementation)
    |> asLibrary
```

Include the same explicit module name in both source strings.
Keep default `.fsi` and `.fs` names paired. Apply options before `compile`.
Use `compile` to consume both sources.
Do not use plain `typecheck`: at this revision, it ignores `AdditionalSources`.
`typecheckProject` handles multiple sources, but changing to that result/assertion model is unnecessary here.

Use one `[<Theory>]` with data rows for the six primary cases: exact original, reduced swap, prefix cycle, equal types, generic struct, and attributes.
Use separately named controls for aligned records, real mismatches, different name sets, nullness, and non-record/recovery behavior.
Parameterize related controls rather than copying test bodies.
Include `Issue 20410` in new method names so one filter selects all new coverage.
Use compact inline fixtures and shared declarations. Do not add a separate source file for every row.

**Assert the entire ordered diagnostic list, including duplicates, severity, number, message, and range.**
`withErrorCode` and substring checks are insufficient.
`withDiagnostics` in `Compiler.fs` calls `assertErrors`, which deduplicates actual diagnostics by range and message.
It also normalizes whitespace. It cannot establish exact duplicate-warning preservation alone.

Use the existing raw result surface: `result.Output.Diagnostics`.
Project each diagnostic to a stable tuple and compare the complete expected and actual lists.
Include the source basename to distinguish implementation and signature ranges.
For example, project `d.Error`, `Path.GetFileName d.NativeRange.FileName`, `d.Range` coordinates, and `d.Message`.
Raw columns are zero-based. DSL `Col` expectations are one-based.
Choose one convention explicitly and do not mix them.
Normalize CRLF to LF if needed, but do not normalize message wording, filter diagnostics, sort, or deduplicate.
A small local exact-assertion helper is justified. Do not modify the shared assertion framework.
Existing raw-result usage is in `tests\FSharp.Compiler.ComponentTests\Attributes\CompiledNameMultipleValues.fs`.

Capture current diagnostics and source line layouts before setting control expectations.
Primary regression expectations must exclude only positional FS0193, not genuine diagnostics.
Freeze those expectations before editing production code.
For every invalid permutation, also assert compilation failure and the presence of an actual `Error 312`.

The harness treats warnings as failure by default, even without compiler `--warnaserror`.
For warning-only controls, assert the complete warning list and absence of errors.
Do not mistake the harness failure wrapper for a language error.
Do not use warning suppression, promotion, or `ignoreWarnings` to simplify the new expectations.

#### Required fixture and behavior matrix

| Case | Source guidance | Complete required outcome |
|---|---|---|
| Exact reported record | Preserve the `ResolvedConfig` declarations below. Define all three dependent types locally and identically in both sources. | Exactly one `Error 312` on the implementation `ResolvedConfig` identifier. No positional FS0193. |
| Reduced swap | Signature `type R = { A: int; B: string }`, implementation `type R = { B: string; A: int }`. | Exactly one `Error 312` on implementation `R`. |
| Shared prefix, three-field cycle | Signature `{ Prefix: bool; A: int; B: string; C: decimal }`, implementation `{ Prefix: bool; B: string; C: decimal; A: int }`. | Exactly one `Error 312`, with no error on the displaced field. |
| Equal-type swap | Use `{ A: int; B: int }` and `{ B: int; A: int }`. | Exactly one `Error 312`. Equal types do not make order legal. |
| Generic struct swap | Use `[<Struct>] type R<'T> = { A: 'T; B: 'T list }` and its reversed implementation. Put `[<Struct>]` in both sources. | Exactly one `Error 312`. Preserve generic remapping and struct representation checking. |
| Attribute conflict plus swap | Put `[<System.Obsolete("sig")>]` on named field `A` in the signature and `"impl"` on `A` in the implementation. Reverse `A` and `B`. | Preserve FS1200 at the attribute and FS0312 at the type. Remove only positional FS0193. Capture actual attribute-target multiplicity. FS1200 is a warning without promotion. |
| Aligned declarations | Compile the same record declaration on both sides. | Successful compilation and an empty raw diagnostic list. |
| Real same-name mismatch | Parameterize `A: int` versus `A: string`, immutable versus mutable `A`, and public versus `internal` record representation. Keep field names and order aligned. | Preserve the genuine field FS0193 and its exact message/range. No spurious FS0312. |
| Different name sets | Signature has `A; B`. For missing, implementation has only `A`. For extra, implementation adds `C`. For renamed, implementation has `A; C`. | Preserve complete lists with FS0313, FS0311, and FS0313 respectively. Do not convert these into order errors. |
| Nullness, aligned and prefix swap | Signature `{ Prefix: string; A: int; B: bool }`. Implementation changes `Prefix` to `string \| null`, then either keeps or swaps `A; B`. | Each variant retains three FS3261 warnings. Aligned has no errors. Prefix-swap adds only FS0312 after the fix. Count raw warnings, including identical duplicates. |
| Union, exception, object fields, recovery | Preserve starting-main controls described below. | Union and exception diagnostics remain unchanged. Reordered class fields can still compile. Malformed fields retain their diagnostics without a compiler crash. |

For nullness cases, use `withLangVersionPreview`, `withCheckNulls`, and `withWarnOn 3261`.
Do not copy the warning-promotion helper from the nullness test module.
FS1200 is emitted by `checkAttribs`, which also reconciles attributes during name-based checks.
Do not assume repeated attribute checks produce repeated FS1200 warnings. Assert the measured complete list.

The exact reported field permutation is:

```fsharp
// Signature declaration
type ResolvedConfig =
    {
        Config: FormatConfig
        Settings: ResolvedSetting list
        EditorConfigFiles: string list
        Problems: EditorConfigProblem list
    }

// Implementation declaration, in the separate implementation source
type ResolvedConfig =
    {
        Config: FormatConfig
        EditorConfigFiles: string list
        Problems: EditorConfigProblem list
        Settings: ResolvedSetting list
    }
```

Add a shared `module M` header and compact dependent definitions to each source.
For example, use distinct single-case unions for `FormatConfig`, `ResolvedSetting`, and `EditorConfigProblem`.
Keep their definitions identical between sources. Preserve all original record field names, types, and order.
Do not introduce Fantomas package dependencies.

The FS0312 message for `R` is:

```text
The type definitions for type 'R' in the signature and implementation are not compatible because the order of the fields is different in the signature and implementation
```

Use `ResolvedConfig` instead of `R` for that fixture.
Locate the implementation type identifier in each actual source string for its full expected range.
Do not copy ranges from a fixture with different leading lines.

For boundary coverage, reuse suitable existing controls when available.
Otherwise add compact data rows through the same source-pair helper:

- Named union fields: `type U = Case of A: int * B: string`, with the two named fields reversed in the implementation.
- Named exception fields: `exception E of A: int * B: string`, with fields reversed in the implementation.
- Explicit class fields: use `val` fields in a class declaration and reverse their order, keeping names, types, and accessibility identical.
- Malformed record fields: duplicate a field name, and retain the missing/extra cases above for unequal-length coverage.

Establish their exact current diagnostics before applying the guard.
Do not invent diagnostic numbers for these controls or claim that each is a new RED test.
The nullness prefix case intentionally loses only the same unwanted FS0193 as the primary cases.
Do not refactor recovered duplicate names or replace list-length behavior.

Run and retain the first RED result while `SignatureConformance.fs` still matches the starting source:

```powershell
$env:BUILDING_USING_DOTNET = 'true'
dotnet test --project tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj -c Debug -- --filter-method "*Issue 20410*"
```

Verify that failures are unwanted positional FS0193, not syntax errors, missing sources, stale binaries, or test-discovery failures.
Record each control's starting result. Preserve full messages, ranges, severities, and duplicate counts.
Do not assert that a specific total equals the historical eighteen compilations. The exact original fixture adds coverage.

### 3. Apply only the record positional guard

In `checkRecordFields`, keep both preceding `NameMap.suball2` expressions unchanged.
Keep the existing constructor-order comment, `List.forall2`, FS0312 error expression, location `m`, and false result.
Replace only the function passed to that final `List.forall2`.
Its body must short-circuit as follows:

```fsharp
implField.LogicalName = sigField.LogicalName
&& checkField aenv infoReader implTycon sigTycon implField sigField
```

Use the existing field type if lambda parameters require annotations.
When names differ, return false without calling `checkField`.
When names match, call the existing `checkField` exactly as before.
Do not replace the predicate with name equality alone.
Do not sort fields, add a set/map, accept reordering, suppress shared FS0193, or alter `checkField`.
Do not change `checkUnionCase`, `checkRecordFieldsForExn`, `checkClassFields`, or diagnostic resources.

Invoke the `fsharp-diagnostics` skill immediately after the compiler edit.
Run its parse check, then its typecheck for this file:

```powershell
.\.github\skills\fsharp-diagnostics\scripts\get-fsharp-errors.ps1 -ParseOnly src\Compiler\Checking\SignatureConformance.fs
.\.github\skills\fsharp-diagnostics\scripts\get-fsharp-errors.ps1 src\Compiler\Checking\SignatureConformance.fs
```

Fix errors before proceeding. A service diagnostic check does not replace compilation tests.

### 4. Prove GREEN and preserve siblings

Rebuild with the edited compiler and rerun the identical `*Issue 20410*` selection.
Do not use `--no-build` for the first run after production edits.
Retain the complete GREEN result beside RED evidence.
The invalid record programs must still fail compilation, but all regression assertions must pass.

Run nearby signature tests and these existing sibling selections:

| Existing test location | Selection |
|---|---|
| `Conformance\Signatures\Signatures.fs` | Class `Conformance.Signatures.SignatureConformance`, including `AttributeMatching01 - attribute mismatch between signature and implementation`. |
| `ErrorMessages\ExtendedDiagnosticDataTests.fs` | `FieldNotContainedDiagnosticExtendedData 01`, both `useTransparentCompiler` rows. |
| `Language\Nullness\NullableRegressionTests.fs` | `Signature conformance` and all three `Micro compilation` rows in `Language.NullableRegressions`. |

All paths in this table are under `tests\FSharp.Compiler.ComponentTests`.
The attribute test, two field-data rows, one signature-nullness row, and three micro rows form the seven reported sibling rows.
Verify their actual discovery locally rather than trusting this count.

Example commands after a fresh Debug build:

```powershell
$env:BUILDING_USING_DOTNET = 'true'
dotnet test --project tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj -c Debug --no-build -- --filter-class "Conformance.Signatures.*"
dotnet test --project tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj -c Debug --no-build -- --filter-method "*FieldNotContainedDiagnosticExtendedData*"
dotnet test --project tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj -c Debug --no-build -- --filter-method "*Signature conformance*" --filter-method "*Micro compilation*"
```

Confirm that repeated method selectors are a union in the installed runner.
If they are not, run the two nullness selections separately.
Start with these targeted tests. Escalate only if failures require wider coverage.
Use Release configuration if running the full component suite.

For build failures, invoke `binlog-analysis`, collect a binary log, and fix the cause.
Investigate stale bootstrap outputs before drawing conclusions about the source.
Clean only verified, task-owned build artifacts when needed. Do not remove repository files or another user's work.
A setup or build failure is not RED evidence for this diagnostic bug.

### 5. Format, review, document, and commit

Format only changed F# files:

```powershell
dotnet fantomas src\Compiler\Checking\SignatureConformance.fs tests\FSharp.Compiler.ComponentTests\Conformance\Signatures\Signatures.fs
```

Restore the declared Fantomas tool only if the command reports that the tool is missing.
Inspect formatter output. Retain only formatting required for changed code.
If whole-file formatting creates unrelated changes, undo only your formatter's unrelated edits.
Do not format the repository or refresh unrelated baselines.
Rerun the affected diagnostics and tests after final edits.

Invoke the `reviewing-compiler-prs` skill and the `expert-reviewer` agent on the final local implementation diff.
Keep the review local. Do not post comments, open a PR, or push.
Ask the review to verify record-only scope, same-name calls, source-range side effects, diagnostic multiplicity, and all controls.
Resolve concrete findings and rerun affected tests.
Apply `code-compaction` if the test diff is bloated, duplicated, or reaches its bug-fix size trigger.
Do not split implementation and tests into separate incomplete sprints.

Invoke `release-notes` after the fix passes.
Confirm `VNEXT` with `gh variable get VNEXT --repo dotnet/fsharp`.
Use the corresponding `.FSharp.Compiler.Service` file and its existing `Fixed` section.
Choose an insertion point with `.github\skills\release-notes\pick-insert-line.fsx`, passing the explicit version file.
Suggested entry:

```markdown
* Remove misleading FS0193 when record fields differ in order between a signature and its implementation, while retaining FS0312. ([Issue #20410](https://github.com/dotnet/fsharp/issues/20410))
```

There is no PR in this commit-only task. Use the real issue link, not a fabricated PR number.
Leave existing release-note entries unchanged.

Run `git diff --check` and inspect the complete implementation diff.
Keep the production change limited to the record predicate.
Remove temporary source probes and generated files that you created, but retain useful evidence under the named evidence directory.
Keep planning documents already tracked by the architect.
Stage only the compiler file, test file, and selected release-note file.
Commit the verified implementation with a descriptive message, for example `Fix misleading record field-order diagnostics (#20410)`.
Include the required Copilot trailers using the implementation session's ID.
Do not amend earlier commits or push.

### Evidence for the independent verifier

Record source HEAD, compiler-build configuration, exact commands, test discovery counts, exit codes, and log paths.
Keep RED and GREEN output in distinct files under `.tools\ralph\evidence\issue-20410`.
Record which tests reused existing controls and which tests added fixtures.
Preserve evidence when work continues in another execution window.
The verifier must inspect assertions and rerun the tests, not rely only on a prose success statement.
Do not mark this sprint complete if RED, GREEN, review resolution, or the implementation commit is missing.

## Definition of Done

- Before the production edit, local regression failures demonstrate unwanted positional FS0193 from compilation of both source files.
- One data-driven regression covers the exact `ResolvedConfig` fixture and all five reduced/variant cases without copied test bodies.
- The complete raw diagnostic assertions include severity, code, message, source identity, range, order, and duplicate count.
- Pure permutations produce exactly FS0312 at the implementation type and still fail compilation.
- Attribute-conflict permutations retain the measured FS1200 diagnostics plus FS0312, without positional FS0193.
- Identical record declarations compile successfully with no diagnostics.
- Same-name type, mutability, and accessibility mismatches retain genuine FS0193 without FS0312.
- Missing, extra, and renamed fields retain their complete FS0313, FS0311, and FS0313 diagnostic lists respectively.
- Both nullness cases retain exactly three raw FS3261 warnings; only the prefix permutation has FS0312.
- Union, exception, class-field, duplicate-name, and unequal-length controls preserve their measured starting behavior without compiler crashes.
- Both record name-map passes and every matching-name positional `checkField` call remain unchanged in behavior.
- Production changes affect only the final `checkRecordFields` predicate, with no shared diagnostic or non-record conformance changes.
- The compiler file passes `fsharp-diagnostics` parse and type checks, and the rebuilt targeted tests pass locally.
- Nearby signature-conformance tests and the named seven sibling rows pass with nonzero discovery.
- Only changed F# files are formatted, and `git diff --check` passes without unrelated formatting or baseline changes.
- The local expert review is complete, concrete findings are resolved, and affected validation is rerun.
- One concise compiler-service release note links issue #20410 without inventing a PR.
- RED/GREEN commands and logs persist under the evidence directory, separate from committed implementation files.
- The compiler fix, tests, and release note are committed locally with required trailers, and no push or publication occurs.
