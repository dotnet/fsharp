// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Core.UnitTests.Linq

open System
open Xunit
open Microsoft.FSharp.Linq

type NullableOperators() =
    [<Fact>]    
    member _.CastingUint () =
        let expected = Nullable(12u)
        let actual = Nullable.uint (Nullable(12))
        Assert.Equal(expected, actual)