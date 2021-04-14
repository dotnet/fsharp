// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test.Utilities

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

        CompilerAssert.CompileLibraryAndVerifyIL source
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
                                ])

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

        CompilerAssert.CompileLibraryAndVerifyIL source
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
                            ])

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

    interface IEnumerable with

        member _.GetEnumerator() = null

module Test =

    let test () =
        let span = Span<obj>.Empty
        for item in span do
            Console.WriteLine(item)
            """

        // The current behavior doesn't optimize, but it could in the future. Making a test to catch if it ever does.
        CompilerAssert.CompileLibraryAndVerifyIL source
            (fun verifier ->
                verifier.VerifyIL
                            [
                            """.method public static void  test() cil managed
  {

    .maxstack  3
    .locals init (valuetype System.Span`1<object> V_0,
             class [System.Runtime]System.Collections.IEnumerator V_1,
             class [FSharp.Core]Microsoft.FSharp.Core.Unit V_2,
             class [System.Runtime]System.IDisposable V_3)
    IL_0000:  ldc.i4.0
    IL_0001:  newarr     [System.Runtime]System.Object
    IL_0006:  newobj     instance void valuetype System.Span`1<object>::.ctor(!0[])
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  box        valuetype System.Span`1<object>
    IL_0012:  unbox.any  [System.Runtime]System.Collections.IEnumerable
    IL_0017:  callvirt   instance class [System.Runtime]System.Collections.IEnumerator [System.Runtime]System.Collections.IEnumerable::GetEnumerator()
    IL_001c:  stloc.1
    .try
    {
      IL_001d:  ldloc.1
      IL_001e:  callvirt   instance bool [System.Runtime]System.Collections.IEnumerator::MoveNext()
      IL_0023:  brfalse.s  IL_0032

      IL_0025:  ldloc.1
      IL_0026:  callvirt   instance object [System.Runtime]System.Collections.IEnumerator::get_Current()
      IL_002b:  call       void [System.Console]System.Console::WriteLine(object)
      IL_0030:  br.s       IL_001d

      IL_0032:  ldnull
      IL_0033:  stloc.2
      IL_0034:  leave.s    IL_004c

    }
    finally
    {
      IL_0036:  ldloc.1
      IL_0037:  isinst     [System.Runtime]System.IDisposable
      IL_003c:  stloc.3
      IL_003d:  ldloc.3
      IL_003e:  brfalse.s  IL_0049

      IL_0040:  ldloc.3
      IL_0041:  callvirt   instance void [System.Runtime]System.IDisposable::Dispose()
      IL_0046:  ldnull
      IL_0047:  pop
      IL_0048:  endfinally
      IL_0049:  ldnull
      IL_004a:  pop
      IL_004b:  endfinally
    }
    IL_004c:  ldloc.2
    IL_004d:  pop
    IL_004e:  ret
  }"""
                        ])

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

        CompilerAssert.CompileLibraryAndVerifyIL source
            (fun verifier ->
                verifier.VerifyIL
                            [
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
                            ])
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

        CompilerAssert.CompileLibraryAndVerifyIL source
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
                        ])

#endif
