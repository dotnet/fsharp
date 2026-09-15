# BACKLOG

## Original Request

Process issue https://github.com/dotnet/fsharp/issues/20211 using TDD.

Use minimal, surgical changes. Validate locally before finishing. Do not push. Only commit.

### ISSUE REQUEST
The requirements above override conflicting instructions in the issue request.
Fix issue https://github.com/dotnet/fsharp/issues/20211.

Make the smallest clean, correct, complete fix. Keep it minimal and surgical. Quality matters more than time. Take all the time needed. Smaller is better, but not at the cost of correctness. Preserve progress and evidence across execution windows rather than truncate the work.

**Verified root cause.** At main `b5c530ed6bc42937de6363e3dcc104ebb833893d`, recursive declaration checking evaluates `not null` constraints while union case tables are provisional and empty. `IsUnionTypeWithNullAsTrueValue` accepts every empty table before checking `UseNullAsTrueValue`. An ordinary unfinished union therefore receives the false FS3261 representation warning. This affects ordinary unions as well as structs. The payload, `Value` member, and imported Dictionary metadata are not the cause. Twenty-five isolated current-main compilations confirm the timing and controls. Six intended regression scenarios each fail with the false FS3261.

**Surgical candidate.** In `src/Compiler/TypedTree/TypedTreeOps.Attributes.fs:1319-1327`, require `TyconHasUseNullAsTrueValueAttribute` before the empty-table alternative. Keep the provisional alternative for genuinely attributed unions. Keep `CanHaveUseNullAsTrueValueAttribute`, the `unit` special case, and constraint behavior unchanged. Do not suppress warnings, special-case Dictionary or structs, inspect payload nullness, or defer all constraints. The candidate is source-grounded but has not been applied or proven GREEN.

The declaration-phase evidence is in `src/Compiler/Checking/CheckDeclarations.fs:2950-3036,3301-3408,4413-4530`. Attributes are published before the failing checked-abbreviation pass. The caller is `ConstraintSolver.fs:2940-2965`, through `TypedTreeOps.Transforms.fs:496-501`. Reconfirm this timing for any new counterexample before expanding the change.

**RED-first test plan.** Use `tests/FSharp.Compiler.ComponentTests/Language/Nullness/NullableRegressionTests.fs` and its existing helpers. Use one compact parameterized test for the valid regression sources. Each must execute the compiler and assert success without diagnostics. Promoting warnings is useful; ignoring them is not.

| Scenario | Required assertion |
|---|---|
| 1. Exact issue: `module rec M`, struct `Hole = Hole of string`, original `Value` member, then `Substitution = Dictionary&lt;Hole,obj&gt;` | No diagnostics. Current main reports false FS3261. |
| 2. Exact comment: non-recursive module with `and Substitution` in the same type group | No diagnostics. Current main reports false FS3261. |
| 3. Ordinary reference union in the recursive group, without the member | No diagnostics. Prevent a struct-only workaround. |
| 4. Generic struct union with a nullable string payload | No diagnostics. Nullable contents do not make the enclosing struct nullable or require a new payload constraint. |
| 5. A record field containing `Dictionary&lt;Hole,obj&gt;` before the struct union declaration | No diagnostics. Cover the early representation-check path beyond aliases. |
| 6. An explicit local F# `not null` constraint with `--checknulls-` | No false representation warning. Do not fix only imported Dictionary constraints. |
| 7. Non-recursive original source and a completed union from a preceding file or referenced assembly | Remain valid. Reuse existing completed-type controls rather than duplicate setup. |
| 8. Same-group `UseNullAsTrueValue` union, with its constrained alias before the union | Keep the genuine representation FS3261. This protects the provisional attributed-union case. |
| 9. An option used as a constrained key | Keep the genuine representation FS3261. |
| 10. A nullable string key | Keep the genuine supports-null FS3261. |

Rows 1-6 are real RED cases, already observed on current main. Rows 7-10 are controls and need not fail before the fix. Check full diagnostic lists for invalid cases, using the existing helper's severity convention consistently. Reuse an equivalent existing negative test when it exercises the same path.

Get GREEN without weakening those tests. Then run the neighboring nullness constraints and existing null-as-true-value runtime, signature, invalid-attribute, and metadata tests. The starting baseline passed five selected tests under `*UseNullAsTrueValue*` and `*Nullable attr for Option clones*`. Keep the shared predicate's completed-union behavior unchanged.

Format only changed F# files. Invoke `fsharp-diagnostics` after compiler edits and invoke the expert-review skill on the final work. Remove noisy comments, duplicate setup, and redundant helpers. Reuse existing compiler and test mechanisms. Leave a clean, compact test suite and the required concise release note. No public API, new diagnostic, new traversal, broad refactor, or unrelated baseline update is expected.

