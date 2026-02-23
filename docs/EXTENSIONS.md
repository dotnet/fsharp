# RFC FS-1043 — Extension Work Items

Follow-up work to harden the `ExtensionConstraintSolutions` feature before shipping.
Each item is written test-first: the **Verification** section says exactly what must pass.

---

## Work Item 1: Narrow the Canonicalization Skip

### Goal

With `--langversion:preview`, only SRTP constraints that **have extension methods in scope** should skip weak resolution during generalization. Constraints on built-in numeric operators with no extension context should still eagerly resolve to concrete types, exactly as they do today.

### Verification — Write These Tests First

Add to `tests/FSharp.Compiler.ComponentTests/Conformance/InferenceProcedures/TypeConstraints/IWSAMsAndSRTPs/IWSAMsAndSRTPsTests.fs`:

```fsharp
// TEST 1: Numeric inline without extensions → monomorphic (must pass BEFORE and AFTER the fix)
//
// Expected: compiles, inferred return type is `int`, NOT `^a`
[<Fact>]
let ``Inline numeric square stays monomorphic with preview when no extensions in scope`` () =
    FSharp """
module Test
let inline square x = x * x
let result : int = square 5
    """
    |> withLangVersionPreview
    |> compile
    |> shouldSucceed

// TEST 2: DateTime + TimeSpan intrinsic → monomorphic (must pass BEFORE and AFTER)
//
// Expected: compiles, resolved concretely (DateTime has intrinsic op_Addition)
[<Fact>]
let ``Inline DateTime add resolves eagerly with preview when no extensions in scope`` () =
    FSharp """
module Test
open System
let inline addDay (x: DateTime) = x + TimeSpan.FromDays(1.0)
let result : DateTime = addDay DateTime.Now
    """
    |> withLangVersionPreview
    |> compile
    |> shouldSucceed

// TEST 3: Extension operator in scope → constraint stays generic (must pass AFTER the fix)
//
// Expected: compiles; the inline function keeps a generic SRTP constraint
[<Fact>]
let ``Inline multiply stays generic when extension operator is in scope`` () =
    FSharp """
module Test
type System.String with
    static member (*) (s: string, n: int) = System.String(s.[0], n)

let inline multiply (x: ^T) (n: int) = x * n
let r1 = multiply "a" 3
let r2 = multiply 5 3
    """
    |> withLangVersionPreview
    |> compile
    |> shouldSucceed

// TEST 4: Extension operator NOT in scope → weak resolution resolves it
//
// Expected: compiles, `multiply` inferred as `int -> int -> int`
[<Fact>]
let ``Inline multiply resolves to int when no extension operator is in scope`` () =
    FSharp """
module Test
let inline multiply (x: ^T) (n: int) = x * n
let result : int = multiply 5 3
    """
    |> withLangVersionPreview
    |> compile
    |> shouldSucceed
```

**How to run:** `dotnet test tests/FSharp.Compiler.ComponentTests -c Release --filter "FullyQualifiedName~IWSAMsAndSRTPs" /p:BUILDING_USING_DOTNET=true`

Tests 1, 2, 4 will **fail** on the current branch (because canonicalization is blanket-skipped for all inline). They must pass after the fix.
Test 3 should pass both before and after.

### Implementation

**Compilation order** (determines what can reference what):
```
TypedTree.fs → TypedTreeOps.fs → TypeHierarchy.fs → infos.fs → AccessibilityLogic.fs →
NameResolution.fs → MethodCalls.fs → ConstraintSolver.fs → CheckBasics.fs → CheckExpressions.fs
```

**Step 1** — `ConstraintSolver.fs`: Add a new function that does selective weak resolution:

