module FSharp.Compiler.ComponentTests.EmittedIL.StructDefensiveCopy

open Xunit
open System.IO
open FSharp.Test
open FSharp.Test.Compiler

let verifyCompilation expectedIl =
    FSharp """
module StructUnion01
open System.Runtime.CompilerServices
open System.Collections.Generic

let doWork(kvp1:inref<KeyValuePair<int,int>>) =
    kvp1.ToString()
    """
    |> ignoreWarnings
    |> compile
    |> shouldSucceed
    |> verifyIL expectedIl

#if NETSTANDARD 
// KeyValuePair defined as a readonly struct (in C#)
[<Fact>]
let ``Defensive copy can be skipped on read-only structs``() =
    verifyCompilation ["""      .method public static string  doWork([in] valuetype [runtime]System.Collections.Generic.KeyValuePair`2<int32,int32>& kvp1) cil managed
  {
    .param [1]
    .custom instance void [runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  constrained. valuetype [runtime]System.Collections.Generic.KeyValuePair`2<int32,int32>
    IL_0007:  callvirt   instance string [runtime]System.Object::ToString()
    IL_000c:  ret
  } 

} """]

#else 
// KeyValuePair just a regular struct. Notice the "ldobj" instruction
[<Fact>]
let ``Non readonly struct needs a defensive copy``() =
    verifyCompilation ["""      .method public static string  doWork([in] valuetype [runtime]System.Collections.Generic.KeyValuePair`2<int32,int32>& kvp1) cil managed
      {
        .param [1]
        .custom instance void [runtime]System.Runtime.CompilerServices.IsReadOnlyAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  3
        .locals init (valuetype [runtime]System.Collections.Generic.KeyValuePair`2<int32,int32> V_0)
        IL_0000:  ldarg.0
        IL_0001:  ldobj      valuetype [runtime]System.Collections.Generic.KeyValuePair`2<int32,int32>
        IL_0006:  stloc.0
        IL_0007:  ldloca.s   V_0
        IL_0009:  constrained. valuetype [runtime]System.Collections.Generic.KeyValuePair`2<int32,int32>
        IL_000f:  callvirt   instance string [runtime]System.Object::ToString()
        IL_0014:  ret
      } """]
#endif
