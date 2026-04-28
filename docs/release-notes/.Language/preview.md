### Added

* **Extension members for operators and SRTP constraints** ([RFC FS-1043](https://github.com/fsharp/fslang-design/blob/main/RFCs/FS-1043-extension-members-for-operators-and-srtp-constraints.md), [fslang-suggestions#230](https://github.com/fsharp/fslang-suggestions/issues/230)): Extension methods now participate in SRTP constraint resolution. This allows defining operators on types you don't own via type extensions:

  ```fsharp
  type System.String with
      static member (*) (s: string, n: int) = String.replicate n s

  let inline multiply (x: ^T) (n: int) = x * n
  let result = multiply "ha" 3  // "hahaha"
  ```

  **Feature flag:** `--langversion:preview` (feature name: `ExtensionConstraintSolutions`)

  **Includes:**
  - Extension operators resolve via SRTP constraints (suggestion #230)
  - Intrinsic members take priority over extension members
  - FS1215 warning suppressed when defining extension operators with preview langversion
  - Weak resolution disabled for inline code, keeping SRTP constraints generic
  - `[<AllowOverloadOnReturnType>]` attribute for defining overloads that differ only by return type (suggestion #820). When applied, return-type information is used during overload resolution to disambiguate call sites.
  - Cross-assembly resolution: extension operators defined in referenced assemblies are resolved via SRTP constraints
  - Extension members solve SRTP constraints but do *not* satisfy nominal static abstract interface constraints (IWSAMs). These are orthogonal mechanisms.
  - Tuple type extensions using syntactic tuple notation: `type ('T1 * 'T2) with` for reference tuples and `type struct ('T1 * 'T2) with` for struct tuples. These are transformed to `System.Tuple<'T1,'T2>` and `System.ValueTuple<'T1,'T2>` extensions respectively.
* Warn (FS3884) when a function or delegate value is used as an interpolated string argument, since it will be formatted via `ToString` rather than being applied. ([PR #19289](https://github.com/dotnet/fsharp/pull/19289))
* Added `MethodOverloadsCache` language feature (preview) that caches overload resolution results for repeated method calls, significantly improving compilation performance. ([PR #19072](https://github.com/dotnet/fsharp/pull/19072))

### Fixed

### Changed

* Inline functions now keep SRTP constraints generic instead of eagerly resolving through weak resolution. This changes inferred types for some inline code — see [RFC FS-1043 compatibility section](https://github.com/fsharp/fslang-design/blob/main/RFCs/FS-1043-extension-members-for-operators-and-srtp-constraints.md) for details and workarounds.
