---
applyTo:
  - "src/**/*.{fs,fsi,fsx}"
  - "vsintegration/src/**/*.{fs,fsi}"
  - "tests/**/*.{fs,fsi,fsx}"
---

# Writing F#

Which constructs to reach for. Code shape and comments are NoBloat's; naming and the abbreviation glossary belong to `docs/coding-standards.md` and `DEVGUIDE.md`.

These rules govern code you write or touch. Do not sweep the codebase to apply them – `CONTRIBUTING.md`: *"DO NOT submit large code formatting changes without discussing with the team first."*

## Tools

When the IDE's F# semantic tools are unavailable, use the `F#` MCP server (`.mcp.json`, FsLangMCP) for navigation, symbol discovery, diagnostics and cross-project usage search. Prefer its semantic tools over plain text search for F#-specific work.

## Strings

- Prefer interpolated strings over `sprintf` and `String.Format`. Use `$"""…"""` when the text itself contains quotes.
- Format specifiers are valid in interpolated strings and help type inference: `$"count %d{n}"`.
- `nameof` over a string literal that names a value, member or type.
- An explicit `StringComparison` on every `Equals`, `StartsWith`, `EndsWith`, `Contains`, `IndexOf` and `Compare`, and an explicit comparer on every `HashSet<string>` and `Dictionary<string, _>`. `Ordinal` by default, `OrdinalIgnoreCase` for identifiers and paths. Culture-sensitive comparison is a decision, never a default.

## Values and types

- `voption` – `ValueSome`/`ValueNone` – over `option` when the value does not escape; it is this compiler's option type. Exception: when an API hands you `'T option` (`Seq.tryHead`, `List.tryFind`), unwrap with `Option.defaultValue`/`Option.defaultWith` directly – do not insert `ValueOption.ofOption` just to switch modules.
- `struct ('T1 * 'T2)` tuples and `[<Struct>]` types on allocation-sensitive paths.
- Anonymous struct records (`struct {| … |}`) over bare tuples for multi-value returns of internal helpers. Public FCS surface is governed by `.fsi` files and compatibility – do not change it for style.
- The compiler generates `IsCaseName` properties (`IsDefault`, `IsCustom`) for DU cases – use them for a specific-case check instead of a full `match`.
- Deconstruct `KeyValuePair` with the `KeyValue` active pattern: `for KeyValue(k, v) in map do …`.
- `[<InlineIfLambda>]` on `inline` higher-order helpers whose lambda argument must not become a closure.

## Lambdas and collections

- Prefer the `_.Property` shorthand in pipeline position: `tys |> List.map _.Type`. Complex expressions (`fun x -> x.Name = name`, `fun x -> x.A, x.B`) cannot use it. Never add a space – `_.MethodCall ()` breaks parsing. Unrelated to the `member _.Foo` self-identifier.
- Eta-reduce: `Seq.map (fun x -> someFunction x)` must become `Seq.map someFunction`.
- Prefer a single traversal – one `fold`, loop, or comprehension – to a chain of transformations: it allocates nothing per element, where a chain allocates at every stage.
- When the chain reads better than one pass, route it through `Seq` and materialize once at the end – a `List`/`Array` chain allocates a whole intermediate collection per stage, a `Seq` chain only an enumerator.
- Concatenate with `[ yield! xs; yield! ys ]` / `seq { yield! xs; yield! ys }` rather than `@` or `Seq.append` – `@` forces both sides to lists and is O(n).
- Cast sequence items with `Seq.cast<Target>`, not `Seq.map (fun item -> item :> Target)`.

## Async and exceptions

- `src/Compiler` targets `netstandard2.0`: prefer `async { }` and the repo's `cancellable { }` (`src/Compiler/Utilities/Cancellable.fs`). `task { }` appears only in `Service/FSharpProjectSnapshot.fs` and `Service/FSharpWorkspaceQuery.fs` – avoid it elsewhere in new core code.
- `vsintegration` runs on the VS threading model where `task { }` is at home. When an override must return non-generic `Task`, annotate explicitly – `override _.M(…) : Task = task { … }` – never cast through pipelines.
- Thread cancellation through; see `ExpertReview.instructions.md`.
- `reraise ()` does not compile inside a `task`/`async` CE (FS0413). There, rethrow with `ExceptionDispatchInfo.Capture(ex).Throw()`; outside CEs plain `reraise ()` is correct.

## Nullness

Enabled in `src/Compiler`, `src/FSharp.Build`, `src/FSharp.Compiler.LanguageServer`. There:

- Declare non-nullable; check for `null` at entry points. Trust C#/F# annotations – no null checks where the type system says a value cannot be null.
- Prefer `match x with | null -> … | x -> …` over `isNull` – the match narrows the type, `isNull` does not.
- Use `withNull` to hint nullability instead of boxing (`isNull (box f)`).
- Before suppressing a nullness warning (3261, 3262, …), escalate in order: `nonNull value` (runtime assert, fail fast) → `Unchecked.nonNull value` (null provably impossible upstream) → inline `#nowarn`/`#warnon` pair, centralised in one interop helper rather than scattered across call sites.

## Warning suppression

Inline `#nowarn "NN"` / `#warnon "NN"` pairs around the smallest possible scope – they are valid anywhere in an `.fs` file, not only at the top. File-level suppression is a last resort.

## Classes (mostly `vsintegration`)

- Initializer syntax over post-construction property assignment: `MyType(ctorArg, MutableProp1 = v1, MutableProp2 = (5 |> string))` – settable properties by name after positional arguments, computed values in parentheses.
- Extension members consumable from C#: `[<AutoOpen; Extension>]` module, `[<Extension; CompiledName "…">]` on each member.
- Prefer a root module (`module Ns.FeatureExtensions`) over `namespace` + a static holder type for extension files; name it after the single target type plus `Extensions`, or after the feature when there are several targets.
- XML doc comments that use markup (`<see/>`, `<c/>`) need their text wrapped in `<summary>`.

## Opens

Sort into blank-line-separated groups, alphabetically within each: `System.*` → `Microsoft.*` → `Internal.Utilities.*` → `FSharp.Compiler.*` (see the top of `src/Compiler/Service/TransparentCompiler.fs`). `open type` last within its group; type and module aliases at the very end.

## Do not mistake for conventions

This codebase implements every F# feature, so finding one here is no evidence that it is used here. These have no foothold – introducing them is a new pattern, not a continuation:

- `while!` and `and!` – no uses at all; the matches are the parser and the checker implementing them.
- `[<TailCall>]` – a handful of deliberate assertions on specific recursive functions, not a habit.

## The language version is not uniform

`FSharp.Profiles.props` sets `LangVersion=preview`, but not under `Configuration=Proto` and not when `BUILDING_USING_DOTNET=true`. The compiler bootstraps, so a feature this repository is *adding* cannot be used in its own source until it ships in the SDK compiler named by `global.json` – otherwise the Proto stage fails.

- `src/FSharp.Build` is pinned to `LangVersion 9`; it can load in Visual Studio against an older FSharp.Core.
- `src/FSharp.Core` leaves nullness off and is bound by `docs/fsharp-core-notes.md`.

## Tests

The ComponentTests DSL and its pipeline are covered by `ComponentTests.instructions.md`. Name tests with backticked spaces: ``let ``Issue 12345 - brief description`` () = …``.
