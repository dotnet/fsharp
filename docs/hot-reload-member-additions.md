# Hot reload: emitting added members (Phases B1b, B2, B3)

This note records the delta format decisions behind emitting ADDED members
(static fields, instance fields, methods, parameters, properties, events) in
F# hot reload deltas, the C# reference deltas they were validated against, and
the runtime (CoreCLR) behavior that forced the EncLog shape used today.

## C# reference EncLog (hotreload-delta-gen)

Scenario: `public static int AddedStatic = 42;` added to a class whose static
constructor already exists (`csharp_delta_test`, SimpleLib_v1.cs). The
generation-1 delta produced by Roslyn (read with mdv) contains:

```
EnC Log:
1: AssemblyRef  0x23000002  Default      (new AssemblyRef row)
2: TypeRef      0x0100000f  Default      (new TypeRef row)
3: TypeDef      0x02000002  AddField     <- PARENT of the added field
4: Field        0x04000002  Default      <- the added Field row
5: MethodDef    0x06000003  Default      (updated .cctor body)

EnC Map: TypeRef 0x0100000f, Field 0x04000002, MethodDef 0x06000003,
         AssemblyRef 0x23000002   (token-sorted; NO entry for the
                                   AddField parent TypeDef)
```

The PDB delta's EncMap contains a single `MethodDebugInformation 0x31000003`
row for the updated constructor plus its sequence points.

Field signature blob: `06-08` (FieldSig per ECMA-335 II.23.2.4: FIELD, int32).

## EncLog shape for added members (CLR requirement, not just parity)

CoreCLR's EnC applier (`CMiniMdRW::ApplyDelta`, metamodelenc.cpp) interprets
an `Add*` operation as: *the token in THIS record is the PARENT; the NEXT
record defines the new member row*:

| Operation     | Parent token in the Add* record | Following record  |
|---------------|---------------------------------|-------------------|
| AddMethod (1) | TypeDef                         | Method, Default   |
| AddField (2)  | TypeDef                         | Field, Default    |
| AddParameter (3) | MethodDef                    | Param, Default    |
| AddProperty (4)  | PropertyMap                  | Property, Default |
| AddEvent (5)     | EventMap                     | Event, Default    |

Newly created PropertyMap/EventMap rows are logged as plain `Default` entries
BEFORE the `AddProperty`/`AddEvent` entries that reference them; the parent
map row id is used even when the map already exists in the baseline.
MethodSemantics rows are plain `Default` entries. Only the added member row
(never the parent entry) appears in EncMap.

Historically the F# writer put the `Add*` operation on the member row itself
(e.g. `(Method, rowId, AddMethod)`). That shape can NOT be applied by the
runtime: `ApplyDelta` treats the member rid as a parent rid, corrupting member
lists, and never applies the member row contents. It went unnoticed because no
test ever runtime-applied an added member. Phase B1b switched ALL member kinds
to the Roslyn/CLR parent+pair shape; there is no remaining method-path
discrepancy.

The numeric operation codes were also corrected to the CLR/SRM values
(`AddParameter=3, AddProperty=4, AddEvent=5`; they were previously 4/5/6).

## Heap offsets for added members

- The baseline `#Strings` size must be SRM's *trimmed* size (alignment padding
  reduced to a single terminating zero, `StringHeap.TrimEnd`), which is what
  Roslyn's `EmitBaseline` uses and what EnC heap aggregation assumes.
  `ILBaselineReader.metadataSnapshotFromBytes` now trims; using the raw
  (padded) stream size shifted every delta-heap string reference by the
  padding bytes.
- Added member rows must write their name/signature into the DELTA heaps
  (offset `None` in the row infos). Offsets captured from the fresh in-memory
  compile's heaps are meaningless against the baseline+delta heap layout.
- Signature blobs entering the delta blob heap (method, field, property) flow
  through `remapSignatureBlobWith` so embedded TypeDefOrRef coded indexes are
  remapped from the fresh compile to baseline rows.

## Param rows

Return-parameter rows are never synthesized (Roslyn parity). The fresh compile
emits Param rows only for real parameters, ordered by sequence number. A
synthesized seq-0 row appended after real seq-1+ rows is out of sequence,
which forces the CLR's `FixParamSequence` onto the indirect ParamPtr-table
path and makes `MetadataUpdater.ApplyUpdate` reject the delta (observed with
added module-value accessors).

The `ParamList` column of ADDED method rows is kept monotone: a parameterless
added method points at the NEXT param row rather than 0 (the CLR suppresses
this column for deltas, but mdv/ECMA readers compute ranges from it).

## Module-level value additions (F# lowering)

`let mutable newCounter = 41` added to `module Sample.Library` lowers to:

- static fields `newCounter@<line>` and `init@` on the startup-code class
  `<StartupCode$asm>.$Sample.Library` (NOT the module type),
- static accessors `get_newCounter`/`set_newCounter` plus a `newCounter`
  property on the module type,
- a `.cctor` on the startup-code class that runs the initializer.

When the baseline already contains the startup `.cctor` (the module already
had values), the diff cannot pair it (it is not a typed-tree binding);
`DeltaBuilder.mapSymbolChangesToDelta` resolves it by name
(`<StartupCode$...>.$<module-path>::.cctor`) and adds it as an updated method.
When the baseline has no startup `.cctor` yet, the fresh compile introduces it
and the emitter discovers it as an added method.

### Initialization semantics (validated at runtime)

Matches C# EnC semantics, verified with both the Roslyn reference delta and
the F# runtime test:

