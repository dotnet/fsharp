### Added

* Better generic unmanaged structs handling. ([Language suggestion #692](https://github.com/fsharp/fslang-suggestions/issues/692), [PR #12154](https://github.com/dotnet/fsharp/pull/12154))
* Deprecate places where `seq` can be omitted. ([Language suggestion #1033](https://github.com/fsharp/fslang-suggestions/issues/1033), [PR #17772](https://github.com/dotnet/fsharp/pull/17772))
* Added type conversions cache, only enabled for compiler runs ([PR#17668](https://github.com/dotnet/fsharp/pull/17668))
* Introduction of the `#warnon` compiler directive, enabling scoped nowarn / warnon sections according to [RFC FS-1146](https://github.com/fsharp/fslang-design/pull/782/files). ([Language suggestion #278](https://github.com/fsharp/fslang-suggestions/issues/278), [PR #17507](https://github.com/dotnet/fsharp/pull/17507))

### Fixed
* Warn on uppercase identifiers in patterns. ([PR #15816](https://github.com/dotnet/fsharp/pull/15816))

### Changed
