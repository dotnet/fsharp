---
---

# Sprint: Fix optional indexer setters end to end with TDD

## Context - WHY this sprint exists

Fix https://github.com/dotnet/fsharp/issues/20046 in `Q:\fsharp-worktrees\issue-892` (`dotnet/fsharp`). This file is the complete implementation and verification assignment; no other sprint or backlog is required. Implement the smallest clean, correct, complete correction, validate locally, and commit locally. Do not push, open a PR, post review comments, or otherwise write to GitHub.

The architecture agent inspected clean HEAD `244f1a38b005eee7ef492ac2fed0405714680178` on 2026-09-17. The issue was open with no comments. The user's runtime research used unmodified upstream `eb0f1377488bc860f3533516a93991f6f192fc5d`, a locally built compiler, FSI and FSharp.Core. The current source still contains the described mechanisms, but no implementation is proved GREEN. Recheck the actual starting HEAD; do not reset to the research SHA.

Two defects must be fixed together:

```fsharp
open System.Runtime.CompilerServices

type A() =
    member _.Item
        with get (key: string, [<CallerFilePath>] ?path: string, [<CallerLineNumber>] ?line: int) =
            printfn "%A" (key, path, line)
            "_"
        and set (key: string, [<CallerFilePath>] ?path: string, [<CallerLineNumber>] ?line: int) (value: string) =
            printfn "%A" (key, path, line, value)

let a = A()
a["key"] <- "payload"
```

Native optional setter parameters currently produce FS1212 at the declaration because lowering appends the required assignment value to the index parameter group. Plain `?index`, generic, struct-optional, setter-only, extension and static named-property forms also fail.

Equivalent `[<OptionalArgument>] index: int option` and CLI `[<Optional; DefaultParameterValue(0)>] index: int` declarations compile, but omitted-index setter calls produce FS0501. Explicit positional, named, reordered and native-option calls work. Getter omission and getter caller information work. An ordinary optional `.fsi` or interface signature works with an attribute-form implementation and explicit consumption; omission fails independently. C# self-use works, while F# omission against a C# indexer fails. Thus suppressing a declaration diagnostic or changing emitted metadata is not a complete fix.

The language contract is existing optional INDEX parameters, not arbitrary optional-before-required methods. The spec distinguishes `set patidx pat = expr`; FS-1012 covers caller information and FS-1027 CLI defaults. ParamArray issue #9369, fixed by #11942 as an intended bug fix, is precedent, not a maintainer decision on this optional case. Preserve #19851's named-index safeguards. #17519 leaves setters to this issue; do not add CallerArgumentExpression support here.

## Description - WHAT to implement with DETAILED guidance

### Working rules and startup

Read `DEVGUIDE.md`, `docs\coding-standards.md` when interpreting compiler abbreviations, and these applicable instructions before changing code:

- `.github\instructions\FSharp.instructions.md`
- `.github\instructions\NoBloat.instructions.md`
- `.github\instructions\ExpertReview.instructions.md`
- `.github\instructions\ComponentTests.instructions.md`

Use the `hypothesis-driven-debugging` skill for reproduction/debugging. Preserve verified findings, failing tests and progress across execution windows in your session artifact directory. Record actual commands, baseline SHA, compiler paths, test names/counts, diagnostic results, and frozen test-file hashes there; no new tracking infrastructure or planning documents belong in the production change. Resume from that evidence instead of weakening coverage to fit an execution window.

Inspect `git status --short` first. Preserve others' work and the tracked architecture files under `.tools\ralph`. This is one atomic sprint: all tests, both fixes, review and release notes belong here. Do not deliver a declaration-only change or substitute attribute syntax for the requested native syntax.

The planning agent's `dotnet --version` failed because the SDK selected in `global.json` was missing. At inspection it was `11.0.100-rc.1.26420.103`. This was not a compiler/test failure. Use the existing Windows bootstrap after confirming the current environment:

```powershell
Set-Location Q:\fsharp-worktrees\issue-892
git --no-pager status --short
git rev-parse HEAD
dotnet --version
# If the repository SDK is missing, use its existing acquisition wrapper:
& .\eng\common\dotnet.ps1 --version
$env:BUILDING_USING_DOTNET = "true"
dotnet msbuild .\tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj -getProperty:TargetFrameworks
```

