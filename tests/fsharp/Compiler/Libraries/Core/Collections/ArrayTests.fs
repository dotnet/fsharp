// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test

[<TestFixture>]
module ``Array Tests`` =

    [<Test>]
    let removeAt() =
        Assert.AreEqual([|2..10|], (Array.removeAt 0 [|1..10|]))
        Assert.AreEqual([|1..9|], (Array.removeAt 9 [|1..10|]))
        Assert.AreEqual([|1; 2; 4; 5|], (Array.removeAt 2 [|1..5|]))
        
        
    [<Test>]
    let removeManyAt() =
        Assert.AreEqual([|3..10|], (Array.removeManyAt 0 2 [|1..10|]))
        Assert.AreEqual([|1..8|], (Array.removeManyAt 9 2 [|1..10|]))
        Assert.AreEqual([|1; 2; 5|], (Array.removeManyAt 2 2 [|1..5|]))
        
    [<Test>]
    let updateAt() =
        Assert.AreEqual([|-1; 2; 3|], (Array.updateAt 0 -1 [|1..3|]))
        Assert.AreEqual([|1; -1; 3|], (Array.updateAt 1 -1 [|1..3|]))
        Assert.AreEqual([|1; 2; -1|], (Array.updateAt 2 -1 [|1..3|]))
    
    [<Test>]
    let updateManyAt() =
        Assert.AreEqual([|-1; -2; 3; 4; 5|], (Array.updateManyAt 0 [-1; -2] [|1..5|]))
        Assert.AreEqual([|1; 2; 3; -1; -2|], (Array.updateManyAt 3 [-1; -2] [|1..5|]))
        Assert.AreEqual([|1; 2; -1; -2; 5|], (Array.updateManyAt 2 [-1; -2] [|1..5|]))
        
    [<Test>]
    let insertAt() =
        Assert.AreEqual([|-1; 1; 2; 3|], (Array.insertAt 0 -1 [|1..3|]))
        Assert.AreEqual([|1; -1; 2; 3|], (Array.insertAt 1 -1 [|1..3|]))
        Assert.AreEqual([|1; 2; 3; -1|], (Array.insertAt 3 -1 [|1..3|]))
        
    [<Test>]
    let insertManyAt() =
        Assert.AreEqual([|-1; -2; 1; 2; 3|], (Array.insertManyAt 0 [-1; -2] [|1..3|]))
        Assert.AreEqual([|1; -1; -2; 2; 3|], (Array.insertManyAt 1 [-1; -2] [|1..3|]))
        Assert.AreEqual([|1; 2; 3; -1; -2|], (Array.insertManyAt 3 [-1; -2] [|1..3|]))