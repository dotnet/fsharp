### Added

* Added "Most Concrete" tiebreaker for overload resolution. When multiple method overloads match, the overload with more concrete type parameters wins. Requires `--langversion:preview`. ([PR #19277](https://github.com/dotnet/fsharp/pull/19277))
* Added support for `System.Runtime.CompilerServices.OverloadResolutionPriorityAttribute` (.NET 9). Methods with higher priority values are preferred during overload resolution, matching C# behavior. Requires `--langversion:preview`. ([PR #19277](https://github.com/dotnet/fsharp/pull/19277))
* Warn (FS3884) when a function or delegate value is used as an interpolated string argument, since it will be formatted via `ToString` rather than being applied. ([PR #19289](https://github.com/dotnet/fsharp/pull/19289))
* Added `MethodOverloadsCache` language feature (preview) that caches overload resolution results for repeated method calls, significantly improving compilation performance. ([PR #19072](https://github.com/dotnet/fsharp/pull/19072))

### Fixed

### Changed