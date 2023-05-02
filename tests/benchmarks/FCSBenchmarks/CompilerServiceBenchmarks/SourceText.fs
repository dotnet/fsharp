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
            }

        sourceText
    
    let toFSharpSourceText (sourceText : SourceText) =
        weakTable.GetValue(sourceText, ConditionalWeakTable<_,_>.CreateValueCallback(create))

type internal FSharpSourceText = SourceText
type internal FSharpSourceHashAlgorithm = SourceHashAlgorithm
