# F# Compiler Repository Activity Report — February–March 2026

> **Coverage:** PRs merged between **2026-02-07** and **2026-03-10** (last 32 days)  
> **Total merged PRs:** ~98 (excluding automated dependency-update bots)

---

## Highlights at a Glance

| Area | # PRs | Standout change |
|---|---|---|
| Language features | 3 | `#elif` directive, FS-1336 interface simplification, new FS3882 warning |
| FSharp.Core | 4 | `partitionWith`, `query` fixes, `Set.intersect` perf, Big-O docs |
| Compiler service / IDE | 10+ | FAR/Rename overhaul, nullness fixes, overload-resolution cache |
| F# Interactive | 3 | `#version;;`, `#exit;;`, FSI settings null-check fix |
| Infrastructure / CI | 10+ | xUnit 3 migration, regression-test pipeline expansion, TFM centralisation |

---

## T-Gro Contributions (25 PRs)

### 🔴 High-Impact Language & Library Changes

#### RFC FS-1336 — Simplified Interface Hierarchy Implementation ([#19241](https://github.com/dotnet/fsharp/pull/19241))
When a derived interface provides a Default Interface Member (DIM) implementation for a base interface slot, F# no longer requires explicit interface re-declarations for that DIM-covered slot.  
This removes a significant source of boilerplate when implementing deep interface hierarchies that mix DIM inheritance.

