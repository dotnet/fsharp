// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.ErrorMessages

open Xunit
open FSharp.Test.Compiler

module ``Warning Test Cases`` =

    [<Fact>]
    let ``Main module of program is empty: -- when no main found``() =
        FSharp """
namespace MyNamespace1

module MyModule1 =
        let irrelevant = 10
        """ |> asExe
            |> ignoreWarnings
            |> compile
            |> shouldSucceed
            |> withDiagnostics [ Warning 988, Line 5, Col 28,  Line 5, Col 28, "Main module of program is empty: nothing will happen when it is run" ]
