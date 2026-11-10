`FromEndSlicing` remains available only with `--langversion:preview`. It is not assigned a stable language version.

Features assigned to F# 11.2 are listed in [11.2](https://fsharp.github.io/fsharp-compiler-docs/release-notes/Language.html#11.2). New, unassigned language features belong here.

### Added

* Ranges can be mixed with values in list, array, sequence and computation expressions, as in `[ -3; 1..10; 19 ]`; the range splices its elements like `yield!`. ([Language suggestion #1031](https://github.com/fsharp/fslang-suggestions/issues/1031), [RFC FS-1031](https://github.com/fsharp/fslang-design/blob/main/RFCs/FS-1031-mixing-ranges-and-values-in-sequences.md), [PR #20759](https://github.com/dotnet/fsharp/pull/20759))

### Fixed

### Changed
