# Simple vs Reflection-based DU and Record printing

This document describes two modes for printing Discriminated Unions (DUs) and Records in F#: a **simple** reflection-free mode that delegates to a `string`-like operator for printing field values, and a `sprintf` mode (`sprintf "%A"`), which uses **reflection** to create output looking like F# code. In this document, the terms *simple* and *reflection* are used to distinguish the two modes.

Without the `--reflectionfree` flag, the compiler generates a `ToString` for DUs and Records that calls `sprintf "%A"`. With the flag, the compiler generates a `ToString` that uses the simple mode.

Users can choose between the two modes by 1. use of `--reflectionfree`, and by 2. calling with a `sprintf`-type caller or a `string`-type caller (e.g. the `string` operator, `ToString`, or interpolated strings).

If `x` is a DU or Record, then output will be simple or reflection-based as follows:
| | `--reflectionfree` | no `--reflectionfree` |
|---|---|---|
| `string x` | simple | reflection |
| `x.ToString()` | simple | reflection |
| `$"{x}"` | simple | reflection |
| `sprintf "%A" x` | disallowed (would be reflection) | reflection |

As such, the current default reflection `ToString` generation forces reflection formatting on all callers. On the other hand, generating simple `ToString` output means that the records and DUs are printed with simple or reflection formatting depending on whether the caller is of simple or reflection affinity. The `--reflectionfree` flag combines this property with a ban on `sprintf` to prevent the reflection mode from being used.

In addition to user-defined types, the FSharp.Core `option` type uses simple printing, while other types either have no `ToString` or use some other format.

## Behaviour: definitions

In simple printing, field values are printed with `string`-type formatting, more precisely `anyToStringShowingNull`. No line breaks are inserted.

- **Record**: `{ Name1 = value1; Name2 = value2 }`.
- **Anonymous record**: the same, but with `{| ` and ` |}`.
- **Union**: A case with no fields renders as just its name. A case with fields renders as `CaseName(value1, value2)`.

`[<Struct>]` records and unions, and struct anonymous records, render identically to their reference-type forms.

A type that supplies its own `ToString` override keeps it, with no `ToString` generated for it (either simple or reflection).

Reflection-mode printing is described in [plain text formatting](https://learn.microsoft.com/en-us/dotnet/fsharp/language-reference/plaintext-formatting).

## Behavioural differences

### Differences in field rendering

The following differences between `string` and `sprintf "%A"` carry over directly into differences in field rendering between simple and reflection printing:

| F# value | simple (`anyToStringShowingNull`) | reflection (`sprintf "%A"`) |
|---|---|---|
| string field `"hi"` | `hi` | `"hi"` |
| char field `'a'` | `a` | `'a'` |
| float `5.0` | `5` | `5.0` |
| `250uy` / `42n` / `1.5M` | `250` / `42` / `1.5` | `250uy` / `42n` / `1.5M` |
| option field `None` | `null` | `None` |
| array field `[\|1;2;3\|]` | `System.Int32[]` | `[\|1; 2; 3\|]` |
| unit field `()` | `null` | `()` |

The overall differences here are:
- Simple printing converts to strings, while reflection printing is more bi-directional, often generating compilable F# code.
- F# types that have null representation (`unit`, `option`, and in general types with `AllowNullLiteral` or `UseNullAsTrueValue`) are printed as `null` in simple printing, while reflection printing uses a more F#-like representation.

### Other differences

These differences are in the printing of the record or DU itself rather than of its fields:

| F# value | simple (`string`) | reflection (`sprintf "%A"`) |
|---|---|---|
| `B 5` (single field) | `B(5)` | `B 5` |
| `C (3, 4)` (two fields) | `C(3, 4)` | `C (3, 4)` |
| record `{ X = 1; Y = 2 }` | `{ X = 1; Y = 2 }` | `{ X = 1`⏎`  Y = 2 }` |
| `[<StructuredFormatDisplay("Custom<{X}>")>]` | `{ X = 5 }` | `Custom<5>` |

The overall differences here are:
- Simple printing always brackets a case's fields and never pads, while reflection printing omits brackets for a single non-tuple field and inserts a space before them otherwise.
- Simple printing uses a single line (unless a field's own rendering contains breaks), while reflection printing breaks records and nested values across lines with indentation.
- `StructuredFormatDisplay` is ignored in simple printing and honoured in reflection printing.

## Recursion and depth

Rendering recurses into nested records and unions. Deep nesting is guarded by `RuntimeHelpers.EnsureSufficientExecutionStack`, raising a catchable `InsufficientExecutionStackException` rather than `StackOverflowException`; cycles (which require mutation to construct) still overflow, as `option` and `list` do.