Re-establish process-local environment variables in fresh shells. Prefer repository-local SDK acquisition; do not change global.json, feeds, package versions, machine-wide environment settings or test infrastructure to bypass a failed invocation.

### Files and exact tracing points

All paths below are relative to the worktree. Line numbers are navigation aids at the inspected HEAD; locate functions again if they move.

| Path | Function or region | Role |
|------|--------------------|------|
| `src\Compiler\SyntaxTree\ParseHelpers.fs` | `adjustValueArg` and setter normalization, lines 441-484 and 543-556 | Read-only explanation of merged index/value patterns and adjusted `SynValInfo`. Preserve this representation. |
| `src\Compiler\Checking\CheckPatterns.fs` | `ValidateOptArgOrder`, `TcSimplePats`, lines 129-146 and 211 | Keep the existing validation gate; exclude only a proven indexed setter's final appended value slot. |
| `src\Compiler\Checking\CheckBasics.fs` and `src\Compiler\Checking\CheckBasics.fsi` | `TcEnv`, alongside `eLambdaArgInfos` | If using scoped indexed-setter context, update implementation and declaration together. |
| `src\Compiler\Checking\CheckDeclarations.fs` | `emptyTcEnv`, around 6078-6095 | Initialize new context to the non-setter state; search for any other record constructions. |
| `src\Compiler\Checking\Expressions\CheckExpressions.fs` | `AnalyzeAndMakeAndPublishRecursiveValue`, around 12857, and `ApplyTypesFromArgumentPatterns`, 12309-12331 | `valSynInfo` is available before the early call. Apply the same verified metadata predicate and scope to early inference. |
| `src\Compiler\Checking\Expressions\CheckExpressions.fs` | `TcNormalizedBinding`, around 11422 and 11634; `TcIteratedLambdas`, 6678-6736 | Binding has `valSynData`; RHS setup installs `eLambdaArgInfos`; lambda checking consumes one group at a time. Use this lifetime without leaking to nested expressions. |
| `src\Compiler\Checking\MethodCalls.fs` | `CalledMeth<'T>`, 563-719 | Existing `isIndexerSetter`, named filtering, optional/out suffix classification and ParamArray conversion. Correct omission matching here. |
| `src\Compiler\Checking\MethodCalls.fs` | `GetDefaultExpressionForCallerSideOptionalArg`, `GetDefaultExpressionForCalleeSideOptionalArg`, `GetDefaultExpressionForOptionalArg`, `AdjustCallerArgsForOptionals`, `AdjustCallerArgs` | Reuse default/caller-info synthesis, explicit conversions, and final `Position` sorting. Do not introduce a parallel binder. |
| `tests\FSharp.Compiler.ComponentTests\Language\IndexerSetterOptionalArguments.fs` | New focused `Language.IndexerSetterOptionalArguments` module | Keep compact executable regression fixtures and related negative cases together. Use theories/shared source for equivalent cases. |
| `tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj` | `<Compile Include="Language\IndexerSetterParamArray.fs" />`, around 375 | Register the new module beside this entry. This project uses explicit compile includes. |
| `docs\release-notes\.FSharp.Compiler.Service\11.0.100.md` | `### Fixed` | Current release-note target; recheck VNEXT before editing. |

### RED: write and freeze coverage before production edits

Write all ten focused groups below before changing compiler files. Every new positive regression test must express its final success expectation, not expect the bug's error. Execute these against the unmodified current compiler and record why they fail. Negative/compatibility controls should already pass. Independently prove the FS0501 omission defect using attribute/CLI/signature/C# forms whose declarations compile, rather than having every test stop at FS1212. Where needed, use a shared theory for native versus equivalent attribute-form library implementation, retaining both cases in GREEN.

Do not count a failing test-project build, absent SDK, undiscovered tests, missing IL tool or fixture syntax mistake as RED. Capture the tested compiler identity; do not accidentally use SDK FSI or an old FCS build. Freeze expectation-bearing test content before production edits. If a genuinely invalid test fixture is discovered, correct it with an explanation and re-establish its baseline evidence; never quietly lower an expected result.

