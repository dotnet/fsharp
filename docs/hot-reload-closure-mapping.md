# F# Hot Reload: Closure & State-Machine Mapping Design (Phase C/D)

Status: C1 landed (2026-06-10) — lambda occurrence model and alignment in `TypedTreeDiff`.
C2 commit 1 landed (2026-06-10) — EnC CDI blob encoder/decoder (`EncMethodDebugInformation.fs`).
C2 commit 2 landed (2026-06-10) — baseline-time EnC Lambda and Closure Map CDI emission in the
portable PDB under `--enable:hotreloaddeltas`.
C2 commit 3 landed (2026-06-10) — baseline EnC CDI read into the hot reload session
(`FSharpEmitBaseline.EncMethodDebugInfos`) plus in-memory generation chaining; delta-PDB CDI
re-emission deferred (see "C2 baseline CDI read" below).
C3 allocator landed (2026-06-10) — occurrence-keyed closure name allocation model
(`ClosureNameAllocator.fs`) with the generation-suffixed name format; lowering wiring pending
(see "C3 occurrence-keyed closure name allocation" below for the wiring design and its blockers).
C3 wiring commit 1 landed (2026-06-10) — lambda root stamps on `LambdaOccurrence` plus
baseline stamp→name capture at the IlxGen closure call site, joined in the fsc emit path and
stored token-keyed as `FSharpEmitBaseline.EncClosureNames` (see "Lowering wiring" below).
C3 wiring commit 2 landed (2026-06-10) — occurrence-keyed closure naming in delta compiles:
the emit hook's codegen step runs the allocator before lowering and installs the
stamp→assigned-name table the closure call site consults first; refreshed tables chain into
the next-generation baseline alongside the refreshed EnC debug infos. C3 is complete; C4
(emitting the added members in deltas) is the next phase.
C4–C5 pending. Implementation notes are folded into the relevant sections below.
Source research: Roslyn main @ June 2026 (`ClosureConversion.cs`, `EncVariableSlotAllocator.cs`, `EditAndContinueMethodDebugInformation.cs`, `DefinitionMap.cs`, `AbstractEditAndContinueAnalyzer.ReportLambdaAndClosureRudeEdits`).

## Problem

F# rejects any member-body edit whose lowered shape changes (`LambdaShapeChange`, `StateMachineShapeChange`,
`QueryExpressionShapeChange`). Because idiomatic F# is closure/CE-heavy, this reduces hot reload to
string/arithmetic tweaks. Roslyn supports editing lambda-containing methods via stable synthesized-member
identity persisted across generations.

## How Roslyn does it (summary of the machinery we must match)

1. **Stable identity**: every lambda and closure scope gets a `DebugId(ordinal, generation)` at lowering
   time. Names embed the id (`<F>b__0#1_0#1`), so matching never depends on fragile name text.
2. **Persistence**: three Portable-PDB CustomDebugInformation blobs per method carry the mapping state:
   *EnC Local Slot Map*, *EnC Lambda and Closure Map*, *EnC State Machine State Map* (compressed-int
   formats documented in `EditAndContinueMethodDebugInformation`). Generation N+1 reads generation N's
   blobs via `EmitBaseline.DebugInformationProvider`.
3. **Slot allocator seam**: lowering consults `EncVariableSlotAllocator.TryGetPreviousLambda/Closure/
   HoistedLocalSlotIndex/StateMachineState`. Hit → reuse the previous id/slot/state; miss → allocate a
   fresh id suffixed with the current generation (new synthesized member, emitted via
   `AddMethodToExistingType` / `NewTypeDefinition` / `AddInstanceFieldToExistingType`).
4. **Compatibility checks**: matched closures validate parent id + capture set; matched lambdas validate
   closure ordinal. Incompatible → the previous member body is replaced with a throwing stub
   (`HotReloadException`, `DeletedMethodBody.GetIL`) and new members are synthesized fresh.
5. **What stays rude in C#** (our parity ceiling, not floor): renaming a captured variable, changing a
   captured variable's type or scope, changing lambda parameters/return/type-params (runtime rude edits),
   inserting an await/yield mid-sequence (`ChangingStateMachineShape`).

## F#-shaped design

F# differs from C# in three load-bearing ways, and the design leans into them instead of transplanting
Roslyn types verbatim:

