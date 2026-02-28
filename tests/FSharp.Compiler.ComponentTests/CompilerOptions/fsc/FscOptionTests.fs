// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test.Compiler

module FscOptionTests =

    // --parseonly

    [<Fact>]
    let ``fsc --parseonly succeeds on valid source`` () =
        Fs """printfn "Hello, World" """
        |> asExe
        |> ignoreWarnings
        |> withOptions ["--parseonly"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``fsc --parseonly catches parse error`` () =
        Fs """let x  1"""
        |> asExe
        |> ignoreWarnings
        |> withOptions ["--parseonly"]
        |> compile
        |> shouldFail
        |> ignore

    // --typecheckonly

    [<Fact>]
    let ``fsc --typecheckonly succeeds on valid source`` () =
        Fs """printfn "Hello, World" """
        |> asExe
        |> ignoreWarnings
        |> withOptions ["--typecheckonly"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``fsc --typecheckonly catches type error`` () =
        Fs """let x: int = "not an int" """
        |> asExe
        |> ignoreWarnings
        |> withOptions ["--typecheckonly"]
        |> compile
        |> shouldFail
        |> withErrorCode 1
        |> ignore

    // --consolecolors

    [<InlineData("--consolecolors")>]
    [<InlineData("--consolecolors+")>]
    [<InlineData("--consolecolors-")>]
    [<Theory>]
    let ``fsc --consolecolors switch`` option =
        Fs """printfn "Hello, World" """
        |> asExe
        |> withOptions [option]
        |> compile
        |> shouldSucceed
        |> ignore

    // --preferreduilang

    [<Fact>]
    let ``fsc --preferreduilang en is accepted`` () =
        Fs """printfn "Hello, World" """
        |> asExe
        |> withOptions ["--preferreduilang:en"]
        |> compile
        |> shouldSucceed
        |> ignore

    [<Fact>]
    let ``fsc --preferreduilang ja is accepted`` () =
        Fs """printfn "Hello, World" """
        |> asExe
        |> withOptions ["--preferreduilang:ja"]
        |> compile
        |> shouldSucceed
        |> ignore

    // --abortonerror

    [<Fact>]
    let ``fsc --abortonerror stops on first error`` () =
        Fs """
let x: int = "not an int"
let y: string = 42
        """
        |> asExe
        |> withOptions ["--abortonerror"]
        |> compile
        |> shouldFail
        |> ignore

    // --jit

    [<InlineData("--jit+")>]
    [<InlineData("--jit-")>]
    [<Theory>]
    let ``fsc --jit optimization switch`` option =
        Fs """printfn "Hello, World" """
        |> asExe
        |> ignoreWarnings
        |> withOptions [option]
        |> compile
        |> shouldSucceed
        |> ignore

    // --localoptimize

    [<InlineData("--localoptimize+")>]
    [<InlineData("--localoptimize-")>]
    [<Theory>]
    let ``fsc --localoptimize optimization switch`` option =
        Fs """printfn "Hello, World" """
        |> asExe
        |> ignoreWarnings
        |> withOptions [option]
        |> compile
        |> shouldSucceed
        |> ignore

    // --splitting

    [<InlineData("--splitting+")>]
    [<InlineData("--splitting-")>]
    [<Theory>]
    let ``fsc --splitting optimization switch`` option =
        Fs """printfn "Hello, World" """
        |> asExe
        |> ignoreWarnings
        |> withOptions [option]
        |> compile
        |> shouldSucceed
        |> ignore

    // Error cases

    [<Fact>]
    let ``fsc --warn with invalid value produces error`` () =
        Fs """printfn "Hello, World" """
        |> asExe
        |> withOptions ["--warn:invalid"]
        |> compile
        |> shouldFail
        |> withDiagnosticMessageMatches "not a valid integer argument"
        |> ignore

    [<Fact>]
    let ``fsc --target with invalid value produces error`` () =
        Fs """printfn "Hello, World" """
        |> asExe
        |> withOptions ["--target:invalid"]
        |> compile
        |> shouldFail
        |> withDiagnosticMessageMatches "Unrecognized target"
        |> ignore
