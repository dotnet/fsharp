// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Language

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

[<Literal>]
let Version = 2

[<Literal>]
let Scalars = $"v{Version}{'-'}{true}|{-1L}|{18446744073709551615UL}|{-12.3400m}|%s{Root}"

let root, version = Root, Version
let scalarsAtRunTime = $"v{version}{'-'}{true}|{-1L}|{18446744073709551615UL}|{-12.3400m}|%s{root}"

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
printfn "%s|%b" Scalars (Scalars = scalarsAtRunTime)
printfn "%s|%s|%s" (String.concat "," tag.Tags) tag.Name quotation
"""
        |> asExe
        |> compileExeAndRun
        |> shouldSucceed
        |> withStdOutContains @"api/v2/items|api/v2/items/{id}|\Rootapi/v2|api/v2/items: 3"
        |> withStdOutContains "v2-True|-1|18446744073709551615|-12.3400|api/v2|true"
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
    let ``Each hole must be a non-null constant string, integer, decimal, character or Boolean without alignment or format specifiers`` () =
        let holeError =
            "This interpolated string is a constant, so each hole must be a non-null constant string, integer, decimal, character or Boolean, with no alignment and no format specifier other than '%s' for a string."

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
let A = $"{1.5}"
[<Literal>]
let B = $"{Root,10}"
[<Literal>]
let C = $"%d{Version}"
[<Literal>]
let D = $"{dir}/x"
[<Literal>]
let E = $"{NoValue}/x"
[<Literal>]
let F = $"{System.DayOfWeek.Monday}"
"""
        |> typecheck
        |> shouldFail
        |> withDiagnostics [
            (Error 3925, Line 13, Col 12, Line 13, Col 15, holeError)
            (Error 3925, Line 15, Col 12, Line 15, Col 16, holeError)
            (Error 3925, Line 17, Col 14, Line 17, Col 21, holeError)
            (Error 267, Line 19, Col 12, Line 19, Col 15, "This is not a valid constant expression or custom attribute value")
            (Error 3925, Line 21, Col 12, Line 21, Col 19, holeError)
            (Error 3925, Line 23, Col 12, Line 23, Col 35, holeError)
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
