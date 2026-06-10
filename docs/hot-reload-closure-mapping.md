# F# Hot Reload: Closure & State-Machine Mapping Design (Phase C/D)

Status: C1 landed (2026-06-10) — lambda occurrence model and alignment in `TypedTreeDiff`.
C2 commit 1 landed (2026-06-10) — EnC CDI blob encoder/decoder (`EncMethodDebugInformation.fs`);
PDB writer/reader wiring pending. C3–C5 pending. Implementation notes are folded into the
relevant sections below.
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
  against Roslyn-emitted blobs (see "C2 blob encoder/decoder as implemented"). Remaining: writer
  wiring in ilwritepdb, reader in the baseline snapshot.
- **C3** `feat(hot-reload): generation-aware closure identity in IlxGen lowering` — `ClosureSlotAllocator`
  seam; matched occurrences reuse closure class/method/field identity; new occurrences get
  generation-suffixed names. Flag-off behavior byte-identical (EmittedIL gate).
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
