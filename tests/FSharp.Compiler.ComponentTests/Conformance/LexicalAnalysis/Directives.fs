// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Conformance.LexicalAnalysis

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module Directives =

    // SOURCE: E_R_01.fsx SCFLAGS: --nologo
    [<Fact(Skip = "FSI test - requires different approach")>]
    let ``Directives - E_R_01_fsx`` () = ()

    // SOURCE: multiple_nowarn01.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Directives", Includes=[|"multiple_nowarn01.fs"|])>]
    let ``Directives - multiple_nowarn01_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: multiple_nowarn_many.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Directives", Includes=[|"multiple_nowarn_many.fs"|])>]
    let ``Directives - multiple_nowarn_many_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: multiple_nowarn_one.fs
    [<Theory; Directory(__SOURCE_DIRECTORY__ + "/../../resources/tests/Conformance/LexicalAnalysis/Directives", Includes=[|"multiple_nowarn_one.fs"|])>]
    let ``Directives - multiple_nowarn_one_fs`` compilation =
        compilation
        |> ignoreWarnings
        |> typecheck
        |> shouldSucceed
        |> ignore

    // SOURCE: multiple_nowarn01.fsx
    [<Fact(Skip = "FSI test - requires different approach")>]
    let ``Directives - multiple_nowarn01_fsx`` () = ()

    // SOURCE: dummy2.fsx SCFLAGS: --load:multiple_nowarn02.fsx --warnaserror+
    [<Fact(Skip = "FSI test - requires different approach")>]
    let ``Directives - dummy2_fsx`` () = ()

    // SOURCE: dummy.fsx SCFLAGS: --use:multiple_nowarn01.fsx --warnaserror+
    [<Fact(Skip = "FSI test - requires different approach")>]
    let ``Directives - dummy_fsx`` () = ()

    // SOURCE: load_script_with_multiple_nowarn01.fsx SCFLAGS: --warnaserror+
    [<Fact(Skip = "FSI test - requires different approach")>]
    let ``Directives - load_script_with_multiple_nowarn01_fsx`` () = ()

    // SOURCE: E_ShebangLocation.fsx
    [<Fact(Skip = "FSI test - requires different approach")>]
    let ``Directives - E_ShebangLocation_fsx`` () = ()