```fsharp
/// Like CanonicalizeRelevantMemberConstraints, but for constraints with a non-None TraitContext,
/// uses PermitWeakResolution.No (keeping them open for future extension method resolution).
/// Constraints with TraitContext=None (e.g. from FSharp.Core) use PermitWeakResolution.Yes as before.
and CanonicalizeRelevantMemberConstraintsForExtensions (csenv: ConstraintSolverEnv) ndeep trace tps =
    // First pass: resolve constraints without extension context (weak=Yes, same as before)
    // Second pass: attempt non-weak resolution on constraints with extension context (weak=No)
    RepeatWhileD ndeep
        (fun ndeep ->
            tps
            |> AtLeastOneD (fun tp ->
                let ty = mkTyparTy tp
                match tryAnyParTy csenv.g ty with
                | ValueSome tp ->
                    let cxst = csenv.SolverState.ExtraCxs
                    let tpn = tp.Stamp
                    let cxs = cxst.FindAll tpn
                    if isNil cxs then ResultD false
                    else
                        // Partition: constraints with extension context vs without
                        let withCtxt, withoutCtxt =
                            cxs |> List.partition (fun (traitInfo, _) -> traitInfo.TraitContext.IsSome)
                        // Remove all, then re-add with-context ones after solving without-context
                        trace.Exec
                            (fun () -> cxs |> List.iter (fun _ -> cxst.Remove tpn))
                            (fun () -> cxs |> List.iter (fun cx -> cxst.Add(tpn, cx)))
                        // Solve without-context constraints eagerly (weak=Yes)
                        let! r1 =
                            withoutCtxt |> AtLeastOneD (fun (traitInfo, m2) ->
                                let csenv = { csenv with m = m2 }
                                SolveMemberConstraint csenv true PermitWeakResolution.Yes (ndeep+1) m2 trace traitInfo)
                        // Attempt non-weak resolution on with-context constraints
                        let! r2 =
                            withCtxt |> AtLeastOneD (fun (traitInfo, m2) ->
                                let csenv = { csenv with m = m2 }
                                SolveMemberConstraint csenv true PermitWeakResolution.No (ndeep+1) m2 trace traitInfo)
                        return r1 || r2
                | ValueNone -> ResultD false))
```

> **NOTE**: This is pseudocode showing the intent. The actual implementation must handle the ExtraCxs bookkeeping correctly (removing solved constraints, re-adding unsolved ones). Study the existing `SolveRelevantMemberConstraintsForTypar` carefully — it removes all constraints for a typar stamp, solves them, and relies on `AddMemberConstraint` re-adding unsolved ones. The new code must follow the same pattern but partition first.

**Step 2** — `ConstraintSolver.fs`: Export in `.fsi` if `CheckExpressions.fs` calls it directly, OR add a wrapper `CanonicalizePartialInferenceProblemForExtensions`.

**Step 3** — `CheckExpressions.fs`: At the 3 skip sites (lines ~7152, ~11620, ~12654), replace:

```fsharp
// BEFORE:
if not (g.langVersion.SupportsFeature LanguageFeature.ExtensionConstraintSolutions && inlineFlag = ValInline.Always) then
    CanonicalizePartialInferenceProblem cenv.css denv m declaredTypars

// AFTER:
if g.langVersion.SupportsFeature LanguageFeature.ExtensionConstraintSolutions && inlineFlag = ValInline.Always then
    CanonicalizePartialInferenceProblemForExtensions cenv.css denv m declaredTypars
else
    CanonicalizePartialInferenceProblem cenv.css denv m declaredTypars
```

**Step 4** — Run all tests. Verify the 4 new tests pass. Run the full SRTP suite:
```bash
dotnet test tests/FSharp.Compiler.ComponentTests -c Release --filter "FullyQualifiedName~IWSAMsAndSRTPs" /p:BUILDING_USING_DOTNET=true
```

---

## Work Item 2: Structured Error Handling in IlxGen

### Goal

When SRTP constraint resolution fails during codegen, the compiler should tell the user — not silently emit a `throw NotSupportedException`. For `ExprRequiresWitness`, it should only return `false` when the failure is due to genuinely unsolved type parameters.

