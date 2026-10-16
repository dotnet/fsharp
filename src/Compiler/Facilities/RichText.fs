// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace FSharp.Compiler.Text

open System
open System.Text
open FSharp.Compiler.DiagnosticMessage
open FSharp.Compiler.Text

[<Sealed>]
type RichText(parts: TaggedText[]) =

    let text =
        match parts with
        | [||] -> ""
        | [| part |] -> part.Text
        | parts ->
            let capacity = parts |> Array.sumBy _.Text.Length
            let buf = StringBuilder(capacity)

            for part in parts do
                buf.Append(part.Text) |> ignore

            buf.ToString()

    member _.Parts = parts

    member _.Text = text

    member _.IsEmpty = Array.isEmpty parts

    override _.ToString() = text

    override _.Equals(other) =
        match other with
        | :? RichText as other -> text = other.Text
        | _ -> false

    override _.GetHashCode() = text.GetHashCode()

module RichText =

    let empty = RichText([||])

    let ofParts (parts: TaggedText[]) =
        if Array.isEmpty parts then empty else RichText(parts)

    let ofTaggedText (part: TaggedText) = RichText([| part |])

    let ofTag tag (text: string) =
        if String.IsNullOrEmpty text then
            empty
        else
            ofTaggedText (TaggedText.mkTag tag text)

    let mkText text = ofTag TextTag.Text text
    let mkActivePatternCase text = ofTag TextTag.ActivePatternCase text
    let mkActivePatternResult text = ofTag TextTag.ActivePatternResult text
    let mkAlias text = ofTag TextTag.Alias text
    let mkClass text = ofTag TextTag.Class text
    let mkDelegate text = ofTag TextTag.Delegate text
    let mkEnum text = ofTag TextTag.Enum text
    let mkEvent text = ofTag TextTag.Event text
    let mkField text = ofTag TextTag.Field text
    let mkFunction text = ofTag TextTag.Function text
    let mkInterface text = ofTag TextTag.Interface text
    let mkKeyword text = ofTag TextTag.Keyword text
    let mkLineBreak text = ofTag TextTag.LineBreak text
    let mkLocal text = ofTag TextTag.Local text
    let mkMember text = ofTag TextTag.Member text
    let mkMethod text = ofTag TextTag.Method text
    let mkModule text = ofTag TextTag.Module text
    let mkModuleBinding text = ofTag TextTag.ModuleBinding text
    let mkNamespace text = ofTag TextTag.Namespace text
    let mkNumericLiteral text = ofTag TextTag.NumericLiteral text
    let mkOperator text = ofTag TextTag.Operator text
    let mkParameter text = ofTag TextTag.Parameter text
    let mkProperty text = ofTag TextTag.Property text
    let mkPunctuation text = ofTag TextTag.Punctuation text
    let mkRecord text = ofTag TextTag.Record text
    let mkRecordField text = ofTag TextTag.RecordField text
    let mkSpace text = ofTag TextTag.Space text
    let mkStringLiteral text = ofTag TextTag.StringLiteral text
    let mkStruct text = ofTag TextTag.Struct text
    let mkTypeParameter text = ofTag TextTag.TypeParameter text
    let mkUnion text = ofTag TextTag.Union text
    let mkUnionCase text = ofTag TextTag.UnionCase text
    let mkUnknownEntity text = ofTag TextTag.UnknownEntity text
    let mkUnknownType text = ofTag TextTag.UnknownType text
    let mkUnresolvedName text = ofTag TextTag.UnresolvedName text

    let append (left: RichText) (right: RichText) =
        if left.IsEmpty then right
        elif right.IsEmpty then left
        else RichText(Array.append left.Parts right.Parts)

    let concat (texts: RichText seq) =
        let parts = ResizeArray()

        for text in texts do
            parts.AddRange(text.Parts)

        ofParts (parts.ToArray())

    let concatWith (separator: RichText) (texts: RichText seq) =
        let parts = ResizeArray()
        let mutable needsSeparator = false

        for text in texts do
            if needsSeparator then
                parts.AddRange separator.Parts

            needsSeparator <- true
            parts.AddRange text.Parts

        ofParts (parts.ToArray())

    let collectParts mapping (text: RichText) =
        ofParts (Array.collect mapping text.Parts)

    let ofQualifiedName leafOfName (name: string) =
        match name.LastIndexOf '.' with
        | -1 -> leafOfName name
        | i ->
            let path = name.Substring(0, i)
            let leaf = name.Substring(i + 1)

            let namespaceParts =
                path.Split '.'
                |> Array.map (ofTag TextTag.Namespace)
                |> concatWith (ofTag TextTag.Punctuation ".")

            concat [ namespaceParts; ofTag TextTag.Punctuation "."; leafOfName leaf ]

    let ofQualifiedTypeName name = ofQualifiedName mkUnknownType name

