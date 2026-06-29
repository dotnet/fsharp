### Added

* Warn (FS3884) when a function or delegate value is used as an interpolated string argument, since it will be formatted via `ToString` rather than being applied. ([PR #19289](https://github.com/dotnet/fsharp/pull/19289))
* Added `MethodOverloadsCache` language feature (preview) that caches overload resolution results for repeated method calls, significantly improving compilation performance. ([PR #19072](https://github.com/dotnet/fsharp/pull/19072))
* Added `ErrorOnMissingSignatureAttribute` preview language feature: makes FS3888 (compiler-semantic attribute on the `.fs` but not on the `.fsi`) an error instead of a warning. ([Issue #19560](https://github.com/dotnet/fsharp/issues/19560), [PR #19880](https://github.com/dotnet/fsharp/pull/19880))
* Added `ImprovedImpliedArgumentNamesPartTwo` language feature: when a function with no recoverable parameter names is coerced to a delegate (e.g. a partial application like `System.Func<int, int>((+) 1)`), the synthesized `Invoke` parameters take their names from the delegate's own `Invoke` signature instead of synthetic `delegateArg0`, `delegateArg1`, … names. ([PR #20001](https://github.com/dotnet/fsharp/pull/20001))

### Fixed

### Changed

* Direct delegate construction ([PR ##19993](https://github.com/dotnet/fsharp/pull/19993))