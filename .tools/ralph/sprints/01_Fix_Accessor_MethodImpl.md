---
---
# Sprint: Fix MethodImpl metadata on property accessors

## Context - WHY this sprint exists

Implement https://github.com/dotnet/fsharp/issues/20288 in `Q:\fsharp-worktrees\issue-877` using TDD. This file contains the complete work unit. No other sprint or backlog is required.

Make the smallest clean, correct, complete fix. Validate locally, then commit. Do not push, open a PR, or post GitHub comments.

At baseline `b5c530ed6bc42937de6363e3dcc104ebb833893d`, property accessor attributes bypass the existing implementation-flag decoder. `MethodImplAttribute` and standalone `PreserveSigAttribute` can enter real CustomAttribute rows instead. Ordinary methods work.

In `src\Compiler\CodeGen\IlxGen.fs`, `GenMethodForBinding` partitions `attrsAppliedToGetterOrSetter` before calling `ComputeMethodImplAttribs`. The decoder therefore cannot see those attributes.

The user reports 26 wrong-metadata assertions across 13 accessors, seven passing ordinary-method controls, and successful external calls. These are supplied investigation results, not tests executed by this planner. Runtime success alone cannot prove this fix.

The issue description calls property-level misuse an error. The verified requirement takes precedence: preserve warning FS0842 at default severity. Explicit promotion can make it an error.

PR #20235, "Enable runtime async via compiler intrinsics", also changes `IlxGen.fs`. It is not this accessor fix. Do not import its feature.

### Starting state and prerequisites

Planning inspected a clean worktree at the baseline above. Planning commits can now precede this sprint. Preserve existing changes and do not reset the branch.

`global.json` requires SDK `11.0.100-rc.1.26420.103` and selects Microsoft.Testing.Platform. During planning, `dotnet --version` failed because that SDK was missing. Installed SDKs were 8, 9, and 10. No local `.dotnet` directory or `artifacts` directory existed.

Provision the required SDK with the repository wrapper if it is still missing. Do not edit `global.json` or dependency versions to avoid setup.

```powershell
Set-Location 'Q:\fsharp-worktrees\issue-877'
$env:BUILDING_USING_DOTNET = 'true'
dotnet --version
# Only when the required SDK is missing:
& .\eng\common\dotnet.ps1 --version
```

After provisioning, use the resolved SDK consistently. The commands below use `dotnet`. Substitute `& .\.dotnet\dotnet.exe` if the local SDK is not selected. Set `BUILDING_USING_DOTNET=true` in each new shell.

Read `.github\copilot-instructions.md`, `.github\instructions\CodeGen.instructions.md`, `.github\instructions\ExpertReview.instructions.md`, `.github\instructions\ComponentTests.instructions.md`, `.github\instructions\NoBloat.instructions.md`, and `docs\representations.md` before editing.

Files under `eng\common` are Arcade-owned. Run their setup wrapper but do not modify them.

## Description - WHAT to implement with DETAILED guidance

### Files to modify

| Path relative to the repository | Change |
|---|---|
| `src\Compiler\CodeGen\IlxGen.fs` | Move the existing accessor partition below `ComputeMethodImplAttribs` in `GenMethodForBinding`. |
| `tests\FSharp.Compiler.ComponentTests\EmittedIL\MethodImplAttribute\MethodImplAttribute.fs` | Add compact raw-metadata regressions and compatibility controls. Preserve existing tests and baselines. |
| `docs\release-notes\.FSharp.Compiler.Service\11.0.100.md` | Add one concise entry under `### Fixed`, after confirming the release target. |

No project-file change is needed when tests remain in the existing module. No public API, new diagnostic, attribute framework, or IL-writer change is needed.

### Existing code and reusable patterns

`ComputeMethodImplAttribs` is near lines 9765-9800 of `IlxGen.fs`. It decodes `NoInlining` (`0x8`), `Synchronized` (`0x20`), `PreserveSig` (`0x80`), and `AggressiveInlining` (`0x100`). It also consumes standalone `PreserveSigAttribute`.

