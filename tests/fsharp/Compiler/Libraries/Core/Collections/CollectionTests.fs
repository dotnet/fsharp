// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework

[<TestFixture>]
module ``Array2D Tests`` =

    [<Test>]
    let ``Iter should not throw on non-zero based 2D arrays``() =
        // Regression for FSHARP1.0: 5919
        // bug in array2D functions would cause iter to blow up

        let a = Array2D.createBased 1 5 10 10 0.0
        let testDelegate = TestDelegate (fun _ -> a |> Array2D.iter (printf "%f"))

        Assert.DoesNotThrow testDelegate

    [<Test>]
    let ``Iteri should not throw on non-zero based 2D arrays``() =
        // Regression for FSHARP1.0: 5919
        // bug in array2D functions would cause iteri to blow up

        let a = Array2D.createBased 1 5 10 10 0.0
        let testDelegate = TestDelegate (fun _ -> a |> Array2D.iteri (fun _ _ x -> printf "%f" x))

        Assert.DoesNotThrow testDelegate