module RichMessage =

    /// Characters that can stand in for a classified argument while the message is formatted. Control
    /// characters, so that in practice the first one is always free.
    let private candidateMarkers =
        [|
            for c in '\u0001' .. '\u001f' do
                if c <> '\n' && c <> '\r' && c <> '\t' then
                    c
        |]

    /// Replaces the markers in a formatted message with the parts they stand for
    let private splice (marker: char) (args: ResizeArray<RichText>) (text: string) =
        let parts = ResizeArray()
        let buf = StringBuilder()
        let mutable i = 0

        let addPendingText () =
            if buf.Length > 0 then
                parts.Add(TaggedText.tagText (buf.ToString()))
                buf.Clear() |> ignore

        while i < text.Length do
            // A marker is the character followed by the argument index and the character again
            let mutable index = 0
            let mutable j = i + 1

            if text[i] = marker then
                while j < text.Length && text[j] >= '0' && text[j] <= '9' do
                    index <- index * 10 + int text[j] - int '0'
                    j <- j + 1

            if
                text[i] = marker
                && j > i + 1
                && j < text.Length
                && text[j] = marker
                && index < args.Count
            then
                addPendingText ()
                parts.AddRange(args[index].Parts)
                i <- j + 1
            else
                buf.Append(text[i]) |> ignore
                i <- i + 1

        addPendingText ()
        RichText.ofParts (parts.ToArray())

    /// A resource accessor returns an already-formatted message, so the holes can no longer be told
    /// apart afterwards. The message is therefore formatted twice: once with the argument texts, which
    /// is what it has to read as, and once with a marker per classified argument, which the parts are
    /// then spliced back into. Splicing the formatted message rather than the template is what makes
    /// this survive a translation reordering, repeating or dropping holes.
    ///
    /// The marker is picked absent from the first result, so no argument and no translation can contain
    /// one. Should the two disagree anyway, the text is what the reader sees, so it wins and the
    /// classification is dropped.
    let private formatWithMarkers (format: (RichText -> string) -> 'T) (getText: 'T -> string) =
        let plain = format (fun arg -> arg.Text)
        let plainText = getText plain

        let marker = candidateMarkers |> Array.tryFind (fun c -> plainText.IndexOf c < 0)

        match marker with
        | None -> plain, RichText.mkText plainText
        | Some marker ->
            let args = ResizeArray()

            let addArg (arg: RichText) =
                let index = args.Count
                args.Add arg
                String.Concat(string marker, string index, string marker)

            let spliced = splice marker args (getText (format addArg))

            if spliced.Text = plainText then
                plain, spliced
            else
                plain, RichText.mkText plainText

    let text (format: (RichText -> string) -> string) = formatWithMarkers format id |> snd

    let numbered (format: (RichText -> string) -> int * RichText) =
        let (number, _), text =
            formatWithMarkers format (fun (_, message: RichText) -> message.Text)

        number, text

[<Sealed>]
type RichTextBuilder() =
    let parts = ResizeArray<TaggedText>()

    // NavigableTaggedText and other subclasses carry data that merging would lose
    let isPlain (part: TaggedText) = part.GetType() = typeof<TaggedText>

    /// A message is built from many pieces, and where one piece ends tells a consumer nothing unless
    /// the classification changes there
    let mergeAdjacentParts () =
        let merged = ResizeArray<TaggedText>(parts.Count)

        for part in parts do
            if
                merged.Count > 0
                && merged[merged.Count - 1].Tag = part.Tag
                && isPlain merged[merged.Count - 1]
                && isPlain part
            then
                merged[merged.Count - 1] <- TaggedText(part.Tag, merged[merged.Count - 1].Text + part.Text)
            else
                merged.Add part

        merged.ToArray()

    member _.Append(value: string) =
        if not (String.IsNullOrEmpty value) then
            parts.Add(TaggedText.tagText value)

    member _.Append(value: TaggedText) = parts.Add value

    member _.Append(value: RichText) = parts.AddRange value.Parts

    member this.Append(message: ResourceString<string -> string>, a0: RichText) =
        this.Append(fun rich -> message.Format(rich a0))

    member this.Append(message: ResourceString<string -> string -> string>, a0: RichText, a1: RichText) =
        this.Append(fun rich -> message.Format (rich a0) (rich a1))

    member this.Append(message: ResourceString<string -> string -> string -> string>, a0: RichText, a1: RichText, a2: RichText) =
        this.Append(fun rich -> message.Format (rich a0) (rich a1) (rich a2))

    member this.Append
        (message: ResourceString<string -> string -> string -> string -> string>, a0: RichText, a1: RichText, a2: RichText, a3: RichText)
        =
        this.Append(fun rich -> message.Format (rich a0) (rich a1) (rich a2) (rich a3))

    member this.Append(format: (RichText -> string) -> string) = this.Append(RichMessage.text format)

    member _.IsEmpty = parts.Count = 0

    member _.ToRichText() =
        RichText.ofParts (mergeAdjacentParts ())

    override this.ToString() = this.ToRichText().Text
