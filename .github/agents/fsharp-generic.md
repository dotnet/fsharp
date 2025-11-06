---
name: F# agent
description: Generic agent for F# coding following the coding guidelines of F# from MsLearn
---

# F# Code Generation Instructions

## CRITICAL: Indentation and Formatting Rules

### Absolute Requirements

**SPACES ONLY - NEVER TABS:**
- Use 4 spaces per indentation level
- Tabs cause compiler errors in F#
- Consistency mandatory across entire file

**The Offside Rule:**
F# uses significant whitespace. Once you establish an indentation level, all subsequent lines in that block MUST align at that exact column.

### Indentation Patterns

**Let Bindings:**
```fsharp
let x = 42
let y =
    someExpression
    + another

let result =
    let inner = 10
    inner + 20
```

**Functions:**
```fsharp
let add a b = a + b

let processData input =
    let validated = validate input
    let transformed = transform validated
    save transformed
```

**Pattern Matching - All | align, bodies indent 4 spaces:**
```fsharp
let describe x =
    match x with
    | 0 -> "zero"
    | 1 -> "one"
    | _ -> "other"

// Multiline arms
let complexMatch x =
    match x with
    | Some value ->
        printfn "Found: %d" value
        value * 2
    | None ->
        printfn "Not found"
        0
```

**If/Then/Else:**
```fsharp
let x = if condition then a else b

// Multiline
let result =
    if condition then
        doSomething()
    else
        doOtherThing()
```

**Pipelines - Each |> at same level:**
```fsharp
let result =
    input
    |> validate
    |> transform
    |> save

// With lambda
let result =
    input
    |> List.map (fun x ->
        compute x * 2)
```

**Records:**
```fsharp
type Person = 
    { 
        Name: string
        Age: int
        Email: string 
    }

let person =
    {
        Name = "Alice"
        Age = 30
        Email = "alice@example.com"
    }

let updated = { person with Age = 31 }
```

**Lists and Arrays:**
```fsharp
let numbers = [1; 2; 3; 4; 5]

// Multiline
let numbers = [
    1
    2
    3
]

let squares = [ for x in 1 .. 10 -> x * x ]
```

**Computation Expressions:**
```fsharp
let fetchData url =
    async {
        let! response = Http.get url
        let! content = response.ReadAsStringAsync()
        return parse content
    }
```

**Function Application:**
```fsharp
let result = myFunction arg1 arg2

// Long arguments
let result =
    myLongFunctionName
        argument1
        argument2
        argument3
```

### Whitespace Rules

**DO:**
- One space after commas: `(1, 2, 3)`
- One space around operators: `x + y`
- Blank line between functions
- `spam (ham 1)` - space between function and args

**DO NOT:**
- Spaces inside parentheses: `spam( ham 1 )` ❌
- Align by variable name length (fragile) ❌
- Use tabs ❌

```fsharp
// CORRECT
let shortName = value1
let veryLongName = value2

// WRONG - aligned by name length
let shortName    = value1
let veryLongName = value2
```

### Comments
```fsharp
// Use // for inline comments

/// Use /// for XML documentation on public APIs
let publicFunction x = x + 1
```

## Core Principles

Generate F# code following five principles:
1. **Succinct, expressive, composable** - Minimal boilerplate, clear intent, natural composition
2. **Interoperable** - Consider .NET language consumption
3. **Object programming selectively** - Use OOP to encapsulate complexity, not as default
4. **Performance without exposed mutation** - Hide mutation behind functional interfaces
5. **Toolable** - Compatible with F# tooling and formatters

## API Design by Consumer Context

### Context 1: Internal/Private F# Code

**Types:**
- Discriminated unions for domain modeling
- Record types for data structures
- `Option<'T>` for absent values
- `Result<'T, 'TError>` for expected failures
- Single-case unions for type-safe primitive wrappers

**Functions:**
- Organize in modules, not classes
- Function composition and pipelines
- Computation expressions (async, result, option)
- Active patterns for complex matching

**Organization:**
- `[<RequireQualifiedAccess>]` to prevent name collisions
- `[<AutoOpen>]` only for computation builders or critical helpers
- Keep mutation local and hidden

### Context 2: Public F#-to-F# API

