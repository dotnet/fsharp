### Added

* Added "Most Concrete" tiebreaker for overload resolution. When multiple method overloads match, the overload with more concrete type parameters wins. Requires `--langversion:preview`. ([PR #19277](https://github.com/dotnet/fsharp/pull/19277))
* Added support for `System.Runtime.CompilerServices.OverloadResolutionPriorityAttribute` (.NET 9). Methods with higher priority values are preferred during overload resolution, matching C# behavior. Requires `--langversion:preview`. ([PR #19277](https://github.com/dotnet/fsharp/pull/19277))
### Fixed

### Changed