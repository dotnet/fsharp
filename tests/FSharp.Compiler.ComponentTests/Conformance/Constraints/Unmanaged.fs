// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Constraints

open Xunit
open FSharp.Test.Compiler
open FSharp.Test

module Unmanaged =

    [<Fact>]
    let ``User-defined struct types considered unmanaged when all members are unmanaged`` () =
        Fsx """
[<Struct>]
type MyStruct(x: int, y: int) =
    member _.X = x
    member _.Y = y
[<Struct>]
type MyStructGeneric<'T when 'T: unmanaged>(x: 'T, y: 'T) =
    member _.X = x
    member _.Y = y
[<Struct>]
type MyStructGenericWithNoConstraint<'T>(x: 'T, y: 'T) =
    member _.X = x
    member _.Y = y
[<Struct>]
type Test<'T when 'T: unmanaged> =
    val element: 'T
[<Struct>]
type S<'T> =
    val X : 'T
    new (x) = { X = x }
let test (x: 'T when 'T : unmanaged) = ()
test(Unchecked.defaultof<S<int>>)
test(S<int>(1))
test(S<MyStruct>(MyStruct(1,2)))
test(S<MyStructGeneric<int>>(MyStructGeneric<int>(1,2)))
test(S<MyStructGenericWithNoConstraint<int>>(MyStructGenericWithNoConstraint<int>(1,2)))
let _ = Test<int>()
let _ = Test<MyStruct>()
let _ = Test<MyStructGeneric<int>>()
let _ = Test<MyStructGeneric<System.TimeSpan>>()
let _ = Test<MyStructGeneric<System.DateTime>>()
let _ = Test<MyStructGeneric<MyStructGeneric<MyStructGeneric<int>>>>()
let _ = Test<MyStructGenericWithNoConstraint<int>>()
let _ = Test<MyStructGenericWithNoConstraint<MyStructGenericWithNoConstraint<int>>>()
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Struct tuples considered unmanaged when all elements are unmanaged`` () =
        Fsx """
[<Struct>]
type Test<'T when 'T: unmanaged> =
    val element: 'T
let test (x: 'T when 'T : unmanaged) = ()
let x = struct(1, 2)
test x
test (struct(1, 2))
test (struct(1, 2, 3))
test (struct(1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, struct(12,13)))

// test that constraint is being propagated
let functionUsingTestInternally (struct(a,b) as s) =
    test s

functionUsingTestInternally (struct(42,42))
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``User-defined struct (anonymous)records considered unmanaged when all fields are unmanaged`` () =
        Fsx """
[<Struct>]
type MyRecd = { X: int; Y: int; }
[<Struct>]
type MyRecdGeneric<'T when 'T: unmanaged> = { X:'T; Y: 'T; }
[<Struct>]
type MyRecdGenericWithNoConstraint<'T> = { X:'T; Y: 'T; }
[<Struct>]
type Test<'T when 'T: unmanaged> =
    val element: 'T
[<Struct>]
type S<'T> =
    val X : 'T
    new (x) = { X = x }
let test (x: 'T when 'T : unmanaged) = ()
test(Unchecked.defaultof<S<int>>)
test(S<int>(1))
test(S<MyRecd>({ X = 1; Y = 1 }))
test(S<MyRecdGeneric<int>>({ X = 1; Y = 1 }))
test(S<MyRecdGenericWithNoConstraint<int>>({ X = 1; Y = 1 }))
let x = struct {| X = 1; Y = 1 |}
test(x)
test(struct {| X = 1; Y = 1 |})

// test that constraint is being propagated
let createAndTestAnonRecd (x) = 
    let created = struct{|OnlyValue = x|}
    test(created)
    ()

createAndTestAnonRecd 15
createAndTestAnonRecd (struct{|Nested = 15|})
createAndTestAnonRecd (struct{|Nested = struct(15,S<MyRecdGeneric<int>>({ X = 1; Y = 1 }))|})

let _ = Test<int>()
let _ = Test<MyRecd>()
let _ = Test<MyRecdGeneric<int>>()
let _ = Test<MyRecdGeneric<MyRecdGeneric<MyRecdGeneric<int>>>>()
let _ = Test<MyRecdGenericWithNoConstraint<int>>()
let _ = Test<MyRecdGenericWithNoConstraint<MyRecdGenericWithNoConstraint<int>>>()
        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Struct single- and multi-case unions considered unmanaged when all cases are all unmanaged`` () =
        Fsx """
[<Struct>]
type SingleCaseUnion = X
[<Struct>]
type MultiCaseUnion = A | B
[<Struct>]
type Single<'T> = C of 'T
[<Struct>]
type SingleC<'T when 'T: unmanaged> = CC of 'T
[<Struct>]
type Result<'T,'TError> =
    | Ok of ok: 'T
    | Error of error: 'TError
[<Struct>]
type ResultC<'T,'TError when 'T: unmanaged and 'TError: unmanaged> =
    | OkC of ok: 'T
    | ErrorC of error: 'TError
[<Struct>]
type Test<'T when 'T: unmanaged> =
    val element: 'T
[<Struct>]
type DateOrTimeStampOrUnixOrCustom = 
    | DuDateTime of dt:System.DateTime
    | DuTimeSpan of ts:System.TimeSpan
    | DuUnix of i:int