Follow the ComponentTests DSL: create, configure, perform an action, then assert. Runtime tests use `FSharp`, `asExe`, `compileExeAndRun`, `shouldSucceed`, and explicit failures:

```fsharp
[<Fact>]
let ``Issue 20046 - omitted optional setter index`` () =
    FSharp """
module Program
type T() =
    member val Seen: int option * string = Some -1, "" with get, set
    member t.Item
        with set (?index: int) (v: string) =
            t.Seen <- index, v

[<EntryPoint>]
let main _ =
    let t = T()
    t.Item() <- "payload"
    if t.Seen <> (None, "payload") then failwith "Unexpected setter arguments"
    0
"""
    |> asExe
    |> compileExeAndRun
    |> shouldSucceed
```

This is a starting fixture shape, not a substitute for the other assertions. Do not use print-only checks or `assert` inside generated programs. Assert each assignment's arguments and RHS so a later assignment cannot hide a failure. Use `typecheck |> shouldFail |> withDiagnostics [...]` for diagnostic-only cases; capture exact baseline codes/messages/ranges where preservation is required. Do not introduce broad `ignoreWarnings` to hide mistakes.

Existing fixtures to reuse/read:

- `tests\FSharp.Compiler.ComponentTests\Language\IndexerSetterParamArray.fs`: executable intrinsic/extension, expanded/explicit-array indexer setter controls.
- `tests\FSharp.Compiler.ComponentTests\ErrorMessages\IndexedSetterNamedArgTests.fs`: named/reordered/bracket/direct-accessor call shapes and the #19851 no-ICE guards.
- `tests\FSharp.Compiler.ComponentTests\Conformance\SpecialAttributesAndTypes\CallerInfo.fs`: namespace `Conformance.SpecialAttributesAndTypes`, module `CallerInfo`; shared `CSharp` library, `withReferences`, explicit caller assertions and remapping directives such as `# 345 "qwerty"`.
- `tests\FSharp.Compiler.ComponentTests\Conformance\BasicGrammarElements\MemberDefinitions\OptionalArguments\OptionalArguments.fs`: module `MemberDefinitions_OptionalArguments`; native/struct optional tests and language-version-9 controls, including `ValueOption optional parameters via ?param syntax fail with langversion 9`.
- `tests\FSharp.Compiler.ComponentTests\Conformance\BasicGrammarElements\MemberDefinitions\OptionalDefaultParamArgs\OptionalDefaultParamArgs.fs`: module `MemberDefinitions_OptionalDefaultParamArgs`; library/reference and CLI default controls.
- `tests\FSharp.Compiler.ComponentTests\Signatures\TypeTests.fs`, around 765-801: `Fsi sigSource |> withAdditionalSourceFile (FsSourceWithFileName "Lib.fs" implSource)` and a distinct consumer source. Use existing `Fsi`, `FsSourceWithFileName`, `withAdditionalSourceFiles`, `asLibrary`, `withName` and `withReferences` instead of writing a new compiler/filesystem harness.

#### 1. Exact issue, caller file/line/member and getter preservation

Retain the native `?path`/`?line` getter AND setter from the issue; replace print-only observation with captured state and execute `a["key"] <- "payload"`. Assert key, caller path, caller line and payload for the assignment, not its declaration or a getter. Use different remapped filenames/lines for declaration, getter and consumer so an incorrect source location cannot pass accidentally.

Follow CallerInfo's line-remapping conventions. For a precise portable path check, construct an absolute logical filename with `System.IO.Path.Combine` in the host fixture and inject that known path into the source directive and expected result; do not hardcode this Windows worktree path. Keep the asserted assignment immediately under its fixed `#line` directive. Verify getter omission still gives the getter callsite's information.

Make one call overriding path only and one overriding line only; the other omitted field must still receive caller information. A small separate CallerMemberName variation must report the named enclosing consumer member/function, not `set_Item` or the declaration's context.

#### 2. Native optional argument modes

