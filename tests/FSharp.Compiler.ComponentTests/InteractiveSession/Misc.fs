// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

/// Tests for FSI Interactive Session - migrated from tests/fsharpqa/Source/InteractiveSession/Misc/
/// NOTE: Many InteractiveSession tests from fsharpqa require FSI-specific features (fsi.CommandLineArgs, 
/// FSIMODE=PIPE with stdin, #r with relative paths, etc.) that cannot be easily migrated to the 
/// ComponentTests framework which runs FSI externally. The tests migrated here are the subset that
/// work with the runFsi external process approach.
namespace InteractiveSession

open Xunit
open FSharp.Test.Compiler

module Misc =

    // ================================================================================
    // Success tests - verify FSI can handle various scenarios
    // ================================================================================

    // Regression test for FSHARP1.0:5599 - Empty list in FSI
    [<Fact>]
    let ``EmptyList - empty list literal``() =
        Fsx """
[];;
exit 0;;
"""
        |> withOptions ["--nologo"]
        |> runFsi
        |> shouldSucceed
        |> ignore

    // ToString returning null should not crash FSI
    [<Fact>]
    let ``ToStringNull - null ToString in FSI``() =
        Fsx """
type NullToString() = 
  override __.ToString() = null;;

let n = NullToString();;
exit 0;;
"""
        |> withOptions ["--nologo"]
        |> runFsi
        |> shouldSucceed
        |> ignore

    // Declare event in FSI
    [<Fact>]
    let ``DeclareEvent``() =
        Fsx """
type T() =
    [<CLIEvent>]
    member x.Event = Event<int>().Publish;;

let test = new T();;
exit 0;;
"""
        |> withOptions ["--nologo"]
        |> runFsi
        |> shouldSucceed
        |> ignore

    // ================================================================================
    // Error tests - verify FSI properly reports errors
    // ================================================================================

    // Regression test for FSHARP1.0:5629 - let =
    [<Fact>]
    let ``E_let_equal01 - incomplete let binding``() =
        Fsx """
let = ;;
exit 1;;
"""
        |> withOptions ["--nologo"]
        |> runFsi
        |> shouldFail
        |> withStdErrContains "Unexpected symbol '=' in binding"
        |> ignore

    // Regression test for FSHARP1.0:5629 - let f
    [<Fact>]
    let ``E_let_id - incomplete binding``() =
        Fsx """
let f;;
exit 1;;
"""
        |> withOptions ["--nologo"]
        |> runFsi
        |> shouldFail
        |> withStdErrContains "Incomplete structured construct"
        |> ignore

    // Regression test for FSHARP1.0:5629 - let mutable =
    [<Fact>]
    let ``E_let_mutable_equal``() =
        Fsx """
let mutable = ;;
exit 1;;
"""
        |> withOptions ["--nologo"]
        |> runFsi
        |> shouldFail
        |> withStdErrContains "Unexpected symbol '=' in binding"
        |> ignore

    // Regression test for FSHARP1.0:5629 - empty record
    [<Fact>]
    let ``E_emptyRecord``() =
        Fsx """
type R = { };;
exit 1;;
"""
        |> withOptions ["--nologo"]
        |> runFsi
        |> shouldFail
        |> withStdErrContains "Expecting record field"
        |> ignore

    // Regression test for FSHARP1.0:5629 - type R = |
    [<Fact>]
    let ``E_type_id_equal_pipe``() =
        Fsx """
type R = | ;;
exit 1;;
"""
        |> withOptions ["--nologo"]
        |> runFsi
        |> shouldFail
        |> withStdErrContains "Incomplete structured construct"
        |> ignore

    // Regression test for FSharp1.0:5260 and FSHARP1.0:5270 - global.Microsoft
    [<Fact>]
    let ``E_GlobalMicrosoft``() =
        Fsx """
global.Microsoft;;
exit 1;;
"""
        |> withOptions ["--nologo"]
        |> runFsi
        |> shouldFail
        |> withStdErrContains "is not defined"
        |> ignore

    // Regression test for FSharp1.0:4164 - malformed range operator
    // Verifies FSI produces proper error without "fsbug" internal error
    [<Fact>]
    let ``E_RangeOperator01 - malformed range operator``() =
        Fsx """
aaaa..;;
"""
        |> withOptions ["--nologo"]
        |> runFsi
        |> shouldFail
        |> withStdErrContains "Incomplete expression"
        |> ignore
