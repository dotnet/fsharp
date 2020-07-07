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
        """ |> compile
            |> shouldFail
            |> withWarnings [213]

    [<Fact>]
    let ``#r "   " is invalid`` () =
        Fsx"""
#r "   "
        """ |> compile
            |> shouldFail
            |> withWarnings [213]
