### Added

* Added `MethodOverloadsCache` language feature (preview) that caches overload resolution results for repeated method calls, significantly improving compilation performance. ([PR #19072](https://github.com/dotnet/fsharp/pull/19072))
* Improved escape analysis for byref-like types prevents returning `Span<T>`, `ReadOnlySpan<T>`, and other `IsByRefLike` structs that capture local byrefs. Consumes `ScopedRefAttribute` and `UnscopedRefAttribute` from C# interop to reduce false positives. Supports `[<ScopedRef>]` on F# parameters for same-assembly calls. ([Language suggestion #1143](https://github.com/fsharp/fslang-suggestions/issues/1143), [RFC FS-XXXX](https://github.com/dotnet/fsharp/blob/main/docs/rfc/FS-XXXX-improved-byreflike-escape-analysis.md))
* Reads `[module: RefSafetyRules(11)]` from referenced assemblies and emits it on F# assemblies. When version ≥ 11: `out` parameters and `ref`/`in` parameters to ref-struct types are implicitly scoped; `ScopedRefAttribute` is trusted on generic methods. This eliminates false-positive escape errors when consuming C# 11+ libraries.

### Fixed

* Fixed constructor calls incorrectly treated as instance method calls with `this` receiver in escape analysis.
* Fixed generic IL return type resolution in escape analysis to correctly identify `Span<T>` when type parameters are instantiated.

### Changed