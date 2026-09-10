### Added

* **Extension members for operators and SRTP constraints** ([RFC FS-1043](https://github.com/fsharp/fslang-design/blob/main/RFCs/FS-1043-extension-members-for-operators-and-srtp-constraints.md), [fslang-suggestions#230](https://github.com/fsharp/fslang-suggestions/issues/230), [PR #19602](https://github.com/dotnet/fsharp/pull/19602)): Extension methods now participate in SRTP constraint resolution. This allows defining operators on types you don't own via type extensions:

  ```fsharp
  type System.String with
      static member (*) (s: string, n: int) = String.replicate n s

  let inline multiply (x: ^T) (n: int) = x * n
  let result = multiply "ha" 3  // "hahaha"
  ```

  **Feature flag:** `--langversion:preview` (feature name: `ExtensionConstraintSolutions`)

  **Includes:**
  - Extension operators resolve via SRTP constraints (suggestion #230)
  - Only **public** members solve SRTP constraints — a `private`, `internal`, or `protected` member (even one visible at the definition site, or exposed via `InternalsVisibleTo`) is not a valid witness and is rejected at compile time
  - Intrinsic members take priority over extension members
  - FS1215 warning suppressed when defining extension operators with preview langversion
  - Weak resolution disabled for inline code, keeping SRTP constraints generic
  - `[<AllowOverloadOnReturnType>]` attribute for defining overloads that differ only by return type (suggestion #820). When applied, return-type information is used during overload resolution to disambiguate call sites.
  - Cross-assembly resolution: extension operators defined in referenced assemblies are resolved via SRTP constraints
  - Extension members solve SRTP constraints but do *not* satisfy nominal static abstract interface constraints (IWSAMs). These are orthogonal mechanisms.
  - Tuple type extensions using syntactic tuple notation: `type ('T1 * 'T2) with` for reference tuples and `type struct ('T1 * 'T2) with` for struct tuples. These are transformed to `System.Tuple<'T1,'T2>` and `System.ValueTuple<'T1,'T2>` extensions respectively.
* Warn (FS3884) when a function or delegate value is used as an interpolated string argument, since it will be formatted via `ToString` rather than being applied. ([PR #19289](https://github.com/dotnet/fsharp/pull/19289))
* Added `MethodOverloadsCache` language feature (preview) that caches overload resolution results for repeated method calls, significantly improving compilation performance. ([PR #19072](https://github.com/dotnet/fsharp/pull/19072))
* Added `ErrorOnMissingSignatureAttribute` preview language feature: makes FS3888 (compiler-semantic attribute on the `.fs` but not on the `.fsi`) an error instead of a warning. ([Issue #19560](https://github.com/dotnet/fsharp/issues/19560), [PR #19880](https://github.com/dotnet/fsharp/pull/19880))
* Support common types of `NotNullIfNotNullAttribute` usage. If a method parameter is marked with `NotNullIfNotNullAttribute`, the compiler will now honor this attribute and mark the return type as non-null. ([PR #19977](https://github.com/dotnet/fsharp/pull/19977))
* Spread operator for records ([RFC FS-1151](https://github.com/fsharp/fslang-design/pull/805), [PR #18927](https://github.com/dotnet/fsharp/pull/18927))
* Added `AccessProtectedBaseFieldFromClosure` preview language feature: a derived member can now read a `protected` base-class field from an ordinary closure (lambda, delegate, `async`/`seq`/`lazy`, `function`, or list/array literal), which previously failed with FS1097 even though direct access compiles. Object expressions remain unsupported — bind the field to a local function or expose it through a member. ([Issue #5302](https://github.com/dotnet/fsharp/issues/5302))
* Added `ImprovedImpliedArgumentNamesPartTwo` language feature: when a function with no recoverable parameter names is coerced to a delegate (e.g. a partial application like `System.Func<int, int>((+) 1)`), the synthesized `Invoke` parameters take their names from the delegate's own `Invoke` signature instead of synthetic `delegateArg0`, `delegateArg1`, … names. ([PR #20001](https://github.com/dotnet/fsharp/pull/20001))
* Added a "most concrete" tiebreaker for overload resolution: when several overloads of a method, constructor, or generic-type member are equally applicable, the one with more concrete parameter types is preferred instead of reporting an ambiguity. Requires `--langversion:preview`. ([RFC FS-1340](https://github.com/fsharp/fslang-design/pull/834), [PR #19277](https://github.com/dotnet/fsharp/pull/19277))
* Added support for `System.Runtime.CompilerServices.OverloadResolutionPriorityAttribute` (.NET 9): overloads with a higher priority value are preferred during resolution, matching C#. Requires `--langversion:preview`. ([RFC FS-1338](https://github.com/fsharp/fslang-design/pull/828), [PR #19277](https://github.com/dotnet/fsharp/pull/19277))
* Allow constructing a record via its all-fields constructor, e.g. `MyRecord(a, b)`, with positional or named arguments (`RecordConstructorSyntax` preview feature). Accessibility matches `{ ... }` construction. ([Suggestion #722](https://github.com/fsharp/fslang-suggestions/issues/722), [RFC FS-1073](https://github.com/fsharp/fslang-design/blob/main/RFCs/FS-1073-record-constructors.md), [PR #19974](https://github.com/dotnet/fsharp/pull/19974))

### Fixed

* Bitwise operators (`|||`, `&&&`, `^^^`) on enums whose underlying type is not an integer type (e.g. `char`) are now a compile-time error (FS0001, consistent with `~~~`, `<<<`, `>>>`) instead of a runtime `NotSupportedException`. ([Issue #11785](https://github.com/dotnet/fsharp/issues/11785), [PR #20322](https://github.com/dotnet/fsharp/pull/20322))

### Changed

* Inline functions now keep SRTP constraints generic instead of eagerly resolving through weak resolution. This changes inferred types for some inline code — see [RFC FS-1043 compatibility section](https://github.com/fsharp/fslang-design/blob/main/RFCs/FS-1043-extension-members-for-operators-and-srtp-constraints.md) for details and workarounds.