Capture all indices and payload. Check ordinary omission yields `None`; positional and named ordinary values yield `Some`; reordered named indices bind to their intended formals; `?index=Some n` preserves `n`; `?index=None` remains `None`. Include native caller-info forwarding with explicit `None`: it is not omission and must not be synthesized.

Use `.Item(...)`, `a[...]` and `a.[...]` selectively where supported, not a Cartesian product. Keep equivalent `[<OptionalArgument>] index: int option` controls for explicit/named/reordered/native-option calls that already work, with identical value assertions.

#### 3. Setter-only minimum and generic tuple RHS

The minimum setter-only `member _.Item with set (?index: int) (v: string)` must support `t.Item() <- "payload"` as in the fixture above. Use one generic containing type for a tuple-valued property; both omitted and explicit optional indices must preserve every tuple component as one required setter value. Do not flatten the tuple RHS into additional index parameters or add unrelated generic type families.

#### 4. Struct optional and language-version restrictions

Use a separate `[<Struct>] ?index` fixture: omission yields `ValueNone`; an ordinary supplied value and supported explicit `?index=ValueSome n` forwarding yield `ValueSome n`; explicit `ValueNone` remains `ValueNone`. Assert RHS too. Follow the existing OptionalArguments tests to preserve version-9 rejection/option-versus-voption behavior; do not gate ordinary optional setter support on a new language flag or weaken the existing struct-optional feature restriction.

#### 5. Intrinsic, extension and static named properties; scope lifetime

Execute an intrinsic indexer, an extension indexer and a static named indexed property using ordinary optional syntax. Use supported static named-property access such as `T.Indexed(...) <- rhs`, not static bracket indexing. Share fixture setup where only member/call spelling differs.

Have a setter use a valid nested lambda/local binding and verify the result. Include a separate invalid nested optional-pattern test (also specified in group 9) to prove the new context does not make ordinary lambdas or local functions permissive.

#### 6. Ordinary signature/implementation, cross-file/assembly and interface

Define an ordinary optional `.fsi` signature and an ordinary `?` `.fs` implementation for the generic tuple-value fixture. Compile a library from those sources and consume it from a separate compilation/assembly via `withReferences`. Also use the existing additional-source helpers to exercise a distinct consumer file in one compilation where appropriate. Verify omitted and explicit arguments, every tuple component, and file/line belonging to the consumer.

Keep an equivalent attribute-form implementation under the ordinary `.fsi` as an independent omission RED case: its declaration and explicit consumer already work on the research baseline. Native implementation must remain a final positive case, not be replaced by this control.

Include an abstract/interface indexed setter with ordinary optional signature and ordinary `?` override/explicit-interface implementation; execute omission and explicit calls through the interface. This covers separate val-spec and body paths. Preserve metadata with a focused parameter/reflection check against the compiled generic library: indices in original order followed by exactly one required tuple value; OptionalArgument and CallerInfo attributes attached to indices, never to the value. Compare F# native optional metadata with its existing attribute-form control; do not assume native optional metadata has the same CLI `IsOptional` semantics as `[<Optional>]`.

#### 7. CLI defaults and C# interop

Use local F# `[<Optional; DefaultParameterValue(...)>]` indexed parameters and a small `CSharp """...""" |> withName "..."` library containing optional indexer setters. C# syntax can use `public string this[string key, int index = 7] { set { ... } }`; keep caller-info parameters on an appropriate separate or shared small indexer fixture using `System.Runtime.CompilerServices`.

From F#, execute omitted ordinary defaults, omitted caller information and explicit overrides; assert the received indices/caller location/RHS. Have C# self-use execute and verify its own result as a control. Native option semantics do not define the CLI case: for CLI caller-side `?line=None`, preserve existing default/caller-information synthesis. Do not turn this into a native `None` assertion.

#### 8. Reservation boundaries, named `value`, overloads and evaluation

Use an optional INDEX actually named `value` and an RHS formal named `v` to catch name-based reservation. Assert all indices named, partial named omission, and no supplied optional indices. All-named filtering may leave only the final RHS formal; this is valid and must not require two remaining unnamed formals just to classify omission.

