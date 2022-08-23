// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.Service.Tests.ConsoleOnlyOptionsTests

open FSharp.Compiler.Text
open NUnit.Framework
open System.IO
open FSharp.Compiler.CompilerConfig
open FSharp.Compiler.CompilerOptions
open Internal.Utilities
open FSharp.Compiler.AbstractIL.ILBinaryReader

[<Test>]
let ``Blah1`` () =
    // just a random thing to make things work
    let builder = TcConfigBuilder.CreateNew(
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

    let blocks = GetCoreServiceCompilerOptions builder
    
    let result = displayHelpFsc builder blocks
    ()

