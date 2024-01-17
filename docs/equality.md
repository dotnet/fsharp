# Compiling Equality and Comparison

## How we compile equality "a = b"

This very much depends on the type involved in the equality as known by the compiler

Aim here is to flesh these all out with
* **Semantics**: what semantics the user expects, and what the semantics actually is
* **Perf expectation**: what perf the user expects
* **Compilation today**: How we actually compile today, with sharplab.io link
* **Perf today**: What is the perf we achieve today
* **Test**: An IL baseline test case that pins down how we compile things today and allows us to measure change 
* **sharplab**: sharplab.io link to how things are in whatever version is selected in sharplab

### primitive integer types (int32, int64, ...)

```fsharp
let f (x: int) (y: int) = (x = y)
```

* Semantics: equality on primitive (PER for floating point)
* Perf: User expects full performance down to native
* Compilation today: compiles to IL instruction ✅
* Perf today: good ✅
* [sharplab int32](https://sharplab.io/#v2:DYLgZgzgNAJiDUAfYBTALgAjBgFADxAwEsA7NASlwE9DSKMBeXPRjK8gWACgg===)

### primitive floating point types (float32, float64)

```fsharp
let f (x: float32) (y: float32) = (x = y)
```

* Semantics: IEEE floating point equality (respecting NaN etc.)
* Perf: User expects full performance down to native
* Compilation today: compiles to IL instruction ✅
* Perf today: good ✅
* [sharplab float32](https://sharplab.io/#v2:DYLgZgzgNAJiDUAfYBTALgAjBgFADxC2AHsBDNAZgCYBKXAT0LBPOroF5c8NP6aBYAFBA===)

### primitive string, decimal

* Semantics: .NET equivalent equality
* Perf: User expects full performance down to native
* Compilation today: compiles to `String.Equals` or `Decimal.op_Equality` call ✅
* Perf today: good ✅
* [sharplab decimal](https://sharplab.io/#v2:DYLgZgzgNALiCWwoBMQGoA+wCmMAEYeAFAB4h7LYDG8AtgIbACUxAnuZTQ83gLzEk+eVkwCwAKCA)
* [sharplab string](https://sharplab.io/#v2:DYLgZgzgNALiCWwoBMQGoA+wCmMAEYeAFAB4h4QwBO8AdgOYCUxAnuZTQ8wLzEl68WjALAAoIA==)

### tuple type (size <= 5)

* Semantics: User expects structural
* Perf: User expects flattening to constituent checks
* Compilation today: tuple equality is flattened to constituent checks up to size 5 ✅
* Perf today: ok up to size 5 ✅
* [sharplab (int * double * 'T), with example reductions/optimizations noted](https://sharplab.io/#v2:DYLgZgzgPgsAUMApgFwARlQCgB4iwSwDs0AqVAEwHsBXAIyVTIHIAVASjdQE9UBeLbH25t48TCVFxB/LpIC0cosCJEA5goB8kgOKJCiAE74AxgFEAjtQCGy5D0Gy48BUpWF1crU7gAJKxAALAGFKAFsABysDRAA6XX0jM0sbfDsAMX80B1R5RUJlQjVNHT1DEwtrWy4ASWIjQggTAB4WAEZGVBYAJg6WAGYNVAdcgHlw5HxQ/AAvQ00sckQAN3wDNHiypMrUmrqiRuMRbwyIZAqbCBZqcKQ+1AAZK3drVUQABSMpiaXECDjSxIhCJRQwCVoAGmwXUhfU4mC4EK40K4sNyrkK7mK3iQaGMYUi0QMQkezysrw+k1S+B+fw2gPxIIM8Dp5WSVQA6qlggzCSdcTzQdh2gjUAAyUXMgGs7Z2TnIbnA3mZVB4xWCnpIsUSuAsrYpWVcoEEwx8lUConYO4o3KDSQ4s1qon8EmqF7vT5Umn/BImI2M+DGRDmIbC9rigNBoYanrhnVSvUcw3m2rIeoHB3Gi1WvqSEhHeBAA==)

### tuple type (size > 5)

* Semantics: User expects structural
* Perf: User expects flattening to constituent checks
* Compilation today: compiled to GenericEqualityIntrinsic
* Perf today: boxes, does type tests, does virtual calls via IStructuralEqualityComparer etc. ❌(Problem3)
* [sharplab for size 6, with example reductions/optimizations noted](https://sharplab.io/#v2:DYLgZgzgPgsAUMApgFwARlQCgB4iwSwDs0AqVI0841MimqyigSidQE9UBeLbL9p+EA==)

### struct tuple type

* Semantics: User expects structural
* Perf: User expects flattening to constituent checks, or at least the same optimizations as tuples
* Compilation today: compiled to GenericEqualityIntrinsic
* Perf today: boxes, does type tests, does virtual calls via IStructuralEqualityComparer etc. ❌(Problem4)
* [sharplab for size 3](https://sharplab.io/#v2:DYLgZgzgPgsAUMApgFwARlQCgB4lRZAJwFcBjNTASwDs0AqVG+x2gSldQE9UBeLbXl1bwgA=)

### array type (byte[], int[], some-struct-type[],  ...)

* Semantics: User expects structural
* Perf: User expects perf is sum of constituent parts
* Compilation today: GenericEqualityIntrinsic<uint8[]>
* Perf today:  this is hand-optimized for some primitive element types ✅ but boxes each element if "other" is struct or T, see Problem1, Problem2 ❌ 
* [sharplab for `byte[]`](https://sharplab.io/#v2:DYLgZgzgPgsAUMApgFwARlQCgB4lQIwE9lEBtAXQEpVDUBeLbemy+IA=)

Effect of 5112:
* Compilation after 5112, either `FSharpEqualityComparer_PER`1<uint8[]>::get_EqualityComparer().Equals(...)` or `FSharpEqualityComparer_PER`1<T[]>::get_EqualityComparer().Equals(...)`
* Perf after 5112: CHECK ME ❔

### C# or F# enum type

* Semantics: User expects identical to equality on underlying
* Perf: User expects same perf as flattening to underlying type
* Compilation today: flattens to underlying type
* Perf today: good ✅
* [sharplab for C# enum int](https://sharplab.io/#v2:DYLgZgzgNALiCWwA+BYAUMApjABGHAFAB4g4DKAnhDJgLYB0AIgIYUDyYA6ppgNYCUOCjgC8hIqKH90QA===)
* [sharplab for F# enum int](https://sharplab.io/#v2:DYLgZgzgNALiCWwA+BYAUDAngBwKYAIBRfAXn3X0qXwEFT8BGCq/AIXoCZ11hcZ8w+ABQAPEEQCU+TPVH1ME9EA=)

### C# struct type

* Semantics: User really expects call to `IEquatable<T>` if present, but F# spec says call `this.Equals(box that)`, in practice these are the same
* Compilation today: `GenericEqualityIntrinsic<SomeStructType>`
* Perf today: always boxes (❌, Problem1)
* [sharplab](https://sharplab.io/#v2:DYLgZgzgNALiCWwA+BYAUMApjABGHAFAB4g4DKAnhDJgLYB0AIgIY0Aq8tmA8mJNgEocFHAF5CRMcIHogA==)

### F# struct type (with compiler-generated structural equality)

* Semantics: User expects field-by-field structural equality with no boxing
* Compilation today: `GenericEqualityIntrinsic<SomeStructType>`
* Perf today: always boxes (❌, Problem1b)
* [sharplab](https://sharplab.io/#v2:DYLgZgzgNALiCWwA+BYAUAbQDwGUYCcBXAYxgD4BddGATwAcBTAAhwHsBbBvI0gCgDcQTeADsYUJoSGiYASiYBedExVNO7AEYN8TAPoA6AGqKm/ZavVadBgKonC6dMAYwmYJrwAeQtp24k5JhoTLxMaWXQgA)

Note: the optimization path is a bit strange here, the reductions are:

```fsharp

(x = y) 

--inline--> 

GenericEquality x y 

--inline--> 

GenericEqualityFast x y 

--inline--> 

GenericEqualityIntrinsic x y

--devirtualize-->

x.Equals(box y, LanguagePrimitives.GenericEqualityComparer);
```

The struct type has these generated methods:
```csharp
    override bool Equals(object y) 
    override bool Equals(SomeStruct obj)
    override bool Equals(object obj, IEqualityComparer comp) //withcEqualsVal
```

These call each other in sequence, boing then bunboxing then boxing. We do NOT generate this method, we probably should:
```csharp
    override bool Equals(SomeStruct obj, IEqualityComparer comp) //withcEqualsValUnboxed
```

If we did, the devirtualizing optimization should reduce to this directly, which would result in no boxing.]

### F# large ref record/union type

Here "large" means the compiler-generated structural equality is not inlined

* Semantics: User expects structural by default
* Perf: User expects perf is sum of constituent parts, type-specialized if generic
* Today: direct call to `Equals(T)`, which has specialized code but boxes fields if struct or T, see Problem1, Problem2 ❌

### F# tiny ref record/union type

Here "tiny" means the compiler-generated structural equality IS inlined

* Semantics: User expects structural by default
* Perf: User expects perf is sum of constituent parts, type-specialized if generic
* Test: Equals06.fsx
* Compilation Today: flattened, calling `GenericEqualityERIntrinsic` on struct and generic fields
* Perf today: boxes on struct and generic fields, see Problem1, Problem2
* 5112: `FSharpEqualityComparer_ER`1<!a>::get_EqualityComparer().Equals(...)` on struct and generic fields
       
### Any ref type supporting IEquatable<T>

* Semantics: User expects calling `IEquatable<T>` implementation is used, actual is call to `this.Equals(that)`. These are generally identical semantics

### ref type only supporting .Equals(object) override

* Semantics: User expects call to this override, perhaps a non-virtual call

### other ref type

* Semantics: User expects fast reference equality
* Compilation today: 

### Generic `'T` in non-inlined generic code

* Semantics: User expects the PER equality semantics of whatever T actually is  
* Perf: User expects no boxing (❌, Problem2, fails if T is any non-reference type)
* Test: Equals06.fsx acts as a proxy because equals on small single-case union is inlined
* Compilation today: GenericEqualityERIntrinsic (❌,boxes)
* Compilation after 5112: FSharpEqualityComparer_ER`1<!a>::get_EqualityComparer().Equals(...)

### Generic `'T` in inlined generic code

* Semantics: User expects perf same as type specialized code (✅)

### Generic `'T` in recursive position in structural comparison

This case happens in structural equality for tuple types and other structural types

For example see [this sharplab](https://sharplab.io/#v2:DYLgZgzgPgsAUMApgFwARlQCgB4iwSwDs0AqVAEwHsBXAIyVTIHIAVASjdQE9UBeLbH25t48TCVFxB/LpIC0cosCJEA5goB8kgOKJCiAE74AxgFEAjtQCGy5D0Gy48BUpWF1crU7gAJKxAALAGFKAFsABysDRAA6XX0jM0sbfDsAMX80B1R5RUJlQjVNHT1DEwtrWy4ASWIjQggTAB4WAEZGVBYAJg6WAGYNVAdcgHlw5HxQ/AAvQ00sckQAN3wDNHiypMrUmrqiRuMRbwyIZAqbCBZqcKQ+1AAZK3drVUQABSMpiaXECDjSxIhCJRQwCVoAGmwXUhfU4mC4EK40K4sNyrkK7mK3iQaGMYUi0QMQkezysrw+k1S+B+fw2gPxIIM8Dp5WSVQA6qlggzCSdcTzQdh2gjUAAyUXMgGs7Z2TnIbnA3mZVB4xWCnpIsUSuAsrYpWVcoEEwx8lUConYO4o3KDSQ4s1qon8EmqF7vT5Umn/BImI2M+DGRDmIbC9rigNBoYanrhnVSvUcw3m2rIeoHB3Gi1WvqSEhHeBAA==)

* Semantics: User expects the PER equality semantics of whatever T actually is  
* Perf: User expects no boxing (❌, Problem2, fails if T is any non-reference type)
* Compilation today: `GenericEqualityWithComparerIntrinsic LanguagePrimitives.GenericComparer` 
* Perf today: boxes
* Compilation after 5112: FSharpEqualityComparer_ER`1<!a>::get_EqualityComparer().Equals(...)  // TODO: check ??
* Perf after 5112: TBD, but much better, no boxing in many cases


## Techniques available to us
 
1. Flatten and inline
2. RCG: Use reflective code generation internally in FSharp.Core
3. KFS: Rely on known semantics of F# structural types and treat those as special
4. TS: Hand-code type-specializations using static optimization conditions in FSharp.Core
5. TT: Type-indexed tables of baked (poss by reflection) equality comparers and functions, where some pre-computation is done 
6. DV: De-virtualization
7. DEQ: Use EqualityComparer<'T>.Default where possible
 
## Previous attempts

### 5112 https://github.com/dotnet/fsharp/pull/5112

* Uses TT, DEQ, KFS, DV
* Focuses on solving Problem2
* Not breaking

Note: this included [changes to the optimizer to reduce GenericEqualityIntrinsic](https://github.com/dotnet/fsharp/pull/5112/files#diff-be48dbef2f0baca27a783ac4a31ec0aedb2704c7f42ea3a2b8228513f9904cfbR2360-R2363) down to a type-indexed table lookup fetching an IEqualityComparer and calling it. These hand-coded reductions appear unnecessary as the reduction doesn't open up any further optimizations. We can simply change the definition in the library like this:

```fsharp

let GenericEqualityIntrinsic (x : 'T) (y : 'T) : bool = 
    FSharpEqualityComparer_PER<'T>.EqualityComparer.Equals(x,y)

let GenericEqualityERIntrinsic (x : 'T) (y : 'T) : bool =
    FSharpEqualityComparer_ER<'T>.EqualityComparer.Equals(x,y)

let GenericHashIntrinsic input =
    FSharpEqualityComparer_PER<'T>.EqualityComparer.Hash(input)
```


```

