### Added

* Added `MethodOverloadsCache` language feature (preview) that caches overload resolution results for repeated method calls, significantly improving compilation performance. ([PR #19072](https://github.com/dotnet/fsharp/pull/19072))
* Improved escape analysis for byref-like types prevents returning `Span<T>`, `ReadOnlySpan<T>`, and other `IsByRefLike` structs that capture local byrefs. Consumes `ScopedRefAttribute` from C# interop to reduce false positives. ([Language suggestion #1143](https://github.com/fsharp/fslang-suggestions/issues/1143), [RFC FS-XXXX](TBD))

### Fixed

### Changed