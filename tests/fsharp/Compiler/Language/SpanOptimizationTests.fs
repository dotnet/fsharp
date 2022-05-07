// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test

#if NETCOREAPP
[<TestFixture>]
module SpanOptimizationTests =

    [<Test>]
    let SpanForInDo() =
        let source =
            """
module Test

open System

let test () =
    let span = Span<obj>.Empty
    for item in span do
        Console.WriteLine(item)
            """

        CompilerAssert.CompileLibraryAndVerifyIL(
            source,
            (fun verifier ->
                verifier.VerifyIL
                            [
                            """
.method public static void  test() cil managed
{

    .maxstack  4
    .locals init (valuetype [runtime]System.Span`1<object> V_0,
             int32 V_1,
             object& V_2)
    IL_0000:  call       valuetype [runtime]System.Span`1<!0> valuetype [runtime]System.Span`1<object>::get_Empty()
    IL_0005:  stloc.0
    IL_0006:  ldc.i4.0
    IL_0007:  stloc.1
    IL_0008:  br.s       IL_0022

    IL_000a:  ldloca.s   V_0
    IL_000c:  ldloc.1
    IL_000d:  call       instance !0& valuetype [runtime]System.Span`1<object>::get_Item(int32)
    IL_0012:  stloc.2
    IL_0013:  ldloc.2
    IL_0014:  ldobj      [runtime]System.Object
    IL_0019:  call       void [System.Console]System.Console::WriteLine(object)
    IL_001e:  ldloc.1
    IL_001f:  ldc.i4.1
    IL_0020:  add
    IL_0021:  stloc.1
    IL_0022:  ldloc.1
    IL_0023:  ldloca.s   V_0
    IL_0025:  call       instance int32 valuetype [runtime]System.Span`1<object>::get_Length()
    IL_002a:  blt.s      IL_000a

    IL_002c:  ret
  }"""
                                ]))

    [<Test>]
    let ReadOnlySpanForInDo() =
        let source =
            """
module Test

open System

let test () =
    let span = ReadOnlySpan<obj>.Empty
    for item in span do
        Console.WriteLine(item)
            """

        CompilerAssert.CompileLibraryAndVerifyIL(
            source,
            (fun verifier ->
                verifier.VerifyIL
                            [
                            """.method public static void  test() cil managed
  {

    .maxstack  4
    .locals init (valuetype [runtime]System.ReadOnlySpan`1<object> V_0,
         int32 V_1,
         object& V_2)
    IL_0000:  call       valuetype [runtime]System.ReadOnlySpan`1<!0> valuetype [runtime]System.ReadOnlySpan`1<object>::get_Empty()
    IL_0005:  stloc.0
    IL_0006:  ldc.i4.0
    IL_0007:  stloc.1
    IL_0008:  br.s       IL_0022

    IL_000a:  ldloca.s   V_0
    IL_000c:  ldloc.1
    IL_000d:  call       instance !0& modreq([runtime]System.Runtime.InteropServices.InAttribute) valuetype [runtime]System.ReadOnlySpan`1<object>::get_Item(int32)
    IL_0012:  stloc.2
    IL_0013:  ldloc.2
    IL_0014:  ldobj      [runtime]System.Object
    IL_0019:  call       void [System.Console]System.Console::WriteLine(object)
    IL_001e:  ldloc.1
    IL_001f:  ldc.i4.1
    IL_0020:  add
    IL_0021:  stloc.1
    IL_0022:  ldloc.1
    IL_0023:  ldloca.s   V_0
    IL_0025:  call       instance int32 valuetype [runtime]System.ReadOnlySpan`1<object>::get_Length()
    IL_002a:  blt.s      IL_000a

    IL_002c:  ret
  }"""
                            ]))

    [<Test>]
    let ExplicitSpanTypeForInDo() =

        let source =
            """
namespace System.Runtime.CompilerServices

open System

[<Sealed>]
type IsByRefLikeAttribute() =
    inherit Attribute()

namespace System

open System.Collections
open System.Runtime.CompilerServices

[<Struct;IsByRefLike>]
type Span<'T>(arr: 'T []) =

    member _.Item
        with get (i: int) = &arr.[i]

    member _.Length
        with get () = 0

    static member Empty = Span<'T>([||])

    member _.GetEnumerator() = arr.AsSpan().GetEnumerator()

module Test =

    let test () =
        let span = Span<obj>.Empty
        for item in span do
            Console.WriteLine(item)
            """

        // The current behavior doesn't optimize, but it could in the future. Making a test to catch if it ever does.
        CompilerAssert.CompileLibraryAndVerifyIL(
            source,
            (fun verifier ->
                verifier.VerifyIL [
                            """
      .method public static void  test() cil managed
      {

        .maxstack  3
        .locals init (valuetype System.Span`1<object> V_0,
                 valuetype [runtime]System.Span`1/Enumerator<object> V_1,
                 valuetype [runtime]System.Span`1<object> V_2,
                 object& V_3)
        IL_0000:  call       !!0[] [runtime]System.Array::Empty<object>()
        IL_0005:  newobj     instance void valuetype System.Span`1<object>::.ctor(!0[])
        IL_000a:  stloc.0
        IL_000b:  ldloca.s   V_0
        IL_000d:  ldfld      !0[] valuetype System.Span`1<object>::arr
        IL_0012:  call       valuetype [runtime]System.Span`1<!!0> [runtime]System.MemoryExtensions::AsSpan<object>(!!0[])
        IL_0017:  stloc.2
        IL_0018:  ldloca.s   V_2
        IL_001a:  call       instance valuetype [runtime]System.Span`1/Enumerator<!0> valuetype [runtime]System.Span`1<object>::GetEnumerator()
        IL_001f:  stloc.1
        .try
        {
          IL_0020:  br.s       IL_0035

          IL_0022:  ldloca.s   V_1
          IL_0024:  call       instance !0& valuetype [runtime]System.Span`1/Enumerator<object>::get_Current()
          IL_0029:  stloc.3
          IL_002a:  ldloc.3
          IL_002b:  ldobj      [runtime]System.Object
          IL_0030:  call       void [runtime]System.Console::WriteLine(object)
          IL_0035:  ldloca.s   V_1
          IL_0037:  call       instance bool valuetype [runtime]System.Span`1/Enumerator<object>::MoveNext()
          IL_003c:  brtrue.s   IL_0022

          IL_003e:  leave.s    IL_0041

        }  
        finally
        {
          IL_0040:  endfinally
        }  
        IL_0041:  ret
      }
"""
                        ]))

    [<Test>]
    let SpanForInBoundsDo() =
        let source =
            """
module Test

open System

let test () =
let span = Span<obj>.Empty
for i in 0 .. span.Length-1 do
    Console.WriteLine(span.[i])
        """

        CompilerAssert.CompileLibraryAndVerifyIL(
            source,
            (fun verifier ->
                verifier.VerifyIL [
                            """.method public static void  test() cil managed
  {

    .maxstack  4
    .locals init (valuetype [runtime]System.Span`1<object> V_0,
             int32 V_1,
             object& V_2)
    IL_0000:  call       valuetype [runtime]System.Span`1<!0> valuetype [runtime]System.Span`1<object>::get_Empty()
    IL_0005:  stloc.0
    IL_0006:  ldc.i4.0
    IL_0007:  stloc.1
    IL_0008:  br.s       IL_0022

    IL_000a:  ldloca.s   V_0
    IL_000c:  ldloc.1
    IL_000d:  call       instance !0& valuetype [runtime]System.Span`1<object>::get_Item(int32)
    IL_0012:  stloc.2
    IL_0013:  ldloc.2
    IL_0014:  ldobj      [runtime]System.Object
    IL_0019:  call       void [System.Console]System.Console::WriteLine(object)
    IL_001e:  ldloc.1
    IL_001f:  ldc.i4.1
    IL_0020:  add
    IL_0021:  stloc.1
    IL_0022:  ldloc.1
    IL_0023:  ldloca.s   V_0
    IL_0025:  call       instance int32 valuetype [runtime]System.Span`1<object>::get_Length()
    IL_002a:  blt.s      IL_000a

    IL_002c:  ret
  }"""
                            ]))

    [<Test>]
    let ReadOnlySpanForInBoundsDo() =
        let source =
            """
module Test

open System

let test () =
let span = ReadOnlySpan<obj>.Empty
for i in 0 .. span.Length-1 do
Console.WriteLine(span.[i])
    """

        CompilerAssert.CompileLibraryAndVerifyIL(
            source,
            (fun verifier ->
                verifier.VerifyIL [
                        """.method public static void  test() cil managed
  {

    .maxstack  4
    .locals init (valuetype [runtime]System.ReadOnlySpan`1<object> V_0,
         int32 V_1,
         object& V_2)
    IL_0000:  call       valuetype [runtime]System.ReadOnlySpan`1<!0> valuetype [runtime]System.ReadOnlySpan`1<object>::get_Empty()
    IL_0005:  stloc.0
    IL_0006:  ldc.i4.0
    IL_0007:  stloc.1
    IL_0008:  br.s       IL_0022

    IL_000a:  ldloca.s   V_0
    IL_000c:  ldloc.1
    IL_000d:  call       instance !0& modreq([runtime]System.Runtime.InteropServices.InAttribute) valuetype [runtime]System.ReadOnlySpan`1<object>::get_Item(int32)
    IL_0012:  stloc.2
    IL_0013:  ldloc.2
    IL_0014:  ldobj      [runtime]System.Object
    IL_0019:  call       void [System.Console]System.Console::WriteLine(object)
    IL_001e:  ldloc.1
    IL_001f:  ldc.i4.1
    IL_0020:  add
    IL_0021:  stloc.1
    IL_0022:  ldloc.1
    IL_0023:  ldloca.s   V_0
    IL_0025:  call       instance int32 valuetype [runtime]System.ReadOnlySpan`1<object>::get_Length()
    IL_002a:  blt.s      IL_000a

    IL_002c:  ret
  }"""
                        ]))

#endif