Exercise missing required index, too many indices, wrong optional type and wrong RHS type separately. Require the appropriate ordinary diagnostics, not an exception/ICE. Derive exact diagnostics from baseline-valid declaration forms where a native declaration error would mask the call error.

Preserve the existing unusual named-binding branch with an unsignatured attribute-form F# setter where named matching consumes the actual final RHS formal. Establish its executed result on unmodified current source, then freeze that control. This is distinct from ordinary named index arguments: do not blindly reserve whichever formal is left last. Do not invent a named-RHS success expectation for a signature that does not expose that F# parameter name.

Include a non-optional overload and an otherwise applicable optional overload; the non-optional overload must retain preference. Record receiver/index/RHS effects and counts using a compact event list, with an explicit baseline-working attribute-form call as control; the corrected omitted/native call must keep each supplied expression single-evaluation and the existing evaluation order. Include enough distinct values/events to detect swaps or duplicate RHS evaluation.

#### 9. Negative optional-order and nested-pattern validation

Freeze the existing diagnostic text and ranges for these independent source cases before implementation:

| Source shape | Required result |
|--------------|-----------------|
| Ordinary member `M(?x:int, y:string)` | FS1212 |
| Indexed setter `set (?x:int, y:string) (v:int)` | FS1212: the later required INDEX is still illegal |
| Non-indexed tuple-VALUE setter `set (a:int, ?b:int, c:int)` | FS1212, despite `PropertySet` and multiple merged simple patterns |
| Indexed setter with nested tuple value `set (k) ((a:int, ?b:int))` | FS0718 |
| Ordinary function/local binding with optional syntax outside a member | Still rejected with baseline diagnostics |
| Invalid lambda inside a setter, `fun (?x:int, y:int) -> ()` | Still rejected with baseline diagnostics; no context leakage |
| Native `?offset` index followed by required `[<ParamArray>]` index | FS1212, not a newly permitted declaration |

Also execute a valid non-indexed tuple-value setter and verify every component, so metadata discrimination does not break this existing shape. Do not change FSComp resources or diagnostics merely to make snapshots match.

#### 10. Existing controls plus optional-before-ParamArray interaction

Run the existing classes listed under local validation below, retaining expanded/explicit-array ParamArray setters, named index arguments, getter-only behavior and supported direct accessor calls.

Add a CLI setter with `[<Optional; DefaultParameterValue(7)>] offset: int` followed by `[<ParamArray>] rest: int[]`, followed by the separate required assignment value. Execute:

- Both indices omitted via supported `.Item()` syntax: default offset `7`, empty array, original RHS. This is an independent FS0501 RED case.
- Explicit offset plus expanded rest.
- Explicit offset plus empty rest.
- Explicit offset plus explicit array.

Assert offset, complete array and RHS on each call. The last three were executable baseline controls in the research. Neither optional classification nor ParamArray expansion may reserve/remove the value twice. Keep the native optional-before-required-ParamArray form in group 9 negative.

Supported direct accessors such as positional `t.set_Item(...)` are controls only where the original source form supports them. Ordinary `.fsi` signatures may leave the F# RHS parameter unnamed even if CLR metadata calls it `value`; do not use `set_Item(..., value=...)` as a universal success oracle.

### GREEN: smallest correction and invariants

#### Declaration checking

Retain the lowered representation and `ValidateOptArgOrder` at `TcSimplePats`. Exclude exactly the appended final value slot only when metadata proves the current group is a genuine indexed setter group.

The essential predicate combines `SynMemberFlags.MemberKind = SynMemberKind.PropertySet` and adjusted `SynValInfo` argument groups, excluding the instance receiver group. In `ParseHelpers.fs`, `adjustValueArg` collapses a non-indexed tuple VALUE to one argument-info, whereas an indexed setter has index argument-info plus that single value slot. `PropertySet` plus final pushed-pattern group is NOT enough; counting merged simple patterns is NOT enough. Reuse the same verified predicate in both checking paths rather than two subtly different approximations.

