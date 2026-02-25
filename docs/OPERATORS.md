# OPERATORS.md — RFC FS-1043 Implementation TODO List

> **Branch**: `feature-operators-extensions` (off `main`)
> **Feature flag**: `LanguageFeature.ExtensionConstraintSolutions` → `--langversion:preview`
> **Last updated**: 2026-02-25

---

## What this is

A self-standing task list for a fresh-context agent or contributor to pick up and implement
remaining work on RFC FS-1043. Every item has file locations, enough context to act on
independently, and a clear "done when" criterion.

## The RFC

**RFC FS-1043**: Extension members become available to solve operator trait constraints.

- **Original RFC**: [`fslang-design/RFCs/FS-1043-extension-members-for-operators-and-srtp-constraints.md`](https://github.com/fsharp/fslang-design/blob/main/RFCs/FS-1043-extension-members-for-operators-and-srtp-constraints.md)
- **Proposed changes**: `docs/RFC_Changes.md` (this branch) — additions are marked with `<!-- BEGIN PROPOSED ADDITION -->` / `<!-- BEGIN PROPOSED CHANGE -->` HTML comments
- **Original (for diffing)**: `docs/RFC_Original.md`
- **Linked suggestions**: [#230](https://github.com/fsharp/fslang-suggestions/issues/230), [#29](https://github.com/fsharp/fslang-suggestions/issues/29), [#820](https://github.com/fsharp/fslang-suggestions/issues/820)

### One-sentence summary

Extension methods were previously ignored by SRTP constraint resolution; this RFC makes them
participate, adds `AllowOverloadOnReturnTypeAttribute`, and suppresses weak resolution for
inline code when extension candidates are in scope.

## Branch orientation

### How to build & test

```bash
# Build the compiler (sets BUILDING_USING_DOTNET=true):
dotnet build src/Compiler/FSharp.Compiler.Service.fsproj -c Debug

# Spot-check the main test file:
dotnet test tests/FSharp.Compiler.ComponentTests -c Release \
  --filter "FullyQualifiedName~IWSAMsAndSRTPs" /p:BUILDING_USING_DOTNET=true

# Cross-version compat:
dotnet fsi tests/FSharp.Compiler.ComponentTests/CompilerCompatibilityTests.fsx

# Full validation (before submit):
./build.sh -c Release --testcoreclr
```

### Branch commit history (chronological, 46 commits)

| Phase | Commits | Summary |
|-------|---------|---------|
| **Foundation** | `156dae0..c3dc196` (4) | Feature flag, `ITraitContext`/`ITraitAccessorDomain` interfaces, `TTrait.traitCtxt` field, thread context through freshening |
| **Core solver** | `7ab37f1..dffe07f` (3) | Extension methods in SRTP solving, FS1215 gating, optimizer fallback |
| **Tests round 1** | `01b6003..2391ff9` (4) | Edge cases, IWSAM interaction, cross-assembly, scope capture |
| **AllowOverloadOnReturnType** | `a5f9c1b..cb1f062` (5) | Attribute definition, compiler wiring, surface area, tests |
| **Weak resolution** | `ff5bf98..c2cc352` (4) | Disable weak resolution for inline+extension, compat tests |
| **Refactoring** | `97fa4e9..b752b29` (5) | `ExtensionMethInfosOfTypeInScope` refactor, quality, docs |
| **Tests round 2** | `bfd4023..b9f9596` (8) | Witness quotations, breaking changes, widening, op_Implicit, CompilerCompat |
| **Hardening** | `fdc857a..dc3b588` (8) | FS3882 warning, canonicalization narrowing, generic `ITraitContext` |

### Key changed files

| Area | Files |
|------|-------|
| **Type system** | `TypedTree.fs/fsi` (TTrait, ITraitContext), `TypedTreeOps.fs/fsi`, `TypedTreeBasics.fs`, `TypedTreePickle.fs` |
| **Constraint solving** | `ConstraintSolver.fs/fsi`, `CheckExpressions.fs/fsi`, `CheckExpressionsOps.fs` |
| **Overload resolution** | `MethodCalls.fs`, `infos.fs/fsi`, `OverloadResolutionCache.fs/fsi` |
| **Code generation** | `IlxGen.fs`, `Optimizer.fs` |
| **Feature gating** | `LanguageFeatures.fs/fsi`, `FSComp.txt`, `CompilerDiagnostics.fs` |
| **FSharp.Core** | `prim-types.fs/fsi` (AllowOverloadOnReturnTypeAttribute) |
| **Tests** | `IWSAMsAndSRTPsTests.fs` (~86 tests), `CompilerCompat/` suite |

### Primary test file

`tests/FSharp.Compiler.ComponentTests/Conformance/Types/TypeConstraints/IWSAMsAndSRTPs/IWSAMsAndSRTPsTests.fs`

~86 test methods covering: extension operators, weak resolution, witness quotations, cross-assembly,
breaking changes, IWSAM interaction, AllowOverloadOnReturnType, instance extensions.

---

## Table of Contents

- [Architecture Overview](#architecture-overview)
- [DONE — Implemented & Tested](#done--implemented--tested)
- [TODO — Implementation Gaps](#todo--implementation-gaps)
- [TODO — Test Coverage Gaps](#todo--test-coverage-gaps)
- [TODO — AllowOverloadOnReturnType](#todo--allowoverloadonreturntype)
- [TODO — Documentation & Cleanup](#todo--documentation--cleanup)
- [Known Risks & Open Questions](#known-risks--open-questions)

---

## Architecture Overview

### Data flow

```
Extension operators defined → TcEnv.TraitContext captures them
    → TTrait.traitCtxt field carries context through constraint solving
    → ConstraintSolver.GetRelevantMethodsForTrait appends extensions
    → Overload resolution picks winner (intrinsic > extension)
    → IlxGen emits resolved call (or FS3882 warning + runtime throw)
```

### Key files

| File | Role |
|------|------|
| `TypedTree.fs:2596` | `TTrait` type with `traitCtxt: ITraitContext option` field |
| `TypedTree.fs:1-2` | `ITraitContext` marker + generic typed interface |
| `CheckBasics.fs:255` | `TcEnv` implements `ITraitContext<AccessorDomain, MethInfo, InfoReader>` |
| `ConstraintSolver.fs:2265-2284` | `GetRelevantMethodsForTrait` — collects extension methods |
| `ConstraintSolver.fs:4356-4411` | `CanonicalizePartialInferenceProblemForExtensions` — weak resolution split |
| `ConstraintSolver.fs:3521-3524` | `AllowOverloadOnReturnType` check in `ResolveOverloading` |
| `CheckExpressions.fs:1154` | FS1215 warning gating |
| `CheckExpressions.fs:7152` | Inline canonicalization branch |
| `IlxGen.fs:5545` | FS3882 warning emission |
| `Optimizer.fs:3033,4414` | Trait context restoration during optimization |
| `TypedTreePickle.fs:2133,2170` | Pickling — `traitCtxt` NOT serialized (correct) |
| `LanguageFeatures.fs:107,255` | Feature flag definition (preview) |
| `infos.fs:987-992` | `HasAllowOverloadOnReturnType` property |
| `prim-types.fs:94-97` | `AllowOverloadOnReturnTypeAttribute` definition |

### Feature flag checks (8 sites)

| Location | Purpose |
|----------|---------|
| `ConstraintSolver.fs:1670` | Core solver — enable extension method collection |
| `ConstraintSolver.fs:2268` | `GetRelevantMethodsForTrait` — extension lookup |
| `CheckExpressions.fs:1154` | Gate FS1215 warning |
| `CheckExpressions.fs:7152` | Inline canonicalization path |
| `CheckExpressions.fs:11620` | Partial inference partitioning |
| `CheckExpressions.fs:12665` | Additional extension gating |
| `Optimizer.fs:3033` | Trait context restoration |
| `Optimizer.fs:4414` | Optimizer extension support |

---

## DONE — Implemented & Tested

- [x] `ITraitContext` / `ITraitAccessorDomain` interfaces defined
- [x] `TTrait` extended with `traitCtxt` field
- [x] `TcEnv` implements `ITraitContext`
- [x] Trait context threaded through freshening and name resolution
- [x] Extension methods collected in `GetRelevantMethodsForTrait`
- [x] Intrinsic methods preferred over extension methods (append order)
- [x] Weak resolution disabled for inline code with extension context
- [x] `CanonicalizePartialInferenceProblemForExtensions` partitions constraints
- [x] FS1215 no longer emitted when feature enabled
- [x] FS3882 warning for unsolved SRTP at codegen
- [x] Pickling: `traitCtxt` correctly NOT serialized
- [x] Optimizer: fallback trait context for optimization passes
- [x] `AllowOverloadOnReturnTypeAttribute` defined in FSharp.Core
- [x] `HasAllowOverloadOnReturnType` wired into `MethInfo`
- [x] Return type considered in `ResolveOverloading` when attribute present
- [x] `PostInferenceChecks` allows return-type-only method overloads with attribute
- [x] Cross-assembly extension operator resolution
- [x] Witness quotation tests for extension operators
- [x] CompilerCompat suite: extension operator on external type (T6)
- [x] Breaking change regression tests (S2, S3, S4, S6)
- [x] Instance extension method SRTP tests
- [x] Cross-accessibility-domain constraint propagation tests

---

## TODO — Implementation Gaps

### T1. `traitCtxtNone` call sites (17 remaining)

Many call sites still pass `traitCtxtNone` (i.e., `None`) instead of a real trait context.
Each is a potential gap where extension operators in scope are invisible to SRTP solving.

**Must audit each — some are correct (no TcEnv available), some are bugs:**

| File | Line | Call | Verdict needed |
|------|------|------|---------------|
| `fsc.fs:839` | `LightweightTcValForUsingInBuildMethodCall` | Likely OK — post-typecheck codegen |
| `fsc.fs:957` | `CreateIlxAssemblyGenerator` | Likely OK — codegen |
| `fsi.fs:2184` | `LightweightTcValForUsingInBuildMethodCall` | Likely OK — FSI codegen |
| `fsi.fs:3101` | `CreateIlxAssemblyGenerator` | Likely OK — FSI codegen |
| `Symbols.fs:62` | `LightweightTcValForUsingInBuildMethodCall` | Likely OK — IDE symbol API |
| `Symbols.fs:392` | `FreshenTypeInst` | **Audit**: IDE type freshening — should extension context flow? |
| `FSharpCheckerResults.fs:3870` | `LightweightTcValForUsingInBuildMethodCall` | Likely OK — checker results |
| `CheckFormatStrings.fs:21` | `FreshenAndFixupTypars` | Likely OK — format strings unrelated to SRTP |
| `ConstraintSolver.fs:4307` | `FreshenTypeInst` | **Audit**: inside solver — should this carry context? |
| `ConstraintSolver.fs:4431` | `FreshenMethInfo` | **Audit**: method freshening in solver |
| `CheckBasics.fs:353` | `FreshenTypars` | **Audit**: `instantiationGenerator` — should env.TraitContext flow? |
| `NameResolution.fs:3232` | `FreshenTypeInst` | **Audit**: name resolution freshening |
| `NameResolution.fs:3609` | `FreshenTypeInst` | **Audit**: name resolution freshening |
| `NameResolution.fs:3772` | `FreshenTypeInst` | **Audit**: name resolution freshening |
| `infos.fs:1365` | `FixupNewTypars` | **Audit**: method info freshening |
| `infos.fs:1367` | `FixupNewTypars` | **Audit**: method info freshening |

**Action**: For each "Audit" site, determine if a `TcEnv` with trait context is available
in the calling scope. If yes, thread it through. If not, document why `None` is correct.

### T2. FS1215 gating only checks `IsLogicalOpName`

**Location**: `CheckExpressions.fs:1154`

```fsharp
if isExtrinsic && IsLogicalOpName id.idText && not (...ExtensionConstraintSolutions) then
    warning(...)
```

The guard `IsLogicalOpName` means FS1215 only fires for *logical operator names* (`op_Addition`, etc.).
Non-operator extension members that could be SRTP witnesses (e.g., `member Foo`) never got this warning
and never needed gating. **Verified**: This is intentional — FS1215 specifically warns about "operator overloads" in extensions.
Non-operator extension members never triggered this warning and don't need gating.
See comment in `CheckExpressions.fs:1154`.

### T3. No feature flag for `AllowOverloadOnReturnType`

**Location**: `LanguageFeatures.fs` — NO entry for AllowOverloadOnReturnType

The attribute is defined but has **no language feature flag**. It's always active in the compiler
if the attribute is present on a method. This means:

- No `--langversion` gating
- No way to disable it
- No feature description string for diagnostics

**Decision**: Attribute-gated is sufficient. The attribute is self-gating — it only takes effect
when explicitly present on a method. No `LanguageFeature` entry needed.
See comment in `ConstraintSolver.fs:3521`.

---

## TODO — Test Coverage Gaps

### TC1. `open type` interaction with SRTP scope

**Gap**: No test verifies whether `open type SomeType` brings extension operators into SRTP solving scope.

```fsharp
module Ops =
    type System.String with
        static member (*) (s, n: int) = String.replicate n s

open type Ops  // Does this bring (*) into SRTP scope?

let inline repeat (s: ^T) (n: int) = s * n
let r = repeat "hello" 3  // Should this work?
```

**Files**: `IWSAMsAndSRTPsTests.fs`

### TC2. Extension operators in computation expressions

**Gap**: No test verifies extension operators work inside `async { }`, `task { }`, or custom CEs.

```fsharp
type System.String with
    static member (+) (s: string, n: int) = s + string n

let result = async {
    let inline add x y = x + y
    return add "test" 42
}
```

### TC3. Extension operators + units of measure

**Gap**: No test for extension operators on types with UoM.

```fsharp
[<Measure>] type kg

type float<[<Measure>]'u> with
    static member (*) (x: float<'u>, y: float<'v>) : float<'u * 'v> = ...
```

### TC4. `AllowOverloadOnReturnType` end-to-end (SDK FSharp.Core)

**Gap**: Test at `IWSAMsAndSRTPsTests.fs:2253` expects **error 39** because the attribute
isn't in the SDK's FSharp.Core yet.

```fsharp
// Currently:
|> shouldFail
|> withDiagnosticMessageMatches "AllowOverloadOnReturnType"
// Needs: actual positive test when attribute ships
```

**Blocker**: Attribute must ship in FSharp.Core NuGet before this test can pass.
Add a conditional test that runs against locally-built FSharp.Core.

### TC5. Ambiguity error messages from extension operators

**Gap**: Only one test (`line 1971`) covers ambiguity. Need more:

- Two extensions with same operator signature → error message quality
- Extension vs intrinsic with same signature → priority verification
- Extension vs IWSAM implementation → which wins, error message

### TC6. Extension operators on struct types

**Gap**: No test specifically targets `[<Struct>]` types with extension operators
and verifies no boxing occurs at the call site.

### TC7. Recursive inline functions with extension SRTP

**Gap**: Test `8041287d2` rewrote a recursive test as negative (error 1114).
Need positive test for valid recursive inline patterns with extensions.

### TC8. Extension operators with `and` constraints

**Gap**: No test for extension operators satisfying multiple SRTP constraints simultaneously:

```fsharp
let inline addAndMultiply (x: ^T) (y: ^T) =
    (x + y) * (x - y)  // where +, -, * are all extensions
```

### TC9. CompilerCompat: AllowOverloadOnReturnType cross-version

**Gap**: `CompilerCompatLib/Library.fs` does not include any `AllowOverloadOnReturnType` test case.
If the attribute changes overload resolution, cross-version behavior must be validated.

### TC10. CompilerCompat: numeric widening cross-version

**Gap**: The RFC widening example (`1 + 2L` via extension operators) is tested in-process
but not in the cross-compiler CompilerCompat suite.

---

## TODO — AllowOverloadOnReturnType

### AO1. Attribute not shipped in SDK FSharp.Core

The attribute is defined in `prim-types.fs:94-97` but only exists in locally-built FSharp.Core.
External consumers cannot use it until it ships in a public NuGet.

**Action**: Track shipping timeline. Ensure surface area baselines are updated.

### AO2. Interaction with `op_Explicit` / `op_Implicit` special casing

**Location**: `ConstraintSolver.fs:3524`

```fsharp
let alwaysConsiderReturnType = isOpConversion || hasAllowOverloadOnReturnType
```

The attribute effectively generalizes the `op_Explicit`/`op_Implicit` special case.
**Verify**: When attribute is on `op_Explicit` overloads, does double-application
of the return-type rule cause issues? Write a test.

### AO3. Composition with extension method candidates

**Question**: When `AllowOverloadOnReturnType` is on an intrinsic method and an extension
method of the same name is also in scope, does return-type resolution correctly pick between them?
No test covers this intersection.

### AO4. Quotation witness determinism

**Question**: With return-type-aware resolution, is the witness captured in quotations
deterministic? The resolution order matters for `<@ expr @>` stability.

---

## TODO — Documentation & Cleanup

### D1. Remove `<!-- BEGIN/END PROPOSED -->` markers from RFC

Before submitting `docs/RFC_Changes.md` to the language design committee,
strip the review-round HTML comment markers.

### D2. Strip `Debug.Assert` traces from IlxGen

Commit `dc971b9fa` added `Debug.Assert` traces to ErrorResult paths in IlxGen.
These are development aids and should be removed or converted to proper diagnostics
before merge.

**Location**: `IlxGen.fs` — 8 `Debug.Assert(false, ...)` calls

### D3. First-person narrative in RFC

Lines 350+ in `RFC_Changes.md` retain first-person prose ("I'm examining...",
"I have found one case...", "I guess there may be..."). This should be
rewritten in specification style before submission.

### D4. Release notes

Commit `b646bb7b0` added release notes. Verify they match the final RFC state,
especially regarding FS3882, `AllowOverloadOnReturnType`, and the IWSAM non-interaction.

---

## Known Risks & Open Questions

### R1. Performance with heavily overloaded operators

The RFC warns: "compiler performance is poor when resolving heavily overloaded constraints."
Extension methods increase the candidate set. Libraries like FSharpPlus define many overloads.

**Action**: Profile constraint solving with a FSharpPlus-sized overload set.
Ensure linear-ish behavior up to ~100 candidates.

### R2. FSharpPlus weak resolution breakage

The RFC documents a specific breakage pattern: return types used as support types
in SRTP constraints (e.g., `(^M or ^I or ^R)`) may fail after weak resolution changes.

**Workaround documented**: Remove `^R` from support types.
**Status**: FSharpPlus coordination deferred.

### R3. `implies` / `consistent` with different accessibility domains

Resolution from the RFC: "Tested with two modules, no failing cases found."
The invariant (SRTP constraints are freshened locally per use site) makes this low-risk,
but no formal proof exists.

**Test references**: `IWSAMsAndSRTPsTests.fs`, `SRTPExtensionOperatorTests`

### R4. Pickling correctness for trait solutions with extensions

`TypedTreePickle.fs:2108-2131` serializes trait solutions, including tags 6-7 for
solutions "with extension constraints". The `traitCtxt` field itself is NOT pickled.

**Verify**: A library compiled with an extension operator solution can be consumed
by a compiler that doesn't know about the extension. The solution tag must be
backward-compatible.

### R5. Optimizer trait context restoration

`Optimizer.fs:3033` restores trait context when the feature is enabled:
```fsharp
| None, Some tc when g.langVersion.SupportsFeature LanguageFeature.ExtensionConstraintSolutions ->
```

**Risk**: If the optimizer inlines across module boundaries, the restored context
may differ from the original. Verify with cross-module optimization tests.
