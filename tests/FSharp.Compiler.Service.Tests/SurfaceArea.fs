// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Service.Tests.SurfaceArea

open System.IO
open System.Reflection
open Xunit

type SurfaceAreaTest() =
    // This relies on a set of baselines to update the baseline set an environment variable before running the tests, then on failure the baselines will be updated
    // Handled by SurfaceArea.verify
    //
    // CMD:
    //    set TEST_UPDATE_BSL=1
    // PowerShell:
    //    $env:TEST_UPDATE_BSL=1
    // Linux/macOS:
    //    export TEST_UPDATE_BSL=1

    [<Fact>]
    member _.VerifySurfaceAreaFSharpCompilerService() =

        let platform = "netstandard20"

        let flavor =
#if DEBUG
            "debug"
#else
            "release"
#endif
        let assembly =
            let path = Path.Combine(Path.GetDirectoryName(typeof<int list>.Assembly.Location), "FSharp.Compiler.Service.dll")
            Assembly.LoadFrom path

        let baseline = Path.Combine(__SOURCE_DIRECTORY__, $"FSharp.Compiler.Service.SurfaceArea.{platform}.{flavor}.bsl")
        let outFileName = $"FSharp.Compiler.Service.SurfaceArea.{platform}.{flavor}.out"
        FSharp.Test.SurfaceArea.verify assembly baseline outFileName
