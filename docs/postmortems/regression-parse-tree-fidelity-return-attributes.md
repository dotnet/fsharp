# Regression: `[<return: X>]` Attributes Disappeared From the Untyped Syntax Tree

## Summary

A semantic lowering that had always run in the type checker was moved into the parser, so `SynBinding.attributes` stopped reporting `[<return: X>]` attributes that were visibly present in the source. Tools that read the untyped tree — formatters, analyzers, source generators — were handed a tree that no longer matched the file it came from. Fantomas silently deleted every `[<return: Struct>]` partial active pattern it formatted.

## Error Manifestation

No error. No warning. No diagnostic anywhere.

Given source that visibly carries an attribute:

```fsharp
[<return: Struct>]
let (|Foo|_|) x = ValueNone
```

`SynBinding.attributes` was `[]`. A round-tripping tool read the binding, found nothing to print, and wrote the file back without the attribute:

```fsharp
let (|Foo|_|) x = ValueNone     // attribute gone, file still compiles, meaning changed
```

The failure is silent by construction: the consumer cannot detect an absence it was never told about. Fantomas' own code base contains 34 such active patterns, and self-formatting would have stripped all of them.

## Root Cause

`[<return: X>]` on a binding is written in front of the binding but targets the method's return value. Routing it to `SynValInfo.retInfo` is correct for the type checker, IL emit and the Symbols API. The mistake was *where* the routing happened.

[PR #19738](https://github.com/dotnet/fsharp/pull/19738) moved the rotation into `mkSynBinding` in `SyntaxTreeOps.fs`, a parser-stage constructor. Before that, the rotation lived in `TcNormalizedBinding` and patched a *local* `valSynData`; the `SynBinding` itself was never touched, so the parse tree stayed faithful to the source.

The violated invariant:

> **The untyped syntax tree describes where the user wrote things. Semantic relocation belongs downstream of it.**

`SynBinding` has exactly one contract — report the source. It is not a type checker input in disguise; it is the public output of `FSharpParseFileResults`, and it is the *only* view some consumers have. Once the parser rewrites a node, no consumer can recover the original, because the parser is the last stage that saw the source.

The rotation was lossy in two ways that made recovery impossible even for a consumer that knew about it:

- The attribute list's range narrowed from the `[< >]` span to the attribute alone, so the brackets the user typed were no longer represented anywhere in the tree.
- Every return attribute was collected into one synthesized `SynAttributeList`, so `[<return: A; return: B>]` and `[<return: A>][<return: B>]` produced identical trees. Neither can be printed back to its original form.

## Why It Escaped

The change was reviewed as a type-checker fix, and as a type-checker fix it was correct — both reported bugs (#17904, #19020) were genuinely fixed, and the tests added with it all passed:

- `AttributeCheckingTests.fs` — diagnostics
- `Symbols.fs` — `mfv.ReturnParameter.Attributes` via the FCS Symbols API

All of them observe the *typed* tree. None observes the parse tree. The blast radius of editing `mkSynBinding` — every untyped-tree consumer in the ecosystem — was never in view.

The `tests/service/data/SyntaxTree` baseline corpus is exactly the mechanism that catches this: it pretty-prints the parse tree to a `.bsl` file, so any change to tree shape shows up as a baseline diff a reviewer must accept. At the time of #19738, **not one file in that corpus contained a `return:` attribute**. The corpus was silent because the case did not exist in it, and a silent corpus reads the same as a passing one.

It shipped to nuget.org in `FSharp.Compiler.Service 43.13.101-preview7.26381.103` (2026-08-11). It was found downstream by [fsprojects/fantomas#3400](https://github.com/fsprojects/fantomas/pull/3400) while bumping vendored compiler sources — caught before any Fantomas release carried it, but only because that bump walked one upstream commit at a time. Fantomas' own suite stayed green throughout: its tests covered the return *type annotation* form (`let f x : [<return: A>] int = x`), which never went through this rotation, and not the prefix form, which did.

## Fix

[PR #20356](https://github.com/dotnet/fsharp/pull/20356) moves the rotation to `BindingNormalization.NormalizeBinding` in `CheckExpressions.fs` — the single funnel from `SynBinding` to `NormalizedBinding`, and already a lowering step. Every consumer of the rotated form (`TcNormalizedBinding`, `AnalyzeAndMakeAndPublishRecursiveValue`, the object-expression paths) reads `NormalizeBinding`'s output, so both fixes from #19738 are unchanged and `retInfo` remains the single source of truth for the checker.

`NormalizedBinding` holds a flat `SynAttribute list`, so `RotateReturnAttributes` now takes and returns that instead of `SynAttributes`. The list-splicing that flattened attribute grouping is gone with it — there is no grouping left to destroy at that layer.

`SynBinding` again carries the attribute with its full `[< >]` range, and `retInfo` is empty at parse time.

## Timeline

| Date | PR | Change |
|---|---|---|
| 2026-05-20 | [#19738](https://github.com/dotnet/fsharp/pull/19738) | Rotation moved from `TcNormalizedBinding` (local `valSynData` patch) into `mkSynBinding`. Fixes #17904 and #19020; parse tree starts diverging from source. |
| 2026-08-11 | — | Ships to nuget.org in `FSharp.Compiler.Service 43.13.101-preview7.26381.103`. |
| 2026-08-21 | [fantomas#3400](https://github.com/fsprojects/fantomas/pull/3400) | Fantomas bumps vendored compiler sources, finds `[<return: Struct>]` silently deleted, works around it with `restoreRotatedReturnAttributes`, and raises the layering question upstream. |
| 2026-08-25 | [#20356](https://github.com/dotnet/fsharp/pull/20356) | Rotation moved to `BindingNormalization.NormalizeBinding`. Parse tree faithful again; grouping and ranges preserved. |

## Prevention

- **Rule encoded** in [`.github/instructions/SyntaxTree.instructions.md`](../../.github/instructions/SyntaxTree.instructions.md): the parser must not perform semantic relocation, and any change to parse-tree shape needs `tests/service/data/SyntaxTree` coverage.
- **Baseline coverage added** in `tests/service/data/SyntaxTree/Attribute/`: `ReturnTargetedAttributeStaysOnBinding.fs` pins the attribute to `SynBinding.attributes` with its `[< >]` range, and `ReturnTargetedAttributeGroupingIsPreserved.fs` pins `[<return: A>][<return: A>]` and `[<return: A; return: A>]` to distinct trees.

The generalizable lesson is about *which* tests a change needs, not about attributes. A fix that is correct in the type checker can still be wrong in the parser, and only a parse-tree baseline will say so.