| Roslyn concept | F# realization | Rationale |
|---|---|---|
| `DebugId(ordinal, generation)` | Same shape, new `LambdaDebugId` in CodeGen | Direct port; embed in synthesized names via the existing replayable name map (`CompilerGeneratedNameMapState` already gives deterministic `@hotreload` names — extend entries with `(ordinal, generation)`). |
| Syntax offsets + SyntaxMap | **Typed-tree occurrence alignment** (landed in C1) | F#'s diff is typed-tree based (no Roslyn-style SyntaxMap). Lambdas are identified by a per-member *occurrence path*: traversal index within the member body + parent-lambda chain + structural digest. Old/new occurrence sequences are aligned with a longest-common-subsequence pass — equivalent tolerance to Roslyn's syntax-offset matching for insertions/deletions/reorderings, expressed on the tree we actually diff. Ranges (which shift with line edits) are recorded for diagnostics only, never identity. See "C1 as implemented" below. |
| `EncVariableSlotAllocator` | `ClosureSlotAllocator` interface threaded into IlxGen/EraseClosures when compiling a delta | Same seam: lowering asks "did the previous generation have this occurrence?" and reuses class/method/field identity on hit. |
| PDB CDI blobs | **Emit the identical Roslyn CDI formats** in `ilwritepdb.fs`; read them in `ILBaselineReader`/`PortablePdbSnapshot` | The blob formats are language-agnostic and debugger-recognized; byte-parity here keeps future debugger EnC (Phase G) viable and lets mdv/Roslyn tooling inspect our maps. In-memory session state remains the fast path; the PDB is the durable source of truth (survives session restarts). |
| Display-class field matching | Closure class field reuse keyed by captured value identity (`CaptureIdentity`: logical name + `RuntimeTypeIdentity`) | Captures map to fields of the F# closure class; compatibility = same name and `RuntimeTypeIdentity`. |
| `DeletedMethodBody`/`HotReloadException` | Same: synthesize the exception type on first need (`NewTypeDefinition`), emit throwing stubs for incompatible/removed lambda methods | Required for safe behavior when stale closure instances call removed code. |

### C1 as implemented (`TypedTreeDiff.fs`)

The occurrence model, alignment, and classification landed in `src/Compiler/TypedTree/TypedTreeDiff.fs`
(`LambdaOccurrenceId`, `LambdaOccurrence`, `CaptureIdentity`, `CaptureSetChange`, `LambdaEdit`,
`MemberLambdaEdits`; `TypedTreeDiffResult.LambdaEdits` carries the per-member payload). Decisions made
while implementing, where they refine the draft above:

- **Occurrence granularity**: consecutive curried lambdas (`fun x -> fun y -> ...`, looking through
  `Expr.DebugPoint`/`Expr.Link`) form ONE occurrence with `CurriedArity = n`, matching how IlxGen forms
  a single closure class for a curried chain. A non-lambda expression between the lambdas (e.g. a `let`)
  ends the group, so an "extra closure layer" is two occurrences.
- **Member-top stripping**: the member's own parameter lambdas are not closures. Extraction strips the
  binding's top-level `Expr.TyLambda`s and exactly `ValReprInfo.NumCurriedArgs` lambda groups before
  walking the body — the same arity decision IlxGen replays when forming closures.
- **Structural digest**: the draft digest was (curried arity, capture identity list, return type
  identity). The implementation additionally includes per-group **parameter type identities**. Without
  them, a parameter-shape change (e.g. `fun (x, y) -> ...` → `fun (x, y, z) -> ...`) would silently
  align two occurrences that the legacy arity digest treated as a shape change — the model must never
  be more permissive than the digest it replaces for signature-shaped changes.
- **Captures**: free locals of the occurrence expression (`freeInExpr CollectTyparsAndLocals`), filtered
  to values captured from enclosing scopes: `IsCompiledAsTopLevel`/`IsMemberOrModuleBinding` values are
  accessed via static paths in IL and are never closure fields. Self identifiers are normalized
  (`this`/`base`) so renaming the F# self identifier is not a capture rename. Capture lists are
  deduplicated and ordered by (name, type identity) so set comparison is deterministic.
- **Alignment**: two LCS passes. Pass 1 matches on the full structural digest (parent chain, curried
  arity, parameter types, return identity, captures): pairs here are compatible (BodyEdited when the
  body hash differs). Pass 2 re-aligns the leftovers on the shape-only digest (sans captures): pairs
  here are either reordered survivors (capture sets still equal) or capture-incompatible
  (`CaptureSetChanged` with a precise sub-kind). Remaining occurrences are `Added`/`Removed`.
  Two *identical* reordered occurrences align positionally by construction — intentional, since
  indistinguishable closures are interchangeable.
