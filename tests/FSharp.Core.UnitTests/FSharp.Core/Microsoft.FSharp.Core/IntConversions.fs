// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Core.UnitTests
open System
open Xunit
open FSharp.Core.UnitTests.LibraryTestFx

type IntConversions() =

    [<Fact>]
    member this.``Unchecked.SignedToUInt64`` () =
        let d = System.Int32.MinValue
        let e = uint64 d
        let f = uint64 (uint32 d)
        Assert.True (e <> f)
        ()

    [<Fact>]
    member this.``Unchecked.SignedToUInt32`` () =
        let d = System.Int16.MinValue
        let e = uint32 d
        let f = uint32 (uint16 d)
        Assert.True (e <> f)
        ()

    [<Fact>]
    member this.``Checked.UnsignedToSignedInt32``() =
        let d = System.UInt16.MaxValue
        CheckThrowsExn<OverflowException>(fun() -> Checked.int16 d |> ignore)
