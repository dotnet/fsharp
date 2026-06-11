# F# Hot Reload: Active Statements & Sequence Point Updates (Phase G)

Status: landed (2026-06-11) — model types, per-edit sequence-point/line-update computation,
active-statement remapping, and the delta-surface exposure a debugger host consumes.
Source research: Roslyn main @ June 2026 (`src/Features/Core/Portable/Contracts/EditAndContinue/*`,
`ActiveStatement.cs`, `ActiveStatementsMap.cs`, `EditSession.cs`,
`AbstractEditAndContinueAnalyzer.cs` `GetLineEdits`/`AnalyzeUnchangedActiveMemberBodies`).

## Scope

This phase lands the COMPILER-side active-statement machinery: what Roslyn's
`WatchHotReloadService`/`EmitSolutionUpdate` carries on every update even in watch mode. Full
debugger-host integration (VS/vsdbg wire-up, `IManagedHotReloadService` plumbing) is out of local
scope; see "Host integration notes" at the end for what a host has to do.

## Model: Roslyn contract mapping

Public types in `src/Compiler/HotReload/ActiveStatements.fs` (namespace
`FSharp.Compiler.CodeAnalysis`), mirroring `Microsoft.CodeAnalysis.Contracts.EditAndContinue`
field-for-field where semantics match:

| F# type | Roslyn contract type | Notes |
|---|---|---|
| `FSharpSourceSpan` | `SourceSpan` | Zero-based lines AND columns (one less than the 1-based Portable-PDB sequence-point coordinates). F# PDBs always carry columns, so the `-1 = missing column` convention is not modeled. |
| `FSharpManagedModuleMethodId` | `ManagedModuleMethodId` | Token + 1-based version. Roslyn's `ManagedMethodId` adds the module MVID; an F# session is single-module, so module identity is implicit (documented deviation). |
| `FSharpManagedInstructionId` | `ManagedInstructionId` | Method id + IL offset. |
| `FSharpActiveStatementFlags` + `FSharpActiveStatementFrameKind` | `ActiveStatementFlags` | The `LeafFrame`/`NonLeafFrame` bits become a closed union (`Leaf \| NonLeaf \| LeafAndNonLeaf`) so "neither leaf nor non-leaf" is unrepresentable; `MethodUpToDate`, `PartiallyExecuted`, `NonUserCode`, `Stale` map to independent booleans (they are genuinely independent). |
| `FSharpManagedActiveStatementDebugInfo` | `ManagedActiveStatementDebugInfo` | Instruction + PDB document name (option) + span + flags. |
| `FSharpSourceLineUpdate` | `SourceLineUpdate` | Zero-based old line -> new line. |
| `FSharpSequencePointUpdates` | `SequencePointUpdates` | Per-PDB-document line updates, sorted by old line; zero-delta entries terminate shifted ranges exactly like Roslyn's. |
| `FSharpManagedActiveStatementUpdate` | `ManagedActiveStatementUpdate` | Method id BEFORE the change (echoed from the debugger), old IL offset, new span. |
| `FSharpActiveStatementRemapResult` | — (F#-shaped) | Roslyn's `ManagedHotReloadUpdate.ActiveStatements` carries only remapped statements; F# also reports `MethodUpToDate of FSharpManagedInstructionId` explicitly so hosts/tests can assert untouched methods. |

Delta surface (`FSharpHotReloadDelta`, service.fsi):

- `SequencePointUpdates: FSharpSequencePointUpdates list` — mirrors
  `ManagedHotReloadUpdate.SequencePoints`.
- `ActiveStatementUpdates: FSharpActiveStatementRemapResult list` — mirrors
  `ManagedHotReloadUpdate.ActiveStatements` (plus the explicit `MethodUpToDate` entries).
- `ManagedHotReloadUpdate.ExceptionRegions` has NO F# counterpart yet (see "Deferred").

## Supplying active statements (API shape)

`FSharpChecker.SetHotReloadActiveStatements: FSharpManagedActiveStatementDebugInfo seq -> bool`
(session-scoped setter; REPLACES the whole set; empty clears; false when no session).

Rationale: Roslyn's `EmitSolutionUpdate(solution, activeStatementSpanProvider)` PULLS the debugger's
break state per edit session (`DebuggingSession` queries
`IManagedHotReloadService.GetActiveStatementsAsync` lazily). FCS has no callback seam into the
host, so the fetch inverts to a push: the host reports the break state whenever the debugger stops,
and the next `EmitHotReloadDelta` consumes it. A session setter (rather than an
`EmitHotReloadDelta` overload parameter) keeps the two `EmitHotReloadDelta` overloads stable and
matches the session-scoped lifetime of the break state.

## Sequence-point updates (line shifts)

The typed-tree diff's hashes are deliberately range-independent (C1: ranges are diagnostics-only,
never identity), so an edit that only MOVES code produces no semantic edits. Confirmed and wired:
the line updates come from sequence-point comparison alone.

