// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Language

open Xunit
open FSharp.Test.Compiler
open FSharp.Quotations.Patterns

module CodeQuotationsTests =

    [<Fact>]
    let ``Quotation on op_UnaryPlus(~+) compiles and runs`` () =
        Fsx """
open FSharp.Linq.RuntimeHelpers
open FSharp.Quotations.Patterns
open FSharp.Quotations.DerivedPatterns

let eval q = LeafExpressionConverter.EvaluateQuotation q

let inline f x = <@ (~+) x @>
let x = <@ f 1 @>
let y : unit =
    match f 1 with
    | Call(_, methInfo, _) when methInfo.Name = "op_UnaryPlus" ->
        ()
    | e ->
        failwithf "did not expect expression for 'y': %A" e
let z : unit =
    match f 5 with
    | (CallWithWitnesses(_, methInfo, methInfoW, _, _) as e) when methInfo.Name = "op_UnaryPlus" && methInfoW.Name = "op_UnaryPlus$W" ->
        if ((eval e) :?> int) = 5 then
            ()
        else
            failwith "did not expect evaluation false"
    | e ->
        failwithf "did not expect expression for 'z': %A" e
        """
        |> asExe
        |> withLangVersion50
        |> compileAndRun
        |> shouldSucceed
