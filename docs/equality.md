# Compiling Equality and Comparison

## How we compile equality "a = b"

This very much depends on the type involved in the equality as known by the compiler

Aim here is to flesh these all out with
* **Semantics**: what semantics the user expects, and what the semantics actually is
* **Perf expectation**: what perf the user expects
* **Compilation today**: How we actually compile today, with sharplab.io link
* **Perf today**: What is the perf we achieve today
* **Test**: An IL baseline test case that pins down how we compile things today and allows us to measure change 

### primitive type (int32, int64, ...)

```fsharp
let f (x: int) (y: int) = (x = y)
```

* Semantics: equality on primitive (PER for floating point)
* Perf: User expects full performance down to native
* Compilation today: compiles to IL instruction ✅
* Perf today: good ✅

### tuple type

* Semantics: User expects structural
* Perf: User expects flattening to constituent checks
* Compilation today: tuple equality is flattened to constituent checks up to size 5 ✅
* Perf today: ok up to size 5 ✅
    
### struct tuple type

* Semantics: User expects structural
* Perf: User expects flattening to constituent checks
* Compilation today: CHECK ME ❔
* Perf today:  CHECK ME ❔

### array type (byte[], int[], some-struct-type[],  ...)

* Semantics: User expects structural
* Perf: User expects perf is sum of constituent parts
* Compilation today: GenericEqualityIntrinsic<uint8[]>
* Perf today:  this is hand-optimized for some primitive element types ✅ but boxes each element if "other" is struct or T, see Problem1, Problem2 ❌ 
* Compilation after 5112, either `FSharpEqualityComparer_PER`1<uint8[]>::get_EqualityComparer().Equals(...)` or `FSharpEqualityComparer_PER`1<T[]>::get_EqualityComparer().Equals(...)`
* Perf after 5112: CHECK ME ❔

### C# or F# enum type

* Semantics: User expects identical to equality on underlying
* Perf: User expects flattening to underlying type
* Compilation today: CHECK ME ❔

### C# struct type

* Semantics: User really expects call to `IEquatable<T>` if present, but F# spec says call `this.Equals(box that)`, in practice these are the same
* Compilation today: `GenericEqualityIntrinsic<SomeStructType>`
* Perf today: always boxes (❌, Problem1)

### F# struct type (with compiler-generated structural equality)

* Semantics: User expects field-by-field structural equality with no boxing
* Compilation today: `GenericEqualityIntrinsic<SomeStructType>`
* Perf today: always boxes (❌, Problem1)

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

### T in inlined generic code

* Semantics: User expects perf same as type specialized code (✅)

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