// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests.CodeGen.EmittedIL

open FSharp.Compiler.UnitTests
open NUnit.Framework

[<TestFixture>]
module ``Mutation`` =
    // Regression test for FSHARP1.0:1206

    [<Test>]
    let ``Mutation 01``() =
        CompilerAssert.CompileLibraryAndVerifyIL
            """
module Mutation01
type Test = struct
              val mutable v: int
              member t.setV _ = t.v <- 0
            end
            """
            (fun verifier -> verifier.VerifyIL [
            """
  .class sequential ansi serializable sealed nested public Test
            """
            """
    .field public int32 v
            """
            """
    .method public hidebysig instance void
            setV<a>(!!a _arg1) cil managed
    {

      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldc.i4.0
      IL_0002:  stfld      int32 Mutation01/Test::v
      IL_0007:  ret
    }
            """
            ])

    [<Test>]
    let ``Mutation 02``() =
        CompilerAssert.CompileLibraryAndVerifyIL
            """
module Mutation02
let x = System.TimeSpan.MinValue
x.ToString()
            """
            (fun verifier -> verifier.VerifyIL [
            """
  .method public specialname static valuetype [mscorlib]System.TimeSpan
          get_x() cil managed
  {
    .maxstack  8
    IL_0000:  ldsfld     valuetype [mscorlib]System.TimeSpan '<StartupCode$Mutation02>'.$Mutation02::x@4
    IL_0005:  ret
  }
            """
            """
  .property valuetype [mscorlib]System.TimeSpan
          x()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 )
    .get valuetype [mscorlib]System.TimeSpan Mutation02::get_x()
  }
            """
            """
        void  .cctor() cil managed
  {

    .maxstack  8
    IL_0000:  ldsfld     valuetype [mscorlib]System.TimeSpan [mscorlib]System.TimeSpan::MinValue
    IL_0005:  stsfld     valuetype [mscorlib]System.TimeSpan '<StartupCode$Mutation02>'.$Mutation02::x@3
    IL_000a:  call       valuetype [mscorlib]System.TimeSpan Mutation02::get_x()
    IL_000f:  stsfld     valuetype [mscorlib]System.TimeSpan '<StartupCode$Mutation02>'.$Mutation02::copyOfStruct@4
    IL_0014:  ldsflda    valuetype [mscorlib]System.TimeSpan '<StartupCode$Mutation02>'.$Mutation02::copyOfStruct@4
    IL_0019:  constrained. [mscorlib]System.TimeSpan
    IL_001f:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0024:  pop
    IL_0025:  ret
  }
            """
            ])

    [<Test>]
    let ``Mutation 03``() =
        CompilerAssert.CompileLibraryAndVerifyIL
            """
module Mutation03
let x = System.DateTime.Now
x.Day
            """
            (fun verifier -> verifier.VerifyIL [
            """
  .method public specialname static valuetype [mscorlib]System.DateTime
          get_x() cil managed
  {

    .maxstack  8
    IL_0000:  ldsfld     valuetype [mscorlib]System.DateTime '<StartupCode$Mutation03>'.$Mutation03::'x@4-2'
    IL_0005:  ret
  }
            """
            """
  .property valuetype [mscorlib]System.DateTime
          x()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 )
    .get valuetype [mscorlib]System.DateTime Mutation03::get_x()
  }
            """
            """
        void  .cctor() cil managed
  {

    .maxstack  8
    IL_0000:  call       valuetype [mscorlib]System.DateTime [mscorlib]System.DateTime::get_Now()
    IL_0005:  stsfld     valuetype [mscorlib]System.DateTime '<StartupCode$Mutation03>'.$Mutation03::x@3
    IL_000a:  call       valuetype [mscorlib]System.DateTime Mutation03::get_x()
    IL_000f:  stsfld     valuetype [mscorlib]System.DateTime '<StartupCode$Mutation03>'.$Mutation03::copyOfStruct@4
    IL_0014:  ldsflda    valuetype [mscorlib]System.DateTime '<StartupCode$Mutation03>'.$Mutation03::copyOfStruct@4
    IL_0019:  call       instance int32 [mscorlib]System.DateTime::get_Day()
    IL_001e:  pop
    IL_001f:  ret
  }
            """
            ])

    [<Test>]
    let ``Mutation 04``() =
        CompilerAssert.CompileLibraryAndVerifyIL
            """
module Mutation04
let x = System.Decimal.MaxValue
x.ToString()
            """
            (fun verifier -> verifier.VerifyIL [
            """
  .method public specialname static valuetype [mscorlib]System.Decimal
          get_x() cil managed
  {
    // Code size       6 (0x6)
    .maxstack  8
    IL_0000:  ldsfld     valuetype [mscorlib]System.Decimal '<StartupCode$Mutation04>'.$Mutation04::'x@4-4'
    IL_0005:  ret
  } // end of method Mutation04::get_x
            """
            """
  .property valuetype [mscorlib]System.Decimal
          x()
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 09 00 00 00 00 00 )
    .get valuetype [mscorlib]System.Decimal Mutation04::get_x()
  } // end of property Mutation04::x
} // end of class Mutation04
            """
            """
void  .cctor() cil managed
  {

    .maxstack  8
    IL_0000:  ldsfld     valuetype [mscorlib]System.Decimal [mscorlib]System.Decimal::MaxValue
    IL_0005:  stsfld     valuetype [mscorlib]System.Decimal '<StartupCode$Mutation04>'.$Mutation04::x@3
    IL_000a:  call       valuetype [mscorlib]System.Decimal Mutation04::get_x()
    IL_000f:  stsfld     valuetype [mscorlib]System.Decimal '<StartupCode$Mutation04>'.$Mutation04::copyOfStruct@4
    IL_0014:  ldsflda    valuetype [mscorlib]System.Decimal '<StartupCode$Mutation04>'.$Mutation04::copyOfStruct@4
    IL_0019:  constrained. [mscorlib]System.Decimal
    IL_001f:  callvirt   instance string [mscorlib]System.Object::ToString()
    IL_0024:  pop
    IL_0025:  ret
  }
            """
            ])

    [<Test>]
    let ``Mutation 05``() =
        CompilerAssert.CompileLibraryAndVerifyIL
            """
module Mutation05
type C() =
    [<VolatileFieldAttribute>]
    let mutable x = 1

    member this.X with get() = x and set v = x <- v


type StaticC() =
    [<VolatileFieldAttribute>]
    static let mutable x = 1

    static member X with get() = x and set v = x <- v
            """
            (fun verifier -> verifier.VerifyIL [
            """
  .class auto ansi serializable nested public C
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 )
    .field assembly int32 x
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.VolatileFieldAttribute::.ctor() = ( 01 00 00 00 )
    .method public specialname rtspecialname
            instance void  .ctor() cil managed
    {

      .maxstack  8
      .language '{AB4F38C9-B6E6-43BA-BE3B-58080B2CCCE3}', '{994B45C4-E6E9-11D2-903F-00C04FA302A1}', '{5A869D0B-6611-11D3-BD2A-0000F80849BD}'
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ldarg.0
      IL_0009:  ldc.i4.1
      IL_000a:  volatile.
      IL_000c:  stfld      int32 Mutation05/C::x
      IL_0011:  ret
    }

    .method public hidebysig specialname
            instance int32  get_X() cil managed
    {

      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  volatile.
      IL_0003:  ldfld      int32 Mutation05/C::x
      IL_0008:  ret
    }

    .method public hidebysig specialname
            instance void  set_X(int32 v) cil managed
    {

      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  ldarg.1
      IL_0002:  volatile.
      IL_0004:  stfld      int32 Mutation05/C::x
      IL_0009:  ret
    }

    .property instance int32 X()
    {
      .set instance void Mutation05/C::set_X(int32)
      .get instance int32 Mutation05/C::get_X()
    }
  }
            """
            """
  .class auto ansi serializable nested public StaticC
         extends [mscorlib]System.Object
  {
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 )
    .field static assembly int32 x
    .custom instance void [FSharp.Core]Microsoft.FSharp.Core.VolatileFieldAttribute::.ctor() = ( 01 00 00 00 )
    .field static assembly int32 init@10
    .method public specialname rtspecialname
            instance void  .ctor() cil managed
    {

      .maxstack  8
      IL_0000:  ldarg.0
      IL_0001:  callvirt   instance void [mscorlib]System.Object::.ctor()
      IL_0006:  ldarg.0
      IL_0007:  pop
      IL_0008:  ret
    }

    .method public specialname static int32
            get_X() cil managed
    {

      .maxstack  8
      IL_0000:  volatile.
      IL_0002:  ldsfld     int32 Mutation05/StaticC::init@10
      IL_0007:  ldc.i4.1
      IL_0008:  bge.s      IL_0011

      IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_000f:  br.s       IL_0011

      IL_0011:  volatile.
      IL_0013:  ldsfld     int32 Mutation05/StaticC::x
      IL_0018:  ret
    }

    .method public specialname static void
            set_X(int32 v) cil managed
    {

      .maxstack  8
      IL_0000:  volatile.
      IL_0002:  ldsfld     int32 Mutation05/StaticC::init@10
      IL_0007:  ldc.i4.1
      IL_0008:  bge.s      IL_0011

      IL_000a:  call       void [FSharp.Core]Microsoft.FSharp.Core.LanguagePrimitives/IntrinsicFunctions::FailStaticInit()
      IL_000f:  br.s       IL_0011

      IL_0011:  ldarg.0
      IL_0012:  volatile.
      IL_0014:  stsfld     int32 Mutation05/StaticC::x
      IL_0019:  ret
    }

    .method private specialname rtspecialname static
            void  .cctor() cil managed
    {

      .maxstack  8
      IL_0000:  ldc.i4.0
      IL_0001:  stsfld     int32 '<StartupCode$Mutation05>'.$Mutation05::init@
      IL_0006:  ldsfld     int32 '<StartupCode$Mutation05>'.$Mutation05::init@
      IL_000b:  pop
      IL_000c:  ret
    }

    .property int32 X()
    {
      .set void Mutation05/StaticC::set_X(int32)
      .get int32 Mutation05/StaticC::get_X()
    }
  }
            """
            """
    IL_0000:  ldc.i4.1
    IL_0001:  volatile.
    IL_0003:  stsfld     int32 Mutation05/StaticC::x
    IL_0008:  ldc.i4.1
    IL_0009:  volatile.
    IL_000b:  stsfld     int32 Mutation05/StaticC::init@10
    IL_0010:  ret
            """
            ])