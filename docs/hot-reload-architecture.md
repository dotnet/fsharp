# Hot reload architecture: the session entity model

Status: implemented on `hot-reload-v2` (June 2026). This documents the entity model the F#
hot reload (Edit and Continue) support is built around, the snapshot-contract decision, and
how the entities map onto Roslyn's.

## The entity model

One `FSharpHotReloadSession` exists per host session (a `dotnet watch` run, a debug session).
It is created from a checker (`FSharpChecker.CreateHotReloadSession`) and returned as an
object the host holds and disposes. Sessions are independent instances: a second session does
not clobber the first, and you cannot emit a delta without a session object that tracks the
project — illegal states are unrepresentable.

```
FSharpHotReloadSession                        (one per watch/debug session; IDisposable)
├── CommittedProjects:  Map<FSharpProjectIdentifier, CommittedProject>
│     └── CommittedProject = { Snapshot view (typed impl files)       // committed diff inputs
│                              Baseline: FSharpEmitBaseline           // keyed to loaded Mvid
│                              Generation chain (CurrentGeneration,
│                                PreviousGenerationId, PendingUpdate)
│                              Chained CDI / closure-name /
│                                sequence-point state                 // rides on the baseline }
├── Capabilities:       EditAndContinueCapabilities                   (session-wide, updatable)
├── ActiveStatements:   per-statement debug infos                     (session-wide, host-pushed)
└── Operations:
      AddProject    : FSharpProjectSnapshot * ?outputPath -> ...      (baseline capture)
      EmitDelta     : FSharpProjectSnapshot -> Result<Delta, Error>   (diff vs committed)
      Commit/Discard: solution-wide (all pending project updates together — Roslyn semantics,
                      prevents cross-project inconsistency)
      UpdateCapabilities / SetActiveStatements : session-wide
```

Internally the session owns a `HotReloadSessionStore` instance (`HotReloadState.fs`): a map of
per-project slots keyed by `HotReloadProjectKey` (the internal mirror of
`FSharpProjectIdentifier`: `projectFileName * outputFileName`), plus the session-wide
capability set and active statements. The per-project chained state — EnC method debug
information (CDI occurrence chains), closure-name tables, committed sequence points — rides on
`FSharpEmitBaseline`, so keying the baseline per project keys all of it; nothing in the delta
engine below `EmitDeltaForCompilation` changed.

### Emit / commit lifecycle

`EmitDelta` diffs the fresh snapshot (typed trees + rebuilt output assembly) against the
project's COMMITTED baseline and stages the result as the project's pending update. After the
host applies the update (`MetadataUpdater.ApplyUpdate`), `Commit()` advances every pending
project update atomically — committed baselines, diff inputs (implementation files), and
generation counters move together. `Discard()` drops all pending updates so the next emit
re-diffs against the unchanged committed view. This is Roslyn's
`EmitSolutionUpdate` / `CommitSolutionUpdate` / `DiscardSolutionUpdate` split; partial
cross-project commits are unrepresentable.

Line-shift-only updates (no metadata/IL, no generation consumed) commit their rebound
sequence-point view immediately in both flows — there is nothing to pass to `ApplyUpdate`, so
there is nothing to stage.

## The snapshot contract decision

The session depends on the snapshot CONTRACT, not the workspace container. It takes
`FSharpProjectSnapshot`s — immutable, content-hash versioned, composable, with recursive
project-to-project references — and `FSharpWorkspace` remains an optional host adapter that
manufactures snapshots from file events. If the experimental workspace API shifts, the session
is untouched; a host with no workspace (Ionide, another LSP server) hands us snapshots some
other way. This mirrors Roslyn's `ISolutionSnapshotProvider` seam (roslyn #82905): the host
provides snapshots; the EnC service consumes them. MSBuild stays the authority on project
discovery and evaluation — F# does not own a project system, and the session does not change
that.

## Roslyn mapping

