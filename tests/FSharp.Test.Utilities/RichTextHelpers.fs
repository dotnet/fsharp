// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Test

open FSharp.Compiler.Text

[<AutoOpen>]
module RichTextHelpers =

    let private escape (text: string) =
        text
            .Replace("\\", "\\\\")
            .Replace("\"", "\\\"")
            .Replace("\r", "\\r")
            .Replace("\n", "\\n")
            .Replace("\t", "\\t")

    /// Renders a single tagged part as `Tag "text"`
    let dumpTaggedText (part: TaggedText) =
        sprintf "%A \"%s\"" part.Tag (escape part.Text)

    /// Renders rich text as one `Tag "text"` line per part, so that tag sequences are readable and
    /// can be compared directly in test expectations.
    let dumpRichText (text: RichText) =
        text.Parts |> Array.map dumpTaggedText |> String.concat "\n"

    /// Asserts that rich text consists of exactly the given parts.
    /// Both sides are compared as dumps, so that a mismatch is reported part by part.
    let assertRichTextParts (expected: (TextTag * string) list) (text: RichText) =
        let expected =
            expected
            |> List.map (fun (tag, text) -> dumpTaggedText (TaggedText(tag, text)))
            |> String.concat "\n"

        FSharp.Test.Assert.shouldEqual expected (dumpRichText text)
