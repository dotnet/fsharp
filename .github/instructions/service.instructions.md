---
applyTo: 'src/Compiler/Service/**'
---

Public FSharpChecker surface and background compilation plumbing: FSharpChecker, BackgroundCompiler, TransparentCompiler.

- Keep `getNavigationFromImplFile` computing ranges with `unionRangesChecked` and `rangeOfDecls`; navigation items must span the whole declaration body, not just the identifier.
- Honor `HasSignature` when mapping notified slots: implementation-file edits with a signature file must not invalidate the build, only propagate existing status.
- Gate `FSharpChecker.TransparentCompiler` behind `UsesTransparentCompiler`; callers must not cast `backgroundCompiler` unless transparent mode is enabled.
- Treat `TypeCheckInfo` as replace-only and key tooltip caching on the current line string, so stale scopes never leak across edits.
- Make `GetUsesOfSymbol` and `GetUsesOfSymbolInFile` drop `ItemOccurrence.RelatedText` and `distinctBy` occurrence and range, so symbol-use results stay stable and duplicate-free.
- When `getOrCreateBuilder` sees `IsReferencesInvalidated`, clear every matching `checkFileInProjectCache` entry before recreating the builder, or stale cached file results get reused.
- In `GetSemanticClassification`, filter `CapturedNameResolutions` to the requested range and dedupe by range before classifying, or overlapping captures emit duplicate spans.
- Keep `FSharpChecker.Create` enforcing the `keepAssemblyContents` and `enablePartialTypeChecking` exclusion.
- Preserve directive post-processing in `processDirectiveLine`, `processHashIfLine`, and `processHashEndElse`; VS colorization depends on the split tokens and their exact offsets.
- Preserve `inferParallelReferenceResolution` environment override behavior when changing `FSharpChecker.Create` options.
