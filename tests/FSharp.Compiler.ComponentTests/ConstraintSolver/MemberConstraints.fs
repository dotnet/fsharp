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
