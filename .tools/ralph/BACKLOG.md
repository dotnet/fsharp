# BACKLOG

## Original Request

Process issue https://github.com/dotnet/fsharp/issues/20410 using TDD.

Use minimal, surgical changes. Validate locally before finishing. Do not push. Only commit.

### ISSUE REQUEST
The requirements above override conflicting instructions in the issue request.
Fix issue https://github.com/dotnet/fsharp/issues/20410.

Make the smallest clean, correct, complete fix. Keep it minimal and surgical. Quality matters more than time. Take all the time needed. Smaller is better, but not at the cost of correctness. Preserve progress and evidence across execution windows rather than truncate the work.

**Verified root cause.** At main `b5c530ed6bc42937de6363e3dcc104ebb833893d`, `checkRecordFields` validates fields by name in both directions, then calls diagnostic-emitting `checkField` again by position. The first reordered pair produces misleading FS0193 before the correct order diagnostic. The actual order diagnostic is **FS0312**, not the issue's FS0313. FS0313 means a required field is missing. Do not change diagnostic numbers. Eighteen current-main matrix compilations give seven expected RED assertions and eleven passing controls. A separate exact original `ResolvedConfig` compilation confirms only FS0193 at the first displaced field and FS0312 on the type.

**Surgical candidate.** Change only the final record positional predicate in `src/Compiler/Checking/SignatureConformance.fs:640-642`: compare `LogicalName` before calling the existing `checkField`. If names differ, return false without that call. Preserve the existing FS0312 at the implementation type and the false conformance result. Keep the two preceding name-map passes at `631-636` unchanged.

This guard avoids the misleading diagnostic and the wrong cross-field documentation/range mutation. Keep matching-name calls unchanged. Replacing the whole predicate with pure name equality would also remove existing repeated nullness warnings on correctly ordered records, which is outside this issue. The candidate is not yet applied or proven GREEN.

Do not sort fields, accept reordered records, suppress shared FS0193, alter diagnostic resources, or broaden the guard to unions, exceptions, or classes. Same-name type, mutability, accessibility, and attribute checks must still run. Constructor parameter order depends on record declaration order.

**RED-first test plan.** Use `tests/FSharp.Compiler.ComponentTests/Conformance/Signatures/Signatures.fs` and the paired-source compile pipeline. A plain `typecheck` call does not consume `AdditionalSources` at this revision. Use the existing `Fsi` plus implementation-source helper and `compile`, or a demonstrated project-typecheck path. Assert the complete diagnostic list, including code, severity, message, and range. GREEN means correct diagnostics while the invalid permutation still fails compilation.

| Scenario | Required assertion |
|---|---|
| 1. Exact reported `ResolvedConfig` field permutation, with compact local definitions for its dependent types | Exactly FS0312 on `ResolvedConfig`, no positional FS0193. Also use the reduced two-field swap to isolate the cause. |
| 2. A shared correctly ordered prefix followed by a three-field cycle | Exactly FS0312 on the type, no positional FS0193. |
| 3. Swapped fields with the same `int` type | Exactly FS0312. Equal field types do not make declaration order legal. |
| 4. Generic struct record with `'T` and `'T list` fields swapped | Exactly FS0312. Preserve generic remapping and the representation constraint. |
| 5. Permutation plus conflicting attribute arguments on the same named field | Keep FS1200 from attribute reconciliation and FS0312. Remove only positional FS0193. |
| 6. Identical names, declarations, and order | Successful compilation without diagnostics. |
| 7. Real same-name mismatch | Parameterize changed type, changed mutability, and less-accessible representation. Keep genuine FS0193 identifying the same field, without a spurious order error. |
| 8. Different name sets | Missing, extra, and renamed fields keep FS0313, FS0311, and FS0313 respectively. Equal counts alone do not establish a permutation. |
| 9. Warning-only nullness differences | Keep existing same-name warnings. The observed aligned and prefix-before-permutation cases each have three FS3261 warnings. The prefix case adds FS0312 but loses positional FS0193. Do not make warning cleanup part of this change. |
| 10. Non-record and recovery boundaries | Reuse union, exception, object-field, and malformed-field controls. Union and exception diagnostics stay unchanged. Reordered class fields can still compile. Do not refactor recovered duplicate names or list lengths. |

Rows 1-5 provide the primary bug and four meaningful RED variants. Current-main reduced forms already fail the intended expectations. Preserve the exact issue case as a compact fixture too. Rows 6-10 are controls, not additional before-fix failure claims. Use one data-driven regression test and separately named controls. Share source-pair construction; do not copy many near-identical files or test bodies.

First prove RED from the unwanted diagnostic. Then make the local guard and obtain GREEN without weakening tests, changing order-error expectations, suppressing warnings, or refreshing unrelated baselines. Existing field-extended-data, signature-nullness, and attribute-matching sibling selections passed seven rows on the starting main build. Run these and the nearby signature-conformance tests after implementation.

Format only changed F# files. Invoke `fsharp-diagnostics` after compiler edits and invoke the expert-review skill on the final work. Remove noise and duplicate setup using existing helpers. Leave a clean, compact suite and concise release note. No API change, new diagnostic, extra name set, general suppression flag, or broad conformance refactor is needed.

