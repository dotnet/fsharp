// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests.ComputedCollectionRangeLoweringTests

open NUnit.Framework
open FSharp.Test

// TODO https://github.com/dotnet/fsharp/issues/16739: Remove /langversion:preview from these tests when LanguageFeature.LowerIntegralRangesToFastLoops is out of preview.
[<TestFixture>]
module Int32 =
    /// [|…|]
    module Array =
        /// [|start..finish|]
        module Range =
            [<Test>]
            let ``Lone RangeInt32 with const args when start > finish lowers to empty array`` () =
                CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize+"; "/langversion:preview"|],
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
                CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize+"; "/langversion:preview"|],
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
                CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize+"; "/langversion:preview"|],
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
                        .locals init (uint32 V_0,
                                 int32[] V_1,
                                 uint32 V_2,
                                 int32 V_3)
                        IL_0000:  ldarg.1
                        IL_0001:  ldarg.0
                        IL_0002:  bge.s      IL_0007
                    
                        IL_0004:  ldc.i4.0
                        IL_0005:  br.s       IL_000e
                    
                        IL_0007:  ldarg.1
                        IL_0008:  conv.i4
                        IL_0009:  ldarg.0
                        IL_000a:  conv.i4
                        IL_000b:  sub
                        IL_000c:  ldc.i4.1
                        IL_000d:  add
                        IL_000e:  stloc.0
                        IL_000f:  ldloc.0
                        IL_0010:  ldc.i4.1
                        IL_0011:  bge.un.s   IL_0019
                    
                        IL_0013:  call       !!0[] [runtime]System.Array::Empty<int32>()
                        IL_0018:  ret
                    
                        IL_0019:  ldloc.0
                        IL_001a:  newarr     [runtime]System.Int32
                        IL_001f:  stloc.1
                        IL_0020:  ldc.i4.0
                        IL_0021:  stloc.2
                        IL_0022:  ldarg.0
                        IL_0023:  stloc.3
                        IL_0024:  br.s       IL_0032
                    
                        IL_0026:  ldloc.1
                        IL_0027:  ldloc.2
                        IL_0028:  ldloc.3
                        IL_0029:  stelem.i4
                        IL_002a:  ldloc.3
                        IL_002b:  ldc.i4.1
                        IL_002c:  add
                        IL_002d:  stloc.3
                        IL_002e:  ldloc.2
                        IL_002f:  ldc.i4.1
                        IL_0030:  add
                        IL_0031:  stloc.2
                        IL_0032:  ldloc.2
                        IL_0033:  ldloc.0
                        IL_0034:  blt.un.s   IL_0026
                    
                        IL_0036:  ldloc.1
                        IL_0037:  ret
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
                CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize+"; "/langversion:preview"|],
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
                                 uint32 V_2,
                                 int32[] V_3,
                                 uint32 V_4,
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
                        IL_0016:  bge.s      IL_001b
                    
                        IL_0018:  ldc.i4.0
                        IL_0019:  br.s       IL_0022
                    
                        IL_001b:  ldloc.1
                        IL_001c:  conv.i4
                        IL_001d:  ldloc.0
                        IL_001e:  conv.i4
                        IL_001f:  sub
                        IL_0020:  ldc.i4.1
                        IL_0021:  add
                        IL_0022:  stloc.2
                        IL_0023:  ldloc.2
                        IL_0024:  ldc.i4.1
                        IL_0025:  bge.un.s   IL_002d
                    
                        IL_0027:  call       !!0[] [runtime]System.Array::Empty<int32>()
                        IL_002c:  ret
                    
                        IL_002d:  ldloc.2
                        IL_002e:  newarr     [runtime]System.Int32
                        IL_0033:  stloc.3
                        IL_0034:  ldc.i4.0
                        IL_0035:  stloc.s    V_4
                        IL_0037:  ldloc.0
                        IL_0038:  stloc.s    V_5
                        IL_003a:  br.s       IL_004e
                    
                        IL_003c:  ldloc.3
                        IL_003d:  ldloc.s    V_4
                        IL_003f:  ldloc.s    V_5
                        IL_0041:  stelem.i4
                        IL_0042:  ldloc.s    V_5
                        IL_0044:  ldc.i4.1
                        IL_0045:  add
                        IL_0046:  stloc.s    V_5
                        IL_0048:  ldloc.s    V_4
                        IL_004a:  ldc.i4.1
                        IL_004b:  add
                        IL_004c:  stloc.s    V_4
                        IL_004e:  ldloc.s    V_4
                        IL_0050:  ldloc.2
                        IL_0051:  blt.un.s   IL_003c
                    
                        IL_0053:  ldloc.3
                        IL_0054:  ret
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
                CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize+"; "/langversion:preview"|],
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
                CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize+"; "/langversion:preview"|],
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
                CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize+"; "/langversion:preview"|],
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
                        .locals init (uint32 V_0,
                                 int32[] V_1,
                                 uint32 V_2,
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
                        IL_0010:  bge.s      IL_0024
                    
                        IL_0012:  ldarg.2
                        IL_0013:  ldarg.0
                        IL_0014:  bge.s      IL_0019
                    
                        IL_0016:  ldc.i4.0
                        IL_0017:  br.s       IL_0037
                    
                        IL_0019:  ldarg.2
                        IL_001a:  conv.i4
                        IL_001b:  ldarg.0
                        IL_001c:  conv.i4
                        IL_001d:  sub
                        IL_001e:  ldarg.1
                        IL_001f:  div.un
                        IL_0020:  ldc.i4.1
                        IL_0021:  add
                        IL_0022:  br.s       IL_0037
                    
                        IL_0024:  ldarg.0
                        IL_0025:  ldarg.2
                        IL_0026:  bge.s      IL_002b
                    
                        IL_0028:  ldc.i4.0
                        IL_0029:  br.s       IL_0037
                    
                        IL_002b:  ldarg.0
                        IL_002c:  conv.i4
                        IL_002d:  ldarg.2
                        IL_002e:  conv.i4
                        IL_002f:  sub
                        IL_0030:  ldarg.1
                        IL_0031:  not
                        IL_0032:  ldc.i4.1
                        IL_0033:  add
                        IL_0034:  div.un
                        IL_0035:  ldc.i4.1
                        IL_0036:  add
                        IL_0037:  stloc.0
                        IL_0038:  ldloc.0
                        IL_0039:  ldc.i4.1
                        IL_003a:  bge.un.s   IL_0042
                    
                        IL_003c:  call       !!0[] [runtime]System.Array::Empty<int32>()
                        IL_0041:  ret
                    
                        IL_0042:  ldloc.0
                        IL_0043:  newarr     [runtime]System.Int32
                        IL_0048:  stloc.1
                        IL_0049:  ldc.i4.0
                        IL_004a:  stloc.2
                        IL_004b:  ldarg.0
                        IL_004c:  stloc.3
                        IL_004d:  br.s       IL_005b
                    
                        IL_004f:  ldloc.1
                        IL_0050:  ldloc.2
                        IL_0051:  ldloc.3
                        IL_0052:  stelem.i4
                        IL_0053:  ldloc.3
                        IL_0054:  ldarg.1
                        IL_0055:  add
                        IL_0056:  stloc.3
                        IL_0057:  ldloc.2
                        IL_0058:  ldc.i4.1
                        IL_0059:  add
                        IL_005a:  stloc.2
                        IL_005b:  ldloc.2
                        IL_005c:  ldloc.0
                        IL_005d:  blt.un.s   IL_004f
                    
                        IL_005f:  ldloc.1
                        IL_0060:  ret
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
                CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize+"; "/langversion:preview"|],
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
                                 uint32 V_3,
                                 int32[] V_4,
                                 uint32 V_5,
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
                        IL_0031:  bge.s      IL_0045
                    
                        IL_0033:  ldloc.2
                        IL_0034:  ldloc.0
                        IL_0035:  bge.s      IL_003a
                    
                        IL_0037:  ldc.i4.0
                        IL_0038:  br.s       IL_0058
                    
                        IL_003a:  ldloc.2
                        IL_003b:  conv.i4
                        IL_003c:  ldloc.0
                        IL_003d:  conv.i4
                        IL_003e:  sub
                        IL_003f:  ldloc.1
                        IL_0040:  div.un
                        IL_0041:  ldc.i4.1
                        IL_0042:  add
                        IL_0043:  br.s       IL_0058
                    
                        IL_0045:  ldloc.0
                        IL_0046:  ldloc.2
                        IL_0047:  bge.s      IL_004c
                    
                        IL_0049:  ldc.i4.0
                        IL_004a:  br.s       IL_0058
                    
                        IL_004c:  ldloc.0
                        IL_004d:  conv.i4
                        IL_004e:  ldloc.2
                        IL_004f:  conv.i4
                        IL_0050:  sub
                        IL_0051:  ldloc.1
                        IL_0052:  not
                        IL_0053:  ldc.i4.1
                        IL_0054:  add
                        IL_0055:  div.un
                        IL_0056:  ldc.i4.1
                        IL_0057:  add
                        IL_0058:  stloc.3
                        IL_0059:  ldloc.3
                        IL_005a:  ldc.i4.1
                        IL_005b:  bge.un.s   IL_0063
                    
                        IL_005d:  call       !!0[] [runtime]System.Array::Empty<int32>()
                        IL_0062:  ret
                    
                        IL_0063:  ldloc.3
                        IL_0064:  newarr     [runtime]System.Int32
                        IL_0069:  stloc.s    V_4
                        IL_006b:  ldc.i4.0
                        IL_006c:  stloc.s    V_5
                        IL_006e:  ldloc.0
                        IL_006f:  stloc.s    V_6
                        IL_0071:  br.s       IL_0086
                    
                        IL_0073:  ldloc.s    V_4
                        IL_0075:  ldloc.s    V_5
                        IL_0077:  ldloc.s    V_6
                        IL_0079:  stelem.i4
                        IL_007a:  ldloc.s    V_6
                        IL_007c:  ldloc.1
                        IL_007d:  add
                        IL_007e:  stloc.s    V_6
                        IL_0080:  ldloc.s    V_5
                        IL_0082:  ldc.i4.1
                        IL_0083:  add
                        IL_0084:  stloc.s    V_5
                        IL_0086:  ldloc.s    V_5
                        IL_0088:  ldloc.3
                        IL_0089:  blt.un.s   IL_0073
                    
                        IL_008b:  ldloc.s    V_4
                        IL_008d:  ret
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
                CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize+"; "/langversion:preview"|],
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
                CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize+"; "/langversion:preview"|],
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
                CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize+"; "/langversion:preview"|],
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
                        .locals init (uint32 V_0,
                                 valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_1,
                                 uint32 V_2,
                                 int32 V_3)
                        IL_0000:  ldarg.1
                        IL_0001:  ldarg.0
                        IL_0002:  bge.s      IL_0007
                    
                        IL_0004:  ldc.i4.0
                        IL_0005:  br.s       IL_000e
                    
                        IL_0007:  ldarg.1
                        IL_0008:  conv.i4
                        IL_0009:  ldarg.0
                        IL_000a:  conv.i4
                        IL_000b:  sub
                        IL_000c:  ldc.i4.1
                        IL_000d:  add
                        IL_000e:  stloc.0
                        IL_000f:  ldc.i4.0
                        IL_0010:  stloc.2
                        IL_0011:  ldarg.0
                        IL_0012:  stloc.3
                        IL_0013:  br.s       IL_0025
                    
                        IL_0015:  ldloca.s   V_1
                        IL_0017:  ldloc.3
                        IL_0018:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
                        IL_001d:  ldloc.3
                        IL_001e:  ldc.i4.1
                        IL_001f:  add
                        IL_0020:  stloc.3
                        IL_0021:  ldloc.2
                        IL_0022:  ldc.i4.1
                        IL_0023:  add
                        IL_0024:  stloc.2
                        IL_0025:  ldloc.2
                        IL_0026:  ldloc.0
                        IL_0027:  blt.un.s   IL_0015
                    
                        IL_0029:  ldloca.s   V_1
                        IL_002b:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
                        IL_0030:  ret
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
                CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize+"; "/langversion:preview"|],
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
                                 uint32 V_2,
                                 valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_3,
                                 uint32 V_4,
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
                        IL_0016:  bge.s      IL_001b
                    
                        IL_0018:  ldc.i4.0
                        IL_0019:  br.s       IL_0022
                    
                        IL_001b:  ldloc.1
                        IL_001c:  conv.i4
                        IL_001d:  ldloc.0
                        IL_001e:  conv.i4
                        IL_001f:  sub
                        IL_0020:  ldc.i4.1
                        IL_0021:  add
                        IL_0022:  stloc.2
                        IL_0023:  ldc.i4.0
                        IL_0024:  stloc.s    V_4
                        IL_0026:  ldloc.0
                        IL_0027:  stloc.s    V_5
                        IL_0029:  br.s       IL_0040
                    
                        IL_002b:  ldloca.s   V_3
                        IL_002d:  ldloc.s    V_5
                        IL_002f:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
                        IL_0034:  ldloc.s    V_5
                        IL_0036:  ldc.i4.1
                        IL_0037:  add
                        IL_0038:  stloc.s    V_5
                        IL_003a:  ldloc.s    V_4
                        IL_003c:  ldc.i4.1
                        IL_003d:  add
                        IL_003e:  stloc.s    V_4
                        IL_0040:  ldloc.s    V_4
                        IL_0042:  ldloc.2
                        IL_0043:  blt.un.s   IL_002b
                    
                        IL_0045:  ldloca.s   V_3
                        IL_0047:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
                        IL_004c:  ret
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
                CompilerAssert.CompileLibraryAndVerifyILWithOptions(["--optimize+"; "/langversion:preview"],
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
                CompilerAssert.CompileLibraryAndVerifyILWithOptions(["--optimize+"; "/langversion:preview"],
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
                CompilerAssert.CompileLibraryAndVerifyILWithOptions(["--optimize+"; "/langversion:preview"],
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
                        .locals init (uint32 V_0,
                                 valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_1,
                                 uint32 V_2,
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
                        IL_0010:  bge.s      IL_0024
                    
                        IL_0012:  ldarg.2
                        IL_0013:  ldarg.0
                        IL_0014:  bge.s      IL_0019
                    
                        IL_0016:  ldc.i4.0
                        IL_0017:  br.s       IL_0037
                    
                        IL_0019:  ldarg.2
                        IL_001a:  conv.i4
                        IL_001b:  ldarg.0
                        IL_001c:  conv.i4
                        IL_001d:  sub
                        IL_001e:  ldarg.1
                        IL_001f:  div.un
                        IL_0020:  ldc.i4.1
                        IL_0021:  add
                        IL_0022:  br.s       IL_0037
                    
                        IL_0024:  ldarg.0
                        IL_0025:  ldarg.2
                        IL_0026:  bge.s      IL_002b
                    
                        IL_0028:  ldc.i4.0
                        IL_0029:  br.s       IL_0037
                    
                        IL_002b:  ldarg.0
                        IL_002c:  conv.i4
                        IL_002d:  ldarg.2
                        IL_002e:  conv.i4
                        IL_002f:  sub
                        IL_0030:  ldarg.1
                        IL_0031:  not
                        IL_0032:  ldc.i4.1
                        IL_0033:  add
                        IL_0034:  div.un
                        IL_0035:  ldc.i4.1
                        IL_0036:  add
                        IL_0037:  stloc.0
                        IL_0038:  ldc.i4.0
                        IL_0039:  stloc.2
                        IL_003a:  ldarg.0
                        IL_003b:  stloc.3
                        IL_003c:  br.s       IL_004e
                    
                        IL_003e:  ldloca.s   V_1
                        IL_0040:  ldloc.3
                        IL_0041:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
                        IL_0046:  ldloc.3
                        IL_0047:  ldarg.1
                        IL_0048:  add
                        IL_0049:  stloc.3
                        IL_004a:  ldloc.2
                        IL_004b:  ldc.i4.1
                        IL_004c:  add
                        IL_004d:  stloc.2
                        IL_004e:  ldloc.2
                        IL_004f:  ldloc.0
                        IL_0050:  blt.un.s   IL_003e
                    
                        IL_0052:  ldloca.s   V_1
                        IL_0054:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
                        IL_0059:  ret
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
                CompilerAssert.CompileLibraryAndVerifyILWithOptions(["--optimize+"; "/langversion:preview"],
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
                                 uint32 V_3,
                                 valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_4,
                                 uint32 V_5,
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
                        IL_0031:  bge.s      IL_0045
                    
                        IL_0033:  ldloc.2
                        IL_0034:  ldloc.0
                        IL_0035:  bge.s      IL_003a
                    
                        IL_0037:  ldc.i4.0
                        IL_0038:  br.s       IL_0058
                    
                        IL_003a:  ldloc.2
                        IL_003b:  conv.i4
                        IL_003c:  ldloc.0
                        IL_003d:  conv.i4
                        IL_003e:  sub
                        IL_003f:  ldloc.1
                        IL_0040:  div.un
                        IL_0041:  ldc.i4.1
                        IL_0042:  add
                        IL_0043:  br.s       IL_0058
                    
                        IL_0045:  ldloc.0
                        IL_0046:  ldloc.2
                        IL_0047:  bge.s      IL_004c
                    
                        IL_0049:  ldc.i4.0
                        IL_004a:  br.s       IL_0058
                    
                        IL_004c:  ldloc.0
                        IL_004d:  conv.i4
                        IL_004e:  ldloc.2
                        IL_004f:  conv.i4
                        IL_0050:  sub
                        IL_0051:  ldloc.1
                        IL_0052:  not
                        IL_0053:  ldc.i4.1
                        IL_0054:  add
                        IL_0055:  div.un
                        IL_0056:  ldc.i4.1
                        IL_0057:  add
                        IL_0058:  stloc.3
                        IL_0059:  ldc.i4.0
                        IL_005a:  stloc.s    V_5
                        IL_005c:  ldloc.0
                        IL_005d:  stloc.s    V_6
                        IL_005f:  br.s       IL_0076
                    
                        IL_0061:  ldloca.s   V_4
                        IL_0063:  ldloc.s    V_6
                        IL_0065:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
                        IL_006a:  ldloc.s    V_6
                        IL_006c:  ldloc.1
                        IL_006d:  add
                        IL_006e:  stloc.s    V_6
                        IL_0070:  ldloc.s    V_5
                        IL_0072:  ldc.i4.1
                        IL_0073:  add
                        IL_0074:  stloc.s    V_5
                        IL_0076:  ldloc.s    V_5
                        IL_0078:  ldloc.3
                        IL_0079:  blt.un.s   IL_0061
                    
                        IL_007b:  ldloca.s   V_4
                        IL_007d:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
                        IL_0082:  ret
                      } 
                    
                    } 
                    
                    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
                           extends [runtime]System.Object
                    {
                    }
                    """
                    ]))
