// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework

[<TestFixture>]
module ``Round Tests`` =

    [<Test>]
    let ``Round of integers``() =
        for i in [1 .. 10000] do
            Assert.areEqual (i |> float   |> round) (float   i)
            Assert.areEqual (i |> float32 |> round) (float32 i)
            Assert.areEqual (i |> decimal |> round) (decimal i)

    [<Test>]
    let ``Round of floats``() =
        // Round down
        Assert.areEqual (round 1.1) 1.0
        Assert.areEqual (round 1.2) 1.0
        Assert.areEqual (round 1.3) 1.0
        Assert.areEqual (round 1.4) 1.0
        Assert.areEqual (round 1.1f) 1.0f
        Assert.areEqual (round 1.2f) 1.0f
        Assert.areEqual (round 1.3f) 1.0f
        Assert.areEqual (round 1.4f) 1.0f
        Assert.areEqual (round 1.1m) 1.0m
        Assert.areEqual (round 1.2m) 1.0m
        Assert.areEqual (round 1.3m) 1.0m
        Assert.areEqual (round 1.4m) 1.0m
        
        // Round down
        Assert.areEqual (round 1.6) 2.0
        Assert.areEqual (round 1.7) 2.0
        Assert.areEqual (round 1.8) 2.0
        Assert.areEqual (round 1.9) 2.0
        Assert.areEqual (round 1.6f) 2.0f
        Assert.areEqual (round 1.7f) 2.0f
        Assert.areEqual (round 1.8f) 2.0f
        Assert.areEqual (round 1.9f) 2.0f
        Assert.areEqual (round 1.6m) 2.0m
        Assert.areEqual (round 1.7m) 2.0m
        Assert.areEqual (round 1.8m) 2.0m
        Assert.areEqual (round 1.9m) 2.0m
        
        // Midpoint rounding. If between two numbers, round to the 'even' one.
        Assert.areEqual (round 1.5 ) 2.0 
        Assert.areEqual (round 1.5f) 2.0f
        Assert.areEqual (round 1.5m) 2.0m
        Assert.areEqual (round 2.5 ) 2.0 
        Assert.areEqual (round 2.5f) 2.0f
        Assert.areEqual (round 2.5m) 2.0m
        
        // If not midpoint, round to nearest as usual
        Assert.areEqual (round 2.500001 ) 3.0 
        Assert.areEqual (round 2.500001f) 3.0f
        Assert.areEqual (round 2.500001m) 3.0m