- If the type holding the backing field has NOT been type-initialized yet,
  the (added/updated) static constructor runs lazily on first access and the
  value reads its initializer (the F# module-value test observes `41`; the
  C# reference observes `42`). This is the common case for F# module values
  whose startup class had no prior static state.
- If the type was already initialized, the constructor does not re-run and
  the added field reads `default(T)`.

## Instance field additions (Phase B2)

### C# reference EncLog (Roslyn EmitDifference, csharp_enc_reference `members` scenario)

Scenario `field_add`: `public int NewInstanceField = 42;` added to an existing
class with an explicit constructor (edits: Insert(field) + Update(.ctor)).
Generation-1 delta (mdv: `csharp_enc_reference/reference_mdv_field_add.txt`):

```
EnC Log:
1: AssemblyRef  0x23000002  Default
2: MemberRef    0x0a000006  Default      (Object::.ctor for the re-emitted ctor body)
3: TypeRef      0x01000007  Default      (Object)
4: TypeDef      0x02000002  AddField     <- PARENT of the added field
5: Field        0x04000002  Default      <- the added instance Field row
6: MethodDef    0x06000001  Default      (updated .ctor body, runs the initializer)

EnC Map: TypeRef add, Field add, MethodDef 1 UPDATE, MemberRef add,
         AssemblyRef add (token-sorted; NO entry for the AddField parent)
```

Identical `(TypeDef, AddField) + (Field, Default)` pairing as the B1b static
case; the only structural difference from a static field add is the paired
constructor update instead of a `.cctor` update.

### F# lowering and classification

- `type C() = let mutable x = 41` folds the initializer into the primary
  constructor: the typed-tree diff pairs the `.ctor` binding (and any member
  body reading the field) as plain MethodBody updates, and the field itself
  surfaces only through the ENTITY representation (`fsobjmodel_rfields`).
  Constructor pairing therefore needs no by-name resolution (unlike the B1b
  startup `.cctor`): the ctor IS a typed-tree binding. The diff's runtime
  return-type identity for constructors is `void` (the IL truth), not the
  constructed type — required for `.ctor` MethodBody edits to resolve against
  baseline method tokens.
- `[<DefaultValue>] val mutable` produces NO binding or body change at all:
  the entity-level field diff emits a `SemanticEditKind.TypeDefinition` edit
  (symbol path mirrors the IL type name) so delta emission runs, and the
  generation-1 delta is a pure Field-row append with zero method updates. The
  metadata writer's empty-delta short-circuit keys on row payload (method
  updates OR added type/field/property/event rows), not method updates alone.
- Entity classification (`compareEntities`) recognizes a PURE field addition
  (non-field representation unchanged, baseline fields preserved verbatim,
  only new fields) on a CLASS (`TFSharpClass`) and gates it on
  `AddInstanceFieldToExistingType` / `AddStaticFieldToExistingType` per added
  field; a missing capability reports `RudeEditKind.NotSupportedByRuntime`
  naming it. STRUCT (and record/union/enum) field additions remain
  `TypeLayoutChange` permanently — the runtime cannot re-layout value types
  (C# identical). The emitter enforces the same restriction fail-closed
  (`typeDef.IsStructOrEnum`).

### Initialization semantics (validated at runtime)

C# EnC parity, asserted by the runtime tests:

- Instances constructed BEFORE the update read `default(T)` for the added
  field (their constructor never ran the new initializer); their existing
  state is preserved across generations.
- Instances constructed AFTER the update run the UPDATED constructor and see
  the initializer value.
- Multi-generation: a second added field chains through the updated baseline
  (gen-1 field tokens resolve; only the new Field row is appended).

## Property and event additions (Phase B3)

### C# reference EncLog (Roslyn EmitDifference, csharp_enc_reference `members` scenario)

Scenario `prop_add`: `public int NewProp { get; set; }` added to a class that
already has one auto-property (single edit: Insert(property); Roslyn
synthesizes the accessors and backing field). Generation-1 delta (mdv:
`csharp_enc_reference/reference_mdv_prop_add.txt`):

```
EnC Log:
 1: AssemblyRef     0x23000002  Default
 2: MemberRef       0x0a000008  Default      (CompilerGeneratedAttribute..ctor)
 3: MemberRef       0x0a000009  Default      (DebuggerBrowsableAttribute..ctor)
 4: TypeRef         0x0100000a  Default      (Object)
 5: TypeRef         0x0100000b  Default      (CompilerGeneratedAttribute)
 6: TypeRef         0x0100000c  Default      (DebuggerBrowsableState)
 7: TypeRef         0x0100000d  Default      (DebuggerBrowsableAttribute)
 8: TypeDef         0x02000002  AddField     <- backing field parent
 9: Field           0x04000002  Default      <- '<NewProp>k__BackingField'
 a: TypeDef         0x02000002  AddMethod
 b: MethodDef       0x06000005  Default      (get_NewProp)
 c: TypeDef         0x02000002  AddMethod
 d: MethodDef       0x06000006  Default      (set_NewProp)
 e: PropertyMap     0x15000001  AddProperty  <- parent: the EXISTING baseline map row
 f: Property        0x17000002  Default      <- the added Property row
10: MethodDef       0x06000006  AddParameter
11: Param           0x08000002  Default      ('value')
12-15: CustomAttribute x4        Default     ([CompilerGenerated]/[DebuggerBrowsable]
                                              on the backing field + accessors)
16: MethodSemantics 0x18000003  Default      (getter binding)
17: MethodSemantics 0x18000004  Default      (setter binding)

Property row Get/Set columns render nil (EnC): accessors are linked through
the MethodSemantics rows, exactly like FieldList/MethodList for added types.
```

### F# emission shape

- **MethodSemantics rows are derived from the fresh compile's accessor
  relationships** (`PropertyDefinition.GetAccessors()` /
  `EventDefinition.GetAccessors()` on the fresh metadata, accessor method
  tokens remapped to baseline/delta MethodDef rows) — Roslyn parity:
  `DeltaMetadataWriter` emits semantics from the symbol model, not from the
  edit list. Every ADDED Property/Event row therefore carries its
  Getter/Setter/Adder/Remover/Raiser bindings even when the accessors are
  compiler-synthesized (module values, `[<CLIEvent>]` members get them too —
  previously the module-value property row shipped without semantics rows).
  A Property/Event row whose accessors cannot be bound fails closed
  (`HotReloadUnsupportedEditException`): such a row is corrupt metadata.
- The delta builder does NOT resolve ADDED accessors against the baseline
  (there is nothing to resolve): added accessor methods are discovered by the
  emitter walking the fresh module, like any added method. `UpdatedAccessors`
  remains the body-update path for EXISTING accessors.
- Events bind BOTH adder and remover (and raiser when present); the
  `EventType` column of ADDED Event rows is remapped from the fresh compile
  to baseline/delta rows through the content-validated reference remapper
  (appending a TypeRef row when no baseline row matches — Roslyn likewise
  re-emits TypeRefs used by the delta).
- New PropertyMap/EventMap rows are logged as plain `Default` entries before
  the `AddProperty`/`AddEvent` entries referencing them; when the map row
  already exists (baseline or chained from an earlier generation) the parent
  entry reuses it and no map row is emitted. Validated across generations at
  runtime (gen-1 adds the map row, gen-2 reuses it from the chained baseline).
- An F# auto-property (`member val`) composes the B2 field machinery with the
  accessor/property machinery: AddField pair (backing field, ctor-paired
  initializer) + two AddMethod pairs + AddProperty pair + two MethodSemantics
  rows — the recorded C# template minus the custom-attribute rows (CA
  emission for added members is still deferred; see Known gaps).
