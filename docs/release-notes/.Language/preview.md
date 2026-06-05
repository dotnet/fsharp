### Added

* Warn (FS3884) when a function or delegate value is used as an interpolated string argument, since it will be formatted via `ToString` rather than being applied. ([PR #19289](https://github.com/dotnet/fsharp/pull/19289))
* Added `MethodOverloadsCache` language feature (preview) that caches overload resolution results for repeated method calls, significantly improving compilation performance. ([PR #19072](https://github.com/dotnet/fsharp/pull/19072))
* Added `ErrorOnMissingSignatureAttribute` language feature (preview): escalates warning FS3888 (consumer-visible attribute present in `.fs` but missing from `.fsi`) to an error. Default behavior remains a suppressible warning so existing libraries are not broken in-place. ([Issue #19560](https://github.com/dotnet/fsharp/issues/19560), [PR #19880](https://github.com/dotnet/fsharp/pull/19880))

### Fixed

### Changed