Pipeline (all inside `IlxDeltaEmitter.emitDeltaWithDebugData` + `ActiveStatementAnalysis`):

1. **Committed view** — `FSharpEmitBaseline.SequencePointSnapshots: Map<MethodDef token,
   MethodSequencePoints>` is the debugger's current view of each method's lines. Seeded from the
   baseline portable PDB at session start (sibling-read from disk in the checker flow, because
   `readIlModule` attaches no debug points) and REPLACED wholesale with the fresh compile's points
   after every committed update. This matches Roslyn diffing line edits against the
   `CommittedSolution`: each generation's updates are relative to what the debugger last applied,
   and they compose across generations.
2. **Fresh view** — decoded from the fresh compile's on-disk PDB (passed down from
   `EmitHotReloadDelta` through `EmitDeltaForCompilation`/`EmitDelta`), falling back to the
   emitter's in-memory PDB for callers that construct modules with debug points. The same bytes
   feed the emitted PDB delta, which therefore carries real sequence points in the checker flow.
3. **Per-method classification** (`compareMethodSequencePoints`, every baseline-matched method not
   already recompiled by the request):
   - `Identical` — nothing to do (still contributes a zero-delta segment for overlap detection,
     Roslyn parity).
   - `UniformLineShift d` — same IL offsets, columns and line extents, every visible point moved
     by `d` lines: a line-edit segment.
   - `Different` — anything else (a line split inside a body, column changes, document changes):
     the method body is RECOMPILED in the delta so its debug information stays accurate. This is
     the analog of Roslyn's trivia edits forcing a member body update
     (`AbstractEditAndContinueAnalyzer` reports `requiresUpdate` and converts the member to a
     semantic edit).
