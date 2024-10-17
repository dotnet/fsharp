module Language.ConstraintIntersectionTests

open Xunit
open FSharp.Test.Compiler
open StructuredResultsAsserts

[<Fact>]
let ``Constraint intersection (appType) works in lang preview``() =
    FSharp """
open System
open System.Threading.Tasks
open System.Collections.Generic

type I =
    abstract g: unit -> unit
    abstract h: #IDisposable & #seq<int> -> unit

type F =
    interface I with
        member _.g () = ()
        member _.h v =
            for _ in v do
                ()
            v.Dispose ()

type E () =
    interface IDisposable with
        member _.Dispose () = ()
    interface IEnumerable<int> with
        member _.GetEnumerator () = null: IEnumerator<int>
        member _.GetEnumerator () = null: Collections.IEnumerator

let x (f: 't & #I) =
    f.g ()
    f.h (new E ())
    ResizeArray<'t> ()

let y (f: 't & #I & #IDisposable, name: string) =
    printfn "%s" name
    f.g ()
    f.Dispose ()
    ResizeArray<'t> ()

let z (f: #I & #IDisposable & #Task<int> & #seq<string>, name: string) =
    printfn "%s" name
    f.g ()
    f.Result |> ignore<int>
    f.Dispose ()
    """
    |> withLangVersion80
    |> typecheck
    |> shouldSucceed

[<Fact>]
let ``Constraint intersection (typarDecl) works in lang preview``() =
    FSharp """
open System

type C<'t & #seq<int> & #IDisposable, 'y & #seq<'t>> =
    member _.G (x: 't, y: 'y) =
        for x in x do
            printfn "%d" x

        x.Dispose ()
        
        for xs in y do
            for x in xs do
                printfn "%d" x
    """
    |> withLangVersion80
    |> typecheck
    |> shouldSucceed

[<Fact>]
let ``Constraint intersection does not work with non-flexible types``() =
    FSharp """
type C<'t & #seq<int> & System.IDisposable, 'y & #seq<'t>> = class end

let y (f: #seq<int> & System.IDisposable) = ()
    """
    |> withLangVersion80
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        Error 3572, Line 2, Col 25, Line 2, Col 43, "Constraint intersection syntax may only be used with flexible types, e.g. '#IDisposable & #ISomeInterface'."
        Error 3572, Line 4, Col 23, Line 4, Col 41, "Constraint intersection syntax may only be used with flexible types, e.g. '#IDisposable & #ISomeInterface'."
    ]

// bug 16309
[<Fact>]
let ``Constraint intersection handles invalid types``() =
    FSharp """
let f (x: 't when 't :> ABRAKADABRAA & #seq<int>) = ()
    """
    |> withLangVersion80
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        Error 0010, Line 2, Col 40, Line 2, Col 41, "Unexpected symbol # in pattern"
        Error 0583, Line 2, Col 7, Line 2, Col 8, "Unmatched '('"
    ]
