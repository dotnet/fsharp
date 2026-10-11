// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

open System.IO
open Xunit
open FSharp.Test.Compiler

// RFC FS-1352: interpolated strings in constant expressions.
module ConstantInterpolatedStringsTests =

    let private preview source = FSharp source |> withLangVersionPreview

    [<Fact>]
    let ``Literals and attribute arguments take the runtime text of a constant interpolated string`` () =
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

let quotation =
    match <@ $"{Root}/items" @> with
    | Quotations.Patterns.Value _ -> "folded"
    | _ -> "not folded"

printfn "%s|%s|%s|%s" Items ItemById Verbatim (sprintf CountFormat 3)
printfn "%s|%s|%s" (String.concat "," tag.Tags) tag.Name quotation
"""
        |> asExe
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains @"api/v2/items|api/v2/items/{id}|\Rootapi/v2|api/v2/items: 3"
        |> withStdOutContains "api/v2/a,api/v2/b|api/v2!|not folded"

    [<Fact>]
    let ``Extended interpolated strings fold with their own delimiters`` () =
        // [<Literal>] let Json = $$"""{"{{Root}}": "100%"}"""
        preview "module Program\n[<Literal>]\nlet Root = \"api\"\n[<Literal>]\nlet Json = $$\"\"\"{\"{{Root}}\": \"100%\"}\"\"\"\nprintfn \"%s\" Json"
        |> asExe
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains "{\"api\": \"100%\"}"

    [<Fact>]
    let ``The compiler reads an attribute argument that is a constant interpolated string`` () =
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
        let providerPath =
            Path.Combine(__SOURCE_DIRECTORY__, "../../fsharp/typeProviders/staticStringParameter/provider.fsx")

        let provider =
            Fsx $"""#load @"{providerPath}" """
            |> withName "StaticStringParameterProvider"
            |> ignoreWarnings
            |> compile
            |> shouldSucceed

        Fsx(
            $"#r @\"{provider.OutputPath.Value}\"\n"
            + """
[<Literal>]
let Root = "api/v2"

let fromConst = Provided.Text<const ($"{Root}/items")>.Value

[<Literal>]
let ItemById = $"{Root}/items/{{id}}"

let fromLiteral = Provided.Text<ItemById>.Value

if fromConst <> "api/v2/items" || fromLiteral <> "api/v2/items/{id}" then
    failwith $"Unexpected static arguments: '{fromConst}', '{fromLiteral}'"
"""
        )
#if NETCOREAPP
        |> withOptions [ "--usesdkrefs-" ]
#endif
        |> withLangVersionPreview
        |> eval
        |> shouldSucceed

    [<Fact>]
    let ``Each hole must be a non-null constant string without alignment or format specifiers`` () =
        preview """
module Program

[<Literal>]
let Root = "api/v2"
[<Literal>]
let Version = 2
[<Literal>]
let NoValue: string = null
let dir = "data"

[<Literal>]
let A = $"v{Version}"
[<Literal>]
let B = $"{Root,10}"
[<Literal>]
let C = $"%s{Root}"
[<Literal>]
let D = $"{dir}/x"
[<Literal>]
let E = $"{NoValue}/x"
"""
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3925, Line 13, Col 13, Line 13, Col 20, "This interpolated string must be a constant, so each hole must be a non-null constant expression of type 'string' without alignment or format specifiers.")
            (Error 3925, Line 15, Col 12, Line 15, Col 16, "This interpolated string must be a constant, so each hole must be a non-null constant expression of type 'string' without alignment or format specifiers.")
            (Error 3925, Line 17, Col 14, Line 17, Col 18, "This interpolated string must be a constant, so each hole must be a non-null constant expression of type 'string' without alignment or format specifiers.")
            (Error 267, Line 19, Col 12, Line 19, Col 15, "This is not a valid constant expression or custom attribute value")
            (Error 3925, Line 21, Col 12, Line 21, Col 19, "This interpolated string must be a constant, so each hole must be a non-null constant expression of type 'string' without alignment or format specifiers.")
        ]

    [<Fact>]
    let ``Only the first hole that breaks a rule is reported`` () =
        preview """
module Program

[<Literal>]
let Version = 2
let dir = "data"

[<Literal>]
let A = $"{Version}{Version}"
[<Literal>]
let B = $"{dir}{Version}"
[<Literal>]
let C = $"{Version}{missing}"
"""
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3925, Line 9, Col 12, Line 9, Col 19, "This interpolated string must be a constant, so each hole must be a non-null constant expression of type 'string' without alignment or format specifiers.")
            (Error 267, Line 11, Col 12, Line 11, Col 15, "This is not a valid constant expression or custom attribute value")
            (Error 39, Line 13, Col 21, Line 13, Col 28, "The value or constructor 'missing' is not defined.")
            (Error 3925, Line 13, Col 12, Line 13, Col 19, "This interpolated string must be a constant, so each hole must be a non-null constant expression of type 'string' without alignment or format specifiers.")
        ]

    [<Fact>]
    let ``Constant interpolated strings need the preview language version`` () =
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
        |> withSingleDiagnostic (Error 3350, Line 8, Col 13, Line 8, Col 28, "Feature 'interpolated strings in constant expressions' is not available in F# 11.2. Please use language version 'PREVIEW' or greater.")
