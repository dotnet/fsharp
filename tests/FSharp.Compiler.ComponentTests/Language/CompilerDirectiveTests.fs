// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Language.ComponentTests

open Xunit
open FSharp.Test.Utilities
open FSharp.Test.Utilities.Compiler
open FSharp.Compiler.SourceCodeServices

module ``Test Compiler Directives`` =

    [<Fact>]
    let ``r# "" is invalid`` () =
        Fsx"""
#r ""
        """ |> ignoreWarnings
            |> compile
            |> shouldSucceed
            |> withWarning (213, (2,1,2,6), "'' is not a valid assembly name")

    [<Fact>]
    let ``#r "    " is invalid`` () =
        Fsx"""
#r "    "
        """ |> compile
            |> shouldFail
            |> withWarning (213, (2,1,2,10), "'' is not a valid assembly name")
