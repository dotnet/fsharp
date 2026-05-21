module ErrorMessages.OverloadResolutionErrorRangeTests

open Xunit
open FSharp.Test.Compiler

// https://github.com/dotnet/fsharp/issues/14284
[<Fact>]
let ``Issue 14284 - overload error should cover only method name, not full expression`` () =
    FSharp
        """
type T() =
    static member Instance = T()

    member _.Method(_: double) = ()
    member _.Method(_: int) = ()

T.Instance.Method("")
        """
    |> typecheck
    |> shouldFail
    |> withDiagnostics
        [ (Error 41, Line 8, Col 12, Line 8, Col 18, "No overloads match for method 'Method'.

Known type of argument: string

Available overloads:
 - member T.Method: double -> unit // Argument at index 1 doesn't match
 - member T.Method: int -> unit // Argument at index 1 doesn't match") ]

[<Fact>]
let ``Issue 14284 - overload error for simple static method`` () =
    FSharp
        """
type T() =
    static member Method(_: double) = ()
    static member Method(_: int) = ()

T.Method("")
        """
    |> typecheck
    |> shouldFail
    |> withDiagnostics
        [ (Error 41, Line 6, Col 3, Line 6, Col 9, "No overloads match for method 'Method'.

Known type of argument: string

Available overloads:
 - static member T.Method: double -> unit // Argument at index 1 doesn't match
 - static member T.Method: int -> unit // Argument at index 1 doesn't match") ]

[<Fact>]
let ``Issue 14284 - overload error on chained expression`` () =
    FSharp
        """
type T() =
    static member Instance = T()

    member _.Next = T()
    member _.Method(_: double) = ()
    member _.Method(_: int) = ()

T.Instance.Next.Next.Method("")
        """
    |> typecheck
    |> shouldFail
    |> withDiagnostics
        [ (Error 41, Line 9, Col 22, Line 9, Col 28, "No overloads match for method 'Method'.

Known type of argument: string

Available overloads:
 - member T.Method: double -> unit // Argument at index 1 doesn't match
 - member T.Method: int -> unit // Argument at index 1 doesn't match") ]

[<Fact>]
let ``Issue 14284 - overload error with lambda argument`` () =
    FSharp
        """
type T() =
    static member Instance = T()

    member _.Method(_: double) = ()
    member _.Method(_: int) = ()

T.Instance.Method(fun () -> "")
        """
    |> typecheck
    |> shouldFail
    |> withDiagnostics
        [ (Error 41, Line 8, Col 12, Line 8, Col 18, "No overloads match for method 'Method'.

Known type of argument: (unit -> string)

Available overloads:
 - member T.Method: double -> unit // Argument at index 1 doesn't match
 - member T.Method: int -> unit // Argument at index 1 doesn't match") ]

[<Fact>]
let ``Issue 14284 - backtick-escaped method name`` () =
    FSharp
        """
type T() =
    static member Instance = T()

    member _.``My Method``(_: double) = ()
    member _.``My Method``(_: int) = ()

T.Instance.``My Method``("")
        """
    |> typecheck
    |> shouldFail
    |> withDiagnostics
        [ (Error 41, Line 8, Col 12, Line 8, Col 25, "No overloads match for method 'My Method'.

Known type of argument: string

Available overloads:
 - member T.``My Method`` : double -> unit // Argument at index 1 doesn't match
 - member T.``My Method`` : int -> unit // Argument at index 1 doesn't match") ]

[<Fact>]
let ``Issue 14284 - multiline method access`` () =
    FSharp
        """
type T() =
    static member Instance = T()

    member _.Method(_: double) = ()
    member _.Method(_: int) = ()

T
  .Instance
  .Method("")
        """
    |> typecheck
    |> shouldFail
    |> withDiagnostics
        [ (Error 41, Line 10, Col 4, Line 10, Col 10, "No overloads match for method 'Method'.

Known type of argument: string

Available overloads:
 - member T.Method: double -> unit // Argument at index 1 doesn't match
 - member T.Method: int -> unit // Argument at index 1 doesn't match") ]

[<Fact>]
let ``Issue 14284 - constructor overload error`` () =
    FSharp
        """
type T(_: int) =
    new(_: double) = T(0)

T("")
        """
    |> typecheck
    |> shouldFail
    |> withErrorCode 41
