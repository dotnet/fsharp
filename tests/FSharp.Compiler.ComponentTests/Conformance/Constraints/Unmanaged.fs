// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.Constraints

open Xunit
open FSharp.Test.Compiler
open FSharp.Test

module Unmanaged =

    [<Fact>]
    let ``voidptr is unmanaged`` () = 
        Fsx """
[<Struct>]
type Test<'T when 'T: unmanaged> =
    val element: 'T

let test (x: 'T when 'T : unmanaged) = ()

test (NativeInterop.NativePtr.nullPtr<voidptr>)
test (NativeInterop.NativePtr.nullPtr<voidptr> |> NativeInterop.NativePtr.toVoidPtr)
let _ = Test<voidptr voption>()
        """
        |> withNoWarn 9
        |> typecheck
        |> shouldSucceed
    

    [<Fact>]
    let ``nativeptr of voidptr works`` () = 
        Fsx """
let myVal : nativeptr<voidptr> =  Unchecked.defaultof<_>
        """
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``Struct with private field can be unmanaged`` () = 
        Fsx """
[<Struct>]
type Test<'T when 'T: unmanaged> =
    val element: 'T
type DoubleType<'T> =
    struct
        val x: float
        val private y: 'T
    end
let _ = Test<DoubleType<int>>()
let _ = Test<DoubleType<DoubleType<byte>>>()
        """
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``Struct with private managed field cannot be unmanaged`` () = 
        Fsx """
[<Struct>]
type Test<'T when 'T: unmanaged> =
    val element: 'T
type DoubleType<'T> =
    struct
        val x: float
        val private y: 'T
    end
let _ = Test<DoubleType<string>>()
let _ = Test<DoubleType<DoubleType<int option>>>()
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
                   Error 1, Line 10, Col 9, Line 10, Col 33, "A generic construct requires that the type 'DoubleType<string>' is an unmanaged type"
                   Error 1, Line 11, Col 9, Line 11, Col 49, "A generic construct requires that the type 'DoubleType<DoubleType<int option>>' is an unmanaged type" ]
   
    [<Fact>]
    let ``voption considered unmanaged when inner type is unmanaged`` () = 
        Fsx """
let test (x: 'T when 'T : unmanaged) = ()
test (ValueSome 15)
test (ValueSome (ValueSome 42))
test (ValueSome (struct {|Field = 42|}))
        """
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``voption not considered unmanaged when inner type is reference type`` () = 
        Fsx """
