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
                                 uint64 V_1,
                                 int32 V_2)
                        IL_0000:  ldc.i4     0x101
                        IL_0005:  conv.i8
                        IL_0006:  conv.ovf.i.un
                        IL_0007:  newarr     [runtime]System.Int32
                        IL_000c:  stloc.0
                        IL_000d:  ldc.i4.0
                        IL_000e:  conv.i8
                        IL_000f:  stloc.1
                        IL_0010:  ldc.i4.1
                        IL_0011:  stloc.2
                        IL_0012:  br.s       IL_0022
                    
                        IL_0014:  ldloc.0
                        IL_0015:  ldloc.1
                        IL_0016:  conv.ovf.i.un
                        IL_0017:  ldloc.2
                        IL_0018:  stelem.i4
                        IL_0019:  ldloc.2
                        IL_001a:  ldc.i4.1
                        IL_001b:  add
                        IL_001c:  stloc.2
                        IL_001d:  ldloc.1
                        IL_001e:  ldc.i4.1
                        IL_001f:  conv.i8
                        IL_0020:  add
                        IL_0021:  stloc.1
                        IL_0022:  ldloc.1
                        IL_0023:  ldc.i4     0x101
                        IL_0028:  conv.i8
                        IL_0029:  blt.un.s   IL_0014
                    
                        IL_002b:  ldloc.0
                        IL_002c:  ret
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
                        IL_0013:  conv.i8
                        IL_0014:  bge.un.s   IL_001c
                    
                        IL_0016:  call       !!0[] [runtime]System.Array::Empty<int32>()
                        IL_001b:  ret
                    
                        IL_001c:  ldloc.0
                        IL_001d:  conv.ovf.i.un
                        IL_001e:  newarr     [runtime]System.Int32
                        IL_0023:  stloc.1
                        IL_0024:  ldc.i4.0
                        IL_0025:  conv.i8
                        IL_0026:  stloc.2
                        IL_0027:  ldarg.0
                        IL_0028:  stloc.3
                        IL_0029:  br.s       IL_0039
                    
                        IL_002b:  ldloc.1
                        IL_002c:  ldloc.2
                        IL_002d:  conv.ovf.i.un
                        IL_002e:  ldloc.3
                        IL_002f:  stelem.i4
                        IL_0030:  ldloc.3
                        IL_0031:  ldc.i4.1
                        IL_0032:  add
                        IL_0033:  stloc.3
                        IL_0034:  ldloc.2
                        IL_0035:  ldc.i4.1
                        IL_0036:  conv.i8
                        IL_0037:  add
                        IL_0038:  stloc.2
                        IL_0039:  ldloc.2
                        IL_003a:  ldloc.0
                        IL_003b:  blt.un.s   IL_002b
                    
                        IL_003d:  ldloc.1
                        IL_003e:  ret
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
                        IL_0027:  conv.i8
                        IL_0028:  bge.un.s   IL_0030
                    
                        IL_002a:  call       !!0[] [runtime]System.Array::Empty<int32>()
                        IL_002f:  ret
                    
                        IL_0030:  ldloc.2
                        IL_0031:  conv.ovf.i.un
                        IL_0032:  newarr     [runtime]System.Int32
                        IL_0037:  stloc.3
                        IL_0038:  ldc.i4.0
                        IL_0039:  conv.i8
                        IL_003a:  stloc.s    V_4
                        IL_003c:  ldloc.0
                        IL_003d:  stloc.s    V_5
                        IL_003f:  br.s       IL_0055
                    
                        IL_0041:  ldloc.3
                        IL_0042:  ldloc.s    V_4
                        IL_0044:  conv.ovf.i.un
                        IL_0045:  ldloc.s    V_5
                        IL_0047:  stelem.i4
                        IL_0048:  ldloc.s    V_5
                        IL_004a:  ldc.i4.1
                        IL_004b:  add
                        IL_004c:  stloc.s    V_5
                        IL_004e:  ldloc.s    V_4
                        IL_0050:  ldc.i4.1
                        IL_0051:  conv.i8
                        IL_0052:  add
                        IL_0053:  stloc.s    V_4
                        IL_0055:  ldloc.s    V_4
                        IL_0057:  ldloc.2
                        IL_0058:  blt.un.s   IL_0041
                    
                        IL_005a:  ldloc.3
                        IL_005b:  ret
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
                                 uint64 V_1,
                                 int32 V_2)
                        IL_0000:  ldc.i4     0x81
                        IL_0005:  conv.i8
                        IL_0006:  conv.ovf.i.un
                        IL_0007:  newarr     [runtime]System.Int32
                        IL_000c:  stloc.0
                        IL_000d:  ldc.i4.0
                        IL_000e:  conv.i8
                        IL_000f:  stloc.1
                        IL_0010:  ldc.i4.1
                        IL_0011:  stloc.2
                        IL_0012:  br.s       IL_0022
                    
                        IL_0014:  ldloc.0
                        IL_0015:  ldloc.1
                        IL_0016:  conv.ovf.i.un
                        IL_0017:  ldloc.2
                        IL_0018:  stelem.i4
                        IL_0019:  ldloc.2
                        IL_001a:  ldc.i4.2
                        IL_001b:  add
                        IL_001c:  stloc.2
                        IL_001d:  ldloc.1
                        IL_001e:  ldc.i4.1
                        IL_001f:  conv.i8
                        IL_0020:  add
                        IL_0021:  stloc.1
                        IL_0022:  ldloc.1
                        IL_0023:  ldc.i4     0x81
                        IL_0028:  conv.i8
                        IL_0029:  blt.un.s   IL_0014
                    
                        IL_002b:  ldloc.0
                        IL_002c:  ret
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
                        IL_0018:  br.s       IL_003f
                    
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
                        IL_0025:  br.s       IL_003f
                    
                        IL_0027:  ldarg.0
                        IL_0028:  ldarg.2
                        IL_0029:  bge.s      IL_002f
                    
                        IL_002b:  ldc.i4.0
                        IL_002c:  conv.i8
                        IL_002d:  br.s       IL_003f
                    
                        IL_002f:  ldarg.0
                        IL_0030:  conv.i8
                        IL_0031:  ldarg.2
                        IL_0032:  conv.i8
                        IL_0033:  sub
                        IL_0034:  ldarg.1
                        IL_0035:  not
                        IL_0036:  conv.i8
                        IL_0037:  ldc.i4.1
                        IL_0038:  conv.i8
                        IL_0039:  add
                        IL_003a:  conv.i8
                        IL_003b:  div.un
                        IL_003c:  ldc.i4.1
                        IL_003d:  conv.i8
                        IL_003e:  add
                        IL_003f:  stloc.0
                        IL_0040:  ldloc.0
                        IL_0041:  ldc.i4.1
                        IL_0042:  conv.i8
                        IL_0043:  bge.un.s   IL_004b
                    
                        IL_0045:  call       !!0[] [runtime]System.Array::Empty<int32>()
                        IL_004a:  ret
                    
                        IL_004b:  ldloc.0
                        IL_004c:  conv.ovf.i.un
                        IL_004d:  newarr     [runtime]System.Int32
                        IL_0052:  stloc.1
                        IL_0053:  ldc.i4.0
                        IL_0054:  conv.i8
                        IL_0055:  stloc.2
                        IL_0056:  ldarg.0
                        IL_0057:  stloc.3
                        IL_0058:  br.s       IL_0068
                    
                        IL_005a:  ldloc.1
                        IL_005b:  ldloc.2
                        IL_005c:  conv.ovf.i.un
                        IL_005d:  ldloc.3
                        IL_005e:  stelem.i4
                        IL_005f:  ldloc.3
                        IL_0060:  ldarg.1
                        IL_0061:  add
                        IL_0062:  stloc.3
                        IL_0063:  ldloc.2
                        IL_0064:  ldc.i4.1
                        IL_0065:  conv.i8
                        IL_0066:  add
                        IL_0067:  stloc.2
                        IL_0068:  ldloc.2
                        IL_0069:  ldloc.0
                        IL_006a:  blt.un.s   IL_005a
                    
                        IL_006c:  ldloc.1
                        IL_006d:  ret
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
                        IL_0039:  br.s       IL_0060
                    
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
                        IL_0046:  br.s       IL_0060
                    
                        IL_0048:  ldloc.0
                        IL_0049:  ldloc.2
                        IL_004a:  bge.s      IL_0050
                    
                        IL_004c:  ldc.i4.0
                        IL_004d:  conv.i8
                        IL_004e:  br.s       IL_0060
                    
                        IL_0050:  ldloc.0
                        IL_0051:  conv.i8
                        IL_0052:  ldloc.2
                        IL_0053:  conv.i8
                        IL_0054:  sub
                        IL_0055:  ldloc.1
                        IL_0056:  not
                        IL_0057:  conv.i8
                        IL_0058:  ldc.i4.1
                        IL_0059:  conv.i8
                        IL_005a:  add
                        IL_005b:  conv.i8
                        IL_005c:  div.un
                        IL_005d:  ldc.i4.1
                        IL_005e:  conv.i8
                        IL_005f:  add
                        IL_0060:  stloc.3
                        IL_0061:  ldloc.3
                        IL_0062:  ldc.i4.1
                        IL_0063:  conv.i8
                        IL_0064:  bge.un.s   IL_006c
                    
                        IL_0066:  call       !!0[] [runtime]System.Array::Empty<int32>()
                        IL_006b:  ret
                    
                        IL_006c:  ldloc.3
                        IL_006d:  conv.ovf.i.un
                        IL_006e:  newarr     [runtime]System.Int32
                        IL_0073:  stloc.s    V_4
                        IL_0075:  ldc.i4.0
                        IL_0076:  conv.i8
                        IL_0077:  stloc.s    V_5
                        IL_0079:  ldloc.0
                        IL_007a:  stloc.s    V_6
                        IL_007c:  br.s       IL_0093
                    
                        IL_007e:  ldloc.s    V_4
                        IL_0080:  ldloc.s    V_5
                        IL_0082:  conv.ovf.i.un
                        IL_0083:  ldloc.s    V_6
                        IL_0085:  stelem.i4
                        IL_0086:  ldloc.s    V_6
                        IL_0088:  ldloc.1
                        IL_0089:  add
                        IL_008a:  stloc.s    V_6
                        IL_008c:  ldloc.s    V_5
                        IL_008e:  ldc.i4.1
                        IL_008f:  conv.i8
                        IL_0090:  add
                        IL_0091:  stloc.s    V_5
                        IL_0093:  ldloc.s    V_5
                        IL_0095:  ldloc.3
                        IL_0096:  blt.un.s   IL_007e
                    
                        IL_0098:  ldloc.s    V_4
                        IL_009a:  ret
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
                      .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> test() cil managed
                      {
                        
                        .maxstack  4
                        .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
                                 uint64 V_1,
                                 int32 V_2)
                        IL_0000:  ldc.i4.0
                        IL_0001:  conv.i8
                        IL_0002:  stloc.1
                        IL_0003:  ldc.i4.1
                        IL_0004:  stloc.2
                        IL_0005:  br.s       IL_0018
                    
                        IL_0007:  ldloca.s   V_0
                        IL_0009:  ldloc.2
                        IL_000a:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
                        IL_000f:  ldloc.2
                        IL_0010:  ldc.i4.1
                        IL_0011:  add
                        IL_0012:  stloc.2
                        IL_0013:  ldloc.1
                        IL_0014:  ldc.i4.1
                        IL_0015:  conv.i8
                        IL_0016:  add
                        IL_0017:  stloc.1
                        IL_0018:  ldloc.1
                        IL_0019:  ldc.i4.s   101
                        IL_001b:  conv.i8
                        IL_001c:  blt.un.s   IL_0007
                    
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
                      .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> test() cil managed
                      {
                        
                        .maxstack  4
                        .locals init (valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32> V_0,
                                 uint64 V_1,
                                 int32 V_2)
                        IL_0000:  ldc.i4.0
                        IL_0001:  conv.i8
                        IL_0002:  stloc.1
                        IL_0003:  ldc.i4.1
                        IL_0004:  stloc.2
                        IL_0005:  br.s       IL_0018
                    
                        IL_0007:  ldloca.s   V_0
                        IL_0009:  ldloc.2
                        IL_000a:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
                        IL_000f:  ldloc.2
                        IL_0010:  ldc.i4.2
                        IL_0011:  add
                        IL_0012:  stloc.2
                        IL_0013:  ldloc.1
                        IL_0014:  ldc.i4.1
                        IL_0015:  conv.i8
                        IL_0016:  add
                        IL_0017:  stloc.1
                        IL_0018:  ldloc.1
                        IL_0019:  ldc.i4     0x81
                        IL_001e:  conv.i8
                        IL_001f:  blt.un.s   IL_0007
                    
                        IL_0021:  ldloca.s   V_0
                        IL_0023:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
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
                        IL_0018:  br.s       IL_003f
                    
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
                        IL_0025:  br.s       IL_003f
                    
                        IL_0027:  ldarg.0
                        IL_0028:  ldarg.2
                        IL_0029:  bge.s      IL_002f
                    
                        IL_002b:  ldc.i4.0
                        IL_002c:  conv.i8
                        IL_002d:  br.s       IL_003f
                    
                        IL_002f:  ldarg.0
                        IL_0030:  conv.i8
                        IL_0031:  ldarg.2
                        IL_0032:  conv.i8
                        IL_0033:  sub
                        IL_0034:  ldarg.1
                        IL_0035:  not
                        IL_0036:  conv.i8
                        IL_0037:  ldc.i4.1
                        IL_0038:  conv.i8
                        IL_0039:  add
                        IL_003a:  conv.i8
                        IL_003b:  div.un
                        IL_003c:  ldc.i4.1
                        IL_003d:  conv.i8
                        IL_003e:  add
                        IL_003f:  stloc.0
                        IL_0040:  ldc.i4.0
                        IL_0041:  conv.i8
                        IL_0042:  stloc.2
                        IL_0043:  ldarg.0
                        IL_0044:  stloc.3
                        IL_0045:  br.s       IL_0058
                    
                        IL_0047:  ldloca.s   V_1
                        IL_0049:  ldloc.3
                        IL_004a:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
                        IL_004f:  ldloc.3
                        IL_0050:  ldarg.1
                        IL_0051:  add
                        IL_0052:  stloc.3
                        IL_0053:  ldloc.2
                        IL_0054:  ldc.i4.1
                        IL_0055:  conv.i8
                        IL_0056:  add
                        IL_0057:  stloc.2
                        IL_0058:  ldloc.2
                        IL_0059:  ldloc.0
                        IL_005a:  blt.un.s   IL_0047
                    
                        IL_005c:  ldloca.s   V_1
                        IL_005e:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
                        IL_0063:  ret
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
                        IL_0039:  br.s       IL_0060
                    
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
                        IL_0046:  br.s       IL_0060
                    
                        IL_0048:  ldloc.0
                        IL_0049:  ldloc.2
                        IL_004a:  bge.s      IL_0050
                    
                        IL_004c:  ldc.i4.0
                        IL_004d:  conv.i8
                        IL_004e:  br.s       IL_0060
                    
                        IL_0050:  ldloc.0
                        IL_0051:  conv.i8
                        IL_0052:  ldloc.2
                        IL_0053:  conv.i8
                        IL_0054:  sub
                        IL_0055:  ldloc.1
                        IL_0056:  not
                        IL_0057:  conv.i8
                        IL_0058:  ldc.i4.1
                        IL_0059:  conv.i8
                        IL_005a:  add
                        IL_005b:  conv.i8
                        IL_005c:  div.un
                        IL_005d:  ldc.i4.1
                        IL_005e:  conv.i8
                        IL_005f:  add
                        IL_0060:  stloc.3
                        IL_0061:  ldc.i4.0
                        IL_0062:  conv.i8
                        IL_0063:  stloc.s    V_5
                        IL_0065:  ldloc.0
                        IL_0066:  stloc.s    V_6
                        IL_0068:  br.s       IL_0080
                    
                        IL_006a:  ldloca.s   V_4
                        IL_006c:  ldloc.s    V_6
                        IL_006e:  call       instance void valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Add(!0)
                        IL_0073:  ldloc.s    V_6
                        IL_0075:  ldloc.1
                        IL_0076:  add
                        IL_0077:  stloc.s    V_6
                        IL_0079:  ldloc.s    V_5
                        IL_007b:  ldc.i4.1
                        IL_007c:  conv.i8
                        IL_007d:  add
                        IL_007e:  stloc.s    V_5
                        IL_0080:  ldloc.s    V_5
                        IL_0082:  ldloc.3
                        IL_0083:  blt.un.s   IL_006a
                    
                        IL_0085:  ldloca.s   V_4
                        IL_0087:  call       instance class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.CompilerServices.ListCollector`1<int32>::Close()
                        IL_008c:  ret
                      } 
                    
                    } 
                    
                    .class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
                           extends [runtime]System.Object
                    {
                    }
                    """
                    ]))
