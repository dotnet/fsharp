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
let ``Blah1`` () =
    let builder = getBuilder()
    let blocks = GetCoreFscCompilerOptions builder
    
    let print text = File.AppendAllText(@"C:\code\text.txt", text)
    let exit() = ()

    displayHelpFsc builder blocks print exit

    ()

[<Test>]
let ``Blah2`` () =
    let builder = getBuilder()

    let fileName = $"{Guid.NewGuid()}"
    let print text = File.AppendAllText(fileName, text)
    let exit() = ()

    displayVersion builder print exit

    let output = File.ReadAllText fileName

    ()

