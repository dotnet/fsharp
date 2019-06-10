namespace FSharp.Compiler.Compilation

open System
open Microsoft.CodeAnalysis.Text
open FSharp.Compiler.Text
open FSharp.Compiler.Range

module private SourceText =

    open System.Runtime.CompilerServices

    /// Ported from Roslyn.Utilities
    [<RequireQualifiedAccess>]
    module Hash =
        /// (From Roslyn) This is how VB Anonymous Types combine hash values for fields.
        let combine (newKey: int) (currentKey: int) = (currentKey * (int 0xA5555529)) + newKey

        let combineValues (values: seq<'T>) =
            (0, values) ||> Seq.fold (fun hash value -> combine (value.GetHashCode()) hash)

    let weakTable = ConditionalWeakTable<SourceText, ISourceText>()

    let create (sourceText: SourceText) =
        let sourceText =
            { 
                new Object() with
                    override __.GetHashCode() =
                        let checksum = sourceText.GetChecksum()
                        let contentsHash = if not checksum.IsDefault then Hash.combineValues checksum else 0
                        let encodingHash = if not (isNull sourceText.Encoding) then sourceText.Encoding.GetHashCode() else 0

                        sourceText.ChecksumAlgorithm.GetHashCode()
                        |> Hash.combine encodingHash
                        |> Hash.combine contentsHash
                        |> Hash.combine sourceText.Length

                interface ISourceText with
            
                    member __.Item with get index = sourceText.[index]

                    member __.GetLineString(lineIndex) =
                        sourceText.Lines.[lineIndex].ToString()

                    member __.GetLineCount() =
                        sourceText.Lines.Count

                    member __.GetLastCharacterPosition() =
                        if sourceText.Lines.Count > 0 then
                            (sourceText.Lines.Count, sourceText.Lines.[sourceText.Lines.Count - 1].Span.Length)
                        else
                            (0, 0)

                    member __.GetSubTextString(start, length) =
                        sourceText.GetSubText(TextSpan(start, length)).ToString()

                    member __.SubTextEquals(target, startIndex) =
                        if startIndex < 0 || startIndex >= sourceText.Length then
                            invalidArg "startIndex" "Out of range."

                        if String.IsNullOrEmpty(target) then
                            invalidArg "target" "Is null or empty."

                        let lastIndex = startIndex + target.Length
                        if lastIndex <= startIndex || lastIndex >= sourceText.Length then
                            invalidArg "target" "Too big."

                        let mutable finished = false
                        let mutable didEqual = true
                        let mutable i = 0
                        while not finished && i < target.Length do
                            if target.[i] <> sourceText.[startIndex + i] then
                                didEqual <- false
                                finished <- true // bail out early                        
                            else
                                i <- i + 1

                        didEqual

                    member __.ContentEquals(sourceText) =
                        match sourceText with
                        | :? SourceText as sourceText -> sourceText.ContentEquals(sourceText)
                        | _ -> false

                    member __.Length = sourceText.Length

                    member __.CopyTo(sourceIndex, destination, destinationIndex, count) =
                        sourceText.CopyTo(sourceIndex, destination, destinationIndex, count)
            }

        sourceText

[<AutoOpen>]
module SourceTextExtensions =

    type SourceText with

        member this.ToFSharpSourceText() =
            SourceText.weakTable.GetValue(this, Runtime.CompilerServices.ConditionalWeakTable<_,_>.CreateValueCallback(SourceText.create))

        member this.TryRangeToSpan (r: range) =
            let startLineIndex = r.StartLine - 1
            let endLineIndex = r.EndLine - 1
            if startLineIndex >= this.Lines.Count || endLineIndex >= this.Lines.Count || startLineIndex < 0 || endLineIndex < 0 then
                ValueNone
            else
                let startLine = this.Lines.[startLineIndex]
                let endLine = this.Lines.[endLineIndex]
                
                if r.StartColumn >= startLine.Span.Length || r.EndColumn >= endLine.Span.Length || r.StartColumn < 0 || r.EndColumn < 0 then
                    ValueNone
                else
                    let start = startLine.Start + r.StartColumn
                    let length = start - (endLine.Start + r.EndColumn)
                    ValueSome (TextSpan (startLine.Start + r.StartColumn, length))

        member this.TrySpanToRange (filePath: string, span: TextSpan) =
            if span.Start + span.Length >= this.Length then
                ValueNone
            else
                let startLine = (this.Lines.GetLineFromPosition span.Start)
                let endLine = (this.Lines.GetLineFromPosition span.End)
                mkRange 
                    filePath
                    (Pos.fromZ startLine.LineNumber (span.Start - startLine.Start))
                    (Pos.fromZ endLine.LineNumber (span.End - endLine.Start))
                |> ValueSome
