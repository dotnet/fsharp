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
module M =
    type T =
        new(_: int) = {}
        new(_: double) = {}

M.T("")
        """
    |> typecheck
    |> shouldFail
    |> withDiagnostics
        [ (Error 41, Line 7, Col 3, Line 7, Col 4, "No overloads match for method 'T'.

Known type of argument: string

Available overloads:
 - new: double -> M.T // Argument at index 1 doesn't match
 - new: int -> M.T // Argument at index 1 doesn't match") ]

[<Fact>]
let ``Regression - ParamArray arg mismatch should not add extra FS0001 duplicates`` () =
    // Pre-existing: main already emits each FS0001 twice (4 total for 2 args).
    // This test ensures the PR does not INCREASE the count.
    let result =
        FSharp
            """
type C() =
    static member M(fmt: string, [<System.ParamArray>] args: int[]) = ()

C.M("{0}", box 1, box 2)
            """
        |> typecheck

    let diags = result.Output.Diagnostics
    let fs0001 = diags |> List.filter (fun d -> d.Error = (Error 1))

    Assert.True(
        fs0001.Length = 4,
        sprintf "Expected 4 FS0001 (pre-existing 2x duplication for 2 args) but got %d. Diagnostics:\n%A" fs0001.Length diags
    )

[<Fact>]
let ``Regression - ByRefKinds library-only types should not add extra FS1204 duplicates`` () =
    // Pre-existing: main already emits each FS1204 twice (4 total for 2 members).
    // This test ensures the PR does not INCREASE the count.
    let result =
        FSharp
            """
type C() =
    static member F(x: ByRefKinds.In) = 1
    static member F(x: ByRefKinds.Out) = 1
            """
        |> typecheck

    let diags = result.Output.Diagnostics
    let fs1204 = diags |> List.filter (fun d -> d.Error = (Error 1204))

    Assert.True(
        fs1204.Length = 4,
        sprintf "Expected 4 FS1204 (pre-existing 2x duplication for 2 members) but got %d. Diagnostics:\n%A" fs1204.Length diags
    )

[<Fact>]
let ``Obsolete diagnostic should point at method name, not full expression`` () =
    FSharp
        """
type T() =
    static member Instance = T()

    [<System.Obsolete("old method")>]
    member _.OldMethod() = ()

T.Instance.OldMethod()
        """
    |> typecheck
    |> shouldFail
    |> withDiagnostics
        [
            // First: narrowed to "OldMethod" via mItem (Col 12-21), not the full "T.Instance.OldMethod"
            (Warning 44, Line 8, Col 12, Line 8, Col 21, "This construct is deprecated. old method")
            // Second: pre-existing duplicate from name resolution attribute check (whole application range)
            (Warning 44, Line 8, Col 1, Line 8, Col 23, "This construct is deprecated. old method")
        ]

[<Fact>]
let ``Obsolete diagnostic on static method should point at method name`` () =
    FSharp
        """
module M =
    type Svc() =
        [<System.Obsolete("use NewMethod instead")>]
        static member OldMethod() = ()

M.Svc.OldMethod()
        """
    |> typecheck
    |> shouldFail
    |> withDiagnostics
        [
            // First: narrowed to "OldMethod" via mItem (Col 7-16), not "M.Svc.OldMethod"
            (Warning 44, Line 7, Col 7, Line 7, Col 16, "This construct is deprecated. use NewMethod instead")
            // Second: pre-existing duplicate from name resolution attribute check (whole expression range)
            (Warning 44, Line 7, Col 1, Line 7, Col 18, "This construct is deprecated. use NewMethod instead")
        ]
