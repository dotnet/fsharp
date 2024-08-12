# Compiling Equality

This spec covers how equality is compiled and executed by the F# compiler and library, based mainly on the types involved in the equality operation after all inlining, type specialization and other optimizations have been applied.

## What do we mean by an equality operation?

This spec is about the semantics and performance of the following coding constructs

* `a = b`
* `a <> b`

It is also about the semantics and performance of uses of the following `FSharp.Core` constructs which, after inlining, generate code that contains an equality check at the specific `EQTYPE`
* `HashIdentity.Structural<'T>`
* `{Array,Seq,List}.contains`
* `{Array,Seq,List}.countBy`
* `{Array,Seq,List}.groupBy`
* `{Array,Seq,List}.distinct`
* `{Array,Seq,List}.distinctBy`
* `{Array,Seq,List}.except`

All of which have implied equality checks. Some of these operations are inlined, see below, which in turn affects the semantics and performance of the overall operation.

## ER vs PER equality

In math, a (binary) relation is a way to describe a relationship between the elements of sets. "Greater than" is a relation for numbers, "Subset of" is a relation for sets.

Here we talk about 3 particular relations:
1) **Reflexivity** - every element is related to itself
  - For integers, `=` is reflexive (`a = a` is always true) and `>` is not (`a > a` is never true)
2) **Symmetry** - if `a` is related to `b`, then `b` is related to `a`
  - For integers, `=` is symmetric (`a = b` -> `b = a`) and `>` is not (if `a > b` then `b > a` is false)
