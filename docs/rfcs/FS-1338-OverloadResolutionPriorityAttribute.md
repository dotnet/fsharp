# F# RFC FS-1338 - OverloadResolutionPriorityAttribute support

This RFC covers F# support for `System.Runtime.CompilerServices.OverloadResolutionPriorityAttribute` (the .NET 9 attribute introduced for C# 13). It **partially** addresses fslang-suggestion [#821 - Use an Attribute to specify overload resolution priority](https://github.com/fsharp/fslang-suggestions/issues/821), which predates the .NET attribute. As implemented here the feature is primarily an **interop** feature (correctly consuming and authoring the .NET attribute), not the full "F#-only priority mechanism" originally discussed in #821.

- [x] [Suggestion #821](https://github.com/fsharp/fslang-suggestions/issues/821) (partially addressed; suggestion remains open)
- [ ] Approved in principle
- [x] [Implementation: dotnet/fsharp #19277](https://github.com/dotnet/fsharp/pull/19277)
- [ ] [Discussion](https://github.com/fsharp/fslang-design/discussions/FILL-ME-IN)

**C# reference:** [csharp-13.0/overload-resolution-priority.md](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-13.0/overload-resolution-priority.md)

# Summary

F# recognises `OverloadResolutionPriorityAttribute` during method overload resolution. Among the *applicable* candidates that share a declaring type, candidates whose priority is lower than the highest priority found in that declaring type are pruned before F#'s normal betterness rules run. The default priority is `0`; higher wins, negative deprioritises. This lets F# consume .NET 9+ libraries (e.g. `Debug.Assert`, `MemoryExtensions`) with the same overload selection the library author intended for C# callers, and lets F# authors annotate their own overloads. The feature is gated behind the `OverloadResolutionPriority` language feature (preview).

# Motivation

Library authors evolve APIs by adding a new, preferred overload beside an existing one while preserving binary compatibility. Without a priority mechanism the new overload is often equally (or more) applicable than the old one, producing ambiguity errors or steering callers to the wrong member. `ObsoleteAttribute` does not help: an obsolete member still participates in resolution.

.NET 9 added `OverloadResolutionPriorityAttribute` and the BCL now uses it. The canonical case is `System.Diagnostics.Debug.Assert`, where the one-argument overload is deprioritised so the `CallerArgumentExpression`-bearing overload is preferred. F# must honour the attribute to resolve these APIs the way the author intended; otherwise F# callers get different (and sometimes ambiguous) results than C# callers of the same API.

```fsharp
open System.Diagnostics

// Both compile; resolution matches C#: the deprioritised 1-arg overload does not
// shadow the message-bearing overload.
Debug.Assert(true)
Debug.Assert(false, "explicit message")
```

Authoring the same pattern in F# is also supported:

```fsharp
open System.Runtime.CompilerServices

type FSharpWithORP =
    [<OverloadResolutionPriority(2)>]
    static member Greet(o: obj) = "obj"
    static member Greet(s: string) = "string"
    static member Greet(i: int) = "int"

// FSharpWithORP.Greet "hello" resolves to the obj overload (priority 2 > 0),
// even though string is the more specific type.
```

# Detailed design

## Attribute semantics

`OverloadResolutionPriorityAttribute` carries a single 32-bit integer:

- **Default 0** when the attribute is absent, when the target runtime does not define the attribute, or for an F# override member.
- **Higher values are more preferred.**
- **Negative values deprioritise** (deprecation without breaking binary compatibility).

Priority for a candidate method is computed by `MethInfo.GetOverloadResolutionPriority`:

- For a C#/IL method (`ILMeth`), the value is decoded from the `OverloadResolutionPriorityAttribute` custom attribute on the method definition; absent ⇒ `0`.
- For an F# method (`FSMeth`), the value is read from the member's own attributes; **an F# override member always yields `0`** (it does not carry or inherit its own priority).
- Type-provider methods and default struct constructors yield `0`.
- If the target runtime does not define the attribute type, the lookup yields `0` and the feature is a no-op.

Priority is a property of the method *definition*: it is read the same way regardless of any generic instantiation at the call site, and the candidates are then grouped by declaring type. Ordinary constructor overloads participate on the same footing as methods (a `DefaultStructCtor` carries the default priority `0`).

## Where the pre-filter runs

Priority is applied as a **pre-filter before betterness**, mirroring C#. In `ResolveOverloading`:

1. Check the guard first: the language feature is on **and** at least one candidate has non-zero priority. If it fails, do nothing extra and fall through to the existing pipeline.
2. Under the guard, compute the *applicable* candidates (argument subsumption/conversion allowed) and prune: group them by declaring type, and within each group keep only those whose priority equals the group maximum. Recombine, and restrict the working candidate set to the survivors. If *no* candidate is applicable, keep the full set unchanged so "no overloads found" diagnostics remain complete.
3. Continue with F#'s existing pipeline: exact-match rule, applicability, then betterness (`GetMostApplicableOverload` → `findDecidingRule`).

The pruning core is `filterByOverloadResolutionPriority`: it is a no-op for 0/1 candidates or when the feature is off, and it has a fast path that returns the input unchanged (no allocation) when no candidate carries a non-zero priority. Only when a non-zero priority is present does it build the enriched `(candidate, declaringTypeStamp, priority)` list, group by `DeclaringTyconRef.Stamp`, and keep the per-group maxima.

Because priority prunes *before* betterness, it overrides normal preferences (specificity, non-generic, params, etc.) between members of the same declaring type — but only among candidates that are already applicable.

### Priority is scoped per declaring type

Grouping is by declaring type. Two members with different priorities in *different* declaring types are **not** compared by priority; ordinary betterness decides between them. This matters for extension members: two extension methods on the same receiver but declared in different static classes/modules are in different groups, so priority does not cross the class boundary. Two extension methods in the *same* static class/module are in one group and priority does apply. Grouping is purely by declaring type, so static and instance members of the same type also share one priority group.

```fsharp
// Same declaring type: priority applies. The high-priority obj overload beats the
// exact int overload.
open System.Runtime.CompilerServices
type C() =
    [<OverloadResolutionPriority(1)>] member _.M(x: obj) = "obj"
    member _.M(x: int) = "int"
// C().M 42 = "obj"
```

### Applicability gates pruning

Only *applicable* candidates are pruned. A higher-priority overload that does not apply to the call cannot suppress a lower-priority overload that does:

```fsharp
open System.Runtime.CompilerServices
type C() =
    [<OverloadResolutionPriority(1)>] member _.M(s: string) = "string"
    member _.M(i: int) = "int"
// C().M 42 = "int" — the string overload is inapplicable, so it never prunes the int overload.
```

If *no* candidate is applicable, the full candidate set is retained so the ordinary ambiguity/overload diagnostics list every overload:

```fsharp
open System.Runtime.CompilerServices
type C() =
    [<OverloadResolutionPriority(1)>] member _.M(s: string) = "string"
    member _.M(b: bool) = "bool"
// C().M 42 fails with FS0041 listing both string and bool overloads.
```

### Equal priority falls through to betterness

When the surviving group members share the top priority, pruning keeps all of them and normal betterness decides:

```fsharp
open System.Runtime.CompilerServices
type C() =
    [<OverloadResolutionPriority(1)>] member _.M(x: obj) = "obj"
    [<OverloadResolutionPriority(1)>] member _.M(x: string) = "string"
// C().M "hi" = "string": priorities are equal, so pruning keeps both and ordinary betterness
// (the more specific 'string' parameter) decides. No concreteness rule is involved — it resolves
// this way at every language version.
```

If betterness cannot separate the equal-priority survivors, the call stays ambiguous (FS0041) exactly as it would without the attribute.

## Override and interface semantics

- **Applying the attribute to an F# override (or explicit interface implementation) is an error, FS3586**, raised during member checking when the `OverloadResolutionPriority` feature is on. When the feature is off the attribute is accepted silently (no error), so turning the feature on is the only behaviour change for such code.
- An **F# override member's effective priority is `0`** regardless of any attribute on it; priority is taken from the least-derived declaration of the member.
- For **C#/IL overrides**, C# forbids the attribute on overrides too, so an overridden virtual's effective priority is whatever the least-derived base declaration carries; F# reads it from the base declaration it resolves, matching C#. This is exercised against a C# `virtual`/`override` hierarchy where the base declares the priorities and a call through the derived type still honours them.
- In F#, every **interface implementation** is an *explicit* implementation, so the attribute cannot be placed on an implementing member at all — attempting to raises FS3586 (as above). Priority for an interface method is therefore expressed on the interface member's own declaration; an implementing member's effective priority is `0`. (F# has no *implicit* interface implementations, so C#'s treatment of the attribute on implicit implementations has no F# counterpart.)

```fsharp
open System.Runtime.CompilerServices
type Base() =
    abstract member DoWork: int -> string
    default _.DoWork(x: int) = "base"

type Derived() =
    inherit Base()
    [<OverloadResolutionPriority(1)>]   // FS3586: apply it to the original declaration instead
    override _.DoWork(x: int) = "derived"
```

# Changes to the F# spec

The relevant section is [§14.4 Method Application Resolution](https://fsharp.github.io/fslang-spec/), step 7, which (after determining applicability) chooses the unique best candidate by applying these criteria in order (condensed from the published specification):

```text
1) Prefer candidates whose use does not constrain the use of a user-introduced generic
   type annotation to be equal to another type.
2) Prefer candidates that do not use ParamArray conversion. If two candidates both use
   ParamArray conversion with types pty1 and pty2, and pty1 feasibly subsumes pty2, prefer
   the second; that is, use the candidate that has the more precise type.
3) Prefer candidates that do not have ImplicitlyReturnedFormalArgs.
4) Prefer candidates that do not have ImplicitlySuppliedFormalArgs.
5) If two candidates have unnamed actual argument types ty11 ... ty1n and ty21 ... ty2n, and
   each ty1i either feasibly subsumes ty2i, or ty2i is a System.Func type and ty1i is some
   other delegate type, then prefer the second candidate (the more specific actual argument
   types; any System.Func type is considered more specific than any other delegate type).
6) Prefer candidates that are not extension members over candidates that are.
7) To choose between two extension members, prefer the one that results from the most recent
   use of open.
8) Prefer candidates that are not generic over candidates that are generic - that is, prefer
   candidates that have empty ActualArgTypes.
```

This RFC inserts a **pre-filter before** the "choose a unique best candidate" step above (it does not renumber or alter rules 1–8):

```diff
 7. Choose a unique M~possible according to the following rules:
    - For each M~possible, determine whether the method is applicable ...
+   - OverloadResolutionPriority pre-filter (when the `OverloadResolutionPriority` feature is
+     enabled and at least one applicable candidate carries a non-zero priority): group the
+     applicable candidates by declaring type; within each group discard every candidate whose
+     priority is lower than the maximum priority in that group; recombine the groups. Priority
+     of an override is that of its least-derived declaration. If no candidate is applicable,
+     the set is left unchanged.
    - If a unique applicable M~possible exists, choose that method. Otherwise, choose the unique
      best M~possible by applying criteria 1) through 8) above (and, under `--langversion:preview`,
      the sibling most-concrete rule that FS-1340 appends as rule 9).
```

Notes for implementors, reflecting the actual pipeline:

- The published rules 1–8 above are not the whole story. The compiler's tiebreak list additionally contains **internal rules that are not in the published spec**, interleaved with the published ones rather than all appended after them: type-directed-conversion preferences run *ahead* of the published rules, while a nullable/optional-interop rule (gated on the F# 5.0 `NullableOptionalInterop` feature) and a property-override rule run *after* published rule 8. All are part of betterness, and all precede the most-concrete tiebreaker.
- The sibling **"most-concrete" tiebreaker (FS-1340, preview)** is deliberately the **last** tiebreak rule, so it can only break ties the earlier rules left unresolved.
- **All of the above betterness machinery runs after the ORPA pre-filter.** ORPA only prunes the candidate set; betterness then runs on the survivors exactly as it would without the attribute.

# Drawbacks

- Adds another dimension to overload resolution that developers must understand when reading library APIs.
- The attribute was designed around C#'s conversion model. F#'s type-directed conversions (`op_Implicit`, numeric widening, `Span`/`ReadOnlySpan`) cover the common BCL patterns, but F# lacks C#'s implicit constant narrowing, so the *applicable* set at a call site can differ between the two languages. Because priority prunes only among applicable candidates (see [Applicability gates pruning](#applicability-gates-pruning)), an author's intended high-priority overload that is applicable in C# but not in F# does not cause an error: F# falls back to the highest-priority overload that *is* applicable, which may be a different member than C# selects. F# reports the ordinary "no overloads match" error only when no candidate applies at all.
- Priority can be misused to force unintuitive selections, though this is an explicit, author-visible choice.

# Alternatives

- **Do nothing / ignore the attribute.** Rejected: F# would resolve .NET 9+ APIs (`Debug.Assert`, `MemoryExtensions`, …) differently from C#, reintroducing the very ambiguities the attribute exists to remove.
- **A different, F#-only priority mechanism** (the broader ambition of suggestion #821, e.g. giving derived-type members strict priority in trait calls). Out of scope here; this RFC targets interop parity with the .NET attribute. The suggestion remains open for that broader design.

# Prior art

- **C# 13 / .NET 9**: [Overload Resolution Priority](https://github.com/dotnet/csharplang/blob/main/proposals/csharp-13.0/overload-resolution-priority.md). The C# proposal updates §12.6.4.1 to group the applicable candidate set by declaring type, order each group by priority, drop members below the per-group maximum, recombine, and only then apply the "better function member" tie-breaking of [§12.6.4.3](https://github.com/dotnet/csharpstandard/blob/draft-v9/standard/expressions.md#12643-better-function-member). This RFC follows that shape: pre-filter per declaring type, then F#'s betterness.
- **.NET BCL**: already annotated (`Debug.Assert`, `MemoryExtensions`, and others).
- The C# proposal resolves the same corner cases this RFC adopts: priority is scoped per declaring type (including for extension methods); the attribute is not inherited and an override's priority comes from the least-derived declaration; applying it where it would be ignored (e.g. an override) is an error.

# Compatibility

* **Is this a breaking change?** Not a binary break. It changes method resolution for code that references members carrying the attribute, but only in the direction the member's author intended. The behaviour is gated behind the `OverloadResolutionPriority` language feature (preview), so it does not affect code compiled at earlier language versions.
* **Previous compilers encountering this as source code:** The attribute is an ordinary .NET attribute. Older compilers accept it but ignore it for resolution; overload resolution proceeds as before (and the FS3586 error is not raised).
* **Previous compilers encountering this in compiled binaries:** The attribute is metadata only; older compilers ignore it. No runtime impact.
* **FSharp.Core interaction:** None. This is a compiler feature; it does not add or change FSharp.Core API.

# Interop

* **Consumed by another .NET language:** F# emits the standard `OverloadResolutionPriorityAttribute` on annotated members, so C# (and any language that honours it) sees the same priority F# does.
* **Related features in other languages:** This is the F# side of C# 13's Overload Resolution Priority; the two are designed to agree. The per-declaring-type grouping and override/least-derived rules are chosen to match C#, so a single annotated library resolves the same way from both languages for method and constructor calls. Two cases still diverge: indexer access, where F# resolves by ordinary specificity and does not apply priority at all even when the applicable set is identical (see [Unresolved questions](#unresolved-questions)); and calls that rely on a C#-only conversion, where the applicable set itself differs between the languages (see [Drawbacks](#drawbacks)).

# Pragmatics

## Diagnostics

- **FS3586** (error, always on when the feature is enabled): `The 'OverloadResolutionPriorityAttribute' cannot be applied to an override member. Apply it to the original declaration instead.` Raised when the attribute is placed on an F# override or explicit interface implementation.
- No other ORPA-specific diagnostic ships. In particular there is **no** informational "resolution selected X because of priority" message. When priority pruning leaves an ambiguity, the ordinary FS0041 "unique overload could not be determined" diagnostic fires, listing the surviving overloads.

## Tooling

Priority participates only in overload resolution, so the standard tooling paths are unaffected: tooltips, Go To Definition, colorization, and brace matching behave as for any other method call. Autocomplete offers the same members; selection among them follows the resolved overload. Debugging (breakpoints, stepping, locals/hover, expression evaluation) is unchanged because no new syntax or lowering is introduced. Error recovery is standard: a misplaced attribute yields FS3586 and checking continues.

## Performance

- **Existing code / no attribute present:** negligible. The pre-filter is skipped for 0/1 candidates and when the feature is off. When the feature is on, a single `List.exists` scan checks for any non-zero priority; if none is found the candidate list is returned unchanged with **no allocation** (fast path). The call site applies the same guard before doing any applicability work.
- **New feature / attribute present:** only when a non-zero priority exists does the compiler first run a speculative applicability pass over the candidate group — so that pruning keeps only *applicable* members — then build the enriched `(candidate, stamp, priority)` list, `groupBy` declaring-type stamp, and keep per-group maxima, all linear in the number of candidates in that method group. The applicable set is recomputed on the survivors as resolution continues, so for an annotated group the applicability pass can run twice; this is bounded by the group size and happens only when a non-zero priority is present.

## Scaling

The dimension is the number of overloads in a single method group being resolved.

- Expected maximum in reasonable hand-written code: ~20.
- Reasonable upper bound the compiler accepts: hundreds.

Pruning is linear in the group size (one existence scan, one `groupBy`, one max per group), so it does not worsen the asymptotics of overload resolution.

## Culture-aware formatting/parsing

Not applicable. The feature affects compile-time overload selection only; it produces no formatted or parsed text and does not interact with culture.

# Unresolved questions

- **C# indexers with priority do not currently steer F# resolution.** Known limitation: for a C# indexer annotated with `OverloadResolutionPriority`, F# selects by ordinary specificity, ignoring the indexer's priority (e.g. a `string`-keyed indexer is chosen over a higher-priority `object`-keyed one). Whether F# should honour priority on indexers is open.
- **Interface-implementation priority** is treated as `0` (not inherited from the interface declaration), matching C#'s `params` precedent, but is not yet covered by a dedicated test.
- The broader, F#-only priority mechanism envisaged in suggestion #821 (e.g. strict derived-over-base priority in SRTP/trait calls) is intentionally out of scope and remains open.
