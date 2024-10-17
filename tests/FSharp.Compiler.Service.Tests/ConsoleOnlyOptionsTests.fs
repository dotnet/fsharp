// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.Service.Tests.ConsoleOnlyOptionsTests

open System
open System.IO
open FSharp.Compiler.CompilerOptions
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
    Assert.Contains("(Default)", versions)