### Added

* Better generic unmanaged structs handling. ([Language suggestion #692](https://github.com/fsharp/fslang-suggestions/issues/692), [PR #12154](https://github.com/dotnet/fsharp/pull/12154))
* Deprecate places where `seq` can be omitted. ([Language suggestion #1033](https://github.com/fsharp/fslang-suggestions/issues/1033), [PR #17772](https://github.com/dotnet/fsharp/pull/17772))
* Added type conversions cache, only enabled for compiler runs ([PR#17668](https://github.com/dotnet/fsharp/pull/17668))
* Support ValueOption + Struct attribute as optional parameter for methods ([Language suggestion #1136](https://github.com/fsharp/fslang-suggestions/issues/1136), [PR #18098](https://github.com/dotnet/fsharp/pull/18098))
* Allow `_` in `use!` bindings values (lift FS1228 restriction) ([PR #18487](https://github.com/dotnet/fsharp/pull/18487))
* Warn when `unit` is passed to an `obj`-typed argument  ([PR #18330](https://github.com/dotnet/fsharp/pull/18330))
* Fix parsing errors using anonymous records and units of measures ([PR #18543](https://github.com/dotnet/fsharp/pull/18543))
* Scoped Nowarn: added the #warnon compiler directive ([Language suggestion #278](https://github.com/fsharp/fslang-suggestions/issues/278), [RFC FS-1146 PR](https://github.com/fsharp/fslang-design/pull/782), [PR #18049](https://github.com/dotnet/fsharp/pull/18049))
* Allow `let!` and `use!` type annotations without requiring parentheses. ([PR #18508](https://github.com/dotnet/fsharp/pull/18508))

### Fixed

* Warn on uppercase identifiers in patterns. ([PR #15816](https://github.com/dotnet/fsharp/pull/15816))

### Changed
