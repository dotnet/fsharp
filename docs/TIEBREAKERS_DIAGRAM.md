# Overload Resolution Flow Diagram

## High-Level Architecture

```
┌─────────────────────────────────────────────────────────────────────────────────┐
│                              F# COMPILER PIPELINE                                │
├─────────────────────────────────────────────────────────────────────────────────┤
│                                                                                 │
│   Source Code              Parsing              Type Checking                   │
│   ┌─────────┐            ┌─────────┐          ┌──────────────┐                 │
│   │  .fs    │───────────▶│  AST    │─────────▶│ ConstraintSolver │             │
│   │  file   │            │         │          │              │                 │
│   └─────────┘            └─────────┘          └──────┬───────┘                 │
│                                                      │                          │
│                                                      ▼                          │
│                                            ┌──────────────────┐                │
│                                            │ ResolveOverloading│                │
│                                            │                  │                │
│                                            │  ┌────────────┐  │                │
│                                            │  │  better()  │  │                │
│                                            │  │  function  │  │                │
│                                            │  └─────┬──────┘  │                │
│                                            │        │         │                │
│                                            │        ▼         │                │
│                                            │  ┌────────────┐  │                │
│                                            │  │ Tiebreaker │  │                │
│                                            │  │   Rules    │  │                │
│                                            │  │  (1-15)    │  │                │
│                                            │  └────────────┘  │                │
│                                            └──────────────────┘                │
│                                                                                 │
└─────────────────────────────────────────────────────────────────────────────────┘
```

## Overload Resolution Process Flow

```
                            ┌─────────────────────────┐
                            │   Method Call Site      │
                            │   Example.Invoke(arg)   │
                            └───────────┬─────────────┘
                                        │
                                        ▼
                            ┌─────────────────────────┐
                            │  Collect All Candidates │
                            │  (matching method name) │
                            └───────────┬─────────────┘
                                        │
                                        ▼
                            ┌─────────────────────────┐
                            │   Filter by Arity &     │
                            │   Argument Compatibility│
                            └───────────┬─────────────┘
                                        │
                                        ▼
                            ┌─────────────────────────┐
                            │  Apply Type Inference   │
                            │  to Each Candidate      │
                            └───────────┬─────────────┘
                                        │
                                        ▼
                    ┌───────────────────┴───────────────────┐
                    │                                       │
                    ▼                                       ▼
        ┌───────────────────┐                   ┌───────────────────┐
        │  0 or 1 Candidate │                   │ Multiple Candidates│
        │     Remaining     │                   │    Remaining      │
        └─────────┬─────────┘                   └─────────┬─────────┘
                  │                                       │
                  ▼                                       ▼
        ┌───────────────────┐               ┌─────────────────────────┐
        │  Done (or FS0041  │               │   TIEBREAKER RULES      │
        │  if 0 candidates) │               │   Pairwise Comparison   │
        └───────────────────┘               │   via better() function │
                                            └───────────┬─────────────┘
                                                        │
                                                        ▼
                                            ┌─────────────────────────┐
                                            │  For each pair (A, B):  │
                                            │  Evaluate Rules 1-15    │
                                            │  until one returns ≠0   │
                                            └───────────┬─────────────┘
                                                        │
                                    ┌───────────────────┼───────────────────┐
                                    │                   │                   │
                                    ▼                   ▼                   ▼
                            ┌─────────────┐    ┌─────────────┐    ┌─────────────┐
                            │   A wins    │    │   B wins    │    │   Tie (0)   │
                            │   (+1)      │    │   (-1)      │    │             │
                            └─────────────┘    └─────────────┘    └──────┬──────┘
                                                                         │
                                                                         ▼
                                                                ┌─────────────────┐
                                                                │ Try next rule   │
                                                                │ (if more rules) │
                                                                └─────────────────┘
```

## The 15 Tiebreaker Rules (TiebreakRuleId Enum)

Rules are defined as a strongly-typed enum with values 1-15 matching F# Language Spec §14.4:

