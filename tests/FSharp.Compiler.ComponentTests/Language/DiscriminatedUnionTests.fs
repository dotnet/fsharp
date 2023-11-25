// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open FSharp.Test.Compiler

module DiscriminatedUnionTests =
    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Simple Is* discriminated union properties are visible, proper values are returned`` () =
        Fsx """
type Foo = | Foo of string | Bar
let foo = Foo.Foo "hi"
if not foo.IsFoo then failwith "Should be Foo"
if foo.IsBar then failwith "Should not be Bar"
        """
        |> withLangVersionPreview
        |> compileExeAndRun
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Is* discriminated union properties with backticks are visible, proper values are returned`` () =
        Fsx """
type Foo = | Foo of string | ``Mars Bar``
let foo = Foo.Foo "hi"
if not foo.IsFoo then failwith "Should be Foo"
if foo.``IsMars Bar`` then failwith "Should not be ``Mars Bar``"

let marsbar = ``Mars Bar``
if marsbar.IsFoo then failwith "Should not be Foo"
if not marsbar.``IsMars Bar`` then failwith "Should be ``Mars Bar``"
        """
        |> withLangVersionPreview
        |> compileExeAndRun
        |> shouldSucceed

    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Is* discriminated union properties are visible, proper values are returned in recursive namespace, before the definition`` () =
        FSharp """
namespace rec Hello

module Main =
    [<EntryPoint>]
    let main _ =
        let foo = Foo.Foo "hi"
        if not foo.IsFoo then failwith "Should be Foo"
        if foo.IsBar then failwith "Should not be Bar"
        0

[<Struct>]
type Foo =
    | Foo of string
    | Bar
        """
        |> withLangVersionPreview
        |> compileExeAndRun
        |> shouldSucceed


    [<FSharp.Test.FactForNETCOREAPP>]
    let ``Is* discriminated union properties are visible, proper values are returned in recursive namespace, in SRTP`` () =
        FSharp """
namespace Hello

[<Struct>]
type Foo =
    | Foo of string
    | Bar

module Main =

    let inline (|HasIsFoo|) x = fun () -> (^a : (member IsFoo: bool) x)
    let inline (|HasIsBar|) x = fun () -> (^a : (member IsBar: bool) x)
    let getIsFooIsBar (HasIsFoo isFoo & HasIsBar isBar) = (isFoo(), isBar())

    [<EntryPoint>]
    let main _ =
        let foo = Foo.Foo "hi"
        let (isFoo, isBar) = getIsFooIsBar foo
        if not isFoo then failwith "Should be Foo"
        if isBar then failwith "Should not be Bar"
        0
        """
        |> withLangVersionPreview
        |> compileExeAndRun
        |> shouldSucceed