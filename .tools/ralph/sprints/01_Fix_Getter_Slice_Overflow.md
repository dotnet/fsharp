---
---

# Sprint: Fix getter slice overflow with RED-first regressions

## Context - WHY this sprint exists

Fix https://github.com/dotnet/fsharp/issues/20530 in `Q:\fsharp-worktrees\issue-875`.
This sprint is the complete implementation unit. It has no prerequisite sprint.
You receive only this file. All implementation boundaries and acceptance requirements are below.
Use TDD, validate locally, and commit the result. Do not push, open a PR, or post GitHub comments.

The planned base is `b5c530ed6bc42937de6363e3dcc104ebb833893d`, on branch `fix/issue-20530`.
Inspect the actual HEAD and working tree before editing. Preserve other agents' work and runner-owned files.
Do not reset or replace the branch to match this document.

F# tolerant getter slices use inclusive bounds.
After clipping to the source dimension, disjoint bounds must return an empty result.
The current getters calculate `finish - start + 1` with unchecked `int` arithmetic.
Extreme reversed bounds can wrap to a positive allocation count.
Fixed getters also loop with an unclamped count. A count of `Int32.MinValue` makes `len - 1` wrap.
The shared `ComputeSlice` additionally overflows at legal based-array endpoints because it calculates the exclusive bound `bound + length`.

The request reports 17 safe failing assertions per previously tested Core target, ten passing syntax controls, and 33 passing sibling tests.
Those results are background evidence, not this sprint's RED record.
The candidate helper has not been compiled or proved GREEN.
An interval proof and 14,504 model cases support its arithmetic but do not validate inline consumer behavior.