Prefer a narrowly scoped internal `TcEnv` field if it avoids changing the `TcSimplePats` callback arity across the compiler. Update `CheckBasics.fs`, `CheckBasics.fsi`, and `CheckDeclarations.emptyTcEnv` together. At early inference, `AnalyzeAndMakeAndPublishRecursiveValue` has `valSynInfo`; ensure `ApplyTypesFromArgumentPatterns` enables the exception only for the relevant final argument group, not the receiver or every pushed pattern. At body checking, `TcNormalizedBinding` has `valSynData` and installs `eLambdaArgInfos`; `TcIteratedLambdas` already consumes these infos group by group.

The environment lifetime must end before checking nested lambdas, nested value patterns, local bindings or other expressions. A whole-body "setter mode" is incorrect. Clear/consume the context at the group boundary and verify both recursion and non-lambda exits. Continue ordinary optional-order and optional-members-only validation for everything else. A setter value that itself contains optional syntax remains illegal.

No public AST changes, source-range/comma heuristics, name-based setter detection in pattern checking, broad relocation of validation, serialization changes or FSharp.Core changes. This is an intended compatibility bug fix, not a new arbitrary optional-before-required feature.

#### Omitted argument matching

In `CalledMeth<'T>`, retain the existing `isIndexerSetter` detection and named filtering. Use the actual final formal's original `CalledArg.Position` from `fullCalledArgs`, not a parameter name and not the last remaining formal after filtering.

For an eligible indexed setter candidate, reserve the true final formal and the final supplied positional RHS while classifying omitted INDEX arguments. Before any `frontAndBack`, `splitAt`, indexing or slicing, validate required nonempty/count conditions. Named filtering can leave only the RHS formal, which is valid. Conversely, if named matching already consumed that original final formal, retain the existing normal matching branch; do not reserve a different index as the value. Missing/excess/ill-typed candidates must fail through ordinary overload diagnostics, not throw.

Classify the remaining supplied positional indices against remaining index formals. Reserve the value BEFORE the existing optional/out suffix code pops a trailing ParamArray, so CLI optional offset plus omitted ParamArray works. Preserve existing optional/out exclusivity checks and fallback for required suffix parameters. Restore the value to explicitly assigned arguments in its original final position; it must never enter `UnnamedCalledOptArgs` or be defaulted.

Preserve the existing ParamArray branch and #19851's `useIndexerSetterShape = isIndexerSetter && nUnnamedCalledArgs >= 2` guard. Restoring one RHS before existing ParamArray processing must not create a second reservation/removal. Keep the ordinary method path unchanged. Reuse `AdjustCallerArgsForOptionals`, normal explicit argument conversions, default-expression helpers and `AdjustCallerArgs`' final position sorting.

Do not build a second binder, duplicate default/caller-info synthesis, reorder metadata, make the required value optional, change overload preference, or silently catch invalid list operations. If a stale comment claims indexed ParamArray setters cannot have optional/named arguments, correct that directly affected claim without narrating the entire fix.

### Local validation, review and release note

After compiler `.fs` edits invoke `fsharp-diagnostics`; resolve parse/type errors before the targeted build. Run a targeted Debug build of `src\Compiler\FSharp.Compiler.Service.fsproj` with `BUILDING_USING_DOTNET=true`. A Release ComponentTests build and execution then validates the same changes under the configuration needed by IL controls. Build from the current source, not cached research binaries.

Use the current Microsoft.Testing.Platform entry point. These are repository-guided commands, not claims that the planning agent ran them:

```powershell
$env:BUILDING_USING_DOTNET = "true"
dotnet build .\src\Compiler\FSharp.Compiler.Service.fsproj -c Debug
dotnet build .\tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj -c Release
dotnet test --project .\tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj -c Release --no-build -- --filter-class "Language.IndexerSetterOptionalArguments" --minimum-expected-tests 1
```

Use that build/run pattern for baseline RED with only test changes and then for GREEN with production changes. Before final execution replace the example minimum `1` with the actual discovered count for the frozen selection. Verify every theory row/group ran; an exit code alone is insufficient.

