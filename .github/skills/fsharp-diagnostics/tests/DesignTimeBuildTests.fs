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

[<Fact>]
let ``custom DtbConfig overrides default values`` () =
    let config = { TargetFramework = Some "net8.0"; Configuration = "Debug" }
    Assert.Equal(Some "net8.0", config.TargetFramework)
    Assert.Equal("Debug", config.Configuration)

[<Fact>]
let ``DtbConfig with no TargetFramework`` () =
    let config = { TargetFramework = None; Configuration = "Release" }
    Assert.Equal(None, config.TargetFramework)

[<Fact>]
let ``DtbResult with empty CompilerArgs`` () =
    let result = { CompilerArgs = [||] }
    Assert.Empty(result.CompilerArgs)
