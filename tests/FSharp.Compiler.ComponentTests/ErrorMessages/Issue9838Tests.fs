module ErrorMessages.Issue9838Tests

open Xunit
open FSharp.Test.Compiler

// https://github.com/dotnet/fsharp/issues/9838
// When an F# program calls a C#-style extension method on the wrong receiver type,
// the overload-error message should render the extension's declaring type
// (e.g. `MemoryExtensions`), not the receiver type (e.g. `Foo`).

[<Fact>]
let ``Issue 9838 - C#-style extension method overload error shows declaring type, not receiver`` () =
    FSharp """
open System
type Foo() = class end
let foo = Foo()
let span = Span<int>.Empty
foo.CopyTo(span)
"""
    |> typecheck
    |> shouldFail
    |> withDiagnostics
        [ (Error 41, Line 6, Col 5, Line 6, Col 11,
            "No overloads match for method 'CopyTo'.

Known type of argument: Span<int>

Available overloads:
 - (extension) MemoryExtensions.CopyTo<'T>(destination: Memory<'T>) : unit // Argument 'destination' doesn't match
 - (extension) MemoryExtensions.CopyTo<'T>(destination: Span<'T>) : unit // Argument 'destination' doesn't match") ]

[<Fact>]
let ``Issue 9838 - error message mentions declaring type MemoryExtensions`` () =
    FSharp """
open System
type Foo() = class end
let foo = Foo()
let span = Span<int>.Empty
foo.CopyTo(span)
"""
    |> typecheck
    |> shouldFail
    |> withDiagnosticMessageMatches "\\(extension\\) MemoryExtensions\\.CopyTo"

[<Fact>]
let ``Issue 9838 - error message must not mislead by mentioning Foo.CopyTo`` () =
    let result =
        FSharp """
open System
type Foo() = class end
let foo = Foo()
let span = Span<int>.Empty
foo.CopyTo(span)
"""
        |> typecheck

    result |> shouldFail |> ignore

    let allMessages =
        result.Output.Diagnostics
        |> List.map (fun d -> d.Message)
        |> String.concat "\n"

    Assert.DoesNotContain("Foo.CopyTo", allMessages)
    Assert.Contains("MemoryExtensions.CopyTo", allMessages)

[<Fact>]
let ``F#-style extension member overload error keeps current rendering`` () =
    FSharp """
type Foo() = class end

module FooExtensions =
    type Foo with
        member _.Bar(_: int) = ()
        member _.Bar(_: double) = ()

open FooExtensions

let foo = Foo()
foo.Bar("")
"""
    |> typecheck
    |> shouldFail
    |> withDiagnosticMessageMatches "member Foo\\.Bar"

[<Fact>]
let ``Regular instance method overload error is unaffected by issue 9838 fix`` () =
    FSharp """
type T() =
    member _.M(_: int) = ()
    member _.M(_: double) = ()

let t = T()
t.M("")
"""
    |> typecheck
    |> shouldFail
    |> withDiagnostics
        [ (Error 41, Line 7, Col 3, Line 7, Col 4,
            "No overloads match for method 'M'.

Known type of argument: string

Available overloads:
 - member T.M: double -> unit // Argument at index 1 doesn't match
 - member T.M: int -> unit // Argument at index 1 doesn't match") ]

[<Fact>]
let ``Issue 9838 - C#-style extension on generic receiver mismatch shows declaring type`` () =
    FSharp """
open System
open System.Collections.Generic
let xs = List<int>()
let span = Span<string>.Empty
xs.CopyTo(span)
"""
    |> typecheck
    |> shouldFail
    |> withDiagnosticMessageMatches "\\(extension\\) MemoryExtensions\\.CopyTo"
