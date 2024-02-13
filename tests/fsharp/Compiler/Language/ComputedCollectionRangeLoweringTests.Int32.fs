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
                        IL_000f:  br.s       IL_001d
                    
                        IL_0011:  ldloc.0
                        IL_0012:  ldloc.1
                        IL_0013:  ldloc.2
                        IL_0014:  stelem.i4
                        IL_0015:  ldloc.2
                        IL_0016:  ldc.i4.1
                        IL_0017:  add
                        IL_0018:  stloc.2
                        IL_0019:  ldloc.1
                        IL_001a:  ldc.i4.1
                        IL_001b:  add
                        IL_001c:  stloc.1
                        IL_001d:  ldloc.1
                        IL_001e:  ldc.i4     0x101
                        IL_0023:  blt.un.s   IL_0011
                    
                        IL_0025:  ldloc.0
                        IL_0026:  ret
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
                        .locals init (uint64 V_0,
                                 int32[] V_1,
                                 uint64 V_2,
                                 int32 V_3)
                        IL_0000:  ldarg.1
                        IL_0001:  ldarg.0
                        IL_0002:  bge.s      IL_0008
                    
                        IL_0004:  ldc.i4.0
                        IL_0005:  conv.i8
                        IL_0006:  br.s       IL_0010
                    
                        IL_0008:  ldarg.1
                        IL_0009:  conv.i8
                        IL_000a:  ldarg.0
                        IL_000b:  conv.i8
                        IL_000c:  sub
                        IL_000d:  ldc.i4.1
                        IL_000e:  conv.i8
                        IL_000f:  add
                        IL_0010:  stloc.0
                        IL_0011:  ldloc.0
                        IL_0012:  ldc.i4.1
                        IL_0013:  bge.un.s   IL_001b
                    
                        IL_0015:  call       !!0[] [runtime]System.Array::Empty<int32>()
                        IL_001a:  ret
                    
                        IL_001b:  ldloc.0
                        IL_001c:  conv.ovf.i.un
                        IL_001d:  newarr     [runtime]System.Int32
                        IL_0022:  stloc.1
                        IL_0023:  ldc.i4.0
                        IL_0024:  conv.i8
                        IL_0025:  stloc.2
                        IL_0026:  ldarg.0
                        IL_0027:  stloc.3
                        IL_0028:  br.s       IL_0038
                    
                        IL_002a:  ldloc.1
                        IL_002b:  ldloc.2
                        IL_002c:  conv.ovf.i.un
                        IL_002d:  ldloc.3
                        IL_002e:  stelem.i4
                        IL_002f:  ldloc.3
                        IL_0030:  ldc.i4.1
                        IL_0031:  add
                        IL_0032:  stloc.3
                        IL_0033:  ldloc.2
                        IL_0034:  ldc.i4.1
                        IL_0035:  conv.i8
                        IL_0036:  add
                        IL_0037:  stloc.2
                        IL_0038:  ldloc.2
                        IL_0039:  ldloc.0
                        IL_003a:  blt.un.s   IL_002a
                    
                        IL_003c:  ldloc.1
                        IL_003d:  ret
                      } 
                    
                    } 
                    
                    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
                           extends [runtime]System.Object
                    {
                    }
                    """
                    ]))

            [<Test>]
            let ``Lone RangeInt32 with dynamic args that are not consts or bound vals stores those in vars before init loop`` () =
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
                                 uint64 V_2,
                                 int32[] V_3,
                                 uint64 V_4,
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
                        IL_0016:  bge.s      IL_001c
                    
                        IL_0018:  ldc.i4.0
                        IL_0019:  conv.i8
                        IL_001a:  br.s       IL_0024
                    
                        IL_001c:  ldloc.1
                        IL_001d:  conv.i8
                        IL_001e:  ldloc.0
                        IL_001f:  conv.i8
                        IL_0020:  sub
                        IL_0021:  ldc.i4.1
                        IL_0022:  conv.i8
                        IL_0023:  add
                        IL_0024:  stloc.2
                        IL_0025:  ldloc.2
                        IL_0026:  ldc.i4.1
                        IL_0027:  bge.un.s   IL_002f
                    
                        IL_0029:  call       !!0[] [runtime]System.Array::Empty<int32>()
                        IL_002e:  ret
                    
                        IL_002f:  ldloc.2
                        IL_0030:  conv.ovf.i.un
                        IL_0031:  newarr     [runtime]System.Int32
                        IL_0036:  stloc.3
                        IL_0037:  ldc.i4.0
                        IL_0038:  conv.i8
                        IL_0039:  stloc.s    V_4
                        IL_003b:  ldloc.0
                        IL_003c:  stloc.s    V_5
                        IL_003e:  br.s       IL_0054
                    
                        IL_0040:  ldloc.3
                        IL_0041:  ldloc.s    V_4
                        IL_0043:  conv.ovf.i.un
                        IL_0044:  ldloc.s    V_5
                        IL_0046:  stelem.i4
                        IL_0047:  ldloc.s    V_5
                        IL_0049:  ldc.i4.1
                        IL_004a:  add
                        IL_004b:  stloc.s    V_5
                        IL_004d:  ldloc.s    V_4
                        IL_004f:  ldc.i4.1
                        IL_0050:  conv.i8
                        IL_0051:  add
                        IL_0052:  stloc.s    V_4
                        IL_0054:  ldloc.s    V_4
                        IL_0056:  ldloc.2
                        IL_0057:  blt.un.s   IL_0040
                    
                        IL_0059:  ldloc.3
                        IL_005a:  ret
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
                        IL_000f:  br.s       IL_001d
                    
                        IL_0011:  ldloc.0
                        IL_0012:  ldloc.1
                        IL_0013:  ldloc.2
                        IL_0014:  stelem.i4
                        IL_0015:  ldloc.2
                        IL_0016:  ldc.i4.2
                        IL_0017:  add
                        IL_0018:  stloc.2
                        IL_0019:  ldloc.1
                        IL_001a:  ldc.i4.1
                        IL_001b:  add
                        IL_001c:  stloc.1
                        IL_001d:  ldloc.1
                        IL_001e:  ldc.i4     0x81
                        IL_0023:  blt.un.s   IL_0011
                    
                        IL_0025:  ldloc.0
                        IL_0026:  ret
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
                        .locals init (uint64 V_0,
                                 int32[] V_1,
                                 uint64 V_2,
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
                    
                        IL_000e:  ldc.i4.0
                        IL_000f:  ldarg.1
                        IL_0010:  bge.s      IL_0027
                    
                        IL_0012:  ldarg.2
                        IL_0013:  ldarg.0
                        IL_0014:  bge.s      IL_001a
                    
                        IL_0016:  ldc.i4.0
                        IL_0017:  conv.i8
                        IL_0018:  br.s       IL_004c
                    
                        IL_001a:  ldarg.2
                        IL_001b:  conv.i8
                        IL_001c:  ldarg.0
                        IL_001d:  conv.i8
                        IL_001e:  sub
                        IL_001f:  ldarg.1
                        IL_0020:  conv.i8
                        IL_0021:  div.un
                        IL_0022:  ldc.i4.1
                        IL_0023:  conv.i8
                        IL_0024:  add
                        IL_0025:  br.s       IL_004c
                    
                        IL_0027:  ldarg.0
                        IL_0028:  ldarg.2
                        IL_0029:  bge.s      IL_002f
                    
                        IL_002b:  ldc.i4.0
                        IL_002c:  conv.i8
                        IL_002d:  br.s       IL_004c
                    
                        IL_002f:  ldarg.0
                        IL_0030:  conv.i8
                        IL_0031:  ldarg.2
                        IL_0032:  conv.i8
                        IL_0033:  sub
                        IL_0034:  ldarg.1
                        IL_0035:  ldc.i4     0x80000000
                        IL_003a:  bne.un.s   IL_0044
                    
                        IL_003c:  ldc.i4     0x80000000
                        IL_0041:  conv.u8
                        IL_0042:  br.s       IL_0047
                    
                        IL_0044:  ldarg.1
                        IL_0045:  neg
                        IL_0046:  conv.i8
                        IL_0047:  conv.i8
                        IL_0048:  div.un
                        IL_0049:  ldc.i4.1
                        IL_004a:  conv.i8
                        IL_004b:  add
                        IL_004c:  stloc.0
                        IL_004d:  ldloc.0
                        IL_004e:  ldc.i4.1
                        IL_004f:  bge.un.s   IL_0057
                    
                        IL_0051:  call       !!0[] [runtime]System.Array::Empty<int32>()
                        IL_0056:  ret
                    
                        IL_0057:  ldloc.0
                        IL_0058:  conv.ovf.i.un
                        IL_0059:  newarr     [runtime]System.Int32
                        IL_005e:  stloc.1
                        IL_005f:  ldc.i4.0
                        IL_0060:  conv.i8
                        IL_0061:  stloc.2
                        IL_0062:  ldarg.0
                        IL_0063:  stloc.3
                        IL_0064:  br.s       IL_0074
                    
                        IL_0066:  ldloc.1
                        IL_0067:  ldloc.2
                        IL_0068:  conv.ovf.i.un
                        IL_0069:  ldloc.3
                        IL_006a:  stelem.i4
                        IL_006b:  ldloc.3
                        IL_006c:  ldarg.1
                        IL_006d:  add
                        IL_006e:  stloc.3
                        IL_006f:  ldloc.2
                        IL_0070:  ldc.i4.1
                        IL_0071:  conv.i8
                        IL_0072:  add
                        IL_0073:  stloc.2
                        IL_0074:  ldloc.2
                        IL_0075:  ldloc.0
                        IL_0076:  blt.un.s   IL_0066
                    
                        IL_0078:  ldloc.1
                        IL_0079:  ret
                      } 
                    
                    } 
                    
                    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
                           extends [runtime]System.Object
                    {
                    }
                    """
                    ]))

            [<Test>]
            let ``Lone RangeInt32 with dynamic args that are not consts or bound vals stores those in vars before init loop`` () =
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
                                 uint64 V_3,
                                 int32[] V_4,
                                 uint64 V_5,
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
                    
                        IL_002f:  ldc.i4.0
                        IL_0030:  ldloc.1
                        IL_0031:  bge.s      IL_0048
                    
                        IL_0033:  ldloc.2
                        IL_0034:  ldloc.0
                        IL_0035:  bge.s      IL_003b
                    
                        IL_0037:  ldc.i4.0
                        IL_0038:  conv.i8
                        IL_0039:  br.s       IL_006d
                    
                        IL_003b:  ldloc.2
                        IL_003c:  conv.i8
                        IL_003d:  ldloc.0
                        IL_003e:  conv.i8
                        IL_003f:  sub
                        IL_0040:  ldloc.1
                        IL_0041:  conv.i8
                        IL_0042:  div.un
                        IL_0043:  ldc.i4.1
                        IL_0044:  conv.i8
                        IL_0045:  add
                        IL_0046:  br.s       IL_006d
                    
                        IL_0048:  ldloc.0
                        IL_0049:  ldloc.2
                        IL_004a:  bge.s      IL_0050
                    
                        IL_004c:  ldc.i4.0
                        IL_004d:  conv.i8
                        IL_004e:  br.s       IL_006d
                    
                        IL_0050:  ldloc.0
                        IL_0051:  conv.i8
                        IL_0052:  ldloc.2
                        IL_0053:  conv.i8
                        IL_0054:  sub
                        IL_0055:  ldloc.1
                        IL_0056:  ldc.i4     0x80000000
                        IL_005b:  bne.un.s   IL_0065
                    
                        IL_005d:  ldc.i4     0x80000000
                        IL_0062:  conv.u8
                        IL_0063:  br.s       IL_0068
                    
                        IL_0065:  ldloc.1
                        IL_0066:  neg
                        IL_0067:  conv.i8
                        IL_0068:  conv.i8
                        IL_0069:  div.un
                        IL_006a:  ldc.i4.1
                        IL_006b:  conv.i8
                        IL_006c:  add
                        IL_006d:  stloc.3
                        IL_006e:  ldloc.3
                        IL_006f:  ldc.i4.1
                        IL_0070:  bge.un.s   IL_0078
                    
                        IL_0072:  call       !!0[] [runtime]System.Array::Empty<int32>()
                        IL_0077:  ret
                    
                        IL_0078:  ldloc.3
                        IL_0079:  conv.ovf.i.un
                        IL_007a:  newarr     [runtime]System.Int32
                        IL_007f:  stloc.s    V_4
                        IL_0081:  ldc.i4.0
                        IL_0082:  conv.i8
                        IL_0083:  stloc.s    V_5
                        IL_0085:  ldloc.0
                        IL_0086:  stloc.s    V_6
                        IL_0088:  br.s       IL_009f
                    
                        IL_008a:  ldloc.s    V_4
                        IL_008c:  ldloc.s    V_5
                        IL_008e:  conv.ovf.i.un
                        IL_008f:  ldloc.s    V_6
                        IL_0091:  stelem.i4
                        IL_0092:  ldloc.s    V_6
                        IL_0094:  ldloc.1
                        IL_0095:  add
                        IL_0096:  stloc.s    V_6
                        IL_0098:  ldloc.s    V_5
                        IL_009a:  ldc.i4.1
                        IL_009b:  conv.i8
                        IL_009c:  add
                        IL_009d:  stloc.s    V_5
                        IL_009f:  ldloc.s    V_5
                        IL_00a1:  ldloc.3
                        IL_00a2:  blt.un.s   IL_008a
                    
                        IL_00a4:  ldloc.s    V_4
                        IL_00a6:  ret
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
                                 int32 V_1,
                                 int32 V_2)
                        IL_0000:  ldc.i4.0
                        IL_0001:  stloc.1
                        IL_0002:  ldc.i4.1
                        IL_0003:  stloc.2
                        IL_0004:  br.s       IL_0016
                    
                        IL_0006:  ldloca.s   V_0
                        IL_0008:  ldloc.2
                        IL_0009:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
                        IL_000e:  ldloc.2
                        IL_000f:  ldc.i4.1
                        IL_0010:  add
                        IL_0011:  stloc.2
                        IL_0012:  ldloc.1
                        IL_0013:  ldc.i4.1
                        IL_0014:  add
                        IL_0015:  stloc.1
                        IL_0016:  ldloc.1
                        IL_0017:  ldc.i4.s   101
                        IL_0019:  blt.un.s   IL_0006
                    
                        IL_001b:  ldloca.s   V_0
                        IL_001d:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
                        IL_0022:  ret
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
                        .locals init (uint64 V_0,
                                 valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_1,
                                 uint64 V_2,
                                 int32 V_3)
                        IL_0000:  ldarg.1
                        IL_0001:  ldarg.0
                        IL_0002:  bge.s      IL_0008
                    
                        IL_0004:  ldc.i4.0
                        IL_0005:  conv.i8
                        IL_0006:  br.s       IL_0010
                    
                        IL_0008:  ldarg.1
                        IL_0009:  conv.i8
                        IL_000a:  ldarg.0
                        IL_000b:  conv.i8
                        IL_000c:  sub
                        IL_000d:  ldc.i4.1
                        IL_000e:  conv.i8
                        IL_000f:  add
                        IL_0010:  stloc.0
                        IL_0011:  ldc.i4.0
                        IL_0012:  conv.i8
                        IL_0013:  stloc.2
                        IL_0014:  ldarg.0
                        IL_0015:  stloc.3
                        IL_0016:  br.s       IL_0029
                    
                        IL_0018:  ldloca.s   V_1
                        IL_001a:  ldloc.3
                        IL_001b:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
                        IL_0020:  ldloc.3
                        IL_0021:  ldc.i4.1
                        IL_0022:  add
                        IL_0023:  stloc.3
                        IL_0024:  ldloc.2
                        IL_0025:  ldc.i4.1
                        IL_0026:  conv.i8
                        IL_0027:  add
                        IL_0028:  stloc.2
                        IL_0029:  ldloc.2
                        IL_002a:  ldloc.0
                        IL_002b:  blt.un.s   IL_0018
                    
                        IL_002d:  ldloca.s   V_1
                        IL_002f:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
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
            let ``Lone RangeInt32 with dynamic args that are not consts or bound vals stores those in vars before init loop`` () =
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
                    
                      .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> test() cil managed
                      {
                        
                        .maxstack  4
                        .locals init (int32 V_0,
                                 int32 V_1,
                                 uint64 V_2,
                                 valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_3,
                                 uint64 V_4,
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
                        IL_0016:  bge.s      IL_001c
                    
                        IL_0018:  ldc.i4.0
                        IL_0019:  conv.i8
                        IL_001a:  br.s       IL_0024
                    
                        IL_001c:  ldloc.1
                        IL_001d:  conv.i8
                        IL_001e:  ldloc.0
                        IL_001f:  conv.i8
                        IL_0020:  sub
                        IL_0021:  ldc.i4.1
                        IL_0022:  conv.i8
                        IL_0023:  add
                        IL_0024:  stloc.2
                        IL_0025:  ldc.i4.0
                        IL_0026:  conv.i8
                        IL_0027:  stloc.s    V_4
                        IL_0029:  ldloc.0
                        IL_002a:  stloc.s    V_5
                        IL_002c:  br.s       IL_0044
                    
                        IL_002e:  ldloca.s   V_3
                        IL_0030:  ldloc.s    V_5
                        IL_0032:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
                        IL_0037:  ldloc.s    V_5
                        IL_0039:  ldc.i4.1
                        IL_003a:  add
                        IL_003b:  stloc.s    V_5
                        IL_003d:  ldloc.s    V_4
                        IL_003f:  ldc.i4.1
                        IL_0040:  conv.i8
                        IL_0041:  add
                        IL_0042:  stloc.s    V_4
                        IL_0044:  ldloc.s    V_4
                        IL_0046:  ldloc.2
                        IL_0047:  blt.un.s   IL_002e
                    
                        IL_0049:  ldloca.s   V_3
                        IL_004b:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
                        IL_0050:  ret
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
                                 int32 V_1,
                                 int32 V_2)
                        IL_0000:  ldc.i4.0
                        IL_0001:  stloc.1
                        IL_0002:  ldc.i4.1
                        IL_0003:  stloc.2
                        IL_0004:  br.s       IL_0016
                    
                        IL_0006:  ldloca.s   V_0
                        IL_0008:  ldloc.2
                        IL_0009:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
                        IL_000e:  ldloc.2
                        IL_000f:  ldc.i4.2
                        IL_0010:  add
                        IL_0011:  stloc.2
                        IL_0012:  ldloc.1
                        IL_0013:  ldc.i4.1
                        IL_0014:  add
                        IL_0015:  stloc.1
                        IL_0016:  ldloc.1
                        IL_0017:  ldc.i4     0x81
                        IL_001c:  blt.un.s   IL_0006
                    
                        IL_001e:  ldloca.s   V_0
                        IL_0020:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
                        IL_0025:  ret
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
                        .locals init (uint64 V_0,
                                 valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_1,
                                 uint64 V_2,
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
                    
                        IL_000e:  ldc.i4.0
                        IL_000f:  ldarg.1
                        IL_0010:  bge.s      IL_0027
                    
                        IL_0012:  ldarg.2
                        IL_0013:  ldarg.0
                        IL_0014:  bge.s      IL_001a
                    
                        IL_0016:  ldc.i4.0
                        IL_0017:  conv.i8
                        IL_0018:  br.s       IL_004c
                    
                        IL_001a:  ldarg.2
                        IL_001b:  conv.i8
                        IL_001c:  ldarg.0
                        IL_001d:  conv.i8
                        IL_001e:  sub
                        IL_001f:  ldarg.1
                        IL_0020:  conv.i8
                        IL_0021:  div.un
                        IL_0022:  ldc.i4.1
                        IL_0023:  conv.i8
                        IL_0024:  add
                        IL_0025:  br.s       IL_004c
                    
                        IL_0027:  ldarg.0
                        IL_0028:  ldarg.2
                        IL_0029:  bge.s      IL_002f
                    
                        IL_002b:  ldc.i4.0
                        IL_002c:  conv.i8
                        IL_002d:  br.s       IL_004c
                    
                        IL_002f:  ldarg.0
                        IL_0030:  conv.i8
                        IL_0031:  ldarg.2
                        IL_0032:  conv.i8
                        IL_0033:  sub
                        IL_0034:  ldarg.1
                        IL_0035:  ldc.i4     0x80000000
                        IL_003a:  bne.un.s   IL_0044
                    
                        IL_003c:  ldc.i4     0x80000000
                        IL_0041:  conv.u8
                        IL_0042:  br.s       IL_0047
                    
                        IL_0044:  ldarg.1
                        IL_0045:  neg
                        IL_0046:  conv.i8
                        IL_0047:  conv.i8
                        IL_0048:  div.un
                        IL_0049:  ldc.i4.1
                        IL_004a:  conv.i8
                        IL_004b:  add
                        IL_004c:  stloc.0
                        IL_004d:  ldc.i4.0
                        IL_004e:  conv.i8
                        IL_004f:  stloc.2
                        IL_0050:  ldarg.0
                        IL_0051:  stloc.3
                        IL_0052:  br.s       IL_0065
                    
                        IL_0054:  ldloca.s   V_1
                        IL_0056:  ldloc.3
                        IL_0057:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
                        IL_005c:  ldloc.3
                        IL_005d:  ldarg.1
                        IL_005e:  add
                        IL_005f:  stloc.3
                        IL_0060:  ldloc.2
                        IL_0061:  ldc.i4.1
                        IL_0062:  conv.i8
                        IL_0063:  add
                        IL_0064:  stloc.2
                        IL_0065:  ldloc.2
                        IL_0066:  ldloc.0
                        IL_0067:  blt.un.s   IL_0054
                    
                        IL_0069:  ldloca.s   V_1
                        IL_006b:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
                        IL_0070:  ret
                      } 
                    
                    } 
                    
                    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
                           extends [runtime]System.Object
                    {
                    }
                    """
                    ]))

            [<Test>]
            let ``Lone RangeInt32 with dynamic args that are not consts or bound vals stores those in vars before init loop`` () =
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
                    
                      .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> test() cil managed
                      {
                        
                        .maxstack  5
                        .locals init (int32 V_0,
                                 int32 V_1,
                                 int32 V_2,
                                 uint64 V_3,
                                 valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_4,
                                 uint64 V_5,
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
                    
                        IL_002f:  ldc.i4.0
                        IL_0030:  ldloc.1
                        IL_0031:  bge.s      IL_0048
                    
                        IL_0033:  ldloc.2
                        IL_0034:  ldloc.0
                        IL_0035:  bge.s      IL_003b
                    
                        IL_0037:  ldc.i4.0
                        IL_0038:  conv.i8
                        IL_0039:  br.s       IL_006d
                    
                        IL_003b:  ldloc.2
                        IL_003c:  conv.i8
                        IL_003d:  ldloc.0
                        IL_003e:  conv.i8
                        IL_003f:  sub
                        IL_0040:  ldloc.1
                        IL_0041:  conv.i8
                        IL_0042:  div.un
                        IL_0043:  ldc.i4.1
                        IL_0044:  conv.i8
                        IL_0045:  add
                        IL_0046:  br.s       IL_006d
                    
                        IL_0048:  ldloc.0
                        IL_0049:  ldloc.2
                        IL_004a:  bge.s      IL_0050
                    
                        IL_004c:  ldc.i4.0
                        IL_004d:  conv.i8
                        IL_004e:  br.s       IL_006d
                    
                        IL_0050:  ldloc.0
                        IL_0051:  conv.i8
                        IL_0052:  ldloc.2
                        IL_0053:  conv.i8
                        IL_0054:  sub
                        IL_0055:  ldloc.1
                        IL_0056:  ldc.i4     0x80000000
                        IL_005b:  bne.un.s   IL_0065
                    
                        IL_005d:  ldc.i4     0x80000000
                        IL_0062:  conv.u8
                        IL_0063:  br.s       IL_0068
                    
                        IL_0065:  ldloc.1
                        IL_0066:  neg
                        IL_0067:  conv.i8
                        IL_0068:  conv.i8
                        IL_0069:  div.un
                        IL_006a:  ldc.i4.1
                        IL_006b:  conv.i8
                        IL_006c:  add
                        IL_006d:  stloc.3
                        IL_006e:  ldc.i4.0
                        IL_006f:  conv.i8
                        IL_0070:  stloc.s    V_5
                        IL_0072:  ldloc.0
                        IL_0073:  stloc.s    V_6
                        IL_0075:  br.s       IL_008d
                    
                        IL_0077:  ldloca.s   V_4
                        IL_0079:  ldloc.s    V_6
                        IL_007b:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
                        IL_0080:  ldloc.s    V_6
                        IL_0082:  ldloc.1
                        IL_0083:  add
                        IL_0084:  stloc.s    V_6
                        IL_0086:  ldloc.s    V_5
                        IL_0088:  ldc.i4.1
                        IL_0089:  conv.i8
                        IL_008a:  add
                        IL_008b:  stloc.s    V_5
                        IL_008d:  ldloc.s    V_5
                        IL_008f:  ldloc.3
                        IL_0090:  blt.un.s   IL_0077
                    
                        IL_0092:  ldloca.s   V_4
                        IL_0094:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
                        IL_0099:  ret
                      } 
                    
                    } 
                    
                    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
                           extends [runtime]System.Object
                    {
                    }
                    """
                    ]))
