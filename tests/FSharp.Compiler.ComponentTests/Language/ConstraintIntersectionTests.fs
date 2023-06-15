module Language.ConstraintIntersectionTests

open Xunit
open FSharp.Test.Compiler
open StructuredResultsAsserts

[<Fact>]
let ``Constraint intersection works in lang preview``() =
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

let y (f: 't & #I & #IDisposable) =
    f.g ()
    f.Dispose ()
    ResizeArray<'t> ()

let z (f: #I & #IDisposable & #Task<int> & #seq<string>) =
    f.g ()
    f.Result |> ignore<int>
    f.Dispose ()
    """
    |> withLangVersionPreview
    |> typecheck
    |> shouldSucceed

[<Fact>]
let ``Constraint intersection does not work with non-flexible types``() =
    FSharp """
let y (f: #seq<int> & System.IDisposable) = ()
    """
    |> withLangVersionPreview
    |> typecheck
    |> shouldFail
    |> withDiagnostics [
        Error 3568, Line 2, Col 23, Line 2, Col 41, "Constraint intersection syntax may only be used with flexible types."
    ]