// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module FSharp.Build.UnitTests.ResourceTests

open System.IO
open System.Reflection
open TestFramework
open Xunit

[<Theory>]
[<InlineData("plain", true)>]
[<InlineData("comma,here", true)>]
[<InlineData("comma, with space", true)>]
[<InlineData("plain", false)>]
[<InlineData("comma,here", false)>]
[<InlineData("comma, with space", false)>]
let ``SDK build embeds substitutions with intermediate path punctuation`` (directory, standardResourceNames: bool) =
    let output = createTemporaryDirectory().FullName
    let buildDirectory = Path.GetDirectoryName(initialConfig.FSharpBuild)
    let compiler = Path.Combine(repoRoot, "artifacts", "bin", "fsc", initialConfig.BUILD_CONFIG, productTfm, "fsc.dll")
    let intermediate = Path.Combine("obj", directory) + string Path.DirectorySeparatorChar
    File.WriteAllText(
        Path.Combine(output, "Directory.Build.props"),
        "<Project><PropertyGroup><ImportDirectoryPackagesProps>false</ImportDirectoryPackagesProps></PropertyGroup></Project>"
    )
    File.WriteAllText(Path.Combine(output, "Directory.Build.targets"), "<Project />")
    File.WriteAllText(Path.Combine(output, "Library.fs"), "module Library\nlet value = 42")
    File.WriteAllText(
        Path.Combine(output, "Library.fsproj"),
        $"""<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>{productTfm}</TargetFramework>
    <IntermediateOutputPath>{intermediate}</IntermediateOutputPath>
    <UseStandardResourceNames>{standardResourceNames}</UseStandardResourceNames>
    <FSharpTargetsShim>{Path.Combine(buildDirectory, "Microsoft.FSharp.NetSdk.targets")}</FSharpTargetsShim>
    <FSharpBuildAssemblyFile>{initialConfig.FSharpBuild}</FSharpBuildAssemblyFile>
    <DotnetFscCompilerPath>"{compiler}"</DotnetFscCompilerPath>
    <DisableImplicitFSharpCoreReference>true</DisableImplicitFSharpCoreReference>
    <NuGetAudit>false</NuGetAudit>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="Library.fs" />
    <Reference Include="FSharp.Core" HintPath="{typeof<unit>.Assembly.Location}" />
  </ItemGroup>
</Project>"""
    )

    let exitCode, stdout, stderr = Commands.executeProcess initialConfig.DotNetExe "build -v:q --disable-build-servers" output
    Assert.True((exitCode = 0), stdout + stderr)
    let assembly = Assembly.Load(File.ReadAllBytes(Path.Combine(output, "bin", "Debug", productTfm, "Library.dll")))
    use reader = new StreamReader(assembly.GetManifestResourceStream("ILLink.Substitutions.xml"))
    Assert.Equal(File.ReadAllText(Path.Combine(output, intermediate, productTfm, "ILLink.Substitutions.xml")), reader.ReadToEnd())
