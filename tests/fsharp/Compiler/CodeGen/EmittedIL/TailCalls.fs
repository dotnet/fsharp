// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests.CodeGen.EmittedIL

open FSharp.Test.Utilities
open NUnit.Framework

[<TestFixture>]
module ``TailCalls`` =
    // Regression test for DevDiv:72571

    [<Test>]
    let ``TailCall 01``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions [|"-g"; "--optimize-"; "--tailcalls+"|]
            """
module TailCall01
let foo(x:int, y) = printfn "%d" x
let run() = let x = 0 in foo(x,5)
            """
            (fun verifier -> verifier.VerifyIL [
            """
  .method public static void  foo<a>(int32 x,
                                     !!a y) cil managed
  {

    .maxstack  4
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_0,
             int32 V_1)
    IL_0000:  ldstr      "%d"
    IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor(string)
    IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_000f:  stloc.0
    IL_0010:  ldarg.0
    IL_0011:  stloc.1
    IL_0012:  ldloc.0
    IL_0013:  ldloc.1
    IL_0014:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0019:  pop
    IL_001a:  ret
  }
            """
            """
  .method public static void  run() cil managed
  {

    .maxstack  4
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldloc.0
    IL_0003:  ldc.i4.5
    IL_0004:  tail.
    IL_0006:  call       void TailCall01::foo<int32>(int32,
                                                     !!0)
    IL_000b:  ret
  }
            """
            ])

    [<Test>]
    let ``TailCall 02``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions [|"-g"; "--optimize-"; "--tailcalls+"|]
            """
module TailCall02
let foo(x:int byref) = x
let run() = let mutable x = 0 in foo(&x)
            """
            (fun verifier -> verifier.VerifyIL [
            """
  .method public static int32  foo(int32& x) cil managed
  {

    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldobj      [runtime]System.Int32
    IL_0006:  ret
  }
            """
            """
  .method public static int32  run() cil managed
  {

    .maxstack  3
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  call       int32 TailCall02::foo(int32&)
    IL_0009:  ret
  }
            """
            ])

    [<Test>]
    let ``TailCall 03``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions [|"-g"; "--optimize-"; "--tailcalls+"|]
            """
module TailCall03
let foo (x:int byref) (y:int byref) z = printfn "%d" (x+y)
let run() = let mutable x = 0 in foo &x &x 5
            """
            (fun verifier -> verifier.VerifyIL [
            """
  .method public static void  foo<a>(int32& x,
                                     int32& y,
                                     !!a z) cil managed
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00
                                                                                                                    00 00 00 00 )

    .maxstack  4
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_0,
             int32 V_1)
    IL_0000:  ldstr      "%d"
    IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor(string)
    IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_000f:  stloc.0
    IL_0010:  ldarg.0
    IL_0011:  ldobj      [runtime]System.Int32
    IL_0016:  ldarg.1
    IL_0017:  ldobj      [runtime]System.Int32
    IL_001c:  add
    IL_001d:  stloc.1
    IL_001e:  ldloc.0
    IL_001f:  ldloc.1
    IL_0020:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0025:  pop
    IL_0026:  ret
  }
            """
            """
  .method public static void  run() cil managed
  {

    .maxstack  5
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  ldloca.s   V_0
    IL_0006:  ldc.i4.5
    IL_0007:  call       void TailCall03::foo<int32>(int32&,
                                                     int32&,
                                                     !!0)
    IL_000c:  nop
    IL_000d:  ret
  }
            """
            ])

    [<Test>]
    let ``TailCall 04``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions [|"-g"; "--optimize-"; "--tailcalls+"|]
            """
module TailCall04
let foo(x:int byref, y) = printfn "%d" x
let run() = let mutable x = 0 in foo(&x,5)
            """
            (fun verifier -> verifier.VerifyIL [
            """
  .method public static void  foo<a>(int32& x,
                                     !!a y) cil managed
  {

    .maxstack  4
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_0,
             int32 V_1)
    IL_0000:  ldstr      "%d"
    IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor(string)
    IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_000f:  stloc.0
    IL_0010:  ldarg.0
    IL_0011:  ldobj      [runtime]System.Int32
    IL_0016:  stloc.1
    IL_0017:  ldloc.0
    IL_0018:  ldloc.1
    IL_0019:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_001e:  pop
    IL_001f:  ret
  }
            """
            """
  .method public static void  run() cil managed
  {

    .maxstack  4
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  ldc.i4.5
    IL_0005:  call       void TailCall04::foo<int32>(int32&,
                                                     !!0)
    IL_000a:  nop
    IL_000b:  ret

            """
            ])

    [<Test>]
    let ``TailCall 05``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions [|"-g"; "--optimize-"; "--tailcalls+"|]
            """
module TailCall05
let foo(x:int byref, y:int byref, z) = printfn "%d" (x+y)
let run() = let mutable x = 0 in foo(&x,&x,5)
            """
            (fun verifier -> verifier.VerifyIL [
            """
  .method public static void  foo<a>(int32& x,
                                     int32& y,
                                     !!a z) cil managed
  {

    .maxstack  4
    .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_0,
             int32 V_1)
    IL_0000:  ldstr      "%d"
    IL_0005:  newobj     instance void class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`5<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit,int32>::.ctor(string)
    IL_000a:  call       !!0 [FSharp.Core]Microsoft.FSharp.Core.ExtraTopLevelOperators::PrintFormatLine<class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>>(class [FSharp.Core]Microsoft.FSharp.Core.PrintfFormat`4<!!0,class [runtime]System.IO.TextWriter,class [FSharp.Core]Microsoft.FSharp.Core.Unit,class [FSharp.Core]Microsoft.FSharp.Core.Unit>)
    IL_000f:  stloc.0
    IL_0010:  ldarg.0
    IL_0011:  ldobj      [runtime]System.Int32
    IL_0016:  ldarg.1
    IL_0017:  ldobj      [runtime]System.Int32
    IL_001c:  add
    IL_001d:  stloc.1
    IL_001e:  ldloc.0
    IL_001f:  ldloc.1
    IL_0020:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
    IL_0025:  pop
    IL_0026:  ret
  }
            """
            """
  .method public static void  run() cil managed
  {

    .maxstack  5
    .locals init (int32 V_0)
    IL_0000:  ldc.i4.0
    IL_0001:  stloc.0
    IL_0002:  ldloca.s   V_0
    IL_0004:  ldloca.s   V_0
    IL_0006:  ldc.i4.5
    IL_0007:  call       void TailCall05::foo<int32>(int32&,
                                                     int32&,
                                                     !!0)
    IL_000c:  nop
    IL_000d:  ret
  }
            """
            ])
