// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open Xunit
open FSharp.Test
open FSharp.Compiler.Diagnostics


module ``Test Compiler Directives`` =

    [<Fact>]
    let ``compiler #r "" is invalid``() =
        let source = """
#r ""
"""
        CompilerAssert.CompileWithErrors(
            Compilation.Create("test.fsx", source, Library),
            [|
                FSharpDiagnosticSeverity.Warning, 213, (2,1,2,6), "'' is not a valid assembly name"
            |])

    [<Fact>]
    let ``compiler #r "   " is invalid``() =
        let source = """
#r "    "
"""
        CompilerAssert.CompileWithErrors(
            Compilation.Create("test.fsx", source, Library),
            [|
                FSharpDiagnosticSeverity.Warning, 213, (2,1,2,10), "'' is not a valid assembly name"
            |])
