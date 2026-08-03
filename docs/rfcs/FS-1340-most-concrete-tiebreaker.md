# F# RFC FS-1340 - "Most Concrete" Tiebreaker for Overload Resolution

The design suggestion ["Most concrete" tiebreaker for generic overloads](https://github.com/fsharp/fslang-suggestions/issues/905) has been marked "approved in principle".

This RFC covers the detailed proposal for this suggestion.

- [x] [Suggestion](https://github.com/fsharp/fslang-suggestions/issues/905)
- [x] Approved in principle
- [x] [Implementation](https://github.com/dotnet/fsharp/pull/19277)
- [ ] [Discussion](https://github.com/fsharp/fslang-design/discussions/FILL-ME-IN)

# Summary

This RFC introduces a new tiebreaker for F# overload resolution that prefers the "more concrete" overload when the pre-existing tiebreakers leave two candidates equally ranked. Today F# reports an `FS0041` ambiguity in cases where one overload is strictly more specific than another — for example a parameter typed `'T option` versus one typed `'T` — even when the argument type is fully known. The new tiebreaker resolves such cases to the more concrete overload, aligning F# with the intent of C#'s "better function member" rules and removing a common source of friction when consuming .NET libraries. The feature is gated behind `--langversion:preview`.

# Motivation

## Generic wrapper "smart constructors" — the constructor pain point

A very common F# pattern is a generic wrapper type that offers several constructors: one taking a bare value and others taking already-wrapped forms. A constructor's type parameters live on the *enclosing type*, and F# infers them from the arguments, so more than one constructor can be applicable. Today F# reports an ambiguity even when one parameter type is strictly more concrete:

```fsharp
type Wrapper<'T>(tag: string) =
    new(x: 'T)        = Wrapper<'T>("value")
    new(x: 'T option) = Wrapper<'T>("option")
    member _.Tag = tag

// Current: FS0041 (ambiguous) — Proposed: resolves to new('T option), Tag = "option"
let w = Wrapper(Some 5)
```

The same shape appears across the .NET BCL — notably `ValueTask<'T>`, whose `ValueTask(result: 'T)` / `ValueTask(task: Task<'T>)` pair motivated the original suggestion — and in widely used libraries:

- **TaskBuilder.fs** uses priority marker types to force resolution.
- **FsToolkit.ErrorHandling** splits extensions across modules for import ordering.

## Basic example (both candidates generic)

When neither pre-existing tiebreaker can separate two **equally generic** overloads, F# still errors even though one parameter is more concrete:

```fsharp
type Api =
    static member Call(x: 'T)        = "generic"
    static member Call(x: 'T option) = "concrete"

// Current: FS0041 — Proposed: resolves to Call('T option), "concrete"
let result = Api.Call(Some 42)
```

> Note: an overload set like `Invoke(Option<'t>)` vs `Invoke(Option<int list>)` — one fully generic, one fully concrete — already compiles *without* this feature, because the pre-existing *prefer non-generic over generic* rule (published spec §14.4 step 7 rule 8) selects the concrete overload before this tiebreaker is reached. That rule keys on whether a candidate has method type arguments at all; the new tiebreaker is needed when both candidates remain generic after it — including constructors and generic-type members, whose parameters mention an enclosing-type type variable rather than a method one.

# Detailed design

## The concreteness partial order

The feature defines a partial order on types by "concreteness". The comparison of two types returns *more concrete*, *less concrete*, or *no preference* (types that are equally concrete or structurally incomparable). It is implemented by `compareTypeConcreteness` in `src/Compiler/Checking/OverloadResolutionRules.fs`:

- A **non-SRTP type variable** (`'t`) is the least concrete form. A non-variable type is **strictly more concrete than a bare non-SRTP type variable, regardless of the non-variable's own shape.** Thus `int`, `int list`, `'a option`, and `int -> int` are all more concrete than a bare `'t`.
- **Two bare type variables** are equally concrete (no preference).
- If **either** side is a statically-resolved type variable (`^T`), the comparison yields no preference. SRTP is excluded entirely (see [SRTP exclusion](#srtp-exclusion)).
- **Two type applications of the same type constructor and the same arity** (e.g. `Result<_,_>` vs `Result<_,_>`) are compared **argument-wise** under the dominance rule below, recursively.
- **Tuples** of equal arity, **function types** (comparing domain and range), **anonymous records** with the same fields, and **universally-quantified types** of equal arity (comparing their bodies) are compared structurally in the same way.
- Everything else yields **no preference**: different type constructors, different arity, and unit-of-measure comparisons. `Option<int>` and `List<int>` are incomparable regardless of their arguments. A bare unit of measure yields no preference; a measure-annotated primitive such as `float<'u>` is an ordinary same-constructor application whose sole argument is a measure, which also yields no preference.

### The dominance rule

When comparing two ordered lists of components (either the formal parameters of two overloads, or the type arguments of two same-constructor applications), one side **dominates** the other when **at least one position ranks it strictly more concrete and no position ranks the other side strictly more concrete**. Positions that are equal, or individually incomparable, are **neutral** — they neither establish nor block dominance. If each side is strictly more concrete at some position, the two are incomparable and the comparison yields no preference. This aggregation is applied recursively, so dominance is evaluated at two levels: across the parameter list, and within each structural type across its components.

For example, comparing `Result<int,'error>` with `Result<'ok,string>`: the first is more concrete at position 1 (`int` vs `'ok`) and the second is more concrete at position 2 (`string` vs `'error`). Neither dominates, so the overloads remain ambiguous.

## When the tiebreaker fires

The tiebreaker is the **last** rule consulted during betterness (see [Changes to the F# spec](#changes-to-the-f-spec)). For a given pair of candidates it yields a preference only when **all** of the following hold (`moreConcreteRule` in `OverloadResolutionRules.fs`):

1. Both candidates have the **same number of formal parameters**.
2. **Both candidates' formal parameter types mention a comparable (non-SRTP) type variable** — bound either by a method type parameter or by the enclosing generic type. The enclosing-type case is what lets constructors and generic-type members participate, since their instantiation is inferred from the arguments and they carry no method type arguments.
3. **Neither candidate touches SRTP** — no `^T` method type parameter, called type argument, or parameter type.

When these hold, the candidates' parameter lists are compared under the dominance rule. A strict dominator is preferred; otherwise the tiebreaker yields no preference and the call remains ambiguous (with the enhanced diagnostic described below).

Two boundary conditions follow from condition 2:

- **Only one candidate generic ⇒ this rule does not fire.** If just one candidate's parameters mention a comparable type variable (e.g. `M(x: int)` vs `M(x: 'T)`, or even `M(x: int)` vs `M(x: 'T option)`), condition 2 fails and an earlier rule — typically *prefer non-generic* (rule 8) — decides. This rule only ever arbitrates between two candidates that both remain generic.
- **Formal parameters, not actual arguments.** This rule compares the candidates' *formal parameter types* under concreteness. The earlier *more specific actual argument types* rule (rule 5) compares the *actual argument types* under feasible subsumption. The two orderings are distinct and can disagree; rule 5 runs first and wins when it applies.

## Scope

The tiebreaker applies uniformly to any overloadable member whose formal parameter types mention a comparable type variable, regardless of where that type variable is bound:

- **Methods** (including interface methods) — the type variable is a method type parameter.
- **Constructors** and **generic-type members** (static or instance) — the type variable belongs to the enclosing generic type and is inferred from the arguments. `compareTypeConcreteness` treats an enclosing-type `'T` identically to a method-level `'T`, so these participate on the same footing once the firing condition is broadened from "the method has type arguments" to "a formal parameter mentions a comparable type variable".

Tuples and nested generics participate whenever the applicability step leaves more than one candidate whose parameter types mention a comparable type variable. Extension methods and `params`/optional-argument overloads are also in scope in principle, but only *reach* this rule in narrow cases: the extension-vs-intrinsic and open-recency preferences (rules 6–7) settle most *extension* ties first (F# intrinsic type augmentations are ordinary members, unaffected by rules 6–7, and reach this rule like any other method), and candidates that differ in `params`/optional-argument *arity* are separated earlier because this rule requires an equal formal-parameter count. `byref`/`inref`/`outref` parameters participate through their element type (they are ordinary type applications); the by-reference kind tag itself is not ranked.

Two consequences are worth calling out:

1. **Constructor type-argument inference is F#-only.** C# cannot infer a constructor's type arguments from its call arguments, so the constructor case has no C# analogue — it is an intentional F# superset, not a parity gap.
2. **The tiebreak can determine the expression's type, not just the implementation.** Choosing `new(x: 'T option)` over `new(x: 'T)` for `Wrapper(Some 5)` fixes the value's type at `Wrapper<int>`. This is not new to constructors: the tiebreaker already determines the result type for the method form (e.g. two `static member Make` overloads returning different types).

### SRTP exclusion

SRTP members are excluded (condition 3 above) because statically-resolved type parameters are resolved by trait-constraint solving, not by type-parameter specificity; mixing the two would produce incorrect results. The firing gate and the enhanced FS0041 diagnostic share one `methodMentionsSRTP` helper so the two cannot drift.

## Diagnostics

Two informational messages, **off by default**, let developers audit resolution decisions. Enable them with `--warnon:3575` / `--warnon:3576`.

| Code | Message | Default |
|------|---------|---------|
| FS3575 | `Overload resolution preferred the more concrete overload '%s' over '%s' based on parameter type concreteness. This is an informational message and can be enabled with --warnon:3575.` | Off |
| FS3576 | `A more generic overload was bypassed: '%s'. The selected overload '%s' was chosen because it has more concrete type parameters.` | Off |

The `%s` placeholders render as full method signatures, so the two candidates are always distinguishable even when they share a name.

### Enhanced ambiguity errors

When the tiebreaker cannot resolve two candidates (incomparable types), the standard FS0041 is extended with a per-position concreteness breakdown. The extra detail is gated behind the same language feature as the tiebreaker and is localized. For the incomparable `Result` example above the compiler emits:

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

(from `Example.Compare(value: Result<int,'error>)` vs `Example.Compare(value: Result<'ok,string>)` called with `Ok 42 : Result<int, string>`.)

**Reading "position".** For a call whose overloads take a **single parameter**, if that parameter is an application of the *same* top-level type constructor in both candidates (here both `Result<_,_>`), the position is the **1-based index of the differing type argument**: `Result<int,'error>` is more concrete at position 1 (`int` vs `'ok`) and `Result<'ok,string>` at position 2 (`string` vs `'error`). For overloads with **several parameters**, the position is instead the **1-based formal-parameter index**, and a parameter on which neither candidate is more concrete is omitted. The breakdown only decomposes the *top-level* type constructor of a parameter; incomparability buried in a nested type argument (e.g. `Result<_,_> option`) is not decomposed further, and such a call falls back to the plain `FS0041` message with no per-position detail.

## Resolved design questions

- **Interaction with `OverloadResolutionPriorityAttribute` (FS-1338).** The priority attribute is applied *before* betterness — it prunes the candidate set among the applicable members of each declaring type — so the concreteness tiebreaker only ever sees the post-priority survivors. Within a declaring type, a higher-priority overload is therefore kept even if a lower-priority one is more concrete. Priority pruning is **per declaring type**; it does not globally dominate across different declaring types.
- **Ordering relative to NullableOptionalInterop and property/override rules.** The concreteness tiebreaker is the last rule consulted (see below), so it can only convert a former `FS0041` ambiguity into a success. It never re-decides a resolution that an already-shipping rule settled.

# Changes to the F# spec

The change adds one rule to [§14.4 Method Application Resolution](https://fsharp.github.io/fslang-spec/), step 7. The published specification lists rules 1–8 (rule 8, *prefer non-generic*, is the last). The new rule is appended as published rule 9. Rules 1–8 are reproduced from the published specification for context (markdown backticks are omitted, since this is a plain-text diff):

```diff
     Otherwise, choose the unique best M~possible by applying the following criteria, in order:
         1) Prefer candidates whose use does not constrain the use of a user-introduced generic
         type annotation to be equal to another type.
         2) Prefer candidates that do not use ParamArray conversion. If two candidates both use
         ParamArray conversion with types pty1 and pty2, and pty1 feasibly subsumes pty2, prefer
         the second; that is, use the candidate that has the more precise type.
         3) Prefer candidates that do not have ImplicitlyReturnedFormalArgs.
         4) Prefer candidates that do not have ImplicitlySuppliedFormalArgs.
         5) If two candidates have unnamed actual argument types ty11 ... ty1n and ty21 ... ty2n, and
            each ty1i either
             - feasibly subsumes ty2i, or
             - ty2i is a System.Func type and ty1i is some other delegate type,
            then prefer the second candidate. That is, prefer any candidate that has the more
            specific actual argument types, and consider any System.Func type to be more specific
            than any other delegate type.
         6) Prefer candidates that are not extension members over candidates that are.
         7) To choose between two extension members, prefer the one that results from the most
         recent use of open.
         8) Prefer candidates that are not generic over candidates that are generic - that is, prefer
         candidates that have empty ActualArgTypes.
+        9) Prefer candidates with more concrete parameter types. This rule applies only when both
+           candidates have an equal number of formal parameters, both candidates' formal parameter
+           types mention a comparable type variable (bound by a method type parameter or the
+           enclosing generic type), and neither candidate involves a statically-resolved (SRTP)
+           type variable in its type parameters, type arguments, or parameters. When it applies,
+           prefer the candidate whose formal parameter types dominate the other's: at least one
+           position is strictly more concrete and no position is strictly less concrete, where
+           concreteness is the recursive partial order defined by this RFC.
-    Report an error if steps 1) through 8) do not result in the selection of a unique better method.
+    Report an error if steps 1) through 9) do not result in the selection of a unique better method.
```

**Relationship to the compiler's execution order.** The published specification lists only rules 1–8. The compiler additionally applies several rules that are not part of the published specification (including type-directed-conversion preferences, the F# 5.0 NullableOptionalInterop rule, and a property/override rule) and, before any betterness comparison, the `OverloadResolutionPriorityAttribute` pre-filter (FS-1338). In execution the concreteness rule is sequenced **after all of these**, i.e. it is genuinely the last tiebreaker consulted. "Rule 9" above is therefore the correct *published-spec* numbering, not a claim that it fires immediately after the non-generic rule at runtime.

# Drawbacks

- **Silent behavior change.** Code that previously failed with `FS0041` now compiles. Developers who relied on the error as a guardrail forcing explicit annotations will see overload selection happen implicitly, and — because the tiebreak can fix a generic result type — the inferred type of an expression may change.
- **Adding overloads can change resolution.** Adding an overload that is *incomparable* to an existing one can introduce a new ambiguity at existing call sites; adding an overload that is *more concrete* than the one previously selected can change which overload is chosen. (Adding a strictly *more generic* overload does not change resolution: it loses the tiebreak to the existing concrete candidate.)
- **Learning curve.** Developers must understand why `Result<int,'e>` vs `Result<'t,string>` remains ambiguous (neither dominates).

# Alternatives

1. **Do nothing.** Continue requiring explicit type annotations or named arguments to disambiguate. This is the status quo but creates friction, especially when consuming .NET libraries designed around C#'s resolution rules.
2. **Full C# semantics.** Implement all of C#'s "better function member" rules. This is a larger change with more risk of breaking existing F# code; the tiebreaker is more conservative.
3. **Attribute-based explicit priority.** `OverloadResolutionPriorityAttribute` (FS-1338) lets library authors mark preferred overloads. This is complementary: priority pruning runs before this tiebreaker, so explicit priority overrides implicit concreteness when both apply.

# Prior art

- **C# "better function member"** (ECMA-334 §12.6.4.3). C# reaches its *uninstantiated, unexpanded* formal-parameter comparison only after the instantiated parameter sequences are found equivalent and the earlier better-function-member tie-breakers have failed; at that point it prefers the more specific signature. This tiebreaker aligns with that intent for the common cases, comparing formal parameter types rather than the inferred instantiation, though F#'s trigger conditions differ.
- Other languages with overloading or implicit selection (e.g. Scala's most-specific rule, C++ partial ordering of function templates) also use specificity-based ordering; the mechanisms differ and are not modelled here.

# Compatibility

* **Is this a breaking change?** No. The tiebreaker only fires when the existing rules leave two candidates ambiguous — i.e. where current behavior produces `FS0041`. Code that compiles today resolves identically.
* **Previous compilers encountering this as source code.** The feature is gated behind `--langversion:preview`. Under an earlier language version the tiebreaker does not fire and the previously-ambiguous call remains an `FS0041` error, exactly as before.
* **Previous compilers encountering this in compiled binaries.** No effect. The tiebreaker changes only compile-time overload selection; it introduces no metadata, attribute, or IL-format change, so compiled assemblies are unchanged.
* **FSharp.Core.** No change to FSharp.Core is required.

### Portability

A call that this tiebreaker resolves remains `FS0041` under older compilers and `--langversion` values below preview. To disambiguate portably, supply an **explicit method type argument**; an argument *type annotation* is not sufficient, because the annotated value still satisfies both overloads:

```fsharp
// Resolved by the tiebreaker on a preview compiler:
let a = Api.Call(Some 42)

// Annotation buys nothing: on a preview compiler this resolves exactly like `a`
// (to Call(x: 'T option)); under older compilers it is still FS0041, because
// Some 42 : Option<int> stays applicable to both 'T and 'T option:
let b = Api.Call(Some 42 : Option<int>)

// Portable to all versions — pins the method type argument:
let c = Api.Call<int>(Some 42)   // selects Call(x: 'T option)
```

Constructors and generic-type members have no method type argument to supply; disambiguate them portably by giving the **enclosing type's** type argument instead:

```fsharp
// Portable — pins the enclosing type argument explicitly:
let w = Wrapper<int>(Some 5)   // selects new(x: 'T option)
```

# Interop

* **Consumed by another .NET language.** No effect. The tiebreaker only selects among candidates at an F# call site and emits no new metadata; the selected member is called through an ordinary method reference. A C# (or other) consumer of F# code sees nothing new.
* **Related features in other languages.** For generic *methods*, C#'s own overload resolution already resolves the motivating cases through §12.6.4.3; this feature narrows the gap so F# resolves those same BCL/library APIs the way C# callers do, without annotations. The constructor form has no C# analogue — C# cannot infer a constructor's type arguments from its call arguments — so there F# is an intentional superset, not a parity fix.

# Pragmatics

## Diagnostics

The informational messages FS3575/FS3576 (off by default) and the enhanced FS0041 breakdown described under [Detailed design](#diagnostics) are the diagnostics for this feature. FS3575/FS3576 make an otherwise-silent resolution auditable; the FS0041 detail explains *why* an ambiguity was not resolved (each candidate is more concrete at a different position), which points the developer at the explicit type argument they need to supply (a method type argument, or the enclosing type's argument for constructors).

## Tooling

No special tooling is required. Go-to-definition, tooltips, and signature help operate on the selected overload as they do for any resolved call. Because resolution now succeeds where it previously produced `FS0041`, IDE experiences that surfaced the ambiguity error instead show the chosen member. No colorization, brace-matching, or completion behavior changes.

## Performance

The tiebreaker is only reached for a candidate pair that the applicability step and all earlier rules left ambiguous. Before doing any structural work it applies cheap fast-outs: differing parameter counts, absence of a comparable type variable in either candidate's parameters, and any SRTP involvement all short-circuit to "no preference". The structural comparison itself is linear in the size of the two parameter-type trees and early-exits as soon as incomparability is detected. Per-method parameter data and SRTP results are cached within a single resolution. Code that already resolves to a unique overload is unaffected, because the rule is never consulted for it.

## Scaling

The relevant dimensions are the number of candidates in the method group and, per candidate pair, the number of formal parameters and the structural size (nesting depth and arity) of parameter types. A single comparison is linear in the combined size of the two signatures, with early exit on incomparability; betterness invokes it pairwise across the surviving candidates, as it does every tiebreak rule. Expected method groups and signatures are small (few overloads, a handful of parameters, shallow nesting); the compiler accepts arbitrarily large ones, with per-comparison cost linear in signature size.

## Culture-aware formatting/parsing

Not applicable. The feature performs no runtime formatting or parsing. Diagnostic text is compiler-localized through the standard resource mechanism; its only numeric output is small position indices, which render culture-invariantly.

# Unresolved questions

- Whether FS3575/FS3576 should ever graduate from off-by-default to a default-on informational level in a future language version, once the feature has seen real-world use.
- How the tiebreaker should compose with future additions to type-directed conversion, should those introduce new sources of applicable-but-ambiguous candidates.