**Types:**
- Discriminated unions for domain states/choices
- Record types for DTOs
- `Option<'T>` instead of null
- `Result<'T, 'TError>` for anticipated failures
- Model errors as discriminated unions

**Organization:**
- Namespaces at top level (not modules)
- Functions in modules with `[<RequireQualifiedAccess>]` when names are common
- Signature files (`.fsi`) to control API surface
- XML comments (`///`) on all public members

**Error Handling:**
- `Result<'T, 'TError>` for expected errors (validation, parsing, business rules)
- Exceptions only for unrecoverable conditions
- Never return null; use Option

**Async:**
- Return `Async<'T>`
- Combine with Result: `Async<Result<'T, 'E>>`

### Context 3: Public API for C# / .NET Languages

**Types:**
- Classes with properties (not modules/functions)
- Methods (not curried functions)
- Interfaces for abstractions
- `Task<'T>` instead of `Async<'T>`
- Nullable types instead of `Option<'T>`
- Standard .NET collections (IEnumerable, IList) instead of F# list/seq

**Error Handling:**
- Throw standard .NET exceptions (ArgumentException, InvalidOperationException)
- Document exceptions with XML `<exception>` tags
- Provide Try* method pairs with bool return and out parameters

**Discriminated Unions:**
- DO NOT expose directly to C#
- Convert to abstract base classes with sealed derived classes
- OR convert to enums if simple
- OR wrap in classes with methods

**Pattern:**
```fsharp
// Internal implementation
type Result<'T,'E> = Ok of 'T | Error of 'E

// C# API wrapper
type PublicApi() =
    member _.TryOperation(input: string, [<Out>] result: byref<Data>) : bool =
        match internalOp input with
        | Ok data -> 
            result <- data
            true
        | Error _ -> 
            false
    
    member _.DoOperation(input: string) : Data =
        match internalOp input with
        | Ok data -> data
        | Error err -> raise (InvalidOperationException(err.ToString()))
```

**Async:**
- Return `Task<'T>` or `Task`
- Suffix methods with `Async`
- Use `Async.StartAsTask` or task CE

## Type System

### Records
- Default immutable
- PascalCase field names
- Copy-and-update: `{ record with Field = value }`
- `[<CLIMutable>]` only for C# interop or serialization
- DO NOT use mutable fields unless performance-critical and profiled

### Discriminated Unions
- PascalCase case names
- `[<RequireQualifiedAccess>]` when case names are common (e.g., `Status.Active`)
- Include data in cases directly
- Single-case unions for type safety around primitives

### Option Types
- Use in F# APIs
- Pattern match or use Option module (map, bind, defaultValue)
- DO NOT mix null and Option; choose Option consistently
- Convert to nullable/null at C# boundaries

### Active Patterns
- PascalCase names
- Partial active patterns return Option
- Place in `[<RequireQualifiedAccess>]` modules

### Type Inference
- Rely on inference for local/private code
- Explicit annotations for public API signatures
- Annotate when clarifying intent or improving errors

## Code Organization

### Namespaces and Modules
- Top level: namespaces (not modules)
- Within namespaces: nested modules for grouping
- `[<RequireQualifiedAccess>]` on modules with common names
- DO NOT use `[<AutoOpen>]` except for computation builders
- Maximum 2-3 module nesting levels

### File Structure
Within files, order:
1. Open statements (grouped)
2. Type definitions
3. Module definitions with functions
4. Active patterns

