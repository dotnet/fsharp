// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace EmittedIL.RealInternalSignature

open Xunit
open FSharp.Test
open FSharp.Test.Compiler

module ClassTypeVisibility =

    let withRealInternalSignature realSig compilation =
        compilation
        |> withOptions [if realSig then "--realsig+" else "--realsig-" ]

    [<InlineData(true, "public")>]
    [<InlineData(true, "private")>]
    [<InlineData(false, "public")>]
    [<InlineData(false, "private")>]
    [<Theory>]
    let ``type visibility - various constructors`` (realSig, typeVisibility: string) =
        FSharp $"""
module RealInternalSignature

type {typeVisibility} TypeOne public () = class end
type {typeVisibility} TypeTwo internal () = class end
type {typeVisibility} TypeThree private () = class end
type {typeVisibility} TypeFour () = class end
"""
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> shouldSucceed

    [<InlineData(true, "public")>]
    [<InlineData(true, "private")>]
    [<InlineData(false, "public")>]
    [<InlineData(false, "private")>]
    [<Theory>]
    let ``type visibility - various methods`` (realSig, typeVisibility: string) =
        FSharp $"""
module RealInternalSignature

type {typeVisibility} TestType () =
    member public _.PublicMethod() = ()
    member internal _.InternalMethod() = ()
    member private _.PrivateMethod() = ()
    member _.DefaultMethod() = ()
"""
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> shouldSucceed

    [<InlineData(true, "public")>]
    [<InlineData(true, "private")>]
    [<InlineData(false, "public")>]
    [<InlineData(false, "private")>]
    [<Theory>]
    let ``type visibility - various properties`` (realSig, typeVisibility: string) =
        FSharp $"""
module RealInternalSignature

type {typeVisibility} TestType () =
    member val public PublicProperty = 0 with get, set
    member val internal InternalProperty = 0 with get, set
    member val private PrivateProperty = 0 with get, set
    member val DefaultProperty = 0 with get, set
"""
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> shouldSucceed

    [<InlineData(true, "public")>]
    [<InlineData(true, "private")>]
    [<InlineData(false, "public")>]
    [<InlineData(false, "private")>]
    [<Theory>]
    let ``type visibility - various mixed properties`` (realSig, typeVisibility: string) =
        FSharp $"""
module RealInternalSignature

type {typeVisibility} TestType () =
    member _.MixedPropertyOne with public get() = 0 and internal set (_:int) = ()
    member _.MixedPropertyTwo with public get() = 0 and private set (_:int) = ()
    member _.MixedPropertyThree with private get() = 0 and public set (_:int) = ()
    member _.MixedPropertyFour with private get() = 0 and internal set (_:int) = ()
    member _.MixedPropertyFive with internal get() = 0 and public set (_:int) = ()
    member _.MixedPropertySix with internal get() = 0 and private set (_:int) = ()
    member _.MixedPropertySeven with get() = 0 and public set (_:int) = ()
    member _.MixedPropertyEight with get() = 0 and internal set (_:int) = ()
    member _.MixedPropertyNine with get() = 0 and private set (_:int) = ()
    member _.MixedPropertyTen with public get() = 0 and set (_:int) = ()
    member _.MixedPropertyEleven with internal get() = 0 and set (_:int) = ()
    member _.MixedPropertyTwelve with private get() = 0 and set (_:int) = ()
"""
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> shouldSucceed

    [<InlineData(true, "public")>]
    [<InlineData(true, "private")>]
    [<InlineData(false, "public")>]
    [<InlineData(false, "private")>]
    [<Theory>]
    let ``type visibility - various static methods`` (realSig, typeVisibility: string) =
        FSharp $"""
module RealInternalSignature

type {typeVisibility} TestType () =
    static member public PublicMethod() = ()
    static member internal InternalMethod() = ()
    static member private PrivateMethod() = ()
    static member DefaultMethod() = ()
"""
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> shouldSucceed

    [<InlineData(true, "public")>]
    [<InlineData(true, "private")>]
    [<InlineData(false, "public")>]
    [<InlineData(false, "private")>]
    [<Theory>]
    let ``type visibility - various static properties`` (realSig, typeVisibility: string) =
        FSharp $"""
module RealInternalSignature

type {typeVisibility} TestType () =
    static member val public PublicProperty = 0 with get, set
    static member val internal InternalProperty = 0 with get, set
    static member val private PrivateProperty = 0 with get, set
    static member val DefaultProperty = 0 with get, set"""
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> shouldSucceed

    [<InlineData(true, "public")>]
    [<InlineData(true, "private")>]
    [<InlineData(false, "public")>]
    [<InlineData(false, "private")>]
    [<Theory>]
    let ``type visibility - various static mixed properties`` (realSig, typeVisibility: string) =
        FSharp $"""
module RealInternalSignature

type {typeVisibility} TestType () =
    static member MixedPropertyOne with public get() = 0 and internal set (_:int) = ()
    static member MixedPropertyTwo with public get() = 0 and private set (_:int) = ()
    static member MixedPropertyThree with private get() = 0 and public set (_:int) = ()
    static member MixedPropertyFour with private get() = 0 and internal set (_:int) = ()
    static member MixedPropertyFive with internal get() = 0 and public set (_:int) = ()
    static member MixedPropertySix with internal get() = 0 and private set (_:int) = ()
    static member MixedPropertySeven with get() = 0 and public set (_:int) = ()
    static member MixedPropertyEight with get() = 0 and internal set (_:int) = ()
    static member MixedPropertyNine with get() = 0 and private set (_:int) = ()
    static member MixedPropertyTen with public get() = 0 and set (_:int) = ()
    static member MixedPropertyEleven with internal get() = 0 and set (_:int) = ()
    static member MixedPropertyTwelve with private get() = 0 and set (_:int) = ()
"""
        |> asLibrary
        |> withRealInternalSignature realSig
        |> compile
        |> shouldSucceed
