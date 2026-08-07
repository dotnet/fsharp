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

// Surface area of the FSharp.Core assembly per host: a modern .NET host binds lib/net10.0,
// net472 binds lib/netstandard2.0. The netstandard2.1 surface is verified separately by the
// FSharp.Core.ApiCompat net10.0-vs-netstandard2.1 identity gate.
#if NET
            "net"
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
