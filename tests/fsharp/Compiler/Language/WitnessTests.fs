// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open Xunit
open FSharp.Test
open FSharp.Test.Utilities
open FSharp.Test.Compiler
open TestFramework

#if !NETCOREAPP


module WitnessTests =

    [<Fact>]
    let ``Witness expressions are created as a result of compiling the type provider tests`` () =
        let dir = __SOURCE_DIRECTORY__ ++ "../../typeProviders/helloWorld"
        Fsx (sprintf """
#load @"%s"
        """ (dir ++ "provider.fsx"))
        |> asExe
        |> ignoreWarnings
        |> withLangVersion50
        |> compile
        |> shouldSucceed
        |> ignore
#endif
        

