`FromEndSlicing` remains available only with `--langversion:preview`. It is not assigned a stable language version.

Features assigned to F# 11.2 are listed in [11.2](https://fsharp.github.io/fsharp-compiler-docs/release-notes/Language.html#11.2). New, unassigned language features belong here.

### Added

* Allow a quotation to read fields of a captured struct local, such as `v.Position` or `h.C.Value`, instead of reporting FS3155 (`CaptureStructFieldReadsInQuotations` preview feature). Method calls and property getters that need the captured local's address still report FS3155. ([Issue #20376](https://github.com/dotnet/fsharp/issues/20376), [PR #PR_NUMBER](https://github.com/dotnet/fsharp/pull/PR_NUMBER))

### Fixed

### Changed
