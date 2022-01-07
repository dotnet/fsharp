// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.Globalization

open Xunit
open FSharp.Test.Compiler
open FSharp.Test.Xunit.Attributes

module ``PseudoCustomAttributes Test Cases`` =

    let ``PseudoCustomAttributes - Compile and Run`` compilation =
        compilation
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyAlgorithmId_001.fs"|])>]
    let ``PseudoCustomAttributes - AssemblyAlgorithmId_001_fs`` compilation =
        ``PseudoCustomAttributes - Compile and Run`` compilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyCompany_001.fs"|])>]
    let ``PseudoCustomAttributes - AssemblyCompany_001_fs`` compilation =
        ``PseudoCustomAttributes - Compile and Run`` compilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyConfiguration_001.fs"|])>]
    let ``PseudoCustomAttributes - AssemblyConfiguration_001_fs`` compilation =
        ``PseudoCustomAttributes - Compile and Run`` compilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyCopyright_001.fs"|])>]
    let ``PseudoCustomAttributes - AssemblyCopyright_001_fs`` compilation =
        ``PseudoCustomAttributes - Compile and Run`` compilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyDescription_001.fs"|])>]
    let ``PseudoCustomAttributes - AssemblyDescription_001_fs`` compilation =
        ``PseudoCustomAttributes - Compile and Run`` compilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyFileVersion_001.fs"|])>]
    let ``PseudoCustomAttributes - AssemblyFileVersion_001_fs`` compilation =
        ``PseudoCustomAttributes - Compile and Run`` compilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyInformationalVersion_001.fs"|])>]
    let ``PseudoCustomAttributes - AssemblyInformationalVersion_001_fs`` compilation =
        ``PseudoCustomAttributes - Compile and Run`` compilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyTitle_001.fs"|])>]
    let ``PseudoCustomAttributes - AssemblyTitle_001_fs`` compilation =
        ``PseudoCustomAttributes - Compile and Run`` compilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyTrademark_001.fs"|])>]
    let ``PseudoCustomAttributes - AssemblyTrademark_001_fs`` compilation =
        ``PseudoCustomAttributes - Compile and Run`` compilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyVersion_001.fs"|])>]
    let ``PseudoCustomAttributes - AssemblyVersion_001_fs`` compilation =
        ``PseudoCustomAttributes - Compile and Run`` compilation

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyVersion_002.fs"|])>]
    let ``PseudoCustomAttributes - AssemblyVersion_002_fs`` compilation =
        compilation
        |> asExe
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; Directory(__SOURCE_DIRECTORY__, Includes=[|"AssemblyVersion_003.fs"|])>]
    let ``PseudoCustomAttributes - AssemblyVersion_003_fs`` compilation =
        compilation
        |> asExe
        |> withAssemblyVersion "4.5.6.7"
        |> ignoreWarnings
        |> compileExeAndRun
        |> shouldSucceed
