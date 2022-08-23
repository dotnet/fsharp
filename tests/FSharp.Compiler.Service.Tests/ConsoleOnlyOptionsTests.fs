// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.Service.Tests.ConsoleOnlyOptionsTests

open FSharp.Compiler.Text
open NUnit.Framework
open System.IO
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerOptions
open Internal.Utilities
open FSharp.Compiler.AbstractIL.ILBinaryReader
open System

let private getBuilder() =
    // just a random thing to make things work
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
    displayHelpFsc builder blocks print exit

    let help = File.ReadAllText fileName
    Assert.AreEqual(expectedHelp, help)
    
    ()

[<Test>]
let ``Version is displayed correctly`` () =
    let builder = getBuilder()
    let fileName = $"{Guid.NewGuid()}"
    let expectedVersionPattern = @"Microsoft \(R\) F# Compiler version \d+\.\d+\.\d+\.\d+ for F# \d+\.\d+"

    let print text = File.AppendAllText(fileName, text)
    let exit() = ()
    displayVersion builder print exit

    let version = File.ReadAllText fileName
    Assert.That(version, Does.Match expectedVersionPattern)
