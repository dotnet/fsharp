// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Compiler.SourceCodeServices
open FSharp.Test.Utilities

[<TestFixture>]
module ``Sign Tests`` =

    // #Regression #Libraries #Operators 
    // Test sign function on unsigned primitives, should get error.

    [<Test>]
    let ``Sign of byte``() =
        CompilerAssert.TypeCheckSingleError
            """
sign 0uy |> ignore
            """
            FSharpErrorSeverity.Error
            1
            (2, 6, 2, 9)
            "The type 'byte' does not support the operator 'get_Sign'"

    [<Test>]
    let ``Sign of uint16``() =
        CompilerAssert.TypeCheckSingleError
            """
sign 0us |> ignore
            """
            FSharpErrorSeverity.Error
            1
            (2, 6, 2, 9)
            "The type 'uint16' does not support the operator 'get_Sign'"

    [<Test>]
    let ``Sign of uint32``() =
        CompilerAssert.TypeCheckSingleError
            """
sign 0u |> ignore
            """
            FSharpErrorSeverity.Error
            1
            (2, 6, 2, 8)
            "The type 'uint32' does not support the operator 'get_Sign'"

    [<Test>]
    let ``Sign of uint64``() =
        CompilerAssert.TypeCheckSingleError
            """
sign 0uL |> ignore
            """
            FSharpErrorSeverity.Error
            1
            (2, 6, 2, 9)
            "The type 'uint64' does not support the operator 'get_Sign'"