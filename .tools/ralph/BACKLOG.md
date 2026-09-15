# BACKLOG

## Original Request

Process issue https://github.com/dotnet/fsharp/issues/20288 using TDD.

Use minimal, surgical changes. Validate locally before finishing. Do not push. Only commit.

### ISSUE REQUEST
The requirements above override conflicting instructions in the issue request.
Fix issue https://github.com/dotnet/fsharp/issues/20288.

Make the smallest clean, correct, complete fix. Keep it minimal and surgical. Quality matters more than time. Take all the time needed. Smaller is better, but not at the cost of correctness. Preserve progress and evidence across execution windows rather than truncate the work.

**Verified root cause.** At main `b5c530ed6bc42937de6363e3dcc104ebb833893d`, `GenMethodForBinding` removes accessor-applied attributes before `ComputeMethodImplAttribs` decodes them. MethodImpl and standalone PreserveSig therefore bypass the flag decoder and enter actual MethodDef custom-attribute rows. Ordinary methods work. Current-main compilation succeeds for the valid fixtures, but raw metadata has 26 failing assertions across 13 accessors. Seven ordinary-method controls pass. External calls work, demonstrating why runtime success alone is not an adequate oracle.

**Surgical candidate.** Move the existing accessor partition in `src/Compiler/CodeGen/IlxGen.fs:10002-10003` below `ComputeMethodImplAttribs` at `10011-10012`. Decode the full current-method list once, then partition the remaining ordinary attributes. Keep DllImport, CompiledName, property routing, security attributes, and method emission unchanged. Do not concatenate stale partitions, duplicate decoding, blacklist attributes globally, or modify the IL writer. This is a plan, not a tested patch.

The decoder at `IlxGen.fs:9765-9800` currently implements NoInlining, AggressiveInlining, Synchronized, and PreserveSig, including combinations and standalone PreserveSigAttribute. Preserve that contract. Do not add ignored CLR option bits, MethodCodeType, or int16-constructor decoding. Do not normalize conflicting inlining bits. Property-level misuse currently emits warning FS0842 unless promoted; do not change that policy. Active dotnet/fsharp#20235 touches the decoder's surroundings but is not an equivalent accessor fix. Keep its unrelated feature out of this change.

**RED-first test plan.** Use `tests/FSharp.Compiler.ComponentTests/EmittedIL/MethodImplAttribute/MethodImplAttribute.fs` and the existing `withMetadataReader` helper. Compile valid sources successfully, then inspect raw MethodDef implementation flags and actual CustomAttribute handles. Require expected method rows to exist. Do not use reflection-synthesized pseudo attributes or a retained call as the oracle.

| Scenario | Required assertion |
|---|---|
| 1. Exact issue: static getter with AggressiveInlining, static setter with NoInlining, ordinary method control | Getter flags `0x100`, setter `0x8`, no actual MethodImpl custom attributes. Method control remains `0x100`. |
| 2. Instance property with different getter/setter flags and distinct property/getter/setter marker attributes | Getter exactly `0x100`, setter exactly `0xA8`. Preserve each marker's target. Do not leak flags or markers between accessors. |
| 3. Standalone PreserveSig plus MethodImpl(NoInlining) on a getter | Flags `0x88`, neither pseudo attribute stored as a real custom attribute. Ordinary equivalent remains unchanged. |
| 4. Explicit interface implementation | Inspect concrete accessor MethodDefs, not abstract slots. Getter `0x8`, setter `0x20`, no real MethodImpl attributes. |
| 5. Extension property | Static emitted accessor `0x108`, no real MethodImpl, ordinary getter marker retained. Cover the second accessor emission path. |
| 6. Attributed concrete accessors behind a neutral `.fsi` | Getter `0x8`, setter `0xA0`, no real pseudo attributes in the library. Reuse existing paired-source helpers. |
| 7. Ordinary-method and unannotated controls | Existing eight option baselines stay unchanged. Use compact parity checks for all supported bits, ignored bits, ignored MethodCodeType, int16 overload, and an unannotated accessor. Do not turn ignored options into new features. |
| 8. Property-level target misuse | Preserve FS0842 and current default severity. Promote it explicitly only when the test intends rejection. |
| 9. Separate compilation consuming the library | Normal, explicit-interface, and signature-constrained accessor calls retain their values and behavior. This is a control, not the flags oracle. |