Sources: [issue](https://github.com/dotnet/fsharp/issues/20410), [record-specific checks](https://github.com/dotnet/fsharp/blob/b5c530ed6bc42937de6363e3dcc104ebb833893d/src/Compiler/Checking/SignatureConformance.fs#L624-L655), [field checks and side effects](https://github.com/dotnet/fsharp/blob/b5c530ed6bc42937de6363e3dcc104ebb833893d/src/Compiler/Checking/SignatureConformance.fs#L559-L623), [diagnostic identities](https://github.com/dotnet/fsharp/blob/b5c530ed6bc42937de6363e3dcc104ebb833893d/src/Compiler/FSComp.txt#L149-L151).

## Analysis

This delivery is architecture only. The implementer receives one sprint with the fix, tests, validation, review, release note, and commit requirements.
Do not implement the compiler change while preparing these files.

### Verified during planning

- Repository: `Q:\fsharp-worktrees\issue-878`, branch `fix/issue-20410`, initially clean.
- HEAD: `b5c530ed6bc42937de6363e3dcc104ebb833893d`. The directory name does not identify the target issue.
- Read `Q:\groundhog-while-not-works\templates\SPRINT_TEMPLATE.md` before creating the sprint.
- Retrieved issue #20410 through `gh issue view`. Its FS0313 label is incorrect.
- Read `checkField`, `checkRecordFields`, `checkRecordFieldsForExn`, `checkClassFields`, and `checkAttribs`.
- The two record name-map passes precede the positional call. The positional call can overwrite another field's documentation and range.
- `FSComp.txt` assigns 311 to an extra field, 312 to field order, and 313 to a missing required field.
- `Signatures.fs` already uses `Fsi |> withAdditionalSourceFile (FsSource ...) |> compile`.
- `Compiler.fs` confirms that plain `typecheck` reads only the primary source. `compile` consumes both sources.
- `Compiler.fs` also shows that `withDiagnostics` deduplicates by range and message. It cannot verify repeated warning counts alone.
- Raw `CompilationResult.Output.Diagnostics` retains duplicates. New tests need raw, complete diagnostic assertions, especially for FS3261.
- `checkAttribs` emits FS1200 as a warning unless options promote it. Its fixup replaces implementation attributes with signature attributes.
- The harness treats warnings as failure by default. That wrapper result alone does not prove a warning-only program has a compiler error.
- Found existing `FieldNotContainedDiagnosticExtendedData 01`, `Signature conformance`, `Micro compilation`, and `AttributeMatching01` sibling tests.
- GitHub repository variable `VNEXT` is `11.0.100`. The corresponding compiler-service release-note file exists.
- `dotnet --version` could not resolve the pinned SDK, `11.0.100-rc.1.26420.103`. No compiler build or runtime matrix ran during planning.
- `eng\common\dotnet.ps1` can install and invoke the repository SDK. SDK setup belongs to execution, not this documentation-only delivery.
- `.tools` is ignored. Commit only the two requested plan files with explicit `git add -f` paths. Do not change `.gitignore`.
- Native PowerShell validation passed: one sprint, 19 completion criteria, 17 existing source references, required scenario coverage, and balanced code fences.
- That validation also confirmed ASCII text, no trailing whitespace, and no tracked source or staged changes before staging the planning files.

The eighteen matrix compilations, seven RED assertions, eleven controls, and separate original fixture are evidence supplied by the request.
No previous-session evidence was found in the bounded history lookup.
Do not present those results as new local executions or infer an exact new test count from them.

### Main risks

Pure name equality would remove matching-name checks and repeated nullness warnings.
A shared `checkField` change would affect unions, exceptions, classes, and real field mismatches.
A test using plain `typecheck`, error-code presence, or deduplicated warnings could produce false confidence.
A test run using stale compiler binaries or discovering no selected tests cannot establish RED or GREEN.
Formatting an entire large compiler file can create unrelated changes even when the functional fix is tiny.

## Approach

Use one complete vertical sprint. Splitting tests, the guard, and controls would make early sprints intentionally incomplete.
The sprint embeds the original record permutation, minimal fixtures, assertion requirements, source helpers, commands, and restrictions.
Its RED phase captures exact diagnostics before editing production code.
Its GREEN phase changes only the final record predicate and reruns the unchanged tests plus siblings.
The final implementation commit contains the compiler change, compact tests, and one release note.
Keep execution logs under `.tools\ralph\evidence\issue-20410`, outside the implementation commit.
Never push, open a PR, post a review, or fabricate a PR URL.

### Final verification checklist

- The sprint starts with two `---` lines and contains all four required headings.
- Every Definition of Done item starts with `- `, without a checkbox.
- Every requested scenario is covered within the same sprint as its implementation.
- The sprint stands alone without this backlog, another sprint, or historical logs.
- The exact request above is preserved, including its distinction between FS0312 and FS0313.
- Local source references, project paths, and release-note path exist.
- The plan explicitly preserves duplicate warnings and complete diagnostic tuples.
- Planning validation checks file structure, paths, coverage, and `git diff --check`. It does not claim compiler GREEN.
- The planning commit contains only `BACKLOG.md` and `01_Record_Field_Order_Diagnostics.md`.
- The implementation verifier later requires actual RED/GREEN logs, passing sibling selections, review resolution, and a local implementation commit.

## Sprint Overview

| # | Name | Purpose |
|---|---|---|
| 01 | Record Field Order Diagnostics | Prove RED, guard the record-only positional check, prove GREEN with all controls, review, document, and commit locally. |