It removes both pseudo attributes from its returned ordinary-attribute list. Preserve its supported bits, combinations, ignored options, ignored `MethodCodeType`, and ignored `int16` constructor behavior.

`GenMethodForBinding` currently has this order near lines 9993-10012:

```fsharp
let attrs = // existing DllImport and CompiledName filtering
    ...
let attrsAppliedToGetterOrSetter, attrs =
    List.partition (fun (Attrib(_, _, _, _, isAppliedToGetterOrSetter, _, _)) -> isAppliedToGetterOrSetter) attrs
let sourceNameAttribs, compiledName =
    ...
let hasPreserveSigImplFlag, hasSynchronizedImplFlag, hasNoInliningFlag, hasAggressiveInliningImplFlag, attrs =
    ComputeMethodImplAttribs cenv v attrs
let securityAttributes, attrs =
    ...
```

The ellipses above indicate existing code, not code to insert. Relocate only the two-line accessor partition. Place it immediately after the decoder call and before the security partition.

This decodes the full current-method list once, after DllImport/CompiledName filtering. Then it partitions the remaining ordinary attributes.

Leave `sourceNameAttribs`, `compiledName`, security processing, property routing, and method construction unchanged. Leave the normal accessor path near lines 10233-10265 and extension path near lines 10270-10285 unchanged. Leave `.WithPreserveSig`, `.WithSynchronized`, `.WithNoInlining`, and `.WithAggressiveInlining` near lines 10297-10303 unchanged.

Use these existing test APIs from `FSharp.Test.Compiler` in `tests\FSharp.Test.Utilities\Compiler.fs`:

```fsharp
FSharp source
|> asLibrary
|> compile
|> shouldSucceed
|> withMetadataReader (fun reader -> (* inspect raw metadata here *))
```

`withMetadataReader`, near line 1000, already owns the PE-reader lifetime. Do not add another output-path/PE-reader wrapper.

For paired sources, use the existing `Fsi` and `FsSource` helpers. They produce matching `test.fsi` and `test.fs` names:

```fsharp
let library =
    Fsi signatureSource
    |> withAdditionalSourceFile (FsSource implementationSource)
    |> asLibrary
    |> withName "AccessorLibrary"
```

Examples exist in `tests\FSharp.Compiler.ComponentTests\Signatures\TestHelpers.fs` and `tests\FSharp.Compiler.ComponentTests\Conformance\Signatures\SignatureEnforcedAttributes.fs`. Reuse the public source helpers, not those modules' private helpers. The signature helpers module compiles after this test module.

For a separate consumer, use `withReferences [ library ]`, `asExe`, an explicit entry point, and `compileExeAndRun |> shouldSucceed`. Pass a `CompilationUnit` to `withReferences`, not a `CompilationResult`. Keep consumer source out of the library's source list.

### Step 1: Establish the baseline and preserve evidence

Create an ignored evidence directory at `.tools\ralph\evidence\20288`. Save logs there, not among committed source files. Preserve the baseline hash, commands, SDK version, exit codes, test counts, and RED/GREEN diagnostics.

Use `.tools\ralph\evidence\20288\progress.txt` for a short resume record. Include completed scenarios and the next command. Never mark a phase complete without its evidence.

Before production edits, run the existing focused MethodImpl tests and `MethodImplNoInline02_fs`. The supplied starting results were nine and four passing cases respectively. Re-establish those counts locally.

Build/test commands are given below. Resolve setup failures before diagnosing the regression. Missing SDKs, invalid fixture syntax, and typechecking failures do not count as RED.

### Step 2: Add raw-metadata tests and obtain RED

Keep additions in the existing `EmittedIL.MethodImplAttribute` module. Retain its eight option-baseline tests and existing inline-keyword warning test without changes.

