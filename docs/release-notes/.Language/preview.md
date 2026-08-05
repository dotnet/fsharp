### Added

* Warn (FS3884) when a function or delegate value is used as an interpolated string argument, since it will be formatted via `ToString` rather than being applied. ([PR #19289](https://github.com/dotnet/fsharp/pull/19289))
* Added `MethodOverloadsCache` language feature (preview) that caches overload resolution results for repeated method calls, significantly improving compilation performance. ([PR #19072](https://github.com/dotnet/fsharp/pull/19072))
* Support `CallerArgumentExpression` ([Language Suggestion #966](https://github.com/fsharp/fslang-suggestions/issues/966), [PR #17519](https://github.com/dotnet/fsharp/pull/17519))
* Added `ErrorOnMissingSignatureAttribute` preview language feature: makes FS3888 (compiler-semantic attribute on the `.fs` but not on the `.fsi`) an error instead of a warning. ([Issue #19560](https://github.com/dotnet/fsharp/issues/19560), [PR #19880](https://github.com/dotnet/fsharp/pull/19880))
* Support common types of `NotNullIfNotNullAttribute` usage. If a method parameter is marked with `NotNullIfNotNullAttribute`, the compiler will now honor this attribute and mark the return type as non-null. ([PR #19977](https://github.com/dotnet/fsharp/pull/19977))
* Spread operator for records ([RFC FS-1151](https://github.com/fsharp/fslang-design/pull/805), [PR #18927](https://github.com/dotnet/fsharp/pull/18927))
* Added `AccessProtectedBaseFieldFromClosure` preview language feature: a derived member can now read a `protected` base-class field from an ordinary closure (lambda, delegate, `async`/`seq`/`lazy`, `function`, or list/array literal), which previously failed with FS1097 even though direct access compiles. Object expressions remain unsupported — bind the field to a local function or expose it through a member. ([Issue #5302](https://github.com/dotnet/fsharp/issues/5302))
* Added `ImprovedImpliedArgumentNamesPartTwo` language feature: when a function with no recoverable parameter names is coerced to a delegate (e.g. a partial application like `System.Func<int, int>((+) 1)`), the synthesized `Invoke` parameters take their names from the delegate's own `Invoke` signature instead of synthetic `delegateArg0`, `delegateArg1`, … names. ([PR #20001](https://github.com/dotnet/fsharp/pull/20001))

### Fixed

### Changed

### Breaking Changes

* `assert` keyword enhancement. When this feature enabled, if there are user-overridden `System.Diagnostics.Debug.Assert`s, make sure there is an override with signature `(condition: bool * message: string) -> ...` (no matter what the return type is). ([Issue #18489](https://github.com/dotnet/fsharp/issues/18489), [PR #17519](https://github.com/dotnet/fsharp/pull/17519))
* Direct delegate construction ([PR #19993](https://github.com/dotnet/fsharp/pull/19993))
  * A delegate built from a method or function now points straight at that method instead of an intermediate closure, so `delegate.Method` is the real target and no closure class is generated.
  * Two delegates built from the same method and target now compare equal, where the previous closure form produced distinct instances; this also makes `Delegate.Remove` (and `-=` on events) match and remove such a delegate that it previously left in place.
  * A `null` instance receiver now faults at delegate construction rather than at the first invoke: an `ArgumentException` for a non-virtual target (the delegate constructor rejects a null `this`) or a `NullReferenceException` for a virtual one (from `ldvirtftn`), matching how C# builds the same delegate.
