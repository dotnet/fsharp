// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework

[<TestFixture>]
module ``Integer Constants`` =

    [<Test>]
    let ``Operations with negative one``() =
        // Verify the ability to specify negative numbers
        // (And not get confused wrt subtraction.)

        let x = -1

        Assert.areEqual -2 (x + x)
        Assert.areEqual  0 (x - x)
        Assert.areEqual  1 (x * x)
        Assert.areEqual  1 (x / x)

    [<Test>]
    let ``Operations with negative integers``() =
        // Verify the ability to specify negative numbers
        // (And not get confused wrt subtraction.)
        
        let fiveMinusSix   = 5 - 6
        let fiveMinusSeven = 5-7
        let negativeSeven  = -7
        
        Assert.areEqual -1       fiveMinusSix   
        Assert.areEqual -2       fiveMinusSeven 
        Assert.areEqual (-1 * 7) negativeSeven  

    [<Test>]
    let ``Functions with negative integers``() =
        // Verify the ability to specify negative numbers
        // (And not get confused wrt subtraction.)
        
        let ident x = x
        let add x y = x + y
        
        Assert.areEqual -10 (ident -10)
        Assert.areEqual -10 (add -5 -5)