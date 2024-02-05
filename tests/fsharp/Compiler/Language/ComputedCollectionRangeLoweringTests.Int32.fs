// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests.ComputedCollectionRangeLoweringTests

open NUnit.Framework
open FSharp.Test

[<TestFixture>]
module Int32 =
    /// [|…|]
    module Array =
        /// [|start..finish|]
        module Range =
            [<Test>]
            let ``Lone RangeInt32 with const args when start > finish lowers to empty array`` () =
                CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize+"|],
                    """
                    module Test

                    let test () = [|10..1|]
                    """,
                    (fun verifier -> verifier.VerifyIL [
                    """
                    .assembly extern runtime { }
                    .assembly extern FSharp.Core { }
                    .assembly assembly
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                                          int32,
                                                                                                                          int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
                      .hash algorithm 0x00008004
                      .ver 0:0:0:0
                    }
                    .mresource public FSharpSignatureData.assembly
                    {
                      
                      
                    }
                    .mresource public FSharpOptimizationData.assembly
                    {
                      
                      
                    }
                    .module assembly.dll
                    
                    .imagebase {value}
                    .file alignment 0x00000200
                    .stackreserve 0x00100000
                    .subsystem 0x0003       
                    .corflags 0x00000001    
                    
                    
                    
                    
                    
                    .class public abstract auto ansi sealed Test
                           extends [runtime]System.Object
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
                      .method public static int32[]  test() cil managed
                      {
                        
                        .maxstack  8
                        IL_0000:  call       !!0[] [runtime]System.Array::Empty<int32>()
                        IL_0005:  ret
                      } 
                    
                    } 
                    
                    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
                           extends [runtime]System.Object
                    {
                    }
                    """
                    ]))

            [<Test>]
            let ``Lone RangeInt32 with const args lowers to init loop`` () =
                CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize+"|],
                    """
                    module Test

                    let test () = [|1..257|]
                    """,
                    (fun verifier -> verifier.VerifyIL [
                    """
                    .assembly extern runtime { }
                    .assembly extern FSharp.Core { }
                    .assembly assembly
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                                          int32,
                                                                                                                          int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
                      .hash algorithm 0x00008004
                      .ver 0:0:0:0
                    }
                    .mresource public FSharpSignatureData.assembly
                    {
                      
                      
                    }
                    .mresource public FSharpOptimizationData.assembly
                    {
                      
                      
                    }
                    .module assembly.dll
                    
                    .imagebase {value}
                    .file alignment 0x00000200
                    .stackreserve 0x00100000
                    .subsystem 0x0003       
                    .corflags 0x00000001    
                    
                    
                    
                    
                    
                    .class public abstract auto ansi sealed Test
                           extends [runtime]System.Object
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
                      .method public static int32[]  test() cil managed
                      {
                        
                        .maxstack  5
                        .locals init (int32[] V_0,
                                 int32 V_1,
                                 int32 V_2)
                        IL_0000:  ldc.i4     0x101
                        IL_0005:  newarr     [runtime]System.Int32
                        IL_000a:  stloc.0
                        IL_000b:  ldc.i4.0
                        IL_000c:  stloc.1
                        IL_000d:  ldc.i4.1
                        IL_000e:  stloc.2
                        IL_000f:  br.s       IL_0021
                    
                        IL_0011:  ldloc.0
                        IL_0012:  ldloc.1
                        IL_0013:  ldloc.2
                        IL_0014:  stelem     [runtime]System.Int32
                        IL_0019:  ldloc.2
                        IL_001a:  ldc.i4.1
                        IL_001b:  add
                        IL_001c:  stloc.2
                        IL_001d:  ldloc.1
                        IL_001e:  ldc.i4.1
                        IL_001f:  add
                        IL_0020:  stloc.1
                        IL_0021:  ldloc.1
                        IL_0022:  ldloc.0
                        IL_0023:  ldlen
                        IL_0024:  conv.i4
                        IL_0025:  blt.s      IL_0011
                    
                        IL_0027:  ldloc.0
                        IL_0028:  ret
                      } 
                    
                    } 
                    
                    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
                           extends [runtime]System.Object
                    {
                    }
                    """
                    ]))

            [<Test>]
            let ``Lone RangeInt32 with dynamic args lowers to init loop`` () =
                CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize+"|],
                    """
                    module Test

                    let test start finish = [|start..finish|]
                    """,
                    (fun verifier -> verifier.VerifyIL [
                    """
                    .assembly extern runtime { }
                    .assembly extern FSharp.Core { }
                    .assembly assembly
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                                          int32,
                                                                                                                          int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
                      .hash algorithm 0x00008004
                      .ver 0:0:0:0
                    }
                    .mresource public FSharpSignatureData.assembly
                    {
                      
                      
                    }
                    .mresource public FSharpOptimizationData.assembly
                    {
                      
                      
                    }
                    .module assembly.dll
                    
                    .imagebase {value}
                    .file alignment 0x00000200
                    .stackreserve 0x00100000
                    .subsystem 0x0003       
                    .corflags 0x00000001    
                    
                    
                    
                    
                    
                    .class public abstract auto ansi sealed Test
                           extends [runtime]System.Object
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
                      .method public static int32[]  test(int32 start,
                                                          int32 finish) cil managed
                      {
                        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
                        
                        .maxstack  5
                        .locals init (int32 V_0,
                                 int32[] V_1,
                                 int32 V_2,
                                 int32 V_3)
                        IL_0000:  ldarg.1
                        IL_0001:  ldarg.0
                        IL_0002:  sub
                        IL_0003:  ldc.i4.1
                        IL_0004:  add
                        IL_0005:  stloc.0
                        IL_0006:  ldloc.0
                        IL_0007:  ldc.i4.1
                        IL_0008:  bge.s      IL_0010
                    
                        IL_000a:  call       !!0[] [runtime]System.Array::Empty<int32>()
                        IL_000f:  ret
                    
                        IL_0010:  ldloc.0
                        IL_0011:  newarr     [runtime]System.Int32
                        IL_0016:  stloc.1
                        IL_0017:  ldc.i4.0
                        IL_0018:  stloc.2
                        IL_0019:  ldarg.0
                        IL_001a:  stloc.3
                        IL_001b:  br.s       IL_002d
                    
                        IL_001d:  ldloc.1
                        IL_001e:  ldloc.2
                        IL_001f:  ldloc.3
                        IL_0020:  stelem     [runtime]System.Int32
                        IL_0025:  ldloc.3
                        IL_0026:  ldc.i4.1
                        IL_0027:  add
                        IL_0028:  stloc.3
                        IL_0029:  ldloc.2
                        IL_002a:  ldc.i4.1
                        IL_002b:  add
                        IL_002c:  stloc.2
                        IL_002d:  ldloc.2
                        IL_002e:  ldloc.1
                        IL_002f:  ldlen
                        IL_0030:  conv.i4
                        IL_0031:  blt.s      IL_001d
                    
                        IL_0033:  ldloc.1
                        IL_0034:  ret
                      } 
                    
                    } 
                    
                    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
                           extends [runtime]System.Object
                    {
                    }
                    """
                    ]))

            [<Test>]
            let ``Lone RangeInt32 with dynamic args that are complex exprs stores those in vars before init loop`` () =
                CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize+"|],
                    """
                    module Test

                    let a () = (Array.zeroCreate 10).Length
                    let b () = (Array.zeroCreate 20).Length

                    let test () = [|a ()..b ()|]
                    """,
                    (fun verifier -> verifier.VerifyIL [
                    """
                    .assembly extern runtime { }
                    .assembly extern FSharp.Core { }
                    .assembly assembly
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                                          int32,
                                                                                                                          int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
                      .hash algorithm 0x00008004
                      .ver 0:0:0:0
                    }
                    .mresource public FSharpSignatureData.assembly
                    {
                      
                      
                    }
                    .mresource public FSharpOptimizationData.assembly
                    {
                      
                      
                    }
                    .module assembly.dll
                    
                    .imagebase {value}
                    .file alignment 0x00000200
                    .stackreserve 0x00100000
                    .subsystem 0x0003       
                    .corflags 0x00000001    
                    
                    
                    
                    
                    
                    .class public abstract auto ansi sealed Test
                           extends [runtime]System.Object
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
                      .method public static int32  a() cil managed
                      {
                        
                        .maxstack  8
                        IL_0000:  ldc.i4.s   10
                        IL_0002:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::ZeroCreate<object>(int32)
                        IL_0007:  ldlen
                        IL_0008:  conv.i4
                        IL_0009:  ret
                      } 
                    
                      .method public static int32  b() cil managed
                      {
                        
                        .maxstack  8
                        IL_0000:  ldc.i4.s   20
                        IL_0002:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::ZeroCreate<object>(int32)
                        IL_0007:  ldlen
                        IL_0008:  conv.i4
                        IL_0009:  ret
                      } 
                    
                      .method public static int32[]  test() cil managed
                      {
                        
                        .maxstack  5
                        .locals init (int32 V_0,
                                 int32 V_1,
                                 int32 V_2,
                                 int32[] V_3,
                                 int32 V_4,
                                 int32 V_5)
                        IL_0000:  ldc.i4.s   10
                        IL_0002:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::ZeroCreate<object>(int32)
                        IL_0007:  ldlen
                        IL_0008:  conv.i4
                        IL_0009:  stloc.0
                        IL_000a:  ldc.i4.s   20
                        IL_000c:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::ZeroCreate<object>(int32)
                        IL_0011:  ldlen
                        IL_0012:  conv.i4
                        IL_0013:  stloc.1
                        IL_0014:  ldloc.1
                        IL_0015:  ldloc.0
                        IL_0016:  sub
                        IL_0017:  ldc.i4.1
                        IL_0018:  add
                        IL_0019:  stloc.2
                        IL_001a:  ldloc.2
                        IL_001b:  ldc.i4.1
                        IL_001c:  bge.s      IL_0024
                    
                        IL_001e:  call       !!0[] [runtime]System.Array::Empty<int32>()
                        IL_0023:  ret
                    
                        IL_0024:  ldloc.2
                        IL_0025:  newarr     [runtime]System.Int32
                        IL_002a:  stloc.3
                        IL_002b:  ldc.i4.0
                        IL_002c:  stloc.s    V_4
                        IL_002e:  ldloc.0
                        IL_002f:  stloc.s    V_5
                        IL_0031:  br.s       IL_0049
                    
                        IL_0033:  ldloc.3
                        IL_0034:  ldloc.s    V_4
                        IL_0036:  ldloc.s    V_5
                        IL_0038:  stelem     [runtime]System.Int32
                        IL_003d:  ldloc.s    V_5
                        IL_003f:  ldc.i4.1
                        IL_0040:  add
                        IL_0041:  stloc.s    V_5
                        IL_0043:  ldloc.s    V_4
                        IL_0045:  ldc.i4.1
                        IL_0046:  add
                        IL_0047:  stloc.s    V_4
                        IL_0049:  ldloc.s    V_4
                        IL_004b:  ldloc.3
                        IL_004c:  ldlen
                        IL_004d:  conv.i4
                        IL_004e:  blt.s      IL_0033
                    
                        IL_0050:  ldloc.3
                        IL_0051:  ret
                      } 
                    
                    } 
                    
                    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
                           extends [runtime]System.Object
                    {
                    }
                    """
                    ]))

        /// [|start..step..finish|]
        module RangeStep =
            [<Test>]
            let ``Lone RangeInt32 with const args when (finish - start) / step + 1 ≤ 0 lowers to empty array`` () =
                CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize+"|],
                    """
                    module Test

                    let test () = [|1..-1..5|]
                    """,
                    (fun verifier -> verifier.VerifyIL [
                    """
                    .assembly extern runtime { }
                    .assembly extern FSharp.Core { }
                    .assembly assembly
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                                          int32,
                                                                                                                          int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
                      .hash algorithm 0x00008004
                      .ver 0:0:0:0
                    }
                    .mresource public FSharpSignatureData.assembly
                    {
                      
                      
                    }
                    .mresource public FSharpOptimizationData.assembly
                    {
                      
                      
                    }
                    .module assembly.dll
                    
                    .imagebase {value}
                    .file alignment 0x00000200
                    .stackreserve 0x00100000
                    .subsystem 0x0003       
                    .corflags 0x00000001    
                    
                    
                    
                    
                    
                    .class public abstract auto ansi sealed Test
                           extends [runtime]System.Object
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
                      .method public static int32[]  test() cil managed
                      {
                        
                        .maxstack  8
                        IL_0000:  call       !!0[] [runtime]System.Array::Empty<int32>()
                        IL_0005:  ret
                      } 
                    
                    } 
                    
                    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
                           extends [runtime]System.Object
                    {
                    }
                    """
                    ]))

            [<Test>]
            let ``Lone RangeInt32 with const args lowers to init loop`` () =
                CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize+"|],
                    """
                    module Test

                    let test () = [|1..2..257|]
                    """,
                    (fun verifier -> verifier.VerifyIL [
                    """
                    .assembly extern runtime { }
                    .assembly extern FSharp.Core { }
                    .assembly assembly
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                                          int32,
                                                                                                                          int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
                      .hash algorithm 0x00008004
                      .ver 0:0:0:0
                    }
                    .mresource public FSharpSignatureData.assembly
                    {
                      
                      
                    }
                    .mresource public FSharpOptimizationData.assembly
                    {
                      
                      
                    }
                    .module assembly.dll
                    
                    .imagebase {value}
                    .file alignment 0x00000200
                    .stackreserve 0x00100000
                    .subsystem 0x0003       
                    .corflags 0x00000001    
                    
                    
                    
                    
                    
                    .class public abstract auto ansi sealed Test
                           extends [runtime]System.Object
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
                      .method public static int32[]  test() cil managed
                      {
                        
                        .maxstack  5
                        .locals init (int32[] V_0,
                                 int32 V_1,
                                 int32 V_2)
                        IL_0000:  ldc.i4     0x81
                        IL_0005:  newarr     [runtime]System.Int32
                        IL_000a:  stloc.0
                        IL_000b:  ldc.i4.0
                        IL_000c:  stloc.1
                        IL_000d:  ldc.i4.1
                        IL_000e:  stloc.2
                        IL_000f:  br.s       IL_0021
                    
                        IL_0011:  ldloc.0
                        IL_0012:  ldloc.1
                        IL_0013:  ldloc.2
                        IL_0014:  stelem     [runtime]System.Int32
                        IL_0019:  ldloc.2
                        IL_001a:  ldc.i4.2
                        IL_001b:  add
                        IL_001c:  stloc.2
                        IL_001d:  ldloc.1
                        IL_001e:  ldc.i4.1
                        IL_001f:  add
                        IL_0020:  stloc.1
                        IL_0021:  ldloc.1
                        IL_0022:  ldloc.0
                        IL_0023:  ldlen
                        IL_0024:  conv.i4
                        IL_0025:  blt.s      IL_0011
                    
                        IL_0027:  ldloc.0
                        IL_0028:  ret
                      } 
                    
                    } 
                    
                    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
                           extends [runtime]System.Object
                    {
                    }
                    """
                    ]))

            [<Test>]
            let ``Lone RangeInt32 with dynamic args lowers to init loop`` () =
                CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize+"|],
                    """
                    module Test

                    let test start step finish = [|start..step..finish|]
                    """,
                    (fun verifier -> verifier.VerifyIL [
                    """
                    .assembly extern runtime { }
                    .assembly extern FSharp.Core { }
                    .assembly assembly
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                                          int32,
                                                                                                                          int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
                      .hash algorithm 0x00008004
                      .ver 0:0:0:0
                    }
                    .mresource public FSharpSignatureData.assembly
                    {
                      
                      
                    }
                    .mresource public FSharpOptimizationData.assembly
                    {
                      
                      
                    }
                    .module assembly.dll
                    
                    .imagebase {value}
                    .file alignment 0x00000200
                    .stackreserve 0x00100000
                    .subsystem 0x0003       
                    .corflags 0x00000001    
                    
                    
                    
                    
                    
                    .class public abstract auto ansi sealed Test
                           extends [runtime]System.Object
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
                      .method public static int32[]  test(int32 start,
                                                          int32 step,
                                                          int32 finish) cil managed
                      {
                        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                                        00 00 00 00 ) 
                        
                        .maxstack  5
                        .locals init (int32 V_0,
                                 int32[] V_1,
                                 int32 V_2,
                                 int32 V_3)
                        IL_0000:  ldarg.1
                        IL_0001:  brtrue.s   IL_000e
                    
                        IL_0003:  ldarg.0
                        IL_0004:  ldarg.1
                        IL_0005:  ldarg.2
                        IL_0006:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                                               int32,
                                                                                                                                                                                               int32)
                        IL_000b:  pop
                        IL_000c:  br.s       IL_000e
                    
                        IL_000e:  ldarg.2
                        IL_000f:  ldarg.0
                        IL_0010:  sub
                        IL_0011:  ldarg.1
                        IL_0012:  div
                        IL_0013:  ldc.i4.1
                        IL_0014:  add
                        IL_0015:  stloc.0
                        IL_0016:  ldloc.0
                        IL_0017:  ldc.i4.1
                        IL_0018:  bge.s      IL_0020
                    
                        IL_001a:  call       !!0[] [runtime]System.Array::Empty<int32>()
                        IL_001f:  ret
                    
                        IL_0020:  ldloc.0
                        IL_0021:  newarr     [runtime]System.Int32
                        IL_0026:  stloc.1
                        IL_0027:  ldc.i4.0
                        IL_0028:  stloc.2
                        IL_0029:  ldarg.0
                        IL_002a:  stloc.3
                        IL_002b:  br.s       IL_003d
                    
                        IL_002d:  ldloc.1
                        IL_002e:  ldloc.2
                        IL_002f:  ldloc.3
                        IL_0030:  stelem     [runtime]System.Int32
                        IL_0035:  ldloc.3
                        IL_0036:  ldarg.1
                        IL_0037:  add
                        IL_0038:  stloc.3
                        IL_0039:  ldloc.2
                        IL_003a:  ldc.i4.1
                        IL_003b:  add
                        IL_003c:  stloc.2
                        IL_003d:  ldloc.2
                        IL_003e:  ldloc.1
                        IL_003f:  ldlen
                        IL_0040:  conv.i4
                        IL_0041:  blt.s      IL_002d
                    
                        IL_0043:  ldloc.1
                        IL_0044:  ret
                      } 
                    
                    } 
                    
                    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
                           extends [runtime]System.Object
                    {
                    }
                    """
                    ]))

            [<Test>]
            let ``Lone RangeInt32 with dynamic args that are complex exprs stores those in vars before init loop`` () =
                CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize+"|],
                    """
                    module Test

                    let a () = (Array.zeroCreate 10).Length
                    let b () = (Array.zeroCreate 20).Length
                    let c () = (Array.zeroCreate 300).Length

                    let test () = [|a () .. b () .. c ()|]
                    """,
                    (fun verifier -> verifier.VerifyIL [
                    """
                    .assembly extern runtime { }
                    .assembly extern FSharp.Core { }
                    .assembly assembly
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                                          int32,
                                                                                                                          int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
                      .hash algorithm 0x00008004
                      .ver 0:0:0:0
                    }
                    .mresource public FSharpSignatureData.assembly
                    {
                      
                      
                    }
                    .mresource public FSharpOptimizationData.assembly
                    {
                      
                      
                    }
                    .module assembly.dll
                    
                    .imagebase {value}
                    .file alignment 0x00000200
                    .stackreserve 0x00100000
                    .subsystem 0x0003       
                    .corflags 0x00000001    
                    
                    
                    
                    
                    
                    .class public abstract auto ansi sealed Test
                           extends [runtime]System.Object
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
                      .method public static int32  a() cil managed
                      {
                        
                        .maxstack  8
                        IL_0000:  ldc.i4.s   10
                        IL_0002:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::ZeroCreate<object>(int32)
                        IL_0007:  ldlen
                        IL_0008:  conv.i4
                        IL_0009:  ret
                      } 
                    
                      .method public static int32  b() cil managed
                      {
                        
                        .maxstack  8
                        IL_0000:  ldc.i4.s   20
                        IL_0002:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::ZeroCreate<object>(int32)
                        IL_0007:  ldlen
                        IL_0008:  conv.i4
                        IL_0009:  ret
                      } 
                    
                      .method public static int32  c() cil managed
                      {
                        
                        .maxstack  8
                        IL_0000:  ldc.i4     0x12c
                        IL_0005:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::ZeroCreate<object>(int32)
                        IL_000a:  ldlen
                        IL_000b:  conv.i4
                        IL_000c:  ret
                      } 
                    
                      .method public static int32[]  test() cil managed
                      {
                        
                        .maxstack  5
                        .locals init (int32 V_0,
                                 int32 V_1,
                                 int32 V_2,
                                 int32 V_3,
                                 int32[] V_4,
                                 int32 V_5,
                                 int32 V_6)
                        IL_0000:  ldc.i4.s   10
                        IL_0002:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::ZeroCreate<object>(int32)
                        IL_0007:  ldlen
                        IL_0008:  conv.i4
                        IL_0009:  stloc.0
                        IL_000a:  ldc.i4.s   20
                        IL_000c:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::ZeroCreate<object>(int32)
                        IL_0011:  ldlen
                        IL_0012:  conv.i4
                        IL_0013:  stloc.1
                        IL_0014:  ldc.i4     0x12c
                        IL_0019:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::ZeroCreate<object>(int32)
                        IL_001e:  ldlen
                        IL_001f:  conv.i4
                        IL_0020:  stloc.2
                        IL_0021:  ldloc.1
                        IL_0022:  brtrue.s   IL_002f
                    
                        IL_0024:  ldloc.0
                        IL_0025:  ldloc.1
                        IL_0026:  ldloc.2
                        IL_0027:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                                               int32,
                                                                                                                                                                                               int32)
                        IL_002c:  pop
                        IL_002d:  br.s       IL_002f
                    
                        IL_002f:  ldloc.2
                        IL_0030:  ldloc.0
                        IL_0031:  sub
                        IL_0032:  ldloc.1
                        IL_0033:  div
                        IL_0034:  ldc.i4.1
                        IL_0035:  add
                        IL_0036:  stloc.3
                        IL_0037:  ldloc.3
                        IL_0038:  ldc.i4.1
                        IL_0039:  bge.s      IL_0041
                    
                        IL_003b:  call       !!0[] [runtime]System.Array::Empty<int32>()
                        IL_0040:  ret
                    
                        IL_0041:  ldloc.3
                        IL_0042:  newarr     [runtime]System.Int32
                        IL_0047:  stloc.s    V_4
                        IL_0049:  ldc.i4.0
                        IL_004a:  stloc.s    V_5
                        IL_004c:  ldloc.0
                        IL_004d:  stloc.s    V_6
                        IL_004f:  br.s       IL_0068
                    
                        IL_0051:  ldloc.s    V_4
                        IL_0053:  ldloc.s    V_5
                        IL_0055:  ldloc.s    V_6
                        IL_0057:  stelem     [runtime]System.Int32
                        IL_005c:  ldloc.s    V_6
                        IL_005e:  ldloc.1
                        IL_005f:  add
                        IL_0060:  stloc.s    V_6
                        IL_0062:  ldloc.s    V_5
                        IL_0064:  ldc.i4.1
                        IL_0065:  add
                        IL_0066:  stloc.s    V_5
                        IL_0068:  ldloc.s    V_5
                        IL_006a:  ldloc.s    V_4
                        IL_006c:  ldlen
                        IL_006d:  conv.i4
                        IL_006e:  blt.s      IL_0051
                    
                        IL_0070:  ldloc.s    V_4
                        IL_0072:  ret
                      } 
                    
                    } 
                    
                    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
                           extends [runtime]System.Object
                    {
                    }
                    """
                    ]))

    /// […]
    module List =
        /// [start..finish]
        module Range =
            [<Test>]
            let ``Lone RangeInt32 with const args when start > finish lowers to empty list`` () =
                CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize+"|],
                    """
                    module Test

                    let test () = [10..1]
                    """,
                    (fun verifier -> verifier.VerifyIL [
                    """
                    .assembly extern runtime { }
                    .assembly extern FSharp.Core { }
                    .assembly assembly
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                                          int32,
                                                                                                                          int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
                      .hash algorithm 0x00008004
                      .ver 0:0:0:0
                    }
                    .mresource public FSharpSignatureData.assembly
                    {
                      
                      
                    }
                    .mresource public FSharpOptimizationData.assembly
                    {
                      
                      
                    }
                    .module assembly.dll
                    
                    .imagebase {value}
                    .file alignment 0x00000200
                    .stackreserve 0x00100000
                    .subsystem 0x0003       
                    .corflags 0x00000001    
                    
                    
                    
                    
                    
                    .class public abstract auto ansi sealed Test
                           extends [runtime]System.Object
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
                      .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
                              test() cil managed
                      {
                        
                        .maxstack  8
                        IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
                        IL_0005:  ret
                      } 
                    
                    } 
                    
                    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
                           extends [runtime]System.Object
                    {
                    }
                    """
                    ]))

            [<Test>]
            let ``Lone RangeInt32 with const args lowers to init loop`` () =
                CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize+"|],
                    """
                    module Test

                    let test () = [1..101]
                    """,
                    (fun verifier -> verifier.VerifyIL [
                    """
                    .assembly extern runtime { }
                    .assembly extern FSharp.Core { }
                    .assembly assembly
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                                          int32,
                                                                                                                          int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
                      .hash algorithm 0x00008004
                      .ver 0:0:0:0
                    }
                    .mresource public FSharpSignatureData.assembly
                    {
                      
                      
                    }
                    .mresource public FSharpOptimizationData.assembly
                    {
                      
                      
                    }
                    .module assembly.dll
                    
                    .imagebase {value}
                    .file alignment 0x00000200
                    .stackreserve 0x00100000
                    .subsystem 0x0003       
                    .corflags 0x00000001    
                    
                    
                    
                    
                    
                    .class public abstract auto ansi sealed Test
                           extends [runtime]System.Object
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
                      .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
                              test() cil managed
                      {
                        
                        .maxstack  4
                        .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
                                 int32 V_1)
                        IL_0000:  ldc.i4.1
                        IL_0001:  stloc.1
                        IL_0002:  ldc.i4.1
                        IL_0003:  stloc.1
                        IL_0004:  br.s       IL_0012
                    
                        IL_0006:  ldloca.s   V_0
                        IL_0008:  ldloc.1
                        IL_0009:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
                        IL_000e:  ldloc.1
                        IL_000f:  ldc.i4.1
                        IL_0010:  add
                        IL_0011:  stloc.1
                        IL_0012:  ldloc.1
                        IL_0013:  ldc.i4.s   101
                        IL_0015:  bge.s      IL_0024
                    
                        IL_0017:  ldc.i4.1
                        IL_0018:  ldloc.1
                        IL_0019:  bge.s      IL_001e
                    
                        IL_001b:  ldc.i4.1
                        IL_001c:  br.s       IL_0029
                    
                        IL_001e:  ldc.i4.1
                        IL_001f:  ldloc.1
                        IL_0020:  ceq
                        IL_0022:  br.s       IL_0029
                    
                        IL_0024:  ldloc.1
                        IL_0025:  ldc.i4.s   101
                        IL_0027:  ceq
                        IL_0029:  brtrue.s   IL_0006
                    
                        IL_002b:  ldloca.s   V_0
                        IL_002d:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
                        IL_0032:  ret
                      } 
                    
                    } 
                    
                    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
                           extends [runtime]System.Object
                    {
                    }
                    """
                    ]))

            [<Test>]
            let ``Lone RangeInt32 with dynamic args lowers to init loop``() =
                CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize+"|],
                    """
                    module Test

                    let test start finish = [start..finish]
                    """,
                    (fun verifier -> verifier.VerifyIL [
                    """
                    .assembly extern runtime { }
                    .assembly extern FSharp.Core { }
                    .assembly assembly
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                                          int32,
                                                                                                                          int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
                      .hash algorithm 0x00008004
                      .ver 0:0:0:0
                    }
                    .mresource public FSharpSignatureData.assembly
                    {
                      
                      
                    }
                    .mresource public FSharpOptimizationData.assembly
                    {
                      
                      
                    }
                    .module assembly.dll
                    
                    .imagebase {value}
                    .file alignment 0x00000200
                    .stackreserve 0x00100000
                    .subsystem 0x0003       
                    .corflags 0x00000001    
                    
                    
                    
                    
                    
                    .class public abstract auto ansi sealed Test
                           extends [runtime]System.Object
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
                      .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
                              test(int32 start,
                                   int32 finish) cil managed
                      {
                        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
                        
                        .maxstack  4
                        .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
                                 int32 V_1)
                        IL_0000:  ldarg.0
                        IL_0001:  stloc.1
                        IL_0002:  ldarg.0
                        IL_0003:  stloc.1
                        IL_0004:  ldarg.1
                        IL_0005:  ldloc.1
                        IL_0006:  bge.s      IL_000a
                    
                        IL_0008:  br.s       IL_002f
                    
                        IL_000a:  br.s       IL_0018
                    
                        IL_000c:  ldloca.s   V_0
                        IL_000e:  ldloc.1
                        IL_000f:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
                        IL_0014:  ldloc.1
                        IL_0015:  ldc.i4.1
                        IL_0016:  add
                        IL_0017:  stloc.1
                        IL_0018:  ldloc.1
                        IL_0019:  ldarg.1
                        IL_001a:  bge.s      IL_0029
                    
                        IL_001c:  ldarg.0
                        IL_001d:  ldloc.1
                        IL_001e:  bge.s      IL_0023
                    
                        IL_0020:  ldc.i4.1
                        IL_0021:  br.s       IL_002d
                    
                        IL_0023:  ldarg.0
                        IL_0024:  ldloc.1
                        IL_0025:  ceq
                        IL_0027:  br.s       IL_002d
                    
                        IL_0029:  ldloc.1
                        IL_002a:  ldarg.1
                        IL_002b:  ceq
                        IL_002d:  brtrue.s   IL_000c
                    
                        IL_002f:  ldloca.s   V_0
                        IL_0031:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
                        IL_0036:  ret
                      } 
                    
                    } 
                    
                    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
                           extends [runtime]System.Object
                    {
                    }
                    """
                    ]))

            [<Test>]
            let ``Lone RangeInt32 with dynamic args that are complex exprs stores those in vars before init loop`` () =
                CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize+"|],
                    """
                    module Test

                    let a () = (Array.zeroCreate 10).Length
                    let b () = (Array.zeroCreate 20).Length

                    let test () = [a ()..b ()]
                    """,
                    (fun verifier -> verifier.VerifyIL [
                    """
                    .assembly extern runtime { }
                    .assembly extern FSharp.Core { }
                    .assembly assembly
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                                          int32,
                                                                                                                          int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
                      .hash algorithm 0x00008004
                      .ver 0:0:0:0
                    }
                    .mresource public FSharpSignatureData.assembly
                    {
                      
                      
                    }
                    .mresource public FSharpOptimizationData.assembly
                    {
                      
                      
                    }
                    .module assembly.dll
                    
                    .imagebase {value}
                    .file alignment 0x00000200
                    .stackreserve 0x00100000
                    .subsystem 0x0003       
                    .corflags 0x00000001    
                    
                    
                    
                    
                    
                    .class public abstract auto ansi sealed Test
                           extends [runtime]System.Object
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
                      .method public static int32  a() cil managed
                      {
                        
                        .maxstack  8
                        IL_0000:  ldc.i4.s   10
                        IL_0002:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::ZeroCreate<object>(int32)
                        IL_0007:  ldlen
                        IL_0008:  conv.i4
                        IL_0009:  ret
                      } 
                    
                      .method public static int32  b() cil managed
                      {
                        
                        .maxstack  8
                        IL_0000:  ldc.i4.s   20
                        IL_0002:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::ZeroCreate<object>(int32)
                        IL_0007:  ldlen
                        IL_0008:  conv.i4
                        IL_0009:  ret
                      } 
                    
                      .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
                              test() cil managed
                      {
                        
                        .maxstack  4
                        .locals init (int32 V_0,
                                 int32 V_1,
                                 valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_2,
                                 int32 V_3)
                        IL_0000:  ldc.i4.s   10
                        IL_0002:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::ZeroCreate<object>(int32)
                        IL_0007:  ldlen
                        IL_0008:  conv.i4
                        IL_0009:  stloc.0
                        IL_000a:  ldc.i4.s   20
                        IL_000c:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::ZeroCreate<object>(int32)
                        IL_0011:  ldlen
                        IL_0012:  conv.i4
                        IL_0013:  stloc.1
                        IL_0014:  ldloc.0
                        IL_0015:  stloc.3
                        IL_0016:  ldloc.0
                        IL_0017:  stloc.3
                        IL_0018:  ldloc.1
                        IL_0019:  ldloc.3
                        IL_001a:  bge.s      IL_001e
                    
                        IL_001c:  br.s       IL_0043
                    
                        IL_001e:  br.s       IL_002c
                    
                        IL_0020:  ldloca.s   V_2
                        IL_0022:  ldloc.3
                        IL_0023:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
                        IL_0028:  ldloc.3
                        IL_0029:  ldc.i4.1
                        IL_002a:  add
                        IL_002b:  stloc.3
                        IL_002c:  ldloc.3
                        IL_002d:  ldloc.1
                        IL_002e:  bge.s      IL_003d
                    
                        IL_0030:  ldloc.0
                        IL_0031:  ldloc.3
                        IL_0032:  bge.s      IL_0037
                    
                        IL_0034:  ldc.i4.1
                        IL_0035:  br.s       IL_0041
                    
                        IL_0037:  ldloc.0
                        IL_0038:  ldloc.3
                        IL_0039:  ceq
                        IL_003b:  br.s       IL_0041
                    
                        IL_003d:  ldloc.3
                        IL_003e:  ldloc.1
                        IL_003f:  ceq
                        IL_0041:  brtrue.s   IL_0020
                    
                        IL_0043:  ldloca.s   V_2
                        IL_0045:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
                        IL_004a:  ret
                      } 
                    
                    } 
                    
                    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
                           extends [runtime]System.Object
                    {
                    }
                    """
                    ]))

        /// [start..step..finish]
        module RangeStep =
            [<Test>]
            let ``Lone RangeInt32 with const args when (finish - start) / step + 1 ≤ 0 lowers to empty list`` () =
                CompilerAssert.CompileLibraryAndVerifyILWithOptions(["--optimize+"],
                    """
                    module Test

                    let test () = [1..-1..5]
                    """,
                    (fun verifier -> verifier.VerifyIL [
                    """
                    .assembly extern runtime { }
                    .assembly extern FSharp.Core { }
                    .assembly assembly
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                                          int32,
                                                                                                                          int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
                      .hash algorithm 0x00008004
                      .ver 0:0:0:0
                    }
                    .mresource public FSharpSignatureData.assembly
                    {
                      
                      
                    }
                    .mresource public FSharpOptimizationData.assembly
                    {
                      
                      
                    }
                    .module assembly.dll
                    
                    .imagebase {value}
                    .file alignment 0x00000200
                    .stackreserve 0x00100000
                    .subsystem 0x0003       
                    .corflags 0x00000001    
                    
                    
                    
                    
                    
                    .class public abstract auto ansi sealed Test
                           extends [runtime]System.Object
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
                      .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
                              test() cil managed
                      {
                        
                        .maxstack  8
                        IL_0000:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
                        IL_0005:  ret
                      } 
                    
                    } 
                    
                    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
                           extends [runtime]System.Object
                    {
                    }
                    """
                    ]))

            [<Test>]
            let ``Lone RangeInt32 with const args lowers to init loop`` () =
                CompilerAssert.CompileLibraryAndVerifyILWithOptions(["--optimize+"],
                    """
                    module Test

                    let test () = [1..2..257]
                    """,
                    (fun verifier -> verifier.VerifyIL [
                    """
                    .assembly extern runtime { }
                    .assembly extern FSharp.Core { }
                    .assembly assembly
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                                          int32,
                                                                                                                          int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
                      .hash algorithm 0x00008004
                      .ver 0:0:0:0
                    }
                    .mresource public FSharpSignatureData.assembly
                    {
                      
                      
                    }
                    .mresource public FSharpOptimizationData.assembly
                    {
                      
                      
                    }
                    .module assembly.dll
                    
                    .imagebase {value}
                    .file alignment 0x00000200
                    .stackreserve 0x00100000
                    .subsystem 0x0003       
                    .corflags 0x00000001    
                    
                    
                    
                    
                    
                    .class public abstract auto ansi sealed Test
                           extends [runtime]System.Object
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
                      .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
                              test() cil managed
                      {
                        
                        .maxstack  4
                        .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
                                 int32 V_1)
                        IL_0000:  ldc.i4.1
                        IL_0001:  stloc.1
                        IL_0002:  ldc.i4.1
                        IL_0003:  stloc.1
                        IL_0004:  br.s       IL_0012
                    
                        IL_0006:  ldloca.s   V_0
                        IL_0008:  ldloc.1
                        IL_0009:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
                        IL_000e:  ldloc.1
                        IL_000f:  ldc.i4.2
                        IL_0010:  add
                        IL_0011:  stloc.1
                        IL_0012:  ldloc.1
                        IL_0013:  ldc.i4     0x101
                        IL_0018:  bge.s      IL_0027
                    
                        IL_001a:  ldc.i4.1
                        IL_001b:  ldloc.1
                        IL_001c:  bge.s      IL_0021
                    
                        IL_001e:  ldc.i4.1
                        IL_001f:  br.s       IL_002f
                    
                        IL_0021:  ldc.i4.1
                        IL_0022:  ldloc.1
                        IL_0023:  ceq
                        IL_0025:  br.s       IL_002f
                    
                        IL_0027:  ldloc.1
                        IL_0028:  ldc.i4     0x101
                        IL_002d:  ceq
                        IL_002f:  brtrue.s   IL_0006
                    
                        IL_0031:  ldloca.s   V_0
                        IL_0033:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
                        IL_0038:  ret
                      } 
                    
                    } 
                    
                    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
                           extends [runtime]System.Object
                    {
                    }
                    """
                    ]))

            [<Test>]
            let ``Lone RangeInt32 with dynamic args lowers to init loop`` () =
                CompilerAssert.CompileLibraryAndVerifyILWithOptions(["--optimize+"],
                    """
                    module Test

                    let test start step finish = [start..step..finish]
                    """,
                    (fun verifier -> verifier.VerifyIL [
                    """
                    .assembly extern runtime { }
                    .assembly extern FSharp.Core { }
                    .assembly assembly
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                                          int32,
                                                                                                                          int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
                      .hash algorithm 0x00008004
                      .ver 0:0:0:0
                    }
                    .mresource public FSharpSignatureData.assembly
                    {
                      
                      
                    }
                    .mresource public FSharpOptimizationData.assembly
                    {
                      
                      
                    }
                    .module assembly.dll
                    
                    .imagebase {value}
                    .file alignment 0x00000200
                    .stackreserve 0x00100000
                    .subsystem 0x0003       
                    .corflags 0x00000001    
                    
                    
                    
                    
                    
                    .class public abstract auto ansi sealed Test
                           extends [runtime]System.Object
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
                      .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
                              test(int32 start,
                                   int32 step,
                                   int32 finish) cil managed
                      {
                        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 03 00 00 00 01 00 00 00 01 00 00 00 01 00 
                                                                                                                                        00 00 00 00 ) 
                        
                        .maxstack  5
                        .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
                                 int32 V_1)
                        IL_0000:  ldarg.0
                        IL_0001:  stloc.1
                        IL_0002:  ldarg.0
                        IL_0003:  stloc.1
                        IL_0004:  ldarg.1
                        IL_0005:  brtrue.s   IL_0012
                    
                        IL_0007:  ldloc.1
                        IL_0008:  ldarg.1
                        IL_0009:  ldarg.2
                        IL_000a:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                                               int32,
                                                                                                                                                                                               int32)
                        IL_000f:  pop
                        IL_0010:  br.s       IL_0062
                    
                        IL_0012:  ldc.i4.0
                        IL_0013:  ldarg.1
                        IL_0014:  bge.s      IL_003d
                    
                        IL_0016:  br.s       IL_0024
                    
                        IL_0018:  ldloca.s   V_0
                        IL_001a:  ldloc.1
                        IL_001b:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
                        IL_0020:  ldloc.1
                        IL_0021:  ldarg.1
                        IL_0022:  add
                        IL_0023:  stloc.1
                        IL_0024:  ldloc.1
                        IL_0025:  ldarg.2
                        IL_0026:  bge.s      IL_0035
                    
                        IL_0028:  ldarg.0
                        IL_0029:  ldloc.1
                        IL_002a:  bge.s      IL_002f
                    
                        IL_002c:  ldc.i4.1
                        IL_002d:  br.s       IL_0039
                    
                        IL_002f:  ldarg.0
                        IL_0030:  ldloc.1
                        IL_0031:  ceq
                        IL_0033:  br.s       IL_0039
                    
                        IL_0035:  ldloc.1
                        IL_0036:  ldarg.2
                        IL_0037:  ceq
                        IL_0039:  brtrue.s   IL_0018
                    
                        IL_003b:  br.s       IL_0062
                    
                        IL_003d:  br.s       IL_004b
                    
                        IL_003f:  ldloca.s   V_0
                        IL_0041:  ldloc.1
                        IL_0042:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
                        IL_0047:  ldloc.1
                        IL_0048:  ldarg.1
                        IL_0049:  add
                        IL_004a:  stloc.1
                        IL_004b:  ldarg.2
                        IL_004c:  ldloc.1
                        IL_004d:  bge.s      IL_005c
                    
                        IL_004f:  ldloc.1
                        IL_0050:  ldarg.0
                        IL_0051:  bge.s      IL_0056
                    
                        IL_0053:  ldc.i4.1
                        IL_0054:  br.s       IL_0060
                    
                        IL_0056:  ldloc.1
                        IL_0057:  ldarg.0
                        IL_0058:  ceq
                        IL_005a:  br.s       IL_0060
                    
                        IL_005c:  ldarg.2
                        IL_005d:  ldloc.1
                        IL_005e:  ceq
                        IL_0060:  brtrue.s   IL_003f
                    
                        IL_0062:  ldloca.s   V_0
                        IL_0064:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
                        IL_0069:  ret
                      } 
                    
                    } 
                    
                    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
                           extends [runtime]System.Object
                    {
                    }
                    """
                    ]))

            [<Test>]
            let ``Lone RangeInt32 with dynamic args that are complex exprs stores those in vars before init loop`` () =
                CompilerAssert.CompileLibraryAndVerifyILWithOptions(["--optimize+"],
                    """
                    module Test

                    let a () = (Array.zeroCreate 10).Length
                    let b () = (Array.zeroCreate 20).Length
                    let c () = (Array.zeroCreate 300).Length

                    let test () = [a () .. b () .. c ()]
                    """,
                    (fun verifier -> verifier.VerifyIL [
                    """
                    .assembly extern runtime { }
                    .assembly extern FSharp.Core { }
                    .assembly assembly
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.FSharpInterfaceDataVersionAttribute::.ctor(int32,
                                                                                                                          int32,
                                                                                                                          int32) = ( 01 00 02 00 00 00 00 00 00 00 00 00 00 00 00 00 ) 
                      .hash algorithm 0x00008004
                      .ver 0:0:0:0
                    }
                    .mresource public FSharpSignatureData.assembly
                    {
                      
                      
                    }
                    .mresource public FSharpOptimizationData.assembly
                    {
                      
                      
                    }
                    .module assembly.dll
                    
                    .imagebase {value}
                    .file alignment 0x00000200
                    .stackreserve 0x00100000
                    .subsystem 0x0003       
                    .corflags 0x00000001    
                    
                    
                    
                    
                    
                    .class public abstract auto ansi sealed Test
                           extends [runtime]System.Object
                    {
                      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
                      .method public static int32  a() cil managed
                      {
                        
                        .maxstack  8
                        IL_0000:  ldc.i4.s   10
                        IL_0002:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::ZeroCreate<object>(int32)
                        IL_0007:  ldlen
                        IL_0008:  conv.i4
                        IL_0009:  ret
                      } 
                    
                      .method public static int32  b() cil managed
                      {
                        
                        .maxstack  8
                        IL_0000:  ldc.i4.s   20
                        IL_0002:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::ZeroCreate<object>(int32)
                        IL_0007:  ldlen
                        IL_0008:  conv.i4
                        IL_0009:  ret
                      } 
                    
                      .method public static int32  c() cil managed
                      {
                        
                        .maxstack  8
                        IL_0000:  ldc.i4     0x12c
                        IL_0005:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::ZeroCreate<object>(int32)
                        IL_000a:  ldlen
                        IL_000b:  conv.i4
                        IL_000c:  ret
                      } 
                    
                      .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
                              test() cil managed
                      {
                        
                        .maxstack  5
                        .locals init (int32 V_0,
                                 int32 V_1,
                                 int32 V_2,
                                 valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_3,
                                 int32 V_4)
                        IL_0000:  ldc.i4.s   10
                        IL_0002:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::ZeroCreate<object>(int32)
                        IL_0007:  ldlen
                        IL_0008:  conv.i4
                        IL_0009:  stloc.0
                        IL_000a:  ldc.i4.s   20
                        IL_000c:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::ZeroCreate<object>(int32)
                        IL_0011:  ldlen
                        IL_0012:  conv.i4
                        IL_0013:  stloc.1
                        IL_0014:  ldc.i4     0x12c
                        IL_0019:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::ZeroCreate<object>(int32)
                        IL_001e:  ldlen
                        IL_001f:  conv.i4
                        IL_0020:  stloc.2
                        IL_0021:  ldloc.0
                        IL_0022:  stloc.s    V_4
                        IL_0024:  ldloc.0
                        IL_0025:  stloc.s    V_4
                        IL_0027:  ldloc.1
                        IL_0028:  brtrue.s   IL_0039
                    
                        IL_002a:  ldloc.s    V_4
                        IL_002c:  ldloc.1
                        IL_002d:  ldloc.2
                        IL_002e:  call       class [runtime]System.Collections.Generic.IEnumerable`1<int32> [FSharp.Core]Microsoft.FSharp.Core.Operators/OperatorIntrinsics::RangeInt32(int32,
                                                                                                                                                                                               int32,
                                                                                                                                                                                               int32)
                        IL_0033:  pop
                        IL_0034:  br         IL_0097
                    
                        IL_0039:  ldc.i4.0
                        IL_003a:  ldloc.1
                        IL_003b:  bge.s      IL_006b
                    
                        IL_003d:  br.s       IL_004e
                    
                        IL_003f:  ldloca.s   V_3
                        IL_0041:  ldloc.s    V_4
                        IL_0043:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
                        IL_0048:  ldloc.s    V_4
                        IL_004a:  ldloc.1
                        IL_004b:  add
                        IL_004c:  stloc.s    V_4
                        IL_004e:  ldloc.s    V_4
                        IL_0050:  ldloc.2
                        IL_0051:  bge.s      IL_0062
                    
                        IL_0053:  ldloc.0
                        IL_0054:  ldloc.s    V_4
                        IL_0056:  bge.s      IL_005b
                    
                        IL_0058:  ldc.i4.1
                        IL_0059:  br.s       IL_0067
                    
                        IL_005b:  ldloc.0
                        IL_005c:  ldloc.s    V_4
                        IL_005e:  ceq
                        IL_0060:  br.s       IL_0067
                    
                        IL_0062:  ldloc.s    V_4
                        IL_0064:  ldloc.2
                        IL_0065:  ceq
                        IL_0067:  brtrue.s   IL_003f
                    
                        IL_0069:  br.s       IL_0097
                    
                        IL_006b:  br.s       IL_007c
                    
                        IL_006d:  ldloca.s   V_3
                        IL_006f:  ldloc.s    V_4
                        IL_0071:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
                        IL_0076:  ldloc.s    V_4
                        IL_0078:  ldloc.1
                        IL_0079:  add
                        IL_007a:  stloc.s    V_4
                        IL_007c:  ldloc.2
                        IL_007d:  ldloc.s    V_4
                        IL_007f:  bge.s      IL_0090
                    
                        IL_0081:  ldloc.s    V_4
                        IL_0083:  ldloc.0
                        IL_0084:  bge.s      IL_0089
                    
                        IL_0086:  ldc.i4.1
                        IL_0087:  br.s       IL_0095
                    
                        IL_0089:  ldloc.s    V_4
                        IL_008b:  ldloc.0
                        IL_008c:  ceq
                        IL_008e:  br.s       IL_0095
                    
                        IL_0090:  ldloc.2
                        IL_0091:  ldloc.s    V_4
                        IL_0093:  ceq
                        IL_0095:  brtrue.s   IL_006d
                    
                        IL_0097:  ldloca.s   V_3
                        IL_0099:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
                        IL_009e:  ret
                      } 
                    
                    } 
                    
                    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
                           extends [runtime]System.Object
                    {
                    }
                    """
                    ]))
