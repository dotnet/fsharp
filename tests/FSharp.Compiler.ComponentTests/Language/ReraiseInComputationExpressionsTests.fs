// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// https://github.com/fsharp/fslang-suggestions/issues/660
module Language.ReraiseInComputationExpressionsTests

open FSharp.Test.Compiler
open Xunit

let private preamble =
    """
open System

exception Original of string

// Kept out of line so the frame it adds is what the assertion below looks for.
[<System.Runtime.CompilerServices.MethodImpl(System.Runtime.CompilerServices.MethodImplOptions.NoInlining)>]
let throwIt () = raise (Original "boom")

let handled = ref Unchecked.defaultof<exn>
"""

// The compiled snippet targets whatever runtime this test host itself runs under (TargetFramework.Current),
// and string.Contains(string, StringComparison) isn't in the netfx BCL the Desktop jobs compile against.
#if NETCOREAPP
let private containsThrowItFrame = """e.StackTrace.Contains("throwIt", StringComparison.Ordinal)"""
#else
let private containsThrowItFrame = """e.StackTrace.Contains("throwIt")"""
#endif

/// Runs a computation that rethrows, then asserts the original exception instance and its original
/// throw site both survived the round trip.
let private assertingRethrow declarations body =
    $"""{preamble}{declarations}
let thrown =
    try
        {body}
        None
    with e -> Some e

[<EntryPoint>]
let main _ =
    match thrown with
    | None -> failwith "expected an exception"
    | Some e ->
        if not (Object.ReferenceEquals(e, handled.Value)) then failwith "expected the original exception instance"
        if not ({containsThrowItFrame}) then failwith $"expected the original stack trace, got: {{e.StackTrace}}"
        0
"""
    |> FSharp
    |> asExe
    |> withLangVersionPreview
    |> compileAndRun
    |> shouldSucceed

module LangVersion =
    let private source =
        """
let f () =
    async {
        try
            return 1
        with e ->
            return reraise ()
    }
"""

    /// The construct was always an error, so an older language version keeps reporting it as one
    /// instead of pointing at the language version.
    [<Fact>]
    let ``11.0 → error`` () =
        FSharp source
        |> withLangVersion11
        |> typecheck
        |> shouldFail
        |> withErrorCode 413
        |> withDiagnosticMessageMatches "reraise"

    [<Fact>]
    let ``preview → success`` () =
        FSharp source
        |> withLangVersionPreview
        |> typecheck
        |> shouldSucceed

module Rethrowing =
    [<Fact>]
    let ``async return reraise`` () =
        assertingRethrow "" """
        async {
            try
                throwIt ()
                return 0
            with e ->
                handled.Value <- e
                return reraise ()
        }
        |> Async.RunSynchronously
        |> ignore"""

    [<Fact>]
    let ``task return reraise`` () =
        assertingRethrow "" """
        let t =
            task {
                try
                    throwIt ()
                    return 0
                with e ->
                    handled.Value <- e
                    return reraise ()
            }
        t.GetAwaiter().GetResult()
        |> ignore"""

    [<Fact>]
    let ``reraise after a bind in the handler`` () =
        assertingRethrow "" """
        async {
            try
                throwIt ()
                return 0
            with e ->
                handled.Value <- e
                do! Async.Sleep 1
                return reraise ()
        }
        |> Async.RunSynchronously
        |> ignore"""

    [<Fact>]
    let ``reraise in statement position`` () =
        assertingRethrow "" """
        async {
            try
                throwIt ()
            with e ->
                handled.Value <- e
                reraise ()
        }
        |> Async.RunSynchronously"""

    [<Fact>]
    let ``reraise in a when guard`` () =
        assertingRethrow "" """
        async {
            try
                throwIt ()
            with
            | e when (handled.Value <- e
                      reraise ()) -> ()
        }
        |> Async.RunSynchronously"""

    [<Fact>]
    let ``reraise in a function written inside the handler`` () =
        assertingRethrow "" """
        async {
            try
                throwIt ()
            with e ->
                handled.Value <- e
                let rethrow () = reraise ()
                rethrow ()
        }
        |> Async.RunSynchronously"""

    [<Fact>]
    let ``reraise in a nested computation expression sees the enclosing handler`` () =
        assertingRethrow "" """
        async {
            try
                throwIt ()
            with e ->
                handled.Value <- e
                return! async { return reraise () }
        }
        |> Async.RunSynchronously"""

    [<Fact>]
    let ``seq comprehension`` () =
        assertingRethrow "" """
        seq {
            try
                throwIt ()
                yield 1
            with e ->
                handled.Value <- e
                yield reraise ()
        }
        |> Seq.toList
        |> ignore"""

    [<Fact>]
    let ``list comprehension`` () =
        assertingRethrow "" """
        [ try
              throwIt ()
              yield 1
          with e ->
              handled.Value <- e
              yield reraise () ]
        |> ignore"""

    [<Fact>]
    let ``array comprehension`` () =
        assertingRethrow "" """
        [| try
               throwIt ()
               yield 1
           with e ->
               handled.Value <- e
               yield reraise () |]
        |> ignore"""

    [<Fact>]
    let ``task reraise after a bind in the handler`` () =
        assertingRethrow "" """
        let t =
            task {
                try
                    throwIt ()
                    return 0
                with e ->
                    handled.Value <- e
                    do! System.Threading.Tasks.Task.Delay 1
                    return reraise ()
            }
        t.GetAwaiter().GetResult()
        |> ignore"""

    [<Fact>]
    let ``reraise inside a for loop in the handler`` () =
        assertingRethrow "" """
        async {
            try
                throwIt ()
            with e ->
                handled.Value <- e
                for _ in 1..3 do
                    reraise ()
        }
        |> Async.RunSynchronously"""

    [<Fact>]
    let ``reraise inside a while loop in the handler`` () =
        assertingRethrow "" """
        async {
            try
                throwIt ()
            with e ->
                handled.Value <- e
                while true do
                    reraise ()
        }
        |> Async.RunSynchronously"""

    [<Fact>]
    let ``try-with inside a for loop`` () =
        assertingRethrow "" """
        async {
            for _ in 1..1 do
                try
                    throwIt ()
                with e ->
                    handled.Value <- e
                    reraise ()
        }
        |> Async.RunSynchronously"""

    [<Fact>]
    let ``try-with inside a for loop in a seq comprehension`` () =
        assertingRethrow "" """
        seq {
            for _ in 1..1 do
                try
                    throwIt ()
                    yield 1
                with e ->
                    handled.Value <- e
                    yield reraise ()
        }
        |> Seq.toList
        |> ignore"""

    /// 'reraise ()' binds to the nearest lexically enclosing handler, so a closure written in the handler
    /// still rethrows the caught exception even when invoked after the computation has completed.
    [<Fact>]
    let ``closure escaping the handler still rethrows the caught exception`` () =
        assertingRethrow "" """
        let rethrow =
            async {
                try
                    throwIt ()
                    return (fun () -> 1)
                with e ->
                    handled.Value <- e
                    return (fun () -> reraise ())
            }
            |> Async.RunSynchronously
        rethrow () |> ignore"""

    [<Fact>]
    let ``custom builder`` () =
        let builder =
            """
type Builder() =
    member _.Delay(f: unit -> int) = f
    member _.Run(f: unit -> int) = f ()
    member _.Return(x: int) = x
    member _.TryWith(body: unit -> int, handler: exn -> int) = try body () with e -> handler e

let custom = Builder()
"""

        assertingRethrow builder """
        custom {
            try
                return throwIt ()
            with e ->
                handled.Value <- e
                return reraise ()
        }
        |> ignore"""

