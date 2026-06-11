# Hot Reload Runtime Capability Negotiation

F# hot reload negotiates *runtime edit-and-continue capabilities* the same way Roslyn does:
the host (for example `dotnet-watch`) collects the capability strings reported by the target
runtime (`MetadataUpdater.GetCapabilities()`) and passes them to the compiler when a hot
reload session starts. Edit classification then distinguishes between:

- edits that are **unsupported by F# hot reload** (always rude: virtual-method insertion,
  signature changes, type-layout changes, ...), and
- edits that are **valid but cannot be applied by the connected runtime**
  (`RudeEditKind.NotSupportedByRuntime`, diagnostic id `FSHRDL016`, message names the
  missing capability).

This mirrors Roslyn's `RudeEditKind.NotSupportedByRuntime` distinction.

## The model

`src/Compiler/Utilities/EditAndContinueCapabilities.fs` (namespace
`FSharp.Compiler.EditAndContinue`):

- `EditAndContinueCapability` — internal DU with one case per runtime capability word:
  `Baseline`, `AddMethodToExistingType`, `AddStaticFieldToExistingType`,
  `AddInstanceFieldToExistingType`, `NewTypeDefinition`, `ChangeCustomAttributes`,
  `UpdateParameters`, `GenericAddMethodToExistingType`, `GenericUpdateMethod`,
  `GenericAddFieldToExistingType`, `AddExplicitInterfaceImplementation`, `AddFieldRva`.
  `Name` returns the exact runtime/Roslyn capability string.
- `EditAndContinueCapabilities` — immutable wrapper over a `Set<EditAndContinueCapability>`
  with `Supports`, `IsBaselineOnly`, `CapabilityNames`, `BaselineOnly` and `Parse`.

Capability strings cross the compiler boundary exactly once, in
`EditAndContinueCapabilities.Parse`. Everything downstream consults the typed model, so
there are no stringly-typed capability checks in classification or emission.

### Parser semantics (Roslyn parity)

Mirrors `EditAndContinueCapabilitiesParser.Parse` in
`roslyn/src/Features/Core/Portable/EditAndContinue/EditAndContinueCapabilities.cs`:

- each input string is one capability name; matching is exact and case-sensitive;
- unknown capability words are **ignored** (forward compatibility with newer runtimes);
- the aggregate word `AddDefinitionToExistingType` expands to
  `AddMethodToExistingType + AddStaticFieldToExistingType + AddInstanceFieldToExistingType`.

One deliberate deviation: an F# session always carries at least `Baseline`. `Parse` of an
empty (or all-unknown) input yields `BaselineOnly`, and `Baseline` is implied whenever any
other capability is recognized. A capability-less session is unrepresentable; method-body
updates are never gated behind a missing `Baseline` word.

## Session plumbing

- `FSharpChecker.StartHotReloadSession(projectOptions | projectSnapshot, ?userOpName,
  ?capabilities: string seq)` (`src/Compiler/Service/service.fs[i]`) — string-based at the
  public boundary, like Roslyn's `WatchHotReloadService(Solution, ImmutableArray<string>)`.
  When omitted the session defaults to `EditAndContinueCapabilities.BaselineOnly`
  (Roslyn-conservative: assume the runtime can only update method bodies).
- The parsed capabilities are stored on the session
  (`HotReloadSession.Capabilities` in `src/Compiler/HotReload/HotReloadState.fs`) by
  `FSharpEditAndContinueLanguageService.StartSession(..., ?capabilities)`.
- `EmitDeltaForCompilation`/`EmitHotReloadDelta` pass `session.Capabilities` into
  `computeSymbolChanges` → `TypedTreeDiff.diffImplementationFile`.

The `fsc --enable:hotreloaddeltas` emit hook (`Driver/HotReloadEmitHook.fs`) does not
negotiate capabilities and therefore runs baseline-only.

## Classification gating

`src/Compiler/TypedTree/TypedTreeDiff.fs` consults a single seam:

```fsharp
[<RequireQualifiedAccess>]
type AdditionKind =
    | Method
    | InstanceField
    | StaticField

let capabilityForAddition: AdditionKind -> EditAndContinueCapability
```

The order of checks for an added declaration follows Roslyn
(`AbstractEditAndContinueAnalyzer` / `CSharpEditAndContinueAnalyzer`, see uses of
`EditAndContinueCapabilities.` there): edit-kind rude edits first (virtual, constructor,
operator, explicit interface, interface member, field), then the runtime-capability check.
A method addition that passes the rude-edit checks requires
`capabilityForAddition AdditionKind.Method = AddMethodToExistingType`; if the session does
not support it, the diff reports `RudeEditKind.NotSupportedByRuntime` with the
`hotReloadAdditionNotSupportedByRuntime` FSComp message naming the capability.

