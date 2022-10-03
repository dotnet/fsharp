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

    [<Test>]
    let ``Overloading on System.Nullable and Result both work without error``() =
        CompilerAssert.Pass
            """
module Test
    
type M() =
    static member A(n: System.Nullable<'T>) = ()
    static member A(r: Result<'T, 'TError>) = ()

let test() =
    M.A(System.Nullable 3)
    M.A(Result<int, string>.Ok 3)
"""     
    

    [<Test>]
    let ``Overloading on System.Nullable and Result produces a builtin conversion warning when Nullable is picked``() =
        CompilerAssert.TypeCheckSingleErrorWithOptions
            [| "--warnon:3389" |]
            """
module Test
    
type M() =
    static member A(n: System.Nullable<int>) = ()
    static member A(r: Result<'T, 'TError>) = ()

let test() =
    M.A(3)
"""     
            FSharpDiagnosticSeverity.Warning
            3389
            (9, 9, 9, 10)
            """This expression uses a built-in implicit conversion to convert type 'int' to type 'System.Nullable<int>'. See https://aka.ms/fsharp-implicit-convs."""
    
    [<Test>]
    let ``Overloading on System.Nullable<int>, System.Nullable<'T> and int all work without error``() =
        CompilerAssert.RunScript
            """
let assertTrue x =
    (x || failwith "Unexpected overload") |> ignore

type M() =
    static member A(n: System.Nullable<'T>) = 1
    static member A(n: System.Nullable<int>) = 2
    static member A(n: int) = 3

let test() =
    M.A(System.Nullable 3.) = 1 |> assertTrue
    M.A(System.Nullable 3) = 2 |> assertTrue
    M.A(3) = 3 |> assertTrue
    
test()
        """ []

    [<Test>]
    let ``Picking overload for typar does not favor any form of System.Nullable nor produce ambiguity warnings``() =
        CompilerAssert.TypeCheckSingleError
            """
module Test
    
type M() =
    static member A(n: System.Nullable<'T>) = ()
//    static member A(n: System.Nullable<float>) = ()
    static member A(n: System.Nullable<int>) = ()
    static member A(n: int) = ()

let test(x: 'T) =
    M.A(x)
"""
         FSharpDiagnosticSeverity.Warning
         64
         (11, 5, 11, 11)
         """This construct causes code to be less generic than indicated by the type annotations. The type variable 'T has been constrained to be type 'int'."""

    [<Test>]
    let ``Picking overload for typar fails when incompatible types are part of the candidate set``() =
        CompilerAssert.TypeCheckWithErrors
            """
module Test
    
type M() =
    static member A(n: System.Nullable<'T>) = ()
    static member A(n: System.Nullable<float>) = ()
    static member A(n: System.Nullable<int>) = ()
    static member A(n: int) = ()

let test(x: 'T) =
    M.A(x)
    
type M2() =
    static member A(n: System.Nullable<float>) = ()
    static member A(n: System.Nullable<int>) = ()
    static member A(n: int) = ()

let test2(x: 'T) =
    M2.A(x)
"""
         [|
             (FSharpDiagnosticSeverity.Error,
             41,
             (11, 5, 11, 11),
             """A unique overload for method 'A' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: 'T

Candidates:
 - static member M.A: n: System.Nullable<'T> -> unit when 'T: (new: unit -> 'T) and 'T: struct and 'T :> System.ValueType
 - static member M.A: n: System.Nullable<float> -> unit
 - static member M.A: n: System.Nullable<int> -> unit
 - static member M.A: n: int -> unit""")
             (FSharpDiagnosticSeverity.Error,
             41,
             (19, 5, 19, 12),
             """A unique overload for method 'A' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: 'T

Candidates:
 - static member M2.A: n: System.Nullable<float> -> unit
 - static member M2.A: n: System.Nullable<int> -> unit
 - static member M2.A: n: int -> unit""")   
         |]
    
    [<Test>]
    let ``Ambiguous overload for typar does not pick System.Nullable<'T>``() =
        CompilerAssert.TypeCheckSingleError
            """
module Test
    
type M() =
    static member A(n: System.Nullable<'T>) = ()
    static member A(n: int) = ()
    static member A(n: float) = ()

let test(x: 'T) =
    M.A(x)
"""
         FSharpDiagnosticSeverity.Error
         41
         (10, 5, 10, 11)
         """A unique overload for method 'A' could not be determined based on type information prior to this program point. A type annotation may be needed.

Known type of argument: 'T

Candidates:
 - static member M.A: n: System.Nullable<'T> -> unit when 'T: (new: unit -> 'T) and 'T: struct and 'T :> System.ValueType
 - static member M.A: n: float -> unit
 - static member M.A: n: int -> unit"""
    
    [<Test>]
    let ``Passing an argument in nested method call property setter works``() =
        CompilerAssert.CompileLibraryAndVerifyILWithOptions([|"--optimize-"|],
            """
module Test

type Input<'T>(_v: 'T) =
    static member op_Implicit(value: 'T): Input<'T> = Input<'T>(value)

type OtherArgs() =
    member val Name: string = Unchecked.defaultof<_> with get,set
type SomeArgs() =
    member val OtherArgs: Input<OtherArgs> = Unchecked.defaultof<_> with get, set
    
let test() =
    SomeArgs(OtherArgs = OtherArgs(Name = "test"))
"""
         ,
        (fun verifier -> verifier.VerifyIL [
        """
  .method public static class Test/SomeArgs 
          test() cil managed
  {
    
    .maxstack  5
    .locals init (class Test/SomeArgs V_0,
             class Test/OtherArgs V_1)
    IL_0000:  newobj     instance void Test/SomeArgs::.ctor()
    IL_0005:  stloc.0
    IL_0006:  ldloc.0
    IL_0007:  newobj     instance void Test/OtherArgs::.ctor()
    IL_000c:  stloc.1
    IL_000d:  ldloc.1
    IL_000e:  ldstr      "test"
    IL_0013:  callvirt   instance void Test/OtherArgs::set_Name(string)
    IL_0018:  ldloc.1
    IL_0019:  call       class Test/Input`1<!0> class Test/Input`1<class Test/OtherArgs>::op_Implicit(!0)
    IL_001e:  callvirt   instance void Test/SomeArgs::set_OtherArgs(class Test/Input`1<class Test/OtherArgs>)
    IL_0023:  ldloc.0
    IL_0024:  ret
  } 
            """
        ]))

    [<Test>]
    let ``Test retrieving an argument provided in a nested method call property setter works``() =
        CompilerAssert.RunScript
            """
type Input<'T>(v: 'T) =
    member _.Value = v
    static member op_Implicit(value: 'T): Input<'T> = Input<'T>(value)

type OtherArgs() =
    member val Name: string = Unchecked.defaultof<_> with get,set
type SomeArgs() =
    member val OtherArgs: Input<OtherArgs> = Unchecked.defaultof<_> with get, set
    
let test() =
    SomeArgs(OtherArgs = OtherArgs(Name = "test"))
    
if not (test().OtherArgs.Value.Name = "test") then failwith "Unexpected value was returned after setting Name"
        """ []