Reference semantics: [FS-1077 tolerant slicing](https://github.com/fsharp/fslang-design/blob/7e3f0db7dcf1daa9486c7c87d3f5a398d460f56b/FSharp-5.0/FS-1077-tolerant-slicing.md).
The fix does not depend on fsharp/fslang-design#849.

## Description - WHAT to implement with DETAILED guidance

### Files and repository rules

All relative paths below start at `Q:\fsharp-worktrees\issue-875`.

| Path | Action |
|---|---|
| `src\FSharp.Core\prim-types.fs` | Add one implementation-only getter normalizer and replace getter count calculations. |
| `tests\FSharp.Core.UnitTests\FSharp.Core\OperatorsModule1.fs` | Add compact regression cases beside the existing intrinsic slicing tests, currently near lines 43-95. |
| `docs\release-notes\.FSharp.Core\11.0.100.md` | Add one concise `Fixed` entry after validation. Recheck the current `VNEXT` value. |

Read `.github\instructions\FSharpCore.instructions.md` and its linked `docs\fsharp-core-notes.md` before changing Core.
Follow `.github\instructions\NoBloat.instructions.md` for compact code and test setup.
Read any additional instructions that apply to the actual files you touch.
Use `hypothesis-driven-debugging` for RED/failure investigation.
Use `binlog-analysis` if a build fails. Diagnose the recorded binlog and repair the cause before continuing.
Do not edit `eng\common` to repair local setup. Those files are maintained by Arcade.

The existing project is `tests\FSharp.Core.UnitTests\FSharp.Core.UnitTests.fsproj`.
The fixture is `FSharp.Core.UnitTests.Operators.OperatorsModule1`.
It uses xUnit and `FSharp.Core.UnitTests.LibraryTestFx`, including `Assert.AreEqual` and `CheckThrowsNullRefException`.
Follow `OptimizedRangesGetArraySlice`, `OptimizedRangesGetArraySlice2D`, and `OptimizedRangesGetStringSlice`.
No new test project, project-file entry, package, public API, or compiler change is expected.

### 1. Establish the local baseline and durable evidence

Create an ignored evidence directory at `.tools\ralph\evidence\issue-20530`.
Preserve commands, exit codes, test discovery/counts, failures, binlogs, and Core assembly identities there.
Use distinct RED, GREEN, and control log names. Do not overwrite RED evidence during later runs.
Record the source commit and whether `prim-types.fs` was unchanged for each RED run.
Record unresolved work before an execution window ends. Resume from that evidence instead of skipping validation.
Do not commit logs, temporary consumers, package caches, or build output.

The planning-time SDK probe failed before MSBuild could start.
`global.json` currently requires `11.0.100-rc.1.26420.103`.
If that SDK remains missing, run the existing acquisition wrapper:

```powershell
Set-Location 'Q:\fsharp-worktrees\issue-875'
& .\eng\common\dotnet.ps1 --info
```

Use the acquired SDK consistently. If it is installed under `.dotnet`, invoke `.\.dotnet\dotnet.exe`.
Below, `dotnet` means that matching SDK, not an unrelated SDK or FSI installation.
Read target frameworks rather than inferring them from the issue's earlier two-target evidence:

```powershell
$env:BUILDING_USING_DOTNET = 'true'
dotnet msbuild src\FSharp.Core\FSharp.Core.fsproj -nologo -getProperty:TargetFrameworks
dotnet msbuild tests\FSharp.Core.UnitTests\FSharp.Core.UnitTests.fsproj -nologo -getProperty:TargetFrameworks,FSharpCoreShippedNetTargetFramework
```

At the planned base, Core targets `netstandard2.0`, `netstandard2.1`, and `net10.0`.
The product/CoreCLR test runtime is `net11.0`. CoreCLR unit tests normally reference the shipped `net10.0` Core.
Core assembly target and test-host runtime are different dimensions.
The test project also builds `netstandard2.1` for a surface-area test. Building it alone does not execute its getters.

Use the repository composite build for Core and the compiler:

```powershell
.\build.cmd -c Debug -noVisualStudio
```

`build.cmd` delegates to `eng\Build.ps1 -restore -build`. The no-Visual-Studio route builds `FSharp.slnx`.
Stop on a nonzero exit code and retain its binlog.
Do not replace the composite with a standalone Core build as the final implementation check.
If bootstrap contamination is diagnosed, preserve evidence before cleaning only the worktree's resolved `artifacts` output and rebuilding.

After a successful build, run the existing sibling selection against unchanged Core:

```powershell
dotnet exec artifacts\bin\FSharp.Core.UnitTests\Debug\net11.0\FSharp.Core.UnitTests.dll --filter-method "*SlicingOutOfBounds" --filter-method "*Fixed*"
```

Substitute the evaluated product TFM if it differs.
The request's starting result was 33 passing tests for this selection.
Record the actual discovery and outcomes. Explain any difference rather than claiming the old count.
Use the executable's `--help` if runner options differ. Never accept zero discovered tests.
The repository uses xUnit v3/Microsoft.Testing.Platform, not a VSTest filter expression.

### 2. Add correct, allocation-safe tests and record RED

Add the tests before editing `prim-types.fs`.
Use separate theory rows or facts so one exception does not prevent all other cases from executing.
Prefer typed `unit -> System.Array` thunks and a small shape assertion over reflection-driven test infrastructure.
A single local shape helper can check `Rank`, every `GetLength(d)`, and every `GetLowerBound(d)`.
Do not assert only total `Length` or use an empty assertion that cannot distinguish `[0;3]` from `[0;0]`.
Use `Assert.AreEqual` or established xUnit assertions. Include a useful case identifier in theory data.

Use `hi = Int32.MaxValue` and `lo = Int32.MinValue`.
Keep sources tiny, for example dimensions `[2;3]`, `[2;3;4]`, and `[2;3;4;5]`.
Use distinct dimension lengths so preserved axes are observable.
Set valid fixed indices, normally zero, unless the case specifically checks validation timing.

**Allocation safety:** never run an array getter with `3..lo`.
That reproducer can request a huge allocation before the fix.
The string `"hello"[3..lo]` is safe.
For arrays, `hi..lo` wraps to count two and `hi..(lo + 1)` wraps to count three.
The fixed-loop case `1..lo` allocates an empty result before the broken loop enters.
Do not add stress allocations or a Cartesian product of element types, wrappers, bounds, and platforms.

Implement these scenario groups:

| Group | Required cases and assertions |
|---|---|
| Original strings | `"hello"[3..lo]` and `"hello"[hi..lo]` both equal `String.Empty`, with no exception. |
| One-dimensional arrays | `[\|1;2;3\|][hi..lo]` and `[\|1;2;3\|][hi..(lo + 1)]` both have rank 1, length 0, and lower bound 0. |
| Full-rank getters | Rotate `hi..lo` through all axes. Other bounds are omitted. Check 2D shapes `[0;3]`, `[2;0]`; 3D shapes `[0;3;4]`, `[2;0;4]`, `[2;3;0]`; and 4D shapes `[0;3;4;5]`, `[2;0;4;5]`, `[2;3;0;5]`, `[2;3;4;0]`. Only one axis becomes empty. |
| Fixed-index getters | Cover each of the six implementation bodies using the representative wrapper cases below. Check reduced rank and all retained lengths. Wrong nonempty shapes are failures even if the call does not throw. |
| Fixed-loop boundary | Repeat a representative 2D, 3D, and 4D fixed case with `1..lo`. Keep other retained axes nonempty and check exact shapes. The call must return without source element access. |
| Based arrays | Use tiny positive-based and negative-based sources. `hi..lo` on a retained axis must produce a zero-based result with only that axis empty. Separately assert values for an ordinary valid negative absolute-index slice. |
| Inclusive upper endpoint | Use lengths `[1;2]` with lower bounds `[hi;0]`. A first-axis slice through `lo` must have shape `[0;2]`. Also use lengths `[2;2]` and bounds `[hi - 1;0]`; slice first-axis `hi - 1 .. hi - 1` and assert shape `[1;2]` and copied values. Its source ends at `hi`, but the finish lies before that endpoint. |
| Empty source endpoint | Use lengths `[0;2]` with bounds `[lo;0]`. Request first-axis start `hi`, first with finish `lo`, then with omitted finish. Assert shape `[0;2]`, rank 2, and zero lower bounds, without element access. |
| Negative controls | Reuse existing list, nearby nonoverflow, omitted-bound, clipping, empty/copy identity, null, setter, and reverse-slice coverage. Preserve fixed-index validation timing as described below. Do not count these controls as RED regressions. |

Representative fixed cases, using the source dimensions above:

| Underlying body | Public wrapper / ordinary syntax | Expected dimensions |
|---|---|---|
| `GetArraySlice2DFixed` | `GetArraySlice2DFixed1` / `a2[0, hi..lo]` | `[0]` |
| `GetArraySlice3DFixedSingle` | `GetArraySlice3DFixedSingle1` / `a3[0, hi..lo, *]` | `[0;4]` |
| `GetArraySlice3DFixedDouble` | `GetArraySlice3DFixedDouble1` / `a3[0, 0, hi..lo]` | `[0]` |
| `GetArraySlice4DFixedSingle` | `GetArraySlice4DFixedSingle1` / `a4[0, hi..lo, *, *]` | `[0;4;5]` |
| `GetArraySlice4DFixedDouble` | `GetArraySlice4DFixedDouble1` / `a4[0, 0, hi..lo, *]` | `[0;5]` |
| `GetArraySlice4DFixedTriple` | `GetArraySlice4DFixedTriple4` / `a4[0, 0, 0, hi..lo]` | `[0]` |

The generic fixed bodies are not exposed in `prim-types.fsi`.
Call their numbered wrappers through `Operators.OperatorIntrinsics`, following the existing tests.
Do not add declarations to expose the helper or these internal bodies.
One representative wrapper per body is enough. Existing tests exercise the other wrappers.

For based fixtures, use `Array2D.initBased` or `Array.CreateInstance(typeof<int>, lengths, lowerBounds) :?> int[,]`.
For example, lower bounds `[-3;5]`, lengths `[2;3]`, and values `100 * i + j` support an ordinary `[-3..-2, *]` control.
Check its `[2;3]` result, zero lower bounds, and values at the corresponding absolute source indices.
Use full-rank or known-correct 2D paths for nonempty based controls.
Do not accidentally turn these tests into a repair of the excluded fixed-getter offset defects.

Existing control locations, all under `tests\FSharp.Core.UnitTests\FSharp.Core`:

| File | Existing coverage to reuse |
|---|---|
| `OperatorsModule1.fs` | Intrinsic array/string getters, normal null exception, and 1D-4D setters. |
| `Microsoft.FSharp.Collections\ArrayModule.fs` | `SlicingOutOfBounds`, omitted bounds, empty arrays, fresh nonempty copies, and referentially equivalent empty slices. |
| `Microsoft.FSharp.Collections\Array2Module.fs` | `SlicingBoundedStartEnd`, `SlicingOutOfBounds`, `SlicingMutation`, and ordinary reverse slicing. |
| `Microsoft.FSharp.Collections\Array3Module.fs` | Full slicing, `SlicingSingleFixed*`, `SlicingDoubleFixed*`, and reverse slicing. |
| `Microsoft.FSharp.Collections\Array4Module.fs` | Full slicing, `SlicingSingleFixed*`, `SlicingDoubleFixed*`, `SlicingTripleFixed*`, and reverse slicing. |
| `Microsoft.FSharp.Collections\StringModule.fs` | Bounded/unbounded, empty, out-of-bounds, and reverse string slicing. |
| `Microsoft.FSharp.Collections\ListType.fs` | Correct list slicing and out-of-bounds behavior. |

If missing, add compact controls for `[1..5][3..lo] = []` and the safe nearby array range `3..(lo + 10)`.
Also preserve this timing with a tiny 2D source: invalid fixed index plus ordinary empty retained range `1..0` returns empty.
The same invalid fixed index with a nonempty retained range `0..0` still raises `IndexOutOfRangeException`.
Null array/string sources must still raise the existing null exception, even for an empty requested slice.
Do not introduce eager fixed-index validation or an early return before reading source dimensions.

Name regression methods consistently, for example `GetterSlicingOverflow*`.
Rebuild the test consumer against unchanged product source and run that selection.
Before changing product source, use the target-selection procedure in step 4 to capture RED for both netstandard targets and the shipped-net target.
Record failures from correct expected-empty and shape assertions, not tests expecting today's exceptions.
Some source-syntax paths can pass because of compiler lowering or optimization.
Record them as passing controls and obtain RED through the relevant public intrinsic entry point or callable body.
Do not claim that a passing syntax example proves a failing intrinsic is covered.

### 3. Implement one getter-only normalizer

Edit only getter logic in `src\FSharp.Core\prim-types.fs`, currently around lines 6265-6708.
Leave the existing `ComputeSlice` definition unchanged for fixed setters.
Add one implementation-only inline helper nearby, for example `ComputeSliceRange`.
Match the existing helper's visibility pattern: an implementation binding absent from `prim-types.fsi`.
Compile it before assuming that it is accessible correctly through public inline optimization data.

Candidate structure, not prevalidated production code:

```fsharp
let inline ComputeSliceRange bound start finish length =
    let low =
        match start with
        | Some n when n >= bound -> n
        | _ -> bound

    let count =
        if length = 0 then
            0
        else
            let upper = bound + (length - 1)
            let high =
                match finish with
                | Some n when n < upper -> n
                | _ -> upper

            if high < low then 0 else high - low + 1

    low, count
```

The returned low must exactly match today's `ComputeSlice`, even for empty slices.
For an empty source dimension, do not derive an upper bound.
For a nonempty legal dimension, the inclusive upper bound `bound + (length - 1)` is representable.
When `high >= low`, `bound <= low <= high <= upper` bounds the count by the source length.
Compare before subtraction. Do not calculate the exclusive endpoint or use widened arithmetic as an unrelated rewrite.
Do not reject all negative indices. Negative absolute indices are valid for negative-based arrays.

Replace every retained-dimension count in these eleven bodies:

| Getter body | Counts to replace |
|---|---:|
| `GetArraySlice` | 1 |
| `GetArraySlice2D` | 2 |
| `GetArraySlice2DFixed` | 1 |
| `GetArraySlice3D` | 3 |
| `GetArraySlice3DFixedSingle` | 2 |
| `GetArraySlice3DFixedDouble` | 1 |
| `GetArraySlice4D` | 4 |
| `GetArraySlice4DFixedSingle` | 3 |
| `GetArraySlice4DFixedDouble` | 2 |
| `GetArraySlice4DFixedTriple` | 1 |
| `GetStringSlice` | 1 |

For example, `GetArraySlice` becomes a `(start, len)` helper call followed by `GetArraySub source start len`.
For each multidimensional getter, obtain `(startN, lenN)` for each retained axis.
Feed those counts to the existing allocation helpers and loops.
Keep `GetArraySub`, `GetArray2DSub`, `GetArray3DSub`, and `GetArray4DSub` at lines 799-922 unchanged.
Keep their copy behavior, all fixed-getter source element expressions, dimension reads, and match-based validation order.
Existing allocation clamps can remain. No negative count can reach a getter loop after normalization.
Do not replace a multidimensional empty result with an all-zero shape or a rank-one empty array.

Do not change setters, reverse-index translation, compiler lowering, signatures, diagnostics, public APIs, or baseline files.
In particular, do not repair missing source offsets inside the 3D/4D fixed getter match arms.
Avoid new generic abstractions, an extra shape framework, duplicate setup, and explanatory comment blocks.

### 4. Rebuild and verify actual consumers GREEN

Rebuild the Debug composite after the implementation change.
Recompile the test consumer before rerunning the exact RED cases.
Do not weaken expected shapes or convert expected-empty assertions into exception expectations.
Do not use `--no-build` against stale consumers.

Public inline arithmetic can be embedded in consumers.
A Core DLL replacement does not necessarily repair previously compiled code.
Record the referenced and loaded Core path, target framework, and file hash or module identity for each validation leg.
A plain `dotnet fsi` session can load SDK Core and is not proof of the local fix.

Run the regression selection against freshly compiled consumers of `netstandard2.0`, `netstandard2.1`, and the default shipped-net Core.
Use the same compact tests. Do not create a permanent target/platform matrix harness.
One local route is to rebuild the unit-test consumer with its existing Core-reference property overridden:

```powershell
$env:BUILDING_USING_DOTNET = 'true'
dotnet build tests\FSharp.Core.UnitTests\FSharp.Core.UnitTests.fsproj -c Debug -t:Rebuild -p:BuildProjectReferences=false -p:FSharpCoreShippedNetTargetFramework=netstandard2.0
dotnet exec artifacts\bin\FSharp.Core.UnitTests\Debug\net11.0\FSharp.Core.UnitTests.dll --filter-method "*GetterSlicingOverflow*"
```

Build all normal dependencies and Core targets through the composite before using `BuildProjectReferences=false`.
This command route is based on project inspection, not an executed planning-time build. Validate its reference resolution before trusting results.
Repeat the consumer rebuild with `netstandard2.1`, then with the actual shipped-net value.
This property selects the test project's `ProjectReference` target. Do not change its `.fsproj`.
Verify the compiler's resolved `/reference:` and loaded Core identity for each run.
Do not assume that an output-directory DLL copy changed the compilation input.
If the property route does not resolve the intended reference, use an ignored temporary consumer with an explicit local DLL reference.
Disable its implicit FSharp.Core package and recompile it separately for each target.
Keep this fallback outside the committed source and reuse the same regression inputs and shape assertions.
Capture the same target distinctions during RED as well as GREEN.

Exercise both ordinary F# slice syntax and emitted callable intrinsic bodies where needed.
An F# call to an inline intrinsic can inline too, so it is not automatically a callable-body test.
For body execution, use a narrowly scoped reflection invocation or a tiny temporary C# consumer.
The existing reflection pattern in `Array2Module.fs`, `RequiresDynamicCodeIsOnBasedApisOnly`, starts from `typeof<int option>.Assembly`.
Resolve only the required public getter wrappers and specialize them to `int` where needed.
Assert the returned shape. Do not build a name-discovery framework or treat `TargetInvocationException` as the expected result.
Verify the actual CLR type/method names rather than assuming F# source names map unchanged.
Make representative body coverage part of the compact regression suite, not only an unrecorded scratch experiment.

Run broader slicing controls after the focused GREEN selection:

```powershell
dotnet exec artifacts\bin\FSharp.Core.UnitTests\Debug\net11.0\FSharp.Core.UnitTests.dll --filter-method "*Slicing*" --filter-method "*slice*" --filter-method "*Fixed*" --filter-method "*OptimizedRanges*"
```

This includes ordinary setters and reverse slices as controls, not as implementation scope.
Ensure the case-sensitive filters include the lowercase-named empty-slice identity test and new control names.

Finally, build the Release composite and run the full Core suite with the default shipped Core reference:

```powershell
.\build.cmd -c Release -noVisualStudio
dotnet exec artifacts\bin\FSharp.Core.UnitTests\Release\net11.0\FSharp.Core.UnitTests.dll
```

Run the focused regression selection in Release against both netstandard Core targets as well, using freshly rebuilt consumers.
Do this after the full default-reference suite, or restore the default reference before the full suite.
`SurfaceArea.fs` uses different baselines for netstandard2.0 and the CoreCLR/default surface.
Do not run the default full-suite surface check against a substituted netstandard2.0 Core and then update its baseline.
Keep `TEST_UPDATE_BSL` unset. Existing Core surface checks must pass without baseline changes.
Record all failures explicitly and resolve regressions before marking this sprint done.

### 5. Format, review, document, and commit

Format only the changed F# files, not the repository:

```powershell
dotnet fantomas src\FSharp.Core\prim-types.fs tests\FSharp.Core.UnitTests\FSharp.Core\OperatorsModule1.fs
```

If Fantomas is missing, restore the repository tool manifest after that missing-tool failure.
Inspect the diff and retain only formatting associated with the changes. Do not keep unrelated whole-file formatting churn.
Rebuild and rerun affected selections after final edits.

Invoke the `reviewing-compiler-prs` skill and the `expert-reviewer` agent on the final local diff.
Focus review on Core stability, inline/binary compatibility, API surface, arithmetic bounds, retained shapes, and test completeness.
Supply the actual RED/GREEN evidence and the explicit exclusions.
Resolve actionable findings, remove duplicate setup and unnecessary helpers, and rerun affected validation.
The review is local. Do not post findings to GitHub.

Invoke `release-notes` after the fix is validated.
Read `gh variable get VNEXT --repo dotnet/fsharp`; its planning-time value was `11.0.100`.
Use the skill's insertion helper for the current `.FSharp.Core` file and its `Fixed` section.
Add one short entry such as:

```markdown
* Fix array and string slices with extreme reversed bounds to return correctly shaped empty results. ([Issue #20530](https://github.com/dotnet/fsharp/issues/20530))
```

There is no PR in this commit-only workflow. Do not fabricate a PR link or open a PR to obtain one.
Do not claim that replacing Core repairs old inline consumers. Explain recompilation in the commit's validation summary.

Remove temporary consumer projects and copied binaries after retaining their commands and results.
Keep evidence outside cleaned build outputs so it survives another execution window.
Inspect `git diff --check`, the complete product diff, and `git status`.
Stage only the intended Core, test, and release-note paths. Never use a broad add that captures runner artifacts.
Commit with a descriptive message that includes a concise RED/GREEN, Core-target, and review summary.
Include the trailers required by your execution session's instructions.
Do not amend another agent's commit. Do not push.

## Definition of Done

- Correct expected-empty tests were added and executed before changing `prim-types.fs`, with durable RED evidence.
- The original safe string symptom and both safe extreme 1D array ranges return empty without exception.
- Full 2D, 3D, and 4D tests rotate the reversed range through every retained axis and assert rank, all lengths, and all zero lower bounds.
- Tests reach all six fixed getter implementations without duplicating every public wrapper.
- Fixed `1..Int32.MinValue` cases return correctly shaped empty results in 2D, 3D, and 4D without entering element access.
- Positive-based, negative-based, valid negative absolute-index, legal `Int32.MaxValue` endpoint, and empty `Int32.MinValue`-based cases pass.
- Exactly one implementation-only getter normalizer covers all eleven getter bodies and twenty-one dimension counts.
- The normalizer preserves the old low exactly, handles zero length before upper-bound arithmetic, and compares endpoints before subtraction.
- The existing `ComputeSlice`, setters, fixed source-offset expressions, compiler code, public signatures, and baseline files are unchanged.
- The same regression assertions are GREEN after recompiling consumers, without weakened expected values or shapes.
- Fresh consumer execution is recorded for `netstandard2.0`, `netstandard2.1`, and the actual shipped-net Core target, with resolved and loaded assembly identities.
- Ordinary F# slicing and representative callable intrinsic bodies are both exercised, with their RED/GREEN outcomes distinguished.
- Null exceptions, fixed-index validation timing, list controls, clipping, omitted bounds, empty/copy identity, ordinary setters, and reverse slices retain their existing behavior.
- The repository composite Release build, focused sibling selection, full Core unit-test suite, and unchanged surface-area checks pass locally.
- Only changed F# files were formatted, and the final diff contains no unrelated formatting or generated-file changes.
- The requested expert review completed, actionable findings were resolved, and affected validation was rerun.
- One concise Core release note links the issue without a fabricated PR or a promise about already compiled inline consumers.
- Intended changes are committed with durable validation details and required trailers, with no uncommitted task edits or temporary consumer files remaining.
- Nothing was pushed, no PR was created, and no GitHub comment or review was posted.
