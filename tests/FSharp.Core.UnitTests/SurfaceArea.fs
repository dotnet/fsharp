// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Core.UnitTests.Portable.SurfaceArea

open Xunit
open System.IO
open System.Reflection
open FSharp.Test

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
    member _.VerifySurfaceAreaFSharpCore () : unit =
        let platform =

// We are testing the surface area of the FSharp.Core assembly.
#if NETCOREAPP
            "netstandard21"
#else
            "netstandard20"
#endif
        let flavor =
#if DEBUG
            "debug"
#else
            "release"
#endif
        let assembly = typeof<int list>.Assembly
        let baseline = Path.Combine(__SOURCE_DIRECTORY__, $"FSharp.Core.SurfaceArea.{platform}.{flavor}.bsl")
#if NETCOREAPP
        SurfaceArea.verifyIgnoringAssemblyReferences assembly baseline
#else
        SurfaceArea.verify assembly baseline
#endif

#if NETCOREAPP
    [<Fact>]
    member _.VerifyNetStandard21SurfaceAreaFSharpCore () : unit =
#if DEBUG
        let configuration = "Debug"
        let flavor = "debug"
#else
        let configuration = "Release"
        let flavor = "release"
#endif
        let assemblyPath =
            Path.Combine(__SOURCE_DIRECTORY__, "..", "..", "artifacts", "bin", "FSharp.Core", configuration, "netstandard2.1", "FSharp.Core.dll")
            |> Path.GetFullPath

        let assembly = Assembly.LoadFile assemblyPath
        let baseline = Path.Combine(__SOURCE_DIRECTORY__, $"FSharp.Core.SurfaceArea.netstandard21.{flavor}.bsl")
        SurfaceArea.verify assembly baseline
#endif
