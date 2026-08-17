# F# file watching: design review vs Roslyn

## Verdict

The `FileChangeWatcher` working tree is **not** a file-change watcher. It is a pull-model I/O change:

- `IFileSystem.OpenFileForReadShimAsync` + `Stream.ReadAllTextAsync`
- `FSharpFileSnapshot.CreateFromFileSystem` still versions the file as `GetLastWriteTimeShim(fileName).Ticks` at construction time

That is useful (it stops blocking the ThreadPool on disk reads) and orthogonal to watching. It is not comparable to Roslyn's `FileChangeWatcher`.

Roslyn's implementation is a **push** service over `IVsAsyncFileChangeEx2`:

- directory subscriptions (`WatchedDirectory`) instead of one cookie per file
- `AsyncBatchingWorkQueue` (500 ms) so advise/unadvise is batched and never blocks the UI / thread pool
- free-threaded sinks (`IVsFreeThreadedFileChangeEvents2`)
- coalesced invalidation of metadata references (`FileWatchedReferenceFactory`)

## What already exists in this repo

Three layers, none of them `IVsAsyncFileChangeEx2`:

| Layer | API | Role |
|---|---|---|
| `vsintegration/src/FSharp.ProjectSystem.Base/FileChangeManager.cs` | `IVsFileChangeEx` | Legacy project-system reload of nested items |
| `vsintegration/src/FSharp.LanguageService/FSharpSource.fs` (`SetDependencyFiles`) | `IVsFileChangeEx` | Deprecated unroslynized LS: watch `#r` / dependency files |
| **uncommitted** `stash@{7}` → `vsintegration/src/FSharp.Editor/LanguageService/FileChangeWatcher.fs` | `IVsFileChangeEx` | Intended modern replacement in `FSharp.Editor` |

There is **no commit** (on any branch, stash patch, or GitHub search) that implements `IVsAsyncFileChangeEx2`. The work that was remembered as "the async watcher" is the stash below; it uses the older sync advise API, marshalled onto the UI thread.

## Recovered implementation (`stash@{7}`)

Stash: `WIP on revert-20080-t-gro-net11-upgrade: 8bf5dca37`

Index commit that added the file:

`54465595717b8bb746cb2633d5a4aa834888a481`

Shape:

- `IFileChangeWatcher.WatchFile` → `IVsFileChangeEx.AdviseFileChange` / `UnadviseFileChange`
- `FileChangeWatcherHub`: one cookie per path, ref-counted, 500 ms debounce
- Wired into `FSharpProjectOptionsReactor` for `-r:` reference assemblies
- Exposed as `FSharpProjectOptionsManager.WatchFile` so `WorkspaceExtensions` snapshot cache can share the same subscriptions

This is the right *place* (FSharp.Editor, reference assemblies, debounce, share across projects). It is the wrong *shell API*:

- `JoinableTaskFactory.Run` + `SwitchToMainThreadAsync` on every advise/unadvise
- no directory watches → N cookies for a NuGet cache
- no batching of subscribe/unsubscribe
- not free-threaded

## Target design (Roslyn-shaped)

1. Keep the async read shim. It is independent and should ship on its own.
2. Restore `FileChangeWatcher.fs` from `stash@{7}`, then replace `IVsFileChangeEx` with `IVsAsyncFileChangeEx2`:
   - obtain the service asynchronously (same as Roslyn's `Task<IVsAsyncFileChangeEx2>`)
   - queue advise/unadvise on a 500 ms batching work queue; never `JTF.Run`
   - subscribe to directories (NuGet cache, output folders) with extension filters; fall back to per-file only for stray paths
   - implement `IVsFreeThreadedFileChangeEvents2` so callbacks do not hop to the UI thread
3. On a coalesced change: `checker.NotifyFileChanged` / `InvalidateConfiguration` for the owning project only. Stop O(N) `stat` of reference timestamps on every incremental check.
4. Scripts: watch `#r` / `#load` paths the same way; drop caret-move `NotifyFileChanged`.
5. Do not invent a second watcher. Project system (`FileChangeManager`) and FCS (`TimeStampCache`) should consume this service or stay on their existing contracts.

## Suggested split

- PR 1: async `OpenFileForReadShim` (already in the `FileChangeWatcher` worktree).
- PR 2: restore stash watcher as-is (`IVsFileChangeEx`) behind the existing reactor hook — functional, limited.
- PR 3: swap the shell API to `IVsAsyncFileChangeEx2` + directory batching.

PR 2 is optional if PR 3 is done immediately.
