namespace FSharp.Compiler.Compilation

open System
open System.Collections.Immutable
open Microsoft.CodeAnalysis.Text
open FSharp.Compiler.Text
open FSharp.Compiler.Range
open Microsoft.CodeAnalysis
open FSharp.Compiler.SourceCodeServices
open FSharp.Compiler
open System.Runtime.CompilerServices
open FSharp.Compiler.ErrorLogger

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
                
                if r.StartColumn > startLine.Span.Length || r.EndColumn > endLine.Span.Length || r.StartColumn < 0 || r.EndColumn < 0 then
                    ValueNone
                else
                    let start = startLine.Start + r.StartColumn
                    let length = (endLine.Start + r.EndColumn) - start
                    ValueSome (TextSpan (startLine.Start + r.StartColumn, length))

        member this.TrySpanToRange (filePath: string, span: TextSpan) =
            if (span.Start + span.Length) > this.Length then
                ValueNone
            else
                let startLine = (this.Lines.GetLineFromPosition span.Start)
                let endLine = (this.Lines.GetLineFromPosition span.End)
                mkRange 
                    filePath
                    (Pos.fromZ startLine.LineNumber (span.Start - startLine.Start))
                    (Pos.fromZ endLine.LineNumber (span.End - endLine.Start))
                |> ValueSome

[<AutoOpen>]
module internal FSharpErrorInfoExtensions =

    let private unnecessary = [|WellKnownDiagnosticTags.Unnecessary;WellKnownDiagnosticTags.Telemetry|]

    let private convertError (error: FSharpErrorInfo) (location: Location) =
        // Normalize the error message into the same format that we will receive it from the compiler.
        // This ensures that IntelliSense and Compiler errors in the 'Error List' are de-duplicated.
        // (i.e the same error does not appear twice, where the only difference is the line endings.)
        let normalizedMessage = error.Message |> ErrorLogger.NormalizeErrorString |> ErrorLogger.NewlineifyErrorString

        let id = "FS" + error.ErrorNumber.ToString("0000")
        let emptyString = LocalizableString.op_Implicit("")
        let description = LocalizableString.op_Implicit(normalizedMessage)
        let severity = if error.Severity = FSharpErrorSeverity.Error then DiagnosticSeverity.Error else DiagnosticSeverity.Warning
        let customTags = 
            match error.ErrorNumber with
            | 1182 -> unnecessary
            | _ -> null
        let descriptor = new DiagnosticDescriptor(id, emptyString, description, error.Subcategory, severity, true, emptyString, String.Empty, customTags)
        Diagnostic.Create(descriptor, location)

    type FSharpErrorInfo with

        member error.ToDiagnostic () =
            convertError error Location.None

        member error.ToDiagnostic (filePath: string, text: SourceText) =
            let linePositionSpan = LinePositionSpan(LinePosition(error.StartLineAlternate - 1, error.StartColumn), LinePosition(error.EndLineAlternate - 1, error.EndColumn))
            let textSpan = text.Lines.GetTextSpan(linePositionSpan)
            let location = Location.Create(filePath, textSpan, linePositionSpan)
            convertError error location

    [<AbstractClass;Extension>]
    type CSharpStyleExtensions private () =

        [<Extension>]
        static member ToDiagnostics(errors: FSharpErrorInfo [], filePath: string, text: SourceText) =
            let diagnostics = ImmutableArray.CreateBuilder (errors.Length)
            diagnostics.Count <- errors.Length
            for i = 0 to errors.Length - 1 do
                diagnostics.[i] <- errors.[i].ToDiagnostic (filePath, text)
            diagnostics.ToImmutable ()

        [<Extension>]
        static member ToDiagnostics(errors: FSharpErrorInfo []) =
            let diagnostics = ImmutableArray.CreateBuilder (errors.Length)
            diagnostics.Count <- errors.Length
            for i = 0 to errors.Length - 1 do
                diagnostics.[i] <- errors.[i].ToDiagnostic ()
            diagnostics.ToImmutable ()

[<AutoOpen>]
module internal ErrorLoggerExtensions =

    [<AbstractClass;Extension>]
    type CSharpStyleExtensions private () =

        [<Extension>]
        static member ToErrorInfos(errors: (PhasedDiagnostic * FSharpErrorSeverity) []) =
            errors
            |> Array.map (fun (error, severity) ->
                let isError = match severity with FSharpErrorSeverity.Error -> true | _ -> false
                FSharpErrorInfo.CreateFromException (error, isError, range0, suggestNames = false)
            )

    type CompilationErrorLogger with

        member this.GetErrorInfos () =
            this.GetErrors().ToErrorInfos ()
