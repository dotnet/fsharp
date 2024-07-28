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
    let ``Another assembly``(realsig) =
        let module1 =
            FSharpWithFileName "Module1.fs"
                """
module Module1
    
    [<Struct>]
    type Struct(v: int, u: int) =
        member _.V = v
        member _.U = u """
            |> withRealInternalSignature realsig
            |> withOptimize
            |> asLibrary
            |> withName "module1"

        let module2 = 
            FSharpWithFileName "Program.fs"
                """
Module1.Struct(1, 2) = Module1.Struct(2, 3) |> ignore"""

        module2
        |> withReferences [module1]
        |> withRealInternalSignature realsig
        |> withOptimize
        |> compileExeAndRun
        |> shouldSucceed
        |> withILContains [ """
IL_0022: call class [mscorlib]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
IL_0027: call instance bool [module1]Module1/Struct::Equals(valuetype [module1]Module1/Struct, class [mscorlib]System.Collections.IEqualityComparer)
""" ]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Single assembly``(realsig) =
        let mainModule = 
            FSharpWithFileName "Program.fs"
                """
module Module1 =

    [<Struct>]
    type Struct(v: int, u: int) =
        member _.V = v
        member _.U = u

module Module2 =
    Module1.Struct(1, 2) = Module1.Struct(2, 3) |> ignore
"""
        mainModule
        |> withRealInternalSignature realsig
        |> withOptimize
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL [
            """
      .method public hidebysig virtual final instance bool  Equals(valuetype Program/Module1/Struct obj) cil managed
      {
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
      } """
            """
    IL_0022:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
    IL_0027:  call       instance bool Program/Module1/Struct::Equals(valuetype Program/Module1/Struct,
                                                                      class [runtime]System.Collections.IEqualityComparer)
""" ]


    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Another assembly with private type`` (realsig) =
        let module1 =
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

        let module2 = 
            FSharpWithFileName "Program.fs"
                """
open Module1

Value.Zero = Value.Create 0 |> ignore"""

            |> withRealInternalSignature realsig
            |> withReferences [module1]
            |> withOptimize

        module1
        |> withRealInternalSignature realsig
        |> compile
        |> shouldSucceed
        |> withILContains ["""
    .method public hidebysig virtual final 
            instance bool  Equals(class Module1/Value obj,
                                  class [runtime]System.Collections.IEqualityComparer comp) cil managed
    {
      .custom instance void [runtime]System.Runtime.CompilerServices.CompilerGeneratedAttribute::.ctor() = ( 01 00 00 00 ) 
      
      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  brfalse.s  IL_0017

      IL_0003:  ldarg.1
      IL_0004:  brfalse.s  IL_0015

      IL_0006:  ldarg.0
      IL_0007:  ldfld      uint32 Module1/Value::value@
      IL_000c:  ldarg.1
      IL_000d:  ldfld      uint32 Module1/Value::value@
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
    } 
        """]
        |> ignore
        
        module2
        |> withRealInternalSignature realsig
        |> compileExeAndRun
        |> shouldSucceed
        |> withILContains ["""
IL_001f:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
IL_0024:  callvirt   instance bool [module1]Module1/Value::Equals(class [module1]Module1/Value,
                                                                  class [runtime]System.Collections.IEqualityComparer)
        """]

    [<InlineData(true)>]        // RealSig
    [<InlineData(false)>]       // Regular
    [<Theory>]
    let ``Single Assembly with private type`` (realsig) =

        let mainModule = 
            FSharpWithFileName "Program.fs"
                """
module Module1 =

    type Value =
        private { value: uint32 }

        static member Zero = { value = 0u }
        static member Create(value: int) = { value = uint value }

module Module2 =
    open Module1

    Value.Zero = Value.Create 0 |> ignore"""

        mainModule
        |> withRealInternalSignature realsig
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL [
            """
      .method public hidebysig virtual final instance bool  Equals(class Program/Module1/Value obj) cil managed
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
      } """
            """
    IL_0020:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
    IL_0025:  callvirt   instance bool Program/Module1/Value::Equals(class Program/Module1/Value,
                                                                     class [runtime]System.Collections.IEqualityComparer)
        """ ]
