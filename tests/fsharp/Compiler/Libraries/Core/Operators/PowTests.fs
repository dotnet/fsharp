// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework

[<TestFixture>]
module ``Pow Tests`` =
    
    type T() =
        static let mutable m = false
        static member Pow (g: T, _: float) = 
            m <- true
            g
        static member Check() = m

    [<Test>]
    let ``Pow of custom type``() =
        // Regression test for FSHARP1.0:4487
        // Feature request: loosen Pow operator constraints

        let t = T()
        let _ = t ** 3.

        Assert.IsTrue (T.Check())