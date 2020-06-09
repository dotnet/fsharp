// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test.Utilities
open FSharp.Compiler.SourceCodeServices

[<TestFixture>]
module ``Invalid Numeric Literal`` =

    [<Test>]
    let ``1up is invalid Numeric Literal``() =
        CompilerAssert.TypeCheckSingleError
            """
let foo = 1up // int
            """
            FSharpErrorSeverity.Error
            1156
            (2, 11, 2, 14)
            "This is not a valid numeric literal. Valid numeric literals include 1, 0x1, 0o1, 0b1, 1l (int), 1u (uint32), 1L (int64), 1UL (uint64), 1s (int16), 1y (sbyte), 1uy (byte), 1.0 (float), 1.0f (float32), 1.0m (decimal), 1I (BigInteger)."
