# File watching in FSharp.Editor

`vsintegration/src/FSharp.Editor/LanguageService/FileChangeWatcher.fs` is the F# counterpart of
Roslyn's `FileChangeWatcher` and `ReferenceFileChangeTracker` (both internal to
`Microsoft.VisualStudio.LanguageServices` and not exposed through `ExternalAccess.FSharp`).

## What the workspace already gives us

Not every on-disk change needs this watcher. A `-r:` that the Roslyn workspace holds as a
`MetadataReference` is already watched by Roslyn: `ProjectSystemProjectFactory` advises every
`PortableExecutableReference` path, and when one changes it swaps the reference on the solution,
which bumps `Project.Version`. `FSharpProjectOptionsReactor` sees that version through
`isProjectInvalidated`, recomputes, and calls `InvalidateConfiguration`. Every source file is a
Roslyn document too, the `#load` sources of a script included (`FSharpMiscellaneousFileService`
gives each one a project context), and Roslyn's own document watcher reloads them.

What the workspace does *not* cover:

- The output dll of a project reference. Roslyn holds it as a `ProjectReference`, not a
  `MetadataReference`, and watches no file for it; the `-r:` built from
  `project.ProjectReferences` is watched only here.
- Every stat FCS performs internally: `IsReferencesInvalidated` on the incremental builder, which
  stats every non-framework reference on every request; `FSharpProjectSnapshot.FromOptions`, which
  stats every `-r:` when a snapshot is built from scratch; `ReferencesOnDisk` on snapshot reuse,
  which did the same per comparison — the consumer below.
- A `#r` or `#load` directive added to a `#load`-ed file outside the editor while the script itself
  is untouched: Roslyn reloads the document, but the script's options are recomputed only when the
  script or its project changes. Known limitation.

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
  an individual advise, ref-counted across consumers by `FSharpReferenceChangeTracker`. The
  tracker keeps the last-write stamp of each watched path (see Consumers) and has no change
  callback: a notification only drops that stamp.

## Consumers

**Reference stamps.** `FSharpProjectOptionsReactor` watches the `-r:` set of every project it
computes options for, diffed on recompute so an unchanged set touches nothing. The tracker keeps
each path's last-write stamp inside its watch entry, serves it only while the watch is active, and
drops it on the raw change notification. The stat that refills a dropped stamp runs outside the
tracker's lock: the same lock serves every notification from the watched roots, and a slow disk
must not hold those up. A generation counter on the entry makes a stat that raced a notification
return its value without storing it. When Roslyn swaps a `MetadataReference`, the reactor drops
that path's stamp as well, so a notification this tracker missed cannot outlive Roslyn's own.

The `ReferencesOnDisk` guard in `createProjectSnapshot` reads stamps through `IReferenceStamps`, so
the comparison that runs for every new `Project` instance is a dictionary read per reference
instead of a stat. A same-path pair with different stamps drops only that path's cached stamp.

Everything the reactor publishes outside its own agent — the options cache, the single-file cache,
the reference watches — goes through one `PublicationGate`. Closing the solution bumps its epoch
and clears the caches in the same critical section, so a computation that started before the close
finds the epoch moved and publishes nothing.

Still to come:

1. `FSharpProjectSnapshot.FromOptionsWithReferenceStamps` (dotnet/fsharp#20459) lets the snapshot
   built from scratch read the same stamps.
2. A host-supplied stamp provider for the incremental builder, so `IsReferencesInvalidated` reads
   the same stamps instead of stat'ing every reference per request.
