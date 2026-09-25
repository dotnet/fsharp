# Runtime async edits

Hot reload rejects changes to the runtime-async implementation flag on an existing method. Restart the application after such an edit.

The method signature alone does not identify this change. Both implementations return `Task<int>` in metadata, but their IL return conventions differ. Normal IL returns a task. Runtime-async IL returns its integer result and requires the `0x2000` implementation flag.

The delta writer preserves implementation flags from the baseline. It compares the async bit before it emits an updated method row. A mismatch raises `HotReloadUnsupportedEditException` instead of an invalid delta.

## Reference contract

The comparison uses Roslyn commit `eb789e2741f6f22d9e283e2049dc1378871323e0`, inspected on September 25, 2026.

- `MetadataWriter.PopulateMethodTableRows` writes the current method implementation flags.
- `AbstractEditAndContinueAnalyzer` rejects changes to the non-custom `MethodImplAttribute`.
- [Roslyn issue #77954](https://github.com/dotnet/roslyn/issues/77954) remains open for runtime-async edit support.
- The [runtime specification](https://github.com/dotnet/runtime/blob/25bd04c79542dfaaab5c5cd42670bcfc20b7d57a/docs/design/specs/runtime-async.md) defines the distinct return conventions.

The F# rejection is a temporary limit. It does not claim complete parity with future runtime-async edit support in Roslyn.

## Verification

Six regressions cover both transition directions, methods added by an earlier delta, unchanged flags, and runtime application. The .NET 11 run passed all 39 hot reload component tests with no skips. The runtime test called the same method before and after `MetadataUpdater.ApplyUpdate`. Its task result changed from 42 to 43.

A full `mdv` review covered the baseline and the unchanged-flag delta. Both method rows retained `0x2000`. The delta reused the baseline signature and emitted only the module and method update entries in `EncLog` and `EncMap`.

Metadata tests do not require runtime-async execution support. The execution test requires CoreCLR, the runtime's `Async` implementation flag, and enabled metadata updates.
