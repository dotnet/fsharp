// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework

[<TestFixture>]
module AsyncExpressionSteppingTests =

    /// original: tests\fsharpqa\Source\CodeGen\EmittedIL\AsyncExpressionStepping\AsyncExpressionSteppingTest1.fs
    [<Test>]
    let AsyncExpressionSteppingTest1() =
        CompilerAssert.CompileLibraryAndVerifyIL
            """// #NoMono #NoMT #CodeGen #EmittedIL #Async  
module AsyncExpressionSteppingTest1      // Regression test for FSHARP1.0:4058
module AsyncExpressionSteppingTest1 = 

    let f1 () = 
        async { printfn "hello"
                printfn "stuck in the middle"
                printfn "goodbye"}

    let _ = f1() |> Async.RunSynchronously

"""
            (fun verifier ->
                verifier.VerifyILWithLineNumbers ("AsyncExpressionSteppingTest1.AsyncExpressionSteppingTest1.f1@6::Invoke", "(this will fail)")
            )

        