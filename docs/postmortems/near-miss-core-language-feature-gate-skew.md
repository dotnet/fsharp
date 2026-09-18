---
title: Core language-feature gate skew
category: Postmortems
categoryindex: 550
index: 600
---
# Near miss: FSharp.Core used a feature gated beyond its build language

## Summary

[#20422](https://github.com/dotnet/fsharp/pull/20422) made `[<OptimizeClosureIfNotInlined>]` part of FSharp.Core and applied it to inline List and Array functions, while the new compiler allowed declarations using the attribute only under `LangVersion=preview`. The .NET VMR stage-2 build then used that compiler to rebuild Proto FSharp.Core at F# 11 and failed with FS3350. VMR CI caught the regression before release; no released SDK, NuGet package, or user workload was affected.

## Error Manifestation

In VMR build 1601188, `SB_CentOSStream10_Offline_CurrentSourceBuiltSdk_x64` passed stage 1 and the vertical builds, then failed when stage 2 rebuilt Proto FSharp.Core with the newly source-built compiler:

```text
list.fs(242,71): error FS3350: Feature 'optimize a curried closure argument when its inlining fails' is not available in F# 11.0. Please use language version 'PREVIEW' or greater.
```

The source line was the unconditional `[<InlineIfLambda; OptimizeClosureIfNotInlined>]` annotation on `List.iteri`. Other annotated List and Array functions had the same incompatible compiler/Core version contract.

## Root Cause

The change crossed a self-hosting boundary that was treated as two independent concerns:

1. FSharp.Core defined `OptimizeClosureIfNotInlinedAttribute`, made affected List and Array functions inline, and attached the attribute to their callback parameters. This made the attribute part of the library surface and its optimization metadata for every FSharp.Core build.
2. The compiler recognized the attribute, stored it in a `ValFlags` bit, and used it to hoist one `OptimizedClosures.Adapt` when an opaque callback could not be inlined. Declaration validation separately mapped `LanguageFeature.OptimizeClosureIfNotInlined` to `previewVersion`.

`Configuration=Proto` does not select preview. Once FSharp.Core used the attribute unconditionally, its declaration gate had to be available in every language version used to build that source, including F# 11.

The asymmetry explained the apparently contradictory results. A stage-1 compiler predating #20422 did not recognize `OptimizeClosureIfNotInlined` as a well-known attribute, so it treated the annotation as ordinary metadata and compiled FSharp.Core. The newly built stage-2 compiler recognized the attribute and ran `checkLanguageFeatureError` while checking its declaration, so the same source failed at F# 11.

Consumer language version was not the problem. Imported metadata does not run the declaration check, and optimizer consumption of the `OptimizeClosureIfNotInlined` flag is not language-version gated. The component test that compiles the attributed library at F# 11 and consumes it at F# 8 proves that an old-language consumer can still receive the optimization.

Compiler/Core binary skew also remains correct. `ValFlags` is serialized as a fixed-width `int64`; a compiler predating this flag reads the same field and ignores the unknown bit, so the metadata layout remains aligned and the inline function still executes correctly. That older compiler cannot perform the new hoisted-`Adapt` transform, however, so an opaque callback can retain per-call curried dispatch inside the newly inline List or Array loop. The skew is therefore a performance compatibility concern, not a correctness or metadata compatibility failure.

The violated invariant was: **when FSharp.Core unconditionally adopts compiler-recognized syntax, attributes, or optimization metadata, the compiler feature gate and every compiler generation that builds Core must agree on the minimum supported language version.**

## Why It Escaped

The positive and structural component tests compiled their attributed sources with `withLangVersionPreview`, so they proved the feature implementation without proving the F# 11 boundary required by Proto. The availability test rejected F# 8 but did not assert that F# 11 was the first accepted version.

The ordinary bootstrap also masked the defect. Stage 1 used an older compiler that did not recognize the new attribute, while the normal non-Proto product build could use preview. Only the VMR stage-2 flow combined the newly built compiler with a Proto FSharp.Core rebuild at F# 11. fsharp CI did not exercise that exact fresh-compiler/Proto-language combination before the change forward-flowed.

The failure appeared in FSharp.Core, but the incompatible decision was in the compiler's language-feature table. That separation made each side look locally valid while violating their shared self-hosting contract.

## Fix

[#20571](https://github.com/dotnet/fsharp/pull/20571) maps `LanguageFeature.OptimizeClosureIfNotInlined` to `languageVersion110`. Positive optimization tests and structural-validation tests now compile at F# 11, the boundary test rejects F# 10 with FS3350, and the cross-assembly test compiles the attributed library at F# 11 while retaining its F# 8 consumer.

The matching language release note moved from preview to F# 11. Validation rebuilt Proto FSharp.Core at F# 11 with the freshly built Release compiler, reproducing the VMR stage-2 compiler/Core pairing.

## Timeline

| Date | Event |
| --- | --- |
| 2026-09-16 | #20422 merged as `ec437d5ac2f03c27eb3b4a9ec14fb829570baadf`, adding the attribute, compiler optimization, and unconditional List/Array usage. |
| 2026-09-17 | VMR build 1601188 passed stage 1 and vertical builds, then failed the offline current-source-built-SDK stage-2 Proto FSharp.Core rebuild with FS3350. |
| 2026-09-18 | #20571 assigned the feature to F# 11, moved the release note, pinned the F# 11/F# 10/F# 8 boundaries, and passed a fresh-compiler Proto FSharp.Core rebuild. |

## Prevention

`.github/instructions/CoreCompilerFeatureCoupling.instructions.md` scopes the shared contract to the FSharp.Core declarations/usages, compiler feature gate and metadata machinery, optimizer, and regression tests involved in this class of change.

For any compiler-recognized construct or optimization metadata adopted unconditionally by FSharp.Core:

- assign its language feature to the lowest language version used to build that Core source, or make the Core usage conditional;
- test successful declaration at that version and FS3350 at the preceding version;
- test imported metadata with an older consumer language version when consumption is intentionally version-independent;
- rebuild Proto FSharp.Core at that language version with the freshly built Release compiler, not only the stage-1 compiler;
- preserve cross-version metadata layout and correctness, and assess the performance fallback when an older compiler ignores new optimization metadata.
