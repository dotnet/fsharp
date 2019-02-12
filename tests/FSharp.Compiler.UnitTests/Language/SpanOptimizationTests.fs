// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework

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
            (ILChecker.getPackageDlls "System.Memory" "4.5.0" "netstandard2.0" [ "System.Memory.dll" ]) 
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
  IL_000a:  call       instance int32 valuetype ['The system type \'System.Span`1\' was required but no referenced system DLL contained this type']System.Span`1<object>::get_Length()
  IL_000f:  ldc.i4.1
  IL_0010:  sub
  IL_0011:  stloc.1
  IL_0012:  ldloc.1
  IL_0013:  ldloc.2
  IL_0014:  blt.s      IL_0034

  IL_0016:  ldloca.s   V_0
  IL_0018:  ldloc.2
  IL_0019:  call       instance !0& valuetype ['The system type \'System.Span`1\' was required but no referenced system DLL contained this type']System.Span`1<object>::get_Item(int32)
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
            (ILChecker.getPackageDlls "System.Memory" "4.5.0" "netstandard2.0" [ "System.Memory.dll" ]) 
            source
            [
            """.method public static void  test() cil managed
  {
    
    .maxstack  5
    .locals init (valuetype [System.Memory]System.ReadOnlySpan`1<object> V_0,
             int32 V_1,
             int32 V_2)
    IL_0000:  call       valuetype [System.Memory]System.ReadOnlySpan`1<!0> valuetype [System.Memory]System.ReadOnlySpan`1<object>::get_Empty()
    IL_0005:  stloc.0
    IL_0006:  ldc.i4.0
    IL_0007:  stloc.2
    IL_0008:  ldloca.s   V_0
    IL_000a:  call       instance int32 valuetype ['The system type \'System.ReadOnlySpan`1\' was required but no referenced system DLL contained this type']System.ReadOnlySpan`1<object>::get_Length()
    IL_000f:  ldc.i4.1
    IL_0010:  sub
    IL_0011:  stloc.1
    IL_0012:  ldloc.1
    IL_0013:  ldloc.2
    IL_0014:  blt.s      IL_002d

    IL_0016:  ldloca.s   V_0
    IL_0018:  ldloc.2
    IL_0019:  call       instance !0 valuetype ['The system type \'System.ReadOnlySpan`1\' was required but no referenced system DLL contained this type']System.ReadOnlySpan`1<object>::get_Item(int32)
    IL_001e:  call       void [mscorlib]System.Console::WriteLine(object)
    IL_0023:  ldloc.2
    IL_0024:  ldc.i4.1
    IL_0025:  add
    IL_0026:  stloc.2
    IL_0027:  ldloc.2
    IL_0028:  ldloc.1
    IL_0029:  ldc.i4.1
    IL_002a:  add
    IL_002b:  bne.un.s   IL_0016

    IL_002d:  ret
  }"""
            ]