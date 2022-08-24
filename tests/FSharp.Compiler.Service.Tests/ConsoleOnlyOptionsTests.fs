// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.Service.Tests.ConsoleOnlyOptionsTests

open System
open System.IO
open FSharp.Compiler.Text
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerOptions
open Internal.Utilities
open NUnit.Framework

// just a random thing to make things work
let private getBuilder() =
    TcConfigBuilder.CreateNew(
        null,
        FSharpEnvironment.BinFolderOfDefaultFSharpCompiler(None).Value,
        ReduceMemoryFlag.Yes,
        Directory.GetCurrentDirectory(),
        false,
        false,
        CopyFSharpCoreFlag.No,
        (fun _ -> None),
        None,
        Range.Zero)

[<Test>]
let ``Help is displayed correctly`` () =
    let builder = getBuilder()
    let blocks = GetCoreFscCompilerOptions builder
    let fileName = $"{Guid.NewGuid()}"
    let expectedHelp = File.ReadAllText $"{__SOURCE_DIRECTORY__}/expected-help-output.txt"
    let print text = File.AppendAllText(fileName, text)
    let exit() = ()

    DisplayHelpFsc print exit builder blocks

    let help = File.ReadAllText fileName
    // contains instead of equals
    // as we don't control the 1st line of the output (the version)
    // it's tested separately
    StringAssert.Contains(expectedHelp, help)

[<Test>]
let ``Version is displayed correctly`` () =
    let builder = getBuilder()
    let fileName = $"{Guid.NewGuid()}"
    let expectedVersionPattern = @"Microsoft \(R\) F# Compiler version \d+\.\d+\.\d+\.\d+ for F# \d+\.\d+"
    let print text = File.AppendAllText(fileName, text)
    let exit() = ()

    DisplayVersion print exit builder

    let version = File.ReadAllText fileName
    Assert.That(version, Does.Match expectedVersionPattern)
