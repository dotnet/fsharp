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

    [<Fact>]
    let ``Extension binary operator does not report duplicate candidates`` () =
        // Regression test: binary operators with same support type (e.g., list<_>) should not report duplicates
        FSharp """
open FSharp.Core.CompilerServices

type List<'t> with
    static member (<*>) (f: list<'T -> 'U>, x: list<'T>) : list<'U> =
        let mutable coll = ListCollector<'U> ()
        f |> List.iter (fun f ->
            x |> List.iter (fun x ->
                coll.Add (f x)))
        coll.Close ()

let result = [(+)] <*> [1;10] <*> [2;3]
"""
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``Nested inline SRTP with multiple overloads should not cause internal error`` () =
        // Regression test: unsolved type variables in trait constraint solutions during codegen
        // caused FS0073 "internal error: Undefined or unsolved type variable" when an inline
        // SRTP function was wrapped in another SRTP dispatch layer with multiple overloads.
        FSharp
            """
type App<'F, 'a> = | App of 'F * 'a

type LA = LA
type LB = LB

type D =
    static member inline Pur(_witness: App<LA, _>, x: 'a) : App<LA, 'a> = App(LA, x)
    static member inline Pur(_witness: App<LB, _>, x: 'a) : App<LB, list<'a>> = App(LB, [x])

let inline pur_impl (_mthd: ^M, output: ^F, x: 'a) : ^F
    when (^M or ^F) : (static member Pur : ^F * 'a -> ^F) =
    ((^M or ^F) : (static member Pur : ^F * 'a -> ^F) (output, x))

let inline pur (x: 'a) : ^F =
    pur_impl (Unchecked.defaultof<D>, Unchecked.defaultof< ^F>, x)

type D with
    static member inline Invoke(_witness: App<LA, _>, f: App<LA, 'a -> 'b>, x: App<LA, 'a>) : App<LA, 'b> =
        let (App(_, fv)) = f
        let (App(_, xv)) = x
        App(LA, fv xv)
    static member inline Invoke(_witness: App<LB, _>, f: App<LB, list<'a -> 'b>>, x: App<LB, list<'a>>) : App<LB, list<'b>> =
        let (App(_, fv)) = f
        let (App(_, xv)) = x
        App(LB, List.map2 (fun f x -> f x) fv xv)

let inline invoke_impl (_mthd: ^M, output: ^R, f: ^FF, x: ^FX) : ^R
    when (^M or ^R) : (static member Invoke : ^R * ^FF * ^FX -> ^R) =
    ((^M or ^R) : (static member Invoke : ^R * ^FF * ^FX -> ^R) (output, f, x))

let inline invoke (f: ^FF) (x: ^FX) : ^R =
    invoke_impl (Unchecked.defaultof<D>, Unchecked.defaultof< ^R>, f, x)

[<EntryPoint>]
let main _ =
    // Test pur with two overloads (Pur has wildcard _ in App<LA, _>)
    let (App(LA, v)) : App<LA, int> = pur 1
    if v <> 1 then failwith "pur failed"

    // Test invoke with two overloads (Invoke has wildcard _ in App<LA, _>)
    let f : App<LA, int -> int> = pur (fun x -> x + 1)
    let x : App<LA, int> = pur 2
    let (App(LA, r)) : App<LA, int> = invoke f x
    if r <> 3 then failwith "invoke failed"
    0
            """
        |> asExe
        |> compileExeAndRun
        |> shouldSucceed

    // Regression for PR #19602 (RFC FS-1043): a non-inline binding with an unsatisfiable operator/SRTP
    // trait must fail at compile time (FS0041), not compile into a NotSupportedException stub that throws
    // at runtime (which also leaked at feature-off langversions). The deleted neg116 shape '(1.0 - t) * p'
    // stages the outer trait into a free return typar on a non-inline value.
    let private nonInlineUnsatisfiableOperatorSrtp = """
module Neg116

type Complex = unit

type Polynomial () =
    static member (*) (s: decimal, p: Polynomial) : Polynomial = failwith ""
    static member (*) (s: Complex, p: Polynomial) : Polynomial = failwith ""

module Foo =
    let test t (p: Polynomial) = (1.0 - t) * p
"""

    [<Theory>]
    [<InlineData("9.0")>]
    [<InlineData("preview")>]
    let ``Non-inline binding with unsatisfiable operator SRTP is rejected at compile time`` (langVersion: string) =
        FSharp nonInlineUnsatisfiableOperatorSrtp
        |> asLibrary
        |> withLangVersion langVersion
        |> compile
        |> shouldFail
        |> withErrorCode 41
        |> withDiagnosticMessageMatches "No overloads match"
        |> withDiagnosticMessageMatches "op_Multiply"
        |> ignore

    // Regression for PR #19602 (RFC FS-1043): a return-type-directed multi-overload SRTP dispatch
    // (FSharpPlus-style '(^a or ^b or ^c) : Transform') that no overload can satisfy must fail at
    // compile time (FS0041), not compile into a NotSupportedException stub. Deleted neg117 shape.
    let private returnDirectedMultiOverloadUnsatisfiableSrtp = """
module Neg117

#nowarn "64" // This construct causes code to be less generic than indicated by the type annotations.

module TargetA =

    [<RequireQualifiedAccess>]
    type TransformerKind =
        | A
        | B

    type M1 = int

    type M2 = float

    type Target() =

        member __.TransformM1 (kind: TransformerKind) : M1[] option = [| 0 |] |> Some
        member __.TransformM2 (kind: TransformerKind) : M2[] option = [| 1. |] |> Some

    type TargetA =

        static member instance : Target option = None

        static member inline Transform(_: ^r, _: TargetA) = fun (kind:TransformerKind) -> TargetA.instance.Value.TransformM1 kind : ^r
        static member inline Transform(_: ^r, _: TargetA) = fun (kind:TransformerKind) ->  TargetA.instance.Value.TransformM2 kind : ^r

        static member inline Transform(kind: TransformerKind) =
            let inline call2(a:^a, b:^b) = ((^a or ^b) : (static member Transform: _ * _ -> _) b, a)
            let inline call (a: 'a) = fun (x: 'x) -> call2(a, Unchecked.defaultof<'r>) x : 'r
            call Unchecked.defaultof<TargetA> kind

    let inline Transform kind = TargetA.Transform kind

module TargetB =
    [<RequireQualifiedAccess>]
    type TransformerKind =
        | C
        | D

    type M1 = | M1

    type M2 = | M2

    type Target() =

        member __.TransformM1 (kind: TransformerKind) = [| M1 |] |> Some
        member __.TransformM2 (kind: TransformerKind) = [| M2 |] |> Some

    type TargetB =

        static member instance : Target option = None
    
        static member inline Transform(_: ^r, _: TargetB) = fun (kind:TransformerKind) -> TargetB.instance.Value.TransformM1 kind : ^r
        static member inline Transform(_: ^r, _: TargetB) = fun (kind:TransformerKind) -> TargetB.instance.Value.TransformM2 kind : ^r

        static member inline Transform(kind: TransformerKind) =
            let inline call2(a:^a, b:^b) = ((^a or ^b) : (static member Transform: _ * _ -> _) b, a)
            let inline call (a: 'a) = fun (x: 'x) -> call2(a, Unchecked.defaultof<'r>) x : 'r
            call Unchecked.defaultof<TargetB> kind
    let inline Transform kind = TargetB.Transform kind

module Superpower =

    type Transformer =
        
        static member inline Transform(_: ^f, _: TargetB.TargetB, _: Transformer) =
            fun x -> TargetB.Transform x : ^f
        
        static member inline Transform(_: ^r, _: TargetA.TargetA, _: Transformer) =
           fun x -> TargetA.Transform x : ^r

        static member inline YeahTransform kind =
            let inline call2(a:^a, b:^b, c: ^c) = ((^a or ^b or ^c) : (static member Transform: _ * _ * _ -> _) c, b, a)
            let inline call (a: 'a) = fun (x: 'x) -> call2(a, Unchecked.defaultof<_>, Unchecked.defaultof<'r>) x : 'r
            call Unchecked.defaultof<Transformer> kind 

module Examples =
    let a kind = Superpower.Transformer.YeahTransform kind : TargetA.M1[]
"""

    [<Theory>]
    [<InlineData("9.0")>]
    [<InlineData("preview")>]
    let ``Return-directed multi-overload unsatisfiable SRTP is rejected at compile time`` (langVersion: string) =
        FSharp returnDirectedMultiOverloadUnsatisfiableSrtp
        |> asLibrary
        |> withLangVersion langVersion
        |> compile
        |> shouldFail
        |> withErrorCode 41
        |> withDiagnosticMessageMatches "No overloads match"
        |> withDiagnosticMessageMatches "Transform"
        |> ignore
