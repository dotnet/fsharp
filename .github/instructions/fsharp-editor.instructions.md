---
applyTo: 'vsintegration/src/FSharp.Editor/Common/**'
---

Shared VS editor helpers for FSharp.Editor: DocumentCache and RoslynHelpers.StartAsyncAsTask.

- Use `StartAsyncAsTask` for background work so Roslyn never blocks the UI thread and cancellations flow through the provided token.
- Treat `DocumentCache` entries as versioned by `Document.GetTextVersionAsync`; stale text versions must miss the cache and never key validity on `Document.Id` alone.
