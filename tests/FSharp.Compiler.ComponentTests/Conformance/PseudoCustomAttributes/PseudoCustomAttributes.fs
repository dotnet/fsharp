// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Globalization

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ``PseudoCustomAttributes Test Cases`` =

    let ``PseudoCustomAttributes - Compile and Run`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compileAndRun
        |> shouldSucceed

    let ``PseudoCustomAttributes - Fail to compile`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compile
        |> shouldFail

    [<Theory; FileInlineData("AssemblyCompany_001.fs")>]
    let ``PseudoCustomAttributes - AssemblyCompany_001_fs`` compilation =
        ``PseudoCustomAttributes - Compile and Run`` compilation

    [<Theory; FileInlineData("AssemblyConfiguration_001.fs")>]
    let ``PseudoCustomAttributes - AssemblyConfiguration_001_fs`` compilation =
        ``PseudoCustomAttributes - Compile and Run`` compilation

    [<Theory; FileInlineData("AssemblyCopyright_001.fs")>]
    let ``PseudoCustomAttributes - AssemblyCopyright_001_fs`` compilation =
        ``PseudoCustomAttributes - Compile and Run`` compilation

    [<Theory; FileInlineData("AssemblyDescription_001.fs")>]
    let ``PseudoCustomAttributes - AssemblyDescription_001_fs`` compilation =
        ``PseudoCustomAttributes - Compile and Run`` compilation

    [<Theory; FileInlineData("AssemblyFileVersion_001.fs")>]
    let ``PseudoCustomAttributes - AssemblyFileVersion_001_fs`` compilation =
        ``PseudoCustomAttributes - Compile and Run`` compilation

    [<Theory; FileInlineData("AssemblyInformationalVersion_001.fs")>]
    let ``PseudoCustomAttributes - AssemblyInformationalVersion_001_fs`` compilation =
        ``PseudoCustomAttributes - Compile and Run`` compilation

    [<Theory; FileInlineData("AssemblyTitle_001.fs")>]
    let ``PseudoCustomAttributes - AssemblyTitle_001_fs`` compilation =
        ``PseudoCustomAttributes - Compile and Run`` compilation

    [<Theory; FileInlineData("AssemblyTrademark_001.fs")>]
    let ``PseudoCustomAttributes - AssemblyTrademark_001_fs`` compilation =
        ``PseudoCustomAttributes - Compile and Run`` compilation

    [<Theory; FileInlineData("AssemblyVersion_001.fs")>]
    let ``PseudoCustomAttributes - AssemblyVersion_001_fs`` compilation =
        ``PseudoCustomAttributes - Compile and Run`` compilation

    [<Theory; FileInlineData("AssemblyVersion_002.fs")>]
    let ``PseudoCustomAttributes - AssemblyVersion_002_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> compileExeAndRun
        |> shouldSucceed

    [<Theory; FileInlineData("AssemblyVersion_003.fs")>]
    let ``PseudoCustomAttributes - AssemblyVersion_003_fs`` compilation =
        compilation
        |> getCompilation
        |> asExe
        |> withAssemblyVersion "4.5.6.7"
        |> ignoreWarnings
        |> compileExeAndRun
        |> shouldSucceed
