### Added

* Better generic unmanaged structs handling. ([Language suggestion #692](https://github.com/fsharp/fslang-suggestions/issues/692), [PR #12154](https://github.com/dotnet/fsharp/pull/12154))
* Deprecate places where `seq` can be omitted. ([Language suggestion #1033](https://github.com/fsharp/fslang-suggestions/issues/1033), [PR #17772](https://github.com/dotnet/fsharp/pull/17772))
* Added type conversions cache, only enabled for compiler runs ([PR#17668](https://github.com/dotnet/fsharp/pull/17668))
* Support ValueOption + Struct attribute as optional parameter for methods ([Language suggestion #1136](https://github.com/fsharp/fslang-suggestions/issues/1136), [PR #18098](https://github.com/dotnet/fsharp/pull/18098))
* Allow `_` in `use!` bindings values (lift FS1228 restriction) ([PR #18487](https://github.com/dotnet/fsharp/pull/18487))
* Warn when `unit` is passed to an `obj`-typed argument  ([PR #18330](https://github.com/dotnet/fsharp/pull/18330))
* Fix parsing errors using anonymous records and units of measures ([PR #18543](https://github.com/dotnet/fsharp/pull/18543))
* Scoped Nowarn: added the #warnon compiler directive ([Language suggestion #278](https://github.com/fsharp/fslang-suggestions/issues/278), [RFC FS-1146 PR](https://github.com/fsharp/fslang-design/pull/782), [PR #18049](https://github.com/dotnet/fsharp/pull/18049))
* Allow `let!`, `use!`, `and!` type annotations without requiring parentheses (([PR #18508](https://github.com/dotnet/fsharp/pull/18508) and [PR #18682](https://github.com/dotnet/fsharp/pull/18682)))
* Exception names are now validated for illegal characters using the same mechanism as types/modules/namespaces ([Issue #18763](https://github.com/dotnet/fsharp/issues/18763))
* Support tail calls in computation expressions ([PR #18804](https://github.com/dotnet/fsharp/pull/18804))

### Fixed

* Warn on uppercase identifiers in patterns. ([PR #15816](https://github.com/dotnet/fsharp/pull/15816))
* Error on invalid declarations in type definitions.([Issue #10066](https://github.com/dotnet/fsharp/issues/10066), [PR #18813](https://github.com/dotnet/fsharp/pull/18813))
* Fix type erasure logic for `nativeptr<'T>` overloads to properly preserve element type differences during duplicate member checking. ([Issue #<ISSUE_NUMBER>](https://github.com/dotnet/fsharp/issues/<ISSUE_NUMBER>), [PR #<PR_NUMBER>](https://github.com/dotnet/fsharp/pull/<PR_NUMBER>))

### Changed
