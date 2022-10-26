// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.Service.Tests.TestDoubles

open System.IO
open FSharp.Compiler.Text
open FSharp.Compiler.AbstractIL.ILBinaryReader
open FSharp.Compiler.CompilerConfig
open Internal.Utilities

// just a random thing to make things work
let internal getArbitraryTcConfigBuilder() =
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
