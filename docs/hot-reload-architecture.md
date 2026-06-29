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

## The default-session compatibility shim — RETIRED

The pre-entity `FSharpChecker` surface (`StartHotReloadSession`, `EmitHotReloadDelta`,
`EndHotReloadSession`, `UpdateHotReloadCapabilities`, `SetHotReloadActiveStatements`,
`HotReloadSessionActive`) and the checker-held default session store behind it are gone:
`CreateHotReloadSession` is the only way to obtain hot reload behaviour from a checker, and
the process-wide store registration (`HotReloadState.setSessionStore`) was deleted with the
shim. All FCS test suites and the demo app drive the session-object API. (The sdk reflection
bridge still needs its move to the session-object shape; that lives outside this repo.)

### Emit-hook ownership after retirement

With no ambient registration, the fsc emit hook resolves its owner per compile:

- **In-process DELTA compiles** (the host rebuilding a session-tracked project through
  `FSharpChecker.Compile`): each session registers the resolved output path of every project
  it baselines (`AddProject`) with its owning checker; `Compile` matches a non-capture
  compile's output path against that registry, arms `--test:HotReloadHook`, and sets a
  SCOPED EMISSION CONTEXT (`HotReloadState.setCurrentEmissionContext`: the session's store +
  the project key) around the compile. The hook prefers the scoped context, so the
  closure-name allocator and synthesized-name replay run against the EMITTING session's
  chained tables — and the hook never ends a scoped session's state (sessions own their
  lifecycle; the legacy clear-on-build stays for context-less compiles). The most recently
  baselined project wins when several live sessions track the same output.
- **Flag-on BASELINE capture compiles** are session-independent: they read config + typed
  tree and write artifacts (the deterministic dll/pdb) plus side-channels. Capture compiles
  never run under a scoped context; the captured baseline is published to the process-local
  module store in `HotReloadState` (a capture slot serving capture-to-capture name chaining
  within one host, standalone-fsc validation and unit-level tests), which is never any
  session's store. Creating a checker resets the slot — the freshness property the retired
  per-checker store registration used to provide — so one owner's captures never chain
  against another's. Sessions reconstruct baselines from the on-disk dll + pdb and never
  read the capture slot.
- `ClosureNameAllocationState` / compiler-generated-name-map side channels stay keyed by
  `CompilerGlobalState` via `ConditionalWeakTable` — per-compile by construction, no
  per-session keying needed.

## Determinism pins for baseline capture

A hot reload baseline must be byte-reproducible: a capture compile of identical source in
another process (the dotnet-watch topology) has to reproduce the exact row/heap layout the
running process baselined, or every chained delta resolves against the wrong tokens. The
per-compile hot reload machinery is deterministic by construction — occurrence keys are
syntactic, emission walks list-ordered typed trees, and the delta emitter hardcodes
deterministic output — so the one real exposure is the BASELINE compile itself, where the
compiler's default parallelism can permute output between runs.

`--test:HotReloadDeltas` therefore silently pins, at config finalization in
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
