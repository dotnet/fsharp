// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test.Utilities.Compiler
open FSharp.Tests

#if !NETCOREAPP

[<TestFixture>]
module WitnessTests =

    [<Test>]
    let ``Witness expressions are created as a result of compiling the type provider tests`` () =
        let dir = Core.getTestsDirectory "typeProviders/helloWorld"
        Fsx (sprintf """
#load @"%s"
        """ (dir ++ "provider.fsx"))
        |> asExe
        |> ignoreWarnings
        |> withLangVersionPreview
        |> compile
        |> shouldSucceed
        |> ignore
#endif
        

