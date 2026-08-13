### Added

* Runtime async: `task`/`async`-style computation expressions can be compiled to use the .NET runtime async support (RuntimeAsync preview feature). ([PR #20235](https://github.com/dotnet/fsharp/pull/20235))
* Added a "most concrete" tiebreaker for overload resolution: when several overloads of a method, constructor, or generic-type member are equally applicable, the one with more concrete parameter types is preferred instead of reporting an ambiguity. Requires `--langversion:preview`. ([RFC FS-1340](https://github.com/fsharp/fslang-design/pull/834), [PR #19277](https://github.com/dotnet/fsharp/pull/19277))
* Added support for `System.Runtime.CompilerServices.OverloadResolutionPriorityAttribute` (.NET 9): overloads with a higher priority value are preferred during resolution, matching C#. Requires `--langversion:preview`. ([RFC FS-1338](https://github.com/fsharp/fslang-design/pull/828), [PR #19277](https://github.com/dotnet/fsharp/pull/19277))
* Allow constructing a record via its all-fields constructor, e.g. `MyRecord(a, b)`, with positional or named arguments (`RecordConstructorSyntax` preview feature). Accessibility matches `{ ... }` construction. ([Suggestion #722](https://github.com/fsharp/fslang-suggestions/issues/722), [RFC FS-1073](https://github.com/fsharp/fslang-design/blob/main/RFCs/FS-1073-record-constructors.md), [PR #19974](https://github.com/dotnet/fsharp/pull/19974))

### Fixed

### Changed