Use one shared metadata-checking path for new fixtures. Parameterize actual variations, rather than copying complete test bodies. Keep each of scenarios 1-6 independently observable in test output. A failure in scenario 1 must not prevent scenarios 2-6 from running.

Select each expected declaring type and method by exact metadata identity. Require exactly one match, or fail with its expected identity. Do not filter discovered methods and then pass on an empty sequence.

Read `MethodDefinition.ImplAttributes` as an integer and assert the complete expected value, not just selected bits. Read `MethodDefinition.GetCustomAttributes()` and resolve actual constructor handles through `MetadataReader.GetCustomAttribute`.

Use `System.Reflection.Metadata`. External attribute constructors usually use `HandleKind.MemberReference` with a `TypeReference` parent. Locally declared markers can use `HandleKind.MethodDefinition` and `GetDeclaringType()`. Resolve both correctly. Handle any encountered `TypeDefinition` parent. Fail explicitly on unexpected shapes rather than returning an empty name or silently skipping a row.

Compare the combined observation of exact flags and pseudo-attribute absence for each accessor. Include full attribute names:

```text
System.Runtime.CompilerServices.MethodImplAttribute
System.Runtime.InteropServices.PreserveSigAttribute
```

No real row for either pseudo attribute is allowed on the tested concrete methods. Do not reject unrelated compiler-generated attributes. For marker checks, compare the marker subset precisely.

Do not use reflection attribute enumeration. Reflection can synthesize pseudo attributes from implementation flags. Do not use an IL call instruction or successful execution as the flags oracle.

#### Required scenario matrix

| # | Fixture and construction guidance | Assertions |
|---|---|---|
| 1 | Exact issue source shown below: static getter, static setter, and ordinary static method. | `A.get_P1 = 0x100`, `B.set_P2 = 0x8`, `C.M1 = 0x100`. Both accessors and the method lack real pseudo attributes. |
| 2 | Instance property with getter `AggressiveInlining` and setter `NoInlining ||| Synchronized ||| PreserveSig`. Define three small marker attribute types, valid respectively on property, getter method, and setter method. | Getter `0x100`, setter `0xA8`, no real pseudo attributes. Read the PropertyDef and both MethodDefs. Each marker appears exactly once on its intended row and on neither other row. |
| 3 | Getter with standalone `[<PreserveSig>]` plus `[<MethodImpl(MethodImplOptions.NoInlining)>]`. Include an ordinary-method equivalent. | Both methods have `0x88`. Neither pseudo attribute has a real row. |
| 4 | Interface with `abstract P: int with get, set`, implemented explicitly by a concrete class. Attribute the concrete getter with `NoInlining`, and setter with `Synchronized`. | Concrete getter `0x8`, concrete setter `0x20`, no real pseudo attributes. Select the implementing type's MethodDefs, not interface slots. Require non-abstract methods with bodies. |
| 5 | Extrinsic extension property, such as `type System.String with ...` inside a module. Apply `NoInlining ||| AggressiveInlining` and an ordinary getter marker. | Emitted extension accessor is static, has `0x108`, retains its marker, and lacks real pseudo attributes. Do not normalize conflicting inlining flags. This must exercise the extension emission path. |
| 6 | Neutral `.fsi` declares a class constructor and `member P: int with get, set`. Matching `.fs` puts `NoInlining` on the getter and `Synchronized ||| PreserveSig` on the setter. | Concrete library getter `0x8`, setter `0xA0`, no real pseudo attributes. No attributes are needed in the signature. |
| 7 | Compact ordinary-method/accessor parity controls, described below. | Preserve current ordinary-method behavior and ensure an unannotated accessor remains `0x0`. Keep all eight existing option baselines unchanged. |
| 8 | Place `[<MethodImpl(MethodImplOptions.NoInlining)>]` before a property member rather than on its accessor. | Default diagnostic is warning FS0842. A separate, explicitly promoted case rejects it as error FS0842. |
| 9 | Separately compile an executable referencing normal, explicit-interface, and signature-constrained library fixtures. Reuse fixture source definitions. | Getters return expected values. Setters update backing values and subsequent getters observe the updates. Assert interface-dispatched calls and signature-constrained calls. This is a control only. |

