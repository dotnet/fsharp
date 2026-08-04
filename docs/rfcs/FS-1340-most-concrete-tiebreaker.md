# F# RFC FS-1340 - "Most Concrete" Tiebreaker for Overload Resolution

The design suggestion ["Most concrete" tiebreaker for generic overloads](https://github.com/fsharp/fslang-suggestions/issues/905) has been marked "approved in principle".

- [x] [Suggestion](https://github.com/fsharp/fslang-suggestions/issues/905)
- [x] Approved in principle
- [x] [Implementation](https://github.com/dotnet/fsharp/pull/19277)
- [ ] [Discussion](https://github.com/fsharp/fslang-design/discussions/FILL-ME-IN)

# Summary

A last-resort tiebreaker for overload resolution: when the existing rules leave two candidates equally ranked and one has strictly more concrete formal parameter types (e.g. `'T option` versus `'T`), prefer the more concrete one instead of reporting `FS0041`. This matches the intent of C#'s "better function member" rules for the common generic-wrapper case. Gated behind `--langversion:preview`.

# Motivation

Generic wrapper types commonly offer one constructor taking a bare value and others taking already-wrapped forms. A constructor's type parameters live on the enclosing type and are inferred from the arguments, so more than one is applicable and F# reports an ambiguity even when one parameter type is strictly more concrete:

```fsharp
type Wrapper<'T>(tag: string) =
    new(x: 'T)        = Wrapper<'T>("value")
    new(x: 'T option) = Wrapper<'T>("option")

// Today: FS0041. Proposed: resolves to new('T option).
let w = Wrapper(Some 5)
```

The same shape appears on `ValueTask<'T>` (`ValueTask('T)` / `ValueTask(Task<'T>)`, which motivated the suggestion) and drives workarounds in TaskBuilder.fs and FsToolkit.ErrorHandling; the method form (`Call(x: 'T)` vs `Call(x: 'T option)`) is identical. A set with one fully-generic and one fully-concrete candidate (`Invoke(Option<'t>)` vs `Invoke(Option<int list>)`) already compiles today, because the pre-existing *prefer non-generic* rule (spec §14.4 step 7 rule 8) fires first; this tiebreaker is needed only when both candidates remain generic after rule 8 — including constructors and generic-type members.

# Detailed design

## Concreteness partial order

`compareTypeConcreteness` (`src/Compiler/Checking/OverloadResolutionRules.fs`) ranks two types as *more concrete*, *less concrete*, or *no preference*:

- A non-variable type is strictly more concrete than a bare non-SRTP type variable, whatever its own shape (`int`, `int list`, `'a option`, `int -> int` all beat a bare `'t`).
- Two bare type variables: no preference. Either side statically-resolved (`^T`): no preference (SRTP is excluded entirely).
- Same type constructor and arity (`Result<_,_>` vs `Result<_,_>`), tuples of equal arity, function types, anonymous records with the same fields, and equal-arity universally-quantified types are compared component-wise, recursively, under the dominance rule. By-reference parameters (`byref`/`inref`/`outref`) are one such application — they compare through their element type, and the by-reference kind tag itself is not ranked.
- Everything else — different constructor or arity, units of measure — is no preference.

**Dominance.** Over an ordered list of components (formal parameters, or the type arguments of a same-constructor application), one side dominates when it is strictly more concrete in at least one position and no position ranks the other strictly more concrete; equal or individually-incomparable positions are neutral. If each side wins some position they are incomparable. E.g. `Result<int,'error>` vs `Result<'ok,string>` — first wins position 1, second wins position 2 — so neither dominates.

## When it fires

`moreConcreteRule` is the **last** rule consulted during betterness. For a candidate pair it yields a preference only when all hold:

1. Both have the same number of formal parameters.
2. Both candidates' formal parameter types mention a comparable (non-SRTP) type variable — bound by a method type parameter *or* the enclosing generic type. The enclosing-type case is what lets constructors and generic-type members participate.
3. Neither candidate touches SRTP (no `^T` in its type parameters, type arguments, or parameters).

The parameter lists are then compared under dominance; a strict dominator wins, otherwise the call stays ambiguous. Two consequences of condition 2: if only one candidate is generic the rule does not fire (rule 8 decides); and it compares *formal parameter types*, not the *actual argument types* that rule 5 compares under subsumption — distinct orderings, and rule 5 runs first. SRTP is excluded because statically-resolved parameters are decided by trait-constraint solving, not specificity; the firing gate and the diagnostic share one `methodMentionsSRTP` helper so they cannot drift.

Constructor type-argument inference has no C# analogue (C# cannot infer a constructor's type arguments from its call arguments), so that case is an intentional F# superset. Because the tiebreak can pick `new(x: 'T option)` over `new(x: 'T)`, it can fix the result type (here `Wrapper<int>`) — as it already can for method overloads returning different types.

## Diagnostics

Two informational messages, **off by default** (`--warnon:3575` / `--warnon:3576`), make an otherwise-silent selection auditable; their `%s` placeholders render full signatures.

| Code | Message |
|------|---------|
| FS3575 | `Overload resolution preferred the more concrete overload '%s' over '%s' based on parameter type concreteness.` |
| FS3576 | `A more generic overload was bypassed: '%s'. The selected overload '%s' was chosen because it has more concrete type parameters.` |

When the tiebreaker cannot separate two candidates, `FS0041` is extended with a per-position breakdown (gated on the same feature, localized). For `Example.Compare(value: Result<int,'error>)` vs `Compare(value: Result<'ok,string>)` called with `Ok 42 : Result<int,string>`:

```text
error FS0041: A unique overload for method 'Compare' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: Result<int,string>

Candidates:
 - static member Example.Compare: value: Result<'ok,string> -> string
 - static member Example.Compare: value: Result<int,'error> -> string
Neither candidate is strictly more concrete than the other:
 - static member Example.Compare: value: Result<'ok,string> -> string is more concrete at position 2
 - static member Example.Compare: value: Result<int,'error> -> string is more concrete at position 1
```

Position numbering: with a single same-constructor parameter it is the 1-based *type-argument* index (as above); with several parameters it is the 1-based *formal-parameter* index, omitting parameters where neither wins. Only the top-level constructor is decomposed — incomparability nested inside a type argument (e.g. `Result<_,_> option`) falls back to a plain `FS0041`.

# Changes to the F# spec

Appends one rule to [§14.4 Method Application Resolution](https://fsharp.github.io/fslang-spec/) step 7, after the published *prefer non-generic* rule 8; rules 1–8 are unchanged:

```text
9) Prefer candidates with more concrete parameter types. Applies only when both candidates have an
   equal number of formal parameters, both mention a comparable type variable (from a method type
   parameter or the enclosing generic type), and neither involves an SRTP type variable. Then prefer
   the candidate whose formal parameter types dominate the other's: at least one position strictly
   more concrete and none strictly less concrete, under the partial order above.
```

At runtime the rule is sequenced after every betterness rule the compiler actually applies (including the internal type-directed-conversion, NullableOptionalInterop and property/override rules) and after the `OverloadResolutionPriorityAttribute` pre-filter (FS-1338); "rule 9" is the published-spec position, not a claim about runtime adjacency. Being last, it can only turn a former `FS0041` into a success — never override a resolution an existing rule settled. Within a declaring type, ORPA priority prunes before betterness, so a higher-priority overload is kept even if a lower-priority one is more concrete.

# Drawbacks

- **Silent behaviour change.** Code that failed with `FS0041` now compiles, and because the tiebreak can fix a generic result type, an expression's inferred type may change.
- **Adding overloads can change resolution.** A new *incomparable* overload can reintroduce ambiguity; a new *more concrete* one can change the selection; a new *more generic* one does not (it loses the tiebreak).

# Alternatives

- **Do nothing** — keep requiring annotations; the status-quo friction, especially against C#-shaped libraries.
- **Full C# "better function member" semantics** — larger and riskier; this tiebreaker is deliberately conservative.
- **`OverloadResolutionPriorityAttribute` (FS-1338)** — complementary and explicit; its pruning runs before this rule.

# Prior art

C# "better function member" (ECMA-334 §12.6.4.3) reaches its uninstantiated formal-parameter comparison only after the instantiated sequences tie and earlier tie-breakers fail, then prefers the more specific signature; this tiebreaker matches that intent for the common cases while comparing formal parameter types. Scala's most-specific rule and C++ partial ordering of function templates are analogous but not modelled.

# Compatibility

Not a breaking change: the rule fires only where today's compiler already produces `FS0041`, and only under `--langversion:preview`; below preview the call stays `FS0041`. No metadata, attribute, or IL change, so binaries are unaffected and FSharp.Core is unchanged.

**Portability.** An argument *type annotation* does not make such a call portable — `Api.Call(Some 42 : Option<int>)` still satisfies both overloads under older compilers (and resolves like the unannotated call under preview). Only an explicit type argument disambiguates portably: `Api.Call<int>(Some 42)` for methods, `Wrapper<int>(Some 5)` (the enclosing type's argument) for constructors and generic-type members.

# Interop

- **Consumed by another .NET language.** No effect — selection is compile-time only and emits no new metadata.
- **Related features.** For generic *methods* C# already resolves these cases via §12.6.4.3; this closes the gap without annotations. The constructor form has no C# analogue, so there F# is a superset rather than a parity fix.

# Pragmatics

- **Diagnostics:** FS3575/FS3576 (above, off by default) and the enhanced `FS0041` breakdown.
- **Tooling:** none; go-to-definition, tooltips and signature help operate on the selected overload, and calls that formerly showed `FS0041` now show the chosen member.
- **Performance:** negligible — reached only for an already-ambiguous pair, with fast-outs on parameter count, absence of a comparable type variable, and SRTP; the structural comparison is linear in signature size and early-exits on incomparability.
- **Scaling / Culture-aware formatting:** N/A.

# Unresolved questions

- Whether FS3575/FS3576 should ever become default-on in a later version.
- How the rule should compose with future type-directed-conversion additions that create new applicable-but-ambiguous candidates.
