# F# RFC FS-1043 - Extension members become available to solve operator trait constraints

These design suggestions:
* https://github.com/fsharp/fslang-suggestions/issues/230
* https://github.com/fsharp/fslang-suggestions/issues/29
* https://github.com/fsharp/fslang-suggestions/issues/820

have been marked "approved in principle". This RFC covers the detailed proposal for these

* [x] Approved in principle
* [x] [Discussion](https://github.com/fsharp/fslang-design/issues/435)
* [x] [Implementation](https://github.com/dotnet/fsharp/pull/8404)


# Summary
[summary]: #summary

Extension methods are previously ignored by SRTP constraint resolution.  This RFC means they are taken into account.

For example, consider
```fsharp

type System.String with
    static member ( * ) (foo, n: int) = String.replicate n foo

let r4 = "r" * 4
let spaces n = " " * n
```
Prior to this RFC the result is:
```
foo.fs(2,21): warning FS1215: Extension members cannot provide operator overloads.  Consider defining the operator as part of the type definition instead.
foo.fs(4,16): error FS0001: The type 'int' does not match the type 'string'
```
With this RFC, the code compiles.

In addition, this RFC adds an attribute `AllowOverloadOnReturnTypeAttribute` to FSharp.Core to implement suggestion [Consider the return type in overload resolution](https://github.com/fsharp/fslang-suggestions/issues/820). If this is present on any applicable overloads in a method overload resolution, then the return type is also checked/unified when determining overload resolution.  Previously, only methods named `op_Explicit` and `op_Implicit` where given this treatment.

In addition, this RFC makes small technical modifications to the process of solving SRTP constraints.  These will be documented in further  sections of this RFC.

# Motivation
[motivation]: #motivation

It is reasonable to use extension methods to retrofit operators and other semantics on to existing types. This "completes the picture" as extension methods in a natural way.


# Detailed design
[design]: #detailed-design


## Adding extension members to SRTP constraint solving

The proposed change is as follows, in the internal logic of the constraint solving process:

1. During constraint solving, the record of each SRTP constraint incorporates the relevant extension methods in-scope at the point the SRTP constraint is asserted. That is, at the point a generic construct is used and "freshened".  The accessibility domain (i.e. the information indicating accessible methods) is also noted as part of the constraint.  Both of these pieces of information are propagated as part of the constraint. We call these the *trait possible extension solutions* and the *trait accessor domain*

2. When checking whether one unsolved SRTP constraint A *implies* another B (note: this a process used to avoid asserting duplicate constraints when propagating a constraint from one type parameter to another - see `implies` in `ConstraintSolver.fs`), both the possible extension solutions and the accessor domain of A are ignored, and those of the existing asserted constraint are preferred.

3. When checking whether one unsolved SRTP constraint is *consistent* with another (note: this is a process used to check for inconsistency errors amongst a set of constraints - see `consistent` in `ConstraintSolver.fs`), the possible extension solutions and accessor domain are ignored.

4. When attempting to solve the constraint via overload resolution, the possible extension solutions which are accessible from the trait accessor domain are taken into account.  

5. Built-in constraint solutions for things like `op_Addition` constraints are applied if and when the relevant types match precisely, and are applied even if some extension methods of that name are available.

## Weak resolution no longer forces overload resolution for SRTP constraints prior to generalizing `inline` code

Prior to this RFC, for generic inline code we apply "weak resolution" to constraints prior to generalization.

Consider this:
```
open System
let inline f1 (x: DateTime) y = x + y;;
let inline f2 (x: DateTime) y = x - y;;
```
The relevant available overloads are:
```fsharp
type System.DateTime with
    static member op_Addition: DateTime * TimeSpan -> DateTime
    static member op_Subtraction: DateTime * TimeSpan -> DateTime
    static member op_Subtraction: DateTime * DateTime -> TimeSpan
```
Prior to this RFC, `f1` is generalized to **non-generic** code, and `f2` is correctly generalized to generic code, as seen by these types:
```
val inline f1 : x:DateTime -> y:TimeSpan -> DateTime
val inline f2 : x:DateTime -> y: ^a ->  ^b  when (DateTime or  ^a) : (static member ( - ) : System.DateTime * ^a ->  ^b)
```
Why?  Well, prior to this RFC, generalization invokes "weak resolution" for both inline and non-inline code.  This caused
overload resolution to be applied even though the second parameter type of "y" is not known.

* In the first case, overload resolution for `op_Addition` succeeded because there is only one overload.

* In the second case, overload resolution for `op_Subtraction` failed because there are two overloads. The failure is ignored, and the code is left generic.

For non-inline code and primitive types this "weak resolution" process is reasonable.  But for inline code it was incorrect, especially in the context of this RFC, because future extension methods may now provide additional witnesses for `+` on DateTime and some other type.  

In this RFC, we disable weak resolution for inline code for cases that involve true overload resolution. This changes
inferred types in some situations, e.g. with this RFC the type is now as follows:
```
> let inline f1 (x: DateTime) y = x + y;;
val inline f1 : x:DateTime -> y: ^a ->  ^b when (DateTime or  ^a) : (static member ( + ) : DateTime * ^a ->  ^b)
```

Some signatures files may need to be updated to account for this change.

<!-- BEGIN PROPOSED ADDITION -->

### Concrete inference example

```fsharp
// Feature disabled:
let inline f x = x + 1       // val f : int -> int

// Feature enabled:
let inline f x = x + 1       // val inline f : x: ^a -> ^b when ( ^a or int) : (static member ( + ) : ^a * int -> ^b)

// Explicit annotations are not affected:
let inline f (x: int) = x + 1  // val f : int -> int  (both modes)
```

Code that binds such a function to a monomorphic type (`let g : int -> int = f`) will need annotation.

<!-- END PROPOSED ADDITION -->


# Drawbacks
[drawbacks]: #drawbacks

* This slightly strengthens the "type-class"-like capabilities of SRTP resolution. This means that people may increasingly use SRTP code as a way to write generic, reusable code rather than passing parameters explicitly.  While this is reasonable for generic arithmetic code, it has many downsides when applied to other things.

# Alternatives
[alternatives]: #alternatives

1. Don't do it


# Examples

## Widening to specific type

**NOTE: this is an example of what is allowed by this RFC, but is not necessarily recommended for standard F# coding. In particular error messages may degrade for existing code, and extensive further prelude definitions would be required to give a consistent programming model.**

By default `1 + 2.0` doesn't check in F#.  By using extension members to provide additional overloads for addition you can make this check.  Note that the set of available extensions determines the "numeric hierarchy" and is used to augment the operators, not the actual numeric types themselves.
```fsharp

type System.Int32 with
    static member inline widen_to_int64 (a: int32) : int64 = int64 a
    static member inline widen_to_single (a: int32) : single = single a
    static member inline widen_to_double (a: int32) : double = double a

type System.Single with
    static member inline widen_to_double (a: int) : double = double a

let inline widen_to_int64 (x: ^T) : int64 = (^T : (static member widen_to_int64 : ^T -> int64) (x))
let inline widen_to_single (x: ^T) : single = (^T : (static member widen_to_single : ^T -> single) (x))
let inline widen_to_double (x: ^T) : double = (^T : (static member widen_to_double : ^T -> double) (x))

type System.Int64 with
    static member inline (+)(a: int64, b: 'T) : int64 = a + widen_to_int64 b
    static member inline (+)(a: 'T, b: int64) : int64 = widen_to_int64 a + b

type System.Single with
    static member inline (+)(a: single, b: 'T) : single = a + widen_to_single b
    static member inline (+)(a: 'T, b: single) : single = widen_to_single a + b

type System.Double with
    static member inline (+)(a: double, b: 'T) : double = a + widen_to_double b
    static member inline (+)(a: 'T, b: double) : double = widen_to_double a + b

let examples() =

    (1 + 2L)  |> ignore<int64>
    (1 + 2.0f)  |> ignore<single>
    (1 + 2.0)  |> ignore<double>

    (1L + 2)  |> ignore<int64>
    (1L + 2.0)  |> ignore<double>
```

## Defining safe conversion corresponding to `op_Implicit`

**NOTE: this is an example of what is allowed by this RFC, but is not necessarily recommended for standard F# coding. In particular compiler performance is poor when resolving heavily overloaded constraints.**

By default there is no function which captures the notion of .NET's safe `op_Implicit` conversion in F# (though note
the conversion is still explicit in F# code, not implicit).

You can define one like this:
```
let inline implicitConv (x: ^T) : ^U = ((^T or ^U) : (static member op_Implicit : ^T -> ^U) (x))
```
With this RFC you can then populate this with instances for existing primitive types:
```fsharp
type System.SByte with
    static member inline op_Implicit (a: sbyte) : int16 = int16 a
    static member inline op_Implicit (a: sbyte) : int32 = int32 a
    static member inline op_Implicit (a: sbyte) : int64 = int64 a
    static member inline op_Implicit (a: sbyte) : nativeint = nativeint a
    static member inline op_Implicit (a: sbyte) : single = single a
    static member inline op_Implicit (a: sbyte) : double = double a

type System.Byte with
    static member inline op_Implicit (a: byte) : int16 = int16 a
    static member inline op_Implicit (a: byte) : uint16 = uint16 a
    static member inline op_Implicit (a: byte) : int32 = int32 a
    static member inline op_Implicit (a: byte) : uint32 = uint32 a
    static member inline op_Implicit (a: byte) : int64 = int64 a
    static member inline op_Implicit (a: byte) : uint64 = uint64 a
    static member inline op_Implicit (a: byte) : nativeint = nativeint a
    static member inline op_Implicit (a: byte) : unativeint = unativeint a
    static member inline op_Implicit (a: byte) : single = single a
    static member inline op_Implicit (a: byte) : double = double a

type System.Int16 with
    static member inline op_Implicit (a: int16) : int32 = int32 a
    static member inline op_Implicit (a: int16) : int64 = int64 a
    static member inline op_Implicit (a: int16) : nativeint = nativeint a
    static member inline op_Implicit (a: int16) : single = single a
    static member inline op_Implicit (a: int16) : double = double a

type System.UInt16 with
    static member inline op_Implicit (a: uint16) : int32 = int32 a
    static member inline op_Implicit (a: uint16) : uint32 = uint32 a
    static member inline op_Implicit (a: uint16) : int64 = int64 a
    static member inline op_Implicit (a: uint16) : uint64 = uint64 a
    static member inline op_Implicit (a: uint16) : nativeint = nativeint a
    static member inline op_Implicit (a: uint16) : unativeint = unativeint a
    static member inline op_Implicit (a: uint16) : single = single a
    static member inline op_Implicit (a: uint16) : double = double a

type System.Int32 with
    static member inline op_Implicit (a: int32) : int64 = int64 a
    static member inline op_Implicit (a: int32) : nativeint = nativeint a
    static member inline op_Implicit (a: int32) : single = single a
    static member inline op_Implicit (a: int32) : double = double a

type System.UInt32 with
    static member inline op_Implicit (a: uint32) : int64 = int64 a
    static member inline op_Implicit (a: uint32) : uint64 = uint64 a
    static member inline op_Implicit (a: uint32) : unativeint = unativeint a
    static member inline op_Implicit (a: uint32) : single = single a
    static member inline op_Implicit (a: uint32) : double = double a

type System.Int64 with
    static member inline op_Implicit (a: int64) : double = double a

type System.UInt64 with
    static member inline op_Implicit (a: uint64) : double = double a

type System.IntPtr with
    static member inline op_Implicit (a: nativeint) : int64 = int64 a
    static member inline op_Implicit (a: nativeint) : double = double a

type System.UIntPtr with
    static member inline op_Implicit (a: unativeint) : uint64 = uint64 a
    static member inline op_Implicit (a: unativeint) : double = double a

type System.Single with
    static member inline op_Implicit (a: int) : double = double a
```


<!-- BEGIN PROPOSED ADDITION -->

# Interop

* C# consumers see no difference — extension SRTP constraints are an F#-only concept resolved at compile time. The emitted IL is standard .NET.
* Inline functions with unsolved SRTP constraints cannot be invoked dynamically via reflection (the non-witness method body throws at runtime). This is pre-existing behavior for SRTP-constrained functions; this RFC extends it to cases that were previously eagerly resolved.

# Pragmatics

## Diagnostics

* **FS1215** ("Extension members cannot provide operator overloads"): suppressed when the feature is enabled. Fires as before when the feature is disabled.
* No new diagnostics are introduced. Overload ambiguity from extension methods uses existing error codes.

## Tooling

* **Tooltips**: will show the more generic inferred type for inline functions (e.g., `^a -> ^b when ...` instead of `int -> int`). This is correct behavior.
* **Auto-complete**: extension operators now appear in SRTP-resolved member lists when the feature is enabled.
* No changes to debugging, breakpoints, colorization, or brace matching.

## Performance

* Extension method lookup during SRTP constraint solving adds overhead proportional to the number of extension methods in scope. The RFC note on `op_Implicit` ("compiler performance is poor when resolving heavily overloaded constraints") applies here as well.
* No impact on generated code performance — the resolved call sites are identical.

## Scaling

* Expected maximum number of extension operator overloads on a single type in hand-written code: ~20
* Expected reasonable upper bound the compiler should handle: ~100
* Resolution cost is linear in the number of candidate extension methods per constraint.

## Witnesses

Extension members now participate as witnesses for SRTP constraints (see [RFC FS-1090](https://github.com/fsharp/fslang-design/blob/main/RFCs/FS-1090-static-abstract-constraint-inference.md)). Two consequences:

* Quotations of inline SRTP calls may now capture extension methods as witnesses:

```fsharp
type [<Struct>] MyNum = { V: int }

[<AutoOpen>]
module MyNumExt =
    type MyNum with
        static member inline (+) (a: MyNum, b: MyNum) = { V = a.V + b.V }

let inline add x y = x + y
let q = <@ add { V = 1 } { V = 2 } @>  // witness is MyNumExt.(+)
```

* The set of functions whose non-inline invocation throws at runtime grows. Previously, weak resolution eagerly picked a concrete implementation; now the constraint stays open. Non-witness fallback method bodies for such functions throw `NotSupportedException`.

<!-- END PROPOSED ADDITION -->


# Compatibility
[compatibility]: #compatibility

<!-- BEGIN PROPOSED CHANGE: replaced "Status: We are trying to determine" with definitive statement -->

Status: This RFC **is** a breaking change, gated behind `--langversion:preview`.

**What breaks**:
- Inferred types of inline SRTP functions become more generic (e.g., `val f : int -> int` → `val inline f : x: ^a -> ^b when ...`). Signature files need updating.
- `let g : int -> int = f` stops compiling when `f` is now generic — needs annotation.
- Extension methods in SRTP resolution may introduce new ambiguity.

**What does NOT break**: Call sites with concrete arguments. Call-site operator resolution for primitive types.

<!-- END PROPOSED CHANGE -->

We assume it must be a breaking change, because additional methods are taken into account in the overload resolution used in SRTP constraint resolution. That must surely cause it to fail where it would have succeeded before. However,

1. All the new methods are extension methods, which are lower priority in overload resolution

Even if it's theoretically a breaking change, we may still decide it's worthwhile because the risk of change is low.  This seems plausible because

1. Taking the extra existing extension methods into account is natural and a lot like an addition to the .NET libraries causing overload resolution to fail. We don't really consider that a breaking change (partly because this is measured differently for C# and F#, as they have different sensitivities to adding overloads).

2. For the built-in operators like `(+)`, there will be relatively few such candidate extension methods in F# code because we give warnings when users try to add extension methods for these

3. Nearly all SRTP constraints (at least the ones for built-in operators) are on static members, and C# code can't introduce extension members that are static - just instance ones. So C# extension members will only cause compat concern for F# code using SRTP constraints on instance members, AND where the C# extension methods make a difference to overload resolution.

Still, we're pretty sure this must be a breaking change. We would appreciate help construct test cases where it is/isn't.

I'm examining some consequences of the part of this RFC "Weak Resolution no longer forces overload resolution...".   In general this seems a great improvement.  However, I have found one case where, for the complicated SRTP code such as found in FSharpPlus, existing code no longer compiles.

### Example

Here is a standalone repro reduced substantially, and where many types are made more explicit:
```fsharp
let inline InvokeMap (mapping: ^F) (source: ^I) : ^R =  
    ((^I or ^R) : (static member Map : ^I * ^F ->  ^R) source, mapping)

 // A simulated collection
type Coll<'T>() =

    // A simulated 'Map' witness
    static member Map (source: Coll<'a>, mapping: 'a->'b) : Coll<'b> = new Coll<'b>()
```
Now consider this generic inline code:
```fsharp
let inline MapTwice (x: Coll<'a>) (v: 'a) : Coll<'a> =
    InvokeMap ((+) v) (InvokeMap ((+) v) x)
```

### Explanation

The characteristics are
1. There is no overloading directly, but this code is generic and there is the *potential* for further overloading by adding further extension methods.

2. The definition of the member constraint allows resolution by **return type**, e.g. `(^I or ^R)` for `Map` .  Because of this, the return type of the inner `InvokeMap` call is **not** known to be `Coll` until weak resolution is applied to the constraints. This is because extra overloads could in theory be added via new witnesses mapping the collection to a different collection type.

3. The resolution of the nested member constraints will eventually imply that the type variable `'a` support the addition operator.
   However after this RFC, the generic function `MapTwice` now gets generalized **before** the member constraints are fully solved
   and the return types known.  The process of generalizing the function makes the type variable `'a` rigid (generalized).  The
   member constraints are then solved via weak resolution in the final phase of inference, and the return type of `InvokeMap`
   is determined to be a `ZipList`, and the `'a` variable now requires an addition operator.  Because the code has already
   been generalized the process of asserting this constraint fails with an obscure error message.

### Workarounds

There are numerous workarounds:

1. sequentialize the constraint problem rather than combining the resolution of the `Apply` and `Map` methods, e.g.
```fsharp
let inline (+) (x: ZipList<'a>, y: ZipList<'a>) : ZipList<'a> =
    let f = InvokeMap (+) x
    InvokeApply f y
```
   This works because using `let f = InvokeMap (+) x` forces weak resolution of the constraints involved in this construct (whereas passing `InvokeMap (+) x` directly as an argument to `InvokeApply f y` leaves the resolution delayed). 

2. Another approach is to annotate, e.g.
```fsharp
let inline (+) (x: ZipList<'a>, y: ZipList<'a>) : ZipList<'a> =
    InvokeApply (InvokeMap ((+): 'a -> 'a -> 'a) x) y
```
   This works because the type annotation means the `op_Addition` constraint is immediately associated with the type variable `'a` that is part of the function signature.

3. Another approach (and likely the best) is to **no longer use return types as support types** in this kind of generic code.  (My understanding is that the use of return types as support types in such cases FSharpPlus was basically "only" to delay weak resolution anyway).  This means using this definition:

```fsharp
let inline CallMapMethod (mapping: ^F, source: ^I, _output: ^R, mthd: ^M) =
    ((^M or ^I) : (static member MapMethod : (^I * ^F) * ^M  -> ^R) (source, mapping), mthd)
```
instead of
```fsharp
let inline CallMapMethod (mapping: ^F, source: ^I, _output: ^R, mthd: ^M) =
    ((^M or ^I or ^R) : (static member MapMethod : (^I * ^F) * ^M  -> ^R) (source, mapping), mthd)
```

   With this change the code compiles.

This is the only example I've found of this in FSharpPlus.  However I guess there may be client code of FSharpPlus that hits this problems.  In general I suppose it may result whenever we have

```
     let inline SomeGenericFunction (...) =
        ...some composition of FSharpPlus operations that use return types to resolve member constraints....
```

We expect this pattern to happening in client code of FSharpPlus code. The recommendation is:

1. We keep the change to avoid weak resolution as part of the RFC 

2. We adjust FSharpPlus to no longer use return types as resolvers unless absolutely necessary

3. We apply workarounds for client code by adding further type annotations

As part of this RFC we should also deliver a guide on writing SRTP code that documents cases like this and
gives guidelines about their use.

### Slightly Larger Example

For completeness here's a longer example of this problem:
```fsharp
module Lib

let inline CallApplyMethod (input1: ^I1, input2: ^I2, mthd : ^M) : ^R =
    ((^M or ^I1 or ^I2 or ^R) : (static member ApplyMethod : ^I1 * ^I2 * ^M -> ^R) input1, input2, mthd)

let inline CallMapMethod (mapping: ^F, source: ^I, _output: ^R, mthd: ^M) =
    ((^M or ^I or ^R) : (static member MapMethod : (^I * ^F) * ^M  -> ^R) (source, mapping), mthd)

type Apply =
    static member inline ApplyMethod (f: ^AF, x: ^AX, _mthd:Apply) : ^AOut = ((^AF or ^AX) : (static member Apply : ^AF * ^AX -> ^AOut) f, x)

type Map =
    static member inline MapMethod ((x: ^FT, f: 'T->'U), _mthd: Map) : ^R = (^FT : (static member Map : ^FT * ('T -> 'U) ->  ^R) x, f)

let inline InvokeApply (f: ^AF) (x: ^AX) : ^AOut = CallApplyMethod(f, x, Unchecked.defaultof<Apply>)

let inline InvokeMap (mapping: 'T->'U) (source: ^FT) : ^FU =  CallMapMethod (mapping, source, Unchecked.defaultof< ^FU >, Unchecked.defaultof<Map>)

[<Sealed>]
type ZipList<'s>() =

    static member Map (_xs: ZipList<'a>, _f: 'a->'b) : ZipList<'b> = failwith ""

    static member Apply (_fs: ZipList<'a->'b>, _xs: ZipList<'a>) : ZipList<'b>  = failwith ""
```
The following code fails to compile with this RFC activated:
```fsharp
let inline AddZipLists (x: ZipList<'a>, y: ZipList<'a>) : ZipList<'a> =
    InvokeApply (InvokeMap (+) x) y
```

# Unresolved questions
[unresolved]: #unresolved-questions

<!-- BEGIN PROPOSED CHANGE: marked resolved with findings -->

* [x] Points 2 & 3 (`consistent` and `implies`) are subtle and I will attempt to expand the test cases where constraints flow together from different accessibility
domains to try to identify a case where this matters. However it's actually very hard and artificial to construct tests where this matters, because SRTP constraints are typically freshened
and solved within quite small scopes where the available methods and accessibility domain is always consistent.

    **Resolution**: Tested with two modules each providing extension operators on the same type; constraints flow correctly when both are opened. No failing cases found.

<!-- END PROPOSED CHANGE -->
