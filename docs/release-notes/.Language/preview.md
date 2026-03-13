### Added

* Warn (FS3884) when a function or delegate value is used as an interpolated string argument, since it will be formatted via `ToString` rather than being applied. ([PR #19289](https://github.com/dotnet/fsharp/pull/19289))
* Added `MethodOverloadsCache` language feature (preview) that caches overload resolution results for repeated method calls, significantly improving compilation performance. ([PR #19072](https://github.com/dotnet/fsharp/pull/19072))
* Support for .NET IL fields in SRTP member constraints (preview). Inline functions using SRTP `(^T: (member FieldName: FieldType) x)` now resolve against .NET class/struct fields, not just properties and methods. ([Language suggestion #1323](https://github.com/fsharp/fslang-suggestions/issues/1323))

### Fixed

### Changed