### Verification — Write These Tests First

```fsharp
// TEST A: Inline function with unsolved constraint used at concrete type → compiler warning
//
// If we define an inline function with an SRTP that stays open (preview + extension context),
// then use it at a site where no extension satisfies it, codegen should warn.
//
// This test might be hard to trigger because type-checking normally catches it.
// The scenario is: constraint solved during type-checking via extensions, but the
// extension method is removed/unavailable by the time codegen re-resolves.
// If we can't trigger it in a non-contrived way, document it as a defensive guard
// and just verify the warning infrastructure works in a unit test.

// TEST B: Existing SRTP reification tests must all pass
// (tests/FSharp.Compiler.ComponentTests/ — look for reification, witness, SRTP in codegen)

// TEST C: FSharp.Core static optimizations still work correctly
// Build FSharp.Core in Release and verify the test suite passes.
```

**How to run:**
```bash
dotnet test tests/FSharp.Compiler.ComponentTests -c Release --filter "FullyQualifiedName~IWSAMsAndSRTPs" /p:BUILDING_USING_DOTNET=true
./build.sh -c Release --testcoreclr
```

### Implementation

**Step 1** — `FSComp.txt`: Add warning:
```
3882,ilTraitCallNotStaticallyResolved,"The member constraint for '%s' could not be statically resolved. A NotSupportedException will be thrown at runtime if this code path is reached."
```

Update all xlf files (`dotnet build src/Compiler -t:UpdateXlf`).

**Step 2** — `IlxGen.fs` `GenTraitCall` (~line 5543):

```fsharp
// BEFORE:
| ErrorResult _ -> None

// AFTER:
| ErrorResult(_, err) ->
    // Emit a diagnostic so the user knows about the runtime throw.
    // This is defensive — type-checking should have caught it — but
    // extension constraint changes can leave unsolved constraints in codegen.
    warning(Error(FSComp.SR.ilTraitCallNotStaticallyResolved(traitInfo.MemberLogicalName), m))
    None
```

**Step 3** — `IlxGen.fs` `ExprRequiresWitness` (~line 7345):

```fsharp
// BEFORE:
| ErrorResult _ -> false

// AFTER:
| ErrorResult _ ->
    // If all support types are concrete (not typars), this is a real failure
    // that should not silently take the non-witness path.
    let allConcrete =
        traitInfo.SupportTypes
        |> List.forall (fun ty -> not (isTyparTy g ty))
    if allConcrete then
        // This shouldn't happen — type-checking should have caught it.
        // But defensively, returning false means "don't use witness path"
        // which leads to the dynamic invocation fallback (safer than broken witnesses).
        false
    else
        // Unsolved typars — genuinely don't know. false = don't use witness path.
        false
```

> For now, both branches return `false` — but the `allConcrete` case is the place to add a warning or assert if we later want to detect impossible scenarios. The structural distinction matters for future maintainers. Add a comment explaining the reasoning.

**Step 4** — Run the full test suite. Verify no regressions. If the new warning fires on any existing test, investigate — it means we found a real latent bug.

---

## Work Item 3: Eliminate `obj` from ITraitContext

### Goal

Replace `obj` with a generic type parameter on `ITraitContext` so that the interface is type-safe. The `obj` was introduced because `TypedTree.fs` compiles before `infos.fs` (which defines `MethInfo`) and `AccessibilityLogic.fs` (which defines `AccessorDomain`). We can solve this with a generic interface.

### Verification — Write These Tests First

The change is purely structural — no behavioral change. Verification:

1. **The compiler must build.** This is the primary test — if layering is wrong, compilation fails.
2. **All existing SRTP tests must pass** (no behavioral change).
3. **No `obj` or downcast `:?` appears in the trait context resolution path.**

