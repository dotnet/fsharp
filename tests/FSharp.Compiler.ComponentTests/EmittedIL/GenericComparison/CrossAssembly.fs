// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL

open Xunit
open FSharp.Test.Compiler

module GenericComparisonCrossAssembly =

    [<Fact>]
    let ``fslib``() =
        FSharpWithFileName "Program.fs"
            """
ValueSome (1, 2) = ValueSome (2, 3) |> ignore"""
        |> withOptimize
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL [ """
  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.1
    IL_0001:  ldc.i4.2
    IL_0002:  newobj     instance void class [runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                          !1)
    IL_0007:  call       valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<class [runtime]System.Tuple`2<int32,int32>>::NewValueSome(!0)
    IL_000c:  stsfld     valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<class [runtime]System.Tuple`2<int32,int32>> Program::x@1
    IL_0011:  ldc.i4.2
    IL_0012:  ldc.i4.3
    IL_0013:  newobj     instance void class [runtime]System.Tuple`2<int32,int32>::.ctor(!0,
                                                                                          !1)
    IL_0018:  call       valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<!0> valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<class [runtime]System.Tuple`2<int32,int32>>::NewValueSome(!0)
    IL_001d:  stsfld     valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<class [runtime]System.Tuple`2<int32,int32>> Program::y@1
    IL_0022:  ldsflda    valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<class [runtime]System.Tuple`2<int32,int32>> Program::x@1
    IL_0027:  call       valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<class [runtime]System.Tuple`2<int32,int32>> Program::get_y@1()
    IL_002c:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
    IL_0031:  call       instance bool valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<class [runtime]System.Tuple`2<int32,int32>>::Equals(valuetype [FSharp.Core]Microsoft.FSharp.Core.FSharpValueOption`1<!0>,
                                                                                                                                                             class [runtime]System.Collections.IEqualityComparer)
    IL_0036:  stsfld     bool Program::arg@1
    IL_003b:  ret
  } 
""" ]

    [<Fact>]
    let ``Another assembly``() =
        let module1 =
            FSharpWithFileName "Module1.fs"
                """
module Module1
    
    [<Struct>]
    type Struct(v: int, u: int) =
        member _.V = v
        member _.U = u """
            |> withOptimize
            |> asLibrary
            |> withName "module1"

        let module2 = 
            FSharpWithFileName "Program.fs"
                """
Module1.Struct(1, 2) = Module1.Struct(2, 3) |> ignore"""

        module2
        |> withReferences [module1]
        |> withOptimize
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL [ """
  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.1
    IL_0001:  ldc.i4.2
    IL_0002:  newobj     instance void [module1]Module1/Struct::.ctor(int32,
                                                                      int32)
    IL_0007:  stsfld     valuetype [module1]Module1/Struct Program::x@1
    IL_000c:  ldc.i4.2
    IL_000d:  ldc.i4.3
    IL_000e:  newobj     instance void [module1]Module1/Struct::.ctor(int32,
                                                                      int32)
    IL_0013:  stsfld     valuetype [module1]Module1/Struct Program::y@1
    IL_0018:  ldsflda    valuetype [module1]Module1/Struct Program::x@1
    IL_001d:  call       valuetype [module1]Module1/Struct Program::get_y@1()
    IL_0022:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
    IL_0027:  call       instance bool [module1]Module1/Struct::Equals(valuetype [module1]Module1/Struct,
                                                                       class [runtime]System.Collections.IEqualityComparer)
    IL_002c:  stsfld     bool Program::arg@1
    IL_0031:  ret
  } 
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

            |> withReferences [module1]
            |> withOptimize

        module1
        |> compile
        |> shouldSucceed
        |> withILContains ["""
    .method public hidebysig instance bool Equals(class Module1/Value obj, class [runtime]System.Collections.IEqualityComparer comp) cil managed
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
        |> compileExeAndRun
        |> shouldSucceed
        |> verifyIL ["""
  .method assembly specialname static void staticInitialization@() cil managed
  {
    
    .maxstack  8
    IL_0000:  call       class [module1]Module1/Value [module1]Module1/Value::get_Zero()
    IL_0005:  stsfld     class [module1]Module1/Value Program::x@1
    IL_000a:  ldc.i4.0
    IL_000b:  call       class [module1]Module1/Value [module1]Module1/Value::Create(int32)
    IL_0010:  stsfld     class [module1]Module1/Value Program::y@1
    IL_0015:  call       class [module1]Module1/Value Program::get_x@1()
    IL_001a:  call       class [module1]Module1/Value Program::get_y@1()
    IL_001f:  call       class [runtime]System.Collections.IEqualityComparer [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives::get_GenericEqualityComparer()
    IL_0024:  callvirt   instance bool [module1]Module1/Value::Equals(class [module1]Module1/Value,
                                                                      class [runtime]System.Collections.IEqualityComparer)
    IL_0029:  stsfld     bool Program::arg@1
    IL_002e:  ret
  } 
        """]
