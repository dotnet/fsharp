// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace CompilerOptions.Fsc

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

// Migrated from FSharpQA suite - CompilerOptions/fsc/subsystemversion
// --subsystemversion option error tests (DesktopOnly)
// Tests for invalid subsystemversion arguments that produce errors.

[<Trait("Category", "DesktopOnly")>]
module Subsystemversion =

    // Test: --subsystemversion:3.99 - version too low
    // Original: SOURCE=E_Error01.fs SCFLAGS="--subsystemversion:3.99"
    [<FactForDESKTOP>]
    let ``subsystemversion - 3.99 too low`` () =
        FSharp """module M
printfn "%A" System.DateTime.Now
let x = 0
x |> exit"""
        |> asExe
        |> withOptions ["--subsystemversion:3.99"]
        |> compile
        |> shouldFail
        |> withErrorCode 1051
        |> withDiagnosticMessageMatches "Invalid version '3\\.99' for '--subsystemversion'"
        |> ignore

    // Test: --subsystemversion: (missing value)
    // Original: SOURCE=E_Error02.fs SCFLAGS="--subsystemversion:"
    [<FactForDESKTOP>]
    let ``subsystemversion - missing value`` () =
        FSharp """module M
printfn "%A" System.DateTime.Now
let x = 0
x |> exit"""
        |> asExe
        |> withOptions ["--subsystemversion:"]
        |> compile
        |> shouldFail
        |> withErrorCode 224
        |> withDiagnosticMessageMatches "Option requires parameter: --subsystemversion:<string>"
        |> ignore

    // Test: --subsystemversion:"" (empty string)
    // Original: SOURCE=E_Error03.fs SCFLAGS="--subsystemversion:\"\""
    [<FactForDESKTOP>]
    let ``subsystemversion - empty string`` () =
        FSharp """module M
printfn "%A" System.DateTime.Now
let x = 0
x |> exit"""
        |> asExe
        |> withOptions ["--subsystemversion:"]
        |> compile
        |> shouldFail
        |> withErrorCode 224
        |> withDiagnosticMessageMatches "Option requires parameter: --subsystemversion:<string>"
        |> ignore

    // Test: --subsystemversion:4,0 (comma instead of dot)
    // Original: SOURCE=E_Error04.fs SCFLAGS="--subsystemversion:4,0"
    [<FactForDESKTOP>]
    let ``subsystemversion - comma instead of dot`` () =
        FSharp """module M
printfn "%A" System.DateTime.Now
let x = 0
x |> exit"""
        |> asExe
        |> withOptions ["--subsystemversion:4,0"]
        |> compile
        |> shouldFail
        |> withErrorCode 1051
        |> withDiagnosticMessageMatches "Invalid version '4,0' for '--subsystemversion'"
        |> ignore

    // Test: --subsystemversion:4 (missing minor)
    // Original: SOURCE=E_Error05.fs SCFLAGS="--subsystemversion:4"
    [<FactForDESKTOP>]
    let ``subsystemversion - missing minor version`` () =
        FSharp """module M
printfn "%A" System.DateTime.Now
let x = 0
x |> exit"""
        |> asExe
        |> withOptions ["--subsystemversion:4"]
        |> compile
        |> shouldFail
        |> withErrorCode 1051
        |> withDiagnosticMessageMatches "Invalid version '4' for '--subsystemversion'"
        |> ignore

    // Test: --subsystemversion:.4 (missing major)
    // Original: SOURCE=E_Error06.fs SCFLAGS="--subsystemversion:.4"
    [<FactForDESKTOP>]
    let ``subsystemversion - missing major version`` () =
        FSharp """module M
printfn "%A" System.DateTime.Now
let x = 0
x |> exit"""
        |> asExe
        |> withOptions ["--subsystemversion:.4"]
        |> compile
        |> shouldFail
        |> withErrorCode 1051
        |> withDiagnosticMessageMatches "Invalid version '\\.4' for '--subsystemversion'"
        |> ignore

    // Test: --subsystemversion:65536.0 (major too large)
    // Original: SOURCE=E_Error07.fs SCFLAGS="--subsystemversion:65536.0"
    [<FactForDESKTOP>]
    let ``subsystemversion - major version too large`` () =
        FSharp """module M
printfn "%A" System.DateTime.Now
let x = 0
x |> exit"""
        |> asExe
        |> withOptions ["--subsystemversion:65536.0"]
        |> compile
        |> shouldFail
        |> withErrorCode 1051
        |> withDiagnosticMessageMatches "Invalid version '65536\\.0' for '--subsystemversion'"
        |> ignore

    // Test: --subsystemversion:4.65536 (minor too large)
    // Original: SOURCE=E_Error08.fs SCFLAGS="--subsystemversion:4.65536"
    [<FactForDESKTOP>]
    let ``subsystemversion - minor version too large`` () =
        FSharp """module M
printfn "%A" System.DateTime.Now
let x = 0
x |> exit"""
        |> asExe
        |> withOptions ["--subsystemversion:4.65536"]
        |> compile
        |> shouldFail
        |> withErrorCode 1051
        |> withDiagnosticMessageMatches "Invalid version '4\\.65536' for '--subsystemversion'"
        |> ignore

    // Test: --subsystemversion:65536.65536 (both too large)
    // Original: SOURCE=E_Error09.fs SCFLAGS="--subsystemversion:65536.65536"
    [<FactForDESKTOP>]
    let ``subsystemversion - both versions too large`` () =
        FSharp """module M
printfn "%A" System.DateTime.Now
let x = 0
x |> exit"""
        |> asExe
        |> withOptions ["--subsystemversion:65536.65536"]
        |> compile
        |> shouldFail
        |> withErrorCode 1051
        |> withDiagnosticMessageMatches "Invalid version '65536\\.65536' for '--subsystemversion'"
        |> ignore

    // Test: --subsystemversion:-1.-2 (negative values)
    // Original: SOURCE=E_Error10.fs SCFLAGS="--subsystemversion:-1.-2"
    [<FactForDESKTOP>]
    let ``subsystemversion - negative values`` () =
        FSharp """module M
printfn "%A" System.DateTime.Now
let x = 0
x |> exit"""
        |> asExe
        |> withOptions ["--subsystemversion:-1.-2"]
        |> compile
        |> shouldFail
        |> withErrorCode 1051
        |> withDiagnosticMessageMatches "Invalid version '-1\\.-2' for '--subsystemversion'"
        |> ignore
