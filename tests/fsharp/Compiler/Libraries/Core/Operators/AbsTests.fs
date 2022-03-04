// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Compiler.Diagnostics
open FSharp.Test

[<TestFixture>]
module ``Abs Tests`` =

    [<Test>]
    let  ``Abs of signed integral types``() =
        // Regression test for FSHARP1.0:3470 - exception on abs of native integer

        Assert.areEqual (abs -1y) 1y   // signed byte
        Assert.areEqual (abs -1s) 1s   // int16
        Assert.areEqual (abs -1l) 1l   // int32
        Assert.areEqual (abs -1n) 1n   // nativeint
        Assert.areEqual (abs -1L) 1L   // int64
        Assert.areEqual (abs -1I) 1I   // bigint

    [<Test>]
    let ``Abs of byte``() =
        CompilerAssert.TypeCheckSingleError
            """
abs -1uy |> ignore
            """
            FSharpDiagnosticSeverity.Error
            1
            (2, 6, 2, 9)
            "The type 'byte' does not support the operator 'Abs'"
    
    [<Test>]
    let ``Abs of uint16``() =
        CompilerAssert.TypeCheckSingleError
            """
abs -1us |> ignore
            """
            FSharpDiagnosticSeverity.Error
            1
            (2, 6, 2, 9)
            "The type 'uint16' does not support the operator 'Abs'"

    [<Test>]
    let ``Abs of uint32``() =
        CompilerAssert.TypeCheckSingleError
            """
abs -1ul |> ignore
            """
            FSharpDiagnosticSeverity.Error
            1
            (2, 6, 2, 9)
            "The type 'uint32' does not support the operator 'Abs'"

        CompilerAssert.TypeCheckSingleError
            """
abs -1u |> ignore
            """
            FSharpDiagnosticSeverity.Error
            1
            (2, 6, 2, 8)
            "The type 'uint32' does not support the operator 'Abs'"
            
    [<Test>]
    let ``Abs of unativeint``() =
        CompilerAssert.TypeCheckSingleError
            """
abs -1un |> ignore
            """
            FSharpDiagnosticSeverity.Error
            1
            (2, 6, 2, 9)
            "The type 'unativeint' does not support the operator 'Abs'"
            
    [<Test>]
    let ``Abs of uint64``() =
        CompilerAssert.TypeCheckSingleError
            """
abs -1uL |> ignore
            """
            FSharpDiagnosticSeverity.Error
            1
            (2, 6, 2, 9)
            "The type 'uint64' does not support the operator 'Abs'"
            
        CompilerAssert.TypeCheckSingleError
            """
abs -1UL |> ignore
            """
            FSharpDiagnosticSeverity.Error
            1
            (2, 6, 2, 9)
            "The type 'uint64' does not support the operator 'Abs'"