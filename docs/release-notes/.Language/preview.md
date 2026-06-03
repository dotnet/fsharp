### Added

* Warn (FS3884) when a function or delegate value is used as an interpolated string argument, since it will be formatted via `ToString` rather than being applied. ([PR #19289](https://github.com/dotnet/fsharp/pull/19289))
* Added `MethodOverloadsCache` language feature (preview) that caches overload resolution results for repeated method calls, significantly improving compilation performance. ([PR #19072](https://github.com/dotnet/fsharp/pull/19072))
* Added diagnostic FS3888 warning when a namespace and a type have the same name, which can cause confusing name resolution behavior. ([Issue #17827](https://github.com/dotnet/fsharp/issues/17827), [PR #19802](https://github.com/dotnet/fsharp/pull/19802))

### Fixed

### Changed