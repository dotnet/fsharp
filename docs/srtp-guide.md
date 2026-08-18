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

### Tuple Type Extensions

Type extensions can be written directly on tuple types using tuple syntax. The tuple type is
rewritten to its underlying named type — `System.Tuple<...>` for reference tuples and
`System.ValueTuple<...>` for struct tuples:

```fsharp
// Reference tuple: rewritten to System.Tuple<'T1, 'T2>
type ('T1 * 'T2) with
    static member inline (<*>) ((a, f), (b, x)) = (a, f x)

let r = (1, string) <*> (2, 3)   // (1, "3")

// Struct tuple: rewritten to System.ValueTuple<'T1, 'T2>
type struct ('T1 * 'T2) with
    static member Fst (struct (a, _b)) = a
```

This capability is gated behind the same preview flag as extension constraint solutions (see
[Feature Flag](#feature-flag)); below preview it is rejected with a feature-availability error.

Only **static** members and operators are supported on a tuple type extension. An **instance**
member (`member x.Foo = ...`) can be declared but cannot be invoked through dot-notation on a
tuple value — resolution fails with `error FS0039: The field, constructor or member 'Foo' is not
defined`. Use a static member or operator (as above) instead.

### Resolution Priority

When solving an SRTP constraint:
1. **Built-in solutions** for primitive operators (e.g. `int + int`) are applied when the types match precisely, regardless of whether extension methods exist
2. **Overload resolution** considers both intrinsic and extension members. Extension members are lower priority in the resolution order (same as in regular F# overload resolution)
3. Among extensions, standard F# name resolution rules apply (later `open` shadows earlier)

### Accessibility

An SRTP constraint may only be solved by a **public** member. A `private`, `internal`, or
`protected` member is never a valid witness — even one that is visible at the point where the
inline function is defined. The inline function carries its solution to arbitrary call sites,
so a non-public witness would be inlined into scopes where it is inaccessible (a runtime
`MethodAccessException`). The compiler therefore rejects it at definition time:

```fsharp
module A =
    type System.Int32 with
        static member private Secret (x: int) = x + 1
    // error FS0001: ... 'Secret' ... is not public
    let inline useSecret (x: ^T) = (^T : (static member Secret: ^T -> ^T) x)
```

This applies to every witness kind — named methods, operators, property accessors, and
`op_Implicit`/`op_Explicit` conversions — and holds across assembly boundaries: an `internal`
member is not a valid witness in a referencing assembly even with `InternalsVisibleTo`.

### Scope Capture

With `--langversion:preview`, extrinsic extension members (defined on a type from another assembly) participate in SRTP constraint resolution when they are in scope where the inline function is defined:

```fsharp
module StringOps =
    // System.String has no built-in (*) — this extension is extrinsic.
    type System.String with
        static member (*) (s: string, n: int) = System.String.Concat(Array.replicate n s)

module GenericLib =
    open StringOps

    // multiply captures the SRTP constraint with String.(*) in scope.
    // The extension is recorded in the constraint at this definition site.
    let inline multiply (x: ^T) (n: int) = x * n

module Consumer =
    open GenericLib
    // StringOps is NOT opened here, but the extension was captured when
    // multiply was defined. It travels with the constraint and resolves here.
    let r = multiply "ha" 3  // "hahaha"
```

**Definition-site capture is intra-assembly only.** The capture shown above travels with the
constraint *within a single assembly*. It is deliberately **not** serialized into compiled
metadata (see [Binary compatibility](#binary-compatibility)), so when `multiply` lives in a
*referenced* assembly the captured `StringOps` extension does not travel to the consumer.
Cross-assembly, SRTP constraints are resolved from the **consumer's** scope: the consumer must
have the extension in scope (e.g. `open StringOps` / `open type`) at its own call site.

```fsharp
// GenericLib compiled into library.dll (opens StringOps at its definition site)
// Consumer.fs references library.dll:
open GenericLib
// error FS0001: None of the types support the operator '*'
//   — StringOps is not in scope here, and cross-assembly capture is not serialized.
let r = multiply "ha" 3

// Fix: bring the extension into the consumer's scope.
open StringOps
let ok = multiply "ha" 3  // "hahaha"
```

### Known Limitations

- **FSharpPlus compatibility**: Code using return types as support types in SRTP constraints may fail to compile. See workarounds below.

### Binary Compatibility

Extension solutions captured during constraint solving are **not** written into compiled
metadata. A trait constraint's set of candidate extension members and its accessor domain live
only in-process while a file is being checked; they are discarded before IL/metadata emission,
so the on-disk pickle format is unchanged and old and new compilers interoperate.

The practical consequence is the intra- vs cross-assembly split described under
[Scope Capture](#scope-capture): within one assembly an inline function carries its
definition-site extensions, but a consumer of a *compiled* inline function resolves SRTP
constraints from its own scope and must have the relevant extensions in scope.

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

> **⚠️ MOSTLY ASPIRATIONAL**: The patterns below are taken from the RFC to illustrate the
> long-term design intent. Except where a subsection explicitly shows a working example, they do
> **not** compile with the current implementation. Cross-type operator extensions (e.g.,
> `float + int`) interact with built-in operator resolution in complex ways that are not yet
> supported. Do not rely on the aspirational snippets in production code.

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

### Defining op_Implicit via Extension Members

Public extension `op_Implicit`/`op_Explicit` conversions **do** participate in SRTP resolution
when the target type is determined at the call site:

```fsharp
module A =
    type Wrap = { X: int }
    type Wrap with
        static member op_Implicit (w: Wrap) : int = w.X
    let inline conv (x: ^T) : int = ((^T) : (static member op_Implicit : ^T -> int) x)

let r = A.conv { A.Wrap.X = 5 }  // 5
```

What is **not** supported is the return-type-polymorphic form the RFC describes — a single
`(^T or ^U)` conversion function backed by several `op_Implicit` overloads that differ only by
return type:

```fsharp
// ⚠️ ASPIRATIONAL — does not compile (error FS0001: None of the types support the operator 'op_Implicit')
let inline implicitConv (x: ^T) : ^U = ((^T or ^U) : (static member op_Implicit : ^T -> ^U) (x))

type System.Int32 with
    static member inline op_Implicit (a: int32) : int64 = int64 a
    static member inline op_Implicit (a: int32) : double = double a
```

> **Note**: Even where supported, these conversions are explicit in F# code
> (you must call the conversion function), not implicit as in C#.