- A `[<CLIEvent>]` member lowers to a backing `Event<_,_>` instance field
  (ctor-paired initializer) + add_/remove_ accessors + the Event row; the
  typed-tree `get_<Name>` PropertyGet symbol has no IL counterpart and is
  ignored. Validated at runtime: the added event is subscribable through
  `Type.GetEvent` reflection and fires.

Runtime validation: added properties are readable/writable through ordinary
`Type.GetProperty` reflection on LIVE instances (semantics rows wired the
accessors); auto-property state follows C# EnC semantics (existing instances
read `default(T)`, new instances run the updated ctor and see initializers).

### mdv parity notes (B2/B3 consolidation)

- A three-generation chain mixing method, instance-field, and auto-property
  additions aggregates cleanly under mdv (no `EnCMap`/table errors), and mdv
  resolves the gen-3 Property row's accessors through the emitted
  MethodSemantics rows.
- Next-generation table-row chaining counts only APPENDED rows (EncMap
  entries past the baseline row counts). The delta's physical tables also
  carry re-emitted rows for UPDATED definitions; counting those advanced the
  row cursors past a gap, so a later generation's added rows produced an
  EncMap readers reject ("EnCMap table not sorted or has missing records")
  and members the runtime could not link. Exposed by the mixed-additions
  chain (gen 1 update+add in the same table), latent before B2/B3.
- Divergences from the recorded C# templates, all deliberate this slice:
  EncLog group ordering is the established F# one (see the C4 notes) rather
  than Roslyn's strict by-table interleaving; F# auto-property backing fields
  keep their F# names (`NewProp@`) instead of C#'s `<NewProp>k__BackingField`.
  (Custom-attribute rows on added members were deferred here and are emitted
  as of Phase F — see "Custom attribute rows" below.)

## New type definitions (Phase C4: added closure classes)

### C# reference EncLog (Roslyn EmitDifference, csharp_enc_reference harness)

Scenario: `Compute(int x)` (no lambdas in the baseline) gains
`System.Func<int,int> f = y => y + x;` — Roslyn synthesizes a NEW display
class `<>c__DisplayClass0#1_0#1` (nested in `SimpleLib`, generation-suffixed
`#1` names) in the generation-1 delta. The hotreload-utils workspace tool
could not run on this SDK (net11-only toolchain), so the reference deltas
were produced with a compiler-level `Compilation.EmitDifference` harness
(`hot_reload_poc/src/csharp_enc_reference`, Roslyn 5.9.0-1.26302.3) — the
IDE/dotnet-watch pipeline ends in the same API, so the delta shape is
identical. Full mdv output: `csharp_enc_reference/reference_mdv_added_lambda.txt`.

Baseline tables: 2 TypeDefs, 2 MethodDefs, 1 Param, 0 Fields, 5 MemberRefs,
6 TypeRefs, 4 CustomAttributes, 0 TypeSpecs, 1 StandAloneSig, 1 AssemblyRef.

