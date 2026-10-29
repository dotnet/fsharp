// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open System.IO
open Xunit
open FSharp.Test.Compiler

module ConstantInterpolatedStringsTests =

    let private preview source = FSharp source |> withLangVersionPreview

    [<Fact>]
    let ``Constant interpolated strings take their run-time text`` () =
        preview """
module Program

[<Literal>]
let Root = "api/v2"

[<Literal>]
let Items = $"{Root}/items"

[<Literal>]
let ItemById = $"{Items}/{{id}}"

[<Literal>]
let CountFormat = $"{Items}: %%d"

[<Literal>]
let Verbatim = $@"\{nameof Root}" + $"{Root}"

type TagAttribute(tags: string[]) =
    inherit System.Attribute()
    member _.Tags = tags
    member val Name = "" with get, set

[<Tag([| $"{Root}/a"; $"{Root}/b" |], Name = $"{Root}!")>]
type Tagged() = class end

let tag = typeof<Tagged>.GetCustomAttributes(typeof<TagAttribute>, false) |> Array.exactlyOne :?> TagAttribute
let quotationIsFolded = match <@ $"{Root}/items" @> with Quotations.Patterns.Value _ -> true | _ -> false

printfn "%s|%s|%s|%s|%s|%s|%b" Items ItemById Verbatim (sprintf CountFormat 3) (String.concat "," tag.Tags) tag.Name quotationIsFolded
"""
        |> asExe
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains @"api/v2/items|api/v2/items/{id}|\Rootapi/v2|api/v2/items: 3|api/v2/a,api/v2/b|api/v2!|false"

    [<Fact>]
    let ``An Obsolete message can be a constant interpolated string`` () =
        preview """
module Program

let getItemV2 (id: int) = id

[<System.Obsolete($"Use {nameof getItemV2} instead")>]
let getItem id = getItemV2 id

let item = getItem 1
"""
        |> typecheck
        |> withSingleDiagnostic (Warning 44, Line 9, Col 12, Line 9, Col 19, "This construct is deprecated. Use getItemV2 instead")

    [<Fact>]
    let ``A signature literal can be a constant interpolated string`` () =
        Fsi "module M\n[<Literal>]\nval Root: string = \"api\"\n[<Literal>]\nval Items: string = $\"{Root}/items\""
        |> withAdditionalSourceFile (FsSource "module M\n[<Literal>]\nlet Root = \"api\"\n[<Literal>]\nlet Items = $\"{Root}/items\"")
        |> withLangVersionPreview
        |> asLibrary
        |> compile
        |> shouldSucceed

    [<Fact>]
    let ``A type provider static argument can be a constant interpolated string`` () =
        let providerPath = Path.Combine(__SOURCE_DIRECTORY__, "../../fsharp/typeProviders/staticStringValue/provider.fsx")

        let provider =
            Fsx $"""#load @"{providerPath}" """
            |> withName "StaticStringValueProvider"
            |> ignoreWarnings
            |> compile
            |> shouldSucceed

        Fsx $$"""#r @"{{provider.OutputPath.Value}}"

[<Literal>]
let Root = "api/v2"

[<Literal>]
let Items = $"{Root}/items"

type FromConst = Provided.Text<const ($"{Root}/items")>
type FromLiteral = Provided.Text<Items>"""
#if NETCOREAPP
        |> withOptions [ "--usesdkrefs-" ]
#endif
        |> withLangVersionPreview
        |> eval
        |> shouldSucceed

    [<Theory>]
    [<InlineData("{Version}", 267)>]
    [<InlineData("{Root,10}", 267)>]
    [<InlineData("%s{Root}", 267)>]
    [<InlineData("{NoValue}", 267)>]
    [<InlineData("{dir}", 267)>]
    [<InlineData("{1 / 0}", 267)>]
    [<InlineData("{dir + dir}{missing}", 39)>]
    let ``A hole must be a non-null constant string without alignment or format specifier`` (hole: string, errorCode: int) =
        preview $"""
module Program

[<Literal>]
let Root = "api/v2"
[<Literal>]
let Version = 2
[<Literal>]
let NoValue: string = null
let dir = "data"

[<Literal>]
let Rejected = $"{hole}"
"""
        |> typecheck
        |> shouldFail
        |> withErrorCode errorCode

    [<Fact>]
    let ``Only the first hole that breaks a rule is reported`` () =
        preview """
module Program

[<Literal>]
let Version = 2

[<Literal>]
let Twice = $"{Version}{Version}"
"""
        |> typecheck
        |> withDiagnostics
            [ Error 267, Line 8, Col 16, Line 8, Col 23, "This is not a valid constant expression or custom attribute value"
              Error 837, Line 8, Col 13, Line 8, Col 34, "This is not a valid constant expression" ]

    [<Theory>]
    [<InlineData("preview")>]
    [<InlineData("11.2")>]
    let ``An interpolated string that is not evaluated is not checked as a constant`` (langVersion: string) =
        FSharp """
module Program

[<Literal>]
let Root = "api/v2"

[<Literal>]
let Unevaluated = false && ($"{1}" = "1")

[<Literal>]
let Picked = if true then Root else $"{1}"
"""
        |> withLangVersion langVersion
        |> typecheck
        |> shouldSucceed

    [<Fact>]
    let ``Before the preview language version a constant interpolated string is still rejected`` () =
        FSharp """
module Program

[<Literal>]
let Root = "api/v2"

[<Literal>]
let Items = $"{Root}/items"
"""
        |> withLangVersion "11.2"
        |> typecheck
        |> shouldFail
        |> withDiagnostics
            [ Error 267, Line 8, Col 13, Line 8, Col 28, "This is not a valid constant expression or custom attribute value"
              Error 837, Line 8, Col 13, Line 8, Col 28, "This is not a valid constant expression" ]
