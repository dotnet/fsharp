# F# Hot Reload: Closure & State-Machine Mapping Design

This document describes how F# hot reload gives lambdas, closures, and state machines stable
identity across edit generations: the typed-tree lambda occurrence model and alignment in
`TypedTreeDiff`, Roslyn-format EnC CustomDebugInformation (CDI) persistence, occurrence-keyed
closure name allocation, added-lambda (new closure class) emission with occurrence-derived
deterministic naming across processes, and state-machine edit support (classification,
emission backstops, state-map persistence).
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
| Syntax offsets + SyntaxMap | **Typed-tree occurrence alignment** | F#'s diff is typed-tree based (no Roslyn-style SyntaxMap). Lambdas are identified by a per-member *occurrence path*: traversal index within the member body + parent-lambda chain + structural digest. Old/new occurrence sequences are aligned with a longest-common-subsequence pass — equivalent tolerance to Roslyn's syntax-offset matching for insertions/deletions/reorderings, expressed on the tree we actually diff. Ranges (which shift with line edits) are recorded for diagnostics only, never identity. See "The lambda occurrence model as implemented" below. |
| `EncVariableSlotAllocator` | `ClosureSlotAllocator` interface threaded into IlxGen/EraseClosures when compiling a delta | Same seam: lowering asks "did the previous generation have this occurrence?" and reuses class/method/field identity on hit. |
| PDB CDI blobs | **Emit the identical Roslyn CDI formats** in `ilwritepdb.fs`; read them in `ILBaselineReader`/`PortablePdbSnapshot` | The blob formats are language-agnostic and debugger-recognized; byte-parity here keeps future debugger EnC (active-statement and sequence-point tracking) viable and lets mdv/Roslyn tooling inspect our maps. In-memory session state remains the fast path; the PDB is the durable source of truth (survives session restarts). |
| Display-class field matching | Closure class field reuse keyed by captured value identity (`CaptureIdentity`: logical name + `RuntimeTypeIdentity`) | Captures map to fields of the F# closure class; compatibility = same name and `RuntimeTypeIdentity`. |
| `DeletedMethodBody`/`HotReloadException` | Same: synthesize the exception type on first need (`NewTypeDefinition`), emit throwing stubs for incompatible/removed lambda methods | Required for safe behavior when stale closure instances call removed code. |

### The lambda occurrence model as implemented (`TypedTreeDiff.fs`)

The occurrence model, alignment, and classification live in `src/Compiler/TypedTree/TypedTreeDiff.fs`
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
  `CaptureAdded`/`CaptureRemoved`. `CaptureAdded` messages note the edit may become applicable
  via `AddInstanceFieldToExistingType` once capture-field additions are supported. Note: scope moves
  that leave every capture set unchanged are invisible to typed-tree capture identity; they surface
  (and are handled) at closure name allocation.
- **Parent-chain identity**: the parent chain stores enclosing-occurrence ordinals from the occurrence's
  own snapshot. Inserting a lambda *before* an outer lambda therefore shifts the ordinals of every
  later occurrence's parent chain and conservatively breaks alignment of their descendants
  (Removed+Added → rude). An accepted conservatism; revisit with hierarchical matching if test
  evidence demands.
- **Exclusions (legacy digest path)**: members containing quotations (`Expr.Quote` — stays on the
  query digest path entirely), object expressions (`Expr.Obj` — separate IlxGen closure path), local
  type functions (`Expr.TyLambda` in the body), or any capture/parameter/return type without a
  computable `RuntimeTypeIdentity` are NOT occurrence-modelled; they keep the legacy whole-body
  `LambdaShapeDigest` comparison and rude-edit behavior, byte-for-byte. Loop/`try` operands
  (`TOp.While`/`TOp.IntegerForLoop`/`TOp.TryWith`/`TOp.TryFinally`) are wrapped by the checker in
  delay lambdas (`mkDummyLambda`) that IlxGen always eliminates; extraction walks through them without
  creating occurrences.
- **Deliberate tightening vs the legacy digest**: capture renames/type changes and lambda return/parameter
  identity changes were invisible to the arity-only digest and were previously (unsoundly) classified as
  plain MethodBody edits. They are now rude with C#-parity kinds. Pure body edits within an unchanged
  lambda set remain plain MethodBody edits, carrying the structured `BodyEdited` payload for the
  delta emitter.
- **Scope of the model**: the occurrence model by itself does not relax the edit surface — edits that
  change the lambda set produce `LambdaShapeChange` rude edits unless the added-lambda emission path
  (below) covers them. The structured messages (counts + ordinals) and the `LambdaEdits` payload are
  what that emitter consumes.

