// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test
open FSharp.Compiler.Diagnostics

[<TestFixture>]
module TypeDirectedConversionTests =
    [<Test>]
    let ``int32 converts to float in method call parameter``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize-"|],
            """
module Test

type Thing() =
    static member Do(i: float) = ()
    
let test() = Thing.Do(100)
        """,
            (fun verifier -> verifier.VerifyIL [
            """
 .method public static void  test() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.s   100
    IL_0002:  conv.r8
    IL_0003:  call       void Test/Thing::Do(float64)
    IL_0008:  ret
  }
            """
            ]))

    [<Test>]
    let ``int32 converts to System.Nullable<float> in method call parameter``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize-"|],
            """
module Test

type Thing() =
    static member Do(i: System.Nullable<float>) = ()
    
let test() = Thing.Do(100)
        """,
            (fun verifier -> verifier.VerifyIL [
            """
  .method public static void  test() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.s   100
    IL_0002:  conv.r8
    IL_0003:  newobj     instance void valuetype [runtime]System.Nullable`1<float64>::.ctor(!0)
    IL_0008:  call       void Test/Thing::Do(valuetype [runtime]System.Nullable`1<float64>)
    IL_000d:  ret
  }
            """
            ]))

    [<Test>]
    let ``int32 converts to float in method call property setter``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize-"|],
            """
module Test

type Thing() =
    member val Do: float = 0.0 with get,set
    
let test() = Thing(Do = 100)
        """,
            (fun verifier -> verifier.VerifyIL [
            """
  .method public static class Test/Thing 
          test() cil managed
  {
    
    .maxstack  4
    .locals init (class Test/Thing V_0)
    IL_0000:  newobj     instance void Test/Thing::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.s   100
    IL_0009:  conv.r8
    IL_000a:  callvirt   instance void Test/Thing::set_Do(float64)
    IL_000f:  ldloc.0
    IL_0010:  ret
  } 
            """
            ]))

    
    [<Test>]
    let ``int32 converts to System.Nullable<float> in method call property setter``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize-"|],
            """
module Test

type Thing() =
    member val Do: System.Nullable<float> = System.Nullable() with get,set
    
let test() = Thing(Do = 100)
        """,
            (fun verifier -> verifier.VerifyIL [
            """
  .method public static class Test/Thing 
          test() cil managed
  {
    
    .maxstack  4
    .locals init (class Test/Thing V_0)
    IL_0000:  newobj     instance void Test/Thing::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.s   100
    IL_0009:  conv.r8
    IL_000a:  newobj     instance void valuetype [runtime]System.Nullable`1<float64>::.ctor(!0)
    IL_000f:  callvirt   instance void Test/Thing::set_Do(valuetype [runtime]System.Nullable`1<float64>)
    IL_0014:  ldloc.0
    IL_0015:  ret
  }
            """
            ]))

    [<Test>]
    let ``int converts to System.Nullable<int> in method call property setter``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize-"|],
            """
module Test

type Thing() =
    member val Do: System.Nullable<int> = System.Nullable() with get,set
    
let test() = Thing(Do = 100)
        """,
            (fun verifier -> verifier.VerifyIL [
            """
  .method public static class Test/Thing 
          test() cil managed
  {
    
    .maxstack  4
    .locals init (class Test/Thing V_0)
    IL_0000:  newobj     instance void Test/Thing::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  ldc.i4.s   100
    IL_0009:  newobj     instance void valuetype [runtime]System.Nullable`1<int32>::.ctor(!0)
    IL_000e:  callvirt   instance void Test/Thing::set_Do(valuetype [runtime]System.Nullable`1<int32>)
    IL_0013:  ldloc.0
    IL_0014:  ret
  } 
            """
            ]))

    [<Test>]
    let ``int converts to System.Nullable<int> in method call parameter``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize-"|],
            """
module Test

type Thing() =
    static member Do(i: System.Nullable<int>) = ()
    
let test() = Thing.Do(100)
        """,
            (fun verifier -> verifier.VerifyIL [
            """
  .method public static void  test() cil managed
  {
    
    .maxstack  8
    IL_0000:  ldc.i4.s   100
    IL_0002:  newobj     instance void valuetype [runtime]System.Nullable`1<int32>::.ctor(!0)
    IL_0007:  call       void Test/Thing::Do(valuetype [runtime]System.Nullable`1<int32>)
    IL_000c:  ret
  } 
            """
            ]))

    [<Test>]
    let ``Passing an incompatible argument for System.Nullable<'T> method call parameter produces accurate error``() =
        CompilerAssert.TypeCheckSingleError
            """
module Test

type Thing() =
    static member Do(i: System.Nullable<float>) = ()
    
let test() = Thing.Do(true)
            """
            FSharpDiagnosticSeverity.Error
            193
            (7, 22, 7, 28)
            """Type constraint mismatch. The type 
    'bool'    
is not compatible with type
    'System.Nullable<float>'    
"""       

    [<Test>]
    let ``Assigning a 'T value to a System.Nullable<'T> binding succeeds``() =
        CompilerAssert.TypeCheckSingleError
            """
module Test
    
let test(): System.Nullable<int> = 1
"""
            FSharpDiagnosticSeverity.Warning
            3391
            (4, 36, 4, 37)
            """This expression uses the implicit conversion 'System.Nullable.op_Implicit(value: int) : System.Nullable<int>' to convert type 'int' to type 'System.Nullable<int>'. See https://aka.ms/fsharp-implicit-convs. This warning may be disabled using '#nowarn "3391"."""
    
    [<Test>]
    let ``Assigning an int32 to a System.Nullable<float> binding fails``() =
        CompilerAssert.TypeCheckSingleError
            """
module Test
    
let test(): System.Nullable<float> = 1
"""
         FSharpDiagnosticSeverity.Error
         1
         (4, 38, 4, 39)
         """This expression was expected to have type
    'System.Nullable<float>'    
but here has type
    'int'    """
