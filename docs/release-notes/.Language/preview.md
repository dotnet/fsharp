### Added

* Added a "most concrete" tiebreaker for overload resolution: when several overloads of a method, constructor, or generic-type member are equally applicable, the one with more concrete parameter types is preferred instead of reporting an ambiguity. Requires `--langversion:preview`. ([RFC FS-1340](https://github.com/fsharp/fslang-design/pull/834), [PR #19277](https://github.com/dotnet/fsharp/pull/19277))
* Added support for `System.Runtime.CompilerServices.OverloadResolutionPriorityAttribute` (.NET 9): overloads with a higher priority value are preferred during resolution, matching C#. Requires `--langversion:preview`. ([RFC FS-1338](https://github.com/fsharp/fslang-design/pull/828), [PR #19277](https://github.com/dotnet/fsharp/pull/19277))
* Warn (FS3884) when a function or delegate value is used as an interpolated string argument, since it will be formatted via `ToString` rather than being applied. ([PR #19289](https://github.com/dotnet/fsharp/pull/19289))
* Added `MethodOverloadsCache` language feature (preview) that caches overload resolution results for repeated method calls, significantly improving compilation performance. ([PR #19072](https://github.com/dotnet/fsharp/pull/19072))
* Added `ErrorOnMissingSignatureAttribute` preview language feature: makes FS3888 (compiler-semantic attribute on the `.fs` but not on the `.fsi`) an error instead of a warning. ([Issue #19560](https://github.com/dotnet/fsharp/issues/19560), [PR #19880](https://github.com/dotnet/fsharp/pull/19880))
* Support common types of `NotNullIfNotNullAttribute` usage. If a method parameter is marked with `NotNullIfNotNullAttribute`, the compiler will now honor this attribute and mark the return type as non-null. ([PR #19977](https://github.com/dotnet/fsharp/pull/19977))
* Spread operator for records ([RFC FS-1151](https://github.com/fsharp/fslang-design/pull/805), [PR #18927](https://github.com/dotnet/fsharp/pull/18927))
* Added `AccessProtectedBaseFieldFromClosure` preview language feature: a derived member can now read a `protected` base-class field from an ordinary closure (lambda, delegate, `async`/`seq`/`lazy`, `function`, or list/array literal), which previously failed with FS1097 even though direct access compiles. Object expressions remain unsupported — bind the field to a local function or expose it through a member. ([Issue #5302](https://github.com/dotnet/fsharp/issues/5302))
* Added `ImprovedImpliedArgumentNamesPartTwo` language feature: when a function with no recoverable parameter names is coerced to a delegate (e.g. a partial application like `System.Func<int, int>((+) 1)`), the synthesized `Invoke` parameters take their names from the delegate's own `Invoke` signature instead of synthetic `delegateArg0`, `delegateArg1`, … names. ([PR #20001](https://github.com/dotnet/fsharp/pull/20001))

### Fixed

### Changed
