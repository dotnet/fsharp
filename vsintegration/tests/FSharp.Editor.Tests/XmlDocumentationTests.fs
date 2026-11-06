// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module FSharp.Editor.Tests.XmlDocumentationTests

open Xunit
open FSharp.Compiler.Text
open Microsoft.VisualStudio.FSharp.Editor

let private collectSummary xml =
    let parts = ResizeArray<TaggedText>()
    let collector = TextSanitizingCollector(parts.Add) :> ITaggedTextCollector
    let reader = XmlDocumentation.XmlDocReader.TryCreate xml |> Option.get

    // AppendMemberData starts the doc the same way before it reads the summary
    collector.StartXMLDoc()
    reader.CollectSummary(collector, false)
    parts

[<Fact>]
let ``see langword shows as a keyword`` () =
    let parts =
        collectSummary """<summary>Returns <see langword="true"/> when it works.</summary>"""

    let expected =
        [|
            TextTag.Text, "Returns "
            TextTag.Keyword, "true"
            TextTag.Text, " when it works."
        |]

    Assert.Equal<(TextTag * string)[]>(expected, parts |> Seq.map (fun part -> part.Tag, part.Text) |> Seq.toArray)

[<Theory>]
[<InlineData("""<summary><see langword="true"/> when it works.</summary>""", "true when it works.")>]
[<InlineData("""<summary><see cref="T:Foo.Bar"/> is good.</summary>""", "Foo.Bar is good.")>]
[<InlineData("""<summary><br/> text</summary>""", "\ntext")>]
let ``the first token of a summary decides if the text after it keeps its space`` (xml: string, expected: string) =
    let text = collectSummary xml |> Seq.map _.Text |> String.concat ""

    Assert.Equal(expected, text)