Exact issue source:

```fsharp
module P
open System.Runtime.CompilerServices

[<AbstractClass; Sealed>]
type A =
    static member P1 with [<MethodImpl(MethodImplOptions.AggressiveInlining)>] get () = 1

[<AbstractClass; Sealed>]
type B =
    static member P2 with [<MethodImpl(MethodImplOptions.NoInlining)>] set (v: int) = ignore v

[<AbstractClass; Sealed>]
type C =
    [<MethodImpl(MethodImplOptions.AggressiveInlining)>]
    static member M1() = 1
```

For instance and explicit-interface fixtures, use a mutable integer backing field with a simple get/set body. Place accessor attributes after `with` or `and`, immediately before `get` or `set`. Ensure setters accept `int`.

For scenario 5, extend an external type in a module to guarantee an extrinsic extension. A declaration such as `type System.String with member s.P with [<...>] get () = s.Length` is the intended shape. Discover the emitted qualified method name, then assert its exact identity and static flag. Do not assume an ordinary PropertyDef exists for this path.

Use the paired-source helper pattern for scenario 6. The neutral signature can have this shape:

```fsharp
module SignatureLibrary
type C =
    new: unit -> C
    member P: int with get, set
```

For scenario 7, use a compact data table, not a flags/platform/optimization/realsig cross-product. These constructor expressions and results describe the existing decoder:

| Attribute argument or form | Expected implementation flags |
|---|---|
| `MethodImplOptions.NoInlining` | `0x8` |
| `MethodImplOptions.Synchronized` | `0x20` |
| `MethodImplOptions.PreserveSig` | `0x80` |
| `MethodImplOptions.AggressiveInlining` | `0x100` |
| Standalone `PreserveSigAttribute` | `0x80` |
| `MethodImplOptions.NoInlining ||| MethodImplOptions.AggressiveInlining` | `0x108` |
| Each of `ForwardRef`, `InternalCall`, `NoOptimization`, `Unmanaged`, and `AggressiveOptimization` | `0x0` |
| `MethodImplOptions.NoInlining, MethodCodeType = MethodCodeType.Native` | `0x8`, with the method still implemented as IL |
| `MethodImpl(8s)` using the `int16` constructor | `0x0` |
| No attribute | `0x0` |

Reuse existing ordinary-option baseline coverage instead of adding equivalent text baselines. Add only the small raw checks needed for accessor parity and uncovered ordinary forms. Apply the pseudo-attribute absence check to annotated controls too. Record ordinary-control results before the compiler edit.

For scenario 8, use `typecheck`, not compilation, because only diagnostic policy matters. Assert the exact diagnostic code and severity. Use `ignoreWarnings` only to let the harness accept the expected warning, then assert it with `withSingleDiagnostic` or equivalent exact diagnostics.

`ignoreWarnings` changes harness acceptance; it is not an instruction to suppress FS0842. For the rejection case, use `withOptions [ "--warnaserror:842" ]` and assert an error. Do not add broad warning suppression to the valid metadata fixtures.

Run all new scenarios against the unchanged compiler. Save metadata failures for each of scenarios 1-6. Each source must compile successfully before its metadata assertion fails. Capture actual flags and actual pseudo attributes, not merely a nonzero test exit code.

Do not edit production code until these RED results exist. The supplied 26-assertion count describes an earlier investigation, not a requirement to duplicate its assertion structure. All six primary scenarios must be demonstrated locally.

### Step 3: Apply the surgical change and obtain GREEN

