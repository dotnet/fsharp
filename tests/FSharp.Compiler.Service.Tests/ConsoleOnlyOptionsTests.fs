// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.Service.Tests.ConsoleOnlyOptionsTests

open System
open System.IO
open FSharp.Compiler.CompilerOptions
open NUnit.Framework
open Tests.TestHelpers

[<Test>]
let ``Help is displayed correctly`` () =
    let builder = getArbitraryTcConfigBuilder()
    let blocks = GetCoreFscCompilerOptions builder
    let fileName = $"{Guid.NewGuid()}"
    let expectedHelp = File.ReadAllText $"{__SOURCE_DIRECTORY__}/expected-help-output.txt"
    let printer text = File.AppendAllText(fileName, text)
    let exiter () = ()

    DisplayHelpFsc printer exiter builder blocks

    let help = File.ReadAllText fileName
    // contains instead of equals
    // as we don't control the 1st line of the output (the version)
    // it's tested separately
    StringAssert.Contains(expectedHelp, help)

[<Test>]
let ``Version is displayed correctly`` () =
    let builder = getArbitraryTcConfigBuilder()
    let fileName = $"{Guid.NewGuid()}"
    let expectedVersionPattern = @"Microsoft \(R\) F# Compiler version \d+\.\d+\.\d+\.\d+ for F# \d+\.\d+"
    let printer text = File.AppendAllText(fileName, text)
    let exiter () = ()

    DisplayVersion printer exiter builder

    let version = File.ReadAllText fileName
    Assert.That(version, Does.Match expectedVersionPattern)