```fsharp
[<RequireQualifiedAccess>]
type TiebreakRuleId =
    | NoTDC = 1              // Prefer no type-directed conversion
    | LessTDC = 2            // Prefer less type-directed conversion
    | NullableTDC = 3        // Prefer nullable-only TDC
    | NoWarnings = 4         // Prefer no "less generic" warnings
    | NoParamArray = 5       // Prefer no param array conversion
    | PreciseParamArray = 6  // Prefer precise param array type
    | NoOutArgs = 7          // Prefer no out args
    | NoOptionalArgs = 8     // Prefer no optional args
    | UnnamedArgs = 9        // Compare unnamed args (subsumption)
    | PreferNonExtension = 10 // Prefer non-extension methods
    | ExtensionPriority = 11  // Prefer recently opened extensions
    | PreferNonGeneric = 12   // Prefer non-generic methods
    | MoreConcrete = 13       // ★ NEW: Prefer more concrete instantiations
    | NullableOptionalInterop = 14 // F# 5.0 all-args comparison
    | PropertyOverride = 15   // Prefer more derived property type
```

```
╔═══════════════════════════════════════════════════════════════════════════════════╗
║                         TIEBREAKER RULES (Priority Order)                         ║
╠═══════════════════════════════════════════════════════════════════════════════════╣
║                                                                                   ║
║  ┌─────────────────────────────────────────────────────────────────────────────┐ ║
║  │ TiebreakRuleId.NoTDC = 1                                                    │ ║
║  │ Prefer methods that don't use type-directed conversion                      │ ║
║  └─────────────────────────────────────────────────────────────────────────────┘ ║
║                                        │                                         ║
║                                        ▼                                         ║
║  ┌─────────────────────────────────────────────────────────────────────────────┐ ║
║  │ TiebreakRuleId.LessTDC = 2                                                  │ ║
║  │ Prefer methods that need less type-directed conversion                      │ ║
║  └─────────────────────────────────────────────────────────────────────────────┘ ║
║                                        │                                         ║
║                                        ▼                                         ║
║  ┌─────────────────────────────────────────────────────────────────────────────┐ ║
║  │ TiebreakRuleId.NullableTDC = 3                                                         │ ║
║  │ Prefer methods with only nullable type-directed conversions                 │ ║
║  └─────────────────────────────────────────────────────────────────────────────┘ ║
║                                        │                                         ║
║                                        ▼                                         ║
║  ┌─────────────────────────────────────────────────────────────────────────────┐ ║
║  │ TiebreakRuleId.NoWarnings = 4                                                          │ ║
║  │ Prefer methods that don't give "less generic" warnings                      │ ║
║  └─────────────────────────────────────────────────────────────────────────────┘ ║
║                                        │                                         ║
║                                        ▼                                         ║
║  ┌─────────────────────────────────────────────────────────────────────────────┐ ║
║  │ TiebreakRuleId.NoParamArray = 5                                                        │ ║
║  │ Prefer methods that don't use param array conversion                        │ ║
║  └─────────────────────────────────────────────────────────────────────────────┘ ║
║                                        │                                         ║
║                                        ▼                                         ║
║  ┌─────────────────────────────────────────────────────────────────────────────┐ ║
║  │ TiebreakRuleId.PreciseParamArray = 6                                                   │ ║
║  │ Prefer methods with more precise param array element type                   │ ║
║  └─────────────────────────────────────────────────────────────────────────────┘ ║
║                                        │                                         ║
║                                        ▼                                         ║
║  ┌─────────────────────────────────────────────────────────────────────────────┐ ║
║  │ TiebreakRuleId.NoOutArgs = 7                                                           │ ║
║  │ Prefer methods that don't use out args                                      │ ║
║  └─────────────────────────────────────────────────────────────────────────────┘ ║
║                                        │                                         ║
║                                        ▼                                         ║
║  ┌─────────────────────────────────────────────────────────────────────────────┐ ║
║  │ TiebreakRuleId.NoOptionalArgs = 8                                                      │ ║
║  │ Prefer methods that don't use optional args                                 │ ║
║  └─────────────────────────────────────────────────────────────────────────────┘ ║
║                                        │                                         ║
║                                        ▼                                         ║
║  ┌─────────────────────────────────────────────────────────────────────────────┐ ║
║  │ TiebreakRuleId.UnnamedArgs = 9                                                         │ ║
║  │ Compare unnamed args using subsumption ordering (dominance)                 │ ║
║  └─────────────────────────────────────────────────────────────────────────────┘ ║
║                                        │                                         ║
║                                        ▼                                         ║
║  ┌─────────────────────────────────────────────────────────────────────────────┐ ║
║  │ TiebreakRuleId.PreferNonExtension = 10                                                 │ ║
║  │ Prefer non-extension methods over extension methods                         │ ║
║  └─────────────────────────────────────────────────────────────────────────────┘ ║
║                                        │                                         ║
║                                        ▼                                         ║
║  ┌─────────────────────────────────────────────────────────────────────────────┐ ║
║  │ TiebreakRuleId.ExtensionPriority = 11                                                  │ ║
║  │ Between extensions, prefer most recently opened                             │ ║
║  └─────────────────────────────────────────────────────────────────────────────┘ ║
║                                        │                                         ║
║                                        ▼                                         ║
║  ┌─────────────────────────────────────────────────────────────────────────────┐ ║
║  │ TiebreakRuleId.PreferNonGeneric = 12                                                   │ ║
║  │ Prefer non-generic methods over generic methods                             │ ║
║  └─────────────────────────────────────────────────────────────────────────────┘ ║
║                                        │                                         ║
║                                        ▼                                         ║
║  ┌─────────────────────────────────────────────────────────────────────────────┐ ║
║  │ TiebreakRuleId.MoreConcrete = 13  ★ NEW (RFC FS-XXXX) ★                                │ ║
║  │ Prefer more concrete type instantiations                                    │ ║
║  │ Example: Option<int> beats Option<'t>                                       │ ║
║  │ ⚠️  Only when BOTH methods are generic                                      │ ║
║  │ ⚠️  Skipped for SRTP methods                                                │ ║
║  └─────────────────────────────────────────────────────────────────────────────┘ ║
║                                        │                                         ║
║                                        ▼                                         ║
║  ┌─────────────────────────────────────────────────────────────────────────────┐ ║
║  │ TiebreakRuleId.NullableOptionalInterop = 14                                            │ ║
║  │ F# 5.0 rule - compare all args including optional/named                     │ ║
║  └─────────────────────────────────────────────────────────────────────────────┘ ║
║                                        │                                         ║
║                                        ▼                                         ║
║  ┌─────────────────────────────────────────────────────────────────────────────┐ ║
║  │ TiebreakRuleId.PropertyOverride = 15                                                   │ ║
║  │ For properties, prefer more derived type                                    │ ║
║  └─────────────────────────────────────────────────────────────────────────────┘ ║
║                                        │                                         ║
║                                        ▼                                         ║
║                              ┌───────────────────┐                               ║
║                              │  All rules = 0?   │                               ║
║                              │  → FS0041 Error   │                               ║
║                              │  (Ambiguous)      │                               ║
║                              └───────────────────┘                               ║
║                                                                                   ║
╚═══════════════════════════════════════════════════════════════════════════════════╝
```

