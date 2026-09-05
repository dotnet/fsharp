# File watching in FSharp.Editor

`vsintegration/src/FSharp.Editor/LanguageService/FileChangeWatcher.fs` is the F# counterpart of
Roslyn's `FileChangeWatcher` and `ReferenceFileChangeTracker` (both internal to
`Microsoft.VisualStudio.LanguageServices` and not exposed through `ExternalAccess.FSharp`).

## Why a watcher

The Roslyn workspace tracks documents, not the `-r:` references an F# project compiles against.
When a referenced assembly is rebuilt outside VS nothing tells the F# language service; FCS only
notices because it stats every reference again on the next request (`IsReferencesInvalidated` on
the incremental builder, `ReferencesOnDisk` when a snapshot is reused). The watcher turns that into
a push: one notification per changed path, delivered to the projects that reference it.

## Shape

- **Service.** `IVsAsyncFileChangeEx2`, obtained asynchronously; nothing ever blocks on the UI
  thread. Callbacks arrive through `IVsFreeThreadedFileChangeEvents2` and stay on background
  threads.
- **Batching.** Subscribe/unsubscribe operations go through a single-consumer queue with a 500 ms
  window (Roslyn's empirical value for solution open/close). Consecutive operations of the same
  kind, and for file watches the same sink, are coalesced into one service call.
- **Directory watches.** Each context starts with recursive `.dll` watches on the places
  reference assemblies live: `DOTNET_ROOT/packs` and the machine-wide `dotnet/packs`, the .NET
  Framework reference assemblies, and the NuGet cache (`NUGET_PACKAGES` or `~/.nuget/packages`).
  A file under one of them costs no cookie of its own. Roslyn does not watch the NuGet cache; we
  do because every `-r:` is watched uniformly and package assemblies are the bulk of them, so the
  alternative is a per-file advise for each.
- **Per-file watches.** Paths outside those directories (project outputs, loose assemblies) get
  an individual advise, ref-counted across projects by `FSharpReferenceChangeTracker`.
- **Debounce.** A rebuild writes a temp file and renames it, producing several notifications; the
  tracker fires one callback per path after 2 s of quiet.

## Consumer

`FSharpProjectOptionsReactor` watches the `-r:` set of every project it computes options for and
calls `FSharpChecker.InvalidateConfiguration` for each project that references a changed path.
The cached options stay valid (same paths); only the FCS build behind them is stale. Watch sets
are diffed on recompute, so an unchanged reference list touches nothing.

## Follow-ups

1. A reference-change notification for the incremental builder on the FCS side, the analogue of
   `useChangeNotifications` for sources, so `IsReferencesInvalidated` stops stat'ing every
   reference on every request.
2. A watcher-invalidated timestamp cache for snapshot reuse (`ReferencesOnDisk`).
3. Scripts: watch `#r` references and `#load` sources the same way.