### EnC CDI blob encoder/decoder as implemented (`EncMethodDebugInformation.fs`)

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
deterministic ints packed from the occurrence ordinal chain (root-first enclosing-occurrence
ordinals ending with the occurrence's own ordinal). Packing is 16-bit segments: a depth-1 chain
`[o]` is the key `o` itself (0..0xFFFF); a depth-2 chain `[p; o]` packs as `((p + 1) <<< 16) ||| o`,
the +1 bias keeping depth-1 and depth-2 key spaces disjoint. Encoding **fails closed**
(`tryEncodeOccurrenceKey` returns `None`, callers must treat the occurrence as unmappable/rude):
chains deeper than 2, ordinals past 0xFFFF, or packed keys past the ECMA-335 compressed-integer
budget (0x1FFFFFFD, leaving room for the -1 baseline adjustment). Keys never truncate.

**Debugger-interop caveat**: the blob *format* is identical to Roslyn's, so mdv and Roslyn
tooling can decode our maps structurally — but the integers are occurrence keys, not source syntax
offsets. A debugger (or any consumer) that interprets them as positions into F# source will get
nonsense; future active-statement/sequence-point debugger work must either teach the debugger the
F# key semantics or emit a parallel source-offset map. Cross-validation: the component test builds
a C# library with the repo SDK, decodes the Roslyn-emitted CDI rows with our decoder, and
re-encodes them byte-identically.

### Baseline CDI emission as implemented

When `--enable:hotreloaddeltas` is on, the fsc emit path (`fsc.fs` `main6`, right where the
baseline-capture hook already receives `optimizedImpls`) computes per-method occurrence data from
the same optimized typed tree the baseline snapshot stores:

- **Conduit**: `TypedTreeDiff.collectMemberLambdaOccurrences` (public occurrence extraction over a
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
  (via the IlxGen token maps) can replace name keying if the exclusions bite.
- **What is emitted**: only the *EnC Lambda and Closure Map*, for methods with at least one
  occurrence whose key chains are encodable; `MethodOrdinal` stays `UndefinedMethodOrdinal` (-1,
  Roslyn semantics for "no partial-method disambiguation needed"). One closure scope per
  occurrence and lambda *i* references closure *i*: IlxGen lowers every lambda occurrence
  (curried group) to its own closure class, so there is no shared display-class scope to model;
  refinement to `Static`/`ThisOnly` closure ordinals is a lowering-side concern. If *any*
  occurrence key of a method fails to encode (chain depth > 2, ordinal > 0xFFFF), the whole
  method gets no map — a partial map could silently mismatch occurrences.
- **What is omitted (documented exclusions)**: the *EnC Local Slot Map* (the lowered local slot
  layout is an IlxGen emission artifact, not trivially derivable from the typed tree — omitted
  rather than guessed). The *EnC State Machine State Map* is emitted separately (see "EnC State
  Machine State Map persistence" below). Members the occurrence model excludes (quotations,
  object expressions, local type functions, uncomputable type identities) and module-level
  values whose initializers run in startup code (no method row matches their compiled name)
  carry no map.
- **Tests**: `PdbCdiEmissionTests` compiles a library with the flag (2 lambdas, one nested),
  opens the PDB with System.Reflection.Metadata, decodes the row with the F# decoder and checks
  occurrence keys `[0]`/`[0; 1]` plus closure ordinals; the flag-off compile of the same source
  must contain zero EnC CDI rows.

### Baseline CDI read & in-memory chaining as implemented

When a hot reload session baseline is captured, the EnC CDI rows of the baseline portable PDB
are decoded into `FSharpEmitBaseline.EncMethodDebugInfos: Map<int, EncMethodDebugInformation>`,
keyed by **MethodDef token** — the CDI parent of the EnC rows is a MethodDef handle, so token
keying is unambiguous (the name keying on the write side exists only because the PDB writer
lacks tokens). Details:

- **Reader**: `EncMethodDebugInformation.readEncMethodDebugInfoFromPortablePdb` (SRM
  `MetadataReaderProvider`, same SRM boundary as `HotReloadPdb.emitDelta`) collects all three
  EnC blob kinds per method and decodes them with the blob decoder. Fail safe/fail closed: a
  non-PDB or empty image yields the empty map (flag-off baselines and baselines emitted without
  EnC CDI rows start sessions fine with no per-method data), and a method whose blobs do not
  decode is omitted entirely.
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
  separate path). Within a session the in-memory chain is the generation-accurate source the
  name allocator consumes. Baseline (generation-0) state fully survives process restarts: the
  on-disk PDB carries the occurrence keys and the occurrence-derived naming (below) makes the
  names derivable from them. Rehydrating MID-SESSION (generation-N) state in a new process
  still needs delta-PDB CDI re-emission. Methods *added* by a delta also carry no entry yet
  (no baseline token at refresh time).

### Occurrence-keyed closure name allocation as implemented

`src/Compiler/TypedTree/ClosureNameAllocator.fs` is the F# `EncVariableSlotAllocator.TryGetPreviousLambda/
TryGetPreviousClosure` analogue, expressed as a pure data transformation over the lambda occurrence model
(no IO, no IlxGen state — fully unit-tested by `ClosureNameAllocatorTests`):

- **API**: `allocateMemberClosureNames baselineOccurrences baselineNamesByOccurrenceChain
  freshOccurrences freshNameBase generation` → per-occurrence `Assignments`
  (`Reused of name` | `Fresh of name`, in occurrence order) plus
  `RefreshedNamesByOccurrenceChain`, the chain-forward table the next generation consumes.
  Tables are keyed by the **unpacked** root-first ordinal chain (`int list`), so the allocator
  carries none of the CDI packing limits; CDI keys translate via
  `EncMethodDebugInformation.decodeOccurrenceKey`.
- **Alignment**: exactly the diff's two-pass LCS — the index-pair core was extracted from
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
  `{baseName}@hotreload#g{generation}_o{occurrenceChain}`, e.g. `f@hotreload#g2_o3` for a
  top-level occurrence and `f@hotreload#g2_o0_3` for a nested one (the rendering covers the
  full root-first chain, underscore-separated, so nested added occurrences can never collide;
  chains are bounded by the CDI key encoding — depth ≤ 2, ordinals ≤ 0xFFFF, deeper chains
  fail closed before any name is derived — so the suffix is bounded and names never truncate).
  Generation 0 is reserved for the baseline derivation (see "Occurrence-derived baseline
  naming" below). The format extends the replay naming (`f@hotreload`, `f@hotreload-1`, ...)
  — same `@hotreload` marker, but the `#g…_o…` suffix is disjoint from the `-{int}`
  replay-ordinal space (it never parses in `tryGetHotReloadOrdinal`; snapshot canonicalization
  drops these names from replay buckets entirely, see the replay-bucket canonicalization note
  below). Occurrence chains are unique within a member and generations strictly increase
  within a session, so a (baseName, generation, chain) triple is allocated at most once.
- **Tests**: `ClosureNameAllocatorTests` covers the synthetic match/add/remove/nested/
  capture-incompatible/fail-closed cases and three-generation chaining, and additionally drives
  the allocator over REAL occurrence extraction (checker compiles via the shared
  `DiffTestHarness`): an added filter lambda gets the generation-2 name while the surviving map
  lambda keeps the baseline name, and generation 3 reuses both from the chained table. The
  component `ClosureIdentityTests` pins the metadata-level invariant the delta path relies on
  (and the added-lambda path extends): flag-on recompiles of an unchanged lambda set produce
  closure classes with identical names across three body-edit generations, so deltas can update
  the existing closure method bodies in place.

**Lowering wiring** (stamp bridge + baseline capture + the delta-compile hook step). As
implemented:

- `LambdaOccurrence.RootExprStamp` records the unique stamp of the occurrence's root lambda
  (the OUTERMOST `Expr.Lambda` of a curried group, looking through `Expr.DebugPoint`/`Expr.Link`)
  — extraction bookkeeping only, never part of the structural digest or alignment, and only
  meaningful within the compilation that produced the expression.
- The IlxGen closure call site (`GetIlxClosureFreeVars`) records stamp→emitted-closure-name into
  `ClosureNameAllocationState` (a `ConditionalWeakTable` side channel keyed by
  `CompilerGlobalState`, mirroring `CompilerGeneratedNameMapState`). Recording is armed only by
  the emit hook's `PrepareForCodeGeneration` for capture compiles; everywhere else the call site
  performs a single failed weak-table lookup and behaves byte-identically (EmittedIL gate).
- The fsc emit path (`main6`, next to the CDI row computation over the same `optimizedImpls`)
  joins the recording with the occurrence extraction via
  `ClosureNameAllocator.computeBaselineClosureNameRows`: per-member occurrence-chain→name tables,
  keyed by compiled name with the same fail-closed rules as the CDI emission, plus a
  member-level completeness rule — if ANY occurrence of a member has no recorded name (closure
  formation diverged from extraction), the member gets no table and stays on sequence replay
  rather than risking fresh names for surviving closures.
- The rows ride the hook contract (`ICompilerEmitHook.TryEmitWithArtifacts` /
  `CompilerEmitArtifacts.ClosureNameRows`) into the capture, where
  `HotReloadBaseline.resolveClosureNameRowsByToken` re-keys them by MethodDef token (unique-name
  resolution shared with `computeRefreshedEncMethodDebugInfos`) and stores them as
  `FSharpEmitBaseline.EncClosureNames`, the companion of `EncMethodDebugInfos`. Baselines
  created without an in-process flag-on emit (e.g. the checker's read-from-disk path) carry the
  empty map: the occurrence-keyed naming stays inert there and delta compiles keep pure
  sequence-replay behavior (fail closed; names are not persisted in the PDB because the CDI
  blob format has no name slots).
- **Delta compiles (the hook codegen step)**: `ICompilerEmitHook.PrepareForCodeGeneration`
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
- **Naming vs emission boundary**: the allocator fixes the NAME of every occurrence — including
  added ones, which lower to `{base}@hotreload#g{N}_o{i}` classes in the delta compile's
  in-memory rewrite (pinned by the `ClosureIdentityTests` added-lambda test). The added-lambda
  emission path EMITS those added members: classification allows Added-only lambda sets when
  the runtime advertises `NewTypeDefinition` + `AddMethodToExistingType` (and Removed-only
  sets unconditionally — the baseline closure class just goes unused), and the delta emitter
  detects the generation-suffix marker on fresh-compile types with no baseline TypeDef token,
  emitting a NEW TypeDef row (+ AddField/AddMethod pairs, NestedClass row) per the Roslyn
  reference template recorded in docs/hot-reload-member-additions.md. The added type token
  chains into the next-generation baseline `TypeTokens`, so later generations body-edit the
  added closure's methods in place (validated by the multi-generation runtime test
  `ApplyUpdate succeeds for added lambda creating a new closure class`). Two wiring
  consequences: `checker.StartHotReloadSession` carries `EncClosureNames` over from the
  in-process capture session it replaces (same MVID) because disk artifacts cannot encode
  the tables, and MemberRef/TypeSpec token passthrough became content-validated (an added
  lambda shifts the fresh compile's reference-row order; see the member-additions doc).
  Genuinely new generic instantiations are no longer rude: the delta writer appends
  TypeSpec rows (Default op, C# template parity), so an added lambda whose closure class
  extends a brand-new `FSharpFunc<A,B>` emits and applies (see the member-additions doc).
  Still rude: capture-set changes of matched occurrences (capture-field mapping is a later
  slice) and generic closure classes (GenericParam emission). The MVID-matched
  `EncClosureNames` carry-over in `checker.StartHotReloadSession` is superseded by the
  occurrence-derived reconstruction (below) and demoted to a consistency check.

### Occurrence-derived baseline naming and cross-process reconstruction

The lowering wiring above initially left one in-memory dependency: the generation-1 chain→name
tables were captured during baseline IlxGen (stamp→name recording) and could not be re-derived
from disk artifacts, so a session started from the on-disk baseline — the dotnet-watch topology,
where fsc builds in a separate process from the FCS session — had empty `EncClosureNames`
and the allocator failed closed on every lambda set change. Occurrence-derived baseline naming
closes this the Roslyn way: **names are functions of identity and are never persisted.**

- **The derivation function**: under `--enable:hotreloaddeltas` the BASELINE compile names
  every mapped closure class `{memberCompiledName}@hotreload#g0_o{chain}` — the allocator's
  fresh-name format at generation 0, with the occurrence's root-first ordinal chain rendered
  underscore-separated (`ClosureNameAllocator.formatGenerationSuffixedClosureName`,
  `formatOccurrenceChainKey`). The base name is the member's compiled name (after IlxGen's
  type-name character cleanup) — the same base the delta allocator already used for fresh
  names — NOT the IlxGen let-bound basename, so the name is recomputable from the MethodDef
  row alone. Implementation: `HotReloadEmitHook.PrepareForCodeGeneration` derives the
  stamp→name table from the same `optimizedImpls` extraction the CDI emission consumes
  (`HotReloadBaseline.computeBaselineOccurrenceKeyedClosureNames`) and arms the existing
  assigned-name side channel before lowering; the closure call site's consume-then-override
  semantics are untouched. Gating mirrors the CDI emission EXACTLY (unique compiled names,
  every chain encodable), so a name is derived if and only if its occurrence key is
  persisted; members failing the gates keep replay naming and stay fail-closed for
  set-changes, byte-for-byte like before.
- **Reconstruction** (`HotReloadBaseline.deriveEncClosureNamesFromEncDebugInfos`): baseline
  creation — both the in-process capture and the checker's read-from-disk path — re-derives
  `EncClosureNames` from the decoded `EncMethodDebugInfos` occurrence keys plus the member
  name from the MethodDef token. Fail closed twice: (1) a baseline containing ANY
  generation-suffixed TypeDef of generation ≥ 1 is a mid-session recapture artifact (its
  added-closure names carry their first-allocation generation and are not derivable from
  gen-0 identity) — no table is reconstructed at all; (2) per member, every derived name
  must exist as a baseline TypeDef simple name — this drops older baselines whose closures
  carry replay names and members with occurrences that never lowered to closure classes, so
  a table can never claim a name the baseline does not contain.
- **Recorder and carry-over demoted**: the stamp→name recording remains the table SOURCE
  only for recapture compiles emitted under an active session (their names legitimately
  carry later generations); for plain baseline captures it is a validation — derived ==
  recorded wherever both produced a table (trace + debug assertion on mismatch). The
  MVID-matched carry-over in `StartHotReloadSession` became a consistency check (a gen-0
  in-process session disagreeing with a non-empty reconstruction asserts; an EMPTY
  reconstruction against a recapture session is the designed fail-closed outcome).
- **Replay-bucket canonicalization**: generation-suffixed names are allocator-managed and
  never replayed, so `FSharpSynthesizedTypeMaps.LoadSnapshot` drops them from replay
  buckets; the surviving replay-ordinal names are placed at their EXACT ordinal slots
  (holes filled with the deterministic slot names), so non-closure synthesized names
  sharing a basic name keep their baseline replay positions across disk restores — the
  slots consumed (and overridden) by derived-named closures stay reserved.
- **Determinism pin**: `RuntimeIntegrationTests` proves an in-process capture session and a
  disk-started session for the same output assembly carry IDENTICAL tables (token, chain
  and name), including depth-2 chains; the cross-process addition/removal tests build the
  baseline through the command-line fsc path, reset all in-process session state, start
  from disk only, and drive add → ApplyUpdate → invoke → follow-up body edit chains.
- **Accepted residuals**: mid-session (generation-N) state still does not survive a process
  restart — that needs delta-PDB CDI re-emission (unchanged deferral); recapture-produced
  DLLs are not reconstructable by design (fail closed to replay); members whose chains
  exceed the CDI encoding (depth > 2) fail closed identically in-process and from disk
  (an earlier in-process recorder kept replay-name tables for them — an in-memory-only
  capability inconsistent with the persistence model, removed).

The underlying design rationale:

1. *Where closure names are born*: one call site — `IlxGen.GetIlxClosureFreeVars` calls
   `StableNameGenerator.GetUniqueCompilerGeneratedName(basename, expr.Range, uniq)`, which lands in
   `ICompilerGeneratedNameMap.GetOrAddName basicName`. The seam is keyed by basic-name SEQUENCE
   only: it carries no occurrence identity and no name-kind discrimination (closure type names,
   static-field helpers, record/union helpers all flow through the same `GetOrAddName`).
2. *Bridging identity*: IlxGen cannot replay the diff's occurrence numbering (curried-group merging,
   member-top stripping, delay-lambda elimination happen in extraction, not lowering, and closure
   formation can be deferred for let-rec/local type functions). The identity available at BOTH
   ends within one compile is the lambda expression's unique stamp (`Expr.Lambda(uniq, ...)`),
   already in hand at the call site. So: record the root expression stamp on `LambdaOccurrence`
   (extraction-time capture, never part of the structural digest), so an allocation can be turned
   into a stamp→name table by extracting occurrences from the very tree IlxGen lowers.
3. *Baseline derivation gap*: the baseline CDI stores occurrence keys, not names, and the name-map
   snapshot stores names by basic-name sequence — joining them by allocation order would
   re-introduce exactly the sequence keying the occurrence-keyed allocation removes. The
   trustworthy generation-1 chain→name table must be captured during baseline IlxGen: record
   stamp→emitted closure type name at the call site and join with the stamp→chain table from the
   same tree's extraction in the fsc emit path (where the emit path already computes CDI rows
   from `optimizedImpls`); store per MethodDef token on the session baseline and chain it like
   `EncMethodDebugInfos`. (The CDI blob format has no name slots — Roslyn recomputes C# names
   from `DebugId` alone, which F# cannot do for baseline occurrences whose names embed line
   numbers or replay ordinals; the occurrence-derived `#g0` naming closes exactly this gap.)
4. *Process topology*: in the watch flow the delta IL is produced by the fsc emit pipeline (the
   emit hook's `PrepareForCodeGeneration` installs the replay map on fsc's `CompilerGlobalState`)
   while the session chains state in the checker. The hook contract therefore has a codegen-time
   step that (a) pulls the session's per-method chain tables, (b) extracts fresh occurrences from
   the impl files being lowered (same-tree stamps), (c) runs the allocator per member, and (d)
   installs the stamp→name table next to the name map (a `ConditionalWeakTable` seam like
   `CompilerGeneratedNameMapState`) for the closure call site to consult before falling back to
   sequence replay. Flag-off and non-session compiles see no table and stay byte-identical.
5. *Seam discrimination*: no heuristics on name text — the closure call site consults the
   occurrence table directly (name-kind discrimination by construction: only that call site
   looks); every other synthesized name keeps the existing `GetOrAddName` replay behavior.

### Force-rebuilt on-disk outputs (the dotnet-watch topology)

The dotnet-watch host force-rebuilds the project output (`dotnet build`, a separate fsc
process) between edits, and the FCS session consumes that on-disk dll as each
generation's FRESH module (`EmitHotReloadDelta` reads the output path; the typed trees
come from the session's own check). Two semantics were pinned down while fixing the
generation-2 crash this topology exposed (`InvalidProgramException` in a
generation-1-added closure's `.cctor`):

- **Name authority is the compile that produced the fresh module.** An in-process delta
  compile (the fsc emit-hook flow) names added occurrences with the allocator's
  `#g{N}_o{chain}` because `PrepareForCodeGeneration` installed the assigned-name table;
  a SESSION-LESS rebuild has no session state, so it names every mapped closure with the
  gen-0 baseline derivation `#g0_o{chain}` — including occurrences the session considers
  "added in generation N". Both are correct: the session is name-agnostic. The delta
  emitter detects added closures by the generation-suffix marker plus the absence of a
  baseline TypeDef token, emits the new TypeDef under the fresh module's ACTUAL name and
  chains that name into `TypeTokens`/`MethodTokens`/`FieldTokens`; the next session-less
  rebuild re-derives the identical `#g0` name (the derivation is a pure function of
  occurrence identity), so later generations resolve the added closure in place. The
  allocator's `#g{N}` fresh names materialize only in modules the allocator actually
  lowered. (With a lambda-free baseline — the crash scenario — `EncClosureNames` is empty
  and chains nothing, so the session tables stay vacuously consistent.)
- **The recapture guard does not (and must not) fire for force-rebuilt baselines.** A
  rebuilt on-disk dll whose sources now contain the edits carries only `#g0` names — it
  is indistinguishable from an original baseline because it IS one: a valid generation-0
  artifact for any session (re)started from it. The `#g{N>=1}` fail-closed guard targets
  mid-session RECAPTURE artifacts only. The LIVE session is unaffected by on-disk
  rebuilds because its baseline (assembly bytes, metadata snapshot, handle caches,
  token maps) is captured once at `StartHotReloadSession` and never re-read; the rebuilt
  dll enters delta emission only as the fresh compile.

The generation-2 crash itself was a fresh-vs-baseline ROW COORDINATE bug in the delta
emitter, not a naming bug: re-emitting rows of methods ADDED by an earlier generation
(the closure's `.cctor`/`.ctor`/`Invoke` become method updates of generation 2 via the
chained baseline tokens, which sit PAST the original tables) read their
attributes/signature/body/parameters from the fresh reader AT the chained baseline row —
but the fresh compile lays the closure out at its natural source position, displacing
every later row (here: the startup-init methods), so the wrong method's body was emitted
and the JIT rejected the `.cctor`. `tryBuildMethodUpdateInput` now reads the fresh module
through the fresh token recorded by `collectTypeMappings` (the baseline token remains the
emission row), pinned by the component test
`Added closure survives a second edit when the on-disk output is force-rebuilt between
generations`.

### State machines (builds on the same map)

#### Ground truth (flag-on compiles, inspected via ikdasm + live session probes)

- **`async`** lowers to closure chains: ~10 nested `FSharpFunc` subclasses for a two-bind
  CE (`compute@hotreload`, `compute@hotreload-1` … `-9`), including classes produced by
  inlined `AsyncPrimitives` internals — MORE closure classes than lambda occurrences,
  with legacy range-erased names (NOT occurrence-derived `#g0_oN` names). Consequences:
  body edits inside `async { }` work end-to-end (deterministic names align 1:1), but
  STRUCTURAL changes (added/removed `let!`, CE-level `try/with`) shift the `-N`
  numbering and the synthesized-type mapping fails closed with the
  "Ambiguous synthesized type mapping" `UnsupportedEdit` (observed live). The
  added-closure path cannot cover these classes because they are not 1:1 with
  occurrence-modelled lambdas (the stamp seam never sees the inlined internals).
- **`task`** lowers to ONE nested STRUCT state machine (`compute@hotreload`, extends
  `ValueType`): fields `Data`, `ResumptionPoint`, hoisted locals (`input`, `x`), and
  positionally named awaiters (`awaiter`, `awaiter0`, …); methods `MoveNext`,
  `SetStateMachine`, `get_ResumptionPoint`, `get_Data`, `set_Data`. Resume-point state
  numbers are assigned POSITIONALLY at lowering (`LowerStateMachines.genPC`, 1..N in
  conversion order). Observed live, before the emission backstops below existed: a body
  edit with stable resume points emitted, applied, and ran correctly; an ADDED `let!`
  emitted, applied, and then CRASHED at invoke (the emitter's compiler-generated-field
  skip silently dropped the new awaiter struct field). Reordered awaits failed closed
  via capture-set rude edits.
- **`seq`** lowers to a CLASS state machine via `GenSequenceExpression` (not
  ResumableCode); body edits apply end-to-end (`tier1-seq` test).

#### Classification

The old blanket `StateMachineShapeDigest` (a distinct-set of TryWith/TryFinally/While/
ForLoop ops + `MoveNext` valref names) was BOTH over-broad (a plain method gaining a
`while` loop, or an expression-level `try/with` inside `async { }`, was rude FSHRDL013)
and under-protective (it never caught the `task` added-await crash above). It is
replaced by typed resumable-code evidence:

- A member contains a genuine state machine iff its body calls members returning
  `ResumableCode<_,_>` (after abbreviation stripping — `TaskCode` etc.; matched by
  compiled identity because the diff compares trees across compilations), constructs a
  `ResumableCode` delegate directly, or makes SRTP trait calls returning resumable code.
- The digest is the ORDERED sequence of those calls with their type instantiations
  (`resumable=[Delay<int,int>(2),Bind<int,int,int>(2),Return<int>(2)]`): state numbers
  are positional and the struct's awaiter/hoisted layout follows the sequence, so order
  is part of the shape.
- Rules for resumable members (C# parity: `ChangingStateMachineShape`, FSHRDL013):
  - step sequence unchanged + lambda occurrence edits all `BodyEdited` → MethodBody
    update (MoveNext body flows through the existing method-update machinery; struct
    TypeDef, fields, and state numbers survive);
  - step sequence changed (added/removed/reordered `let!`/`do!`/`return!`/CE control
    flow, including non-suspending steps like an inserted `Zero`/`Combine` — documented
    over-approximation) → rude FSHRDL013 with the digest diff in the message;
  - sequence unchanged but structural lambda churn (added/removed continuations or
    capture-set changes) → rude FSHRDL013 (hoisted layout would change).
  - Append-only new awaits are NOT allowed, unlike C# Debug-mode async methods: C#'s
    Debug state machines are classes (new awaiter/hoisted fields ride
    `AddInstanceFieldToExistingType`); F# task state machines are structs, whose layout
    is immutable under EnC, so ANY resume-point addition is rude.
- Plain control flow (while/for/try) is no longer state-machine evidence anywhere:
  those constructs lower to ordinary IL in the containing (closure) body.
- `async` stays under the lambda occurrence model (its builder returns `FSharpAsync`,
  not `ResumableCode`): body edits (including new expression-level control flow) are
  MethodBody updates; structural CE changes keep failing closed at emission via the
  synthesized-type-mapping guard (precise `UnsupportedEdit`, see ground truth above).
- `seq { }` also stays under the lambda occurrence model (its desugaring is a closure
  chain at `--optimize-`, not ResumableCode): body edits apply as MethodBody updates and
  an ADDED yield rides the added-closure path (requires `NewTypeDefinition` +
  `AddMethodToExistingType`; fresh enumerations observe the new yields — pinned by
  `ApplyUpdate succeeds for seq body edit with added yield`). Documented caveat,
  deliberately weaker than C# (which reports `ChangingStateMachineShape` for iterator
  edits): an enumerator already SUSPENDED mid-sequence at apply time resumes on the new
  code with its old state; F# accepts the edit because the lowering regenerates the
  whole closure chain consistently for fresh enumerations.

#### Emission backstops (`IlxDeltaEmitter.collectTypeMappings`)

Classification runs on typed trees; the delta emitter independently guards the lowered
artifacts (defense in depth, and the only guard in skew scenarios such as a disk-started
session whose on-disk binary diverges from the baseline source):

- **Struct layout gate**: a fresh instance field on an EXISTING struct raises a precise
  `UnsupportedEdit` BEFORE the compiler-generated-name skip (which previously swallowed
  it). For compiler-generated structs the message names the state machine and the
  changed resume-point/hoisted layout — this was the exact hole behind the task
  added-await runtime crash described above.
- **Injective synthesized-type mapping**: the new→baseline type-name mapping must be
  1:1; two fresh closures alias-matching one baseline class (legacy `-N` numbering
  shift after a structural async CE change) fail closed instead of patching the wrong
  rows.
- **Legacy closure fallthrough**: a non-generation-suffixed `@hotreload` type with no
  baseline counterpart raises a precise "closure chain cannot be aligned" message
  instead of falling through to garbage baseline token lookups.

#### EnC State Machine State Map persistence

The Roslyn-format state map blob (`serializeStateMachineStates`) is emitted at baseline
and read back:

- `LowerStateMachines` surfaces the conversion's resume points on
  `LoweredStateMachine.resumptionPoints` — (state number, source range) in
  state-number order, the AUTHORITATIVE numbering (`genPC`), not a typed-tree guess.
- `IlxGen.GenStructStateMachine` records the state numbers into the closure-name
  recording seam (`ClosureNameAllocationState.recordStateMachineResumePoints`), keyed by
  the emitted struct's full type name; recording shares the closure-name recording
  lifecycle (capture compiles only, flag-off byte-identical).
- The fsc emit path joins recordings to members by the struct simple name's basic name
  (`{member}@hotreload…` → `{member}` = the member's compiled name, this conduit's key),
  failing closed on collisions (same-named members, nested CEs lowering several
  machines in one member); the PDB writer additionally drops any name that does not
  identify exactly one IL method row.
- Blob semantics: `StateNumber` = the positional pc (1..N); the syntax-offset slot
  carries the resume point's ORDINAL (the same philosophy as the lambda-map occurrence
  keys — deterministic ints, not source offsets). Decodable by Roslyn-format tooling.
- The baseline read side (`readEncMethodDebugInfoFromPortablePdb` →
  `FSharpEmitBaseline.EncMethodDebugInfos`) decodes the rows for disk-started sessions.

Deviations from Roslyn, stated precisely: (1) rows attach to the KICKOFF member's
MethodDef, not MoveNext — F# state-machine MoveNexts share one name and the writer
conduit is name-keyed, while every consumer addresses members; (2) disk-started-session
CLASSIFICATION remains source-derived (the session re-typechecks the baseline source,
so the resumable-call digests exist without the CDI), with binary skew guarded by the
struct-layout emission gates; CDI-driven cross-checking of fresh-vs-persisted state
counts at emission is deferred until a scenario demands it.

### Resulting rude-edit surface (parity with C#)

Allowed: adding/removing/reordering lambdas; editing lambda bodies; editing bodies inside
`async`/`task`/`seq` CEs when the CE structure is unchanged; plain control-flow additions
anywhere. Still rude: captured-variable rename/type/scope change, lambda signature
changes, ANY resume-point sequence change in resumable (task) members (struct layout),
structural `async` CE changes (closure-chain alignment, fails closed at emission),
struct-closure capture additions.

## Open questions

1. ~~Occurrence alignment tolerance~~ — resolved: full structural digest first, shape-only digest
   second; positional fallback only between indistinguishable occurrences.
2. F# `let` local functions: lowered as methods or closures depending on capture/inline decisions —
   the occurrence model keys off the typed tree the diff already walks; the lowering decision is
   replayed via the name-map state where occurrence identity threads into IlxGen.
3. Quotation-bearing code (`QueryExpressionShapeChange`): kept rude (members containing
   `Expr.Quote` are excluded from occurrence modelling); revisit as added-lambda emission
   coverage broadens.
4. Whole-body hash sensitivity: the FNV/XOR combination in `exprDigest` treats swaps of adjacent
   independent `let` bindings as identical (XOR commutes), so a pure reorder may produce no
   MethodBody edit at all. Pre-existing behavior, documented by the occurrence-reorder test;
   tighten when the hash is next revised.
