// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Core.UnitTests.FSharp_Core.Linq.NullableOperators

open NUnit.Framework
open Microsoft.FSharp.Linq

[<TestFixture>]
type NullableOperators() =
    [<Test>]    
    member _.CastingUint () =
        let expected = Nullable(12u)
        let actual = Nullable.uint (Nullable(12))
        Assert.AreEqual(expected, actual)