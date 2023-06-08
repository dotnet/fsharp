// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Core.UnitTests.Portable.SurfaceArea

open Xunit
open System
open System.IO
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
    member this.VerifySurfaceAreaFSharpCore () : unit =
        let platform =

// We are testing the surface area of the FSharp.Core assembly.
// NETCOREAPP builds with netstandard2.1
// Net472 builds with netstandard1.0
//
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
        let outFileName = $"FSharp.Core.SurfaceArea.{platform}.{flavor}.out"
        FSharp.Test.SurfaceArea.verify assembly baseline outFileName
