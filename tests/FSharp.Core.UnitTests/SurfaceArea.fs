// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Core.UnitTests.Portable.SurfaceArea

open Xunit
open System
open System.IO
open FSharp.Core.UnitTests.LibraryTestFx

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
    member this.VerifyArea () : unit =
        let platform =
#if NETCOREAPP
            "coreclr"
#else
            "net472"
#endif
        let flavor =
#if DEBUG
            "debug"
#else
            "release"
#endif
        let path = Path.Combine(__SOURCE_DIRECTORY__, $"FSharp.Core.SurfaceArea.{platform}.{flavor}.bsl")
        SurfaceArea.verify path platform flavor
