// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.UnitTests

open NUnit.Framework

[<TestFixture>]
module ``Map Tests`` =

    [<Test>]
    let ``Equality should be implemented on map``() =
        // Dev11:19569 - this used to throw an ArgumentException saying Object didn't implement IComparable
    
        let m = Map.ofArray [| 1, obj() |]
        let testDelegate = TestDelegate (fun _ -> (m = m) |> ignore)
    
        Assert.DoesNotThrow testDelegate