```bash
# Must succeed:
dotnet build src/Compiler/FSharp.Compiler.Service.fsproj -c Debug /p:BUILDING_USING_DOTNET=true

# Must succeed:
dotnet test tests/FSharp.Compiler.ComponentTests -c Release --filter "FullyQualifiedName~IWSAMsAndSRTPs" /p:BUILDING_USING_DOTNET=true

# Must find zero matches (no obj casts in the trait context path):
grep -n ":? MethInfo\|:? InfoReader\|:? AccessorDomain" src/Compiler/Checking/ConstraintSolver.fs
# Expected: 0 lines

grep -n "minfo :> obj\|infoReader :> obj\|eAccessRights :> ITraitAccessorDomain" src/Compiler/Checking/CheckBasics.fs
# Expected: 0 lines
```

### Implementation

**Compilation order reminder:**
```
TypedTree.fs (line 330)  ←  ITraitContext lives here, with TTrait
    ↓
infos.fs (line 348)      ←  MethInfo defined here
    ↓
AccessibilityLogic.fs (line 350)  ←  AccessorDomain defined here
    ↓
NameResolution.fs (line 362)      ←  SelectExtensionMethInfosForTrait lives here
    ↓
ConstraintSolver.fs (line 372)    ←  consumes ITraitContext
    ↓
CheckBasics.fs (line 384)         ←  TcEnv implements ITraitContext
```

The problem: `ITraitContext` in `TypedTree.fs` cannot name `MethInfo`, `AccessorDomain`, or `InfoReader`.

**Step 1** — Make `ITraitContext` generic in `TypedTree.fs/fsi`:

```fsharp
/// Represents information about extension methods available at SRTP constraint creation.
/// 'AccessRights is the accessibility domain type (AccessorDomain at use site).
/// 'MethodInfo is the method info type (MethInfo at use site).
/// 'InfoReader is the info reader type (InfoReader at use site).
type ITraitContext<'AccessRights, 'MethodInfo, 'InfoReader> =
    abstract SelectExtensionMethods: traitInfo: TraitConstraintInfo * range: range * infoReader: 'InfoReader -> (TType * 'MethodInfo) list
    abstract AccessRights: 'AccessRights
```

**Step 2** — Define a concrete type alias in `ConstraintSolver.fs` (or a shared file between `ConstraintSolver.fs` and `CheckBasics.fs`):

```fsharp
/// Concrete ITraitContext used throughout the compiler.
type TraitContext = ITraitContext<AccessorDomain, MethInfo, InfoReader>
```

**Step 3** — In `TraitConstraintInfo` (`TypedTree.fs`), the field is currently `traitCtxt: ITraitContext option`. It needs to stay non-generic in the DU (because `TraitConstraintInfo` is used everywhere). Two sub-approaches:

**Sub-approach 3a — Existential via non-generic base interface:**

Keep a non-generic marker interface for storage, add the generic one for typed access:

```fsharp
// TypedTree.fs — at the bottom of the layering
type ITraitContext = interface end

type ITraitContext<'AccessRights, 'MethodInfo, 'InfoReader> =
    inherit ITraitContext
    abstract SelectExtensionMethods: traitInfo: TraitConstraintInfo * range: range * infoReader: 'InfoReader -> (TType * 'MethodInfo) list
    abstract AccessRights: 'AccessRights
```

`TraitConstraintInfo` stores `ITraitContext option` (non-generic). The consumer in `ConstraintSolver.fs` does:

```fsharp
match traitInfo.TraitContext with
| Some (:? TraitContext as tc) -> tc.SelectExtensionMethods(...)
| ...
```

This still has a downcast, but now it's **one** downcast at the boundary, typed as `ITraitContext<AccessorDomain, MethInfo, InfoReader>`, and all method calls after that are fully typed. The `obj` is gone from the return values and parameters.

**Sub-approach 3b — Callback closures (no generics needed):**

Instead of an interface, store typed closures directly:

