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