## Phase B and beyond

Field additions are capability-gated as of Phase B1b/B2:

- Module-level values (static backing field + accessors) require
  `AddStaticFieldToExistingType` + `AddMethodToExistingType` (B1b).
- Instance fields on CLASSES (`let mutable` / `[<DefaultValue>] val mutable` /
  auto-property backing fields) require `AddInstanceFieldToExistingType`,
  checked by the entity-level field diff in `compareEntities` (a pure field
  addition no longer reports `TypeLayoutChange`); per-field staticness selects
  the static or instance capability (B2).
- STRUCT (and record/union/enum) field additions stay `TypeLayoutChange`
  permanently — the runtime cannot re-layout value types (C# identical).

Generic edits are capability-gated as of Phase E (Roslyn parity:
`AbstractEditAndContinueAnalyzer.InGenericContext`, which walks the symbol chain
for a generic method arity or a generic containing type):

- BODY EDITS of a member in a generic context (the compiled method has its own
  generic parameters — including auto-generalized module functions — or is
  declared in a generic type) require `GenericUpdateMethod`; without it the
  diff reports `RudeEditKind.NotSupportedByRuntime` with the
  `hotReloadGenericUpdateNotSupportedByRuntime` FSComp message (FSHRDL016)
  naming the capability. (Roslyn reports
  `RudeEditKind.UpdatingGenericNotSupportedByRuntime`.)
- METHOD ADDITIONS in a generic context additionally require
  `GenericAddMethodToExistingType` on top of `AddMethodToExistingType`
  (Roslyn `GetRequiredAddMethodCapabilities`).
- FIELD ADDITIONS in a generic context (binding-level and the entity-level
  field diff in `compareEntities`) additionally require
  `GenericAddFieldToExistingType` (Roslyn `GetRequiredAddFieldCapabilities`).

`InGenericContext` is computed on `BindingSnapshot` from the compiled-form
typar split (`GetValReprTypeInCompiledForm`, method typars vs enclosing
typars); erased (measure) typars do not count, so measure-only generic types
stay gated like non-generic IL. `EntitySnapshot.IsGeneric` mirrors this for
the entity-level field diff.

Attribute edits are capability-gated as of Phase F: changing the custom
attributes of an EXISTING member (add/remove/argument change, detected via
`BindingSnapshot.AttributesDigest`) requires `ChangeCustomAttributes`;
without it the diff reports `RudeEditKind.NotSupportedByRuntime` with the
`hotReloadAttributeChangeNotSupportedByRuntime` FSComp message (FSHRDL016)
naming the capability (Roslyn:
`RudeEditKind.ChangingAttributesNotSupportedByRuntime`). With the capability
the edit is an ordinary member update; members whose attribute rows are
Property/Event-parented (accessors, module values) fail closed — see
docs/hot-reload-member-additions.md.

Parameter renames are capability-gated as of Phase F: a matched binding whose
compiled parameter NAMES differ (`BindingSnapshot.ParameterNames` — curried/
tupled groups flattened, the implicit `this` argument excluded; renaming the
self identifier is not a parameter rename) requires `UpdateParameters`;
without it the diff reports `RudeEditKind.NotSupportedByRuntime` with the
`hotReloadParameterRenameNotSupportedByRuntime` FSComp message (FSHRDL016)
naming the capability (Roslyn: `RudeEditKind.RenamingNotSupportedByRuntime`).
With the capability the member re-emits as an ordinary update whose Param
rows carry the new names. Parameter TYPE changes remain `SignatureChange`
rude edits.

`NewTypeDefinition` gates added-lambda closure classes (Phase C4) and, as of
Phase F, USER-DEFINED type additions: adding a class/record/union/struct
classifies as a `SemanticEditKind.Insert` entity edit when the capability is
granted (`RudeEditKind.NotSupportedByRuntime` naming it otherwise); other
representations (interfaces, enums, delegates) stay `DeclarationAdded` rude
with precise messages. The new type's member bindings ride along with the
entity edit and are exempt from the existing-type member-addition gates.

## Roslyn references

- `roslyn/src/Features/Core/Portable/EditAndContinue/EditAndContinueCapabilities.cs` —
  flags + parser (capability set mirrored here).
- `roslyn/src/Features/Core/Portable/EditAndContinue/AbstractEditAndContinueAnalyzer.cs` —
  capability-gated classification and `RudeEditKind.NotSupportedByRuntime` reporting.
- `roslyn/src/Features/CSharp/Portable/EditAndContinue/CSharpEditAndContinueAnalyzer.cs` —
  language-specific required-capability computation.
