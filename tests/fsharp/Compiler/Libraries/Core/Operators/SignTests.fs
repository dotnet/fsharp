// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Compiler.Diagnostics
open FSharp.Test

[<TestFixture>]
module ``Sign Tests`` =

    [<Test>]
    let ``Sign of signed types``() =
        Assert.areEqual (sign 1y) 1       // byte
        Assert.areEqual (sign 1s) 1       // int16
        Assert.areEqual (sign 1) 1        // int32
        Assert.areEqual (sign 1L) 1       // int64
        Assert.areEqual (sign 1.0f) 1     // float
        Assert.areEqual (sign 1.0) 1      // double
        Assert.areEqual (sign 1.0m) 1     // decimal
        Assert.areEqual (sign 0y) 0       // byte
        Assert.areEqual (sign 0s) 0       // int16
        Assert.areEqual (sign 0) 0        // int32
        Assert.areEqual (sign 0L) 0       // int64
        Assert.areEqual (sign 0.0f) 0     // float
        Assert.areEqual (sign 0.0) 0      // double
        Assert.areEqual (sign 0.0m) 0     // decimal
        Assert.areEqual (sign -1y) -1     // byte
        Assert.areEqual (sign -1s) -1     // int16
        Assert.areEqual (sign -1) -1      // int32
        Assert.areEqual (sign -1L) -1     // int64
        Assert.areEqual (sign -1.0f) -1   // float
        Assert.areEqual (sign -1.0) -1    // double
        Assert.areEqual (sign -1.0m) -1   // decimal

    // #Regression #Libraries #Operators 
    // Test sign function on unsigned primitives, should get error.

    [<Test>]
    let ``Sign of byte``() =
        CompilerAssert.TypeCheckSingleError
            """
sign 0uy |> ignore
            """
            FSharpDiagnosticSeverity.Error
            1
            (2, 6, 2, 9)
            "The type 'byte' does not support the operator 'get_Sign'"

    [<Test>]
    let ``Sign of uint16``() =
        CompilerAssert.TypeCheckSingleError
            """
sign 0us |> ignore
            """
            FSharpDiagnosticSeverity.Error
            1
            (2, 6, 2, 9)
            "The type 'uint16' does not support the operator 'get_Sign'"

    [<Test>]
    let ``Sign of uint32``() =
        CompilerAssert.TypeCheckSingleError
            """
sign 0u |> ignore
            """
            FSharpDiagnosticSeverity.Error
            1
            (2, 6, 2, 8)
            "The type 'uint32' does not support the operator 'get_Sign'"

    [<Test>]
    let ``Sign of uint64``() =
        CompilerAssert.TypeCheckSingleError
            """
sign 0uL |> ignore
            """
            FSharpDiagnosticSeverity.Error
            1
            (2, 6, 2, 9)
            "The type 'uint64' does not support the operator 'get_Sign'"