```
EnC Log (generation 1):
 1: AssemblyRef   0x23000002  Default      (new AssemblyRef row)
 2: MemberRef     0x0a000006  Default      (display-class Object::.ctor ref)
 3: MemberRef     0x0a000007  Default      (Func`2<int,int>..ctor, parent TypeSpec)
 4: MemberRef     0x0a000008  Default      (Func`2<int,int>.Invoke, parent TypeSpec)
 5: MemberRef     0x0a000009  Default      (Object::.ctor TypeRef ref)
 6: TypeRef       0x01000007  Default      (Object)
 7: TypeRef       0x01000008  Default      (Func`2)
 8: TypeRef       0x01000009  Default      (CompilerGeneratedAttribute)
 9: TypeSpec      0x1b000001  Default      (Func`2<int32,int32>)
 a: StandAloneSig 0x11000002  Default      (Compute's new locals signature)
 b: TypeDef       0x02000003  Default      <- the NEW TypeDef row, PLAIN row add
 c: TypeDef       0x02000003  AddField     <- parent: the NEW TypeDef
 d: Field         0x04000001  Default      <- capture field 'x'
 e: MethodDef     0x06000001  Default      (updated Compute body)
 f: TypeDef       0x02000003  AddMethod
10: MethodDef     0x06000003  Default      (display class .ctor)
11: TypeDef       0x02000003  AddMethod
12: MethodDef     0x06000004  Default      (lambda method <Compute>b__0#1)
13: Param         0x08000001  Default      (updated Compute's param row re-emitted)
14: MethodDef     0x06000004  AddParameter
15: Param         0x08000002  Default      (lambda param 'y')
16: CustomAttribute 0x0c000005 Default     ([CompilerGenerated] on the new TypeDef)
17: NestedClass   0x29000001  Default      <- trails the log

EnC Map: token-sorted; contains TypeDef 0x02000003 (add), Field 0x04000001
(add), MethodDef 1 (update) / 3,4 (add), Param 1 (update) / 2 (add),
NestedClass 0x29000001 (add), the TypeRef/MemberRef/TypeSpec/StandAloneSig/
AssemblyRef adds, CustomAttribute 0x0c000005 (add). NO entry for any Add*
parent record.
```

Key facts the F# writer mirrors:

- The new TypeDef row is logged as a **plain Default entry** (no special
  "create type" operation) and is applied via ApplyTableDelta like any other
  appended row. It MUST precede every AddField/AddMethod entry that names it
  as the parent.
- The TypeDef row's FieldList/MethodList columns are written as **0**
  (Roslyn `DeltaMetadataWriter.GetFirstFieldDefinitionHandle/
  GetFirstMethodDefinitionHandle` return `default` in EnC deltas — members
  are linked through the EncLog pairs; mdv renders the columns "n/a (EnC)").
- The NestedClass row is a plain Default entry trailing the log; it appears
  in EncMap.
- Generation 2 (body edit of the lambda added in generation 1) emits NO
  TypeDef/Field/NestedClass rows: MethodDef 0x06000001 (update) and
  0x06000004 (update of the gen-1 added row) only — the added closure class
  is simply part of the chained baseline.

### F# emission shape (and divergences)

`FSharpDeltaMetadataWriter.emitWithTypeDefinitions` accepts
`TypeDefinitionRowInfo`/`NestedClassRowInfo` lists. EncLog group order is the
established F# one (Module, new TypeDef rows, AddField pairs, method entries,
parameter pairs, reference tables, property/event groups, NestedClass last).
This differs from Roslyn's strict by-table interleaving (e.g. Roslyn logs the
updated parent MethodDef *between* the AddField pair and the AddMethod pairs)
— the CLR applier only requires that parents exist before Add* entries
reference them and that each Add* pair stays adjacent, which both orders
satisfy; the F# order was already validated by ApplyUpdate for B1b member
additions and is revalidated by the C4 runtime tests.

The emitter (`IlxDeltaEmitter`) detects added types by the C3 allocator's
generation-suffix marker (`{base}@hotreload#g{N}_o{i}`, see
`ClosureNameAllocator.isGenerationSuffixedClosureName`): a fresh-compile type
with that name and no baseline TypeDef token allocates the next delta TypeDef
row; its fields/methods register as added members parented to the new row
(including INSTANCE fields — the existing-type instance-field restriction
does not apply to a type introduced by the same delta). The row's Extends is
remapped from the fresh compile (TypeRef via the reference remapper, TypeDef
via the definition remapper). The new type token chains into the
next-generation baseline `TypeTokens` under its full name, so later body
edits of the added lambda resolve in place. The machinery is general; only
the detection is closure-scoped (widening it to user types is Phase F).

Validated end-to-end at runtime
(`ApplyUpdate succeeds for added lambda creating a new closure class`): a
2-lambda member gains a third (capture-free) lambda → the generation-1 delta
carries the new TypeDef + 3 AddMethod pairs (.ctor/Invoke/.cctor — F#'s
capture-free closures add a static singleton initializer, one more method
than C#'s display class), AddParameter pair for Invoke, NestedClass row →
MetadataUpdater.ApplyUpdate accepts → the new lambda executes. Generation 2
body-edits the ADDED lambda: method updates only, no new TypeDef/Field rows
(the closure class chained into the baseline), exactly like the C# gen-2
reference.

### Content-validated MemberRef/TypeSpec token passthrough

Landing the runtime test exposed a pre-existing positional fragility: the
emitter passed fresh-compile MemberRef/TypeSpec tokens through verbatim when
the row id fit inside the baseline table. An added lambda changes the ORDER
OF FIRST USE of references (e.g. `ListModule.Map` now precedes `Filter`), so
the fresh rows no longer line up positionally — the delta's MethodSpec rows
silently bound `Filter<int32,int32>`/`Map<int32>` (swapped genericity) and
the applied body threw BadImageFormatException.

The fix mirrors the TypeReferenceTokens approach: the baseline now snapshots
MemberRef row contents (name, decoded parent token, signature blob) and
TypeSpec signature blobs from the assembly bytes
(`FSharpEmitBaseline.MemberReferenceRows`/`TypeSpecSignatures`, read by the
SRM-free ILBaselineReader, which also gained the previously missing
InterfaceImpl row size — assemblies WITH interface implementations had every
table offset past InterfaceImpl misread). The remapper then:

1. trusts a positional row id only when the fresh row's content (with parent
   and signature REMAPPED to baseline coordinates) matches the baseline row;
