// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test
open FSharp.Compiler.Diagnostics

[<TestFixture>]
module ``Test Compiler Directives`` =

    [<Test>]
    let ``compiler #r "" is invalid``() =
        let source = """
#r ""
"""
        CompilerAssert.CompileWithErrors(
            Compilation.Create(source, Library),
            [|
                FSharpDiagnosticSeverity.Warning, 213, (2,1,2,6), "'' is not a valid assembly name"
            |])

    [<Test>]
    let ``compiler #r "   " is invalid``() =
        let source = """
#r "    "
"""
        CompilerAssert.CompileWithErrors(
            Compilation.Create(source, Library),
            [|
                FSharpDiagnosticSeverity.Warning, 213, (2,1,2,10), "'' is not a valid assembly name"
            |])