```fsharp
// TypedTree.fs
type TraitContext = {
    SelectExtensionMethods: TraitConstraintInfo * range * obj -> (TType * obj) list
    AccessRights: obj
}
```

This doesn't help with obj.

**Sub-approach 3c — Move the type parameter to the TTrait field using a wrapper:**

```fsharp
// TypedTree.fs
[<AbstractClass>]
type TraitContextBase() =
    abstract SelectExtensionMethodsUntyped: TraitConstraintInfo * range * obj -> (TType * obj) list
    abstract AccessRightsUntyped: obj

// ConstraintSolver.fs (or a file between infos.fs and ConstraintSolver.fs)
[<AbstractClass>]
type TraitContext() =
    inherit TraitContextBase()
    abstract SelectExtensionMethods: TraitConstraintInfo * range * InfoReader -> (TType * MethInfo) list
    abstract AccessRights: AccessorDomain
    override x.SelectExtensionMethodsUntyped(ti, m, ir) =
        x.SelectExtensionMethods(ti, m, ir :?> InfoReader) |> List.map (fun (ty, mi) -> (ty, mi :> obj))
    override x.AccessRightsUntyped = x.AccessRights :> obj
```

The `TraitContextBase` in TypedTree has `obj`, but **nobody calls the Untyped methods** — they go through the typed `TraitContext` subclass. The cast is hidden inside the base class adapter, never at call sites.

**Recommended: Sub-approach 3a.** It's the cleanest:

- `ITraitContext` (non-generic marker) stays in `TypedTree.fs` for storage
- `ITraitContext<'A, 'M, 'I>` (generic, typed) also in `TypedTree.fs` for the contract
- `type TraitContext = ITraitContext<AccessorDomain, MethInfo, InfoReader>` alias in ConstraintSolver.fs
- One `:? TraitContext` cast at the entry to `SolveMemberConstraint` and `GetRelevantMethodsForTrait`
- All `minfo :> obj` and `:? MethInfo as mi` gone from the resolution path
- `ITraitAccessorDomain` marker interface deleted — `AccessRights` returns `'AccessRights` directly

**Step 4** — Update `CheckBasics.fs` `TcEnv` to implement the generic interface:

```fsharp
interface ITraitContext<AccessorDomain, MethInfo, InfoReader> with
    member tenv.SelectExtensionMethods(traitInfo, m, infoReader) =
        SelectExtensionMethInfosForTrait(traitInfo, m, tenv.eNameResEnv, infoReader)
    member tenv.AccessRights = tenv.eAccessRights
```

No more `minfo :> obj` or `infoReader :> obj`.

**Step 5** — Update `CreateImplFileTraitContext` in `ConstraintSolver.fs`:

```fsharp
{ new TraitContext with
    member _.SelectExtensionMethods(traitInfo, _m, _infoReader) =
        [ for supportTy in traitInfo.SupportTypes do
              match tryTcrefOfAppTy g supportTy with
              | ValueSome tcref ->
                  for vref in extensionVals.Value.FindAll(tcref.Stamp) do
                      if vref.LogicalName = nm then
                          yield (supportTy, MethInfo.FSMeth(g, supportTy, vref, None))
              | _ -> () ]
    member _.AccessRights = AccessibleFromEverywhere }
```

No `obj` anywhere.

**Step 6** — Delete `ITraitAccessorDomain` from `TypedTree.fs/fsi` and its `interface` impl from `AccessibilityLogic.fs/fsi`.

**Step 7** — Run verification commands from above.

---

## Ordering

| Item | Priority | Parallelizable | Estimated scope |
|------|----------|----------------|-----------------|
| 1 — Narrow canonicalization | **Highest** | Yes | 1–2 days |
| 2 — IlxGen error handling | High | Yes | 0.5–1 day |
| 3 — Type-safe ITraitContext | Medium | Yes | 1–2 days |

All three are independent. Start with Item 1 (biggest user-facing impact). Item 3 can be done at any time as a pure refactor.