4. **Segment merge** (`mergeLineShiftSegments`) — verbatim port of Roslyn `GetLineEdits`: sort by
   (document, old start line); zero-delta reset entries terminate shifted ranges; consecutive
   equal deltas collapse; segments overlapping a previous segment with a DIFFERENT delta cannot be
   expressed as line updates and fall back to recompilation of that method ("the debugger does not
   apply line deltas to recompiled methods").

### Line-shift-only deltas

Before Phase G a line-shift edit emitted `NoChanges` (the debugger's lines went silently stale).
Now `EmitDeltaForCompilation` always runs the emitter and decides from the artifacts:

- semantic/trivia changes -> normal delta (line updates ride along for the methods that only moved);
- ONLY line shifts -> a delta carrying ONLY `SequencePointUpdates`: `Metadata`/`IL` empty,
  `UpdatedMethods` empty, `GenerationId = Guid.Empty`, NO generation consumed. The committed
  sequence-point view and the session's implementation files advance
  (`HotReloadSessionStore.UpdateCommittedSequencePoints`).
- nothing at all -> `NoChanges`, as before.

**Documented deviation**: Roslyn emits a Module-row-only EnC generation for line-only updates (its
`EmitDifference` always runs); the F# emitter deliberately skips the no-op `ApplyUpdate` — there is
nothing for the runtime to apply, hosts just rebind the debugger's lines. If byte-parity with
Roslyn's generation chain ever matters here, the empty-update emit path is the place to add the
Module-row-only delta.

Also note: emits with NO semantic edits no longer register unmatched fresh definitions as
additions — regeneration noise (e.g. a generative type provider re-emitting its members under a
changed static argument with unchanged consumed IL) previously hid behind the `NoChanges`
short-circuit and would otherwise materialize into spurious delta rows.

## Active statement remapping

`ActiveStatementAnalysis.remapActiveStatement(s)`, invoked by
`FSharpEditAndContinueLanguageService.EmitDelta` after the emitter returns and BEFORE any session
state is staged (a rude outcome blocks the whole update and leaves the session at the previous
generation, surfacing as `FSharpHotReloadError.UnsupportedEdit`).

Per supplied statement:

- containing method NOT recompiled by the delta -> `MethodUpToDate` (line shifts, if any, are
  covered by `SequencePointUpdates`; Roslyn likewise leaves these to the line deltas).
- containing method recompiled -> resolve the statement's IL offset to the last visible committed
  sequence point at or before it, and map BY ORDINAL to the fresh visible points when both sides
  have the same visible-point count; the result is
  `Remapped { Method = (echoed debugger id); ILOffset = (old offset); NewSpan = (fresh span) }`.

Conservative-but-honest rude fallbacks (F# has no Roslyn syntax map — the diff is typed-tree
based — so ambiguous sequence-point alignment fails TOWARD rude, Roslyn parity for the
statement-destroying classes `ActiveStatementUpdate`/`DeleteActiveStatement`):

| Condition | Outcome |
|---|---|
| visible-point counts differ (statement added/deleted at or around the active statement) | rude |
| NON-LEAF frame and the aligned span changed by anything other than a pure line shift (same columns + line extent) | rude (the runtime cannot remap a non-topmost frame; LEAF frames may move freely — the debugger remaps them) |
| statement is `Stale` or not `MethodUpToDate` and its method is updated again | rude (the executing version is older than the committed view; alignment would be a guess) |
| `PartiallyExecuted` statement in an updated method | rude (Roslyn: partially executed statements can't be edited) |
| no committed or no fresh sequence points for an updated method (no PDB, lost debug info) | rude |
| IL offset resolves into a hidden region or before the first sequence point | rude |

Note the ordinal alignment is stricter than Roslyn: adding a statement anywhere in a method with an
active statement is rude in F# (Roslyn tracks the statement through the syntax map and allows it).
Loosening this requires source-span tracking of the supplied statement spans against the typed
tree — see "Deferred".

## Where things live

- `src/Compiler/HotReload/ActiveStatements.fs` — public model + `ActiveStatementAnalysis`
  (decode/classify/merge/remap).
- `src/Compiler/CodeGen/HotReloadBaseline.fs` — `FSharpEmitBaseline.SequencePointSnapshots`.
- `src/Compiler/CodeGen/IlxDeltaEmitter.fs` — `emitDeltaWithDebugData`, in-emitter analysis,
  trivia-recompile injection, committed-view chaining, `IlxDelta.SequencePointUpdates` /
  `ChainedSequencePoints` / `ActiveStatementUpdates`.
- `src/Compiler/HotReload/EditAndContinueLanguageService.fs` — emit-or-NoChanges decision,
  line-only commit, remap + rude gating.
- `src/Compiler/HotReload/HotReloadState.fs` — session `ActiveStatements`,
  `UpdateActiveStatements`, `UpdateCommittedSequencePoints`.
- `src/Compiler/Service/service.fs(i)` — `FSharpHotReloadDelta.SequencePointUpdates` /
  `ActiveStatementUpdates`, `FSharpChecker.SetHotReloadActiveStatements`, on-disk PDB plumbing.
- `tests/FSharp.Compiler.Service.Tests/HotReload/ActiveStatementTests.fs` — line-shift (incl.
  disk-restarted session), remap-to-new-span cross-checked against the delta PDB's sequence-point
  table, MethodUpToDate, delete-statement rude, non-leaf-edit rude.

## Host integration notes (future watch/debugger wiring)

- **dotnet-watch (no debugger)**: never call `SetHotReloadActiveStatements`; consume
  `SequencePointUpdates` to decide that a line-only change needs NO restart and NO `ApplyUpdate`
  (`Metadata` empty). Watch's `HotReloadService` forwards the updates to the agent unchanged.
- **Debugger host**: on break, translate the debugger's active statement info
  (`ICorDebugFunction` token/version + IL offset + PDB span) into
  `FSharpManagedActiveStatementDebugInfo` and push it; after a successful emit, apply
  `SequencePointUpdates` (rebind sequence points/breakpoints) and feed
  `ActiveStatementUpdates`' `Remapped` entries to the debugger's IP remapper; on
  `UnsupportedEdit` mentioning active statements, surface Roslyn's "restart required" UX.
- Line updates compose: each delta's `OldLine` refers to the lines as of the PREVIOUS applied
  update (Roslyn `CommittedSolution` semantics), not to generation 0.
- The committed view survives process restarts only at generation 0 (decoded from the on-disk
  PDB); mid-session restore inherits the C2 limitation that delta PDBs are not re-read on restore
  of a mid-chain session.

## Deferred

- `ManagedExceptionRegionUpdate` (exception-region tracking and non-remappable regions) — Roslyn
  computes these from syntax; F# would derive them from the IL exception-region tables of old vs
  new bodies.
- Span-based statement tracking to relax the ordinal-alignment rude edits (statement insertion in
  a method with an active statement).
- Module-row-only EnC generation for line-only updates (Roslyn byte-parity).
- VS/vsdbg `IManagedHotReloadLanguageService` adapter exposing these payloads over the contract
  types.
