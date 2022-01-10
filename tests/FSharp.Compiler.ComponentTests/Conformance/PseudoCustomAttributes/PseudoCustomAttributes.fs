// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Globalization

open Xunit
open FSharp.Test.Compiler
open System.IO
module ``PseudoCustomAttributes Test Cases`` =

    [<Fact>]
    let ``PseudoCustomAttributes - AssemblyAlgorithmId_001.fs``() =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "AssemblyAlgorithmId_001.fs")))
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``PseudoCustomAttributes - AssemblyCompany_001.fs``() =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "AssemblyCompany_001.fs")))
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``PseudoCustomAttributes - AssemblyConfiguration_001.fs``() =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "AssemblyConfiguration_001.fs")))
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``Globalization - AssemblyCopyright_001.fs``() =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "AssemblyCopyright_001.fs")))
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``PseudoCustomAttributes - AssemblyDescription_001.fs``() =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "AssemblyDescription_001.fs")))
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``PseudoCustomAttributes - AssemblyFileVersion_001.fs``() =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "AssemblyFileVersion_001.fs")))
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<Fact>]
    let ``PseudoCustomAttributes - AssemblyInformationalVersion_001.fs``() =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "AssemblyInformationalVersion_001.fs")))
        |> asExe
        |> compileExeAndRun
        |> shouldSucceed

    [<Fact>]
    let ``PseudoCustomAttributes - AssemblyTitle_001.fs``() =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "AssemblyTitle_001.fs")))
        |> asExe
        |> compileExeAndRun
        |> shouldSucceed

    [<Fact>]
    let ``PseudoCustomAttributes - AssemblyTrademark_001.fs``() =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "AssemblyTrademark_001.fs")))
        |> asExe
        |> compileExeAndRun
        |> shouldSucceed

    [<Fact>]
    let ``PseudoCustomAttributes - AssemblyVersion_001.fs``() =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "AssemblyVersion_001.fs")))
        |> asExe
        |> compileExeAndRun
        |> shouldSucceed

    [<Fact>]
    let ``PseudoCustomAttributes - AssemblyVersion_002.fs``() =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "AssemblyVersion_002.fs")))
        |> asExe
        |> compileExeAndRun
        |> shouldSucceed

    [<Fact>]
    let ``PseudoCustomAttributes - AssemblyVersion_003.fs``() =
        FSharp (loadSourceFromFile (Path.Combine(__SOURCE_DIRECTORY__, "AssemblyVersion_003.fs")))
        |> asExe
        |> withAssemblyVersion "4.5.6.7"
        |> ignoreWarnings
        |> compileExeAndRun
        |> shouldSucceed
