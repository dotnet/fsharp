# Hot reload: emitting added members (Phase B1b)

This note records the delta format decisions behind emitting ADDED members
(static fields, methods, parameters, properties, events) in F# hot reload
deltas, the C# reference deltas they were validated against, and the runtime
(CoreCLR) behavior that forced the EncLog shape used today.

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

Known divergences from the C# reference (deliberate this slice):

- Custom attributes on the added TypeDef are not emitted (C# adds
  `[CompilerGenerated]`). F# closure classes function without them; CA
  emission for added types can reuse the existing CustomAttribute row path
  later.
- `Extends` pointing at a generic instantiation (`FSharpFunc`2<A,B>` =
  TypeSpec) relies on TypeSpec token passthrough: the fresh row id must
  already exist in the baseline (true whenever the same instantiation occurs
  anywhere in the baseline). A genuinely NEW instantiation needs TypeSpec
  rows in the delta writer — not supported yet; the emitter fails closed
  with a rebuild-required message.
- Added GENERIC closure classes (closures over generic methods) need
  GenericParam rows — not supported yet; the emitter fails closed.

## Known gaps / later slices

- Instance field additions remain rejected (Phase B2); classification keeps
  them rude, and the emitter fails closed with a static-fields-only message.
- Custom attributes on added fields/properties (e.g. `DebuggerBrowsable` on
  backing fields, `CompilationMapping` on the module-value property) are not
  emitted into the delta yet; the members function without them.
- mdv renders `<bad metadata>` after every non-empty member-list range in EnC
  generations by convention (member lists are associated via EncLog); this is
  a rendering artifact, not delta corruption — Roslyn deltas render the same.
