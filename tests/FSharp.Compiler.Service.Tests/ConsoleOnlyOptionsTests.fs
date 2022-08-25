// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.Service.Tests.ConsoleOnlyOptionsTests

open System
open System.IO
open FSharp.Compiler.CompilerOptions
open NUnit.Framework
open TestDoubles

[<Test>]
let ``Help is displayed correctly`` () =
    try
        if System.Console.BufferWidth < 80 then
            System.Console.BufferWidth <- 80
    with _ -> ()

    let builder = getArbitraryTcConfigBuilder()
    builder.showBanner <- false                 // We don't need the banner

    let blocks = GetCoreFscCompilerOptions builder

    let expectedHelp = File.ReadAllText $"{__SOURCE_DIRECTORY__}/expected-help-output.bsl"
    let help = GetHelpFsc builder blocks

    let actualHelp = help.Replace("\r\n", Environment.NewLine)
    Assert.AreEqual(expectedHelp, actualHelp, $"Console width: {System.Console.BufferWidth}\nExpected: {expectedHelp}\n Actual: {actualHelp}") |> ignore

[<Test>]
let ``Version is displayed correctly`` () =
    let builder = getArbitraryTcConfigBuilder()
    let expectedVersionPattern = @"Microsoft \(R\) F# Compiler version \d+\.\d+\.\d+\.\d+ for F# \d+\.\d+"

    let version = GetVersion builder

    Assert.That(version, Does.Match expectedVersionPattern)