let test (x: 'T when 'T : unmanaged) = ()
test (ValueSome "xxx")
test (ValueSome (ValueSome "xxx"))
test (ValueSome (struct {|Field = Some 42|}))
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
                Error 1, Line 3, Col 17, Line 3, Col 22, "A generic construct requires that the type 'string' is an unmanaged type"
                Error 1, Line 4, Col 28, Line 4, Col 33, "A generic construct requires that the type 'string' is an unmanaged type"
                Error 1, Line 5, Col 35, Line 5, Col 42, "A generic construct requires that the type ''a option' is an unmanaged type" ]

    [<Fact>]
    let ``Option not considered unmanaged`` () = 
        Fsx """
let test (x: 'T when 'T : unmanaged) = ()
test (None)
test (Some 42 )
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
                        Error 1, Line 3, Col 7, Line 3, Col 11, "A generic construct requires that the type ''a option' is an unmanaged type"
                        Error 1, Line 4, Col 7, Line 4, Col 14, "A generic construct requires that the type ''a option' is an unmanaged type" ]

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
test(Result<int,int>.Ok 1)
test(Result<int,int>.Error 2)
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
let resultCreatingFunction (x:'a) = 
    try
        let capturedVal = Result<'a,int>.Ok x
        test capturedVal
        capturedVal
    with _ -> Error 15

let _ = resultCreatingFunction A

        """
        |> typecheck
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Typechecker can handle recursive unions passed in and will not stack overflow`` () =
        Fsx """
type RefTree =
    | Tip
    | Node of i:int * left:RefTree * right:RefTree

[<Struct>]
type StructTree =
    | Tip
    | Node of i:int * left:StructTree * right:StructTree

[<Struct>]
type ComboTree =
    | Tip
    | Node of i:int * left:RefTree * right:RefTree
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [Error 954, Line 7, Col 6, Line 7, Col 16, "This type definition involves an immediate cyclic reference through a struct field or inheritance relation"]

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
type MyDu<'T1,'T2> = DuA of first:'T1 | DuB of second:'T2
 
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
let _ = Test<MyDu<int,MyDu<int,string voption>>>()
        """
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
                       Error 1, Line 23, Col 6, Line 23, Col 33, "A generic construct requires that the type 'S<obj>' is an unmanaged type"
                       Error 193, Line 24, Col 6, Line 24, Col 14, "A generic construct requires that the type 'X<'a>' is an unmanaged type"
                       Error 193, Line 25, Col 7, Line 25, Col 20, "A generic construct requires that the type 'A<obj,int>' is an unmanaged type"
                       Error 193, Line 26, Col 24, Line 26, Col 36, "A generic construct requires that the type 'A<'T,obj>' is an unmanaged type"
                       Error 1, Line 27, Col 9, Line 27, Col 18, "A generic construct requires that the type 'obj' is an unmanaged type"
                       Error 1, Line 28, Col 9, Line 28, Col 28, "A generic construct requires that the type 'NonStructRecd' is an unmanaged type"
                       Error 1, Line 29, Col 9, Line 29, Col 49, "A generic construct requires that the type 'string' is an unmanaged type" ]

    [<Fact>]
    let ``Disallow both 'unmanaged' and 'not struct' constraints`` () =
        Fsx "type X<'T when 'T: unmanaged and 'T: not struct> = class end"
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
        (Error 43, Line 1, Col 34, Line 1, Col 48, "The constraints 'unmanaged' and 'not struct' are inconsistent")]

    [<FactForNETCOREAPP>]
    let ``Multi constraint IL test together with struct and interface constraints`` () =
        Fsx "[<Struct;NoEquality;NoComparison>] type Test<'T when 'T: unmanaged and 'T: struct and 'T:>System.IComparable> = struct end"
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
    .class public abstract auto ansi sealed Test
           extends [runtime]System.Object
    {
      .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 07 00 00 00 00 00 ) 
      .class sequential ansi serializable sealed nested public beforefieldinit Test`1<valuetype (class [runtime]System.ValueType modreq([runtime]System.Runtime.InteropServices.UnmanagedType), [runtime]System.IComparable) T>
             extends [runtime]System.ValueType
      {
        .pack 0
        .size 1
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.StructAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoEqualityAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.NoComparisonAttribute::.ctor() = ( 01 00 00 00 ) 
        .custom instance void [FSharp.Core]Microsoft.FSharp.Core.CompilationMappingAttribute::.ctor(valuetype [FSharp.Core]Microsoft.FSharp.Core.SourceConstructFlags) = ( 01 00 03 00 00 00 00 00 ) 
        .param type T 
          .custom instance void [runtime]System.Runtime.CompilerServices.IsUnmanagedAttribute::.ctor() = ( 01 00 00 00 ) 
      } 
    
    } """]

    [<FactForNETCOREAPP>]
    let ``IsUnmanagedAttribute Attribute is emitted for function with unmanaged constraint`` () =
        Fsx "let testMyFunction (x: 'TUnmanaged when 'TUnmanaged : unmanaged) = struct(x,1)"
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
    .method public static valuetype [runtime]System.ValueTuple`2<!!TUnmanaged,int32> 
              testMyFunction<valuetype (class [runtime]System.ValueType modreq([runtime]System.Runtime.InteropServices.UnmanagedType)) TUnmanaged>(!!TUnmanaged x) cil managed
      {
        .param type TUnmanaged 
          .custom instance void [runtime]System.Runtime.CompilerServices.IsUnmanagedAttribute::.ctor() = ( 01 00 00 00 ) 
        
        .maxstack  8
        IL_0000:  ldarg.0
        IL_0001:  ldc.i4.1
        IL_0002:  newobj     instance void valuetype [runtime]System.ValueTuple`2<!!TUnmanaged,int32>::.ctor(!0,
                                                                                                                    !1)
        IL_0007:  ret
      }"""]



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
                |> withLangVersion80

        app
        |> compile
        |> shouldFail
        |> withDiagnostics [(Error 1, Line 3, Col 13, Line 3, Col 45, "A generic construct requires that the type 'string' is an unmanaged type")]

    [<Fact>]
    let ``F# can consume C#-defined unmanaged constraint and call method with modreq`` () = 
        let csLib =
            CSharp """
namespace CsLib
{ 
    public record struct CsharpStruct<T>(T item) where T:unmanaged
    {
        public static string Hi<TOther>() where TOther:unmanaged
            {
                return typeof(TOther).Name;
            }}}"""
            |> withCSharpLanguageVersion CSharpLanguageVersion.Preview
            |> withName "csLib"

        let app = FSharp """module MyFsharpApp
open CsLib
[<Struct>]
type MultiCaseUnion = A | B of i:int
let _ = new CsharpStruct<MultiCaseUnion>(B 42)
printf "%s" (CsharpStruct<int>.Hi<MultiCaseUnion>())
        """     |> withReferences [csLib]

        app
        |> withLangVersion80
        |> asExe
        |> compile
        |> run
        |> verifyOutput "MultiCaseUnion"

    [<FactForNETCOREAPP>]
    let ``FSharp generates modreq for CSharp to consume in preview`` () = 
        Fsx "let testMyFunction (x: 'TUnmanaged when 'TUnmanaged : unmanaged) = ()"
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
      .method public static void  testMyFunction<valuetype (class [runtime]System.ValueType modreq([runtime]System.Runtime.InteropServices.UnmanagedType)) TUnmanaged>(!!TUnmanaged x) cil managed
  {
    .param type TUnmanaged 
      .custom instance void [runtime]System.Runtime.CompilerServices.IsUnmanagedAttribute::.ctor() = ( 01 00 00 00 ) 
    
    .maxstack  8
    IL_0000:  ret
  } """]

    [<Fact>]
    let ``FSharp does not generate modreq for VBNET to consume in v7`` () = 
        Fsx "let testMyFunction (x: 'TUnmanaged when 'TUnmanaged : unmanaged) = ()"
        |> withLangVersion70
        |> compile
        |> shouldSucceed
        |> verifyIL ["""
      .method public static void  testMyFunction<TUnmanaged>(!!TUnmanaged x) cil managed
  {
    
    .maxstack  8
    IL_0000:  ret
  } """]


    [<Fact>]
    let ``C# can consume F#-defined struct with unmanaged constraint - valid`` () =
        let fsharpLib = 
            Fs """namespace MyFsLib
[<Struct>]
type FsharpStructWrapper<'TInner when 'TInner: unmanaged> = 
    val Item : 'TInner   
    with static member Hi() = typeof<'TInner>.Name"""
            |> asLibrary
            |> withName "fsLib"

        let app = 
            CSharp """
    using System;
    using MyFsLib;
    public record struct CSharpStruct(int first, long second);
    public class C {
        public static void Main() {
            var text = FsharpStructWrapper<CSharpStruct>.Hi();
            Console.Write(text);
        }
    }
            """
            |> withReferences [fsharpLib]
            |> withCSharpLanguageVersion CSharpLanguageVersion.Preview
            |> asExe
            |> withName "myCsharpApp"

        app
        |> compile
        |> run
        |> verifyOutput "CSharpStruct"

    [<Fact>]
    let ``C# can consume F#-defined struct with unmanaged constraint - and report error when invalid`` () =
        let fsharpLib = 
            Fs """namespace MyFsLib
[<Struct>]
type FsharpStructWrapper<'T when 'T: unmanaged> = 
    val Item : 'T    
    with static member Hi() = typeof<'T>.Name

[<Struct>]
type MultiDu = A | B of s:string

module FsharpFunc = 
    let test (x: 'T when 'T : unmanaged) = ()

module FsharpPreparedData = 
    let structTuple = struct(1,2,"42")
    let structDuExample1 = A
    let structDuExample2 = B "42"
    """
            |> asLibrary
            |> withLangVersionPreview
            |> withName "fsLib"

        let app = 
            CSharp """
    using System;
    using MyFsLib;
    public record struct CSharpStructWithString(int first, string second);
    public class C {
        public static void Main() {
            var text = FsharpStructWrapper<CSharpStructWithString>.Hi();
            FsharpFunc.test(FsharpPreparedData.structTuple);
            FsharpFunc.test(FsharpPreparedData.structDuExample1);
            FsharpFunc.test(FsharpPreparedData.structDuExample2);
            Console.WriteLine(text);
        }
    }
            """
            |> withReferences [fsharpLib]
            |> withCSharpLanguageVersion CSharpLanguageVersion.Preview
            |> asExe
            |> withName "myCsharpApp"

        app
        |> compile
        |> shouldFail
        |> withDiagnostics [
                 Error 8377, Line 6, Col 44, Line 6, Col 66, "The type 'CSharpStructWithString' must be a non-nullable value type, along with all fields at any level of nesting, in order to use it as parameter 'T' in the generic type or method 'FsharpStructWrapper<T>'"
                 Error 8377, Line 7, Col 24, Line 7, Col 28, "The type '(int, int, string)' must be a non-nullable value type, along with all fields at any level of nesting, in order to use it as parameter 'T' in the generic type or method 'FsharpFunc.test<T>(T)'"
                 Error 8377, Line 8, Col 24, Line 8, Col 28, "The type 'MultiDu' must be a non-nullable value type, along with all fields at any level of nesting, in order to use it as parameter 'T' in the generic type or method 'FsharpFunc.test<T>(T)'"
                 Error 8377, Line 9, Col 24, Line 9, Col 28, "The type 'MultiDu' must be a non-nullable value type, along with all fields at any level of nesting, in order to use it as parameter 'T' in the generic type or method 'FsharpFunc.test<T>(T)'" ]