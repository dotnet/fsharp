// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests.CodeGen.EmittedIL

open FSharp.Test
open NUnit.Framework

[<TestFixture>]
module DebugScopes =

    [<Test>]
    let SimpleFunction() =
        CompilerAssert.CompileLibraryAndVerifyDebugInfoWithOptions(
            [|"--debug:portable"; "--optimize-"; "--optimize-"|],
            (__SOURCE_DIRECTORY__ + "/SimpleFunction.debuginfo.expected"),
            """
module Test
let f x = 
    let y = 1
    2
            """)

    [<Test>]
    let SimpleShadowingFunction() =
        CompilerAssert.CompileLibraryAndVerifyDebugInfoWithOptions(
            [|"--debug:portable"; "--optimize-"; "--optimize-"|],
            (__SOURCE_DIRECTORY__ + "/SimpleShadowingFunction.debuginfo.expected"),
            """
module Test
let f x = 
    let y = 1
    let y = y+1
    let y = y+1
    2
            """)

    [<Test>]
    let ComplexShadowingFunction() =
        CompilerAssert.CompileLibraryAndVerifyDebugInfoWithOptions(
            [|"--debug:portable"; "--optimize-"; "--optimize-"|],
            (__SOURCE_DIRECTORY__ + "/ComplexShadowingFunction.debuginfo.expected"),
            """
module Test

let f2 (a, b) =
    let v1 = 1
    if a then
        let v2 = 1.4
        if b then
           let v1 = "3"
           let v2 = 5
           v1
        else
           let v1 = "3"
           let v2 = 5
           v1
    else
        let v2 = 1.4
        if b then
           let v1 = "3"
           let v2 = 5
           v1
        else
           let v1 = "3"
           let v2 = 5
           v1



            """)

