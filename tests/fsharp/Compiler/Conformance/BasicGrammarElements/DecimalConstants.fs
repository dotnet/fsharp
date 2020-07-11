// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework

[<TestFixture>]
module ``Decimal Constants`` =

    [<Test>]
    let ``Product of decimal constants``() =
        let oneOfOneMiDec = 1.0E-6M
        let oneMiDec      = 1.0E+6M

        Assert.areEqual 1.0M (oneOfOneMiDec * oneMiDec)

    [<Test>]
    let ``Sum of decimal constants``() =
        let x = 
            1.0E0M 
            + 2.0E1M
            + 3.0E2M
            + 4.0E3M
            + 5.0E4M
            + 6.0E5M
            + 7.0E6M 
            + 8.0E7M
            + 9.0E8M 
            + 1.0E-1M
            + 2.0E-2M
            + 3.0E-3M 
            + 4.0E-4M
            + 5.0E-5M 
            + 6.0E-6M 
            + 7.0E-7M 
            + 8.0E-8M 
            + 9.0E-9M

        Assert.areEqual 987654321.123456789M x

    [<Test>]
    let ``Sum of decimal literals with leading zero in exponential``() =
        let x = 1.0E00M + 2.0E01M + 3.E02M + 1.E-01M + 2.0E-02M

        Assert.areEqual 321.12M x

    [<Test>]
    let ``Non-representable small values are rounded to zero``() =
        // This test involves rounding of decimals. The F# design is to follow the BCL.
        // This means that the behavior is not deterministic, e.g. Mono and NetFx4 round; NetFx2 gives an error
        // This is a positive test on Dev10, at least until
        // FSHARP1.0:4523 gets resolved.

        Assert.areEqual 0.0M 1.0E-50M