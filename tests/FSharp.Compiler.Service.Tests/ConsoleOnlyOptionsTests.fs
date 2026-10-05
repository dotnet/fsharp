// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.Service.Tests.ConsoleOnlyOptionsTests

open System
open System.IO
open FSharp.Compiler.CompilerOptions
open FSharp.Compiler.Features
open FSharp.Compiler.Text.Range
open Xunit
open TestDoubles

[<Fact>]
let ``fsc help text is displayed correctly`` () =

     let builder = getArbitraryTcConfigBuilder()
     builder.showBanner <- false                     // We don't need the banner
     builder.TurnWarningOff(rangeCmdArgs, "75")      // We are going to use a test only flag
     builder.bufferWidth <- Some 80                  // Fixed width 80

     let expectedHelp = File.ReadAllText $"{__SOURCE_DIRECTORY__}/expected-help-output.bsl"

     let blocks = GetCoreFscCompilerOptions builder
     let help = GetHelpFsc builder blocks
     let actualHelp = help.Replace("\r\n", Environment.NewLine)

     Assert.Equal(expectedHelp, actualHelp)

[<Fact>]
let ``FSC version is displayed correctly`` () =
    let builder = getArbitraryTcConfigBuilder()
    let expectedVersionPattern = @"Microsoft \(R\) F# Compiler version \d+\.\d+\.\d+\.\d+ for F# \d+\.\d+"

    let version = GetVersion builder

    Assert.Matches(expectedVersionPattern, version)

[<Fact>]
let ``Language versions are displayed correctly`` () =
    let versions = GetLanguageVersions()

    Assert.Contains("Supported language versions", versions)
    Assert.Contains("preview", versions)
    Assert.Contains("default", versions)
    Assert.Contains("latest", versions)
    Assert.Contains("latestmajor", versions)
    Assert.Contains("11.2 (Default)", versions)

[<Theory>]
[<InlineData("MoreConcreteTiebreaker")>]
[<InlineData("OverloadResolutionPriority")>]
[<InlineData("RuntimeAsync")>]
[<InlineData("RecordConstructorSyntax")>]
[<InlineData("RequireNamedArguments")>]
[<InlineData("ReraiseInComputationExpressions")>]
[<InlineData("ExtensionConstraintSolutions")>]
let ``Preview features graduate to FSharp 11.2`` featureName =
    let feature = LanguageVersion.TryParseFeature(featureName) |> Option.get
    Assert.True(LanguageVersion.ContainsVersion "11.2")
    Assert.False(LanguageVersion("11.0").SupportsFeature feature)
    Assert.True(LanguageVersion("11.2").SupportsFeature feature)
    Assert.True(LanguageVersion.Default.SupportsFeature feature)
    Assert.True(LanguageVersion("preview").SupportsFeature feature)
    Assert.Equal("11.2", LanguageVersion.GetFeatureVersionString feature)
    Assert.False(LanguageVersion("11.2", [| feature |]).SupportsFeature feature)

[<Fact>]
let ``From end slicing remains preview only`` () =
    Assert.False(LanguageVersion("11.2").SupportsFeature LanguageFeature.FromEndSlicing)
    Assert.False(LanguageVersion.Default.SupportsFeature LanguageFeature.FromEndSlicing)
    Assert.True(LanguageVersion("preview").SupportsFeature LanguageFeature.FromEndSlicing)
    Assert.Equal("'PREVIEW'", LanguageVersion.GetFeatureVersionString LanguageFeature.FromEndSlicing)

[<Theory>]
[<InlineData("11", 110)>]
[<InlineData("11.0", 110)>]
[<InlineData("11.2", 112)>]
[<InlineData("default", 112)>]
[<InlineData("latest", 112)>]
[<InlineData("latestmajor", 112)>]
let ``Language version aliases select FSharp 11.2 without changing explicit versions`` version (expected: int) =
    Assert.Equal(decimal expected / 10m, LanguageVersion(version).SpecifiedVersion)