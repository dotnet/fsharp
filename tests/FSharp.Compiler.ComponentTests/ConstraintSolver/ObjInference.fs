namespace FSharp.Compiler.ComponentTests.ConstraintSolver

open Xunit
open FSharp.Test.Compiler

module ObjInference =

    let message = "A type inference variable has been implicitly inferred to have type `obj`. Consider adding explicit type annotations. This warning is off by default and has been explicitly enabled for this project. You may suppress this warning by using #nowarn \"3525\"."

    let warningCases =
        [
            // TODO: for this case, we're definitely emitting the warning (according to the debugger),
            // but somehow it's not showing up in the output?
            """let f<'b> () : 'b = (let a = failwith "" in unbox a)""", 1, 1, 1, 1
            "let f() = ([] = [])", 1, 17, 1, 19
            """System.Object.ReferenceEquals(null, "hello") |> ignore""", 1, 31, 1, 35
            """System.Object.ReferenceEquals("hello", null) |> ignore""", 1, 40, 1, 44
        ]
        |> List.map (fun (str, line1, col1, line2, col2) -> [| box str ; line1 ; col1 ; line2 ; col2 |])

    [<Theory>]
    [<MemberData(nameof(warningCases))>]
    let ``Warning is emitted when type Obj is inferred``(code: string, line1: int, col1: int, line2: int, col2: int) =
        FSharp code
        |> withErrorRanges
        |> withWarnOn 3525
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Warning 3525, Line line1, Col col1, Line line2, Col col2, message)

    let noWarningCases =
        [
            "let add x y = x + y" // inferred as int
            "let f x = string x" // inferred as generic 'a -> string
            "let f() = ([] = ([] : obj list))" // obj is inferred, but is annotated
            "let f() = (([] : obj list) = [])" // obj is inferred, but is annotated
            """let x<[<Measure>]'m> : int<'m> = failwith ""
let f () = x = x |> ignore""" // measure is inferred as 1, but that's not covered by this warning
            "let a = 5 |> unbox<obj> in let b = a in ()" // explicit obj annotation
        ]
        |> List.map Array.singleton

    [<Theory>]
    [<MemberData(nameof(noWarningCases))>]
    let ``Warning does not fire unless required``(code: string) =
        FSharp code
        |> withWarnOn 3525
        |> typecheck
        |> shouldSucceed

    let nullNoWarningCases =
        [
            """System.Object.ReferenceEquals("hello", (null: string))"""
            """System.Object.ReferenceEquals((null: string), "hello")"""
        ]
        |> List.map Array.singleton

    [<Theory>]
    [<MemberData(nameof(nullNoWarningCases))>]
    let ``Don't warn on an explicit null``(expr: string) =
        sprintf "%s |> ignore" expr
        |> FSharp
        |> withWarnOn 3525
        |> typecheck
        |> shouldSucceed

    [<Theory>]
    [<MemberData(nameof(nullNoWarningCases))>]
    let ``Don't warn on an explicit null, inside quotations``(expr: string) =
        sprintf "<@ %s @> |> ignore" expr
        |> FSharp
        |> withWarnOn 3525
        |> typecheck
        |> shouldSucceed

    let quotationNoWarningCases =
        [
            "<@ List.map ignore [1;2;3] @>"
        ]
        |> List.map Array.singleton

    [<Theory>]
    [<MemberData(nameof(quotationNoWarningCases))>]
    let ``Don't warn inside quotations of acceptable code``(expr: string) =
        sprintf "%s |> ignore" expr
        |> FSharp
        |> withWarnOn 3525
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``Warn when the error appears inside a quotation``() =
        "<@ [] = [] @> |> ignore"
        |> FSharp
        |> withWarnOn 3525
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Warning 3525, Line 1, Col 9, Line 1, Col 11, message)
