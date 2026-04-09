### Added

* Warn (FS3884) when a function or delegate value is used as an interpolated string argument, since it will be formatted via `ToString` rather than being applied. ([PR #19289](https://github.com/dotnet/fsharp/pull/19289))
* Added `MethodOverloadsCache` language feature (preview) that caches overload resolution results for repeated method calls, significantly improving compilation performance. ([PR #19072](https://github.com/dotnet/fsharp/pull/19072))
* Warn (FS3885) when `let ... in` with explicit `in` keyword has a body that extends to subsequent lines, causing unexpected scoping. ([Issue #7741](https://github.com/dotnet/fsharp/issues/7741), [PR #19501](https://github.com/dotnet/fsharp/pull/19501))

### Fixed

### Changed