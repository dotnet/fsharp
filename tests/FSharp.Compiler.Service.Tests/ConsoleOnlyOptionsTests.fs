// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.Service.Tests.ConsoleOnlyOptionsTests

open System
open System.IO
open FSharp.Compiler.CompilerOptions
open NUnit.Framework
open TestDoubles

[<Test>]
let ``Help is displayed correctly`` () =
    let builder = getArbitraryTcConfigBuilder()
    let blocks = GetCoreFscCompilerOptions builder
    let expectedHelp = File.ReadAllText $"{__SOURCE_DIRECTORY__}/expected-help-output.txt"

    let help = GetHelpFsc builder blocks

    // contains instead of equals
    // as we don't control the 1st line of the output (the version)
    // it's tested separately
    StringAssert.Contains(expectedHelp, help.Replace("\r\n", Environment.NewLine))

[<Test>]
let ``Version is displayed correctly`` () =
    let builder = getArbitraryTcConfigBuilder()
    let expectedVersionPattern = @"Microsoft \(R\) F# Compiler version \d+\.\d+\.\d+\.\d+ for F# \d+\.\d+"

    let version = GetVersion builder

    Assert.That(version, Does.Match expectedVersionPattern)
