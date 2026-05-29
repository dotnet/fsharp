// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.BasicGrammarElements

open FSharp.Test.Compiler
open Xunit

// Regression tests for https://github.com/dotnet/fsharp/issues/19664
// Fix: https://github.com/dotnet/fsharp/pull/19698
module StaticMethodResolution =

    let private sharedSource = """
module Lib =

    type StaticGeneric<'T> =
        static member Bar() = ()
        static member Bar(_: int, _: int) = ()

    [<AutoOpen>]
    module Extensions =
        type StaticGeneric<'T> with
            static member Bar(_: int) = ()
"""

    [<Fact>]
    let ``Static extension on generic type resolves with explicit type arguments``() =
        Fsx (sharedSource + """
module Program =
    open Lib

    StaticGeneric<int>.Bar()
    StaticGeneric<int>.Bar(42)      // extension overload — regressed to FS0505 in issue #19664
    StaticGeneric<int>.Bar(42, 0)
        """)
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``Static extension on generic type - wrong arity produces diagnostic``() =
        Fsx (sharedSource + """
module Program =
    open Lib

    StaticGeneric<int>.Bar(1, 2, 3)
        """)
        |> typecheck
        |> shouldFail
        |> withErrorCode 505

    [<Fact>]
    let ``Static extension on generic type resolves alongside instance members``() =
        Fsx """
module Lib =

    type Container<'T> =
        static member Create() = Unchecked.defaultof<Container<'T>>
        member _.Value = Unchecked.defaultof<'T>

    [<AutoOpen>]
    module Extensions =
        type Container<'T> with
            static member Create(_: 'T) = Unchecked.defaultof<Container<'T>>

module Program =
    open Lib

    let c = Container<int>.Create()
    let c2 = Container<int>.Create(42)
    let v = c.Value
        """
        |> typecheck
        |> shouldSucceed
