module ErrorMessages.Issue9838Tests

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

// https://github.com/dotnet/fsharp/issues/9838
// When an F# program calls a C#-style extension method on the wrong receiver type,
// the overload-error message should render the extension's declaring type
// (e.g. `MemoryExtensions`), not the receiver type (e.g. `Foo`).
//
// The Span<T> / MemoryExtensions APIs used by the RED tests below ship in the BCL on
// .NET (Core), but on .NET Framework (net48) they live in the optional System.Memory
// NuGet package which the test sandbox does not reference. We therefore gate those
// tests on NETCOREAPP so they exercise the actual overload-resolution diagnostic
// rather than failing with FS0039 "The value or constructor 'Span' is not defined".

[<FactForNETCOREAPP>]
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

[<FactForNETCOREAPP>]
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

[<FactForNETCOREAPP>]
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

[<FactForNETCOREAPP>]
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

(*
 * Captured RED-phase failure output (net10.0) - kept as a reference for the
 * implementation sprint. Each RED test asserts the desired post-fix wording
 * (`(extension) MemoryExtensions.CopyTo<'T>(...)`) and currently FAILS because
 * the compiler renders the *receiver* type instead of the extension's
 * declaring type. Once the fix lands the actual text below should change to
 * match the expected text and the RED tests will go green.
 *
 *   Test: Issue 9838 - C#-style extension method overload error shows declaring type, not receiver
 *   Expected (post-fix):
 *     "No overloads match for method 'CopyTo'.
 *      Known type of argument: Span<int>
 *      Available overloads:
 *       - (extension) MemoryExtensions.CopyTo<'T>(destination: Memory<'T>) : unit // Argument 'destination' doesn't match
 *       - (extension) MemoryExtensions.CopyTo<'T>(destination: Span<'T>) : unit // Argument 'destination' doesn't match"
 *   Actual (current bug):
 *     "No overloads match for method 'CopyTo'.
 *      Known type of argument: Span<int>
 *      Available overloads:
 *       - (extension) Foo.CopyTo<'T>(destination: Memory<'T>) : unit
 *       - (extension) Foo.CopyTo<'T>(destination: Span<'T>) : unit"
 *
 *   Test: Issue 9838 - error message mentions declaring type MemoryExtensions
 *     regex "\(extension\) MemoryExtensions\.CopyTo" does NOT match actual text
 *     "(extension) Foo.CopyTo<'T>(destination: ...)".
 *
 *   Test: Issue 9838 - error message must not mislead by mentioning Foo.CopyTo
 *     Assert.DoesNotContain("Foo.CopyTo", ...) fails because the diagnostic
 *     contains "(extension) Foo.CopyTo<'T>(destination: Memory<'T>)".
 *
 *   Test: Issue 9838 - C#-style extension on generic receiver mismatch shows declaring type
 *     Expected regex "\(extension\) MemoryExtensions\.CopyTo" against actual:
 *     "(extension) List.CopyTo<'T>(destination: Span<'T>) : unit ..." - the
 *     extensions are attributed to the receiver `List` rather than to
 *     `MemoryExtensions`.
 *
 * GATE tests (F#-style extension and regular instance method) PASS today and
 * must continue to pass after the fix, guaranteeing the fix is scoped to
 * C#-style (`[<Extension>]`) extension overload rendering only.
 *)