Current `global.json` selects MTP; `tests\Directory.Build.props` sets `IsTestProject=true` for ComponentTests. The research runner was misclassified by `dotnet test`; do not assume that persists or mistake it for a compiler bug. If the supported entry fails to launch/discover correctly, obtain the built runner path with `dotnet msbuild ... -p:Configuration=Release -p:TargetFramework=<actual-TFM> -getProperty:TargetPath`, inspect its `--help`, and run the built executable (or its DLL with the repository dotnet host) directly using `--filter-class` and `--minimum-expected-tests`. Use an actual resolved framework/path, not a guessed `net10.0` directory. Record which invocation really executes.

Run these sibling class selections, preferably together using the runner's supported filter syntax; otherwise invoke each class with a meaningful minimum count:

| Class/filter | Purpose |
|--------------|---------|
| `Language.IndexerSetterParamArray` | Intrinsic/extension and expanded/explicit-array setter preservation |
| `FSharp.Compiler.ComponentTests.ErrorMessages.IndexedSetterNamedArgTests` | #19851 named/reordered/getter/direct-accessor guards |
| `Conformance.BasicGrammarElements.MemberDefinitions_OptionalArguments` | Native/struct optional arguments and language gates |
| `Conformance.BasicGrammarElements.MemberDefinitions_OptionalDefaultParamArgs` | CLI default and interface/library controls |
| `Conformance.SpecialAttributesAndTypes.CallerInfo` | Caller path/line/member, overrides and getter-independent semantics |

Identify related optional/out overload-resolution tests in `Conformance\BasicGrammarElements\MethodResolution` and run the relevant discovered parent cases if matching changes affect them. `.fs` files there can be fixture inputs rather than test classes; inspect the registered parent instead of filtering by a filename and claiming success.

For a missing bootstrap or a needed compiler/Core composite validation, the existing Windows entry point is `eng\Build.ps1`; inspect its current help and use the no-Visual-Studio/dotnet path, e.g. `& .\eng\Build.ps1 -configuration Release -restore -build -bootstrap -noVisualStudio -msbuildEngine dotnet`. Do not introduce a build script. If the runner identifies you as the Copilot Coding Agent, perform its mandatory final Release/coreclr validation using the compatible Windows `eng\Build.ps1` entry with `-testCoreClr` in addition to the targeted checks.

The user's research composite Release build had zero warnings/errors and its direct MTP runner passed 47 supported siblings. Two selected IL cases could not start x64 ILDasm on macOS arm64; they are UNVERIFIED, not passed. Identify the current IL-using selected tests (including struct optional caller-info verification) and execute affected checks on this compatible Windows setup. A skipped/unlaunchable IL test is not a pass. Do not discard IL checks or update baselines to conceal an emitted-shape change.

On actual build failure use `binlog-analysis`, inspect the local binlog and correct the cause. Verify bootstrap freshness; before any artifact cleanup preserve evidence and inspect the exact generated paths, never erase source or unrelated work. Rebuild and rerun tests after corrections. Invoke `hypothesis-driven-debugging` for test failures rather than declaring the environment at fault.

Format only the changed F# files with `dotnet fantomas <changed-files>` (explicit paths, not `dotnet fantomas .`). Restore the existing dotnet tool only if the formatting command reports it missing. Rebuild/rerun after formatting or review changes. Use `code-compaction` if the diff meets its size/duplication triggers; retain all behavioral assertions while removing duplicate setup, explanations and speculative helpers. No broad cleanup or baseline churn.

Invoke the `reviewing-compiler-prs` skill and the available `expert-reviewer` agent on the actual compiler/test diff. Give the reviewer this sprint's metadata-discrimination, lifetime, original-position reservation, named-RHS-consumed, ParamArray, native/CLI, ABI and test requirements. Require local read-only findings; override any workflow step that would post to GitHub. Address concrete defects, then rerun affected tests and the unchanged frozen regression selection. Preserve the review result in session artifacts. Do not substitute this plan for review of the implementation.

