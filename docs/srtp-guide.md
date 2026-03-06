---
status: draft
target: Microsoft Learn (F# language guide)
notes: >
  This document is a draft intended for eventual inclusion in the official
  F# documentation on Microsoft Learn. It lives here during development of
  RFC FS-1043 so reviewers can evaluate the guidance alongside the implementation.
---

# Guide to Writing SRTP Code in F#

This guide documents best practices for using Statically Resolved Type Parameters (SRTP) in F#, including the new extension constraint solutions feature (RFC FS-1043).

## What Are SRTPs?

Statically Resolved Type Parameters (SRTPs) allow you to write generic code that requires types to have specific members, resolved at compile time:

```fsharp
let inline add (x: ^T) (y: ^T) = x + y
```

The `^T` syntax (hat-type) declares a statically resolved type parameter. The use of `(+)` generates a member constraint that the compiler resolves at each call site based on the concrete type.

## Extension Constraint Solutions (Preview)

With `--langversion:preview`, extension methods now participate in SRTP constraint resolution.

### Defining Extension Operators

```fsharp
open System

type String with
    static member (*) (s: string, n: int) = String.replicate n s

let inline multiply (x: ^T) (n: int) = x * n
let result = multiply "ha" 3  // "hahaha"
```

### Resolution Priority

When solving an SRTP constraint:
1. **Built-in solutions** for primitive operators (e.g. `int + int`) are applied when the types match precisely, regardless of whether extension methods exist
2. **Overload resolution** considers both intrinsic and extension members. Extension members are lower priority in the resolution order (same as in regular F# overload resolution)
3. Among extensions, standard F# name resolution rules apply (later `open` shadows earlier)

### Scope Capture

With `--langversion:preview`, extrinsic extension members (defined in a separate module from the type) participate in SRTP constraint resolution when they are in scope:

```fsharp
module Lib =
    let inline add (x: ^T) (y: ^T) = x + y

type Widget = { V: int }

type Widget with
    static member (+) (a: Widget, b: Widget) = { V = a.V + b.V }

open Lib
// Widget.(+) is in scope HERE at the call site, not at Lib.add's definition site.
let r = add { V = 1 } { V = 2 }  // resolved at call site using Widget.(+)
```

### Known Limitations

- **FSharpPlus compatibility**: Code using return types as support types in SRTP constraints may fail to compile. See workarounds below.

## Weak Resolution Changes

With `--langversion:preview`, inline code no longer eagerly resolves SRTP constraints via weak resolution when true overload resolution is involved:

```fsharp
// Before: f1 inferred as DateTime -> TimeSpan -> DateTime (non-generic, because op_Addition
//         had only one overload and weak resolution eagerly picked it)
// After:  f1 stays generic: DateTime -> ^a -> ^b (because weak resolution no longer forces
//         overload resolution for inline code)
let inline f1 (x: DateTime) y = x + y
```

### Workarounds for Breaking Changes

If existing inline code breaks:

1. **Add explicit type annotations:**
   ```fsharp
   let inline f1 (x: DateTime) (y: TimeSpan) : DateTime = x + y
   ```

2. **Use sequentialization** to force resolution order

3. **Sequentialize nested calls** when using FSharpPlus-style patterns with return types in support types. If nesting `InvokeMap` calls directly produces errors, sequentialize with a let-binding (see the sequentialization example above). Do NOT remove return types from support types unless you understand the impact on overload resolution — return types are the fundamental mechanism for return-type-driven resolution in type-class encodings.

## Feature Flag

Enable with: `--langversion:preview`  
Feature name: `ExtensionConstraintSolutions`

This feature is gated at the preview language version and will be stabilized in a future F# release.

## AllowOverloadOnReturnType Attribute

The `[<AllowOverloadOnReturnType>]` attribute (in `FSharp.Core`) enables return-type-based overload resolution for any method, extending behavior previously reserved for `op_Explicit` and `op_Implicit`:

```fsharp
type Converter =
    [<AllowOverloadOnReturnType>]
    static member Convert(x: string) : int = int x
    [<AllowOverloadOnReturnType>]
    static member Convert(x: string) : float = float x

let resultInt: int = Converter.Convert("42")       // resolves to int overload
let resultFloat: float = Converter.Convert("42")   // resolves to float overload
```

Without the attribute, these overloads would produce an ambiguity error. Note that the call site must provide enough type context (e.g., a type annotation) for the compiler to select the correct overload.

## Design Intent: Aspirational Patterns

> **⚠️ NOT IMPLEMENTED**: The patterns below are taken from the RFC to illustrate the
> long-term design intent. They do **not** compile with the current implementation.
> Cross-type operator extensions (e.g., `float + int`) interact with built-in operator
> resolution in complex ways that are not yet supported. Do not use these patterns in
> production code.

### Numeric Widening via Extension Operators (NOT IMPLEMENTED)

The RFC describes retrofitting widening conversions onto primitive types:

```fsharp
// ⚠️ ASPIRATIONAL — does not compile
type System.Int32 with
    static member inline widen_to_double (a: int32) : double = double a

let inline widen_to_double (x: ^T) : double = (^T : (static member widen_to_double : ^T -> double) (x))

type System.Double with
    static member inline (+)(a: double, b: 'T) : double = a + widen_to_double b
    static member inline (+)(a: 'T, b: double) : double = widen_to_double a + b
```

> **Warning**: Defining `(+)` extensions on `System.Double` would shadow built-in
> arithmetic for all `float` operations in scope. This pattern requires careful design
> to avoid degrading error messages and performance for existing code.

### Defining op_Implicit via Extension Members (NOT IMPLEMENTED)

The RFC describes populating a generic implicit conversion function:

```fsharp
// ⚠️ ASPIRATIONAL — does not compile
let inline implicitConv (x: ^T) : ^U = ((^T or ^U) : (static member op_Implicit : ^T -> ^U) (x))

type System.Int32 with
    static member inline op_Implicit (a: int32) : int64 = int64 a
    static member inline op_Implicit (a: int32) : double = double a
```

> **Note**: Even if implemented, these conversions would be explicit in F# code
> (you must call `implicitConv`), not implicit as in C#.
