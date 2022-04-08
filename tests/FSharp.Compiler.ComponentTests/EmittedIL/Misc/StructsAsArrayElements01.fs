// #Regression #NoMono #NoMT #CodeGen #EmittedIL   
// Regression test for FSHARP1.0:4560
// Handling of structs as array elements
// For functional tests, see fsharp\core\array
module StructsAsArrayElements01
[<Struct>]
type T =
   val mutable public i  : int
   member public this.Set i = this.i <- i

let a  = Array.create 10 Unchecked.defaultof<T>
a.[0].Set 27

// This is the incorrect code that used to be generated
//
//    L_0000: call valuetype Mmm/T[] Mmm::get_a()
//    L_0005: ldc.i4.0 
//    L_0006: ldelem.any Mmm/T
//    L_000b: stloc.0 
//    L_000c: ldloca.s t
//    L_000e: ldfld int32 Mmm/T::i@
//   L_0013: ret
//
//Expected: no locals (no stloc.0/ldloca.s t between ldelem and ldfld)
