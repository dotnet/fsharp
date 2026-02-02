---
name: F# agent
description: Generic agent for F# coding following the coding guidelines of F# from MsLearn
---

# F# Code Generation

## Formatting
4 spaces, never tabs
offside rule: block lines must align

## Types
F# internal: DU, record, Option, Result, modules, functions
F# public API: DU, record, Option, Result, .fsi for surface, /// docs

## Patterns

```fsharp
// let
let x = 42
let y =
    expr
    + more

// match - all | align
match x with
| A -> ...
| B -> ...

// pipeline - each |> aligns
x
|> f
|> g

// record
{ Field1 = v1; Field2 = v2 }

// async/task
async { let! x = op(); return x }
```

## Rules
DO Option for absence. DON'T null.
DO Result for expected errors. DON'T exceptions in F# APIs.
DO immutable default. DON'T mutable default.
DO pattern match. DON'T if-else chains.
DO modules + functions. DON'T methods on records.
DO [<RequireQualifiedAccess>]. DON'T [<AutoOpen>] (except CE builders).
DO explicit .fsi for public API. DON'T implicit surface.
DO PascalCase: types, modules, fields. DO camelCase: functions, values, params.

## Domain modeling

```fsharp
// make illegal states unrepresentable
type Email = private Email of string
module Email =
    let create s = if valid s then Some(Email s) else None
    let value (Email s) = s

// workflow as type transformation
Unvalidated -> Validated -> Priced
```

## Performance
array: indexed access
list: small, functional ops
seq: lazy, large data
tail recursion for loops
