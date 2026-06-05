# Research Notes: dotnet/fsharp#16432 / PR #19862

## Summary

For an `inherit T()` clause where `T` is undefined, FSC emits FS0039 ("The type
`T` is not defined") **three times** at the exact same `(line, column)` range.
The current PR (#19862) masks the duplicates with a `cenv`-scoped
`InheritDedupDiagnosticsLogger` keyed on `(idRange, idText)` of the
`UndefinedName` exception. Empirical tracing confirms the root cause: the
identical `SynType` node embedded in the inherit clause is type-checked
**three** times by three independent passes of `CheckDeclarations`:
Phase 1D (FirstPass) and Phase 1F (SecondPass) of
`TcTyconDefnCore_Phase1D_Phase1F_EstablishSuperTypesAndInterfaceTypes`, then
Phase 2A `Phase2AInherit` of `MutRecBindingChecking`. Each pass independently
invokes name resolution on the same `SynType`, and each independent failure
emits FS0039 through the diagnostics sink.

## The three call sites (verified)

| # | Phase                  | File:Line                        | Call             | Occurrence  | CheckCxs     | env          |
| - | ---------------------- | -------------------------------- | ---------------- | ----------- | ------------ | ------------ |
| 1 | Phase 1D (FirstPass)   | `CheckDeclarations.fs:3354`      | `TcTypeAndRecover` | `UseInType` | `NoCheckCxs` | `envinner`   |
| 2 | Phase 1F (SecondPass)  | `CheckDeclarations.fs:3354`      | `TcTypeAndRecover` | `UseInType` | `CheckCxs`   | `envinner`   |
| 3 | Phase 2A               | `CheckDeclarations.fs:1399` (inside `try`, call at line 1403) | `TcType` | `Use`       | `CheckCxs`   | `envInstance` |

Drivers for sites #1 and #2 (same function, two passes), in the same file:

```text
CheckDeclarations.fs:4180   (envMutRecPrelim, withAttrs) |> TcTyconDefnCore_Phase1D_Phase1F_EstablishSuperTypesAndInterfaceTypes cenv tpenv inSig FirstPass
CheckDeclarations.fs:4216   (envMutRecPrelim, withAttrs) |> TcTyconDefnCore_Phase1D_Phase1F_EstablishSuperTypesAndInterfaceTypes cenv tpenv inSig SecondPass
```

Each pass walks `SynTypeDefnSimpleRepr.General(_, inherits, ...)` and feeds
the inherit `SynType` to `TcTypeAndRecover`. The recovery point that actually
calls `errorRecovery` (i.e. emits the diagnostic) is
`TcTypeOrMeasureAndRecover` at `CheckExpressions.fs:5217`:

```fsharp
and TcTypeOrMeasureAndRecover kindOpt (cenv: cenv) newOk checkConstraints occ iwsam env tpenv ty =
    let g = cenv.g
    try
        TcTypeOrMeasure kindOpt cenv newOk checkConstraints occ iwsam env tpenv ty
    with RecoverableException e ->
        errorRecovery e ty.Range
        ...
        recoveryTy, tpenv
```

Site #3 uses raw `TcType` and recovers via its own outer `try/with
RecoverableException e -> errorRecovery e m; mkUnit g m, tpenv` at
`CheckDeclarations.fs:1407-1410`. All three sites ultimately funnel into
`errorRecovery`, which routes through the active `DiagnosticsLogger`.

## Why each pass exists (with quotes from source)

- **Phase 1D / Phase 1F** — both runs share one function whose header
  documents the dual-pass design (`CheckDeclarations.fs:3322-3324`):

  > `// Third phase: check and publish the super types. Run twice, once before constraints are established`
  > `// and once after`

  The first pass (`NoCheckCxs`) is needed so the *shape* of the super-type
  hierarchy is published into the symbol table before user-supplied
  constraints are checked. The second pass (`CheckCxs`) re-resolves the same
  syntax now with full constraint checking enabled.

- **Phase 2A** (`MutRecBindingChecking`, `CheckDeclarations.fs:1398-1399`):

  > `// Phase2B: typecheck the argument to an 'inherits' call and build the new object expr for the inherit-call`
  > `| Phase2AInherit (synBaseTy, arg, baseValOpt, m) ->`

  This is the *expression* type-checking pass. It needs to resolve
  `synBaseTy` again because at this point it builds the actual `TcNewExpr`
  for the constructor call to the base type (the previous two passes only
  computed `TType`s for the inheritance graph; they did not elaborate the
  `inherit Base(args)` expression). It also uses `envInstance`, which is the
  enriched environment containing the implicit-constructor args, not
  `envinner`.

## Evidence — failing test output with PR mechanism neutralised

`useInheritDedupLogger` was changed locally to a pass-through:

```fsharp
let private useInheritDedupLogger (cenv: cenv) =
    ignore cenv
    UseTransformedDiagnosticsLogger(fun inner -> inner)
```

Running:

```bash
dotnet test --project tests/FSharp.Compiler.ComponentTests/FSharp.Compiler.ComponentTests.fsproj \
    -c Debug --no-build -- --filter-method "*Inherit nonexistent type reports single FS0039*" --output Detailed
```

Verbatim assertion failure (test source: `tests/FSharp.Compiler.ComponentTests/Conformance/ObjectOrientedTypeDefinitions/ClassTypes/InheritsDeclarations/InheritsDeclarations.fs:77`):

```
Expected exactly 1 FS0039 but got 3. Diagnostics:
[{ Error = Error 39
   Range = { StartLine = 3
             StartColumn = 12
             EndLine = 3
             EndColumn = 27 }
   NativeRange = (3,12--3,27)
   Message = "The type 'NonExistentBase' is not defined."
   SubCategory = "typecheck" };
 { Error = Error 39
   Range = { StartLine = 3
             StartColumn = 12
             EndLine = 3
             EndColumn = 27 }
   NativeRange = (3,12--3,27)
   Message = "The type 'NonExistentBase' is not defined."
   SubCategory = "typecheck" };
 { Error = Error 39
   Range = { StartLine = 3
             StartColumn = 12
             EndLine = 3
             EndColumn = 27 }
   NativeRange = (3,12--3,27)
   Message = "The type 'NonExistentBase' is not defined."
   SubCategory = "typecheck" }]
```

All three diagnostics carry **identical** `NativeRange = (3,12--3,27)` and
identical message, confirming they originate from re-resolving the *same*
syntactic node, not from three different syntactic occurrences.

## Evidence — `eprintfn` trace

Per-site `eprintfn` was added at sites #1/#2 (`CheckDeclarations.fs:3353`)
and #3 (`CheckDeclarations.fs:1400`). Test stdout captured:

```
[TRACE 16432] Phase1D/1F inherit-resolve pass=FirstPass tycon=MyClass
[TRACE 16432] Phase1D/1F inherit-resolve pass=SecondPass tycon=MyClass
[TRACE 16432] Phase2A inherit-resolve range=(3,12--3,27)
```

Exactly three resolutions for the one `inherit NonExistentBase()` clause; the
Phase 2A range `(3,12--3,27)` matches the range of every emitted FS0039,
confirming the three diagnostics are produced by these three sites and only
these three sites.

## SynType identity across phases

`CheckDeclarations.fs:4579-4583` collects inherits from member definitions
into the `SynTypeDefnSimpleRepr.General.inherits` list:

```fsharp
let inherits =
    members |> List.choose (function
        | SynMemberDefn.Inherit (Some ty, idOpt, m, _) -> Some(ty, m, idOpt)
        | SynMemberDefn.ImplicitInherit (ty, _, idOpt, m, _) -> Some(ty, m, idOpt)
        | _ -> None)
```

The very same `ty: SynType` object — produced once by the parser — flows two
ways:

1. Into the `SynTypeDefnSimpleRepr.General(_, inherits, ...)` list consumed
   by Phase 1D/1F (`CheckDeclarations.fs:3348-3354`), where each pass
   destructures `(ty, m, _)` and passes `ty` to `TcTypeAndRecover`.
2. Remains inside the original `SynMemberDefn.ImplicitInherit (ty, _, _, _, _)`
   (resp. `SynMemberDefn.Inherit (Some ty, ...)`) which is what
   `MutRecBindingChecking` later receives as `Phase2AInherit (synBaseTy, arg,
   baseValOpt, m)` — i.e. **`synBaseTy` is the same `SynType` reference**.

Therefore `synBaseTy.Range == ty.Range` is true *by construction*, not by
coincidence. The trace above provides empirical confirmation: the Phase 2A
range `(3,12--3,27)` equals the range of every reported diagnostic, which is
the parser-assigned range of that one `SynType` node.

## Cache-key rationale

Because the `SynType` node is shared across passes, the pair
`(tcref.Stamp, ty.Range)` — equivalently `(tcref.Stamp, synBaseTy.Range)` at
the Phase 2A site — **uniquely identifies a single inherit clause across all
three passes**. `tcref.Stamp` disambiguates two different tycons that happen
to share a source range (e.g. when the syntax came from a synthetic source),
and `ty.Range` distinguishes multiple inherits within the same tycon
(currently only legal once per class, but the key is robust to interface
implementations and to future relaxation).

Notably, the PR's current key `(idRange, idText)` of the `UndefinedName`
exception works too because `idRange` for an inherit identifier coincides
with the embedded `SynType`'s identifier range — but it is keyed on the
diagnostic payload rather than on the syntactic node itself. The root-cause
key `(tcref.Stamp, ty.Range)` is preferable because it can short-circuit the
*resolution attempt* rather than just suppress the resulting diagnostic.

## Recommended next step (root-cause fix)

Replace the diagnostics-sink dedup with a per-`cenv`
`ConcurrentDictionary<struct(Stamp * range), unit>` (call it
`inheritResolutionFailed`). At each of the three call sites, when the
`TcType*` call raises a `RecoverableException` whose underlying exception is
an `UndefinedName`, add `(tcref.Stamp, ty.Range)` to the set *and* let the
first pass emit its diagnostic normally. On the subsequent two passes, before
invoking `TcTypeAndRecover` / `TcType`, look up the key; if present,
short-circuit to `g.obj_ty_ambivalent` / `mkUnit g m` directly without
calling name resolution at all. This avoids:

- emitting duplicate FS0039s (the primary goal),
- doing redundant work in name resolution and the tc-sink for the two
  later passes, and
- masking *unrelated* recoverable diagnostics (constraint failures, type
  provider failures) that the current sink-level dedup would also silently
  drop if they happened to share the `(idRange, idText)` key with a previously
  reported `UndefinedName`.

Only `UndefinedName` should mark the cache key; other recoverable failures
must still flow through the diagnostics sink so that constraint, IWSAM, and
type-provider diagnostics surface correctly on Phase 1F and Phase 2A.
