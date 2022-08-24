// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Compiler.Service.Tests.VisualStudioVersusConsoleContextTests

open NUnit.Framework
open FSharp.Compiler.CompilerOptions
open Tests.TestHelpers

// copypasted from the CompilerOptions code,
// not worth changing that code's accessibility just for this test
let private getOptionsFromOptionBlocks blocks =
    let GetOptionsOfBlock block =
        match block with
        | PublicOptions (_, opts) -> opts
        | PrivateOptions opts -> opts
    
    List.collect GetOptionsOfBlock blocks

[<Test>] // controls https://github.com/dotnet/fsharp/issues/13549
let ``Console-only options are filtered out for fsc in the VS context`` () =
    // just a random thing to make things work
    let builder = getArbitraryTcConfigBuilder()

    let blocks = GetCoreServiceCompilerOptions builder
    let options = getOptionsFromOptionBlocks blocks

    // this is a very whitebox testing but arguably better than nothing
    Assert.IsFalse(
        options 
        |> List.exists (function 
            | CompilerOption (_, _, OptionConsoleOnly _, _, _) -> true
            | _ -> false))
    
    // and a couple of shots in the dark
    Assert.False(
        options 
        |> List.exists (function
            // ignore deprecated options
            // one of them actually allows specifying the compiler version
            | CompilerOption (name, _, _, None, _) -> name = "version"
            | _ -> false))

    Assert.False(
        options 
        |> List.exists (function 
            | CompilerOption (name, _, _, _, _) -> name = "help"))
