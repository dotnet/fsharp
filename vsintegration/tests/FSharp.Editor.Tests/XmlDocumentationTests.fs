// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.XmlDocumentationTests

open Xunit
open FSharp.Compiler.Text
open Microsoft.VisualStudio.FSharp.Editor

[<Fact>]
let ``see langword shows as a keyword`` () =
    let parts = ResizeArray<TaggedText>()

    let reader =
        XmlDocumentation.XmlDocReader.TryCreate("""<summary>Returns <see langword="true"/> when it works.</summary>""")
        |> Option.get

    reader.CollectSummary(TextSanitizingCollector(parts.Add), false)

    let expected =
        [|
            TextTag.Text, "Returns "
            TextTag.Keyword, "true"
            TextTag.Text, " when it works."
        |]

    Assert.Equal<(TextTag * string)[]>(expected, parts |> Seq.map (fun part -> part.Tag, part.Text) |> Seq.toArray)