let test (x: 'T when 'T : unmanaged) = ()
test(SingleCaseUnion.X)
test(MultiCaseUnion.A)
test(MultiCaseUnion.B)
test(C 1)
test(CC 1)
test(Ok 1)
test(Error 2)
test(ResultC<int,int>.OkC 1)
test(ResultC<int,int>.ErrorC 1)
test(DuUnix 132456)
test(DuDateTime System.DateTime.Now)
let _ = Test<SingleCaseUnion>()
let _ = Test<MultiCaseUnion>()
let _ = Test<Single<int>>()
let _ = Test<Single<Single<Single<int>>>>()
let _ = Test<Single<SingleC<Single<MultiCaseUnion>>>>()
let _ = Test<SingleC<int>>()
let _ = Test<SingleC<SingleC<int>>>()
let _ = Test<Result<int, byte>>()
let _ = Test<Result<Result<int, byte>, Single<MultiCaseUnion>>>()
let _ = Test<ResultC<int, byte>>()

// test that constraint is being propagated
let resultCreatingFunction (x) = 
    try
        let capturedVal = Ok x
        test capturedVal
        capturedVal
    with _ -> Error 15

let _ = resultCreatingFunction A

        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Generic user-defined type with non-unmanaged types is NOT considered unmanaged`` () =
        Fsx """
type NonStructRecd = { X: int }
type NonStructRecdC<'T when 'T : unmanaged> = { X: 'T }
[<Struct>]
type Test<'T when 'T: unmanaged> =
    val element: 'T
[<Struct>]
type W<'T> = { x: 'T }
[<Struct>]
type S<'T> = { x: W<'T> }
[<Struct>]
type X<'T> =
    val Z : 'T
    new (x) = { Z = x }
 
[<Struct>]
type A<'T, 'U> =
    [<DefaultValue(false)>] val X : 'T
let test (x: 'T when 'T : unmanaged) = ()
test(Unchecked.defaultof<S<obj>>)
test(X(obj()))
test (A<obj, int>())
let foo<'T> () = test (A<'T, obj>())
let _ = Test<obj>()
let _ = Test<NonStructRecd>()
let _ = Test<NonStructRecdC<int>>()
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
             (Error 1, Line 20, Col 6, Line 20, Col 33, "A generic construct requires that the type 'S<obj>' is an unmanaged type")
             (Error 193, Line 21, Col 6, Line 21, Col 14, "A generic construct requires that the type 'X<'a>' is an unmanaged type")
             (Error 193, Line 22, Col 7, Line 22, Col 20, "A generic construct requires that the type 'A<obj,int>' is an unmanaged type")
             (Error 193, Line 23, Col 24, Line 23, Col 36, "A generic construct requires that the type 'A<'T,obj>' is an unmanaged type")
             (Error 1, Line 24, Col 9, Line 24, Col 18, "A generic construct requires that the type 'obj' is an unmanaged type")
             (Error 1, Line 25, Col 9, Line 25, Col 28, "A generic construct requires that the type 'NonStructRecd' is an unmanaged type")
             (Error 1, Line 26, Col 9, Line 26, Col 34, "A generic construct requires that the type 'NonStructRecdC<int>' is an unmanaged type")]

    [<Fact>]
    let ``Disallow both 'unmanaged' and 'not struct' constraints`` () =
        Fsx "type X<'T when 'T: unmanaged and 'T: not struct> = class end"
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
        (Error 43, Line 1, Col 34, Line 1, Col 48, "The constraints 'unmanaged' and 'not struct' are inconsistent")]

    [<Fact>]
    let ``IsUnmanagedAttribute Attribute is emitted and generated for unmanaged constraint on type`` () =
        Fsx "[<Struct;NoEquality;NoComparison>] type Test<'T when 'T: unmanaged and 'T: struct> = struct end"
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
    .class public abstract auto ansi sealed Test
           extends [runtime]System.Object
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
      .class sequential ansi serializable sealed nested public beforefieldinit Test`1<valuetype T>
             extends [runtime]System.ValueType
      {
        .pack 0
        .size 1
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.StructAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoEqualityAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoComparisonAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
        .param type T 
          .custom instance void System.Runtime.CompilerServices.IsUnmanagedAttribute::.ctor() = ( 01 00 00 00 ) 
      } 
    
    }""";"""
.class private auto ansi beforefieldinit System.Runtime.CompilerServices.IsUnmanagedAttribute
       extends [runtime]System.Attribute"""]

    [<Fact>]
    let ``IsUnmanagedAttribute Attribute is emitted for function with unmanaged constraint`` () =
        Fsx "let testMyFunction (x: 'TUnmanaged when 'TUnmanaged : unmanaged) = struct(x,1)"
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
      .method public static valuetype [runtime]System.ValueTuple`2<!!TUnmanaged,int32> 
          testMyFunction<TUnmanaged>(!!TUnmanaged x) cil managed
  {
    .param type TUnmanaged 
      .custom instance void System.Runtime.CompilerServices.IsUnmanagedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ldarg.0
    IL_0001:  ldc.i4.1
    IL_0002:  newobj     instance void valuetype [runtime]System.ValueTuple`2<!!TUnmanaged,int32>::.ctor(!0,
                                                                                                          !1)
    IL_0007:  ret
  } """;"""
.class private auto ansi beforefieldinit System.Runtime.CompilerServices.IsUnmanagedAttribute
       extends [runtime]System.Attribute"""]



    [<Fact>]
    let ``Consume C#-defined unmanaged constraint incorrectly in F# - report error`` () = 
        let csLib =
            CSharp "namespace CsLib{ public record struct CsharpStruct<T>(T item) where T:unmanaged;}"
            |> withCSharpLanguageVersion CSharpLanguageVersion.Preview
            |> withName "csLib"

        let app = FSharp """module MyFsharpApp
open CsLib
let y = new CsharpStruct<struct(int*string)>(struct(1,"this is string"))
        """     |> withReferences [csLib]

        app
        |> compile
        |> shouldFail
        |> withDiagnostics [(Error 1, Line 3, Col 13, Line 3, Col 45, "A generic construct requires that the type 'string' is an unmanaged type")]