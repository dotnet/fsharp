// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module FSharp.Build.UnitTests.FeaturesTests

open System
open System.IO
open Xunit
open FSharp.Build
open FSharp.Build.UnitTests.BuildTaskTestHelpers
open TestFramework

[<Theory>]
[<InlineData("mode=on,other=two")>]
[<InlineData("opaque value=one=two")>]
let ``Features preserve opaque CLI operands before OtherFlags`` (entry: string) =
    let host = FauxHostObject()
    let task = Fsc(BuildEngine = MockEngine(),
                   Features = [|Microsoft.Build.Utilities.TaskItem("inherited=one"); Microsoft.Build.Utilities.TaskItem(entry)|],
                   OtherFlags = "--features:mode=off", HostObject = host)
    task.InternalGenerateResponseFileCommands() |> ignore
    Assert.Equal(0, task.InternalExecuteTool("", "", ""))
    let features = host.Flags |> Array.filter (fun flag -> flag.StartsWith("--features:", StringComparison.Ordinal))
    Assert.Equal<string[]>([|"--features:inherited=one"; $"--features:{entry}"; "--features:mode=off"|], features)

[<Fact>]
let ``Features select real symbols across inherited and incremental SDK builds`` () =
    let directory = createSdkProjectDirectory()
    let write name (text: string) = File.WriteAllText(Path.Combine(directory, name), text)
    let props = Path.Combine(directory, "Directory.Build.props")
    File.WriteAllText(props, File.ReadAllText(props).Replace("<PropertyGroup>", """<PropertyGroup>
    <LangVersion>preview</LangVersion>
    <Features>
      inherited=one;
      runtime-async=off
    </Features>"""))
    write "Provider.fs" """
namespace Providers
module Classic =
    let selected = 1
    let inheritedTag = 0
module Modern = let selected = 2
module Shared = let inheritedTag = 3
[<assembly: AutoOpen("Providers.Classic")>]
[<assembly: ConditionalAutoOpen("Providers.Modern", "--features:runtime-async=on")>]
[<assembly: ConditionalAutoOpen("Providers.Shared", "--features:inherited=one")>]
do ()
"""
    write "Provider.fsproj" """<Project Sdk="Microsoft.NET.Sdk"><ItemGroup><Compile Include="Provider.fs" /></ItemGroup></Project>"""
    Directory.CreateDirectory(Path.Combine(directory, "shared")) |> ignore
    write (Path.Combine("shared", "Shared.csproj")) """<Project Sdk="Microsoft.NET.Sdk"><ItemGroup><ProjectReference Include="..\Provider.fsproj" /></ItemGroup></Project>"""
    write (Path.Combine("shared", "Probe.cs")) "namespace Shared; public static class Probe { public static int Value => Providers.Modern.selected; }"
    Directory.CreateDirectory(Path.Combine(directory, "app")) |> ignore
    write (Path.Combine("app", "Program.fs")) """
module Program
[<EntryPoint>]
let main _ =
    if Shared.Probe.Value <> 2 then failwith "Mixed-language reference"
    printf "%d" (selected + inheritedTag)
    0
"""
    write (Path.Combine("app", "Consumer.fsproj")) """<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <OutputType>Exe</OutputType>
    <Features>$(Features);runtime-async=on</Features>
  </PropertyGroup>
  <ItemGroup>
    <ProjectReference Include="..\Provider.fsproj" />
    <ProjectReference Include="..\shared\Shared.csproj" />
    <Compile Include="Program.fs" />
  </ItemGroup>
</Project>"""
    for flags, expected in
        ["", "5"
         "-p:Features=runtime-async=ON", "1"
         "-p:Features=runtime-async=on", "2"
         "-p:OtherFlags=--features:runtime-async=off", "5"
         "-p:Features=--features:runtime-async=on -p:OtherFlags=", "1"
         "-p:Features= -p:OtherFlags=--features:runtime-async=on", "2"] do
        let exitCode, stdout, stderr =
            Commands.executeProcess initialConfig.DotNetExe
                $"run --project app{Path.DirectorySeparatorChar}Consumer.fsproj -v:q --disable-build-servers {flags}" directory
        Assert.True((exitCode = 0), stdout + stderr)
        Assert.Equal(expected, stdout.Trim())
    let exitCode, stdout, stderr =
        Commands.executeProcess initialConfig.DotNetExe
            $"msbuild app{Path.DirectorySeparatorChar}Consumer.fsproj -t:Compile -p:DesignTimeBuild=true -p:BuildingInsideVisualStudio=true -p:BuildingProject=false -p:ProvideCommandLineArgs=true -p:SkipCompilerExecution=true -getItem:FscCommandLineArgs -v:q" directory
    Assert.True((exitCode = 0), stdout + stderr)
    for flag in ["inherited=one"; "runtime-async=off"; "runtime-async=on"] do
        Assert.Contains($"\"--features:{flag}\"", stdout, StringComparison.Ordinal)
