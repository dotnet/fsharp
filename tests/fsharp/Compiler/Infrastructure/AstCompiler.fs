// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests.AstCompiler

open FSharp.Test
open NUnit.Framework
open System.Reflection

[<TestFixture>]
module ``AST Compiler Smoke Tests`` =

    [<Test>]
    let ``Simple E2E module compilation``() =
        let assembly = 
            CompilerAssert.CompileOfAstToDynamicAssembly
                """
module TestModule

    let rec fib n = if n <= 1 then n else fib (n - 2) + fib (n - 1)
"""

        let method = assembly.GetType("TestModule").GetMethod("fib", BindingFlags.Static ||| BindingFlags.Public)
        Assert.NotNull(method)
        Assert.AreEqual(55, method.Invoke(null, [|10|]))

    [<Test>]
    let ``Compile to Assembly``() =
        let assembly = 
            CompilerAssert.CompileOfAst false
                """
module LiteralValue

[<Literal>]
let x = 7
"""

        (ILVerifier assembly).VerifyIL [
            """
.field public static literal int32 x = int32(0x00000007)
            """
        ]