### Added

* Warn (FS3884) when a function or delegate value is used as an interpolated string argument, since it will be formatted via `ToString` rather than being applied. ([PR #19289](https://github.com/dotnet/fsharp/pull/19289))
* Added `MethodOverloadsCache` language feature (preview) that caches overload resolution results for repeated method calls, significantly improving compilation performance. ([PR #19072](https://github.com/dotnet/fsharp/pull/19072))
* Added `ErrorOnMissingSignatureAttribute` preview language feature: makes FS3888 (compiler-semantic attribute on the `.fs` but not on the `.fsi`) an error instead of a warning. ([Issue #19560](https://github.com/dotnet/fsharp/issues/19560), [PR #19880](https://github.com/dotnet/fsharp/pull/19880))
* Allow constructing a record via its all-fields constructor, e.g. `MyRecord(a, b)`, with positional or named arguments (`RecordConstructorSyntax` preview feature). Accessibility matches `{ ... }` construction. ([Suggestion #722](https://github.com/fsharp/fslang-suggestions/issues/722), [RFC FS-1073](https://github.com/fsharp/fslang-design/blob/main/RFCs/FS-1073-record-constructors.md), [PR #19974](https://github.com/dotnet/fsharp/pull/19974))

### Fixed

### Changed