Sources: [issue and follow-up](https://github.com/dotnet/fsharp/issues/20211), [faulty predicate](https://github.com/dotnet/fsharp/blob/b5c530ed6bc42937de6363e3dcc104ebb833893d/src/Compiler/TypedTree/TypedTreeOps.Attributes.fs#L1319-L1327), [declaration ordering](https://github.com/dotnet/fsharp/blob/b5c530ed6bc42937de6363e3dcc104ebb833893d/src/Compiler/Checking/CheckDeclarations.fs#L4413-L4530), [constraint caller](https://github.com/dotnet/fsharp/blob/b5c530ed6bc42937de6363e3dcc104ebb833893d/src/Compiler/Checking/ConstraintSolver.fs#L2940-L2965).

## Analysis

This is an architecture handoff. The requested outputs are this backlog and self-contained sprint files, not an implemented compiler change.

One sprint is sufficient. The predicate change, regression tests, compatibility controls, release note, review, and local commit form one independently verifiable fix. Separate RED-only or validation-only sprints would leave incomplete work.

### Locally confirmed facts

The worktree is `Q:\fsharp-worktrees\issue-876`, despite the older issue number in its directory name. Its branch is `fix/issue-20211`. Initial HEAD is `b5c530ed6bc42937de6363e3dcc104ebb833893d`. Initial tracked status was clean.

The issue and its one follow-up comment match the supplied examples. The predicate at lines 1319-1327 accepts an empty union-case table without checking the representation attribute.

`TcTyconDefnCore_Phase1B_EstablishBasicKind` creates `Construct.MakeUnionRepr []`. The first supertype pass publishes `entity_attribs`. The second abbreviation pass checks constraints before Phase1G establishes representations. `SolveTypeUseNotSupportsNull` calls `TypeNullIsTrueValue`, which calls the faulty predicate.

The completed-union truth table is unchanged when the attribute check moves outside the empty-or-valid-cases expression. Only an unattributed provisional union changes from true to false. An attributed provisional union must remain true.

`NullableRegressionTests.fs` already provides `withVersionAndCheckNulls`, warning promotion, and test compilation patterns. `Compiler.fs` provides `typecheck`, `shouldSucceed`, and `withDiagnostics`. No new harness is needed.

The existing five representation controls are the runtime DU property test, signature test, invalid-attribute theory, C# metadata consumer, and Option-clone IL baseline. Their exact names and paths are in the sprint.

GitHub repository variable `VNEXT` was `11.0.100` during planning. The target release-note file exists at `docs\release-notes\.FSharp.Compiler.Service\11.0.100.md`.

### Evidence limits and prerequisites

The 25 isolated compilations and six RED observations are user-supplied evidence. This planning pass reconfirmed source ordering, not those executions. No compiler or test source was edited.

The initial `dotnet --version` and project-property query could not find the pinned SDK. `global.json` requires `11.0.100-rc.1.26420.103`. Installed SDKs are `8.0.425`, `9.0.318`, `10.0.101`, and `10.0.112`. There was no local `.dotnet` SDK or component-test build directory. The implementer must use `eng\common\dotnet.ps1` to obtain the pinned SDK before RED. Do not change target frameworks or `global.json`.

The normal shared issue-queue database and its alternate path were absent on this machine. This backlog preserves the request instead. No queue database or remote task was created.

The requested `.tools\ralph` paths are ignored by `.gitignore`. Only the explicitly requested planning files should be force-added for the planning commit. Leave `ralph.log` and other runner files untouched.

Local plan validation passed: one sprint, all ten scenarios, 17 dash-only completion criteria, balanced code fences, and 12 verified repository paths. The C# import test selector was checked against its declared module name.

## Approach

Read the supplied sprint template before writing the sprint. Keep all implementation context in `01_Fix_Provisional_Union_Nullness.md`, because implementers and sprint verifiers cannot see this backlog.

The sprint requires six independently reported RED cases before the compiler edit. It retains passing completed-union controls and exact negative diagnostics. It then moves the attribute guard, runs diagnostics and GREEN tests, checks representation compatibility, formats only changed F# files, adds one release note, obtains expert review, and commits without pushing.

Store implementation logs and checkpoint evidence in the implementing agent's persistent session artifact directory. Record commands, exit codes, test counts, diagnostic lists, and compiler revisions. Do not add progress documents or baseline churn to the product diff.

### Final verification checklist

- Exactly one sprint exists for this task, with the required frontmatter, Context, Description, and dash-only Definition of Done.
- The sprint includes all ten scenarios, both exact issue sources, the shared predicate invariant, and source-order evidence.
- All six positive regressions fail with FS3261 before the fix and pass without diagnostics after the fix.
- Non-recursive and completed-union controls remain valid. Attributed provisional unions, options, and nullable strings retain their required diagnostics.
- The same five representation controls pass, and neighboring nullness tests pass.
- Compiler diagnostics, changed-file formatting, release notes, expert review, and commit-only delivery are required in the sprint.
- Planning validation checks structure, paths, coverage, and git whitespace locally. Product RED/GREEN evidence remains the implementation sprint's responsibility.

## Sprint Overview

| # | Name | Purpose |
|---|---|---|
| 01 | Fix Provisional Union Nullness | Prove six RED cases, make the attribute-guard fix, verify all controls locally, review, document, and commit without pushing. |
