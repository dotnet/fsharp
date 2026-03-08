### Added

* Warn (FS3884) when a function or delegate value is used as an interpolated string argument, since it will be formatted via `ToString` rather than being applied. ([PR #19289](https://github.com/dotnet/fsharp/pull/19289))
* Added `MethodOverloadsCache` language feature (preview) that caches overload resolution results for repeated method calls, significantly improving compilation performance. ([PR #19072](https://github.com/dotnet/fsharp/pull/19072))
* Support `CallerArgumentExpression` ([Language Suggestion #966](https://github.com/fsharp/fslang-suggestions/issues/966), [PR #17519](https://github.com/dotnet/fsharp/pull/17519))

### Fixed

### Changed

### Breaking Changes

* `assert` keyword enhancement. When this feature enabled, if there are user-overridden `System.Diagnostics.Debug.Assert`s, make sure there is an override with signature `(condition: bool * message: string) -> ...` (no matter what the return type is). ([Issue #18489](https://github.com/dotnet/fsharp/issues/18489), [PR #17519](https://github.com/dotnet/fsharp/pull/17519))