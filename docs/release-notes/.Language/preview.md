`FromEndSlicing` remains available only with `--langversion:preview`. It is not assigned a stable language version.

Features assigned to F# 11.2 are listed in [11.2](https://fsharp.github.io/fsharp-compiler-docs/release-notes/Language.html#11.2). New, unassigned language features belong here.

### Added

* Allow interpolated strings in constant expressions, such as `[<Literal>] let Items = $"{Root}/items"` and `[<Obsolete($"Use {nameof f} instead")>]`, when each hole is a non-null constant string without alignment or format specifier (`ConstantInterpolatedStrings` preview feature). ([Suggestion #1347](https://github.com/fsharp/fslang-suggestions/issues/1347), [RFC FS-1352](https://github.com/fsharp/fslang-design/blob/main/RFCs/FS-1352-interpolated-strings-in-constant-expressions.md), [PR #20753](https://github.com/dotnet/fsharp/pull/20753))

### Fixed

### Changed