Invoke `release-notes` when the fix is complete. Recheck `gh variable get VNEXT --repo dotnet/fsharp`; at planning it returned `11.0.100`. The compiler sink is `docs\release-notes\.FSharp.Compiler.Service\11.0.100.md`. Use the skill's `pick-insert-line.fsx` helper with the explicit target path and `--section Fixed`, then add one concise entry describing optional indexer setters/default and caller-info omission, linked to issue #20046. No PR exists as part of this local-only task: do not fabricate a PR number, add a placeholder PR URL, open a PR or set labels. No public API change is intended; internal `.fsi` synchronization is required but an API-baseline update is not.

Before committing, inspect `git diff --check`, the full scoped diff and the staged paths. Stage only the surgical source/tests/project/release-note changes. Do not add session logs, generated output, or unrelated work. Commit with a descriptive message and trailer:

```text
Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>
```

Do not amend someone else's commit and do not push. Retain local RED/GREEN/review evidence across the final handoff and report the resulting local commit and exact executed checks without labeling unverified work complete.

## Definition of Done - bullet list starting with '- '

- All ten numbered coverage groups are present in registered ComponentTests; equivalent cases share fixture/theory setup without a Cartesian product.
- Before production edits, native declaration cases demonstrably failed with FS1212, independent compilable omission cases failed with FS0501, and compatibility/negative controls passed; baseline SHA, commands, counts and frozen expectation hashes are preserved.
- The unchanged positive regression expectations execute setter assignments and verify every index/caller-info field and RHS using Release-safe failures; declaration acceptance or print output alone does not count.
- The exact native caller-file/line getter-and-setter scenario passes, including partial overrides, getter callsite preservation and actual enclosing CallerMemberName.
- Native omitted/explicit/named/reordered/Some/None modes pass, and explicit native caller-info None is not synthesized.
- Setter-only omission, a generic containing type with intact tuple RHS, struct optional modes and existing language-version restrictions pass.
- Intrinsic, extension and static named indexed setters pass; valid nested lambdas work and invalid nested optional patterns remain rejected.
- Ordinary `.fsi` plus ordinary `?` implementation passes cross-file and cross-assembly execution with omitted/explicit indices and consumer caller location; the independent attribute-form control also passes.
- An ordinary optional interface/abstract signature with ordinary `?` implementation passes actual interface setter calls; compiled parameter order/required RHS and index metadata remain unchanged.
- F# CLI default and C# interop setters pass omission/override checks, C# self-use passes, and CLI `?line=None` retains caller-side synthesis.
- Named-index `value`, all-named filtering, partial/no optional indices and the named-consumed-true-RHS control preserve correct bindings; invalid argument shapes produce ordinary diagnostics, not ICEs.
- Non-optional overload preference and single evaluation of receiver/index/RHS in the existing order are preserved.
- Ordinary optional-before-required methods and indices, non-indexed optional tuple-value setter, nested optional tuple value, ordinary/local optional syntax and native optional-before-required-ParamArray retain their baseline diagnostic codes/text/ranges; the valid tuple-value setter executes correctly.
- CLI default-7-plus-ParamArray omission yields 7, an empty array and the unchanged RHS, while explicit expansion, empty-rest and explicit-array controls pass with exactly one value assignment.
- Both early inference and body checking use metadata-proven indexed-setter context confined to the correct parameter group; non-indexed tuple values and nested expressions cannot receive the exemption.
- CalledMeth reservation uses the true final formal's original position, guards list/count operations, skips reservation if that formal was named-consumed, and preserves the normal/ParamArray paths, explicit conversions and existing default/position-sorting helpers.
- fsharp-diagnostics reports no introduced compiler errors, the targeted compiler build succeeds without introduced warnings/errors, and the final Release regression and named sibling selections pass with recorded discovery/execution counts.
- Affected IL checks run on a compatible local environment and pass; no missing, skipped, undiscovered or unlaunchable test is represented as passed.
- Only changed F# files are formatted; local expert-review workflow has run on the actual diff and concrete findings are addressed and revalidated.
- One release-note entry is added in the verified compiler-service target; there is no public AST/API expansion, serialization/FSharp.Core change, new infrastructure, unrelated cleanup or unexplained baseline churn.
- The scoped diff passes git diff --check, contains no session/generated artifacts, and is committed locally with the Co-authored-by trailer; no push or GitHub write occurred.
