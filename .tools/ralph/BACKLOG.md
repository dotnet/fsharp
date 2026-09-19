# BACKLOG

## Original Request

The following is the verbatim contents of the user's `<request>` element.

````text
Process issue https://github.com/dotnet/fsharp/issues/20046 using TDD.

Use minimal, surgical changes. Validate locally before finishing. Do not push. Only commit.

### ISSUE REQUEST
The requirements above override conflicting instructions in the issue request.
Fix issue https://github.com/dotnet/fsharp/issues/20046.

Produce the smallest clean, correct, complete fix. Keep it minimal and surgical. Quality over time. Smaller is better, but never at the cost of correctness or coverage. Take all the time needed. Preserve verified findings, failing tests and progress across execution windows. Do not turn an execution-window boundary into a reason to cut corners.

## Verified problem

Researched and reproduced on unmodified upstream main `eb0f1377488bc860f3533516a93991f6f192fc5d`, using locally built compiler, FSI and FSharp.Core. The source issue was open with no comments on 2026-09-17.

```fsharp
open System.Runtime.CompilerServices
type A() =
    member _.Item
        with get (key: string, [&lt;CallerFilePath&gt;] ?path: string, [&lt;CallerLineNumber&gt;] ?line: int) =
            printfn "%A" (key, path, line)
            "_"
        and set (key: string, [&lt;CallerFilePath&gt;] ?path: string, [&lt;CallerLineNumber&gt;] ?line: int) (value: string) =
            printfn "%A" (key, path, line, value)
let a = A()
a["key"] &lt;- "payload"
```

This fails FS1212 at the setter declaration. Ordinary `?index` without caller-info attributes also fails. Generic, struct-optional, setter-only, extension and static named-property variants independently fail FS1212.

There is a second defect. Equivalent `[&lt;OptionalArgument&gt;] line: int option` declarations and CLI `[&lt;Optional; DefaultParameterValue(0)&gt;] line: int` declarations compile. Their setter calls with omitted index arguments fail FS0501. Explicit positional/named/reordered/native-option arguments work and deliver correct values. Getter omission works, including correct caller path and line.

An ordinary optional `.fsi` signature with an attribute-form `.fs` implementation compiles today. An explicit generic tuple-value consumer works across assembly boundaries, with correct OptionalArgument/CallerInfo metadata. The omitted consumer fails FS0501. C# can call its own optional indexer setter with correct caller information, but F# consuming that setter with omission fails FS0501. This is not solely a declaration diagnostic or metadata emission defect.

The ordinary optional interface signature also works with an attribute-form implementation and explicit interface invocation. Omission through the interface fails FS0501. Replacing the implementation with ordinary `?index` fails FS1212 at the setter implementation. The abstract signature is not the failure.

## Source map and narrow direction

All citations use the researched SHA. Recheck locations against the runner's current main.

