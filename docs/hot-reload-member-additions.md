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

## Known gaps / later slices

- Instance field additions remain rejected (Phase B2); classification keeps
  them rude, and the emitter fails closed with a static-fields-only message.
- Custom attributes on added fields/properties (e.g. `DebuggerBrowsable` on
  backing fields, `CompilationMapping` on the module-value property) are not
  emitted into the delta yet; the members function without them.
- mdv renders `<bad metadata>` after every non-empty member-list range in EnC
  generations by convention (member lists are associated via EncLog); this is
  a rendering artifact, not delta corruption — Roslyn deltas render the same.
