module FSharpDiagServer.Tests.DesignTimeBuildTests

open Xunit
open FSharpDiagServer.DesignTimeBuild

[<Fact>]
let ``defaultConfig has expected values`` () =
    Assert.Equal(Some "net10.0", defaultConfig.TargetFramework)
    Assert.Equal("Release", defaultConfig.Configuration)

[<Fact>]
let ``DtbResult can hold compiler args`` () =
    let result = { CompilerArgs = [| "--debug"; "src/A.fs" |] }
    Assert.Equal(2, result.CompilerArgs.Length)
    Assert.Equal("--debug", result.CompilerArgs.[0])
