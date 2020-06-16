// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework
open FSharp.Test.Utilities

[<TestFixture>]
module ``Comparison Tests`` =

    type 'a www = W of 'a

    [<Test>]
    let ``Comparisons with wrapped NaN``() =
        // Regression test for FSHARP1.0:5640
        // This is a sanity test: more coverage in FSHARP suite...

        Assert.IsFalse (W System.Double.NaN = W System.Double.NaN)
        Assert.IsTrue ((W System.Double.NaN).Equals(W System.Double.NaN))
        Assert.areEqual (compare (W System.Double.NaN) (W System.Double.NaN)) 0

    [<Test>]
    let ``Comparisons with wrapped NaN in FSI``() =
        // Regression test for FSHARP1.0:5640
        // This is a sanity test: more coverage in FSHARP suite...

        CompilerAssert.RunScriptWithOptions [| "--langversion:preview" |]
            """
type 'a www = W of 'a

let assertTrue a =
    if (not a) then failwithf "Expected true, but found false."
    ()

let p = W System.Double.NaN = W System.Double.NaN
let q = (W System.Double.NaN).Equals(W System.Double.NaN)
let z = compare (W System.Double.NaN) (W System.Double.NaN)

assertTrue (not p)
assertTrue q
assertTrue (z = 0)
            """
            []