2. otherwise searches the baseline table for a unique content match;
3. otherwise appends a new MemberRef — or TypeSpec — row (always legal).

Delta-appended MemberRef and TypeSpec rows chain into the next generation's
`MemberReferenceRows`/`TypeSpecSignatures` so later compiles can
validate/reuse them. Baselines without byte-derived snapshots (legacy
construction paths) keep the historic positional behavior; an empty TypeSpec
snapshot with a zero baseline row count is treated as validated (every fresh
TypeSpec is genuinely new and is appended).

### TypeSpec row emission (new generic instantiations)

An added lambda whose closure class extends a generic instantiation with no
matching baseline TypeSpec row (the common case: the first
`FSharpFunc<A,B>` of that shape in the assembly, e.g. a `List.filter`
predicate added to a baseline that only has `int -> int` closures) appends a
TypeSpec row to the delta, mirroring the recorded C# reference template
above (gen-1 entry `9: TypeSpec 0x1b000001 Default`):

- The row is a single #Blob signature column (ECMA-335 II.22.39); the blob
  is taken from the fresh compile and remapped to baseline coordinates via
  `remapTypeSpecBlobWith` (inner TypeDefOrRef coded indexes through the
  entity remapper). TypeSpec coded indexes embedded in signature blobs
  (TypeDefOrRefOrSpec tag 2) route through the same content-validated remap.
- EncLog logs the row with the plain **Default** operation (applied via
  ApplyTableDelta, like TypeRef/MemberRef adds); the row id appears in the
  token-sorted EncMap.
- The appended row id and signature chain into the next-generation
  baseline's `TypeSpecSignatures`, so a later generation content-matches and
  reuses the row (a generation-2 lambda adding yet another new instantiation
  appends its TypeSpec row PAST the generation-1 row; validated by
  `ApplyUpdate succeeds for added lambdas with new generic instantiations
  across generations`).
- Methods added by an earlier generation and re-emitted by a later delta
  write their name/signature into the later delta's heaps (the baseline
  handle cache only covers the on-disk assembly); fresh-compile heap offsets
  were previously emitted for these rows and produced garbage references
  ("Bad binary signature") once the blobs no longer coincided with baseline
  content.

Generic closure CLASSES (closures over generic methods) get their
GenericParam rows as of Phase E (see "Generic edits" below); the historic
fail-closed gate there is lifted.

### Session wiring (watch flow)

`checker.StartHotReloadSession` rebuilds the baseline from the on-disk
assembly, which cannot carry the C3 closure-name tables (the EnC CDI blob
format has no name slots). The session-start path now carries
`EncClosureNames` over from the in-process capture session it replaces when
the module identity (MVID) matches — without this, every watch-flow delta
compile fell back to sequence-replay naming and added lambdas were
unmappable.

Known divergences from the C# reference (deliberate this slice):

- Custom attributes on the added TypeDef are not emitted (C# adds
  `[CompilerGenerated]`). F# closure classes function without them; CA
  emission for added types can reuse the existing CustomAttribute row path
  later.
- `Extends` pointing at a generic instantiation (`FSharpFunc`2<A,B>` =
  TypeSpec) resolves through the content-validated TypeSpec remap: it reuses
  a matching baseline row, or appends a new TypeSpec row to the delta for a
  genuinely NEW instantiation (see "TypeSpec row emission" above).
- Added GENERIC closure classes (closures over generic methods) are
  supported as of Phase E: the new TypeDef row gets GenericParam rows (see
  "Generic edits" below). Constrained typars still fail closed.
- The AsyncStateMachineAttribute synthesis heuristic (nested
  `{method}@hotreload*` type) now additionally requires a `MoveNext` method,
  so ordinary closure classes sharing the naming no longer pick up a spurious
  attribute.

### Classification (TypedTreeDiff)

- `LambdaEdit.Added`-only sets (no capture-set changes) are allowed when the
  runtime advertises `NewTypeDefinition` + `AddMethodToExistingType`;
  otherwise `RudeEditKind.NotSupportedByRuntime` names the missing
  capability (C# parity).
- `LambdaEdit.Removed`-only sets are allowed at Baseline capabilities (C#
  parity: deleted lambda bodies just become unreachable; the baseline
  closure class stays in place, unused). Validated at runtime
  (`ApplyUpdate succeeds for removed lambda leaving baseline closure
  unused`): the delta is a plain set of method updates touching no TypeDef
  rows, and the new behavior takes effect on apply.
- `CaptureSetChanged` stays rude this slice (capture-field mapping is a
  later slice).

## Generic edits (Phase E)

### C# reference EncLog (Roslyn EmitDifference, csharp_enc_reference `generics` scenarios)

Four recorded scenarios (mdv outputs:
`csharp_enc_reference/reference_mdv_generic_{method_update,class_update,class_add,method_add}.txt`):

1. **Body edit of `T Identity<T>(T x)`** (`generic_method_update`): MethodDef
   1 UPDATE + Param 1 update + a new StandAloneSig (locals use MVAR `!!0`) +
   TypeRef/AssemblyRef adds. **NO GenericParam rows** — they are baseline rows
   and are never re-emitted for updates; the re-emitted MethodDef row's
   GenericParameters column renders nil.
2. **Body edit of `Container<T>.Get()`** (`generic_class_update`): MethodDef
   UPDATE only. The body reaches `this.Value` through a **MemberRef parented
   by the TypeSpec self-instantiation** `Container'1<!0>` (delta-appended
   MemberRef + TypeSpec rows). No GenericParam rows.
