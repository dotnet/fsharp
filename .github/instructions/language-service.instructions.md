---
applyTo: 'vsintegration/src/FSharp.Editor/LanguageService/**'
---

F# VS language service wiring: FSharpWorkspaceServiceFactory, FSharpProjectOptionsManager, FSharpSolutionEvents, and workspace parse/check routing.

- Update `FSharpProjectOptionsManager` cache invalidation on `WorkspaceChanged` and `DocumentClosed` so project and single-file options never drift.
- Keep workspace-change handling in `FSharpWorkspaceServiceFactory` lightweight and notify checker state changes with `checker.NotifyFileChanged`.
- Clear FSharpChecker caches and metadata-as-source state in `FSharpSolutionEvents.OnAfterCloseSolution` before releasing workspace state.
- Route document parse/check requests through `ParseAndCheckDocument` and respect `UseTransparentCompiler` and stale-result settings.
