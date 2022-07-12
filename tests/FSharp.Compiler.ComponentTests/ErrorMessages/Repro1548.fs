// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.ComponentTests.ErrorMessages

open Xunit
open FSharp.Test.Compiler
open FSharp.Test.Utilities
open NUnit.Framework
open FSharp.Compiler.Diagnostics
open FSharp.Test


module ``Repro 1548`` =

    [<Fact>]
    let ``The type 'Inherit from non defined type``() =
        let reference =
            let source = """
namespace B
type PublicType =
    member _.Y() = ()
            """
            Compilation.Create(source, Fs, CompileOutput.Library)  |> CompilationReference.CreateFSharp

        let testCmpl =
            let source = """
module Test

type E() =
    inherit B.Type()
    member x.Y() = ()
            """
            Compilation.Create(source, Fs, CompileOutput.Exe, options = [||], cmplRefs = [reference])

        CompilerAssert.CompileWithErrors(testCmpl, [| (FSharpDiagnosticSeverity.Error, 39, (5, 15, 5, 19), "The type 'Type' is not defined in 'B'.") |])
