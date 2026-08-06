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
// The netstandard2.1 surface is no longer exercised by a running test host; it is
// verified by the FSharp.Core.ApiCompat net10.0-vs-netstandard2.1 identity gate instead.
//
#if NET
            "net"
#elif NETCOREAPP
            // Currently unreachable for this test: a net5.0+ host defines NET (above), and net472 falls
            // to the netstandard20 branch below. Retained so a future .NET Standard 2.1 test host still
            // resolves the right baseline. The netstandard21 baselines are intentionally kept but no
            // longer regenerated here (ns2.1 coverage moved to the ApiCompat identity gate).
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