3. **Adding `T GetAgain()` to `Container<T>`** (`generic_class_add`): the
   ordinary `(TypeDef, AddMethod) + (MethodDef, Default)` pair; the added
   method's signature simply uses VAR (`20-00-13-00` = HASTHIS, 0 params,
   ret `!0`). **No GenericParam rows** for non-generic methods of generic
   types.
4. **Adding `T Identity<T>(T x)` to a class** (`generic_method_add`): the
   AddMethod and AddParameter pairs are followed by a **`GenericParam
   0x2a000001 Default`** EncLog entry (plain row add, operation 0); the row
   appears in EncMap as an add. GenericParam row: Number=0, Flags=0,
   Owner=the NEW MethodDef (TypeOrMethodDef coded index), Name='T'. The
   delta's #~ header keeps GenericParam in the sorted mask.

### Roslyn capability semantics mirrored in classification

`AbstractEditAndContinueAnalyzer`: body updates in a generic context
(`InGenericContext`: own arity > 0 or any containing type generic) require
`GenericUpdateMethod` (else `UpdatingGenericNotSupportedByRuntime`); method
additions in a generic context require `AddMethodToExistingType +
GenericAddMethodToExistingType`; field additions require the static/instance
field capability + `GenericAddFieldToExistingType`. See
`docs/hot-reload-capabilities.md` for the F# gating implementation.

### F# emission

- **Generic body updates needed no emitter change**: `MethodDefinitionKey`
  carries `GenericArity`, the II.23.2 signature walker copies VAR/MVAR
  elements verbatim through the blob remapper, and member-of-generic-type
  field access flows through the content-validated MemberRef/TypeSpec remap
  (the TypeSpec self-instantiation pattern). Pinned by runtime tests
  (`GenericEditTests`): generic module function and generic-class member
  body edits ApplyUpdate and observe the edit via reflection with two
  instantiations (int, string); a two-generation chain whose second edit
  introduces a brand-new generic instantiation (`List.replicate<'T>`,
  MethodSpec with an MVAR blob) also applies. A template test asserts a
  generic body-update delta carries ZERO GenericParam rows.
- **Added generic methods** emit one GenericParam row per type parameter
  (`GenericParamRowInfo`: Number/Flags/Owner/Name per ECMA-335 II.22.20),
  snapshotted from the fresh compile's SRM reader, owner = the new delta
  MethodDef row, row ids continuing from the chained baseline GenericParam
  row count, ordered by (owner coded index, number) to respect the table's
  sort key. Logged as plain Default entries after the parameter pairs;
  EncMap adds. Before Phase E the added MethodDef row shipped WITHOUT its
  GenericParam rows: ApplyUpdate accepted the delta but the method was
  corrupt (`MakeGenericMethod` threw NullReferenceException, member access
  BadImageFormatException) — there was no fail-closed gate.
- **Added methods on generic types** need no GenericParam rows (template 3);
  they worked once classification allowed them and are pinned by a runtime
  test (VAR signature + TypeSpec-parented field access).
- **Added generic closure classes** (an added lambda inside a generic member
  mentioning 'T): the C4 added-TypeDef path no longer fails closed on
  generic closures; the new TypeDef row carries GenericParam rows (owner
  tag TypeDef). Validated at runtime: a generic member with one baseline
  lambda gains a second 'T-typed lambda, ApplyUpdate succeeds, both
  instantiations observe the edit. (Members gaining their FIRST lambda still
  fail closed — the C4 occurrence mapping needs a baseline chain table for
  the member; that constraint is orthogonal to generics.)
- **Constrained typars on ADDED definitions emit GenericParamConstraint
  rows as of Phase F (sub-slice 5)**. C# reference template
  ('generic_constraint_add', reference_mdv_generic_constraint_add.txt):
  adding `void DisposeIt<T>(T x) where T : IDisposable` logs
  `GenericParamConstraint 0x2c000001 Default` immediately after the
  `GenericParam 0x2a000001 Default` entry; EncMap lists both as adds; the
  constraint row's Owner is the NEW GenericParam row and its Constraint the
  interface TypeRef. The F# writer mirrors this (Owner-sorted rows continuing
  from the chained baseline count, constraint types remapped through the
  TypeRef/TypeDef/TypeSpec remappers); validated at runtime
  (`ApplyUpdate succeeds for added generic function with constrained typar`:
  reflection reports the IDisposable constraint on the live type parameter
  and the constrained method executes). Flag-only constraints (`not struct`,
  `new()`) live in the Flags column as before; F#-only constraints
  (`equality`, `comparison`) have no IL encoding.
- dotnet-watch topology: disk-started sessions (baseline reconstructed from
  the on-disk dll + pdb after a full session reset) apply both a generic
  body edit and an added generic function (runtime tests).

### Honest scoping (stays rude / fail-closed)

- SRTP/`inline` changes: already `InlineChange` rude (inline bodies are
  statically expanded into callers the delta cannot reach).
- Constraint changes on EXISTING members: `SignatureChange` rude
  (typar-constraint digest comparison).
- Statically-resolved instantiation changes that alter lowered shapes
  surface through the existing lowered-shape classifiers (state machine /
  lambda shape) or the emitter's fail-closed gates with precise messages.

## Custom attribute rows (Phase F)

### Added members (sub-slice 1)

The recurring B1b/C4 gap is closed: members ADDED by a delta now carry their
fresh-compile CustomAttribute rows. The C# reference templates show the
pattern (`prop_add`: 4 CA rows — `[CompilerGenerated]`/`[DebuggerBrowsable]`
on the backing field and accessors; `added lambda`: `[CompilerGenerated]` on
the new TypeDef), recorded as plain Default EncLog entries / EncMap adds
appended past the baseline CustomAttribute row count.

