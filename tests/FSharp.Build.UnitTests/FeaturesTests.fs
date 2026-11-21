// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module FSharp.Build.UnitTests.FeaturesTests

open System.IO
open Microsoft.Build.Utilities
open Xunit
open FSharp.Build
open FSharp.Build.UnitTests.BuildTaskTestHelpers
open TestFramework

[<Fact>]
let ``Each Features item becomes a --features option before OtherFlags`` () =
    let fsc = Fsc(BuildEngine = MockEngine(), Features = [| TaskItem("inherited=one"); TaskItem("mode=on,other=two") |], OtherFlags = "--features:last")
    let args = fsc.InternalGenerateResponseFileCommands()
    let positions = [ "--features:inherited=one"; "--features:mode=on,other=two"; "--features:last" ] |> List.map args.IndexOf
    Assert.True(positions.Head >= 0 && positions = List.sort positions, args)

[<Fact>]
let ``Features from Directory.Build.props select auto-opens in mixed-language SDK builds`` () =
    let directory = ResourceTests.createSdkProjectDirectory ()
    let write (name: string) (text: string) =
        Directory.CreateDirectory(Path.GetDirectoryName(Path.Combine(directory, name))) |> ignore
        File.WriteAllText(Path.Combine(directory, name), text)
    let props = Path.Combine(directory, "Directory.Build.props")
    File.WriteAllText(props, File.ReadAllText(props).Replace("<PropertyGroup>", "<PropertyGroup><LangVersion>preview</LangVersion><Features>inherited=one</Features>"))
    write "Provider.fs" """
namespace Providers
module Classic =
    let selected = 1
    let tag = 0
module Modern = let selected = 2
module Inherited = let tag = 10
[<assembly: AutoOpen("Providers.Classic")>]
[<assembly: ConditionalAutoOpen("Providers.Modern", "--features:runtime-async=on")>]
[<assembly: ConditionalAutoOpen("Providers.Inherited", "--features:inherited=one")>]
do ()
"""
    write "Provider.fsproj" """<Project Sdk="Microsoft.NET.Sdk"><ItemGroup><Compile Include="Provider.fs" /></ItemGroup></Project>"""
    write "shared/Shared.csproj" """<Project Sdk="Microsoft.NET.Sdk"><ItemGroup><ProjectReference Include="../Provider.fsproj" /></ItemGroup></Project>"""
    write "shared/Probe.cs" "namespace Shared; public static class Probe { public static int Value => Providers.Modern.selected; }"
    write "app/Program.fs" "module Program\n[<EntryPoint>]\nlet main _ = printf \"%d\" (selected + tag + Shared.Probe.Value); 0"
    write "app/Consumer.fsproj" """<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup><OutputType>Exe</OutputType><Features>$(Features);runtime-async=on</Features></PropertyGroup>
  <ItemGroup><ProjectReference Include="../Provider.fsproj" /><ProjectReference Include="../shared/Shared.csproj" /><Compile Include="Program.fs" /></ItemGroup>
</Project>"""
    for flags, expected in [ "", "14"; "-p:Features=runtime-async=on", "4"; "-p:Features=runtime-async=ON", "3" ] do
        let exitCode, stdout, stderr =
            Commands.executeProcess initialConfig.DotNetExe $"run --project app/Consumer.fsproj -v:q --disable-build-servers {flags}" directory
        Assert.True((exitCode = 0), stdout + stderr)
        Assert.Equal(expected, stdout.Trim())