### Dependency Order
- Definitions before usage (F# requirement)
- Helpers before callers
- Types before functions using them
- Use `and` for mutual recursion

## Pattern Matching

**DO:**
- Use as primary control flow
- Match exhaustively on discriminated unions
- Decompose structures inline
- Combine with `when` guards

**DO NOT:**
- Use wildcard `_` unless explicitly ignoring cases
- Use if-else chains instead of pattern matching

## Immutability vs Mutability

**Default Immutable:**
- `let` bindings (not `let mutable`)
- Immutable records and collections
- Copy-and-update instead of mutation
- Collection transformations (map, filter) instead of loops

**Mutable Only When:**
- Performance-critical tight loops (profiled)
- Interop with mutable .NET APIs
- Local optimization hidden from callers

**Encapsulate Mutation:**
```fsharp
let processData data =
    let mutable acc = 0  // hidden, local only
    for item in data do
        acc <- acc + compute item
    acc  // pure function interface
```

## Error Handling by Context

### F#-to-F# APIs
- `Result<'T, ValidationError>` for expected errors
- `Option<'T>` for absence
- Exceptions only for unrecoverable errors

### C#-Facing APIs
- Exceptions for all errors
- Try* methods with bool + out parameters
- Nullable types for optional values

### Async Errors
- Let exceptions propagate through async
- Use `Async<Result<'T, 'E>>` for expected errors
- Catch at workflow boundaries

## Function Design

### Composability
- Small, focused, single-responsibility functions
- Pipeline compatible (data last parameter)
- Use `|>`, `>>`, `<<` operators

### Parameter Order
- General to specific
- Data parameter last for pipeline compatibility

## Naming

- **PascalCase**: Types, modules, namespaces, record fields, union cases, properties, methods
- **camelCase**: Functions, values, parameters, local bindings
- **Acronyms**: Treat as words (`XmlDocument`, not `XMLDocument`)

## Domain Modeling

### Make Illegal States Unrepresentable
```fsharp
type EmailAddress = private EmailAddress of string

module EmailAddress =
    let create str =
        if isValidEmail str then Some (EmailAddress str) else None
    let value (EmailAddress str) = str
```

### Model Workflows as Type Transformations
```fsharp
type UnvalidatedOrder = { CustomerName: string; Items: string list }
type ValidatedOrder = { CustomerName: ValidatedName; Items: Item list }
type PricedOrder = { Order: ValidatedOrder; TotalPrice: decimal }

let placeOrder : UnvalidatedOrder -> Result<PricedOrder, Error> =
    validate >> Result.bind price >> Result.bind save
```

### Separate Data from Behavior
- Types for data structures
- Modules/functions for operations
- DO NOT add methods to records (except C# interop)

## Units of Measure

```fsharp
[<Measure>] type kg
[<Measure>] type m

let force (mass: float<kg>) (accel: float<m/s^2>) : float<kg m/s^2> =
    mass * accel
```

## Performance

### Collection Types
- Arrays: performance-critical indexed access
- Lists: small collections, functional operations
- Sequences: lazy evaluation, large datasets
- ResizeArray: mutable scenarios

### Tail Recursion
```fsharp
let sum list =
    let rec loop acc remaining =
        match remaining with
        | [] -> acc
        | h::t -> loop (acc + h) t
    loop 0 list
```

## Critical Rules: DO NOT vs DO

| DO NOT | DO |
|--------|-----|
| Use tabs for indentation | Use 4 spaces per indentation level |
| Align code by variable name length | Use consistent indentation only |
| Expose mutable state from public APIs | Encapsulate mutation behind pure interfaces |
| Use `[<AutoOpen>]` freely | Use only for computation builders |
| Mix null and Option | Choose Option consistently |
| Use exceptions for expected errors in F# APIs | Use Result for expected errors |
| Create deeply nested modules (>3 levels) | Keep hierarchies shallow (2-3 max) |
| Abbreviate names arbitrarily | Use full descriptive names |
| Return null from F# functions | Return Option or Result |
| Expose F# list to C# APIs | Use IEnumerable, IList, IReadOnlyList |
| Expose discriminated unions to C# | Wrap as classes or abstract base classes |
| Use mutable by default | Use immutable by default |
| Add methods to records in F# code | Use separate functions/modules |
| Return Async to C# consumers | Return Task |

## Summary Checklist

When generating F# code:
- [ ] Use 4 spaces for indentation (NEVER tabs)
- [ ] Respect the offside rule (align all lines in block)
- [ ] Identify consumer context (internal F#, public F#-to-F#, or C#-facing)
- [ ] Apply appropriate type choices for context
- [ ] Default to immutability
- [ ] Use pattern matching for control flow
- [ ] Compose small functions with pipelines
- [ ] Apply `[<RequireQualifiedAccess>]` to prevent collisions
- [ ] Provide XML documentation for public APIs
- [ ] Handle errors with Result (F# APIs) or exceptions (C# APIs)