F# emission (`IlxDeltaEmitter.buildCustomAttributeRows`):

- Added METHODS were already walked by the method-row loop; their CA value
  blobs now enter the DELTA blob heap (previously the fresh compile's heap
  offset was reused, which is garbage against the baseline+delta layout —
  reflection could not decode the attribute value).
- Added FIELDS / PROPERTIES / EVENTS / TYPES are walked from the added-token
  maps: each fresh-compile attribute row becomes a CustomAttribute row add
  whose parent is the new delta row (`HCA_Field`/`HCA_Property`/`HCA_Event`/
  `HCA_TypeDef`), with the constructor remapped through the content-validated
  MemberRef reuse/append path and the value blob written to the delta heap.
- All appended CA rows are ordered by the HasCustomAttribute parent coded
  index (ECMA-335 II.22.10 sort key; Roslyn DeltaMetadataWriter parity) and
  renumbered contiguously past the chained baseline row count.

What F#'s lowering actually decorates (validated against a fresh fsc
compile): module-value properties carry `[CompilationMapping(Value)]` and
their startup-class backing fields `[DebuggerBrowsable(Never)]` (+
`[CompilerGenerated]`/`[DebuggerNonUserCode]` on `init@`); auto-property
accessors carry `[CompilerGenerated]`/`[DebuggerNonUserCode]` (the property
row and the `Name@` backing field carry nothing — unlike C#'s
`k__BackingField`); user attributes (e.g. `[<Obsolete>]`) ride on the added
member like any other row.

Runtime evidence (component `AttributeEditTests`): an added module function
with `[<Obsolete("warn")>]` exposes the attribute (and its message — the
delta-heap value blob decodes) through reflection after ApplyUpdate; an added
module value's property reports `CompilationMapping(Value)`; added accessors
report `[CompilerGenerated]`; the template test pins Default-op CA EncLog
entries, parent-sorted physical rows, and Field-/Property-parented rows.

Known scoping: CA rows on added PARAM rows are not emitted (F#'s lowering
does not decorate parameters in the supported scenarios).

### Attribute changes on existing members (sub-slice 2)

C# reference templates (csharp_enc_reference `phasef` scenarios,
`reference_mdv_attr_{add,change,remove}.txt`):

- `attr_add` (`[Description("x")]` added to an existing method): MethodDef +
  Param row updates plus ONE appended CustomAttribute row (parent = the
  existing MethodDef token; EncMap add).
- `attr_change` (argument changes): the CustomAttribute row is UPDATED at its
  EXISTING row id (EncLog Default at that id; EncMap update).
- `attr_remove`: the row is UPDATED with all-nil columns — raw row bytes
  `00000000 03000000 00000000` (Parent = nil MethodDef, Constructor = nil
  MemberRef tag 3, Value = nil blob).

Classification (`TypedTreeDiff`): `BindingSnapshot.AttributesDigest` captures
the member's attributes structurally (attribute type compiled name,
positional/named argument digests, getter/setter routing, explicit targets;
order-sensitive). Previously attribute-only edits were INVISIBLE to the diff
(no digest covered them) — the delta applied without the attribute change. A
digest change on a matched binding now:

- without `ChangeCustomAttributes`: `RudeEditKind.NotSupportedByRuntime`
  naming the capability (FSHRDL016, `hotReloadAttributeChangeNotSupportedByRuntime`);
- with the capability, when the attribute rows are MethodDef-parented (plain
  members/constructors, module functions): an ordinary member update edit —
  emission pairs the rows (below);