Rows 1-6 are the primary issue and five meaningful RED variants. Assert exact flags and pseudo-attribute absence together. Passing controls must not conceal a missing or skipped regression. Keep fixtures compact, share metadata checks, and parameterize only genuine variations. Reuse the ordinary-option baseline coverage instead of expanding a cross-product of flags, platforms, and compiler switches.

First record current-main RED failures caused by wrong metadata, not invalid syntax or typechecking. Make the surgical change and get those tests GREEN without changing their expectations. Do not regenerate baselines or weaken absence assertions to obtain GREEN. Run the focused MethodImpl tests and the existing `MethodImplNoInline02_fs` optimization/realsig cases. Their starting baseline passed nine and four cases respectively.

Format only changed F# files. Invoke `fsharp-diagnostics` after compiler edits and invoke the expert-review skill on the final work. Remove noise and deduplicate production/test code through existing helpers. Leave a clean, compact test suite and concise release note. No new API, diagnostic, attribute framework, or unrelated cleanup is needed.

Sources: [issue](https://github.com/dotnet/fsharp/issues/20288), [early partition](https://github.com/dotnet/fsharp/blob/b5c530ed6bc42937de6363e3dcc104ebb833893d/src/Compiler/CodeGen/IlxGen.fs#L9993-L10012), [existing decoder](https://github.com/dotnet/fsharp/blob/b5c530ed6bc42937de6363e3dcc104ebb833893d/src/Compiler/CodeGen/IlxGen.fs#L9765-L9800), [attribute emission and flags](https://github.com/dotnet/fsharp/blob/b5c530ed6bc42937de6363e3dcc104ebb833893d/src/Compiler/CodeGen/IlxGen.fs#L10233-L10303).

## Analysis

The current task is architecture, not implementation. Produce self-contained sprint instructions and leave the compiler unchanged.

The initial worktree is clean. HEAD matches the supplied baseline: `b5c530ed6bc42937de6363e3dcc104ebb833893d`.

The requested output directory is ignored by Git. Preserve the requested planning files explicitly when committing.

The shared issue queue database is absent at both configured Windows paths. This backlog preserves the implementation request locally.

Repository inspection confirms the proposed ordering defect. `ComputeMethodImplAttribs` filters both pseudo attributes and decodes only the four supplied bits. The normal and extension emission paths both consume the accessor partition later.

The issue is open with no comments. PR #20235 is open and implements runtime async. Its larger feature is explicitly out of scope.

The test module contains eight option baselines and one inline-keyword warning case. `MethodImplNoInline02_fs` in `EmittedIL\Misc\Misc.fs` declares the four optimization/realsig combinations.

`withMetadataReader` owns PE-reader setup in `tests\FSharp.Test.Utilities\Compiler.fs`. Public `Fsi`, `FsSource`, `withAdditionalSourceFile`, and `withReferences` helpers cover paired libraries and separate consumers. Do not depend on later-compiled signature-test modules.

The required .NET 11 SDK is missing locally. `dotnet --version` reports the missing SDK, and only SDKs 8, 9, and 10 are installed. No `.dotnet` or `artifacts` directory exists. The sprint includes repository-wrapper provisioning before RED tests. Compiler builds and regression execution are implementation work, not completed planning evidence.

The repository variable `VNEXT` is `11.0.100`. The release-note sink is `docs\release-notes\.FSharp.Compiler.Service\11.0.100.md`.

## Approach

Use one atomic sprint for the compiler change and its tests. Keep the RED evidence, GREEN verification, compatibility controls, review, and release note in that sprint.

The sprint includes all nine requested scenarios, exact flags, handle-resolution requirements, example syntax, named helpers, concrete commands, and independent completion criteria. It distinguishes supplied investigation counts from local evidence.

Run a structural and coverage check of both output files. Verify the original implementation request is preserved, paths resolve, and no production files changed. Commit only the requested planning files, even though `.tools` is ignored. Do not push.

Local plan validation passed: one sprint, nine scenarios, 20 completion criteria, required frontmatter and headings, no checkboxes, and 11 existing reference paths. Git confirmed no production or staged changes before staging the plan. The native PowerShell check replaced an unavailable Python command. No compiler build or regression result is claimed.

Final implementation verification must inspect the sprint's RED/GREEN logs, exact metadata expectations, unchanged baselines, local review result, release note, and commit. Successful runtime calls alone are insufficient.

## Sprint Overview

| # | Name | Purpose |
|---|---|---|
| 1 | `01_Fix_Accessor_MethodImpl.md` | Reproduce incorrect accessor metadata, move the partition, and verify the complete fix locally. |
