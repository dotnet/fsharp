# F# RFC FS-1338 - OverloadResolutionPriorityAttribute support

This RFC adds F# support for `System.Runtime.CompilerServices.OverloadResolutionPriorityAttribute`, introduced in .NET 9 for C# 13. It partially addresses the suggestion [Use an attribute to specify overload resolution priority](https://github.com/fsharp/fslang-suggestions/issues/821). This RFC covers the interop part, which is consuming and authoring the .NET attribute. It does not cover the broader F#-only priority mechanism that the suggestion also discusses.

- [x] [Suggestion #821](https://github.com/fsharp/fslang-suggestions/issues/821) (partially addressed; remains open)
- [ ] Approved in principle
- [x] [Implementation](https://github.com/dotnet/fsharp/pull/19277)
- [ ] [Discussion](https://github.com/fsharp/fslang-design/discussions/FILL-ME-IN)

**C# reference:** [csharplang proposal csharp-13.0/overload-resolution-priority.md](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-13.0/overload-resolution-priority.md)

# Summary

F# honours `OverloadResolutionPriorityAttribute` during method overload resolution. Among the applicable candidates that share a declaring type, those below the highest priority in that type are pruned before F#'s betterness rules run. The default priority is `0`; a higher value wins, and a negative value deprioritises. This lets F# consume .NET 9+ APIs such as `Debug.Assert` and `MemoryExtensions` with the selection their authors intended for C#, and lets F# authors annotate their own overloads. The feature is gated behind `--langversion:preview`.

# Motivation

Library authors often add a preferred overload beside an existing one while keeping binary compatibility. The new overload is usually equally applicable, which causes an ambiguity or the wrong selection. `ObsoleteAttribute` does not help, because obsolete members still take part in resolution.

.NET 9 added the attribute and the BCL uses it. The canonical example is `Debug.Assert`, whose one-argument overload is deprioritised in favour of the overload that carries a `CallerArgumentExpression`. F# must honour the attribute, or F# callers resolve these APIs differently from C#.

```fsharp
open System.Runtime.CompilerServices
type FSharpWithORP =
    [<OverloadResolutionPriority(2)>]
    static member Greet(o: obj) = "obj"
    static member Greet(s: string) = "string"
// Greet "hello" resolves to the obj overload (priority 2 > 0), even though string is more specific.
```

# Detailed design

## Attribute semantics

The priority of a method is a 32-bit integer; a higher value is preferred and a negative value deprioritises. The priority is `0` when the attribute is absent, when the target runtime does not define it, or when the method is an F# override, a default struct constructor, or a type-provider method. For a C# or IL method the priority is read from the attribute on the definition. For an F# method it is read from the member's own attributes.

## The pre-filter

Priority acts as a pre-filter before betterness, mirroring C#. Overload resolution proceeds as follows.

1. Guard first. The pre-filter runs only when the feature is on and some candidate has a non-zero priority. Otherwise resolution continues unchanged, with no extra work.
2. Otherwise, take the applicable candidates and prune them. Group the applicable candidates by declaring type, keep each group's maximum priority, and recombine, so that the working set is restricted to the survivors. If no candidate is applicable, keep the full set, so that "no overloads match" diagnostics stay complete.
3. Continue with the existing pipeline: exact match, applicability, and betterness.

Three properties follow.

- **Scoped per declaring type.** Members in different declaring types are never compared by priority; ordinary betterness decides between them. So two extension methods in different static classes are independent, while two in the same class share a group, as do the static and instance members of one type.
- **Applicability gates pruning.** Only applicable candidates are pruned, so an inapplicable high-priority overload never shadows an applicable lower-priority one:
  ```fsharp
  type C() =
      [<OverloadResolutionPriority(1)>] member _.M(s: string) = "string"
      member _.M(i: int) = "int"
  // C().M 42 = "int": the string overload is inapplicable, so it never prunes the int one.
  ```
- **Equal priority falls through.** When the survivors share the top priority, betterness decides between them as usual, and can still end in `FS0041`.

## Override and interface semantics

- Applying the attribute to an F# override or an explicit interface implementation is an error, **FS3586**, raised during member checking when the feature is on. It is accepted silently when the feature is off.
- An F# override therefore never carries its own priority, and its effective priority is `0`. For a C# or IL override, where C# also forbids the attribute on the override, F# reads the priority from the base declaration that the override resolves to, which matches C#. The one divergence is an F# override of an already-prioritized base member: its effective priority is still `0`, so a call resolved against it does not see the base priority.
- Every F# interface implementation is explicit, so the attribute cannot sit on the implementing member. The priority lives on the interface member's declaration, and the implementation's effective priority is `0`. F# has no implicit interface implementations, so C#'s handling of those has no F# counterpart.

```fsharp
type Derived() =
    inherit Base()
    [<OverloadResolutionPriority(1)>]   // FS3586: apply it to the original declaration instead
    override _.DoWork(x: int) = "derived"
```

# Changes to the F# spec

This inserts a pre-filter before the "choose a unique best candidate" step of [§14.4](https://fsharp.github.io/fslang-spec/) step 7. The published rules 1 to 8, and, under preview, FS-1340's rule 9, are unchanged and run afterwards on the survivors.

```diff
 7. Choose a unique M~possible:
    - Determine applicability of each candidate ...
+   - OverloadResolutionPriority pre-filter (feature on and some applicable candidate has non-zero
+     priority): group applicable candidates by declaring type; within each group discard those
+     below the group's maximum priority; recombine. For a C#/IL override the priority is that of
+     its least-derived declaration; an F# override has priority 0. If none is applicable, leave the
+     set unchanged.
    - If a unique applicable candidate exists choose it; otherwise apply criteria 1)-8) (and rule 9).
```

The compiler applies further betterness preferences that are not written in the published spec. All of them run after this pre-filter and before FS-1340's most-concrete rule.

# Drawbacks

- The attribute adds a dimension that developers must understand when reading an annotated API.
- F#'s conversions (`op_Implicit`, widening, `Span`) cover the common BCL patterns, but lack C#'s implicit constant narrowing, so the applicable set can differ between the two languages. Because pruning is over applicable candidates only, a high-priority overload that is applicable in C# but not in F# does not produce an error. F# falls back to the highest-priority applicable overload, which may be a different member than C# selects, and only reports "no overloads match" when nothing applies.
- Priority can be misused to force unintuitive selections, though the annotation is explicit in the source.

# Alternatives

- **Ignore the attribute.** Rejected, because F# would then resolve annotated .NET 9+ APIs differently from C#.
- **A separate F#-only priority mechanism.** This is the broader ambition of the suggestion, for example a derived-over-base priority in trait calls. It is out of scope here, and the suggestion stays open for it.

# Prior art

C# 13 and .NET 9 introduced [Overload Resolution Priority](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-13.0/overload-resolution-priority.md), which updates §12.6.4.1 to group the applicable set by declaring type, drop members below the per-group maximum, and then apply the better-function-member rules. This RFC follows that shape, and the BCL is already annotated (`Debug.Assert`, `MemoryExtensions`). The corner cases adopted here follow the C# proposal and are covered in the detailed design above.

# Compatibility

This is not a binary break. It changes resolution only for code that references annotated members, in the direction the author intended, and only under the preview feature. Older compilers accept the attribute but ignore it during resolution, and do not raise FS3586. The attribute is metadata only, with no runtime impact, and FSharp.Core is unchanged.

# Interop

- **Consumed by another .NET language.** F# emits the standard attribute on annotated members, so C# and any other language that honours it see the same priority.
- **Related features.** This is the F# side of C# 13's feature, and the two are designed to agree, so an annotated library resolves the same way from both languages for method and constructor calls. Two cases diverge. Indexer access is one: F# resolves indexers by ordinary specificity and ignores priority, even when the applicable set is identical (see [Unresolved questions](#unresolved-questions)). A call that relies on a C#-only conversion is the other: there the applicable set itself differs (see [Drawbacks](#drawbacks)).

# Pragmatics

- **Diagnostics.** FS3586 only, for the attribute on an override or explicit interface implementation. There is no "selected X by priority" message; when pruning leaves an ambiguity the ordinary `FS0041` fires and lists the survivors.
- **Tooling and culture-aware formatting.** Not applicable.
- **Performance.** The guard and fast path make the feature a no-op unless a non-zero priority is present. Otherwise the grouping is linear in the size of the candidate group. Applicability may be computed twice for an annotated group, once to prune and once on the survivors, bounded by the group size.
- **Scaling.** Linear in the number of candidates in a method group.

# Unresolved questions

- **Indexers.** F# ignores priority on C# indexers and resolves them by specificity. Whether it should honour priority there is open.
- Whether F# should later add the broader, F#-only priority mechanism the suggestion describes, for example a derived-over-base priority in trait calls.