module NestedRealTryWith =
    /// A real try-with inside a computation expression handler keeps IL 'rethrow' semantics, so its
    /// 'reraise' rethrows the inner exception, not the one caught by the computation expression.
    [<Fact>]
    let ``inner reraise rethrows the inner exception`` () =
        """
exception Outer
exception Inner

let thrown =
    try
        async {
            try
                raise Outer
            with _ ->
                try
                    raise Inner
                with _ ->
                    reraise ()
        }
        |> Async.RunSynchronously
        None
    with e -> Some e

[<EntryPoint>]
let main _ =
    match thrown with
    | Some Inner -> 0
    | other -> failwith $"expected Inner, got {other}"
"""
        |> FSharp
        |> asExe
        |> withLangVersionPreview
        |> compileAndRun
        |> shouldSucceed

module StillRejected =
    let private rejected source =
        FSharp source
        |> withLangVersionPreview
        |> typecheck
        |> shouldFail

    [<Fact>]
    let ``outside any handler`` () =
        rejected """
let f () =
    async {
        return reraise ()
    }
"""
        |> withErrorCode 413
        |> withDiagnosticMessageMatches "reraise"

    [<Fact>]
    let ``in the try body`` () =
        rejected """
let f () =
    async {
        try
            return reraise ()
        with _ ->
            return 0
    }
"""
        |> withErrorCode 413
        |> withDiagnosticMessageMatches "reraise"

    [<Fact>]
    let ``first-class use in a handler`` () =
        rejected """
let f () =
    async {
        try
            return 1
        with _ ->
            let g = reraise
            return g ()
    }
"""
        |> withErrorCode 417

    /// The rewrite rethrows the value the handler catches, so a builder catching anything else keeps the old error.
    [<Fact>]
    let ``builder whose TryWith handler does not take an exn`` () =
        rejected """
type Builder() =
    member _.Delay(f: unit -> int) = f
    member _.Run(f: unit -> int) = f ()
    member _.Return(x: int) = x
    member _.TryWith(body: unit -> int, handler: string -> int) =
        try body () with e -> handler e.Message

let custom = Builder()

let f () =
    custom {
        try
            return 1
        with _ ->
            return reraise ()
    }
"""
        |> withErrorCode 413
        |> withDiagnosticMessageMatches "reraise"

module Emitted =
    [<Fact>]
    let ``computation expression handler rethrows through ExceptionDispatchInfo`` () =
        """
module Test

let f () =
    task {
        try
            return 1
        with e ->
            return reraise ()
    }
"""
        |> FSharp
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> verifyILPresent [
            "ExceptionDispatchInfo::Capture(class [runtime]System.Exception)"
            "ExceptionDispatchInfo::Throw()"
        ]

    [<Fact>]
    let ``a real try-with still emits rethrow`` () =
        """
module Test

let f () =
    try
        1
    with e ->
        reraise ()
"""
        |> FSharp
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> verifyILPresent [ "rethrow" ]

module Quotations =
    [<Fact>]
    let ``handler containing reraise can be quoted`` () =
        """
module Test

let q =
    <@
        async {
            try
                return 1
            with e ->
                return reraise ()
        }
    @>
"""
        |> FSharp
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
