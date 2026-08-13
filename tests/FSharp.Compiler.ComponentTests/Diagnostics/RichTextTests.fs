// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Diagnostics

open Xunit
open FSharp.Test
open FSharp.Test.Assert
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Layout
open FSharp.Compiler.DiagnosticsLogger

module RichTextTests =

    let private tagged tag text = TaggedText(tag, text)

    let private proxy = stringThatIsAProxyForANewlineInFlatErrors

    [<Fact>]
    let ``Empty text has no parts and empty string`` () =
        RichText.empty.Parts |> shouldBeEmpty
        RichText.empty.Text |> shouldEqual ""
        RichText.empty.IsEmpty |> shouldBeTrue

    [<Fact>]
    let ``Plain string becomes a single Text part`` () =
        let text = RichText.mkText "The type 'int' is not defined."

        text |> assertRichTextParts [ TextTag.Text, "The type 'int' is not defined." ]
        text.Text |> shouldEqual "The type 'int' is not defined."

    [<Fact>]
    let ``Empty string produces no parts, whatever the classification`` () =
        (RichText.mkText "").IsEmpty |> shouldBeTrue
        (RichText.mkMethod "").IsEmpty |> shouldBeTrue
        (RichText.ofTag TextTag.Class "").IsEmpty |> shouldBeTrue

    [<Fact>]
    let ``Parts are dumped as tag and text pairs`` () =
        RichText.ofParts
            [| tagged TextTag.Text "The type "
               tagged TextTag.Punctuation "'"
               tagged TextTag.Class "Foo"
               tagged TextTag.Punctuation "'" |]
        |> assertRichTextParts
            [ TextTag.Text, "The type "
              TextTag.Punctuation, "'"
              TextTag.Class, "Foo"
              TextTag.Punctuation, "'" ]

    [<Fact>]
    let ``Text is the concatenation of all parts`` () =
        let text =
            RichText.ofParts
                [| tagged TextTag.Text "The type "
                   tagged TextTag.Class "Foo"
                   tagged TextTag.Text " is not defined." |]

        text.Text |> shouldEqual "The type Foo is not defined."

    [<Fact>]
    let ``Control characters are escaped in the dump`` () =
        RichText.mkText "line\r\n\tcolumn \"quoted\" back\\slash"
        |> dumpRichText
        |> shouldEqual "Text \"line\\r\\n\\tcolumn \\\"quoted\\\" back\\\\slash\""

    /// Equality asks what reaches the reader, so neither the classification nor where the part
    /// boundaries fall takes part in it
    [<Fact>]
    let ``Texts that read the same are equal`` () =
        let classified =
            RichText.ofParts [| tagged TextTag.Class "Fo"; tagged TextTag.Struct "o" |]

        classified = RichText.mkText "Foo" |> shouldBeTrue
        classified.GetHashCode() |> shouldEqual ((RichText.mkText "Foo").GetHashCode())

        RichText.empty = RichText.mkText "" |> shouldBeTrue

    [<Fact>]
    let ``Texts that read differently are not equal`` () =
        RichText.ofTaggedText (tagged TextTag.Class "Foo") = RichText.ofTaggedText (tagged TextTag.Class "Bar")
        |> shouldBeFalse

        RichText.mkText("Foo").Equals(box 1) |> shouldBeFalse

    [<Fact>]
    let ``Append keeps parts of both sides`` () =
        RichText.append (RichText.mkText "expected ") (RichText.ofTaggedText (tagged TextTag.Class "int"))
        |> assertRichTextParts [ TextTag.Text, "expected "; TextTag.Class, "int" ]

    [<Fact>]
    let ``Append with an empty operand returns the other one`` () =
        let text = RichText.mkText "abc"

        RichText.append RichText.empty text |> shouldBe text
        RichText.append text RichText.empty |> shouldBe text

    [<Fact>]
    let ``Concat flattens all parts in order`` () =
        let text =
            RichText.concat
                [ RichText.mkText "a"
                  RichText.empty
                  RichText.ofTaggedText (tagged TextTag.Keyword "let")
                  RichText.mkText "b" ]

        text |> assertRichTextParts [ TextTag.Text, "a"; TextTag.Keyword, "let"; TextTag.Text, "b" ]
        text.Text |> shouldEqual "aletb"

    [<Fact>]
    let ``Concat of nothing is empty`` () =
        (RichText.concat []).IsEmpty |> shouldBeTrue

    [<Fact>]
    let ``ConcatWith puts the separator between the texts only`` () =
        let comma = RichText.mkText ","

        [ RichText.ofTaggedText (tagged TextTag.Class "A")
          RichText.ofTaggedText (tagged TextTag.Struct "B") ]
        |> RichText.concatWith comma
        |> assertRichTextParts [ TextTag.Class, "A"; TextTag.Text, ","; TextTag.Struct, "B" ]

        [ RichText.mkText "only" ] |> RichText.concatWith comma |> assertRichTextParts [ TextTag.Text, "only" ]
        (RichText.concatWith comma []).IsEmpty |> shouldBeTrue

    [<Fact>]
    let ``CollectParts can split a part into several`` () =
        let splitTextOnNewline (part: TaggedText) =
            if part.Tag <> TextTag.Text then
                [| part |]
            else
                part.Text.Split('\n')
                |> Array.mapi (fun i line ->
                    if i = 0 then
                        [| tagged TextTag.Text line |]
                    else
                        [| tagged TextTag.LineBreak "\n"; tagged TextTag.Text line |])
                |> Array.concat

        let text =
            RichText.ofParts
                [| tagged TextTag.Text "first\nsecond"
                   tagged TextTag.Class "Foo\nBar" |]
            |> RichText.collectParts splitTextOnNewline

        text
        |> assertRichTextParts
            [ TextTag.Text, "first"
              TextTag.LineBreak, "\n"
              TextTag.Text, "second"
              TextTag.Class, "Foo\nBar" ]

        text.Text |> shouldEqual "first\nsecondFoo\nBar"

    [<Fact>]
    let ``CollectParts dropping every part gives empty text`` () =
        (RichText.mkText "abc" |> RichText.collectParts (fun _ -> [||])).IsEmpty
        |> shouldBeTrue

    [<Fact>]
    let ``Layout parts are preserved`` () =
        let layout =
            wordL (TaggedText.tagKeyword "val") ^^ wordL (TaggedText.tagClass "int")

        let text = LayoutRender.toRichText layout

        text
        |> assertRichTextParts [ TextTag.Keyword, "val"; TextTag.Space, " "; TextTag.Class, "int" ]

        text.Text |> shouldEqual (LayoutRender.showL layout)

    [<Fact>]
    let ``Builder appends strings, parts, texts and layouts`` () =
        let builder = RichTextBuilder()
        builder.IsEmpty |> shouldBeTrue

        builder.Append "The type "
        builder.Append ""
        builder.Append(tagged TextTag.Class "Foo")
        builder.Append(RichText.mkText " is not compatible with ")
        builder.Append(LayoutRender.toRichText (wordL (TaggedText.tagClass "Bar")))

        builder.IsEmpty |> shouldBeFalse

        let text = builder.ToRichText()

        text
        |> assertRichTextParts
            [ TextTag.Text, "The type "
              TextTag.Class, "Foo"
              TextTag.Text, " is not compatible with "
              TextTag.Class, "Bar" ]

        text.Text |> shouldEqual "The type Foo is not compatible with Bar"
        builder.ToString() |> shouldEqual text.Text

    [<Fact>]
    let ``Empty builder produces empty text`` () =
        let builder = RichTextBuilder()
        builder.Append ""
        builder.ToRichText().IsEmpty |> shouldBeTrue

    /// The marker that stands in for a classified argument while the message is formatted is chosen
    /// absent from the message, so an argument that happens to contain one cannot be mistaken for it
    [<Fact>]
    let ``An argument containing a marker character does not corrupt the message`` () =
        let hostile = "before\u000110\u0001after"

        let text =
            RichMessage.text (fun rich -> sprintf "%s and %s" hostile (rich (RichText.mkClass "Foo")))

        text.Text |> shouldEqual (sprintf "%s and Foo" hostile)
        text.Parts |> Array.exists (fun part -> part.Tag = TextTag.Class && part.Text = "Foo") |> shouldBeTrue

    /// The message has to read the same whether or not the arguments are classified
    [<Fact>]
    let ``Splicing survives a hole that is reordered, repeated and dropped`` () =
        let one = RichText.mkClass "One"
        let two = RichText.mkStruct "Two"

        let text =
            RichMessage.text (fun rich -> sprintf "%s %s %s" (rich two) (rich one) (rich two))

        text.Text |> shouldEqual "Two One Two"

        text
        |> assertRichTextParts
            [ TextTag.Struct, "Two"
              TextTag.Text, " "
              TextTag.Class, "One"
              TextTag.Text, " "
              TextTag.Struct, "Two" ]

    [<Fact>]
    let ``Normalization keeps the classification of every part`` () =
        RichText.ofParts
            [| tagged TextTag.Text "  The type\n"
               tagged TextTag.Class "Foo\tBar"
               tagged TextTag.Text "\r\nis not defined.  " |]
        |> NormalizeErrorRichText
        |> assertRichTextParts
            [ TextTag.Text, $"The type{proxy}"
              TextTag.Class, "Foo Bar"
              TextTag.Text, $"{proxy}is not defined." ]

    [<Theory>]
    // Line break forms, including ones split across parts
    [<InlineData("a\r\nb", "")>]
    [<InlineData("a\r", "\nb")>]
    [<InlineData("a\r", "\r\nb")>]
    [<InlineData("a\n", "\rb")>]
    [<InlineData("a\r\r\n", "b")>]
    [<InlineData("a\r\n", "\nb")>]
    [<InlineData("a", "\n\r\nb")>]
    // Control characters
    [<InlineData("a\tb", "c")>]
    // Trimming spans parts
    [<InlineData("   ", "  ")>]
    [<InlineData("  \n a ", "  \r\n ")>]
    [<InlineData("", "  a  ")>]
    // No normalization needed
    [<InlineData("The type ", "Foo is not defined.")>]
    let ``Normalization of parts agrees with normalization of the whole message`` (first: string) (second: string) =
        let text =
            RichText.ofParts [| tagged TextTag.Text first; tagged TextTag.Class second |]

        (NormalizeErrorRichText text).Text |> shouldEqual (NormalizeErrorString text.Text)
