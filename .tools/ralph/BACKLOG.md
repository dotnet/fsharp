# BACKLOG

## Original Request

Process issue https://github.com/dotnet/fsharp/issues/20530 using TDD.

Use minimal, surgical changes. Validate locally before finishing. Do not push. Only commit.

### ISSUE REQUEST
The requirements above override conflicting instructions in the issue request.
Fix issue https://github.com/dotnet/fsharp/issues/20530.

Make the smallest clean, correct, complete fix. Keep it minimal and surgical. Quality matters more than time. Take all the time needed. Smaller is better, but not at the cost of correctness. Preserve progress and evidence across execution windows rather than truncate the work.

**Verified root cause.** At main `b5c530ed6bc42937de6363e3dcc104ebb833893d`, getter slices compute inclusive lengths with unchecked arithmetic. Reversed bounds can wrap to a positive count. Fixed-index getters can also iterate incorrectly when `len = Int32.MinValue` makes `len - 1` wrap, despite an empty allocation. Legal based-array endpoints expose a directly coupled overflow in `ComputeSlice`'s exclusive upper-bound arithmetic. Current-main source execution and both Core targets reproduce the defect. Each Core target has 17 safe failing expected-empty assertions. Ten source-syntax controls pass.

**Surgical candidate.** Keep changes in `src/FSharp.Core/prim-types.fs`. Add one implementation-only getter normalizer near `ComputeSlice` at `6265-6275`, returning `(start, count)`. Handle a zero-length dimension before deriving an upper bound. For a nonempty legal dimension, use `bound + (length - 1)`, then compare the clipped endpoints before subtraction. Use it across the eleven getter implementations, covering twenty-one dimension counts between `6277` and `6708`. This includes all full-rank and fixed-index getter families and string slicing.

The proposed helper is not yet compiled or GREEN. Its arithmetic is supported by an interval-intersection proof and 14,504 allocation-free boundary-model cases. For ordered clipped endpoints, the count cannot exceed the source dimension length. Verify the actual inline implementation and emitted consumer behavior, not just the model.

Reuse existing allocation and copy helpers at `prim-types.fs:799-922`. The helper's returned start must equal today's `ComputeSlice` low exactly. Preserve existing element-access expressions while replacing length calculations. Keep `ComputeSlice` for fixed setters because changing it would alter setter semantics. Do not repair unrelated fixed-getter source offsets, setter arithmetic, reverse-index translation, compiler syntax, or public APIs. These adjacent issues were observed and are explicitly outside this fix. Preserve inclusive bounds, retained rank and shape, zero-based results, normal null exceptions, and current fixed-index validation timing.

**RED-first test plan.** Use the existing Core unit-test project, preferably local cases near `tests/FSharp.Core.UnitTests/FSharp.Core/OperatorsModule1.fs:43-95` and existing slicing tests. Use compact typed thunks, data rows, or intrinsic entry points instead of a new reflection framework. Assert correct empty results, not exceptions from the broken implementation.

**Allocation safety:** do not run the original array `[3..Int32.MinValue]` as an OOM experiment. `"hello"[3..Int32.MinValue]` is safe and preserves the original symptom. Array bounds `Int32.MaxValue..Int32.MinValue` produce a broken count of only two, giving deterministic safe RED. Keep all actual arrays tiny.

| Scenario | Required assertion |
|---|---|
| 1. Original string `3..MinValue`, plus `MaxValue..MinValue` | Empty string without exception. |
| 2. 1D arrays with `MaxValue..MinValue` and `MaxValue..(MinValue+1)` | Empty arrays. Broken counts are two and three, not huge allocations. |
| 3. Full 2D, 3D, and 4D getters | Rotate the extreme reversed range through retained axes. Assert rank, every dimension length, and zero result lower bounds. Empty one axis, not all axes. |
| 4. Fixed-index 2D-4D getters | Cover the six underlying fixed getter implementations. Assert reduced rank and the lengths of other retained dimensions. Some current failures return wrong nonempty shapes rather than throw. |
| 5. Fixed-loop boundary `1..MinValue` | Empty result with exact shape in 2D, 3D, and 4D. Current allocation can be empty while the loop still enters. |
| 6. Positive and negative source lower bounds | Extreme reversed retained ranges yield correctly shaped empty slices. Separately preserve valid negative absolute indices. |
| 7. Legal upper-bound endpoint | A one-element dimension based at MaxValue, sliced through MinValue, must produce an empty retained dimension. Include a tiny dimension ending at MaxValue with a finish inside it. Do not compute an overflowing exclusive bound. |
| 8. Empty dimension based at MinValue, requested start MaxValue | Correct empty retained dimension, no element access, and unchanged lengths of other axes. Do not derive its upper bound before noticing zero length. |
| 9. Existing negative controls | Reuse list, nearby nonoverflow, omitted-bound, ordinary clipping, empty/copy identity, null, invalid fixed-index timing, normal setter, and normal reverse-slice tests. Keep these separate from RED claims. |