- with the capability but Property/Event-parented rows (property/event
  accessors, module VALUES — F# routes their attrs to the Property row):
  fail closed (`RudeEditKind.Unsupported`, precise message) — the writer
  cannot update Property/Event-parented CA rows yet.

Emission (`buildCustomAttributeRows` + `FSharpEmitBaseline.CustomAttributeRows`):
the baseline now snapshots CustomAttribute row contents (decoded parent/ctor
tokens + value blob) from the assembly bytes (SRM-free ILBaselineReader,
which also fixed the simplified HasCustomAttribute coded-index width to the
full 22-table ECMA set), chained with delta-emitted rows per generation. For
each updated method the fresh compile's attributes pair IN ORDER against the
baseline rows of that parent:

- content-identical sets (remapped ctor token + value bytes) emit NOTHING —
  this also retires the historic behavior of appending duplicate CA rows on
  every body update of an attributed method;
- changed attributes UPDATE the baseline row in place;
- extra fresh attributes append rows (renumbered past the chained count,
  parent-coded-index sorted);
- extra baseline rows are ZEROED with the exact template encoding.

The synthesized AsyncStateMachineAttribute path composes: when the fresh
compile omits the attribute, pairing zeroes the stale baseline row and the
synthesis appends the row pointing at the renamed state machine type.

Runtime evidence (`AttributeEditTests`): attribute add/change/remove on an
existing module function ApplyUpdate and reflection observes exactly the new
attribute state (`Seq.exactlyOne` pins the absence of duplicated rows);
template tests pin the in-place row id and the zeroed columns; negative
tests pin the ChangeCustomAttributes gate and the Property-parented
fail-closed path. Legacy baselines without byte-derived CA snapshots keep
the historic append-only behavior.

### Parameter metadata updates (sub-slice 3)

C# reference template (`reference_mdv_param_rename.txt`): renaming a
parameter re-emits the MethodDef and Param rows as UPDATES at their existing
row ids (EncMap updates, no adds); the Param row's Name column carries the
NEW name in the delta string heap.

F#: `BindingSnapshot.ParameterNames` (see docs/hot-reload-capabilities.md for
the gating) classifies renames; emission resolves the baseline parameter NAME
alongside its heap offset (`ParameterDefinitionMetadataHandles.Name`, read
from the baseline #Strings heap) and reuses the baseline name offset only
when the fresh name matches — a differing name writes the fresh name into the
delta string heap, producing exactly the template's row-update shape.

Runtime evidence (`ParameterEditTests`): ApplyUpdate accepts the Param row
update and `MethodInfo.GetParameters()` observes the new name on the live
assembly — provided the parameter info was not reflected on BEFORE the
update (the runtime caches ParameterInfo per MethodInfo; a primed cache keeps
the old name — parameter names are debugger-facing metadata). Negative tests
pin the UpdateParameters gate and that self-identifier renames do not gate.

## User-defined new type definitions (Phase F sub-slice 4)

### C# reference EncLog (csharp_enc_reference 'new_class', reference_mdv_new_class.txt)

A new top-level class (field + ctor + method, implementing IDisposable) used
from an edited method emits in generation 1: the new TypeDef row as a plain
Default entry (top-level: EnclosingType nil, no NestedClass row), an AddField
pair, three AddMethod pairs with their AddParameter pairs, the updated using
method as MethodDef/Param row updates, and an **InterfaceImpl row as a plain
Default entry trailing the log** (EncMap add — the table needed new writer
support). Generation 2 body-edits a method of the added class: a plain
MethodDef update of the generation-1 row, no type/field/interface rows.

### Classification

`compareEntities` (TypedTreeDiff): an ADDED entity gates on
`NewTypeDefinition` (Roslyn parity; previously always `DeclarationAdded`
rude). Allowed representations: **class, record, union, struct**
(`EntitySnapshot.SupportsAddition`); interfaces (abstract slots have no IL
bodies), enums (values need Constant rows the writer cannot emit), delegates
and exotic representations stay `DeclarationAdded` rude with a precise
message (surfaced through FSHRDL004, whose diagnostic now appends the diff
detail). The allowed addition becomes a `SemanticEditKind.Insert` edit whose
symbol path mirrors the IL name; the new type's member bindings (including
ctors, explicit interface implementations and the SYNTHESIZED record/union
members that would otherwise hit the member-addition or lowered-shape gates)
are SKIPPED by the binding diff — they ride along with the single entity
Insert edit.

### Emission

- `DeltaBuilder` excludes added entities from baseline type-token resolution
  (no baseline row exists); the emitter receives their names via
  `FSharpSymbolChanges.addedEntitySymbols` and matches fresh-compile TypeDefs
  dot-normalized ('+' vs '.' nesting).
- `collectTypeMappings` allocates delta TypeDef rows for matched fresh types
  through the C4 added-TypeDef machinery (rows, AddField/AddMethod pairs,
  GenericParam rows from E, CustomAttribute rows from sub-slice 1, NestedClass
  rows for types declared inside modules). Types NESTED inside an added type
  (union case classes, Tags holders, DebugTypeProxy companions) are detected
  via their enclosing type's delta row (parents visit first).
- **InterfaceImpl rows** (new writer table): one per interface of an added
  type, Interface remapped through the TypeRef/TypeDef/TypeSpec remappers,
  rows sorted by (Class, Interface coded index), plain Default EncLog entries
  trailing the log, EncMap adds.
- **MethodImpl rows** (new writer table): F# classes implement interfaces
  explicitly (`interface X with`), so every implemented slot carries a
  MethodImpl row — C# never shows them for implicit impls. Body/Declaration
  remap through the MethodDef/MemberRef remappers; sorted by Class.
- PropertyMap/EventMap rows of added types parent the NEW delta TypeDef row
  (previously baseline-only lookup).
- Added methods without IL bodies (abstract/extern) fail closed precisely.

### Runtime evidence (`NewTypeDefinitionTests`)

- Added class implementing IDisposable, used from the edited function:
  ApplyUpdate succeeds, the new type is instantiable through reflection, the
  interface is assignable and callable; generation 2 body-edits the added
  class's member in place (C# gen-2 parity).
- Added record: synthesized accessors/comparers/equality work on the live
  type (structural Equals validated), IComparable assignable, the using
  function reads fields.
- Added union: case constructors, Tag/IsCircle accessors work; nested case
  classes flow as nested-in-added types.
- Template test pins: TypeDef Default entry precedes the AddMethod pairs,
  three AddMethod pairs, InterfaceImpl + MethodImpl Default entries and
  EncMap adds. Negative tests pin the NewTypeDefinition gate and the
  interface-addition fail-closed message.

### Honest scoping (stays rude / fail-closed)

- New MODULES (and types/values inside them) are not classified as type
  additions (module entities are not snapshotted); their member additions
  fail closed at emission against the missing module TypeDef.
- Added interfaces/enums/delegates: rude with precise messages (above).
- Attributes ON InterfaceImpl rows (F# allows them) are not emitted.
- DeclSecurity, ClassLayout (explicit struct layout), Constant rows
  (literals), FieldMarshal/ImplMap (interop) have no writer support; types
  needing them surface through the emitter's fail-closed gates.

## Known gaps / later slices

- Attribute changes on PARAMETERS (e.g. adding `[<Optional>]`) are not
  classified yet (the attribute digest covers member-level attributes only);
  the Param row's flags/attributes re-emit from the fresh compile when the
  member updates for other reasons.
- mdv renders `<bad metadata>` after every non-empty member-list range in EnC
  generations by convention (member lists are associated via EncLog); this is
  a rendering artifact, not delta corruption — Roslyn deltas render the same.
