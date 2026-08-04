# F# RFC FS-1338 - OverloadResolutionPriorityAttribute support

Adds F# support for `System.Runtime.CompilerServices.OverloadResolutionPriorityAttribute` (introduced in .NET 9 for C# 13). It **partially** addresses fslang-suggestion [#821](https://github.com/fsharp/fslang-suggestions/issues/821): this is the *interop* part (consuming and authoring the .NET attribute), not the broader F#-only priority mechanism #821 also discusses.

- [x] [Suggestion #821](https://github.com/fsharp/fslang-suggestions/issues/821) (partially addressed; remains open)
- [ ] Approved in principle
- [x] [Implementation](https://github.com/dotnet/fsharp/pull/19277)
- [ ] [Discussion](https://github.com/fsharp/fslang-design/discussions/FILL-ME-IN)
- C# reference: [overload-resolution-priority.md](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-13.0/overload-resolution-priority.md)

# Summary

F# honours `OverloadResolutionPriorityAttribute` during method overload resolution. Among the *applicable* candidates that share a declaring type, those below the highest priority in that type are pruned before F#'s betterness rules run. Default priority is `0`; higher wins, negative deprioritises. This lets F# consume .NET 9+ APIs (`Debug.Assert`, `MemoryExtensions`) with the selection their authors intended for C#, and lets F# authors annotate their own overloads. Gated behind the `OverloadResolutionPriority` language feature (preview).

# Motivation

Authors add a preferred overload beside an existing one while keeping binary compatibility, but the new overload is often equally applicable and causes ambiguity or wrong selection (`ObsoleteAttribute` does not help — obsolete members still resolve). .NET 9 added the attribute and the BCL uses it — canonically `Debug.Assert`, whose 1-argument overload is deprioritised in favour of the `CallerArgumentExpression` one. F# must honour it or F# callers diverge from C#.

```fsharp
open System.Runtime.CompilerServices
type FSharpWithORP =
    [<OverloadResolutionPriority(2)>]
    static member Greet(o: obj) = "obj"
    static member Greet(s: string) = "string"
// Greet "hello" resolves to the obj overload (priority 2 > 0), though string is more specific.
```

# Detailed design

## Attribute semantics

`MethInfo.GetOverloadResolutionPriority` yields the priority (a 32-bit int; higher preferred, negative deprioritises). It is `0` when the attribute is absent, the target runtime does not define it, or the method is an F# override, a default struct constructor, or a type-provider method. For C#/IL methods it is decoded from the attribute on the definition; for F# methods it is read from the member's own attributes.

## The pre-filter

Priority is a **pre-filter before betterness**, mirroring C#. In `ResolveOverloading`:

1. Guard first: the feature is on **and** some candidate has non-zero priority. If not, fall through unchanged (no extra work).
2. Otherwise compute the *applicable* candidates and prune via `filterByOverloadResolutionPriority`: group by declaring type (`DeclaringTyconRef.Stamp`), keep each group's priority maximum, recombine, restrict the working set to the survivors. If *no* candidate is applicable, keep the full set so "no overloads match" diagnostics stay complete.
3. Continue with the existing pipeline: exact-match, applicability, betterness.

Three properties follow:

- **Scoped per declaring type.** Members in different declaring types are never compared by priority — ordinary betterness decides. So two extension methods in *different* static classes are independent, while two in the *same* class share a group, as do the static and instance members of one type.
- **Applicability gates pruning.** Only applicable candidates are pruned, so an inapplicable high-priority overload never shadows an applicable lower-priority one:
  ```fsharp
  type C() =
      [<OverloadResolutionPriority(1)>] member _.M(s: string) = "string"
      member _.M(i: int) = "int"
  // C().M 42 = "int": the string overload is inapplicable, so it never prunes the int one.
  ```
- **Equal priority falls through.** When the survivors share the top priority, betterness decides as usual (and can still end in `FS0041`).

## Override and interface semantics

- Applying the attribute to an F# override or explicit interface implementation is an error, **FS3586**, raised during member checking when the feature is on (silently accepted when off).
- An override's effective priority is `0`; priority is taken from the least-derived declaration. For C#/IL overrides (where C# also forbids the attribute on the override) F# reads the base declaration it resolves, matching C#.
- Every F# interface implementation is *explicit*, so the attribute cannot sit on an implementing member — priority lives on the interface member's declaration and the implementation's effective priority is `0`. (F# has no implicit interface implementations, so C#'s handling of those has no F# counterpart.)

```fsharp
type Derived() =
    inherit Base()
    [<OverloadResolutionPriority(1)>]   // FS3586: apply it to the original declaration instead
    override _.DoWork(x: int) = "derived"
```

# Changes to the F# spec

Inserts a pre-filter before the "choose a unique best candidate" step of [§14.4](https://fsharp.github.io/fslang-spec/) step 7; the published rules 1–8 (and, under preview, FS-1340's rule 9) are unchanged and run afterwards on the survivors:

```diff
 7. Choose a unique M~possible:
    - Determine applicability of each candidate ...
+   - OverloadResolutionPriority pre-filter (feature on and some applicable candidate has non-zero
+     priority): group applicable candidates by declaring type; within each group discard those
+     below the group's maximum priority; recombine. An override's priority is that of its
+     least-derived declaration. If none is applicable, leave the set unchanged.
    - If a unique applicable candidate exists choose it; otherwise apply criteria 1)-8) (and rule 9).
```

The compiler's betterness list also contains internal rules absent from the published spec, interleaved with 1–8: type-directed-conversion preferences run *ahead* of them, while a NullableOptionalInterop rule and a property/override rule run *after* rule 8. All run after this pre-filter and before FS-1340's most-concrete rule.

# Drawbacks

- Adds a dimension developers must understand when reading annotated APIs.
- F#'s conversions (`op_Implicit`, widening, `Span`) cover the common BCL patterns but lack C#'s implicit constant narrowing, so the *applicable* set can differ. Because pruning is over applicable candidates only, a high-priority overload applicable in C# but not F# does not error — F# falls back to the highest-priority *applicable* overload (possibly a different member than C#), and only reports "no overloads match" when nothing applies.
- Priority can be misused to force unintuitive selections, though that is an author-visible choice.

# Alternatives

- **Ignore the attribute.** Rejected: F# would resolve annotated .NET 9+ APIs differently from C#.
- **A separate F#-only priority mechanism** (the broader #821 ambition, e.g. derived-over-base priority in trait calls). Out of scope; #821 stays open for it.

# Prior art

C# 13 / .NET 9 [Overload Resolution Priority](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-13.0/overload-resolution-priority.md) updates §12.6.4.1 to group the applicable set by declaring type, drop members below the per-group maximum, then apply better-function-member. This RFC follows that shape, and the BCL is already annotated (`Debug.Assert`, `MemoryExtensions`). The corner cases adopted here — per-declaring-type scope (including extensions), no inheritance of priority, override priority from the least-derived declaration, and an error when applied where it would be ignored — all follow the C# proposal.

# Compatibility

Not a binary break: it changes resolution only for code referencing annotated members, in the author's intended direction, and only under the preview feature. Older compilers accept the attribute but ignore it for resolution (and do not raise FS3586). The attribute is metadata only — no runtime impact — and FSharp.Core is unchanged.

# Interop

- **Consumed by another .NET language.** F# emits the standard attribute on annotated members, so C# (and any language honouring it) sees the same priority.
- **Related features.** This is the F# side of C# 13's feature; the two are designed to agree, so an annotated library resolves the same way from both for method and constructor calls. Two cases diverge: indexer access, where F# resolves by ordinary specificity and ignores priority even on an identical applicable set (see [Unresolved questions](#unresolved-questions)); and calls relying on a C#-only conversion, where the applicable set itself differs (see [Drawbacks](#drawbacks)).

# Pragmatics

- **Diagnostics:** FS3586 only (attribute on an override / explicit interface impl). There is no "selected X by priority" message; when pruning leaves an ambiguity the ordinary `FS0041` fires listing the survivors.
- **Tooling / Culture-aware formatting:** N/A.
- **Performance:** the guard and fast path make it a no-op unless a non-zero priority is present; otherwise the grouping is linear in the candidate group, plus one speculative applicability pass to compute the pruning set (recomputed on the survivors, so applicability can run twice for an annotated group, bounded by group size).
- **Scaling:** linear in the number of candidates in a method group.

# Unresolved questions

- **Indexers.** F# ignores priority on C# indexers (resolves by specificity); whether it should honour it is open.
- **Interface-implementation priority** is treated as `0` (not inherited), matching C#'s `params` precedent, but has no dedicated test yet.
- The broader F#-only priority mechanism of #821 remains out of scope and open.