Rows 1-6 provide the primary and five meaningful variants. Rows 7-8 directly protect the arithmetic proof at legal source boundaries. Row 9 uses existing coverage wherever possible. Do not create a Cartesian product of all types, axes, flags, or platforms. Test the six fixed implementations without duplicating every wrapper fixture.

First record RED with correct assertions against current main. Apply the smallest complete getter change and get the same cases GREEN without weakening shape checks or changing expected exceptions to match the bug. Recompile consumers against the new Core: existing consumers can retain old public-inline arithmetic. Exercise both ordinary F# slicing and callable intrinsic bodies where needed. Do not promise a Core DLL replacement alone repairs previously compiled consumers.

Run the focused sibling slicing selection and Core suite. The starting baseline passed 33 tests selected by `*SlicingOutOfBounds` and `*Fixed*`. Use the repository's composite Core build for the implementation. Format only changed F# files. Invoke the expert-review skill on the final work. Remove noise, duplicate setup, and unnecessary helpers. Leave a clean, compact suite and concise release note. Avoid setter changes, public surface changes, baseline churn, and unrelated cleanup.

Sources: [issue](https://github.com/dotnet/fsharp/issues/20530), [current slicing intrinsics](https://github.com/dotnet/fsharp/blob/b5c530ed6bc42937de6363e3dcc104ebb833893d/src/FSharp.Core/prim-types.fs#L6265-L6708), [FS-1077 tolerant slicing](https://github.com/fsharp/fslang-design/blob/7e3f0db7dcf1daa9486c7c87d3f5a398d460f56b/FSharp-5.0/FS-1077-tolerant-slicing.md), [related design work](https://github.com/fsharp/fslang-design/pull/849). This fix does not depend on that draft RFC.

## Analysis

### Planning scope and verified repository facts

This is an architecture handoff, not the implementation. The deliverables are this backlog and one self-contained sprint.
The sprint includes tests, the complete getter fix, consumer validation, review, release notes, and a local commit.
Splitting RED tests into a separate sprint would leave an intentionally failing unit of work.
Splitting getter families would leave one shared arithmetic defect only partly fixed.

The worktree is `Q:\fsharp-worktrees\issue-875`, on branch `fix/issue-20530`.
Its initial HEAD is exactly `b5c530ed6bc42937de6363e3dcc104ebb833893d`.
The tracked worktree was clean before planning.
The existing `.tools\ralph\ralph.log` is runner-owned and must remain untouched.
`.gitignore` ignores `.tools`, so commit the two requested planning files with explicit paths and `git add -f`.
Do not force-add the directory, logs, or later evidence.

The issue is open and has no comments at planning time.
The pinned FS-1077 text defines inclusive, clipped getter slices, with empty results for disjoint bounds.
No compiler feature gate or dependency on fsharp/fslang-design#849 is required.

Direct inspection confirms the three coupled arithmetic problems:

1. `finish - start + 1` can wrap before allocation helpers see the count.
2. Fixed getters clamp allocation dimensions but loop using the original count.
3. `ComputeSlice` uses `bound + length` for its comparison, which overflows at legal inclusive endpoints.

The existing `ComputeSlice` low is `max(bound, requestedStart)`, with omitted start mapped to `bound`.
Keep that exact low, including empty intervals and valid negative absolute indices.
The new getter-only helper must not change fixed setters, which still call `ComputeSlice`.
Full-rank allocation/copy helpers already exist at `prim-types.fs:799-922`.

The implementation inventory contains eleven bodies and twenty-one dimension calculations:

| Getter body | Dimension counts |
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

The six fixed bodies are implementation-only. Their numbered wrappers are public inline functions in `prim-types.fsi`.
Use those wrappers to reach the six bodies without exposing new APIs.
Some fixed bodies omit retained source offsets. Preserve those expressions exactly, as the request explicitly excludes those defects.

### Validation facts and limitations

The prior 17 failing assertions per Core target, ten passing syntax controls, 33 sibling passes, and 14,504 model cases are user-supplied evidence.
They were not rerun during architecture work. Do not label them as this sprint's RED or GREEN.
New tests must independently record failures against unchanged product source.
There is no required new-test count of 17. Cover every requested behavior without duplicating wrappers.

The current Core project declares **three** non-Proto targets: `netstandard2.0`, `netstandard2.1`, and the shipped-net target.
`eng\TargetFrameworks.props` currently pins the shipped-net target to `net10.0` and the product/test runtime to `net11.0`.
The unit-test project normally references the shipped-net Core on CoreCLR.
It also builds `netstandard2.1` for a separate surface-area test.
Therefore, a default CoreCLR run alone does not prove both netstandard implementations.
The sprint requires focused consumer execution against both netstandard assemblies and the default shipped-net assembly.
This is a Core-target check, not a Cartesian platform matrix.

`global.json` selects SDK `11.0.100-rc.1.26420.103` and Microsoft.Testing.Platform.
The planning-time `dotnet msbuild ... -getProperty:...` probe could not start because the pinned SDK is unavailable.
There is no worktree `.dotnet\dotnet.exe` or built Core output.
Use the repository's Windows SDK acquisition wrapper before implementation validation.
No product build or test was attempted during planning.
The existing `build.cmd` invokes `eng\Build.ps1 -restore -build`.
Its `-noVisualStudio` route uses the composite `FSharp.slnx`, including Core and the Core unit-test project.
Direct test-assembly execution avoids ambiguity between MTP and older VSTest filter syntax.

Local planning validation passed: required headings/frontmatter, 19 plain Definition of Done criteria, Markdown table widths, and ten principal repository paths.
The getter inventory was checked against the actual source: eleven bodies with twenty-one `ComputeSlice` calls.
The sprint was read independently for scenario coverage and does not require this backlog.
Implementation commands remain future work. Their inclusion is not a claim that the compiler, candidate helper, or tests have run.

The GitHub `VNEXT` variable is `11.0.100`.
The release-note destination currently exists at `docs\release-notes\.FSharp.Core\11.0.100.md`.
The no-push/no-PR instruction means the local note must use the real issue link, not a fabricated PR number.

The shared issue-queue database was not available at either configured user path.
No remote triage job or daily-monitor dispatch was requested or created.
This backlog preserves the request locally instead.

## Approach

Use one end-to-end sprint, with no prerequisite sprint:

1. Bootstrap the missing SDK through repository tooling, build unchanged Core, and record sibling controls.
2. Add compact, separately discoverable expected-empty regression cases in `OperatorsModule1.fs`.
3. Record safe RED before touching `prim-types.fs`, including wrong-shape returns and fixed-loop failures.
4. Add one implementation-only inline `(start, count)` normalizer.
5. Wire all eleven getters and all twenty-one retained-dimension counts without changing source element access.
6. Rebuild the composite and recompile consumers. Run identical tests GREEN against each relevant Core target.
7. Run focused sibling slicing tests, the full Core suite, and unchanged surface-area checks.
8. Format only changed F# files, obtain the requested expert review, resolve findings, and rerun affected validation.
9. Add one concise release note and commit only the intended implementation files. Never push or create a PR.

For a nonempty legal dimension, `upper = bound + (length - 1)` is representable.
If the clipped high is below the unchanged low, return count zero before subtraction.
Otherwise, `bound <= low <= high <= upper`, so `1 <= high - low + 1 <= length`.
For an empty source dimension, return count zero before calculating an upper bound.
This proof supports the candidate. It does not replace compilation or consumer execution.

Preserve raw RED/GREEN logs, exact commands, Core paths/hashes, and review outcomes in ignored issue-specific evidence.
Include a short verification summary in the implementation commit message so the result remains durable beyond the runner session.
Do not commit generated logs, temporary projects, or copied Core assemblies.

### Final verification checklist

- The sprint file has the required frontmatter, sections, and plain dash-list Definition of Done.
- An implementer can complete the work from the sprint alone, without this backlog or another sprint.
- All nine scenario rows, six fixed bodies, eleven getter bodies, and twenty-one dimension counts are covered.
- Required RED assertions remain unchanged in GREEN, with rank, all lengths, and all lower bounds checked.
- Both netstandard Core targets and the default shipped-net target have freshly compiled consumer evidence.
- Source syntax and callable intrinsic bodies are distinguished in the evidence.
- No setter, element-offset, compiler, public-signature, baseline, or unrelated formatting change is accepted.
- Composite build, focused controls, full Core suite, formatting, and expert review are recorded.
- The release note exists and does not promise repair of previously compiled inline consumers.
- Only intended files are committed. Nothing is pushed, and no GitHub write operation occurs.

## Sprint Overview

| # | Name | Purpose |
|---|---|---|
| 01 | Fix Getter Slice Overflow | Complete RED-to-GREEN fix for every getter, including shape/boundary regressions, consumer validation, review, release note, and local commit. |
