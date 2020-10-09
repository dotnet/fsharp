// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Core.UnitTests.FSharp_Core.Linq.NullableOperators

open Xunit
open Microsoft.FSharp.Linq

[<TestFixture>]
type NullableOperators() =
    [<Fact>]    
    member _.CastingUint () =
        let expected = Nullable(12u)
        let actual = Nullable.uint (Nullable(12))
        Assert.AreEqual(expected, actual)