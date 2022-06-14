namespace FSharp.Compiler.ComponentTests.ConstraintSolver

open Xunit
open FSharp.Test.Compiler

module ObjInference =

    let failureCases =
        [
            "let f() = ([] = [])", 1, 17, 1, 19
        ]
        |> List.map (fun (str, line1, col1, line2, col2) -> [| box str ; line1 ; col1 ; line2 ; col2 |])

    [<Theory>]
    [<MemberData(nameof(failureCases))>]
    let ``Inference of obj``(code: string, line1: int, col1: int, line2: int, col2: int) =
        FSharp code
        |> withErrorRanges
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Warning 3524, Line line1, Col col1, Line line2, Col col2, "A type was not refined away from `obj`, which may be unintended. Consider adding explicit type annotations.")

    let nullSuccessCases =
        [
            """System.Object.ReferenceEquals("hello", null)"""
            """System.Object.ReferenceEquals("hello", (null: string))"""
            """System.Object.ReferenceEquals(null, "hello")"""
            """System.Object.ReferenceEquals((null: string), "hello")"""
        ]
        |> List.map Array.singleton

    [<Theory>]
    [<MemberData(nameof(nullSuccessCases))>]
    let ``Don't warn on an explicit null``(expr: string) =
        sprintf "%s |> ignore" expr
        |> FSharp
        |> typecheck
        |> shouldSucceed

    [<Theory>]
    [<MemberData(nameof(nullSuccessCases))>]
    let ``Don't warn inside quotations, explicit nulls``(expr: string) =
        sprintf "<@ %s @> |> ignore" expr
        |> FSharp
        |> typecheck
        |> shouldSucceed

    let quotationSuccessCases =
        [
            "<@ List.map ignore [1;2;3] @>"
        ]
        |> List.map Array.singleton

    [<Theory>]
    [<MemberData(nameof(quotationSuccessCases))>]
    let ``Don't warn inside quotations``(expr: string) =
        sprintf "%s |> ignore" expr
        |> FSharp
        |> typecheck
        |> shouldSucceed