| Roslyn (EnC) | F# hot reload | Notes |
|---|---|---|
| `DebuggingSession` | `FSharpHotReloadSession` | One per debug/watch session; holds committed views and baselines |
| `CommittedSolution` | per-project committed snapshot view + `Map<FSharpProjectIdentifier, CommittedProject>` | F# composes recursively per project; no solution-level snapshot type needed for EnC |
| `_projectBaselines: ProjectId → ProjectBaseline` | `CommittedProjects: HotReloadProjectKey → HotReloadProjectState` (baseline + generation chain) | Keyed by `FSharpProjectIdentifier` (projectFileName * outputFileName) |
| `EmitBaseline` | `FSharpEmitBaseline` | Carries the chained CDI/closure-name/sequence-point state per project |
| `EditAndContinueCapabilities` (session-level) | session-wide `EditAndContinueCapabilities` | Updatable mid-session (`UpdateCapabilities`) |
| `GetActiveStatementsAsync` (debugger pull) | `SetActiveStatements` (host push) | FCS has no callback seam into the host, so the host pushes the break state before emitting |
| `EmitSolutionUpdate` | `EmitDelta(snapshot)` | Per-project diff against the committed view; result staged as pending |
| `CommitSolutionUpdate` / `DiscardSolutionUpdate` | `Commit()` / `Discard()` | Solution-wide across all pending project updates |
| `ISolutionSnapshotProvider` | the `FSharpProjectSnapshot` parameter | Hosts provide snapshots; the session consumes them |

## The default-session compatibility shim

The pre-entity `FSharpChecker` surface (`StartHotReloadSession`, `EmitHotReloadDelta`,
`EndHotReloadSession`, `UpdateHotReloadCapabilities`, `SetHotReloadActiveStatements`,
`HotReloadSessionActive`) keeps working unchanged by delegating to a DEFAULT session store
held by the checker:

- the default store is registered as the process-wide store
  (`HotReloadState.setSessionStore`), so the module-level helpers and the fsc emit hook —
  which has no project identity and writes baseline captures into an `Ambient` slot — route
  to it;
- `StartHotReloadSession` keeps its replace-existing semantics (starting a session resets the
  default session to a single project, now keyed by the derived project identity);
- identity-less operations resolve against the most recently started project slot, which for
  single-project flows is exactly the previous one-session behaviour;
- `EmitHotReloadDelta` on the compatibility surface still auto-commits each successful emit
  (the session entity defers to `Commit()`/`Discard()` instead).

Two pieces of state remain genuinely process-global and route to the default session only:

1. the process-wide active store the fsc emit hook (`FSharpEditAndContinueLanguageService
   .Instance`) and `HotReloadState` module helpers consult — the hook's synthesized-name
   replay and closure-name allocator installation during in-process compiles therefore serve
   the DEFAULT session, not entity sessions (method-body edits do not need them; closure-name
   chaining for entity sessions through the in-process compile hook is a follow-up);
2. `ClosureNameAllocationState` / compiler-generated-name-map side channels keyed by
   `CompilerGlobalState` via `ConditionalWeakTable` — these are per-compile by construction
   and need no per-session keying.

**Retirement**: the shim exists so the ~600 pre-entity tests and the sdk reflection bridge
keep working during the restructure. Before upstream PR-2 (the session-entity PR), the
static-shaped checker members are retired in favour of `CreateHotReloadSession` (they are
`[<Experimental>]`, so there is no compatibility obligation), the sdk bridge moves to the
session-object shape, and the process-wide store registration goes away with them — at which
point the emit hook needs an explicit owner (the session whose compile is running) rather
than the ambient store.

## Determinism pins for baseline capture

A hot reload baseline must be byte-reproducible: a capture compile of identical source in
another process (the dotnet-watch topology) has to reproduce the exact row/heap layout the
running process baselined, or every chained delta resolves against the wrong tokens. The
per-compile hot reload machinery is deterministic by construction — occurrence keys are
syntactic, emission walks list-ordered typed trees, and the delta emitter hardcodes
deterministic output — so the one real exposure is the BASELINE compile itself, where the
compiler's default parallelism can permute output between runs.

`--enable:hotreloaddeltas` therefore silently pins, at config finalization in
`Driver/fsc.fs` (`main1`, after all flags are processed):

- `deterministic <- true` — stable MVID/timestamp, deterministic PE emission (upstream MVID
  determinism, dotnet/fsharp #19801, is already in the base);
- `parallelIlxGen <- false` — parallel IlxGen's name-set merge ordering can permute
  synthesized closure/type rows between identical compiles (same family as dotnet/fsharp
  #19732 and #19928);
- `optSettings.processingMode <- Sequential` — parallel optimization can reorder the
  optimized method bodies feeding codegen.

These are silent flag-implies-flag pins, not user-facing errors: msbuild cannot reliably
switch parallelism off itself (dotnet/fsharp #19935), so the user has no meaningful way to
choose otherwise, and under the already-required `--debug+ --optimize-` the pins have no
optimization cost. Graph type-checking (`TypeCheckingMode.Graph`) is deliberately left as
configured: lambda occurrence keys and typed-tree emission order derive from the
compilation's file order, not from the order files are checked in, so checking-order
parallelism cannot perturb captured output.
