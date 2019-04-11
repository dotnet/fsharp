// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework

#if !NETCOREAPP
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

        ILChecker.checkWithDlls 
            (ILChecker.getPackageDlls "System.Memory" "4.5.2" "netstandard2.0" [ "System.Memory.dll" ]) 
            source
            [
            """.method public static void  test() cil managed
{
  
  .maxstack  5
  .locals init (valuetype [System.Memory]System.Span`1<object> V_0,
           int32 V_1,
           int32 V_2,
           object& V_3)
  IL_0000:  call       valuetype [System.Memory]System.Span`1<!0> valuetype [System.Memory]System.Span`1<object>::get_Empty()
  IL_0005:  stloc.0
  IL_0006:  ldc.i4.0
  IL_0007:  stloc.2
  IL_0008:  ldloca.s   V_0
  IL_000a:  call       instance int32 valuetype [System.Memory]System.Span`1<object>::get_Length()
  IL_000f:  ldc.i4.1
  IL_0010:  sub
  IL_0011:  stloc.1
  IL_0012:  ldloc.1
  IL_0013:  ldloc.2
  IL_0014:  blt.s      IL_0034

  IL_0016:  ldloca.s   V_0
  IL_0018:  ldloc.2
  IL_0019:  call       instance !0& valuetype [System.Memory]System.Span`1<object>::get_Item(int32)
  IL_001e:  stloc.3
  IL_001f:  ldloc.3
  IL_0020:  ldobj      [mscorlib]System.Object
  IL_0025:  call       void [mscorlib]System.Console::WriteLine(object)
  IL_002a:  ldloc.2
  IL_002b:  ldc.i4.1
  IL_002c:  add
  IL_002d:  stloc.2
  IL_002e:  ldloc.2
  IL_002f:  ldloc.1
  IL_0030:  ldc.i4.1
  IL_0031:  add
  IL_0032:  bne.un.s   IL_0016

  IL_0034:  ret
} """
            ]

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

        ILChecker.checkWithDlls 
            (ILChecker.getPackageDlls "System.Memory" "4.5.2" "netstandard2.0" [ "System.Memory.dll" ]) 
            source
            [
            """.method public static void  test() cil managed
  {
    
    .maxstack  5
    .locals init (valuetype [System.Memory]System.ReadOnlySpan`1<object> V_0,
             int32 V_1,
             int32 V_2,
             object& V_3)
    IL_0000:  call       valuetype [System.Memory]System.ReadOnlySpan`1<!0> valuetype [System.Memory]System.ReadOnlySpan`1<object>::get_Empty()
    IL_0005:  stloc.0
    IL_0006:  ldc.i4.0
    IL_0007:  stloc.2
    IL_0008:  ldloca.s   V_0
    IL_000a:  call       instance int32 valuetype [System.Memory]System.ReadOnlySpan`1<object>::get_Length()
    IL_000f:  ldc.i4.1
    IL_0010:  sub
    IL_0011:  stloc.1
    IL_0012:  ldloc.1
    IL_0013:  ldloc.2
    IL_0014:  blt.s      IL_0034

    IL_0016:  ldloca.s   V_0
    IL_0018:  ldloc.2
    IL_0019:  call       instance !0& modreq([netstandard]System.Runtime.InteropServices.InAttribute) valuetype [System.Memory]System.ReadOnlySpan`1<object>::get_Item(int32)
    IL_001e:  stloc.3
    IL_001f:  ldloc.3
    IL_0020:  ldobj      [mscorlib]System.Object
    IL_0025:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_002a:  ldloc.2
    IL_002b:  ldc.i4.1
    IL_002c:  add
    IL_002d:  stloc.2
    IL_002e:  ldloc.2
    IL_002f:  ldloc.1
    IL_0030:  ldc.i4.1
    IL_0031:  add
    IL_0032:  bne.un.s   IL_0016

    IL_0034:  ret
  }"""
            ]

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

    member __.Item
        with get (i: int) = &arr.[0]

    member __.Length
        with get () = 0

    static member Empty = Span<'T>([||])

    interface IEnumerable with

        member __.GetEnumerator() = null

module Test =

    let test () =
        let span = Span<obj>.Empty
        for item in span do
            Console.WriteLine(item)
            """

        // The current behavior doesn't optimize, but it could in the future. Making a test to catch if it ever does.
        ILChecker.checkWithDlls 
            (ILChecker.getPackageDlls "System.Memory" "4.5.2" "netstandard2.0" [ "System.Memory.dll" ]) 
            source
            [
            """.method public static void  test() cil managed
  {
    
    .maxstack  3
    .locals init (valuetype System.Span`1<object> V_0,
             class [mscorlib]System.Collections.IEnumerator V_1,
             class [FSharp.Core]Microsoft.FSharp.Core.Unit V_2,
             class [mscorlib]System.IDisposable V_3)
    IL_0000:  ldc.i4.0
    IL_0001:  newarr     [mscorlib]System.Object
    IL_0006:  newobj     instance void valuetype System.Span`1<object>::.ctor(!0[])
    IL_000b:  stloc.0
    IL_000c:  ldloc.0
    IL_000d:  box        valuetype System.Span`1<object>
    IL_0012:  unbox.any  [mscorlib]System.Collections.IEnumerable
    IL_0017:  callvirt   instance class [mscorlib]System.Collections.IEnumerator [mscorlib]System.Collections.IEnumerable::GetEnumerator()
    IL_001c:  stloc.1
    .try
    {
      IL_001d:  ldloc.1
      IL_001e:  callvirt   instance bool [mscorlib]System.Collections.IEnumerator::MoveNext()
      IL_0023:  brfalse.s  IL_0032

      IL_0025:  ldloc.1
      IL_0026:  callvirt   instance object [mscorlib]System.Collections.IEnumerator::get_Current()
      IL_002b:  call       void [mscorlib]System.Console::WriteLine(object)
      IL_0030:  br.s       IL_001d

      IL_0032:  ldnull
      IL_0033:  stloc.2
      IL_0034:  leave.s    IL_004c

    }  
    finally
    {
      IL_0036:  ldloc.1
      IL_0037:  isinst     [mscorlib]System.IDisposable
      IL_003c:  stloc.3
      IL_003d:  ldloc.3
      IL_003e:  brfalse.s  IL_0049

      IL_0040:  ldloc.3
      IL_0041:  callvirt   instance void [mscorlib]System.IDisposable::Dispose()
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
            ]
#endif
