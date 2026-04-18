// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace ConstraintSolver

open Xunit
open FSharp.Test.Compiler

module MemberConstraints =

    [<Fact>]
    let ``Invalid member constraint with ErrorRanges``() = // Regression test for FSharp1.0:2262
        FSharp """
 let inline length (x: ^a) : int = (^a : (member Length : int with get, set) (x, ()))
        """
        |> withErrorRanges
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Error 697, Line 2, Col 43, Line 2, Col 76, "Invalid constraint")

    [<Fact>]
    let ``We can overload operators on a type and not add all the extra jazz such as inlining and the ^ operator.``() =

        FSharp """
type Foo(x : int) =
    member this.Val = x

    static member (-->) ((src : Foo), (target : Foo)) = new Foo(src.Val + target.Val)
    static member (-->) ((src : Foo), (target : int)) = new Foo(src.Val + target)

    static member (+) ((src : Foo), (target : Foo)) = new Foo(src.Val + target.Val)
    static member (+) ((src : Foo), (target : int)) = new Foo(src.Val + target)

let x = Foo(3) --> 4
let y = Foo(3) --> Foo(4)
let x2 = Foo(3) + 4
let y2 = Foo(3) + Foo(4)

if x.Val <> 7 then failwith "x.Val <> 7"
elif y.Val <> 7 then  failwith "y.Val <> 7"
elif x2.Val <> 7 then  failwith "x2.Val <> 7"
elif y2.Val <> 7 then  failwith "x.Val <> 7"
else ()
"""
        |> asExe
        |> compile
        |> run
        |> shouldSucceed

    [<Fact>]
    let ``Respect nowarn 957 for extension method`` () =
        FSharp """        
module Foo

type DataItem<'data> =
    { Identifier: string
      Label: string
      Data: 'data }

    static member Create<'data>(identifier: string, label: string, data: 'data) =
        { DataItem.Identifier = identifier
          DataItem.Label = label
          DataItem.Data = data }

#nowarn "957"

type DataItem< ^input> with

    static member inline Create(item: ^input) =
        let stringValue: string = (^input: (member get_StringValue: unit -> string) (item))
        let friendlyStringValue: string = (^input: (member get_FriendlyStringValue: unit -> string) (item))

        DataItem.Create< ^input>(stringValue, friendlyStringValue, item)
"""
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``Indirect constraint by operator`` () =
        FSharp """
List.average [42] |> ignore
"""
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic
            (Error 1, Line 2, Col 15, Line 2, Col 17, "'List.average' does not support the type 'int', because the latter lacks the required (real or built-in) member 'DivideByInt'")

    [<Fact>]
    let ``Direct constraint by named (pseudo) operator`` () =
        FSharp """
abs -1u |> ignore
"""
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic
            (Error 1, Line 2, Col 6, Line 2, Col 8, "The type 'uint32' does not support the operator 'abs'")

    [<Fact>]
    let ``Direct constraint by simple operator`` () =
        FSharp """
"" >>> 1 |> ignore
"""
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic
            (Error 1, Line 2, Col 1, Line 2, Col 3, "The type 'string' does not support the operator '>>>'")

    [<Fact>]
    let ``Direct constraint by pseudo operator`` () =
        FSharp """
ignore ["1" .. "42"]
"""
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic
            (Error 1, Line 2, Col 9, Line 2, Col 12, "The type 'string' does not support the operator 'op_Range'")

    // https://github.com/dotnet/fsharp/issues/12386
    [<Fact>]
    let ``Issue 12386 - SRTP trait call should resolve correct overload at runtime`` () =
        FSharp
            """
type A =
    | A
    static member ($) (A, _a: float) = 0.0
    static member ($) (A, _a: decimal) = 0M
    static member ($) (A, _a: 't) = 0

let inline call x = ($) A x

[<EntryPoint>]
let main _ =
    let resultFloat = call 42.0
    let resultDecimal = call 42M
    let resultInt = call 42
    if resultFloat <> 0.0 then failwithf "Expected 0.0 but got %A" resultFloat
    if resultDecimal <> 0M then failwithf "Expected 0M but got %A" resultDecimal
    if resultInt <> 0 then failwithf "Expected 0 but got %A" resultInt
    printfn "All SRTP overload resolutions correct"
    0
            """
        |> asExe
        |> compileExeAndRun
        |> shouldSucceed

    // https://github.com/dotnet/fsharp/issues/6648
    [<Fact>]
    let ``Issue 6648 - DU of DUs with inline static members should compile`` () =
        FSharp
            """
type SomeUnion1<'T> =
    | Case1A of 'T
    | Case1B of 'T
    static member inline (-) (a, b) =
        match a, b with
        | Case1A x, Case1A y -> Case1A(x - y)
        | Case1B x, Case1B y -> Case1B(x - y)
        | _ -> failwith "mismatch"

type SomeUnion2<'T> =
    | Case2A of 'T
    | Case2B of 'T
    static member inline (-) (a, b) =
        match a, b with
        | Case2A x, Case2A y -> Case2A(x - y)
        | Case2B x, Case2B y -> Case2B(x - y)
        | _ -> failwith "mismatch"

type UnionOfUnions<'T> =
    | ParentCase1 of SomeUnion1<'T>
    | ParentCase2 of SomeUnion2<'T>
    static member inline (-) (a, b) =
        match a, b with
        | ParentCase1 x, ParentCase1 y -> x - y |> ParentCase1
        | ParentCase2 x, ParentCase2 y -> x - y |> ParentCase2
        | _ -> failwith "mismatch"
            """
        |> asLibrary
        |> typecheck
        |> shouldSucceed

    // https://github.com/dotnet/fsharp/issues/9878
    [<Fact>]
    let ``Issue 9878 - SRTP with phantom type parameter should compile`` () =
        FSharp
            """
type DuCaseName<'T> =
    static member ToCaseName<'t, 'u>(value: 't) = failwith "delayed resolution"
    static member ToCaseName(value: 'T) =
        match FSharp.Reflection.FSharpValue.GetUnionFields(value, typeof<'T>) with case, _ -> case.Name
    static member inline Invoke(value: 'a) =
        let inline call (other: ^M, value: ^I) = ((^M or ^I) : (static member ToCaseName: ^I -> string) value)
        call (Unchecked.defaultof<DuCaseName<_>>, value)
            """
        |> asLibrary
        |> typecheck
        |> shouldSucceed

    // https://github.com/dotnet/fsharp/issues/9382
    [<Fact>]
    let ``Issue 9382 - SRTP stress test with matrix inverse should compile`` () =
        FSharp
            """
type Matrix<'a> =
    { m11: 'a; m12: 'a; m13: 'a
      m21: 'a; m22: 'a; m23: 'a
      m31: 'a; m32: 'a; m33: 'a }

    static member inline (/) (m, s) =
        { m11 = m.m11 / s; m12 = m.m12 / s; m13 = m.m13 / s
          m21 = m.m21 / s; m22 = m.m22 / s; m23 = m.m23 / s
          m31 = m.m31 / s; m32 = m.m32 / s; m33 = m.m33 / s }

    static member inline (*) (a, b) =
        { m11 = a.m11 * b.m11 + a.m12 * b.m21 + a.m13 * b.m31
          m12 = a.m11 * b.m12 + a.m12 * b.m22 + a.m13 * b.m32
          m13 = a.m11 * b.m13 + a.m12 * b.m23 + a.m13 * b.m33
          m21 = a.m21 * b.m11 + a.m22 * b.m21 + a.m23 * b.m31
          m22 = a.m21 * b.m12 + a.m22 * b.m22 + a.m23 * b.m32
          m23 = a.m21 * b.m13 + a.m22 * b.m23 + a.m23 * b.m33
          m31 = a.m31 * b.m11 + a.m32 * b.m21 + a.m33 * b.m31
          m32 = a.m31 * b.m12 + a.m32 * b.m22 + a.m33 * b.m32
          m33 = a.m31 * b.m13 + a.m32 * b.m23 + a.m33 * b.m33 }

let inline determinant m =
    m.m11 * m.m22 * m.m33 + m.m12 * m.m23 * m.m31 + m.m13 * m.m21 * m.m32
    - m.m13 * m.m22 * m.m31 - m.m12 * m.m21 * m.m33 - m.m11 * m.m23 * m.m32

let inline inverse m =
    { m11 = m.m22 * m.m33 - m.m32 * m.m23
      m12 = m.m13 * m.m32 - m.m12 * m.m33
      m13 = m.m12 * m.m23 - m.m13 * m.m22
      m21 = m.m23 * m.m31 - m.m21 * m.m33
      m22 = m.m11 * m.m33 - m.m13 * m.m31
      m23 = m.m21 * m.m13 - m.m11 * m.m23
      m31 = m.m21 * m.m32 - m.m31 * m.m22
      m32 = m.m31 * m.m12 - m.m11 * m.m32
      m33 = m.m11 * m.m22 - m.m21 * m.m12 }
    / (determinant m)
            """
        |> typecheck
        |> shouldSucceed