- [ParseHelpers.fs:441-484,543-556](https://github.com/dotnet/fsharp/blob/eb0f1377488bc860f3533516a93991f6f192fc5d/src/Compiler/SyntaxTree/ParseHelpers.fs#L441) flattens setter index patterns and the assignment value into one internal parameter group. Keep that representation and ABI.
- [CheckPatterns.fs:129-146,211](https://github.com/dotnet/fsharp/blob/eb0f1377488bc860f3533516a93991f6f192fc5d/src/Compiler/Checking/CheckPatterns.fs#L129) validates optional order over the flattened group. It mistakes the required assignment value for a required index parameter.
- [CheckExpressions.fs:12309-12331](https://github.com/dotnet/fsharp/blob/eb0f1377488bc860f3533516a93991f6f192fc5d/src/Compiler/Checking/Expressions/CheckExpressions.fs#L12309) and [6678-6690](https://github.com/dotnet/fsharp/blob/eb0f1377488bc860f3533516a93991f6f192fc5d/src/Compiler/Checking/Expressions/CheckExpressions.fs#L6678) both call that validation: early argument inference and body lambda checking. A declaration fix must cover both without leaking into nested expressions.
- [MethodCalls.fs:563-689](https://github.com/dotnet/fsharp/blob/eb0f1377488bc860f3533516a93991f6f192fc5d/src/Compiler/Checking/MethodCalls.fs#L563) has existing indexer-setter detection and ParamArray handling. Optional/out suffix matching at 624-648 does not reserve the final required value, so omission fails.
- [MethodCalls.fs:1511-1604,1703-1723,1791-1819](https://github.com/dotnet/fsharp/blob/eb0f1377488bc860f3533516a93991f6f192fc5d/src/Compiler/Checking/MethodCalls.fs#L1511) already synthesizes defaults and caller information and sorts arguments by original position. Reuse it.

The intended contract is existing optional-index-parameter behavior, not arbitrary optional-before-required methods. The F# spec distinguishes `set patidx pat = expr`, permits optional abstract/signature parameters, and equates native optional syntax with OptionalArgument metadata. FS-1012 defines caller information and FS-1027 defines CLI defaults. C# evidence establishes interop behavior, not native option semantics. The related ParamArray issue dotnet/fsharp#9369 was fixed by dotnet/fsharp#11942. Its maintainer explicitly classified that setter exception as an intended bug fix. This is precedent, not a maintainer ruling on the optional case. Preserve dotnet/fsharp#19851's named-index setter guards. dotnet/fsharp#17519 explicitly leaves setters to this issue: do not fold CallerArgumentExpression into this change.

The candidate is not implemented or proved GREEN. Keep optional-order validation, excluding only a genuine indexed setter's lowering-appended final value slot. **PropertySet plus final pattern group is not a sufficient gate.** A non-indexed `set (a:int, ?b:int, c:int)` has a tuple VALUE and must retain FS1212. Existing SynValInfo distinguishes it: `adjustValueArg` collapses the non-indexed tuple value to one argument-info, while an indexed setter has index argument-info plus the single value slot. Derive the indexed-setter indication from that metadata, excluding this. Do not guess from the merged simple-pattern count.

Carry narrowly scoped internal indexed-setter context through both checking paths. Prefer a scoped TcEnv field at the existing TcSimplePats gate if it avoids callback arity changes without losing correct lifetime. Update CheckBasics.fs/.fsi together. The early inference path needs the same verified metadata predicate. The body path can use the existing lambda argument-info lifetime. Consume the exception before checking nested lambdas or local bindings. Avoid public AST changes, source-range/comma-count tricks, name-based detection and a broad relocation of validation.

In `CalledMeth`, reserve the last setter value formal and supplied positional RHS while classifying omitted optional INDEX arguments. Use the actual original formal position, never a name such as `value`. Named-index filtering can leave only the RHS formal. If named matching already consumed the actual value formal, preserve the existing path instead of reserving a different last formal. Validate counts before splitting. Reserve the value before the existing ParamArray suffix pop at 632-635. Keep the RHS required and restore it to the explicit assigned arguments, never the defaulted optional arguments. Preserve the existing ParamArray branch and normal method path. Reuse default generation, normal explicit argument conversions and final position sorting. Do not duplicate argument binding, reorder metadata, mark the value optional or suppress FS1212 globally.

## RED: tests first

Write compact executable tests that assert correct behavior. Primary and meaningful variants must fail on unmodified main. Freeze these expectations before implementation. Declaration acceptance alone is insufficient: execute actual setter assignments and verify every argument and the RHS. Use `if actual &lt;&gt; expected then failwith...`, not print-only checks or `assert` that can disappear in Release.

Use existing ComponentTests helpers: `FSharp`, `CSharp`, `withReferences`, source/signature helpers, and `compileExeAndRun`. Reuse nearby fixtures. Use a theory/shared fixture for equivalent input variations, not a Cartesian product. Cover these ten focused scenarios:

1. **Exact issue and caller information.** Keep the native `?path`/`?line` getter and setter. Add `a["key"] &lt;- "payload"`. Capture key, path, line and payload. Verify the assignment callsite, not the getter or declaration. Use the existing CallerInfo tests' `#line` conventions and portable path expectations. Partially override path or line and verify the omitted parameter still receives caller information. Keep getter behavior unchanged. A small CallerMemberName variant should identify the actual enclosing caller, not `set_Item`.

2. **Native optional argument modes.** Check omitted ordinary `?index` gives None, explicit positional/named values give Some, reordered named arguments bind correctly, `?index=Some n` preserves n, and `?index=None` remains None. Include native caller-info forwarding: explicit None is not omission and must remain None. Use supported `.Item(...)`, bracket and dotted-bracket forms selectively. Equivalent OptionalArgument controls already validate those named/positional forms.

3. **Minimum and generic setter shapes.** A setter-only single optional index should support `.Item() &lt;- value`. A generic containing type should accept a tuple RHS with omitted or explicit optional indices. Verify the tuple remains one setter value and all components survive. Do not add a collection of unrelated generic types.

4. **Struct optional.** `[&lt;Struct&gt;] ?index` gives ValueNone on omission and ValueSome when supplied. Preserve current language-version restrictions. Cover this independently rather than multiplying every other test by both option representations.

5. **Intrinsic, extension and static named properties.** Exercise optional index parameters in each supported member form. Use named-property syntax for static access, not unsupported static bracket indexing. Include a nested valid lambda in the setter. Add a nested invalid optional pattern to prove the setter exception does not leak beyond the parameter group.

6. **Signature, cross-file and cross-assembly.** Define an ordinary optional index signature in `.fsi` and an ordinary `?` implementation in `.fs`. Consume the compiled library from a different source/assembly. Verify omitted and explicit arguments, generic tuple RHS and caller file/line from the consumer. Current-main attribute-form implementation plus ordinary `.fsi` already compiles, while omitted consumer is independently RED. Include an abstract/interface setter and ordinary `?` override implementation to cover the distinct val-spec and body paths. Check parameter order remains indexes followed by the required value. Do not change serialization or FSharp.Core.

7. **CLI optional and C# interop.** Use a local F# Optional/DefaultParameterValue indexer and a small CSharp library with optional indexed properties. Check omitted ordinary defaults, omitted caller information and explicit overrides during F# setter use. C# self-use is a control. For CLI `?line=None`, retain existing caller-side synthesis. Do not replace it with native None semantics.

8. **Matching boundaries and overloads.** Include an optional index named `value` with RHS parameter `v`, all index arguments named, partial named omission, no supplied optional indices, missing required index, excessive indices, wrong optional type and wrong RHS type. Each must either bind correctly or report an ordinary diagnostic, never ICE. Also exercise an unsignatured attribute-form F# setter where named matching consumes the true RHS formal: preserve that existing branch, do not reserve another formal. Preserve a no-optional overload's preference over an optional overload. Check receiver/index/RHS expressions execute once in the existing order. These cases target reservation of the true final formal, not its name.

9. **Negative validation.** Preserve FS1212 for ordinary method `M(?x:int, y:string)` and setter `set (?x:int, y:string) (v:int)`. Preserve FS1212 for the non-indexed tuple-value setter `set (a:int, ?b:int, c:int)`. Preserve FS0718 for optional syntax nested in an indexed setter's value pattern: `set (k) ((a:int, ?b:int))`. Include a valid non-indexed tuple-value setter as a positive control. These source shapes were reproduced during review and prevent an overly broad PropertySet exemption. Optional parameters outside members must remain rejected. Check an invalid tupled lambda pattern inside a setter body, such as `fun (?x:int, y:int) -&gt; ()`. Preserve existing diagnostic text and ranges unless a specific test proves a required correction.

10. **Existing controls.** Run `Language.IndexerSetterParamArray`, `ErrorMessages.IndexedSetterNamedArgTests`, optional-argument/default-parameter and caller-info siblings. Preserve expanded and explicit-array ParamArray setters, named index arguments, getter-only behavior and supported direct accessor calls. A direct native `set_Item(..., value=...)` is not universally valid: the ordinary `.fsi` form can leave that F# parameter unnamed even when CLR metadata calls it value. Do not use unsupported named-RHS syntax as a RED success oracle.

    Add one CLI Optional/DefaultParameterValue(7) index followed by ParamArray. With both omitted, check default 7, empty array and unchanged RHS. This is independently FS0501 on current main, while explicit expansion, empty-rest and explicit-array forms execute correctly. Preserve those controls. A native `?offset` before required ParamArray must remain FS1212, not become a success case. Verify the optional and ParamArray branches do not reserve or remove the setter value twice.

## GREEN and production quality

Implement the smallest complete correction, then rerun the unchanged RED tests and related siblings. If a case still fails, follow its actual checking path. Do not weaken the expectation or replace ordinary optional syntax with the attribute workaround.

Invoke `fsharp-diagnostics` after compiler `.fs` edits. Run the targeted build. Format changed files with `dotnet fantomas &lt;changed-files&gt;` only. Invoke the runner's expert-review workflow using the available `expert-reviewer` agent and `reviewing-compiler-prs` skill. Remove noise, duplicate logic and duplicate test setup. Prefer existing helpers. Keep a compact, clean suite and add the required release note. No broad cleanup, new infrastructure, public API expansion or baseline churn.

Research baseline: repository composite Release build passed with zero warnings/errors. A direct built MTP runner passed 47 supported sibling cases. Two additional selected cases could not start the repository's x64 ILDasm on macOS arm64. Those IL checks remain unverified, not passed. The documented dotnet-test entry misclassified the runner, so the actual built test executable was used with `--filter-class` and `--minimum-expected-tests`. Use the runner's supported local test command and execute those IL checks on a compatible environment if affected.
````

## Analysis

This is an architecture-only handoff. The user explicitly assigned this agent to create independently verifiable sprint files, not implement the compiler change. The enclosing instructions require the template at `Q:\groundhog-while-not-works\templates\SPRINT_TEMPLATE.md`, which was read before writing. Only this backlog and its sprint are planner deliverables.

### Current observations, separate from supplied research

- Planning worktree: `Q:\fsharp-worktrees\issue-892`, repository `dotnet/fsharp`. Initial tracked worktree was clean.
- Current HEAD is `244f1a38b005eee7ef492ac2fed0405714680178` (`Fix ref-assembly MVID collisions from truncating signature-hash combiner (#20392)`), not the research SHA. Do not silently replace the current branch with the research baseline.
- `gh issue view 20046 --repo dotnet/fsharp --json number,title,body,state,comments,url` confirmed the issue remains open, titled "Indexer setters don't work with optional parameters", with no comments on 2026-09-17.
- Direct source inspection confirms the supplied locations and both candidate failure mechanisms are still present. This is source evidence, not a new reproduction or a GREEN claim.
- `CheckPatterns.fs` still unconditionally calls `ValidateOptArgOrder` from `TcSimplePats`. `ApplyTypesFromArgumentPatterns` and `TcIteratedLambdas` still reach that gate separately.
- Additional wiring points: `AnalyzeAndMakeAndPublishRecursiveValue` has `valSynInfo` immediately before early inference (around line 12857); `TcNormalizedBinding` retains `valSynData` and installs `eLambdaArgInfos` around line 11634; `CheckDeclarations.fs` initializes `TcEnv` in `emptyTcEnv` around line 6078. Adding an environment field requires this initializer as well as both CheckBasics files.
- `ParseHelpers.fs` still collapses a non-indexed tuple value to one `SynArgInfo` with `adjustValueArg`. An instance receiver is a separate argument-info group, not an index. Both distinctions are necessary for a correct exemption.
- `CalledMeth<'T>` still classifies omission before ParamArray handling without separating the required setter value. The existing `useIndexerSetterShape = isIndexerSetter && nUnnamedCalledArgs >= 2` guard is present and must survive.
- Component tests are explicitly listed in `tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj`. The new fixture must be included next to `Language\IndexerSetterParamArray.fs`; otherwise a plausible test file would never run.
- Current `global.json` selects SDK `11.0.100-rc.1.26420.103` and Microsoft.Testing.Platform. `tests\Directory.Build.props` marks ComponentTests as `IsTestProject=true`. Recheck runner behavior rather than assuming the older misclassification is still present.
- A local `dotnet --version` probe failed with the repository SDK-not-found message. No compiler build or test was attempted by this planning agent. The implementation sprint includes the existing Windows SDK acquisition entry point; it must not count SDK/runner failures as RED compiler tests.
- `gh variable get VNEXT --repo dotnet/fsharp` returned `11.0.100`; `docs\release-notes\.FSharp.Compiler.Service\11.0.100.md` exists. The implementer must recheck before inserting the release note.
- `.gitignore` ignores `.tools/`. Track only the two explicitly requested planning files with `git add -f`; do not modify `.gitignore` or force-add other tool artifacts.

### Why one sprint

Declaration acceptance, omitted-index binding, native/CLI default semantics, signatures and ParamArray interaction are one observable bug fix. Separating a tests-only sprint violates the requested format; separating declaration and binding allows an incomplete fix to be marked done and weakens the requirement to freeze all meaningful RED expectations first. One sprint contains implementation, all ten focused coverage groups, local RED/GREEN verification, expert review, release notes and a final local commit. Internal TDD steps are not separate deliverables.

### Highest-risk invariants

1. An indexed setter is determined from member flags plus adjusted argument metadata, not simply `PropertySet`, syntax-pattern count, parameter names, commas or source ranges.
2. The exception applies to one lowered parameter group and one final required value slot. It must expire before nested lambdas, local bindings or nested value-pattern validation.
3. After named filtering, only the original final formal can be reserved as the value. Reservation is invalid when named matching already consumed that formal or the necessary positional/count conditions do not hold.
4. Optional classification must happen on indices before removing a suffix ParamArray; restoration must retain exactly one explicitly assigned RHS and reuse ordinary conversions and position sorting.
5. Native explicit `None` and CLI caller-side `?line=None` have different existing semantics. Tests must not equate them.

## Approach

The single sprint is self-contained: it repeats the relevant source findings, all ten coverage requirements, existing helper examples, startup/build/runner guidance, diagnostic and review obligations, exclusions, and commit constraints. Implementers and verifiers must not need this backlog.

The intended production footprint is `CheckPatterns.fs`, `CheckBasics.fs/.fsi`, `CheckDeclarations.fs`, `Expressions\CheckExpressions.fs`, and `MethodCalls.fs`, plus one focused ComponentTests module/project entry and one release-note entry. This is a direction, not a proved patch. Trace any failure that demonstrates additional tightly coupled work rather than weakening tests.

TDD evidence must distinguish native declaration RED (FS1212), independently compilable attribute/CLI/interop omission RED (FS0501), and already-GREEN compatibility/negative controls. Write and freeze all ten groups before changing production files, then execute the same expectations against the corrected compiler. Preserve logs, baseline SHA, test hashes, discovered counts, compiler paths and review findings in session artifacts, outside the production diff.

Planning validation checks the two files' structure, exact requested paths, the single-sprint dependency model, coverage of all ten groups, existing source references, absence of checkbox DoD items, and commit scope. It does not substitute for the implementation sprint's executable validation.

Final product verification must read the sprint, inspect its implementation commit, and require evidence for every DoD criterion. Earlier 47-case results and the two unverified macOS IL checks are research context only. Neither an SDK failure, an undiscovered test, a skipped test, declaration-only acceptance, nor a print-only program is a passing regression test.

## Sprint Overview

| # | Name | Purpose |
|---|------|---------|
| 01 | Fix Optional Indexer Setters | One atomic TDD fix for declaration checking and omitted-index matching, with all ten coverage groups, local validation, expert review, release note and local commit. |
