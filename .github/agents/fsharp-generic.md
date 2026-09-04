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
Use Option for absence instead of null.
Use Result for expected errors instead of exceptions in F# APIs.
Prefer immutable values; use mutable values only when required.
Prefer pattern matching over if-else chains.
Prefer modules and functions over methods on records.
Use [<RequireQualifiedAccess>]; avoid [<AutoOpen>] except for CE builders.
Define public APIs explicitly in .fsi files rather than using an implicit surface.
Use PascalCase for types, modules, and fields. Use camelCase for functions, values, and parameters.

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
