namespace FSharp.Compiler.ComponentTests.ConstraintSolver

open Xunit
open FSharp.Test.Compiler

module ObjInference =

    let message = "A type has been implicitly inferred as 'obj', which may be unintended. Consider adding explicit type annotations. You can disable this warning by using '#nowarn \"3559\"' or '--nowarn:3559'."

    let quotableWarningCases =
        [
            """System.Object.ReferenceEquals(null, "hello") |> ignore""", 1, 31, 1, 35
            """System.Object.ReferenceEquals("hello", null) |> ignore""", 1, 40, 1, 44
            "([] = []) |> ignore", 1, 7, 1, 9
            "<@ [] = [] @> |> ignore", 1, 9, 1, 11
            "let _ = Unchecked.defaultof<_> in ()", 1, 29, 1, 30
        ]
        |> List.map (fun (str, line1, col1, line2, col2) -> [| box str ; line1 ; col1 ; line2 ; col2 |])

    let unquotableWarningCases =
        [
            "let f() = ([] = [])", 1, 17, 1, 19
            """let f<'b> (x : 'b) : int = failwith ""
let deserialize<'v> (s : string) : 'v = failwith ""
let x = deserialize "" |> f""", 3, 9, 3, 28
            "let f = typedefof<_>", 1, 19, 1, 20
            """let f<'b> () : 'b = (let a = failwith "" in unbox a)""", 1, 26, 1, 27
        ]
        |> List.map (fun (str, line1, col1, line2, col2) -> [| box str ; line1 ; col1 ; line2 ; col2 |])

    let warningCases =
        quotableWarningCases @ unquotableWarningCases

    [<Theory>]
    [<MemberData(nameof(warningCases))>]
    let ``Warning is emitted when type Obj is inferred``(code: string, line1: int, col1: int, line2: int, col2: int) =
        FSharp code
        |> withErrorRanges
        |> withWarnOn 3559
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Information 3559, Line line1, Col col1, Line line2, Col col2, message)

    let quotableNoWarningCases =
        [
            "let a = 5 |> unbox<obj> in let b = a in ()" // explicit obj annotation
            "let add x y = x + y in ()" // inferred as int
            "let f() = ([] = ([] : obj list)) in ()" // obj is inferred, but is annotated
            "let f() = (([] : obj list) = []) in ()" // obj is inferred, but is annotated
            "let f () : int = Unchecked.defaultof<_> in ()" // explicitly int
            "let f () = Unchecked.defaultof<int> in ()" // explicitly int
        ]
        |> List.map Array.singleton

    let unquotableNoWarningCases =
        [
            "let add x y = x + y" // inferred as int
            "let inline add x y = x + y" // inferred with SRTP
            "let inline add< ^T when ^T : (static member (+) : ^T * ^T -> ^T)> (x : ^T) (y : ^T) : ^T = x + y" // with SRTP
            "let f x = string x" // inferred as generic 'a -> string
            "let f() = ([] = ([] : obj list))" // obj is inferred, but is annotated
            "let f() = (([] : obj list) = [])" // obj is inferred, but is annotated
            """let x<[<Measure>]'m> : int<'m> = failwith ""
let f () = x = x |> ignore""" // measure is inferred as 1, but that's not covered by this warning
            "let f () : int = Unchecked.defaultof<_>" // explicitly int
            "let f () = Unchecked.defaultof<int>" // explicitly int
            "let f () = Unchecked.defaultof<_>" // generic
        ]
        |> List.map Array.singleton

    let noWarningCases = quotableNoWarningCases @ unquotableNoWarningCases

    [<Theory>]
    [<MemberData(nameof(noWarningCases))>]
    let ``Warning does not fire unless required``(code: string) =
        FSharp code
        |> withWarnOn 3559
        |> withLangVersionPreview
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
    let ``Don't warn on an explicitly annotated null``(expr: string) =
        sprintf "%s |> ignore" expr
        |> FSharp
        |> withWarnOn 3559
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed

    [<Theory>]
    [<MemberData(nameof(nullNoWarningCases))>]
    let ``Don't warn on an explicitly annotated null, inside quotations``(expr: string) =
        sprintf "<@ %s @> |> ignore" expr
        |> FSharp
        |> withWarnOn 3559
        |> typecheck
        |> shouldSucceed

    [<Theory>]
    [<MemberData(nameof(quotableWarningCases))>]
    let ``Warn also inside quotations of acceptable code``(expr: string, line1: int, col1: int, line2: int, col2: int) =
        sprintf "<@ %s @> |> ignore" expr
        |> FSharp
        |> withWarnOn 3559
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail
        |> withSingleDiagnostic (Information 3559, Line line1, Col (col1 + 3), Line line2, Col (col2 + 3), message)

    [<Theory>]
    [<MemberData(nameof(quotableNoWarningCases))>]
    let ``Don't warn inside quotations of acceptable code``(expr: string) =
        sprintf "<@ %s @> |> ignore" expr
        |> FSharp
        |> withWarnOn 3559
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed

    [<Theory>]
    [<MemberData(nameof(warningCases))>]
    let ``Warning is off by default``(expr: string, _: int, _: int, _: int, _: int) =
        expr
        |> FSharp
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed
