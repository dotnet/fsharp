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

// Verify that the error range is narrow also for simple direct method calls
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

// Verify that a long expression before the method doesn't widen the error range
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

// Verify error range with lambda argument
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

// Verify that backtick-escaped method names are also correctly narrowed
// (itemIdentRange from ComputeItemRange includes the backtick delimiters)
[<Fact>]
let ``Issue 14284 - backtick-escaped method name is narrowed to identifier`` () =
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

// Verify multiline method access is also correctly narrowed to the method name
[<Fact>]
let ``Issue 14284 - multiline method access narrows to method name`` () =
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
    |> withErrorCode 41

// Additional Phase 1 tests (issue #3920 follow-up): diagnostic sites beyond
// UnresolvedOverloading. These verify that the narrow terminal-identifier
// range applies to every diagnostic site that reports on a resolved item
// reached through a long identifier.
//
// NOTE: Obsolete warnings for members reached through a dotted long-identifier
// are currently emitted at the full `mItem` range (the whole `T.Instance.M`
// span). Narrowing those requires plumbing `mItemIdent` all the way to
// `CheckMethInfoAttributes` / property-access diagnostic sites, which is out
// of scope for this change. Tracked as follow-up.