## Rule 13: MoreConcrete - Detail View

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                          TiebreakRuleId.MoreConcrete = 13 (Detail)                             │
├─────────────────────────────────────────────────────────────────────────────────────┤
│                                                                                     │
│   Entry Conditions:                                                                 │
│   ┌─────────────────────────────────────────────────────────────────────────────┐  │
│   │ ✓ LanguageFeature.MoreConcreteTiebreaker enabled (F# 10.0+)                 │  │
│   │ ✓ BOTH candidates have non-empty CalledTyArgs (both are generic)            │  │
│   │ ✓ Neither method has SRTP type parameters                                   │  │
│   │ ✓ No SRTP type variables in formal parameters                               │  │
│   └─────────────────────────────────────────────────────────────────────────────┘  │
│                                        │                                            │
│                                        ▼                                            │
│   ┌─────────────────────────────────────────────────────────────────────────────┐  │
│   │              Get Formal (Uninstantiated) Parameter Types                    │  │
│   │                      via FormalMethodInst                                   │  │
│   └───────────────────────────────────┬─────────────────────────────────────────┘  │
│                                       │                                             │
│                                       ▼                                             │
│   ┌─────────────────────────────────────────────────────────────────────────────┐  │
│   │           Compare Each Corresponding Parameter Pair                         │  │
│   │                   using compareTypeConcreteness                             │  │
│   └───────────────────────────────────┬─────────────────────────────────────────┘  │
│                                       │                                             │
│                                       ▼                                             │
│   ┌─────────────────────────────────────────────────────────────────────────────┐  │
│   │                     aggregateComparisons (Dominance)                        │  │
│   │  ┌─────────────────────────────────────────────────────────────────────┐   │  │
│   │  │ All ≥0 and some >0  →  Return +1 (candidate wins)                   │   │  │
│   │  │ All ≤0 and some <0  →  Return -1 (other wins)                       │   │  │
│   │  │ Mixed or all =0     →  Return  0 (incomparable)                     │   │  │
│   │  └─────────────────────────────────────────────────────────────────────┘   │  │
│   └─────────────────────────────────────────────────────────────────────────────┘  │
│                                                                                     │
└─────────────────────────────────────────────────────────────────────────────────────┘
```

## compareTypeConcreteness Algorithm

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                      compareTypeConcreteness(ty1, ty2)                              │
├─────────────────────────────────────────────────────────────────────────────────────┤
│                                                                                     │
│                            ┌───────────────────┐                                    │
│                            │  stripTyEqns(ty)  │                                    │
│                            │  (normalize type) │                                    │
│                            └─────────┬─────────┘                                    │
│                                      │                                              │
│                                      ▼                                              │
│          ┌───────────────────────────┴───────────────────────────┐                 │
│          │                    Match Type Form                     │                 │
│          └───────────────────────────┬───────────────────────────┘                 │
│                                      │                                              │
│     ┌────────────┬────────────┬──────┴──────┬────────────┬────────────┐            │
│     ▼            ▼            ▼             ▼            ▼            ▼            │
│ ┌────────┐  ┌────────┐  ┌──────────┐  ┌──────────┐ ┌──────────┐ ┌──────────┐      │
│ │TType_  │  │TType_  │  │TType_app │  │TType_    │ │TType_fun │ │TType_    │      │
│ │var vs  │  │var vs  │  │ (same    │  │tuple     │ │(function)│ │anon      │      │
│ │TType_  │  │concrete│  │ tcref)   │  │          │ │          │ │(record)  │      │
│ │var     │  │        │  │          │  │          │ │          │ │          │      │
│ └───┬────┘  └───┬────┘  └────┬─────┘  └────┬─────┘ └────┬─────┘ └────┬─────┘      │
│     │           │            │             │            │            │             │
│     ▼           ▼            ▼             ▼            ▼            ▼             │
│ ┌────────┐  ┌────────┐  ┌──────────┐  ┌──────────┐ ┌──────────┐ ┌──────────┐      │
│ │Return 0│  │concrete│  │Recurse on│  │Recurse on│ │Recurse on│ │Recurse on│      │
│ │(equal) │  │wins    │  │type args │  │elements  │ │dom + rng │ │fields    │      │
│ │        │  │(-1/+1) │  │aggregate │  │aggregate │ │aggregate │ │aggregate │      │
│ └────────┘  └────────┘  └──────────┘  └──────────┘ └──────────┘ └──────────┘      │
│                                                                                     │
│   Note: SRTP type variables (^T) always return 0 (excluded from comparison)        │
│                                                                                     │
└─────────────────────────────────────────────────────────────────────────────────────┘
```

## Type Concreteness Hierarchy

```
                    ┌─────────────────────────────────────┐
                    │         MOST CONCRETE               │
                    │                                     │
                    │   int, string, MyClass, etc.        │
                    │   (Fully instantiated types)        │
                    │                                     │
                    └──────────────────┬──────────────────┘
                                       │
                                       ▼
                    ┌─────────────────────────────────────┐
                    │                                     │
                    │   Option<int>, List<string>         │
                    │   Result<int, Exception>            │
                    │   (Generic apps with concrete args) │
                    │                                     │
                    └──────────────────┬──────────────────┘
                                       │
                                       ▼
                    ┌─────────────────────────────────────┐
                    │                                     │
                    │   Option<'t>, List<'a>              │
                    │   (Generic apps with type vars)     │
                    │                                     │
                    └──────────────────┬──────────────────┘
                                       │
                                       ▼
                    ┌─────────────────────────────────────┐
                    │         LEAST CONCRETE              │
                    │                                     │
                    │   't, 'a, 'TResult                  │
                    │   (Bare type variables)             │
                    │                                     │
                    └─────────────────────────────────────┘


    Example Comparisons:
    ┌──────────────────────────────────────────────────────────────────┐
    │  Option<int>     vs  Option<'t>     →  Option<int> wins (+1)     │
    │  Option<int>     vs  Option<string> →  Incomparable (0)          │
    │  Result<int,'e>  vs  Result<'t,str> →  Incomparable (mixed)      │
    │  Option<'t>      vs  List<'t>       →  Incomparable (diff tcref) │
    │  'a              vs  int            →  int wins (-1)             │
    │  'a              vs  'b             →  Equal (0)                 │
    └──────────────────────────────────────────────────────────────────┘
```

## File Structure

```
src/Compiler/
├── Checking/
│   ├── ConstraintSolver.fs          ◄── ResolveOverloading, better()
│   ├── OverloadResolutionRules.fs   ◄── Rule definitions, compareTypeConcreteness
│   └── OverloadResolutionRules.fsi  ◄── Public API
├── Facilities/
│   ├── LanguageFeatures.fs          ◄── MoreConcreteTiebreaker feature flag
│   └── LanguageFeatures.fsi
├── Driver/
│   └── CompilerDiagnostics.fs       ◄── FS3575/FS3576 off-by-default config
└── FSComp.txt                       ◄── Diagnostic messages

tests/FSharp.Compiler.ComponentTests/
└── Conformance/Tiebreakers/
    └── TiebreakerTests.fs           ◄── 95 test cases
```

## Diagnostic Flow

```
┌─────────────────────────────────────────────────────────────────────────────────────┐
│                           DIAGNOSTIC EMISSION                                       │
├─────────────────────────────────────────────────────────────────────────────────────┤
│                                                                                     │
│   Rule 13 returns +1 or -1 (resolved via concreteness)                             │
│                          │                                                          │
│                          ▼                                                          │
│   ┌───────────────────────────────────────────────────────────────────────────┐    │
│   │  wasDecidedByRule("MoreConcrete", ...) returns true                       │    │
│   └───────────────────────────────────────────────────────────────────────────┘    │
│                          │                                                          │
│           ┌──────────────┴──────────────┐                                          │
│           ▼                             ▼                                          │
│   ┌───────────────────┐       ┌───────────────────┐                                │
│   │ --warnon:3575 set?│       │ --warnon:3576 set?│                                │
│   └─────────┬─────────┘       └─────────┬─────────┘                                │
│             │                           │                                          │
│      ┌──────┴──────┐             ┌──────┴──────┐                                   │
│      ▼             ▼             ▼             ▼                                   │
│   ┌─────┐       ┌─────┐       ┌─────┐       ┌─────┐                                │
│   │ Yes │       │ No  │       │ Yes │       │ No  │                                │
│   └──┬──┘       └──┬──┘       └──┬──┘       └──┬──┘                                │
│      │             │             │             │                                    │
│      ▼             ▼             ▼             ▼                                    │
│ ┌─────────┐   ┌─────────┐  ┌─────────┐   ┌─────────┐                               │
│ │ Emit    │   │ Silent  │  │ Emit    │   │ Silent  │                               │
│ │ FS3575  │   │         │  │ FS3576  │   │         │                               │
│ │         │   │         │  │ (for    │   │         │                               │
│ │"selected│   │         │  │ each    │   │         │                               │
│ │ '%s'    │   │         │  │ loser)  │   │         │                               │
│ │ based on│   │         │  │         │   │         │                               │
│ │concrete-│   │         │  │         │   │         │                               │
│ │ness..." │   │         │  │         │   │         │                               │
│ └─────────┘   └─────────┘  └─────────┘   └─────────┘                               │
│                                                                                     │
│   Default: Both diagnostics OFF (informational only)                               │
│   Enable for debugging: fsc --warnon:3575 --warnon:3576 ...                        │
│                                                                                     │
└─────────────────────────────────────────────────────────────────────────────────────┘
```