Move the existing partition as described above. Do not concatenate saved attribute partitions, decode twice, blacklist attributes globally, or modify `ComputeMethodImplAttribs`.

Invoke the `fsharp-diagnostics` skill immediately after editing `IlxGen.fs`. Follow its parse-first, typecheck-second workflow:

```powershell
& .\.github\skills\fsharp-diagnostics\scripts\get-fsharp-errors.ps1 -ParseOnly src\Compiler\CodeGen\IlxGen.fs
& .\.github\skills\fsharp-diagnostics\scripts\get-fsharp-errors.ps1 src\Compiler\CodeGen\IlxGen.fs
```

These checks do not replace a build. Rebuild the compiler and tests, then rerun the unchanged regression expectations. All scenarios must become GREEN.

Keep DllImport handling, CompiledName handling, ordinary attributes, security attributes, property rows, and both accessor emission paths unchanged except for the corrected pseudo-attribute consumption.

### Local validation commands

Use Release for EmittedIL tests and optimization cases. `BUILDING_USING_DOTNET=true` selects the current .NET target instead of also building desktop tests on Windows.

```powershell
$env:BUILDING_USING_DOTNET = 'true'
dotnet msbuild tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj -getProperty:TargetFrameworks
dotnet build tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj -c Release -v minimal
```

Stop on a failed build. Use `--no-build` only after a successful build of the current source. Confirm the test project uses the rebuilt local compiler, not an installed SDK compiler or stale output.

Run these focused selections before adding tests, for RED, and after the fix as appropriate:

```powershell
$env:BUILDING_USING_DOTNET = 'true'
dotnet test --project tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj -c Release --no-build -- --filter-class "*EmittedIL.MethodImplAttribute*"
dotnet test --project tests\FSharp.Compiler.ComponentTests\FSharp.Compiler.ComponentTests.fsproj -c Release --no-build -- --filter-method "*MethodImplNoInline02_fs*"
```

The existing `MethodImplNoInline02_fs` test is in `tests\FSharp.Compiler.ComponentTests\EmittedIL\Misc\Misc.fs`, near line 200. Its `FileInlineData` sets both `Realsig` and `Optimize` to `BooleanOptions.Both`, yielding four cases. Preserve that matrix.

Verify selection and nonzero test counts. If the installed runner rejects a selector, inspect its help and use the equivalent xUnit v3 filter. Do not report an empty run as success. If necessary, execute the built test DLL with the same filters after discovering its output path.

Keep logs and exit codes for each command. Escalate to related tests only when focused results require it. A full component run is not a substitute for the specific regression evidence.

On an actual build failure, invoke `binlog-analysis`, collect a binary log, and fix the cause. If stale bootstrap output is suspected, preserve logs first. Inspect `git clean -ndx -- artifacts` before cleaning only that generated directory with `git clean -xfd -- artifacts`, then rebuild. Do not delete source, `.tools\ralph`, or another worktree.

### Step 4: Format, review, document, and commit

Format only changed F# files. The explicit request overrides repository-wide formatting:

```powershell
dotnet fantomas src\Compiler\CodeGen\IlxGen.fs tests\FSharp.Compiler.ComponentTests\EmittedIL\MethodImplAttribute\MethodImplAttribute.fs
```

If Fantomas is missing, restore the existing local tools with `dotnet tool restore`, then rerun. Do not install a new formatter or change tool versions.

Inspect the formatting diff and remove unrelated churn. Re-run compiler diagnostics if formatting changes compiler source. Rebuild and rerun focused tests after the final source changes.

Invoke the `reviewing-compiler-prs` skill and its `expert-reviewer` agent on the final implementation diff. This is the available expert-review workflow. Give it the issue contract, baseline hash, exact changed files, and RED/GREEN evidence. Review locally only. Do not let a reviewer post, push, or open a PR.

