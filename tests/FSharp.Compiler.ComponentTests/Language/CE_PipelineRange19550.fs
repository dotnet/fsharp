// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open Xunit
open FSharp.Test.Compiler

// https://github.com/dotnet/fsharp/issues/19550
module CE_PipelineRange19550 =

    let private errorsOf (result: CompilationResult) : ErrorInfo list =
        let diags =
            match result with
            | CompilationResult.Success r
            | CompilationResult.Failure r -> r.Diagnostics
        diags
        |> List.filter (fun d ->
            match d.Error with ErrorType.Error _ -> true | _ -> false)

    let private dump (result: CompilationResult) : string =
        errorsOf result
        |> List.map (fun d ->
            let n =
                match d.Error with
                | ErrorType.Error n
                | ErrorType.Warning n
                | ErrorType.Information n
                | ErrorType.Hidden n -> n
            sprintf "  FS%04d (L%d,C%d)-(L%d,C%d): %s"
                n d.Range.StartLine (d.Range.StartColumn + 1)
                d.Range.EndLine (d.Range.EndColumn + 1) d.Message)
        |> String.concat "\n"

    let private hasDiagAt (code: int) (sLine, sCol, eLine, eCol) (result: CompilationResult) : CompilationResult =
        let found =
            errorsOf result
            |> List.exists (fun d ->
                (match d.Error with ErrorType.Error c -> c = code | _ -> false)
                && d.Range.StartLine = sLine
                && d.Range.StartColumn + 1 = sCol
                && d.Range.EndLine = eLine
                && d.Range.EndColumn + 1 = eCol)
        if not found then
            failwithf
                "Expected diagnostic FS%04d at (Line %d, Col %d)-(Line %d, Col %d). Actual diagnostics:\n%s"
                code sLine sCol eLine eCol (dump result)
        result

    let private hasNoRange0Error (result: CompilationResult) : CompilationResult =
        let bad =
            errorsOf result
            |> List.filter (fun d ->
                d.Range.StartLine = 1 && d.Range.StartColumn = 0
                && d.Range.EndLine = 1 && d.Range.EndColumn = 0)
        if not (List.isEmpty bad) then
            failwithf
                "Unexpected range0 / unknown(1,1) error diagnostic. All diagnostics:\n%s"
                (dump result)
        result

    [<Fact>]
    let ``Issue 19550 - empty CE body in pipeline reports source range``() =
        FSharp """
module Repro19550_01
type FooBuilder() =
  member _.Zero() = fun x -> x + 42

let foo = FooBuilder()

"" |> foo {} |> printfn "%d"
        """
        |> compile
        |> shouldFail
        |> hasDiagAt 193 (8, 7, 8, 13)

    [<Fact>]
    let ``Issue 19550 - single pipe with empty CE body has non-zero range``() =
        FSharp """
module Repro19550_02
type FooBuilder() =
  member _.Zero() = fun x -> x + 42

let foo = FooBuilder()

foo {} |> printfn "%d"
        """
        |> compile
        |> shouldFail
        |> hasNoRange0Error

    [<Fact>]
    let ``Issue 19550 - empty CE body as function argument``() =
        FSharp """
module Repro19550_03
type FooBuilder() =
  member _.Zero() = fun x -> x + 42

let foo = FooBuilder()

let take (x: int) = x
let _ = take (foo {})
        """
        |> compile
        |> shouldFail
        |> hasDiagAt 193 (9, 15, 9, 21)

    [<Fact>]
    let ``Issue 19550 - explicit type annotation mismatch on empty CE body``() =
        FSharp """
module Repro19550_04
type FooBuilder() =
  member _.Zero() = fun x -> x + 42

let foo = FooBuilder()

let x : int = foo {}
        """
        |> compile
        |> shouldFail
        |> hasDiagAt 193 (8, 15, 8, 21)

    [<Fact>]
    let ``Issue 19550 - Yield builder in pipeline keeps non-zero range``() =
        FSharp """
module Repro19550_05a
type YBuilder() =
  member _.Yield(x) = fun y -> x + y

let yb = YBuilder()

"" |> yb { yield 1 } |> printfn "%d"
        """
        |> compile
        |> shouldFail
        |> hasNoRange0Error

    [<Fact>]
    let ``Issue 19550 - Return builder in pipeline keeps non-zero range``() =
        FSharp """
module Repro19550_05b
type RBuilder() =
  member _.Return(x) = fun y -> x + y

let rb = RBuilder()

"" |> rb { return 1 } |> printfn "%d"
        """
        |> compile
        |> shouldFail
        |> hasNoRange0Error

    [<Fact>]
    let ``Issue 19550 - Bind builder in pipeline keeps non-zero range``() =
        FSharp """
module Repro19550_05c
type BBuilder() =
  member _.Bind(x, f) = f x
  member _.Return(x) = fun y -> x + y

let bb = BBuilder()

"" |> bb { let! x = 1 in return x } |> printfn "%d"
        """
        |> compile
        |> shouldFail
        |> hasNoRange0Error

    [<Fact>]
    let ``Issue 19550 - nested CE - inner empty body in pipeline``() =
        FSharp """
module Repro19550_06
type FooBuilder() =
  member _.Zero() = fun x -> x + 42

let foo = FooBuilder()

type Outer() =
  member _.Yield(_) : unit = ()
  member _.Combine(_, _) = ()
  member _.Delay(f: unit -> unit) = f ()
  member _.Zero() = ()

let outer = Outer()

let _ = outer { do "" |> foo {} |> ignore }
        """
        |> compile
        |> shouldFail
        |> hasDiagAt 193 (16, 26, 16, 32)

    [<Fact>]
    let ``Issue 19550 - valid empty CE body without type mismatch still compiles``() =
        FSharp """
module Repro19550_07
type IdBuilder() =
  member _.Zero() = 0

let id1 = IdBuilder()
let v : int = id1 {}
        """
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``Issue 19550 - non-pipeline empty CE body reports source range``() =
        FSharp """
module Repro19550_08
type FooBuilder() =
  member _.Zero() = fun x -> x + 42

let foo = FooBuilder()

let f : string -> string = foo {}
        """
        |> compile
        |> shouldFail
        |> hasDiagAt 193 (8, 28, 8, 34)

    [<Fact>]
    let ``Issue 19550 - non-CE pipeline mismatch keeps non-zero range``() =
        FSharp """
module Repro19550_09
let _ = "" |> (fun (x: int) -> x) |> printfn "%d"
        """
        |> compile
        |> shouldFail
        |> hasNoRange0Error

    [<Fact>]
    let ``Issue 19550 - multi-line empty CE body in pipeline``() =
        FSharp """
module Repro19550_10
type FooBuilder() =
  member _.Zero() = fun x -> x + 42

let foo = FooBuilder()

"" |>
   foo {
   } |>
   printfn "%d"
        """
        |> compile
        |> shouldFail
        |> hasDiagAt 193 (9, 4, 10, 5)

    [<Fact>]
    let ``Issue 19550 - empty CE body in match arm pipeline``() =
        FSharp """
module Repro19550_11
type FooBuilder() =
  member _.Zero() = fun x -> x + 42

let foo = FooBuilder()

let r = match 0 with
        | _ -> "" |> foo {} |> printfn "%d"
        """
        |> compile
        |> shouldFail
        |> hasDiagAt 193 (9, 22, 9, 28)

    [<Fact>]
    let ``Issue 19550 - both Zero and Yield - empty body picks Zero path``() =
        FSharp """
module Repro19550_12
type BothBuilder() =
  member _.Zero() = fun x -> x + 1
  member _.Yield(x) = fun y -> x + y

let bb = BothBuilder()

"" |> bb {} |> printfn "%d"
        """
        |> compile
        |> shouldFail
        |> hasDiagAt 193 (9, 7, 9, 12)