3) **Transitivity** -  if `a` is related to `b`, and `b` is related to `c`, then `a` is also related `c`
  - For integers, `>` is transitive (`a > b` && `b > c` -> `a > c`) and `√` is not (`a = √b` && `b = √c` doesn't mean `a = √c`)

If a relation has 1, 2, and 3, we talk about **Equivalence Relation (ER)**. If a relation only has 2 and 3, we talk about **Partial Equivalence Relation (PER)**.

This matters in comparing floats since they include [NaN](https://en.wikipedia.org/wiki/NaN). Depending on if we consider `NaN = NaN` true or false, we talk about ER or PER comparison respectively. 

## What is the type known to the compiler and library for an equality operation?

The static type known to the F# compiler is crucial to determining the performance of the operation. The runtime type of the equality check is also significant in some situations.

Here we define the relevant static type `EQTYPE` for the different constructs above:

### Basics

* `a = b`:  `EQTYPE` is the statically known type of `a` or `b`
* `a <> b`: `EQTYPE` is the statically known type of `a` or `b`

### Inlined constructs

* `HashIdentity.Structural<'T>`, `EQTYPE` is the **inlined** `'T` (results in specialized equality)
* `Array.contains<'T>`, `EQTYPE` is the **inlined** `'T` (results in specialized equality)
* `List.contains<T>` likewise
* `Seq.contains<T>` likewise

These only result in naked generic equality if themselves used from a non-inlined generic context.

### Non-inlined constructs always resulting in naked generic equality

* `Array.groupBy<'Key, 'T> f array`, `EQTYPE` is non-inlined `'Key`, results in naked generic equality
* `Array.countBy array` likewise for `'T`
* `Array.distinct<'T> array` likewise
* `Array.distinctBy array` likewise
* `Array.except array` likewise
* `List.groupBy` likewise
* `List.countBy` likewise
* `List.distinct` likewise
* `List.distinctBy` likewise
* `List.except` likewise
* `Seq.groupBy` likewise
* `Seq.countBy` likewise
* `Seq.distinct` likewise
* `Seq.distinctBy` likewise
* `Seq.except` likewise

These **always** result in naked generic equality checks.

Example 1:

```fsharp
let x = HashIdentity.Structural<byte>  // EQTYPE known to compiler is `byte`
```

Example 2 (a non-inlined "naked" generic context):

```fsharp
let f2<'T> () =
   ... some long code
   // EQTYPE known to the compiler is `'T`
   // RUNTIME-EQTYPE known to the library is `byte`
   let x = HashIdentity.Structural<'T>
   ... some long code

f2<byte>() // performance of this is determined by EQTYPE<'T> and RUNTIME-EQTYPE<byte>
```

Example 3 (an inlined generic context):

```fsharp
let f3<'T> () =
   ... some long code
   // EQTYPE known to the compiler is `byte`
   // RUNTIME-EQTYPE known to the library is `byte`
   let x = HashIdentity.Structural<'T>
   ... some long code

f3<byte>() // performance of this is determined by EQTYPE<byte> and RUNTIME-EQTYPE<byte>
```

Example 4 (a generic struct type in a non-inline generic context):

```fsharp
let f4<'T> () =
   ... some long code
   // EQTYPE known to the compiler is `SomeStructType<'T>`
   // RUNTIME-EQTYPE known to the library is `SomeStructType<byte>`
   let x = HashIdentity.Structural<SomeStructType<'T>>
   ... some long code

f4<byte>() // performance of this determined by EQTYPE<SomeStructType<'T>> and RUNTIME-EQTYPE<SomeStructType<byte>>
```

## How we compile equality "a = b"

This very much depends on the `EQTYPE` involved in the equality as known by the compiler

Aim here is to flesh these all out with:
* **Semantics**: what semantics the user expects, and what the semantics actually is
* **Perf expectation**: what perf the user expects
* **Compilation today**: How we actually compile today
* **Perf today**: What is the perf we achieve today
* (Optional) sharplab.io link to how things are in whatever version is selected in sharplab
* (Optional) notes

### primitive integer types (`int32`, `int64`, ...)

```fsharp
let f (x: int) (y: int) = (x = y)
```

* Semantics: equality on primitive
* Perf: User expects full performance down to native
* Compilation today: compiles to IL instruction ✅
* Perf today: good ✅
* [sharplab int32](https://sharplab.io/#v2:DYLgZgzgNAJiDUAfYBTALgAjBgFADxAwEsA7NASlwE9DSKMBeXPRjK8gWACgg===)

### primitive floating point types (`float32`, `float64`)

```fsharp
let f (x: float32) (y: float32) = (x = y)
```

* Semantics: IEEE floating point equality (respecting NaN etc.)
* Perf: User expects full performance down to native
* Compilation today: compiles to IL instruction ✅
* Perf today: good ✅
* [sharplab float32](https://sharplab.io/#v2:DYLgZgzgNAJiDUAfYBTALgAjBgFADxC2AHsBDNAZgCYBKXAT0LBPOroF5c8NP6aBYAFBA===)

### primitive `string`, `decimal`

* Semantics: .NET equivalent equality, non-localized for strings
* Perf: User expects full performance down to native
* Compilation today: compiles to `String.Equals` or `Decimal.op_Equality` call ✅
* Perf today: good ✅
* [sharplab decimal](https://sharplab.io/#v2:DYLgZgzgNALiCWwoBMQGoA+wCmMAEYeAFAB4h7LYDG8AtgIbACUxAnuZTQ83gLzEk+eVkwCwAKCA)
* [sharplab string](https://sharplab.io/#v2:DYLgZgzgNALiCWwoBMQGoA+wCmMAEYeAFAB4h4QwBO8AdgOYCUxAnuZTQ8wLzEl68WjALAAoIA==)

### reference tuple type (size <= 5)

* Semantics: User expects structural
* Perf: User expects flattening to constituent checks
* Compilation today: tuple equality is flattened to constituent checks ✅
* Perf today: good ✅
* [sharplab (int * double * 'T), with example reductions/optimizations noted](https://sharplab.io/#v2:DYLgZgzgPgsAUMApgFwARlQCgB4iwSwDs0AqVAEwHsBXAIyVTIHIAVASjdQE9UBeLbH25t48TCVFxB/LpIC0cosCJEA5goB8kgOKJCiAE74AxgFEAjtQCGy5D0Gy48BUpWF1crU7gAJKxAALAGFKAFsABysDRAA6XX0jM0sbfDsAMX80B1R5RUJlQjVNHT1DEwtrWy4ASWIjQggTAB4WAEZGVBYAJg6WAGYNVAdcgHlw5HxQ/AAvQ00sckQAN3wDNHiypMrUmrqiRuMRbwyIZAqbCBZqcKQ+1AAZK3drVUQABSMpiaXECDjSxIhCJRQwCVoAGmwXUhfU4mC4EK40K4sNyrkK7mK3iQaGMYUi0QMQkezysrw+k1S+B+fw2gPxIIM8Dp5WSVQA6qlggzCSdcTzQdh2gjUAAyUXMgGs7Z2TnIbnA3mZVB4xWCnpIsUSuAsrYpWVcoEEwx8lUConYO4o3KDSQ4s1qon8EmqF7vT5Umn/BImI2M+DGRDmIbC9rigNBoYanrhnVSvUcw3m2rIeoHB3Gi1WvqSEhHeBAA==)

### reference tuple type (size > 5)

* Semantics: User expects structural
* Perf: User expects flattening to constituent checks
* Compilation today: not flattened, compiled to `GenericEqualityIntrinsic`
* Perf today: the check does type tests, does virtual calls via `IStructuralEqualityComparer`, boxes etc. ❌(Problem1)
* [sharplab for size 6](https://sharplab.io/#v2:DYLgZgzgPgsAUMApgFwARlQCgB4iwSwDs0AqVI0841MimqyigSidQE9UBeLbL9p+EA==)

### struct tuple type

* Semantics: User expects structural
* Perf: User expects flattening to constituent checks or at least the same optimizations as tuples
* Compilation today: compiled to `GenericEqualityIntrinsic`
* Perf today: boxes, does type tests, does virtual calls via `IStructuralEqualityComparer` etc. ❌(Problem2)
* [sharplab for size 3](https://sharplab.io/#v2:DYLgZgzgPgsAUMApgFwARlQCgB4lRZAJwFcBjNTASwDs0AqVG+x2gSldQE9UBeLbXl1bwgA=)

### C# or F# enum type

* Semantics: User expects identical to equality on the underlying type
* Perf: User expects same perf as flattening to underlying type
* Compilation today: flattens to underlying type
* Perf today: good ✅
* [sharplab for C# enum int](https://sharplab.io/#v2:DYLgZgzgNALiCWwA+BYAUMApjABGHAFAB4g4DKAnhDJgLYB0AIgIYUDyYA6ppgNYCUOCjgC8hIqKH90QA===)
* [sharplab for F# enum int](https://sharplab.io/#v2:DYLgZgzgNALiCWwA+BYAUDAngBwKYAIBRfAXn3X0qXwEFT8BGCq/AIXoCZ11hcZ8w+ABQAPEEQCU+TPVH1ME9EA=)

### C# struct type

* Semantics: User expects call to `IEquatable<T>` if present, but F# spec says call `this.Equals(box that)`, in practice these are the same
* Perf expected: no boxing
* Compilation today: EqualityComparer<T>.Default
* Perf today: good ✅
* [sharplab](https://sharplab.io/#v2:DYLgZgzgNALiCWwA+BYAUMApjABGHAFAB4g4DKAnhDJgLYB0AIgIY0Aq8tmA8mJNgEocFHAF5CRMcIHogA==)

### F# struct type (records, tuples - with compiler-generated structural equality)

* Semantics: User expects field-by-field structural equality
* Perf expected: no boxing
* Compilation today: `GenericEqualityIntrinsic<SomeStructType>`
* Perf today: good ✅
* [sharplab](https://sharplab.io/#v2:DYLgZgzgNALiCWwA+BYAUAbQDwGUYCcBXAYxgD4BddGATwAcBTAAhwHsBbBvI0gCgDcQTeADsYUJoSGiYASiYBedExVNO7AEYN8TAPoA6AGqKm/ZavVadBgKonC6dMAYwmYJrwAeQtp24k5JhoTLxMaWXQgA)

</details>

### array type (byte[], int[], some-struct-type[],  ...)

* Semantics: User expects structural
* Perf expected: User expects perf is sum of constituent parts
* Compilation today: either ``FSharpEqualityComparer_PER`1<uint8[]>::get_EqualityComparer().Equals(...)`` or ``FSharpEqualityComparer_PER`1<T[]>::get_EqualityComparer().Equals(...)``
* Perf today: good ✅
* [sharplab for `byte[]`](https://sharplab.io/#v2:DYLgZgzgPgsAUMApgFwARlQCgB4lQIwE9lEBtAXQEpVDUBeLbemy+IA=)

### F# large reference record/union type

Here "large" means the compiler-generated structural equality is NOT inlined.

* Semantics: User expects structural by default
* Perf expected: User expects perf is sum of constituent parts, type-specialized if generic
* Compilation today: direct call to `Equals(T)`
* Perf today: the call to `Equals(T)` has specialized code but boxes fields if struct or generic, see Problem3 ❌, Problem4 ❌

### F# tiny reference (anonymous) record or union type

Here "tiny" means the compiler-generated structural equality IS inlined.

* Semantics: User expects structural by default
* Perf expected: User expects perf is sum of constituent parts, type-specialized if generic
* Compilation today: ``FSharpEqualityComparer_ER`1<!a>::get_EqualityComparer().Equals(...)``
* Perf today: good ✅

### Generic `'T` in non-inlined generic code

* Semantics: User expects the PER equality semantics of whatever `'T` actually is  
* Perf expected: User expects no boxing
* Compilation today: ``FSharpEqualityComparer_ER`1<!a>::get_EqualityComparer().Equals(...)``
* Perf today: good ✅

### Generic `'T` in recursive position in structural comparison

This case happens in structural equality for tuple types and other structural types

* Semantics: User expects the PER equality semantics of whatever `'T` actually is  
* Perf: User expects no boxing
* Compilation today: ``FSharpEqualityComparer_ER`1<!a>::get_EqualityComparer().Equals(...)`` 
* Perf today: good ✅
* [Sharplab](https://sharplab.io/#v2:DYLgZgzgPgsAUMApgFwARlQCgB4iwSwDs0AqVAEwHsBXAIyVTIHIAVASjdQE9UBeLbH25t48TCVFxB/LpIC0cosCJEA5goB8kgOKJCiAE74AxgFEAjtQCGy5D0Gy48BUpWF1crU7gAJKxAALAGFKAFsABysDRAA6XX0jM0sbfDsAMX80B1R5RUJlQjVNHT1DEwtrWy4ASWIjQggTAB4WAEZGVBYAJg6WAGYNVAdcgHlw5HxQ/AAvQ00sckQAN3wDNHiypMrUmrqiRuMRbwyIZAqbCBZqcKQ+1AAZK3drVUQABSMpiaXECDjSxIhCJRQwCVoAGmwXUhfU4mC4EK40K4sNyrkK7mK3iQaGMYUi0QMQkezysrw+k1S+B+fw2gPxIIM8Dp5WSVQA6qlggzCSdcTzQdh2gjUAAyUXMgGs7Z2TnIbnA3mZVB4xWCnpIsUSuAsrYpWVcoEEwx8lUConYO4o3KDSQ4s1qon8EmqF7vT5Umn/BImI2M+DGRDmIbC9rigNBoYanrhnVSvUcw3m2rIeoHB3Gi1WvqSEhHeBAA==)

## Techniques available to us
 
1. Flatten and inline
2. RCG: Use reflective code generation internally in FSharp.Core
3. KFS: Rely on known semantics of F# structural types and treat those as special
4. TS: Hand-code type-specializations using static optimization conditions in FSharp.Core
5. TT: Type-indexed tables of baked (poss by reflection) equality comparers and functions, where some pre-computation is done 
6. DV: De-virtualization
7. DEQ: Use `EqualityComparer<'T>.Default` where possible

## Notes on previous attempts to improve things

### [#5112](https://github.com/dotnet/fsharp/pull/5112)

* Uses TT, DEQ, KFS, DV
* Focuses on solving Problem4
* 99% not breaking, apart from the case of value types with custom equality implemented differently than the `EqualityComparer.Default` - the change would lead to the usage of the custom implementation which is reasonable

Note: this included [changes to the optimizer to reduce GenericEqualityIntrinsic](https://github.com/dotnet/fsharp/pull/5112/files#diff-be48dbef2f0baca27a783ac4a31ec0aedb2704c7f42ea3a2b8228513f9904cfbR2360-R2363) down to a type-indexed table lookup fetching an `IEqualityComparer` and calling it. These hand-coded reductions appear unnecessary as the reduction doesn't open up any further optimizations.