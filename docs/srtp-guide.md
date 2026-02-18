# Guide to Writing SRTP Code in F#

This guide documents best practices for using Statically Resolved Type Parameters (SRTP) in F#, including the new extension constraint solutions feature (RFC FS-1043).

## What Are SRTPs?

Statically Resolved Type Parameters (SRTPs) allow you to write generic code that requires types to have specific members, resolved at compile time:

```fsharp
let inline add (x: ^T) (y: ^T) = x + y
```

The `^T` syntax (hat-type) creates an SRTP constraint. The compiler resolves `(+)` at each call site based on the concrete type.

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

### Priority Rules

When multiple methods could satisfy an SRTP constraint:
1. **Intrinsic members** (defined on the type itself) always take priority over extension members
2. **Extension members** are considered only if no intrinsic member matches
3. Among extensions, standard F# name resolution rules apply (later `open` shadows earlier)

### Scope Capture

Extension methods are captured at the point where the SRTP constraint is **used** (the call site), not where it's **defined**:

```fsharp
module Lib =
    let inline add (x: ^T) (y: ^T) = x + y  // no extensions in scope here

module Consumer =
    type Widget = { V: int }
    type Widget with
        static member (+) (a: Widget, b: Widget) = { V = a.V + b.V }
    
    open Lib
    let r = add { V = 1 } { V = 2 }  // Widget.(+) is in scope at call site
```

### Known Limitations

- **FSharpPlus compatibility**: Code using return types as support types in SRTP constraints may fail to compile. See workarounds below.

## Weak Resolution Changes

With `--langversion:preview`, inline code no longer eagerly resolves SRTP constraints through weak resolution:

```fsharp
// Before: f1 inferred as DateTime -> TimeSpan -> DateTime (non-generic)
// After:  f1 stays generic: DateTime -> ^a -> ^b
let inline f1 (x: DateTime) y = x + y
```

### Workarounds for Breaking Changes

If existing inline code breaks:

1. **Add explicit type annotations:**
   ```fsharp
   let inline f1 (x: DateTime) (y: TimeSpan) : DateTime = x + y
   ```

2. **Use sequentialization** to force resolution order

3. **Remove return types from SRTP support types** if using FSharpPlus patterns

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

## Advanced Examples

### Numeric Widening via Extension Operators

Extension members can retrofit widening conversions onto primitive types, enabling expressions like `1 + 2.0`:

```fsharp
type System.Int32 with
    static member inline widen_to_double (a: int32) : double = double a

let inline widen_to_double (x: ^T) : double = (^T : (static member widen_to_double : ^T -> double) (x))

type System.Double with
    static member inline (+)(a: double, b: 'T) : double = a + widen_to_double b
    static member inline (+)(a: 'T, b: double) : double = widen_to_double a + b

let result = 1 + 2.0  // double
```

> **Note:** This pattern is powerful but can degrade error messages for existing code. Use judiciously.

### Defining op_Implicit via Extension Members

Extension members can populate a generic implicit conversion function for primitive types:

```fsharp
let inline implicitConv (x: ^T) : ^U = ((^T or ^U) : (static member op_Implicit : ^T -> ^U) (x))

type System.Int32 with
    static member inline op_Implicit (a: int32) : int64 = int64 a
    static member inline op_Implicit (a: int32) : double = double a

let r1: int64 = implicitConv 42   // 42L
let r2: double = implicitConv 42  // 42.0
```

> **Note:** These conversions are explicit in F# code (you must call `implicitConv`), not implicit as in C#.