Require checks of exact flags, actual pseudo-attribute absence, non-vacuous method selection, both emission paths, marker routing, and compatibility controls. Resolve concrete findings and rerun affected checks. Invoke `code-compaction` if the test diff becomes repetitive, overengineered, or exceeds its size trigger. Do not add test-only frameworks or unrelated cleanup.

Invoke the `release-notes` skill. `VNEXT` was `11.0.100` during planning. Confirm with `gh api repos/dotnet/fsharp/actions/variables/VNEXT --jq .value`. Use the compiler-service sink, not FSharp.Core or a new language-feature note.

Use the insertion helper rather than prepending:

```powershell
dotnet fsi .github\skills\release-notes\pick-insert-line.fsx --file docs\release-notes\.FSharp.Compiler.Service\11.0.100.md --section Fixed
```

One suitable entry is:

```markdown
* Fix `MethodImpl` and `PreserveSig` attributes on property accessors to emit method implementation flags instead of real custom attributes. ([Issue #20288](https://github.com/dotnet/fsharp/issues/20288))
```

Use the issue link because this is commit-only work. Do not fabricate a PR number or open a PR to obtain one.

Finish with `git diff --check` and inspect the complete diff. Existing `.bsl` and `.il.bsl` files must be unchanged. Do not set `TEST_UPDATE_BSL` or regenerate baselines to obtain GREEN.

Stage only the implementation, regression tests, and release note. Do not commit evidence logs, generated binaries, temporary source files, or changes owned by another task. Existing planning files can remain tracked but must not enter the production diff as new implementation changes.

Commit with a descriptive message, for example `Fix MethodImpl flags on property accessors`. Include these trailers, substituting the implementing agent's actual session ID:

```text
Co-authored-by: Copilot <223556219+Copilot@users.noreply.github.com>
Copilot-Session: <implementing-session-id>
```

Record the resulting commit hash and final verification outcomes in the ignored progress record. Leave no uncommitted changes from this sprint. Do not push.

## Definition of Done - independently verifiable criteria

- The existing focused suite passes nine cases before new tests, and the existing optimization/realsig selection passes four cases.
- Before production edits, scenarios 1-6 compile successfully and each exposes incorrect raw metadata in preserved RED logs.
- Every expected type and concrete accessor is required to exist exactly once; no regression can pass through an empty selection.
- Scenarios 1-6 assert the exact flags from the matrix and absence of actual MethodImpl/PreserveSig custom attributes together.
- Scenario 2 verifies property/getter/setter marker ownership, including absence from the other two targets.
- Scenario 4 checks concrete explicit-interface methods with bodies, not abstract slots.
- Scenario 5 checks the static extension accessor, retains its marker, and preserves the combined `0x108` flags.
- Scenario 6 verifies the emitted library behind a neutral signature using paired-source helpers.
- Scenario 7 preserves supported bits, standalone PreserveSig, ignored bits, ignored MethodCodeType, ignored int16 decoding, and unannotated-accessor behavior.
- Scenario 8 preserves warning FS0842 by default and rejects only the explicitly promoted test case.
- Scenario 9 separately compiles and executes normal, explicit-interface, and signature-constrained getter/setter calls with value assertions.
- The production diff only relocates the existing accessor partition after the decoder; unrelated routing and emission logic remain unchanged.
- Compiler diagnostics and the Release build succeed with no new warnings after the final source edit.
- All focused MethodImpl tests and all four `MethodImplNoInline02_fs` cases pass locally with unchanged expectations and no skipped regressions.
- Existing option baselines are unchanged, and no baseline regeneration was used.
- Only changed F# files are formatted, and `git diff --check` succeeds.
- Local expert review is complete, concrete findings are resolved, and affected checks were rerun.
- A concise compiler-service release note links to issue #20288.
- Evidence and resume state persist under `.tools\ralph\evidence\20288`, while temporary implementation artifacts are removed.
- The implementation, tests, and release note are committed with the required trailers; no sprint-owned changes remain uncommitted, and nothing was pushed.
