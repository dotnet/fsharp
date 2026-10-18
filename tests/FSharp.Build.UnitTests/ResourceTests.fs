// Copyright (c) Microsoft Corporation. All Rights Reserved. See License.txt in the project root for license information.

module FSharp.Build.UnitTests.ResourceTests

open System.IO
open System.Reflection

open Microsoft.Build.Framework
open Microsoft.Build.Utilities

open FSharp.Build
open FSharp.Build.UnitTests.BuildTaskTestHelpers
open FSharp.Test.Compiler

open Scripting
open TestFramework
open Xunit

let private embeddedTextDirectory = __SOURCE_DIRECTORY__ ++ "resources" ++ "EmbeddedText"

let private createSdkProjectDirectory () =
    let output = createTemporaryDirectory().FullName
    let buildDirectory = Path.GetDirectoryName(initialConfig.FSharpBuild)
    File.WriteAllText(
        output ++ "Directory.Build.props",
        $"""<Project>
  <PropertyGroup>
    <ImportDirectoryPackagesProps>false</ImportDirectoryPackagesProps>
    <TargetFramework>{productTfm}</TargetFramework>
    <FSharpTargetsShim>{buildDirectory ++ "Microsoft.FSharp.NetSdk.targets"}</FSharpTargetsShim>
    <FSharpBuildAssemblyFile>{initialConfig.FSharpBuild}</FSharpBuildAssemblyFile>
    <DotnetFscCompilerPath>"{initialConfig.DOTNETFSCCOMPILERPATH}"</DotnetFscCompilerPath>
    <DisableImplicitFSharpCoreReference>true</DisableImplicitFSharpCoreReference>
    <NuGetAudit>false</NuGetAudit>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include="FSharp.Core" HintPath="{typeof<unit>.Assembly.Location}" />
  </ItemGroup>
</Project>"""
    )
    File.WriteAllText(output ++ "Directory.Build.targets", "<Project />")
    output

[<Theory>]
[<InlineData(false, ".fs")>]
[<InlineData(false, ".fsi")>]
[<InlineData(true, ".fs")>]
[<InlineData(true, ".fsi")>]
let ``Embedded text generates expected source`` (richText: bool, extension: string) =
    withTaskEnvironment (fun environment directory ->
        File.Copy(embeddedTextDirectory ++ "Messages.txt", directory.FullName ++ "Messages.txt")
        let output = Directory.CreateDirectory(directory.FullName ++ "obj")
        let item = TaskItem("Messages.txt") :> ITaskItem
        item.SetMetadata("RichText", if richText then "true" else "false")
        let engine = MockEngine()
        let task =
            FSharpEmbedResourceText(
                BuildEngine = engine,
                EmbeddedText = [| item |],
                IntermediateOutputPath = "obj"
            )
            |> assignTaskEnvironment environment

        Assert.True(task.Execute(), "Embedded text generation failed")
        Assert.Empty(engine.Errors)
        let mode = if richText then "RichText" else "Plain"
        checkBaseline
            (File.ReadAllText(output.FullName ++ $"Messages{extension}"))
            (embeddedTextDirectory ++ $"Messages.{mode}{extension}.bsl"))

[<Theory>]
[<InlineData("enable", "Debug")>]
[<InlineData("disable", "Debug")>]
[<InlineData("enable", "Release")>]
[<InlineData("disable", "Release")>]
let ``Embedded text builds and runs with nullable settings`` (nullable: string, configuration: string) =
    let output = createSdkProjectDirectory ()
    for name in [ "Messages.txt"; "Runtime.fsproj"; "Program.fs" ] do
        File.Copy(embeddedTextDirectory ++ name, output ++ name)

    let exitCode, stdout, stderr =
        Commands.executeProcess initialConfig.DotNetExe
            $"run --project Runtime.fsproj -c {configuration} -p:Nullable={nullable} -v:q --disable-build-servers"
            output
    Assert.True((exitCode = 0), stdout + stderr)
    Assert.Equal(nullable, stdout.Trim())

[<Theory>]
[<InlineData("plain", true)>]
[<InlineData("comma,here", true)>]
[<InlineData("comma, with space", true)>]
[<InlineData("plain", false)>]
[<InlineData("comma,here", false)>]
[<InlineData("comma, with space", false)>]
let ``SDK build embeds substitutions with intermediate path punctuation`` (directory, standardResourceNames: bool) =
    let output = createSdkProjectDirectory ()
    let intermediate = ("obj" ++ directory) + string Path.DirectorySeparatorChar
    File.WriteAllText(output ++ "Library.fs", "module Library\nlet value = 42")
    File.WriteAllText(
        output ++ "Library.fsproj",
        $"""<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <IntermediateOutputPath>{intermediate}</IntermediateOutputPath>
    <UseStandardResourceNames>{standardResourceNames}</UseStandardResourceNames>
  </PropertyGroup>
  <ItemGroup>
    <Compile Include="Library.fs" />
  </ItemGroup>
</Project>"""
    )

    let exitCode, stdout, stderr = Commands.executeProcess initialConfig.DotNetExe "build -v:q --disable-build-servers" output
    Assert.True((exitCode = 0), stdout + stderr)
    let assembly = Assembly.Load(File.ReadAllBytes(output ++ "bin" ++ "Debug" ++ productTfm ++ "Library.dll"))
    use reader = new StreamReader(assembly.GetManifestResourceStream("ILLink.Substitutions.xml"))
    Assert.Equal(File.ReadAllText(output ++ intermediate ++ productTfm ++ "ILLink.Substitutions.xml"), reader.ReadToEnd())
