// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests.CodeGen.EmittedIL

open System.IO
open System.Reflection
open FSharp.Compiler.UnitTests
open NUnit.Framework

[<TestFixture>]
module ``Inline Functions`` =
    
    [<Test>]
    let ``No allocation when doing partial application on a NoInlining function passed to an inlined function`` () =
        let fs =
            """
module Test

open System
open System.Runtime.CompilerServices

[<MethodImpl(MethodImplOptions.NoInlining)>]
let printIt a b c d e f g (x: int) = Console.WriteLine x

let test1 a b c d e f g xs =
    xs |> Array.iter (printIt a b c d e f g)
            """

        CompilerAssert.CompileLibraryAndVerifyILWithOptions [|"--optimize+"|] fs
            (fun verifier ->
                verifier.VerifyIL
                    [
                        """.method public static void  test1<a,b,c,d,e,f,g>(!!a a,
                        !!b b,
                        !!c c,
                        !!d d,
                        !!e e,
                        !!f f,
                        !!g g,
                        int32[] xs) cil managed
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 08 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                         00 00 01 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                         00 00 01 00 00 00 00 00 ) 
        
        .maxstack  11
        .locals init (int32 V_0)
        IL_0000:  ldarg.s    xs
        IL_0002:  brfalse.s  IL_0006
    
        IL_0004:  br.s       IL_0011
    
        IL_0006:  ldstr      "array"
        IL_000b:  newobj     instance void [runtime]System.ArgumentNullException::.ctor(string)
        IL_0010:  throw
    
        IL_0011:  ldc.i4.0
        IL_0012:  stloc.0
        IL_0013:  br.s       IL_0030
    
        IL_0015:  ldarg.0
        IL_0016:  ldarg.1
        IL_0017:  ldarg.2
        IL_0018:  ldarg.3
        IL_0019:  ldarg.s    e
        IL_001b:  ldarg.s    f
        IL_001d:  ldarg.s    g
        IL_001f:  ldarg.s    xs
        IL_0021:  ldloc.0
        IL_0022:  ldelem     [runtime]System.Int32
        IL_0027:  call       void Test::action@1<!!0,!!1,!!2,!!3,!!4,!!5,!!6>(!!0,
                                               !!1,
                                               !!2,
                                               !!3,
                                               !!4,
                                               !!5,
                                               !!6,
                                               int32)
        IL_002c:  ldloc.0
        IL_002d:  ldc.i4.1
        IL_002e:  add
        IL_002f:  stloc.0
        IL_0030:  ldloc.0
        IL_0031:  ldarg.s    xs
        IL_0033:  ldlen
        IL_0034:  conv.i4
        IL_0035:  blt.s      IL_0015
    
        IL_0037:  ret
      }"""
                    ]
            )

    [<Test>]
    let ``No allocation when passing a lambda wrapping a NoInlining function to an inlined function`` () =
        let fs =
            """
module Test

open System
open System.Runtime.CompilerServices

[<MethodImpl(MethodImplOptions.NoInlining)>]
let printIt a b c d e f g (x: int) = Console.WriteLine x

let test1 a b c d e f g xs =
    xs |> Array.iter (fun x -> printIt a b c d e f g x)
            """

        CompilerAssert.CompileLibraryAndVerifyILWithOptions [|"--optimize+"|] fs
            (fun verifier ->
                verifier.VerifyIL
                    [
                        """.method public static void  test1<a,b,c,d,e,f,g>(!!a a,
                        !!b b,
                        !!c c,
                        !!d d,
                        !!e e,
                        !!f f,
                        !!g g,
                        int32[] xs) cil managed
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 08 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                         00 00 01 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                         00 00 01 00 00 00 00 00 ) 
        
        .maxstack  11
        .locals init (int32 V_0)
        IL_0000:  ldarg.s    xs
        IL_0002:  brfalse.s  IL_0006
    
        IL_0004:  br.s       IL_0011
    
        IL_0006:  ldstr      "array"
        IL_000b:  newobj     instance void [runtime]System.ArgumentNullException::.ctor(string)
        IL_0010:  throw
    
        IL_0011:  ldc.i4.0
        IL_0012:  stloc.0
        IL_0013:  br.s       IL_0030
    
        IL_0015:  ldarg.0
        IL_0016:  ldarg.1
        IL_0017:  ldarg.2
        IL_0018:  ldarg.3
        IL_0019:  ldarg.s    e
        IL_001b:  ldarg.s    f
        IL_001d:  ldarg.s    g
        IL_001f:  ldarg.s    xs
        IL_0021:  ldloc.0
        IL_0022:  ldelem     [runtime]System.Int32
        IL_0027:  call       void Test::action@1<!!0,!!1,!!2,!!3,!!4,!!5,!!6>(!!0,
                                               !!1,
                                               !!2,
                                               !!3,
                                               !!4,
                                               !!5,
                                               !!6,
                                               int32)
        IL_002c:  ldloc.0
        IL_002d:  ldc.i4.1
        IL_002e:  add
        IL_002f:  stloc.0
        IL_0030:  ldloc.0
        IL_0031:  ldarg.s    xs
        IL_0033:  ldlen
        IL_0034:  conv.i4
        IL_0035:  blt.s      IL_0015
    
        IL_0037:  ret
      }"""
                    ]
            )

    [<Test>]
    let ``No allocation when doing partial application on a possible inline-able function to an inlined function`` () =
        let fs =
            """
module Test

open System
open System.Runtime.CompilerServices

let printIt a b c d e f g (x: int) = Console.WriteLine x

let test1 a b c d e f g xs =
    xs |> Array.iter (printIt a b c d e f g)
            """

        CompilerAssert.CompileLibraryAndVerifyILWithOptions [|"--optimize+"|] fs
            (fun verifier ->
                verifier.VerifyIL
                    [
                        """.method public static void  test1<a,b,c,d,e,f,g>(!!a a,
                        !!b b,
                        !!c c,
                        !!d d,
                        !!e e,
                        !!f f,
                        !!g g,
                        int32[] xs) cil managed
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 08 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                         00 00 01 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                         00 00 01 00 00 00 00 00 ) 
        
        .maxstack  4
        .locals init (int32 V_0)
        IL_0000:  ldarg.s    xs
        IL_0002:  brfalse.s  IL_0006
    
        IL_0004:  br.s       IL_0011
    
        IL_0006:  ldstr      "array"
        IL_000b:  newobj     instance void [runtime]System.ArgumentNullException::.ctor(string)
        IL_0010:  throw
    
        IL_0011:  ldc.i4.0
        IL_0012:  stloc.0
        IL_0013:  br.s       IL_0026
    
        IL_0015:  ldarg.s    xs
        IL_0017:  ldloc.0
        IL_0018:  ldelem     [runtime]System.Int32
        IL_001d:  call       void [runtime]System.Console::WriteLine(int32)
        IL_0022:  ldloc.0
        IL_0023:  ldc.i4.1
        IL_0024:  add
        IL_0025:  stloc.0
        IL_0026:  ldloc.0
        IL_0027:  ldarg.s    xs
        IL_0029:  ldlen
        IL_002a:  conv.i4
        IL_002b:  blt.s      IL_0015
    
        IL_002d:  ret
      }"""
                    ]
            )

    [<Test>]
    let ``No allocation when doing partial application on a possible inline-able function to an inlined function - 2`` () =
        let fs =
            """
module Test

open System
open System.Runtime.CompilerServices

let printIt a b c d e f g (x: int) = Console.WriteLine x

let test1 a b c d e f g xs =
    let action = printIt a b c d e f g
    xs |> Array.iter action
            """

        CompilerAssert.CompileLibraryAndVerifyILWithOptions [|"--optimize+"|] fs
            (fun verifier ->
                verifier.VerifyIL
                    [
                        """.method public static void  test1<a,b,c,d,e,f,g>(!!a a,
                        !!b b,
                        !!c c,
                        !!d d,
                        !!e e,
                        !!f f,
                        !!g g,
                        int32[] xs) cil managed
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 08 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                         00 00 01 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                         00 00 01 00 00 00 00 00 ) 
        
        .maxstack  4
        .locals init (int32 V_0)
        IL_0000:  ldarg.s    xs
        IL_0002:  brfalse.s  IL_0006
    
        IL_0004:  br.s       IL_0011
    
        IL_0006:  ldstr      "array"
        IL_000b:  newobj     instance void [runtime]System.ArgumentNullException::.ctor(string)
        IL_0010:  throw
    
        IL_0011:  ldc.i4.0
        IL_0012:  stloc.0
        IL_0013:  br.s       IL_0026
    
        IL_0015:  ldarg.s    xs
        IL_0017:  ldloc.0
        IL_0018:  ldelem     [runtime]System.Int32
        IL_001d:  call       void [runtime]System.Console::WriteLine(int32)
        IL_0022:  ldloc.0
        IL_0023:  ldc.i4.1
        IL_0024:  add
        IL_0025:  stloc.0
        IL_0026:  ldloc.0
        IL_0027:  ldarg.s    xs
        IL_0029:  ldlen
        IL_002a:  conv.i4
        IL_002b:  blt.s      IL_0015
    
        IL_002d:  ret
      }"""
                    ]
            )

    [<Test>]
    let ``No allocation when passing a lambda wrapping a possible inline-able function to an inlined function`` () =
        let fs =
            """
module Test

open System
open System.Runtime.CompilerServices

let printIt a b c d e f g (x: int) = Console.WriteLine x

let test1 a b c d e f g xs =
    xs |> Array.iter (fun x -> printIt a b c d e f g x)
            """

        CompilerAssert.CompileLibraryAndVerifyILWithOptions [|"--optimize+"|] fs
            (fun verifier ->
                verifier.VerifyIL
                    [
                        """.method public static void  test1<a,b,c,d,e,f,g>(!!a a,
                        !!b b,
                        !!c c,
                        !!d d,
                        !!e e,
                        !!f f,
                        !!g g,
                        int32[] xs) cil managed
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 08 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                         00 00 01 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                         00 00 01 00 00 00 00 00 ) 
        
        .maxstack  4
        .locals init (int32 V_0)
        IL_0000:  ldarg.s    xs
        IL_0002:  brfalse.s  IL_0006
    
        IL_0004:  br.s       IL_0011
    
        IL_0006:  ldstr      "array"
        IL_000b:  newobj     instance void [runtime]System.ArgumentNullException::.ctor(string)
        IL_0010:  throw
    
        IL_0011:  ldc.i4.0
        IL_0012:  stloc.0
        IL_0013:  br.s       IL_0026
    
        IL_0015:  ldarg.s    xs
        IL_0017:  ldloc.0
        IL_0018:  ldelem     [runtime]System.Int32
        IL_001d:  call       void [runtime]System.Console::WriteLine(int32)
        IL_0022:  ldloc.0
        IL_0023:  ldc.i4.1
        IL_0024:  add
        IL_0025:  stloc.0
        IL_0026:  ldloc.0
        IL_0027:  ldarg.s    xs
        IL_0029:  ldlen
        IL_002a:  conv.i4
        IL_002b:  blt.s      IL_0015
    
        IL_002d:  ret
      }"""
                    ]
            )

    [<Test>]
    let ``Allocation when fully applying a function that returns a partial application on a possible inline-able function to an inlined function`` () =
        let fs =
            """
module Test

open System
open System.Runtime.CompilerServices

let printIt a b c d e f g : int -> unit = 
    printfn "hello"
    Console.WriteLine

let test1 a b c d e f g xs =
    xs |> Array.iter (printIt a b c d e f g)
            """

        CompilerAssert.CompileLibraryAndVerifyILWithOptions [|"--optimize+"|] fs
            (fun verifier ->
                verifier.VerifyIL
                    [
                        """.method public static void  test1<a,b,c,d,e,f,g>(!!a a,
                        !!b b,
                        !!c c,
                        !!d d,
                        !!e e,
                        !!f f,
                        !!g g,
                        int32[] xs) cil managed
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 08 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                         00 00 01 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                         00 00 01 00 00 00 00 00 ) 
        
        .maxstack  9
        .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_0,
                 int32 V_1)
        IL_0000:  ldarg.0
        IL_0001:  ldarg.1
        IL_0002:  ldarg.2
        IL_0003:  ldarg.3
        IL_0004:  ldarg.s    e
        IL_0006:  ldarg.s    f
        IL_0008:  ldarg.s    g
        IL_000a:  call       class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit> Test::printIt<!!0,!!1,!!2,!!3,!!4,!!5,!!6>(!!0,
                                                                                                                                                    !!1,
                                                                                                                                                    !!2,
                                                                                                                                                    !!3,
                                                                                                                                                    !!4,
                                                                                                                                                    !!5,
                                                                                                                                                    !!6)
        IL_000f:  stloc.0
        IL_0010:  ldarg.s    xs
        IL_0012:  brfalse.s  IL_0016
    
        IL_0014:  br.s       IL_0021
    
        IL_0016:  ldstr      "array"
        IL_001b:  newobj     instance void [runtime]System.ArgumentNullException::.ctor(string)
        IL_0020:  throw
    
        IL_0021:  ldc.i4.0
        IL_0022:  stloc.1
        IL_0023:  br.s       IL_0038
    
        IL_0025:  ldloc.0
        IL_0026:  ldarg.s    xs
        IL_0028:  ldloc.1
        IL_0029:  ldelem     [runtime]System.Int32
        IL_002e:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
        IL_0033:  pop
        IL_0034:  ldloc.1
        IL_0035:  ldc.i4.1
        IL_0036:  add
        IL_0037:  stloc.1
        IL_0038:  ldloc.1
        IL_0039:  ldarg.s    xs
        IL_003b:  ldlen
        IL_003c:  conv.i4
        IL_003d:  blt.s      IL_0025
    
        IL_003f:  ret
      }"""
                    ]
            )

    [<Test>]
    let ``Allocation when fully applying a function that returns a partial application on a possible inline-able function to an inlined function - 2`` () =
        let fs =
            """
module Test

open System
open System.Runtime.CompilerServices

let printIt a b c d e f g : int -> unit = 
    Console.WriteLine "hello"
    Console.WriteLine

let test1 a b c d e f g xs =
    let action = printIt a b c d e f g
    Console.WriteLine "world"
    xs |> Array.iter action
            """

        CompilerAssert.CompileLibraryAndVerifyILWithOptions [|"--optimize+"|] fs
            (fun verifier ->
                verifier.VerifyIL
                    [
                        """.method public static void  test1<a,b,c,d,e,f,g>(!!a a,
                        !!b b,
                        !!c c,
                        !!d d,
                        !!e e,
                        !!f f,
                        !!g g,
                        int32[] xs) cil managed
      {
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 08 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                         00 00 01 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                         00 00 01 00 00 00 00 00 ) 
        
        .maxstack  5
        .locals init (class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit> V_0,
                 int32 V_1)
        IL_0000:  ldstr      "hello"
        IL_0005:  call       void [runtime]System.Console::WriteLine(string)
        IL_000a:  newobj     instance void Test/action@12::.ctor()
        IL_000f:  stloc.0
        IL_0010:  ldstr      "world"
        IL_0015:  call       void [runtime]System.Console::WriteLine(string)
        IL_001a:  ldarg.s    xs
        IL_001c:  brfalse.s  IL_0020
    
        IL_001e:  br.s       IL_002b
    
        IL_0020:  ldstr      "array"
        IL_0025:  newobj     instance void [runtime]System.ArgumentNullException::.ctor(string)
        IL_002a:  throw
    
        IL_002b:  ldc.i4.0
        IL_002c:  stloc.1
        IL_002d:  br.s       IL_0042
    
        IL_002f:  ldloc.0
        IL_0030:  ldarg.s    xs
        IL_0032:  ldloc.1
        IL_0033:  ldelem     [runtime]System.Int32
        IL_0038:  callvirt   instance !1 class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,class [FSharp.Core]Microsoft.FSharp.Core.Unit>::Invoke(!0)
        IL_003d:  pop
        IL_003e:  ldloc.1
        IL_003f:  ldc.i4.1
        IL_0040:  add
        IL_0041:  stloc.1
        IL_0042:  ldloc.1
        IL_0043:  ldarg.s    xs
        IL_0045:  ldlen
        IL_0046:  conv.i4
        IL_0047:  blt.s      IL_002f
    
        IL_0049:  ret
      }"""
                    ]
            )