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
  - `[<AllowOverloadOnReturnType>]` attribute for return-type-based overload disambiguation (suggestion #820)

  **Known limitation:** Extension operators defined in a referenced assembly (cross-assembly resolution) are not yet supported.

### Fixed

### Changed

* Inline functions now keep SRTP constraints generic instead of eagerly resolving through weak resolution. This changes inferred types for some inline code — see [RFC FS-1043 compatibility section](https://github.com/fsharp/fslang-design/blob/main/RFCs/FS-1043-extension-members-for-operators-and-srtp-constraints.md) for details and workarounds.