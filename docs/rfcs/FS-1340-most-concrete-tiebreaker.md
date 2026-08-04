# F# RFC FS-1340 - "Most concrete" tiebreaker for overload resolution

The design suggestion ["Most concrete" tiebreaker for generic overloads](https://github.com/fsharp/fslang-suggestions/issues/905) has been marked "approved in principle". This RFC covers the detailed proposal for this suggestion.

- [x] [Suggestion](https://github.com/fsharp/fslang-suggestions/issues/905)
- [x] Approved in principle
- [x] [Implementation](https://github.com/dotnet/fsharp/pull/19277)
- [ ] [Discussion](https://github.com/fsharp/fslang-design/discussions/FILL-ME-IN)

# Summary

This RFC adds a last-resort tiebreaker to overload resolution. When the existing rules leave two candidates equally ranked and one of them has strictly more concrete formal parameter types (for example `'T option` rather than `'T`), the more concrete candidate is preferred instead of reporting `FS0041`. This matches the intent of C#'s "better function member" rules for the common generic-wrapper case. The feature is gated behind `--langversion:preview`.

# Motivation

Generic wrapper types commonly provide one constructor that takes a bare value and others that take an already-wrapped value. A constructor's type parameters belong to the enclosing type and are inferred from the arguments, so more than one constructor is applicable and F# reports an ambiguity, even when one parameter type is strictly more concrete than the other:

```fsharp
type Wrapper<'T>(tag: string) =
    new(x: 'T)        = Wrapper<'T>("value")
    new(x: 'T option) = Wrapper<'T>("option")

// Today: FS0041. Proposed: resolves to new('T option).
let w = Wrapper(Some 5)
```

The same shape occurs on `ValueTask<'T>`, which has both a `ValueTask('T)` and a `ValueTask(Task<'T>)` constructor and which motivated the original suggestion. It also drives the marker-argument workarounds in TaskBuilder.fs and FsToolkit.ErrorHandling. The method form (`Call(x: 'T)` versus `Call(x: 'T option)`) behaves identically.

A related case already compiles today: when one candidate is fully generic and the other is fully concrete, such as `Invoke(Option<'t>)` versus `Invoke(Option<int list>)`. The existing *prefer non-generic* rule (spec §14.4 step 7 rule 8) fires first there. This tiebreaker is only needed when both candidates are still generic after rule 8, which includes constructors and other members of a generic type.

# Detailed design

## Concreteness ordering

Two types are ranked as *more concrete*, *less concrete*, or *no preference*, as follows.

- A non-variable type is strictly more concrete than a bare non-SRTP type variable, regardless of its own shape. `int`, `int list`, `'a option`, and `int -> int` are all more concrete than a bare `'t`.
- Two bare type variables give no preference. A statically-resolved type variable (`^T`) on either side also gives no preference; SRTP is excluded entirely.
- Two applications of the same type constructor and arity (`Result<_,_>` versus `Result<_,_>`), tuples of equal arity, function types, anonymous records with the same fields, and universally-quantified types of equal arity are compared position by position and recursively, under the dominance rule below. By-reference parameters (`byref`, `inref`, `outref`) are ordinary type applications and so compare through their element type; the by-reference kind itself is not ranked.
- Everything else gives no preference. This includes different constructors, different arities, and units of measure.

**Dominance.** Given an ordered list of components (the formal parameters of two candidates, or the type arguments of a same-constructor application), one side dominates the other when it is strictly more concrete in at least one position and no position ranks the other side strictly more concrete. Positions that are equal or individually incomparable are neutral. If each side wins at least one position, the two are incomparable. For example, `Result<int,'error>` and `Result<'ok,string>` are incomparable: the first wins position 1 and the second wins position 2.

## When the rule fires

The most-concrete rule is the last rule consulted during betterness. For a given pair of candidates it produces a preference only when all of the following hold:

1. Both candidates declare the same number of formal parameters. This is the declared arity: a `ParamArray` parameter counts once, and each optional or defaulted parameter counts once; the expanded call-site form is not used.
2. The formal parameter types of both candidates mention a comparable (non-SRTP) type variable, bound either by a method type parameter or by the enclosing generic type. The enclosing-type case is what allows constructors and other members of a generic type to take part.
3. Neither candidate involves an SRTP type variable (no `^T` in its type parameters, type arguments, or parameters).

The two parameter lists are then compared under dominance. A strict dominator wins; otherwise the call remains ambiguous.

The comparison is on the *uninstantiated formal parameter types*, not the *actual argument types* that rule 5 compares under subsumption, and rule 5 runs first. If only one candidate is generic the rule does not fire, because rule 8 has already decided.

Extension members take part only after the existing intrinsic-over-extension and extension-scope preferences. The rule can at most break a tie those leave, for example between two extension members in the same scope.

With three or more candidates the rule composes with ordinary betterness in the usual way: a candidate is selected only if it is strictly preferred over every other applicable candidate. If several candidates are pairwise incomparable, none is uniquely best and the call remains `FS0041`.

Because the rule can prefer `new(x: 'T option)` over `new(x: 'T)`, it can also fix the result type (here `Wrapper<int>`), just as it already can for method overloads that return different types.

## Diagnostics

Two informational messages are added, both off by default (`--warnon:3575` and `--warnon:3576`), so that an otherwise-silent selection can be inspected on demand. Their `%s` placeholders render full signatures.

| Code | Message |
|------|---------|
| FS3575 | `Overload resolution preferred the more concrete overload '%s' over '%s' based on parameter type concreteness.` |
| FS3576 | `A more generic overload was bypassed: '%s'. The selected overload '%s' was chosen because it has more concrete type parameters.` |

When the tiebreaker cannot separate two candidates, the `FS0041` message is extended with a per-position breakdown (under the same feature, localized). For `Example.Compare(value: Result<int,'error>)` versus `Compare(value: Result<'ok,string>)` called with `Ok 42 : Result<int,string>`:

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

The position numbering depends on the shape. With a single same-constructor parameter it is the 1-based *type-argument* index, as above. With several parameters it is the 1-based *formal-parameter* index, and parameters where neither candidate wins are omitted. Only the top-level constructor is decomposed; incomparability nested inside a type argument (for example `Result<_,_> option`) falls back to a plain `FS0041`.

# Changes to the F# spec

This appends one rule to [§14.4 Method Application Resolution](https://fsharp.github.io/fslang-spec/) step 7, after the existing *prefer non-generic* rule 8. Rules 1 to 8 are unchanged.

```text
9) Prefer candidates with more concrete parameter types. Applies only when both candidates have an
   equal number of formal parameters, both mention a comparable type variable (from a method type
   parameter or the enclosing generic type), and neither involves an SRTP type variable. Then prefer
   the candidate whose formal parameter types dominate the other's: at least one position strictly
   more concrete and none strictly less concrete, under the ordering above.
```

At runtime the rule runs after every betterness preference the compiler applies and after the `OverloadResolutionPriorityAttribute` pre-filter (FS-1338). "Rule 9" therefore describes the position in the published spec, not the runtime order. Because it runs last, it can only turn a former `FS0041` into a success; it never overrides a resolution that an earlier rule has already settled. Within a single declaring type, `OverloadResolutionPriorityAttribute` pruning runs before betterness, so a higher-priority overload is kept even when a lower-priority one is more concrete.

# Drawbacks

- This is a silent behaviour change. Code that previously failed with `FS0041` now compiles, and because the rule can fix a generic result type, an expression's inferred type can change.
- Adding overloads can change resolution. A new incomparable overload can reintroduce ambiguity, and a new more concrete overload can change the selection. A new more generic overload does not, because it loses the tiebreak.

# Alternatives

- **Do nothing.** Keep requiring annotations. This preserves the status-quo friction, especially against C#-shaped libraries.
- **Adopt full C# "better function member" semantics.** Larger and riskier. This tiebreaker covers only the generic-wrapper case.
- **`OverloadResolutionPriorityAttribute` (FS-1338).** Complementary and explicit. Its pruning runs before this rule.

# Prior art

C#'s "better function member" rule (ECMA-334 §12.6.4.3) reaches its uninstantiated formal-parameter comparison only after the instantiated argument sequences tie and the earlier tiebreakers fail, and then prefers the more specific signature. This tiebreaker matches that intent for the common cases while comparing formal parameter types. Scala's most-specific rule and C++ partial ordering of function templates are analogous but are not modelled here.

# Compatibility

This is not a breaking change. The rule fires only where the current compiler already produces `FS0041`, and only under `--langversion:preview`; below preview the call still fails with `FS0041`. There is no change to metadata, attributes, or IL, so binaries are unaffected and FSharp.Core is unchanged.

**Portability.** An argument type annotation does not make such a call portable to older compilers. `Api.Call(Some 42 : Option<int>)` still satisfies both overloads under an older compiler, and under preview it resolves the same way as the unannotated call. Only an explicit type argument disambiguates portably: `Api.Call<int>(Some 42)` for methods, and `Wrapper<int>(Some 5)` (the enclosing type's argument) for constructors and other members of a generic type.

# Interop

- **Consumed by another .NET language.** No effect. Selection happens at compile time and emits no new metadata.
- **Related features.** For generic methods, C# already resolves these cases through §12.6.4.3, so this closes the gap without annotations. The constructor form has no C# analogue, so there F# is a superset rather than a parity fix.

# Pragmatics

- **Diagnostics.** FS3575 and FS3576 above (off by default), plus the extended `FS0041` breakdown.
- **Tooling.** No new tooling. Go-to-definition, tooltips, and signature help all operate on the selected overload, and calls that previously showed `FS0041` now show the chosen member.
- **Performance.** Negligible. The rule is reached only for an already-ambiguous pair, and it exits early on parameter count, on the absence of a comparable type variable, and on SRTP. The structural comparison is linear in the size of the signature.
- **Scaling and culture-aware formatting.** Not applicable.

# Unresolved questions

- Whether FS3575 and FS3576 should ever become on by default in a later version.
- How the rule should compose with future type-directed-conversion additions that introduce new applicable-but-ambiguous candidates.
