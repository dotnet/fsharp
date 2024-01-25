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
        let ``Lone RangeInt32 with const args ≤ 1024 bytes lowers to blob`` () =
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
                  .field static assembly valuetype '<PrivateImplementationDetails$assembly>'/T3169_40Bytes@ field3170@ at I_000028A7
                  .custom instance void [runtime]System.Diagnostics.DebuggerBrowsableAttribute::.ctor(valuetype [runtime]System.Diagnostics.DebuggerBrowsableState) = ( 01 00 00 00 00 00 00 00 ) 
                  .method public static int32[]  test() cil managed
                  {
                    
                    .maxstack  8
                    IL_0000:  ldc.i4.s   10
                    IL_0002:  newarr     [runtime]System.Int32
                    IL_0007:  dup
                    IL_0008:  ldtoken    field valuetype '<PrivateImplementationDetails$assembly>'/T3169_40Bytes@ Test::field3170@
                    IL_000d:  call       void [runtime]System.Runtime.CompilerServices.RuntimeHelpers::InitializeArray(class [runtime]System.Array,
                                                                                                                              valuetype [runtime]System.RuntimeFieldHandle)
                    IL_0012:  ret
                  } 
                
                } 
                
                .class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
                       extends [runtime]System.Object
                {
                } 
                
                .class private abstract auto ansi sealed beforefieldinit '<PrivateImplementationDetails$assembly>'
                       extends [runtime]System.Object
                {
                  .class explicit ansi sealed nested assembly beforefieldinit T3169_40Bytes@
                         extends [runtime]System.ValueType
                  {
                    .pack 0
                    .size 40
                  } 
                
                } 
                
                
                
                
                .data cil I_000028A7 = bytearray (
                                 01 00 00 00 02 00 00 00 03 00 00 00 04 00 00 00
                                 05 00 00 00 06 00 00 00 07 00 00 00 08 00 00 00
                                 09 00 00 00 0A 00 00 00) 
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
                    IL_0000:  ldc.i4.1
                    IL_0001:  ldc.i4     0x101
                    IL_0006:  bgt.s      IL_001c
                
                    IL_0008:  ldc.i4     0x101
                    IL_000d:  ldc.i4.1
                    IL_000e:  sub
                    IL_000f:  ldc.i4.1
                    IL_0010:  add
                    IL_0011:  ldsfld     class Test/test@1 Test/test@1::@_instance
                    IL_0016:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Initialize<int32>(int32,
                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!0>)
                    IL_001b:  ret
                
                    IL_001c:  call       !!0[] [runtime]System.Array::Empty<int32>()
                    IL_0021:  ret
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
                    IL_0000:  ldarg.0
                    IL_0001:  ldarg.1
                    IL_0002:  bgt.s      IL_0015
                
                    IL_0004:  ldarg.1
                    IL_0005:  ldarg.0
                    IL_0006:  sub
                    IL_0007:  ldc.i4.1
                    IL_0008:  add
                    IL_0009:  ldarg.0
                    IL_000a:  newobj     instance void Test/test@1::.ctor(int32)
                    IL_000f:  call       !!0[] [FSharp.Core]Microsoft.FSharp.Collections.ArrayModule::Initialize<int32>(int32,
                                                                                                                        class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!0>)
                    IL_0014:  ret
                
                    IL_0015:  call       !!0[] [runtime]System.Array::Empty<int32>()
                    IL_001a:  ret
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
        let ``Lone small RangeInt32 with const args ≤ 100 lowers to conses`` () =
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
                  .method public static class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32> 
                          test() cil managed
                  {
                    
                    .maxstack  13
                    IL_0000:  ldc.i4.1
                    IL_0001:  ldc.i4.2
                    IL_0002:  ldc.i4.3
                    IL_0003:  ldc.i4.4
                    IL_0004:  ldc.i4.5
                    IL_0005:  ldc.i4.6
                    IL_0006:  ldc.i4.7
                    IL_0007:  ldc.i4.8
                    IL_0008:  ldc.i4.s   9
                    IL_000a:  ldc.i4.s   10
                    IL_000c:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
                    IL_0011:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
                    IL_0016:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
                    IL_001b:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
                    IL_0020:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
                    IL_0025:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
                    IL_002a:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
                    IL_002f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
                    IL_0034:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
                    IL_0039:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
                    IL_003e:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::Cons(!0,
                                                                                                                                                                                    class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0>)
                    IL_0043:  ret
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
                    IL_0000:  ldc.i4.1
                    IL_0001:  ldc.i4.s   101
                    IL_0003:  bgt.s      IL_0016
                
                    IL_0005:  ldc.i4.s   101
                    IL_0007:  ldc.i4.1
                    IL_0008:  sub
                    IL_0009:  ldc.i4.1
                    IL_000a:  add
                    IL_000b:  ldsfld     class Test/test@1 Test/test@1::@_instance
                    IL_0010:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Initialize<int32>(int32,
                                                                                                                                                                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!0>)
                    IL_0015:  ret
                
                    IL_0016:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
                    IL_001b:  ret
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
                    IL_0000:  ldarg.0
                    IL_0001:  ldarg.1
                    IL_0002:  bgt.s      IL_0015
                
                    IL_0004:  ldarg.1
                    IL_0005:  ldarg.0
                    IL_0006:  sub
                    IL_0007:  ldc.i4.1
                    IL_0008:  add
                    IL_0009:  ldarg.0
                    IL_000a:  newobj     instance void Test/test@1::.ctor(int32)
                    IL_000f:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!!0> [FSharp.Core]Microsoft.FSharp.Collections.ListModule::Initialize<int32>(int32,
                                                                                                                                                                                   class [FSharp.Core]Microsoft.FSharp.Core.FSharpFunc`2<int32,!!0>)
                    IL_0014:  ret
                
                    IL_0015:  call       class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<!0> class [FSharp.Core]Microsoft.FSharp.Collections.FSharpList`1<int32>::get_Empty()
                    IL_001a:  ret
                  } 
                
                } 
                
                .class private abstract auto ansi sealed '<StartupCode$assembly>'.$Test
                       extends [runtime]System.Object
                {
                }
                """
                ]))