- **Capture sub-kinds** (C# parity names appear verbatim in the rude-edit messages):
  same name/different type → `TypeChanged` (*ChangingCapturedVariableType*); same type/different name →
  `Renamed` (*RenamingCapturedVariable*); a value that stops being captured by one occurrence and starts
  being captured by another in the same member → `ScopeChanged` on both pairs
  (*ChangingCapturedVariableScope*, detected by a cross-occurrence post-pass); leftovers →
  `CaptureAdded`/`CaptureRemoved`. `CaptureAdded` messages note the edit may become applicable in C4
  via `AddInstanceFieldToExistingType`. Note: scope moves that leave every capture set unchanged are
  invisible to typed-tree capture identity; they surface (and are handled) at closure allocation in C3.
- **Parent-chain identity**: the parent chain stores enclosing-occurrence ordinals from the occurrence's
  own snapshot. Inserting a lambda *before* an outer lambda therefore shifts the ordinals of every
  later occurrence's parent chain and conservatively breaks alignment of their descendants
  (Removed+Added → rude). Acceptable for C1; revisit with hierarchical matching if test evidence demands.
- **Exclusions (legacy digest path)**: members containing quotations (`Expr.Quote` — stays on the
  query digest path entirely), object expressions (`Expr.Obj` — separate IlxGen closure path), local
  type functions (`Expr.TyLambda` in the body), or any capture/parameter/return type without a
  computable `RuntimeTypeIdentity` are NOT occurrence-modelled; they keep the pre-C1 whole-body
  `LambdaShapeDigest` comparison and rude-edit behavior, byte-for-byte. Loop/`try` operands
  (`TOp.While`/`TOp.IntegerForLoop`/`TOp.TryWith`/`TOp.TryFinally`) are wrapped by the checker in
  delay lambdas (`mkDummyLambda`) that IlxGen always eliminates; extraction walks through them without
  creating occurrences.
- **Deliberate tightening vs the legacy digest**: capture renames/type changes and lambda return/parameter
  identity changes were invisible to the arity-only digest and were previously (unsoundly) classified as
  plain MethodBody edits. They are now rude with C#-parity kinds. Pure body edits within an unchanged
  lambda set remain plain MethodBody edits, now carrying the structured `BodyEdited` payload for C4.
- **What C1 does NOT change**: edits that change the lambda set still produce `LambdaShapeChange` rude
  edits — but with structured messages (counts + ordinals) and a `LambdaEdits` payload ready for the C4
  emitter to consume.

### C2 blob encoder/decoder as implemented (`EncMethodDebugInformation.fs`)

`src/Compiler/CodeGen/EncMethodDebugInformation.fs` replicates Roslyn's three EnC CDI blob formats
byte for byte (`roslyn/src/Compilers/Core/Portable/Emit/EditAndContinueMethodDebugInformation.cs`:
`SerializeLocalSlots`/`SerializeLambdaMap`/`SerializeStateMachineStates` and the `Uncompress*`
readers), including the syntax-offset-baseline optimization and the slot-map kind/ordinal byte
packing (0xFF baseline marker, 0x00 temp, bit 7 = has-ordinal, low bits = kind + 1). The CDI kind
GUIDs (from `roslyn/src/Dependencies/CodeAnalysis.Debugging/PortableCustomDebugInfoKinds.cs`):

| CDI | Kind GUID |
|---|---|
| EnC Local Slot Map | `755F52A8-91C5-45BE-B4B8-209571E552BD` |
| EnC Lambda and Closure Map | `A643004C-0240-496F-A783-30D64F4979DE` |
| EnC State Machine State Map | `8B78CD68-2EDE-420B-980B-E15884B8AAA3` |

**Occurrence-key encoding**: the blob slots Roslyn fills with *syntax offsets* carry, in F#,
deterministic ints packed from the C1 occurrence ordinal chain (root-first enclosing-occurrence
ordinals ending with the occurrence's own ordinal). Packing is 16-bit segments: a depth-1 chain
`[o]` is the key `o` itself (0..0xFFFF); a depth-2 chain `[p; o]` packs as `((p + 1) <<< 16) ||| o`,
the +1 bias keeping depth-1 and depth-2 key spaces disjoint. Encoding **fails closed**
(`tryEncodeOccurrenceKey` returns `None`, callers must treat the occurrence as unmappable/rude):
chains deeper than 2, ordinals past 0xFFFF, or packed keys past the ECMA-335 compressed-integer
budget (0x1FFFFFFD, leaving room for the -1 baseline adjustment). Keys never truncate.

**Phase-G debugger-interop caveat**: the blob *format* is identical to Roslyn's, so mdv and Roslyn
tooling can decode our maps structurally — but the integers are occurrence keys, not source syntax
offsets. A debugger (or any consumer) that interprets them as positions into F# source will get
nonsense; Phase G must either teach the debugger the F# key semantics or emit a parallel
source-offset map. Cross-validation: the component test builds a C# library with the repo SDK,
decodes the Roslyn-emitted CDI rows with our decoder, and re-encodes them byte-identically.

### C2 baseline CDI emission as implemented (commit 2)

When `--enable:hotreloaddeltas` is on, the fsc emit path (`fsc.fs` `main6`, right where the
baseline-capture hook already receives `optimizedImpls`) computes per-method occurrence data from
the same optimized typed tree the baseline snapshot stores:

- **Conduit**: `TypedTreeDiff.collectMemberLambdaOccurrences` (public C1 extraction over a
  `CheckedImplFile`; returns every member binding, with an empty occurrence list for members the
  model cannot represent) → `EncMethodDebugInformation.computeMethodCustomDebugInfoRows`
  (occurrences → serialized blobs) → new `methodCustomDebugInfoRows: Map<string,
  PdbMethodCustomDebugInfo list>` field on the IL writer `options` → `generatePortablePdb` →
  CDI rows on `MethodDef` parents in the portable PDB. Flag-off builds pass the empty map
  everywhere; the writer path is a strict no-op then (EmittedIL gate: byte-identical output).
- **Keying (fail closed twice)**: rows are keyed by IL method name (`SymbolId.CompiledName`).
  The producer drops members without a compiled name and any compiled name claimed by more than
  one member binding in the assembly (overloads, same-named members on different types); the PDB
  writer independently drops any name that does not identify exactly one `MethodDef` row in the
  module. A map is therefore never attached to the wrong method; ambiguous methods simply carry
  no lambda map (later generations must treat their lambdas as unmappable). Token-precise keying
  (via the IlxGen token maps) can replace name keying in C3 if the exclusions bite.
- **What is emitted**: only the *EnC Lambda and Closure Map*, for methods with at least one
  occurrence whose key chains are encodable; `MethodOrdinal` stays `UndefinedMethodOrdinal` (-1,
  Roslyn semantics for "no partial-method disambiguation needed"). One closure scope per
  occurrence and lambda *i* references closure *i*: IlxGen lowers every lambda occurrence
  (curried group) to its own closure class, so there is no shared display-class scope to model;
  refinement to `Static`/`ThisOnly` closure ordinals is a C3 lowering concern. If *any*
  occurrence key of a method fails to encode (chain depth > 2, ordinal > 0xFFFF), the whole
  method gets no map — a partial map could silently mismatch occurrences.
- **What is omitted (documented exclusions)**: the *EnC Local Slot Map* (the lowered local slot
  layout is an IlxGen emission artifact, not trivially derivable from the typed tree — omitted
  rather than guessed) and the *EnC State Machine State Map* (Phase D). Members the C1 model
  excludes (quotations, object expressions, local type functions, uncomputable type identities)
  and module-level values whose initializers run in startup code (no method row matches their
  compiled name) carry no map.
- **Tests**: `PdbCdiEmissionTests` compiles a library with the flag (2 lambdas, one nested),
  opens the PDB with System.Reflection.Metadata, decodes the row with the F# decoder and checks
  occurrence keys `[0]`/`[0; 1]` plus closure ordinals; the flag-off compile of the same source
  must contain zero EnC CDI rows.

### C2 baseline CDI read & in-memory chaining as implemented (commit 3)

When a hot reload session baseline is captured, the EnC CDI rows of the baseline portable PDB
are decoded into `FSharpEmitBaseline.EncMethodDebugInfos: Map<int, EncMethodDebugInformation>`,
keyed by **MethodDef token** — the CDI parent of the EnC rows is a MethodDef handle, so token
keying is unambiguous (the name keying on the write side exists only because the PDB writer
lacks tokens). Details:

- **Reader**: `EncMethodDebugInformation.readEncMethodDebugInfoFromPortablePdb` (SRM
  `MetadataReaderProvider`, same SRM boundary as `HotReloadPdb.emitDelta`) collects all three
  EnC blob kinds per method and decodes them with the C2 decoder. Fail safe/fail closed: a
  non-PDB or empty image yields the empty map (flag-off and pre-C2 baselines start sessions
  fine with no per-method data), and a method whose blobs do not decode is omitted entirely.
- **Sources**: the fsc emit hook decodes the exact emitted PDB bytes
  (`PortablePdbSnapshot.Bytes`). The checker path (`FSharpChecker.StartHotReloadSession`)
  rewrites the baseline in memory *without* the CDI side channel, so it additionally reads the
  on-disk PDB as a sibling input when the snapshot carried no EnC rows (`service.fs`,
  `createBaseline`).
- **Generation chaining**: when a delta is emitted through the session
  (`EmitDeltaForCompilation`), the per-method occurrence data is recomputed from the fresh
  typed tree (`HotReloadBaseline.computeRefreshedEncMethodDebugInfos`, name→token resolution
  fail closed on non-unique names) and `chainEncMethodDebugInfos` replaces the updated/added
  methods' entries in the next-generation baseline — entries the fresh compile did not produce
  are dropped rather than left stale (their lambdas must be treated as unmappable). Unchanged
  methods keep their baseline entries, mirroring the `AddedOrChangedMethods` plumbing.
- **Deferred**: the delta PDB does **not** yet re-emit EnC CDI rows (delta-PDB writing is a
  separate path). Within a session the in-memory chain is the generation-accurate source C3
  consumes; persisting refreshed maps into delta PDBs (so a restarted session can rehydrate
  generation-N state) is the remaining gap. Methods *added* by a delta also carry no entry yet
  (no baseline token at refresh time) — added lambda members are a C4 concern.

### C3 occurrence-keyed closure name allocation as implemented (allocator; lowering wiring pending)

`src/Compiler/TypedTree/ClosureNameAllocator.fs` is the F# `EncVariableSlotAllocator.TryGetPreviousLambda/
TryGetPreviousClosure` analogue, expressed as a pure data transformation over the C1 occurrence model
(no IO, no IlxGen state — fully unit-tested by `ClosureNameAllocatorTests`):

- **API**: `allocateMemberClosureNames baselineOccurrences baselineNamesByOccurrenceChain
  freshOccurrences freshNameBase generation` → per-occurrence `Assignments`
  (`Reused of name` | `Fresh of name`, in occurrence order) plus
  `RefreshedNamesByOccurrenceChain`, the chain-forward table the next generation consumes.
  Tables are keyed by the **unpacked** root-first ordinal chain (`int list`), so the allocator
  carries none of the CDI packing limits; CDI keys translate via
  `EncMethodDebugInformation.decodeOccurrenceKey`.
- **Alignment**: exactly the C1 two-pass LCS — the index-pair core was extracted from
  `alignLambdaOccurrences` into `TypedTreeDiff.alignLambdaOccurrenceIndexPairs` and is shared by
  both consumers, so diff classification and name allocation can never disagree about pairing.
- **Decision rules** (all fail closed): matched pair with equal capture sets and a recorded
  baseline name → `Reused` (verbatim baseline closure class name). Matched but
  capture-incompatible (a pass-2 shape-only pair) → `Fresh` (Roslyn parity: the previous closure
  member is stubbed, a new one is synthesized). Matched but no recorded name (unmappable method,
  dropped chain entry) → `Fresh` — a name is never guessed. Unmatched (added) → `Fresh`. Removed
  occurrences contribute nothing to the refreshed table, so their names are unused forever and
  can never be reused (later allocations are always generation-suffixed, and the generation
  counter never repeats within a session).
- **Generation-suffixed name format** (the Roslyn `DebugId(ordinal, generation)` analogue):
  `{baseName}@hotreload#g{generation}_o{occurrenceOrdinal}`, e.g. `f@hotreload#g2_o3`. It extends
  the baseline replay naming (`f@hotreload`, `f@hotreload-1`, ...) — same `@hotreload` marker, but
  the `#g…_o…` suffix is disjoint from the `-{int}` replay-ordinal space (it never parses in
  `tryGetHotReloadOrdinal`, so snapshot canonicalization leaves it alone). Occurrence ordinals are
  unique within a member and generations strictly increase, so a (baseName, generation, ordinal)
  triple is allocated at most once per session.
- **Tests**: `ClosureNameAllocatorTests` covers the synthetic match/add/remove/nested/
  capture-incompatible/fail-closed cases and three-generation chaining, and additionally drives
  the allocator over REAL C1 extraction (checker compiles via the shared `DiffTestHarness`):
  an added filter lambda gets the generation-2 name while the surviving map lambda keeps the
  baseline name, and generation 3 reuses both from the chained table. The component
  `ClosureIdentityTests` pins the metadata-level invariant the delta path relies on today (and
  C4 extends): flag-on recompiles of an unchanged lambda set produce closure classes with
  identical names across three body-edit generations, so deltas can update the existing closure
  method bodies in place.

**Lowering wiring design.** Fully landed (stamp bridge + baseline capture, then the
delta-compile hook step). As implemented:

- `LambdaOccurrence.RootExprStamp` records the unique stamp of the occurrence's root lambda
  (the OUTERMOST `Expr.Lambda` of a curried group, looking through `Expr.DebugPoint`/`Expr.Link`)
  — extraction bookkeeping only, never part of the structural digest or alignment, and only
  meaningful within the compilation that produced the expression.
- The IlxGen closure call site (`GetIlxClosureFreeVars`) records stamp→emitted-closure-name into
  `ClosureNameAllocationState` (a `ConditionalWeakTable` side channel keyed by
  `CompilerGlobalState`, mirroring `CompilerGeneratedNameMapState`). Recording is armed only by
  the emit hook's `PrepareForCodeGeneration` for capture compiles; everywhere else the call site
  performs a single failed weak-table lookup and behaves byte-identically (EmittedIL gate).
- The fsc emit path (`main6`, next to the C2 CDI row computation over the same `optimizedImpls`)
  joins the recording with the C1 extraction via
  `ClosureNameAllocator.computeBaselineClosureNameRows`: per-member occurrence-chain→name tables,
  keyed by compiled name with the C2 fail-closed rules, plus a member-level completeness rule —
  if ANY occurrence of a member has no recorded name (closure formation diverged from
  extraction), the member gets no table and stays on sequence replay rather than risking fresh
  names for surviving closures.
- The rows ride the hook contract (`ICompilerEmitHook.TryEmitWithArtifacts` /
  `CompilerEmitArtifacts.ClosureNameRows`) into the capture, where
  `HotReloadBaseline.resolveClosureNameRowsByToken` re-keys them by MethodDef token (unique-name
  resolution shared with `computeRefreshedEncMethodDebugInfos`) and stores them as
  `FSharpEmitBaseline.EncClosureNames`, the C3 companion of `EncMethodDebugInfos`. Baselines
  created without an in-process flag-on emit (e.g. the checker's read-from-disk path) carry the
  empty map: the occurrence-keyed naming stays inert there and delta compiles keep pure
  sequence-replay behavior (fail closed; names are not persisted in the PDB because the CDI
  blob format has no name slots).
- **Delta compiles (the hook codegen step)**: `ICompilerEmitHook.PrepareForCodeGeneration` now
  receives `tcGlobals` and the `optimizedImpls` about to be lowered (fsc `main4`, immediately
  before `GenerateIlxCode`). When a session with non-empty `EncClosureNames` is active, the hook
  calls `HotReloadBaseline.computeOccurrenceKeyedClosureNames`: extract occurrences from the
  session's previous-generation impl files and from the fresh tree (unique-compiled-name keying,
  fail closed on ambiguity), resolve each member's baseline chain→name table via its MethodDef
  token, run `ClosureNameAllocator.allocateMemberClosureNames` per member with
  `generation = session.CurrentGeneration`, and install the resulting stamp→assigned-name map
  via `ClosureNameAllocationState.setAssignedClosureNames`.
- **Call-site semantics (consume-then-override)**: the closure call site still consumes its
  replay-map slot unconditionally (`GetUniqueCompilerGeneratedName` memoizes per stamp, exactly
  one slot per closure), so non-closure synthesized names sharing a basic name keep their
  baseline replay positions; the occurrence-keyed table then overrides the closure's own name
  when its stamp was allocated. Stamps absent from the table (fail-closed members, IlxGen-
  synthesized closures such as object expressions/sequence closures) keep the replayed name —
  for unchanged lambda sets the allocator's `Reused` names equal the replayed names, so the
  body-edit behavior is unchanged by construction.
- **Chaining**: `DeltaEmissionRequest.RefreshedClosureNameRows` carries the allocator's
  refreshed per-method tables (recomputed in `EmitDeltaForCompilation` from the same session
  state + fresh tree — deterministic, so it agrees with what the emit-time install produced)
  and `HotReloadBaseline.chainClosureNameRows` replaces/drops the updated methods' tables in
  the next-generation baseline, exactly mirroring `chainEncMethodDebugInfos`. Flag-on
  recapture compiles (the component-test flow) instead re-record and re-join, so the replaced
  baseline's tables carry the generation's final names either way.
- **Known residual (accepted)**: when a lambda set changes, replay slots consumed by overridden
  closures shift the replay sequence for any OTHER synthesized-name consumer sharing the same
  basic name; such members are exactly the ones still classified rude for emission today, and
  the collision window (closure base names shared with non-closure synthesized names) predates
  this wiring.
- **C3/C4 boundary**: C3 fixes the NAME of every occurrence — including added ones, which lower
  to `{base}@hotreload#g{N}_o{i}` classes in the delta compile's in-memory rewrite (pinned by
  the `ClosureIdentityTests` added-lambda test). Phase C4 (landed) EMITS those added members:
  classification allows Added-only lambda sets when the runtime advertises
  `NewTypeDefinition` + `AddMethodToExistingType` (and Removed-only sets unconditionally —
  the baseline closure class just goes unused), and the delta emitter detects the
  generation-suffix marker on fresh-compile types with no baseline TypeDef token, emitting a
  NEW TypeDef row (+ AddField/AddMethod pairs, NestedClass row) per the Roslyn reference
  template recorded in docs/hot-reload-member-additions.md. The added type token chains into
  the next-generation baseline `TypeTokens`, so later generations body-edit the added
  closure's methods in place (validated by the multi-generation runtime test
  `ApplyUpdate succeeds for added lambda creating a new closure class`). Two C4 wiring
  consequences: `checker.StartHotReloadSession` carries `EncClosureNames` over from the
  in-process capture session it replaces (same MVID) because disk artifacts cannot encode
  the tables, and MemberRef/TypeSpec token passthrough became content-validated (an added
  lambda shifts the fresh compile's reference-row order; see the member-additions doc).
  Still rude: capture-set changes of matched occurrences (capture-field mapping is a later
  slice), genuinely new generic instantiations (TypeSpec emission), and generic closure
  classes (GenericParam emission).

The original design rationale, recorded before the wiring landed:

1. *Where closure names are born*: one call site — `IlxGen.GetIlxClosureFreeVars` calls
   `StableNameGenerator.GetUniqueCompilerGeneratedName(basename, expr.Range, uniq)`, which lands in
   `ICompilerGeneratedNameMap.GetOrAddName basicName`. The seam is keyed by basic-name SEQUENCE
   only: it carries no occurrence identity and no name-kind discrimination (closure type names,
   static-field helpers, record/union helpers all flow through the same `GetOrAddName`).
2. *Bridging identity*: IlxGen cannot replay the C1 occurrence numbering (curried-group merging,
   member-top stripping, delay-lambda elimination happen in extraction, not lowering, and closure
   formation can be deferred for let-rec/local type functions). The identity available at BOTH
   ends within one compile is the lambda expression's unique stamp (`Expr.Lambda(uniq, ...)`),
   already in hand at the call site. Plan: record the root expression stamp on `LambdaOccurrence`
   (extraction-time capture, never part of the structural digest), so an allocation can be turned
   into a stamp→name table by extracting occurrences from the very tree IlxGen lowers.
3. *Baseline derivation gap*: the baseline CDI stores occurrence keys, not names, and the name-map
   snapshot stores names by basic-name sequence — joining them by allocation order would
   re-introduce exactly the sequence keying C3 removes. The trustworthy generation-1
   chain→name table must be captured during baseline IlxGen: record stamp→emitted closure type
   name at the call site and join with the stamp→chain table from the same tree's extraction in
   the fsc emit path (where C2 already computes CDI rows from `optimizedImpls`); store per
   MethodDef token on the session baseline and chain it like `EncMethodDebugInfos`. (The CDI blob
   format has no name slots — Roslyn recomputes C# names from `DebugId` alone, which F# cannot do
   for baseline occurrences whose names embed line numbers or replay ordinals.)
4. *Process topology*: in the watch flow the delta IL is produced by the fsc emit pipeline (the
   emit hook's `PrepareForCodeGeneration` installs the replay map on fsc's `CompilerGlobalState`)
   while the session chains state in the checker. The hook contract must grow a codegen-time step
   that (a) pulls the session's per-method chain tables, (b) extracts fresh occurrences from the
   impl files being lowered (same-tree stamps), (c) runs the allocator per member, and (d)
   installs the stamp→name table next to the name map (a `ConditionalWeakTable` seam like
   `CompilerGeneratedNameMapState`) for the closure call site to consult before falling back to
   sequence replay. Flag-off and non-session compiles see no table and stay byte-identical.
5. *Seam discrimination*: no heuristics on name text — the closure call site consults the
   occurrence table directly (name-kind discrimination by construction: only that call site
   looks); every other synthesized name keeps the existing `GetOrAddName` replay behavior.

### State machines (Phase D, builds on the same map)

- F# `async` lowers to closure chains → covered by the lambda/closure map directly (a major
  simplification vs C#: most F# CE code needs *no* extra state-machine machinery).
- `task`/resumable code lowers to genuine state machines → port the *EnC State Machine State Map*
  blob + state-number monotonicity rule (new resume points take numbers above the previous
  generation's max; insertion mid-sequence stays rude, matching C#'s `ChangingStateMachineShape`).
- Hoisted locals in resumable state machines reuse the Local Slot Map mechanism (slot append-only,
  type-checked reuse).

### Rude-edit surface after Phase C/D (parity with C#)

Allowed: adding/removing/reordering lambdas; editing lambda bodies; adding captures (new closure field
via `AddInstanceFieldToExistingType`, or fresh closure class when incompatible); new hoisted locals.
Still rude (same as C#): captured-variable rename/type/scope change (runtime rude edit → throwing stub),
lambda signature changes, mid-sequence resume-point insertion, struct-closure capture additions.

## Commit plan (each independently reviewable, each with tests)

- **C1** `feat(hot-reload): structured lambda occurrence model and alignment in the typed-tree diff` —
  ✅ landed: occurrence extraction + LCS alignment + capture-compatibility classification. Replaces the
  blanket `LambdaShapeChange` digest check with structured `LambdaEdit` data (still conservatively rude
  at emission until C4).
- **C2** `feat(hot-reload): Roslyn-format EnC CDI blobs in portable PDBs` — commit 1 ✅ landed:
  encoder/decoder module (`EncMethodDebugInformation.fs`) with round-trip + cross-validation tests
  against Roslyn-emitted blobs (see "C2 blob encoder/decoder as implemented"). Commit 2 ✅ landed:
  baseline-time EnC Lambda and Closure Map emission in ilwritepdb under the flag (see "C2 baseline
  CDI emission as implemented"). Commit 3 ✅ landed: baseline read into the session +
  in-memory generation chaining (see "C2 baseline CDI read & in-memory chaining as
  implemented"); delta-PDB CDI re-emission deferred.
- **C3** `feat(hot-reload): generation-aware closure identity in IlxGen lowering` — commit 1 ✅
  landed: occurrence-keyed closure name allocation model (`ClosureNameAllocator.fs`): matched
  occurrences reuse baseline closure class names verbatim, new occurrences get generation-suffixed
  names (`…@hotreload#g{gen}_o{ord}`), removed names are never reused (see "C3 occurrence-keyed
  closure name allocation as implemented"). Commit 2 ✅ landed: lambda root stamps + baseline
  stamp→name capture (`EncClosureNames`). Commit 3 ✅ landed: the allocator threaded into IlxGen
  for delta compiles via the stamp-keyed seam, with chain-forward through
  `RefreshedClosureNameRows`. Flag-off behavior byte-identical (EmittedIL gate).
- **C4** `feat(hot-reload): emit added lambda members in deltas` — new methods on existing closure
  classes (`AddMethodToExistingType`), new display classes (`NewTypeDefinition`), new capture fields
  (`AddInstanceFieldToExistingType`), throwing stubs for incompatible occurrences; capability-gated
  via the Phase A model.
- **C5** `test(hot-reload): closure edit end-to-end coverage` — component + runtime-apply tests
  (add `List.map (fun ...)` to a live method, nested closures, capture addition, incompatible capture
  change → stub), dotnet-watch e2e scenario added to the demo driver.

## Open questions (to resolve during C3)

1. ~~Occurrence alignment tolerance~~ — resolved in C1: full structural digest first, shape-only digest
   second; positional fallback only between indistinguishable occurrences.
2. F# `let` local functions: lowered as methods or closures depending on capture/inline decisions —
   the occurrence model keys off the typed tree the diff already walks; the lowering decision must be
   replayed via the name-map state when C3 threads occurrence identity into IlxGen.
3. Quotation-bearing code (`QueryExpressionShapeChange`): kept rude in C1 (members containing
   `Expr.Quote` are excluded from occurrence modelling); revisit after C4.
4. Whole-body hash sensitivity: the FNV/XOR combination in `exprDigest` treats swaps of adjacent
   independent `let` bindings as identical (XOR commutes), so a pure reorder may produce no
   MethodBody edit at all. Pre-existing behavior, documented by the C1 reorder test; tighten when the
   hash is next revised.
