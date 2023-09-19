namespace FSharp.Compiler.Benchmarks

open System
open Microsoft.CodeAnalysis.Text
open FSharp.Compiler.Text

module internal SourceText =

    open System.Runtime.CompilerServices

    let private weakTable = ConditionalWeakTable<SourceText, ISourceText>()

    let private create (sourceText: SourceText) =

        let sourceText =
            { new ISourceText with
            
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
                        raise (ArgumentOutOfRangeException("startIndex"))

                    if String.IsNullOrEmpty(target) then
                        raise (ArgumentException("Target is null or empty.", "target"))

                    let lastIndex = startIndex + target.Length
                    if lastIndex <= startIndex || lastIndex >= sourceText.Length then
                        raise (ArgumentException("Target is too big.", "target"))

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

                member this.GetSubTextFromRange range =
                    let totalAmountOfLines = sourceText.Lines.Count

                    if
                        range.StartLine = 0
                        && range.StartColumn = 0
                        && range.EndLine = 0
                        && range.EndColumn = 0
                    then
                        String.Empty
                    elif
                        range.StartLine < 1
                        || (range.StartLine - 1) > totalAmountOfLines
                        || range.EndLine < 1
                        || (range.EndLine - 1) > totalAmountOfLines
                    then
                        invalidArg (nameof range) "The range is outside the file boundaries"
                    else
                        let startLine = range.StartLine - 1
                        let line = this.GetLineString startLine

                        if range.StartLine = range.EndLine then
                            let length = range.EndColumn - range.StartColumn
                            line.Substring(range.StartColumn, length)
                        else
                            let firstLineContent = line.Substring(range.StartColumn)
                            let sb = System.Text.StringBuilder().AppendLine(firstLineContent)

                            for lineNumber in range.StartLine .. range.EndLine - 2 do
                                sb.AppendLine(this.GetLineString lineNumber) |> ignore

                            let lastLine = this.GetLineString(range.EndLine - 1)
                            sb.Append(lastLine.Substring(0, range.EndColumn)).ToString()
            }

        sourceText
    
    let toFSharpSourceText (sourceText : SourceText) =
        weakTable.GetValue(sourceText, ConditionalWeakTable<_,_>.CreateValueCallback(create))

type internal FSharpSourceText = SourceText
type internal FSharpSourceHashAlgorithm = SourceHashAlgorithm
