// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Core.UnitTests.Portable.SurfaceArea

open Xunit
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
// A modern .NET host (net11+) binds lib/net10.0, net472 binds lib/netstandard2.0.
// netstandard2.1 is no longer host-loaded here; its identity is verified by the
// FSharp.Core.ApiCompat net10.0-vs-netstandard2.1 gate.
//
#if NET
            "net"
#elif NETCOREAPP
            // Unreachable today (a net5.0+ host defines NET); kept for a future .NET Standard 2.1 host.
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
        SurfaceArea.verify assembly baseline
