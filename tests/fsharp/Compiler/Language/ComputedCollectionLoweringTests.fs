// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test

[<TestFixture>]
module ComputedCollectionLoweringTests =
    module Array =
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
        let ``Lone RangeInt32 with const args ≤ 1024 bytes lowers to call to init`` () =
            CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize+"|],
                """
                module Test

                let test () = [|1..10|]
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
                  .class auto ansi serializable sealed nested assembly beforefieldinit test@1
                         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
                  {
                    .field static assembly initonly class Test/test@1 @_instance
                    .method assembly specialname rtspecialname 
                            instance void  .ctor() cil managed
                    {
                      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
                      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
                      
                      .maxstack  8
                      IL_0000:  ldarg.0
                      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
                      IL_0006:  ret
                    } 
                
                    .method public strict virtual instance int32 
                            Invoke(int32 i) cil managed
                    {
                      
                      .maxstack  8
                      IL_0000:  ldc.i4.1
                      IL_0001:  ldarg.1
                      IL_0002:  add
                      IL_0003:  ret
                    } 
                
                    .method private specialname rtspecialname static 
                            void  .cctor() cil managed
                    {
                      
                      .maxstack  10
                      IL_0000:  newobj     instance void Test/test@1::.ctor()
                      IL_0005:  stsfld     class Test/test@1 Test/test@1::@_instance
                      IL_000a:  ret
                    } 
                
                  } 
                
                  .method public static int32[]  test() cil managed
                  {
                    
                    .maxstack  8
                    IL_0000:  ldc.i4.s   10
                    IL_0002:  ldc.i4.1
                    IL_0003:  sub
                    IL_0004:  ldc.i4.1
                    IL_0005:  add
                    IL_0006:  ldc.i4.s   10
                    IL_0008:  ldc.i4.1
                    IL_0009:  sub
                    IL_000a:  ldc.i4.1
                    IL_000b:  add
                    IL_000c:  ldc.i4.s   10
                    IL_000e:  ldc.i4.1
                    IL_000f:  sub
                    IL_0010:  ldc.i4.1
                    IL_0011:  add
                    IL_0012:  ldc.i4.0
                    IL_0013:  clt
                    IL_0015:  neg
                    IL_0016:  and
                    IL_0017:  xor
                    IL_0018:  ldsfld     class Test/test@1 Test/test@1::@_instance
                    IL_001d:  tail.
                    IL_001f:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Initialize<int32>(int32,
                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!0>)
                    IL_0024:  ret
                  } 
                
                } 
                
                .class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
                       extends [runtime]System.Object
                {
                }
                """
                ]))

        [<Test>]
        let ``Lone RangeInt32 with const args > 1024 bytes lowers to call to init`` () =
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
                  .class auto ansi serializable sealed nested assembly beforefieldinit test@1
                         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
                  {
                    .field static assembly initonly class Test/test@1 @_instance
                    .method assembly specialname rtspecialname 
                            instance void  .ctor() cil managed
                    {
                      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
                      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
                      
                      .maxstack  8
                      IL_0000:  ldarg.0
                      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
                      IL_0006:  ret
                    } 
                
                    .method public strict virtual instance int32 
                            Invoke(int32 i) cil managed
                    {
                      
                      .maxstack  8
                      IL_0000:  ldc.i4.1
                      IL_0001:  ldarg.1
                      IL_0002:  add
                      IL_0003:  ret
                    } 
                
                    .method private specialname rtspecialname static 
                            void  .cctor() cil managed
                    {
                      
                      .maxstack  10
                      IL_0000:  newobj     instance void Test/test@1::.ctor()
                      IL_0005:  stsfld     class Test/test@1 Test/test@1::@_instance
                      IL_000a:  ret
                    } 
                
                  } 
                
                  .method public static int32[]  test() cil managed
                  {
                    
                    .maxstack  8
                    IL_0000:  ldc.i4     0x101
                    IL_0005:  ldc.i4.1
                    IL_0006:  sub
                    IL_0007:  ldc.i4.1
                    IL_0008:  add
                    IL_0009:  ldc.i4     0x101
                    IL_000e:  ldc.i4.1
                    IL_000f:  sub
                    IL_0010:  ldc.i4.1
                    IL_0011:  add
                    IL_0012:  ldc.i4     0x101
                    IL_0017:  ldc.i4.1
                    IL_0018:  sub
                    IL_0019:  ldc.i4.1
                    IL_001a:  add
                    IL_001b:  ldc.i4.0
                    IL_001c:  clt
                    IL_001e:  neg
                    IL_001f:  and
                    IL_0020:  xor
                    IL_0021:  ldsfld     class Test/test@1 Test/test@1::@_instance
                    IL_0026:  tail.
                    IL_0028:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Initialize<int32>(int32,
                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!0>)
                    IL_002d:  ret
                  } 
                
                } 
                
                .class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
                       extends [runtime]System.Object
                {
                }
                """
                ]))

        [<Test>]
        let ``Lone RangeInt32 with dynamic args lowers to call to init`` () =
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
                  .class auto ansi serializable sealed nested assembly beforefieldinit test@1
                         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
                  {
                    .field public int32 start
                    .method assembly specialname rtspecialname 
                            instance void  .ctor(int32 start) cil managed
                    {
                      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
                      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
                      
                      .maxstack  8
                      IL_0000:  ldarg.0
                      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
                      IL_0006:  ldarg.0
                      IL_0007:  ldarg.1
                      IL_0008:  stfld      int32 Test/test@1::start
                      IL_000d:  ret
                    } 
                
                    .method public strict virtual instance int32 
                            Invoke(int32 i) cil managed
                    {
                      
                      .maxstack  8
                      IL_0000:  ldarg.0
                      IL_0001:  ldfld      int32 Test/test@1::start
                      IL_0006:  ldarg.1
                      IL_0007:  add
                      IL_0008:  ret
                    } 
                
                  } 
                
                  .method public static int32[]  test(int32 start,
                                                      int32 finish) cil managed
                  {
                    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
                    
                    .maxstack  8
                    IL_0000:  ldarg.1
                    IL_0001:  ldarg.0
                    IL_0002:  sub
                    IL_0003:  ldc.i4.1
                    IL_0004:  add
                    IL_0005:  ldarg.1
                    IL_0006:  ldarg.0
                    IL_0007:  sub
                    IL_0008:  ldc.i4.1
                    IL_0009:  add
                    IL_000a:  ldarg.1
                    IL_000b:  ldarg.0
                    IL_000c:  sub
                    IL_000d:  ldc.i4.1
                    IL_000e:  add
                    IL_000f:  ldc.i4.0
                    IL_0010:  clt
                    IL_0012:  neg
                    IL_0013:  and
                    IL_0014:  xor
                    IL_0015:  ldarg.0
                    IL_0016:  newobj     instance void Test/test@1::.ctor(int32)
                    IL_001b:  tail.
                    IL_001d:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Initialize<int32>(int32,
                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!0>)
                    IL_0022:  ret
                  } 
                
                } 
                
                .class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
                       extends [runtime]System.Object
                {
                }
                """
                ]))

    module List =
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
        let ``Lone small RangeInt32 with const args ≤ 100 lowers to call to init`` () =
            CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize+"|],
                """
                module Test

                let test () = [1..10]
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
                  .class auto ansi serializable sealed nested assembly beforefieldinit test@1
                         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
                  {
                    .field static assembly initonly class Test/test@1 @_instance
                    .method assembly specialname rtspecialname 
                            instance void  .ctor() cil managed
                    {
                      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
                      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
                      
                      .maxstack  8
                      IL_0000:  ldarg.0
                      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
                      IL_0006:  ret
                    } 
                
                    .method public strict virtual instance int32 
                            Invoke(int32 i) cil managed
                    {
                      
                      .maxstack  8
                      IL_0000:  ldc.i4.1
                      IL_0001:  ldarg.1
                      IL_0002:  add
                      IL_0003:  ret
                    } 
                
                    .method private specialname rtspecialname static 
                            void  .cctor() cil managed
                    {
                      
                      .maxstack  10
                      IL_0000:  newobj     instance void Test/test@1::.ctor()
                      IL_0005:  stsfld     class Test/test@1 Test/test@1::@_instance
                      IL_000a:  ret
                    } 
                
                  } 
                
                  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
                          test() cil managed
                  {
                    
                    .maxstack  8
                    IL_0000:  ldc.i4.s   10
                    IL_0002:  ldc.i4.1
                    IL_0003:  sub
                    IL_0004:  ldc.i4.1
                    IL_0005:  add
                    IL_0006:  ldc.i4.s   10
                    IL_0008:  ldc.i4.1
                    IL_0009:  sub
                    IL_000a:  ldc.i4.1
                    IL_000b:  add
                    IL_000c:  ldc.i4.s   10
                    IL_000e:  ldc.i4.1
                    IL_000f:  sub
                    IL_0010:  ldc.i4.1
                    IL_0011:  add
                    IL_0012:  ldc.i4.0
                    IL_0013:  clt
                    IL_0015:  neg
                    IL_0016:  and
                    IL_0017:  xor
                    IL_0018:  ldsfld     class Test/test@1 Test/test@1::@_instance
                    IL_001d:  tail.
                    IL_001f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Initialize<int32>(int32,
                                                                                                                                                                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!0>)
                    IL_0024:  ret
                  } 
                
                } 
                
                .class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
                       extends [runtime]System.Object
                {
                }
                """
                ]))

        [<Test>]
        let ``Lone RangeInt32 with const args > 100 lowers to call to init`` () =
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
                  .class auto ansi serializable sealed nested assembly beforefieldinit test@1
                         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
                  {
                    .field static assembly initonly class Test/test@1 @_instance
                    .method assembly specialname rtspecialname 
                            instance void  .ctor() cil managed
                    {
                      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
                      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
                      
                      .maxstack  8
                      IL_0000:  ldarg.0
                      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
                      IL_0006:  ret
                    } 
                
                    .method public strict virtual instance int32 
                            Invoke(int32 i) cil managed
                    {
                      
                      .maxstack  8
                      IL_0000:  ldc.i4.1
                      IL_0001:  ldarg.1
                      IL_0002:  add
                      IL_0003:  ret
                    } 
                
                    .method private specialname rtspecialname static 
                            void  .cctor() cil managed
                    {
                      
                      .maxstack  10
                      IL_0000:  newobj     instance void Test/test@1::.ctor()
                      IL_0005:  stsfld     class Test/test@1 Test/test@1::@_instance
                      IL_000a:  ret
                    } 
                
                  } 
                
                  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
                          test() cil managed
                  {
                    
                    .maxstack  8
                    IL_0000:  ldc.i4.s   101
                    IL_0002:  ldc.i4.1
                    IL_0003:  sub
                    IL_0004:  ldc.i4.1
                    IL_0005:  add
                    IL_0006:  ldc.i4.s   101
                    IL_0008:  ldc.i4.1
                    IL_0009:  sub
                    IL_000a:  ldc.i4.1
                    IL_000b:  add
                    IL_000c:  ldc.i4.s   101
                    IL_000e:  ldc.i4.1
                    IL_000f:  sub
                    IL_0010:  ldc.i4.1
                    IL_0011:  add
                    IL_0012:  ldc.i4.0
                    IL_0013:  clt
                    IL_0015:  neg
                    IL_0016:  and
                    IL_0017:  xor
                    IL_0018:  ldsfld     class Test/test@1 Test/test@1::@_instance
                    IL_001d:  tail.
                    IL_001f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Initialize<int32>(int32,
                                                                                                                                                                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!0>)
                    IL_0024:  ret
                  } 
                
                } 
                
                .class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
                       extends [runtime]System.Object
                {
                }
                """
                ]))

        [<Test>]
        let ``Lone RangeInt32 with dynamic args lowers to call to init``() =
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
                  .class auto ansi serializable sealed nested assembly beforefieldinit test@1
                         extends class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>
                  {
                    .field public int32 start
                    .method assembly specialname rtspecialname 
                            instance void  .ctor(int32 start) cil managed
                    {
                      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
                      .custom instance void [runtime]System.Diagnostics.DebuggerNonUserCodeAttribute::.ctor() = ( 01 00 00 00 ) 
                      
                      .maxstack  8
                      IL_0000:  ldarg.0
                      IL_0001:  call       instance void class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,int32>::.ctor()
                      IL_0006:  ldarg.0
                      IL_0007:  ldarg.1
                      IL_0008:  stfld      int32 Test/test@1::start
                      IL_000d:  ret
                    } 
                
                    .method public strict virtual instance int32 
                            Invoke(int32 i) cil managed
                    {
                      
                      .maxstack  8
                      IL_0000:  ldarg.0
                      IL_0001:  ldfld      int32 Test/test@1::start
                      IL_0006:  ldarg.1
                      IL_0007:  add
                      IL_0008:  ret
                    } 
                
                  } 
                
                  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
                          test(int32 start,
                               int32 finish) cil managed
                  {
                    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationArgumentCountsAttribute::.ctor(int32[]) = ( 01 00 02 00 00 00 01 00 00 00 01 00 00 00 00 00 ) 
                    
                    .maxstack  8
                    IL_0000:  ldarg.1
                    IL_0001:  ldarg.0
                    IL_0002:  sub
                    IL_0003:  ldc.i4.1
                    IL_0004:  add
                    IL_0005:  ldarg.1
                    IL_0006:  ldarg.0
                    IL_0007:  sub
                    IL_0008:  ldc.i4.1
                    IL_0009:  add
                    IL_000a:  ldarg.1
                    IL_000b:  ldarg.0
                    IL_000c:  sub
                    IL_000d:  ldc.i4.1
                    IL_000e:  add
                    IL_000f:  ldc.i4.0
                    IL_0010:  clt
                    IL_0012:  neg
                    IL_0013:  and
                    IL_0014:  xor
                    IL_0015:  ldarg.0
                    IL_0016:  newobj     instance void Test/test@1::.ctor(int32)
                    IL_001b:  tail.
                    IL_001d:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Initialize<int32>(int32,
                                                                                                                                                                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!0>)
                    IL_0022:  ret
                  } 
                
                } 
                
                .class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
                       extends [runtime]System.Object
                {
                }
                """
                ]))
