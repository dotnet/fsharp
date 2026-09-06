# File watching in FSharp.Editor

`vsintegration/src/FSharp.Editor/LanguageService/FileChangeWatcher.fs` is the F# counterpart of
Roslyn's `FileChangeWatcher` and `ReferenceFileChangeTracker` (both internal to
`Microsoft.VisualStudio.LanguageServices` and not exposed through `ExternalAccess.FSharp`).

## What the workspace already gives us

Not every on-disk change needs this watcher. A `-r:` that the Roslyn workspace holds as a
`MetadataReference` is already watched by Roslyn: `ProjectSystemProjectFactory` advises every
reference path, and when one changes it swaps the reference on the solution, which bumps
`Project.Version`. `FSharpProjectOptionsReactor` sees that version through `isProjectInvalidated`,
recomputes, and calls `InvalidateConfiguration` — measured in VS, that path wins the race against
a watcher subscribed to the same file, because Roslyn batches over 500 ms where this tracker
additionally debounces for 2 s.

So a second subscription to the same reference set buys nothing. What the workspace does *not*
cover is everything it has no document or reference for, and every stat FCS still performs
internally:

- `#load` sources of a script: not documents, not references, invisible to the workspace.
- `IsReferencesInvalidated` on the incremental builder, which stats every reference on every
  request.
- `ReferencesOnDisk` on snapshot reuse, which does the same per comparison.

## Shape

- **Service.** `IVsAsyncFileChangeEx2`, obtained asynchronously; nothing ever blocks on the UI
  thread. Callbacks arrive through `IVsFreeThreadedFileChangeEvents2` and stay on background
  threads.
- **Batching.** Subscribe/unsubscribe operations go through a single-consumer queue with a 500 ms
  window (Roslyn's empirical value for solution open/close). Consecutive operations of the same
  kind, and for file watches the same sink, are coalesced into one service call.
- **Directory watches.** A context starts with recursive `.dll` watches on the places reference
  assemblies live: `DOTNET_ROOT/packs` and the machine-wide `dotnet/packs`, the .NET Framework
  reference assemblies, and the NuGet cache (`NUGET_PACKAGES` or `~/.nuget/packages`). A file
  under one of them costs no cookie of its own. Roslyn does not watch the NuGet cache; a consumer
  that watches every `-r:` uniformly wants it, since package assemblies are the bulk of them and
  the alternative is a per-file advise for each.
- **Per-file watches.** Paths outside those directories (project outputs, loose assemblies) get
  an individual advise, ref-counted across consumers by `FSharpReferenceChangeTracker`.
- **Debounce.** A rebuild writes a temp file and renames it, producing several notifications; the
  tracker fires one callback per path after 2 s of quiet.

## Consumers

None yet — this is the transport, added on its own so the changes that need it stay reviewable:

1. Scripts: watch `#load` sources (and the script's own `#r` set) so an edit outside the editor
   drops the cached options for that document.
2. A watcher-invalidated timestamp cache serving `ReferencesOnDisk`, replacing the stat per
   reference per snapshot comparison.
3. A reference-change notification for the incremental builder on the FCS side, the analogue of
   `useChangeNotifications` for sources, so `IsReferencesInvalidated` stops stat'ing at all.
