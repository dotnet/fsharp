// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL

open Xunit
open FSharp.Test.Compiler

module GenericComparisonCrossAssembly =

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``fslib``(realsig) =
        FSharpWithFileName "Program.fs"
            """
ValueSome (1, 2) = ValueSome (2, 3) |> ignore"""
        |> withRealInternalSignature realsig
        |> withOptimize
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL [ """
    IL_002c:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
    IL_0031:  call       instance bool valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<class [runtime]System.Tuple`2<int32,int32>>::Equals(valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<!0>,
""" ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Another Assembly - record with private fields`` (realsig) =
        let library =
            FSharpWithFileName "Module1.fs"
                """
module Module1
    
type Value =
    private { value: uint32 }

    static member Zero = { value = 0u }
    static member Create(value: int) = { value = uint value } """
            |> withRealInternalSignature realsig
            |> withOptimize
            |> asLibrary
            |> withName "module1"

        let mainModule = 
            FSharpWithFileName "Program.fs"
                $"""
open Module1
Value.Zero = Value.Create 0 |> ignore"""

        mainModule
        |> withRealInternalSignature realsig
        |> withReferences [ library ]
        |> compileExeAndRun
        |> shouldSucceed

    [<InlineData(false, "private",  "assembly")>]   // Legacy, private WrapType, assembly visibility in IL
    [<InlineData(false, "internal", "assembly")>]   // RealSig, internal WrapType, assembly visibility in IL
    [<InlineData(false, "public",   "public")>]     // Legacy, public WrapType, public visibility in IL
    [<InlineData(true,  "private",  "public")>]     // RealSig, private WrapType, public visibility in IL
    [<InlineData(true,  "internal", "public")>]     // RealSig, internal WrapType, public visibility in IL
    [<InlineData(true,  "public",   "public")>]     // RealSig, public WrapType, public visibility in IL
    [<Theory>]
    let ``Generated typed Equals`` (realsig, typeScope, targetVisibility) =
        let library = 
            FSharpWithFileName "Program.fs"
                $"""
module Module1 =

    [<Struct>]
    type {typeScope} Struct(v: int, u: int) =
        member _.V = v
        member _.U = u"""

        library
        |> asExe
        |> withNoWarn 988
        |> withRealInternalSignature realsig
        |> withOptimize
        |> compile
        |> shouldSucceed
        |> verifyIL [
            $"""
      .method {targetVisibility} hidebysig instance bool Equals(valuetype Program/Module1/Struct obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {{
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldfld      int32 Program/Module1/Struct::v
        IL_0006:  ldarga.s   obj
        IL_0008:  ldfld      int32 Program/Module1/Struct::v
        IL_000d:  bne.un.s   IL_001f

        IL_000f:  ldarg.0
        IL_0010:  ldfld      int32 Program/Module1/Struct::u
        IL_0015:  ldarga.s   obj
        IL_0017:  ldfld      int32 Program/Module1/Struct::u
        IL_001c:  ceq
        IL_001e:  ret

        IL_001f:  ldc.i4.0
        IL_0020:  ret
      }} """ ]


    [<InlineData(false, "private")>]   // Legacy, private WrapType
    [<InlineData(false, "internal")>]   // RealSig, internal WrapType
    [<InlineData(false, "public")>]     // Legacy, public WrapType
    [<InlineData(true,  "private")>]     // RealSig, private WrapType
    [<InlineData(true,  "internal")>]     // RealSig, internal WrapType
    [<InlineData(true,  "public")>]     // RealSig, public WrapType
    [<Theory>]
    let ``Record with various scoped fields`` (realsig, fieldScope) =

        let mainModule = 
            FSharpWithFileName "Program.fs"
                $$"""
module Module1 =
    type Value =
        {{fieldScope}} { value: uint32 }

        static member Zero = { value = 0u }
        static member Create(value: int) = { value = uint value }

    Value.Zero = Value.Create 0 |> ignore
    printfn "Hello, World" """

        mainModule
        |> withRealInternalSignature realsig
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL [
            """
      .method public hidebysig instance bool Equals(class Program/Module1/Value obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0017

        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_0015

        IL_0006:  ldarg.0
        IL_0007:  ldfld      uint32 Program/Module1/Value::value@
        IL_000c:  ldarg.1
        IL_000d:  ldfld      uint32 Program/Module1/Value::value@
        IL_0012:  ceq
        IL_0014:  ret

        IL_0015:  ldc.i4.0
        IL_0016:  ret

        IL_0017:  ldarg.1
        IL_0018:  ldnull
        IL_0019:  cgt.un
        IL_001b:  ldc.i4.0
        IL_001c:  ceq
        IL_001e:  ret
      } """ ]


    [<InlineData(false, "public", "private",  "assembly")>]   // public module - Legacy, private WrapType, private visibility in IL
    [<InlineData(false, "public", "internal", "assembly")>]   // public module - RealSig, internal WrapType, assembly visibility in IL
    [<InlineData(false, "public", "public",   "public")>]     // public module - Legacy, public WrapType, public visibility in IL
    [<InlineData(true,  "public", "private",  "public")>]     // public module - RealSig, private WrapType, public visibility in IL
    [<InlineData(true,  "public", "internal", "public")>]     // public module - RealSig, internal WrapType, public visibility in IL
    [<InlineData(true,  "public", "public",   "public")>]     // public module - RealSig, public WrapType, public visibility in IL
    [<InlineData(false, "private", "private",  "assembly")>]  // private module - Legacy, private WrapType, private visibility in IL
    [<InlineData(false, "private", "internal", "assembly")>]  // private module - RealSig, internal WrapType, assembly visibility in IL
    [<InlineData(false, "private", "public",   "assembly")>]  // private module - Legacy, public WrapType, assembly visibility in IL
    [<InlineData(true,  "private", "private",  "public")>]    // private module - RealSig, private WrapType, public visibility in IL
    [<InlineData(true,  "private", "internal", "public")>]    // private module - RealSig, internal WrapType, public visibility in IL
    [<InlineData(true,  "private", "public",   "public")>]    // private module - RealSig, public WrapType, public visibility in IL
    [<Theory>]
    let ``scoped main and scoped type Equals`` (realsig, moduleScope, argScope, targetVisibility) =
        let mainModule = 
            FSharpWithFileName "Program.fs"
                $"""
module {moduleScope} IPartialEqualityComparer =
    open System.Collections.Generic

    [<StructuralEquality; NoComparison>]
    type {argScope} WrapType<'T> = Wrap of 'T
"""
        mainModule
        |> asExe
        |> withNoWarn 988
        |> withRealInternalSignature realsig
        |> withOptimize
        |> compile
        |> shouldSucceed
        |> verifyIL [
            $"""
      .method {targetVisibility} hidebysig instance bool Equals(class Program/IPartialEqualityComparer/WrapType`1<!T> obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
      {{
        .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  5
        .locals init (class Program/IPartialEqualityComparer/WrapType`1<!T> V_0,
                 class Program/IPartialEqualityComparer/WrapType`1<!T> V_1,
                 !T V_2,
                 !T V_3)
        IL_0000:  ldarg.0
        IL_0001:  brfalse.s  IL_0027

        IL_0003:  ldarg.1
        IL_0004:  brfalse.s  IL_0025

        IL_0006:  ldarg.0
        IL_0007:  pop
        IL_0008:  ldarg.0
        IL_0009:  stloc.0
        IL_000a:  ldarg.1
        IL_000b:  stloc.1
        IL_000c:  ldloc.0
        IL_000d:  ldfld      !0 class Program/IPartialEqualityComparer/WrapType`1<!T>::item
        IL_0012:  stloc.2
        IL_0013:  ldloc.1
        IL_0014:  ldfld      !0 class Program/IPartialEqualityComparer/WrapType`1<!T>::item
        IL_0019:  stloc.3
        IL_001a:  ldarg.2
        IL_001b:  ldloc.2
        IL_001c:  ldloc.3
        IL_001d:  tail.
        IL_001f:  call       bool [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/HashCompare::GenericEqualityWithComparerIntrinsic<!T>(class [runtime]System.Collections.IEqualityComparer,
                                                                                                                                              !!0,
                                                                                                                                              !!0)
        IL_0024:  ret

        IL_0025:  ldc.i4.0
        IL_0026:  ret

        IL_0027:  ldarg.1
        IL_0028:  ldnull
        IL_0029:  cgt.un
        IL_002b:  ldc.i4.0
        IL_002c:  ceq
        IL_002e:  ret
      }} """ ]
