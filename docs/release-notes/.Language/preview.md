### Added

* Lower integral ranges to fast loops in more cases and optimize list and array construction from ranges. ([PR #16650](https://github.com/dotnet/fsharp/pull/16650), [PR #16832](https://github.com/dotnet/fsharp/pull/16832))
* Better generic unmanaged structs handling. ([Language suggestion #692](https://github.com/fsharp/fslang-suggestions/issues/692), [PR #12154](https://github.com/dotnet/fsharp/pull/12154))
* Bidirectional F#/C# interop for 'unmanaged' constraint. ([PR #12154](https://github.com/dotnet/fsharp/pull/12154))
* Make `.Is*` discriminated union properties visible. ([Language suggestion #222](https://github.com/fsharp/fslang-suggestions/issues/222), [PR #16341](https://github.com/dotnet/fsharp/pull/16341))
* Allow returning bool instead of unit option for partial active patterns. ([Language suggestion #1041](https://github.com/fsharp/fslang-suggestions/issues/1041), [PR #16473](https://github.com/dotnet/fsharp/pull/16473))

### Fixed

* Allow extension methods without type attribute work for types from imported assemblies. ([PR #16368](https://github.com/dotnet/fsharp/pull/16368))
* Enforce AttributeTargets on let values and functions. ([PR #16692](https://github.com/dotnet/fsharp/pull/16692))
* Enforce AttributeTargets on union case declarations. ([PR #16764](https://github.com/dotnet/fsharp/pull/16764))
* Enforce AttributeTargets on implicit constructors. ([PR #16845](https://github.com/dotnet/fsharp/pull/16845/))
* Enforce AttributeTargets on structs and classes ([PR #16790](https://github.com/dotnet/fsharp/pull/16790))

### Changed

* Lower interpolated strings to string concatenation. ([PR #16556](https://github.com/dotnet/fsharp/pull/16556))