#### Feature: `#elif` Preprocessor Directive ([#19323](https://github.com/dotnet/fsharp/pull/19323))
Implements [RFC FS-1334](https://github.com/fsharp/fslang-design/blob/main/RFCs/FS-1334-elif-preprocessor-directive.md).  
Adds the familiar `#elif` / `#elifdef` / `#elifndef` preprocessor directives, reducing deeply-nested `#if`/`#else`/`#endif` ladders.

```fsharp
#if WIN64
let platform = "win64"
#elif LINUX
let platform = "linux"
#else
let platform = "other"
#endif
```

#### New Warning FS3882 — Function Value in Interpolated String ([#19289](https://github.com/dotnet/fsharp/pull/19289))
Warns when a function or delegate value is used as an interpolated string fill expression. The value is formatted via `ToString` (which prints the closure type name) rather than being applied, almost always a bug.

```fsharp
let f x = x + 1
let s = $"result: {f}"   // FS3882: function will be formatted by ToString
let s = $"result: {f 5}" // correct
```

#### FSharp.Core: `partitionWith` Functions ([#19335](https://github.com/dotnet/fsharp/pull/19335))
Implements [Language Suggestion #1119](https://github.com/fsharp/fslang-suggestions/issues/1119).  
Adds `Array.partitionWith`, `List.partitionWith`, `Set.partitionWith`, and `Array.Parallel.partitionWith` — partitions a collection using a discriminated-union classifier (`'T -> Choice<'T1, 'T2>`), returning two typed result arrays/lists.

```fsharp
let evens, odds =
    [1..10] |> List.partitionWith (fun x -> if x % 2 = 0 then Choice1Of2 x else Choice2Of2 x)
```

#### FSharp.Core Query Expression Fixes ([#19243](https://github.com/dotnet/fsharp/pull/19243))
Fixes **10 query expression bugs** affecting LINQ providers (EF Core, Cosmos DB, etc.):
- Anonymous record field ordering in LINQ expression trees (issues [#11131](https://github.com/dotnet/fsharp/issues/11131), [#15648](https://github.com/dotnet/fsharp/issues/15648))
- Array indexing in LINQ expressions now generates proper array-index expressions ([#16918](https://github.com/dotnet/fsharp/issues/16918))
- Tuple join conditions now use structural equality via generated `Equals`/`GetHashCode` on anonymous objects ([#7885](https://github.com/dotnet/fsharp/issues/7885))
- Tuple projections now use `Queryable.Select` to preserve query composition ([#3782](https://github.com/dotnet/fsharp/issues/3782))
- `EvaluateQuotation` now handles `Sequential`, void, and unit-returning expressions ([#19099](https://github.com/dotnet/fsharp/issues/19099))
- Fix false FS1182 (unused variable) warning for query expression variables ([#422](https://github.com/dotnet/fsharp/issues/422))

#### Find All References & Rename Symbol Overhaul ([#19252](https://github.com/dotnet/fsharp/pull/19252), follow-ups [#19311](https://github.com/dotnet/fsharp/pull/19311), [#19358](https://github.com/dotnet/fsharp/pull/19358), [#19361](https://github.com/dotnet/fsharp/pull/19361))
Addresses **8+ longstanding bugs** in Find All References (FAR) and Rename Symbol:
- Active patterns in signature files now reported correctly ([#19173](https://github.com/dotnet/fsharp/issues/19173), [#14969](https://github.com/dotnet/fsharp/issues/14969))
- Rename now handles operators containing `.` ([#17221](https://github.com/dotnet/fsharp/issues/17221))
- `#line` directive remapping now applied in FAR ([#9928](https://github.com/dotnet/fsharp/issues/9928))
- `SynPat.Or` non-left-most variables correctly classified as uses ([#5546](https://github.com/dotnet/fsharp/issues/5546))
- DU types inside modules found by FAR ([#5545](https://github.com/dotnet/fsharp/issues/5545))
- Synthetic event handler values removed from FAR results ([#4136](https://github.com/dotnet/fsharp/issues/4136))
- C# extension methods found correctly ([#16993](https://github.com/dotnet/fsharp/issues/16993))
- DU case tester properties (`.IsCase`) included in FAR ([#16621](https://github.com/dotnet/fsharp/issues/16621))
- Record copy-and-update expressions included in FAR ([#15290](https://github.com/dotnet/fsharp/issues/15290))
- Constructor definitions now find all usages ([#14902](https://github.com/dotnet/fsharp/issues/14902))
- Corrupted `.ctor` duplicate symbol references removed ([#19336](https://github.com/dotnet/fsharp/issues/19336))
- Semantic coloring fixed for case testers and `with` expressions

A new `CallRelatedSymbolSink` / `NotifyRelatedSymbolUse` API ([#19361](https://github.com/dotnet/fsharp/pull/19361)) was introduced so that union-case tester and copy-and-update record lookups are surfaced separately from actual name resolutions, preserving the integrity of name-resolution data structures.

#### Nullness Bug Fixes ([#19262](https://github.com/dotnet/fsharp/pull/19262))
Five nullness-analysis fixes:
- `int<kg>.ToString()` returns `string` not `string | null` ([#17539](https://github.com/dotnet/fsharp/issues/17539))
- Pipe operator nullness warning points at the nullable argument ([#18013](https://github.com/dotnet/fsharp/issues/18013))
- No false-positive warning for non-null `AllowNullLiteral` constructor results ([#18021](https://github.com/dotnet/fsharp/issues/18021))
- `not null` constraint allowed on type extensions ([#18334](https://github.com/dotnet/fsharp/issues/18334))
- Tuple null elimination simplified to prevent over-inference of non-null ([#19042](https://github.com/dotnet/fsharp/issues/19042))

#### FSharp.Core: Time Complexity Documentation ([#19240](https://github.com/dotnet/fsharp/pull/19240))
Added comprehensive Big-O complexity remarks to all **462 functions** across `Array`, `List`, `Seq`, `Map`, and `Set` modules. Users can now see complexity at a glance in IDE tooltips and generated docs.

---

### 🟡 Infrastructure & Build Improvements

#### Centralize Target Framework Moniker ([#19251](https://github.com/dotnet/fsharp/pull/19251))
All product TFM values are now defined in a single `eng/TargetFrameworks.props` file. Previously scattered across multiple `.props` files, changing the target framework now requires editing exactly one location.

#### Source-Build on Mono: Stack Overflow Fixes ([#19360](https://github.com/dotnet/fsharp/pull/19360), [#19391](https://github.com/dotnet/fsharp/pull/19391))
Added `StackGuard` protection to `visitSynExpr` in `FileContentMapping.fs` for deeply recursive ASTs when running on Mono (used in VMR source-build CI). Further stack depth reduction followed in a second PR.

#### Expanded Regression Test Pipeline ([#19260](https://github.com/dotnet/fsharp/pull/19260))
Added **IcedTasks**, **FsToolkit.ErrorHandling**, and **OpenTK** to the PR regression test pipeline. Discovered and fixed two compiler regressions in the process (FS0229 B-stream misalignment, FS3356 false positive).

#### Orphan Test Recovery ([#19275](https://github.com/dotnet/fsharp/pull/19275))
Recovered test files that existed on disk but were excluded from `.fsproj` builds and therefore never compiled or run.

#### Add Regression Tests for 3 Codegen Issues ([#19309](https://github.com/dotnet/fsharp/pull/19309))
Added 9 regression tests covering previously-fixed but untested codegen issues (`--platform:x64` PE flag, `unmanaged` constraint modreq, struct layout attribute).

---

### 🟢 Tooling / Agent Workflow

- **fsharp-diagnostics skill** ([#19263](https://github.com/dotnet/fsharp/pull/19263)): An `.md` skill file for getting fast F# syntax and semantic errors via `FSharpChecker` in AI coding sessions, without a full build.
- **Review council skill** ([#19282](https://github.com/dotnet/fsharp/pull/19282)): Multi-agent review skill for deep code reviews, with false-positive reduction ([#19324](https://github.com/dotnet/fsharp/pull/19324)).
- **Glossary-maintainer workflow** ([#19302](https://github.com/dotnet/fsharp/pull/19302)): Agentic workflow for maintaining the F# compiler glossary.
- **Release announcement agent** ([#19390](https://github.com/dotnet/fsharp/pull/19390)): Agent to collect initial input for release announcements.
- **Remove duplicate file** ([#19383](https://github.com/dotnet/fsharp/pull/19383)): Removed a file left from a migration that caused build failures on case-insensitive filesystems (macOS/Windows).
- **Fix flaky Linux Project25 test** ([#19373](https://github.com/dotnet/fsharp/pull/19373)): Root cause was NuGet-restored type provider DLLs being loaded in multiple-DLL order, now fixed.
- **Source-build package version fix** ([#19397](https://github.com/dotnet/fsharp/pull/19397)): Moved four `System.*` package versions back to `Version.Details.xml` to fix prebuilt failures in dotnet/dotnet source-build.

---

## Other Notable PRs by the Wider Community

### New Language / F# Interactive Features

| PR | Author | Change |
|---|---|---|
| [#19332](https://github.com/dotnet/fsharp/pull/19332) | bbatsov | Add `#version;;` directive to F# Interactive to print the F# language/runtime version |
| [#19329](https://github.com/dotnet/fsharp/pull/19329) | bbatsov | Support `#exit;;` as an alias for `#quit;;` in FSI |
| [#19347](https://github.com/dotnet/fsharp/pull/19347) | evgTSV | Compiler now emits a **FS0750 error** when `let!` or `use!` are used outside a computation expression (previously: confusing downstream type error) |

### Compiler Service / FCS

| PR | Author | Change |
|---|---|---|
| [#19314](https://github.com/dotnet/fsharp/pull/19314) | auduchinok | Type checker: recover gracefully on argument/overload checking errors instead of bailing out |
| [#19305](https://github.com/dotnet/fsharp/pull/19305) | auduchinok | FCS: capture additional types encountered during analysis |
| [#19325](https://github.com/dotnet/fsharp/pull/19325) | auduchinok | `FSharpType`: add `IsTupleType`, `IsStructTupleType`, `IsAnonRecordType` and related predefined type checks |
| [#19300](https://github.com/dotnet/fsharp/pull/19300) | auduchinok | `FSharpType`: add `ImportILType` to create FCS types from IL type references |
| [#19298](https://github.com/dotnet/fsharp/pull/19298) | auduchinok | `FSharpSymbol`: safer qualified name retrieval (avoids exceptions on certain generic types) |
| [#19353](https://github.com/dotnet/fsharp/pull/19353) | auduchinok | Prevent optional parameter rewritten-tree symbols from being reported to the symbol sink |
| [#19361](https://github.com/dotnet/fsharp/pull/19361) | T-Gro | Introduced separate `NotifyRelatedSymbolUse` API (see FAR section above) |
| [#19375](https://github.com/dotnet/fsharp/pull/19375) | Copilot | Remove `RoslynForTestingButNotForVSLayer` hack, simplifying the test layer |

### Performance

| PR | Author | Change |
|---|---|---|
| [#19072](https://github.com/dotnet/fsharp/pull/19072) | Copilot | **Cache overload resolution results** for repeated method calls — preview language feature `MethodOverloadsCache` significantly reduces compile time for code with many repeated method calls ([Issue #18807](https://github.com/dotnet/fsharp/issues/18807)) |
| [#19292](https://github.com/dotnet/fsharp/pull/19292) | aw0lid | `Set.intersect`: improved performance symmetry; result now always preserves identity from the first set argument |
| [#19297](https://github.com/dotnet/fsharp/pull/19297) | majocha | Improved static compilation of state machines (reduces allocations in certain async/task workflows) |
| [#19271](https://github.com/dotnet/fsharp/pull/19271) | majocha | Ensure correct memory ordering in `LazyWithContext` on ARM64 |

### Bug Fixes

| PR | Author | Change |
|---|---|---|
| [#19403](https://github.com/dotnet/fsharp/pull/19403) | majocha | Fix `YieldFromFinal`/`ReturnFromFinal` being emitted in non-tail positions |
| [#19408](https://github.com/dotnet/fsharp/pull/19408) | Martin521 | Remove leading spaces from `#warn` / `#nowarn` directive ranges |
| [#19370](https://github.com/dotnet/fsharp/pull/19370) | brianrourkeboll | Use culture-independent `IndexOf` in interpolated string parsing to avoid locale-dependent behavior |
| [#19317](https://github.com/dotnet/fsharp/pull/19317) | apoorvdarshan | Fix `Seq.empty` rendering as `"EmptyEnumerable"` in serializers — now delegates to `Enumerable.Empty<'T>()` ([Issue #17864](https://github.com/dotnet/fsharp/issues/17864)) |
| [#19270](https://github.com/dotnet/fsharp/pull/19270) | DedSec256 | F# Scripts: fix default reference paths not resolving when an SDK directory is explicitly specified |
| [#19242](https://github.com/dotnet/fsharp/pull/19242) | aw0lid | Fix `StrongNameSignatureSize` failure on Linux when using full RSA keys ([Issue #17451](https://github.com/dotnet/fsharp/issues/17451)) |
| [#19306](https://github.com/dotnet/fsharp/pull/19306) | bbatsov | Fix null check bug in FSI settings initialization |
| [#19293](https://github.com/dotnet/fsharp/pull/19293) | bbatsov | Fix misleading `Seq.init` XML doc |
| [#19307](https://github.com/dotnet/fsharp/pull/19307) | bbatsov | Fix assorted typos in FSharp.Core comments and XML docs |
| [#19318](https://github.com/dotnet/fsharp/pull/19318) | bbatsov | Rename "inline hints" to "inlay hints" throughout the codebase |
| [#19299](https://github.com/dotnet/fsharp/pull/19299) | auduchinok | Activity: pass default version values to the source constructors |

### Infrastructure / CI

| PR | Author | Change |
|---|---|---|
| [#18950](https://github.com/dotnet/fsharp/pull/18950) | Copilot | **xUnit 3 migration** — migrated the entire test suite to xUnit 3 with the new `IDataAttribute` interface pattern (5,939+ tests) |
| [#19287](https://github.com/dotnet/fsharp/pull/19287) | akoeplinger | Switch Linux CI jobs to dnceng managed pool |
| [#19283](https://github.com/dotnet/fsharp/pull/19283) | Copilot | Add automatic merge flow from `main` to `feature/net11-scouting` |
| [#19280](https://github.com/dotnet/fsharp/pull/19280) | Copilot | Add Prime and Nu to regression test matrix; fix graph resolution for `Module+Module` merges |

---

## Summary Statistics

| Category | Count |
|---|---|
| T-Gro PRs | **25** |
| Language feature PRs | 4 |
| FSharp.Core additions/fixes | 5 |
| Bug fix PRs | 15+ |
| Performance PRs | 4 |
| Infrastructure / CI | 10+ |
| Automated dependency updates (bots) | ~30 |
