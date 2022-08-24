// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// Open up the compiler as an incremental service for parsing, 
// type checking and intellisense-like environment-reporting.
//--------------------------------------------------------------------------

namespace FSharp.Compiler.Diagnostics

open System

open Internal.Utilities.Library  
open Internal.Utilities.Library.Extras

open FSharp.Core.Printf
open FSharp.Compiler 
open FSharp.Compiler.CompilerDiagnostics
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Position
open FSharp.Compiler.Text.Range

type FSharpDiagnostic(m: range, severity: FSharpDiagnosticSeverity, message: string, subcategory: string, errorNum: int, numberPrefix: string) =
    member _.Range = m

    member _.Severity = severity

    member _.Message = message

    member _.Subcategory = subcategory

    member _.ErrorNumber = errorNum

    member _.ErrorNumberPrefix = numberPrefix

    member _.ErrorNumberText = numberPrefix + errorNum.ToString("0000")

    member _.Start = m.Start

    member _.End = m.End

    member _.StartLine = m.Start.Line

    member _.EndLine = m.End.Line
    
    member _.StartColumn = m.Start.Column

    member _.EndColumn = m.End.Column

    member _.FileName = m.FileName

    member _.WithStart newStart =
        let m = mkFileIndexRange m.FileIndex newStart m.End
        FSharpDiagnostic(m, severity, message, subcategory, errorNum, numberPrefix)

    member _.WithEnd newEnd =
        let m = mkFileIndexRange m.FileIndex m.Start newEnd
        FSharpDiagnostic(m, severity, message, subcategory, errorNum, numberPrefix)

    override _.ToString() =
        let fileName = m.FileName
        let s = m.Start
        let e = m.End
        let severity = 
            match severity with
            | FSharpDiagnosticSeverity.Warning -> "warning"
            | FSharpDiagnosticSeverity.Error -> "error"
            | FSharpDiagnosticSeverity.Info -> "info"
            | FSharpDiagnosticSeverity.Hidden -> "hidden"
        sprintf "%s (%d,%d)-(%d,%d) %s %s %s" fileName s.Line (s.Column + 1) e.Line (e.Column + 1) subcategory severity message

    /// Decompose a warning or error into parts: position, severity, message, error number
    static member CreateFromException(diagnostic, severity, fallbackRange: range, suggestNames: bool) =
        let m = match GetRangeOfDiagnostic diagnostic with Some m -> m | None -> fallbackRange 
        let msg = buildString (fun buf -> OutputPhasedDiagnostic buf diagnostic false suggestNames)
        let errorNum = GetDiagnosticNumber diagnostic
        FSharpDiagnostic(m, severity, msg, diagnostic.Subcategory(), errorNum, "FS")

    /// Decompose a warning or error into parts: position, severity, message, error number
    static member CreateFromExceptionAndAdjustEof(diagnostic, severity, fallbackRange: range, (linesCount: int, lastLength: int), suggestNames: bool) =
        let diagnostic = FSharpDiagnostic.CreateFromException(diagnostic, severity, fallbackRange, suggestNames)

        // Adjust to make sure that diagnostics reported at Eof are shown at the linesCount
        let startLine, startChanged = min (Line.toZ diagnostic.Range.StartLine, false) (linesCount, true)
        let endLine, endChanged = min (Line.toZ diagnostic.Range.EndLine, false)  (linesCount, true)
        
        if not (startChanged || endChanged) then
            diagnostic
        else
            let r = if startChanged then diagnostic.WithStart(mkPos startLine lastLength) else diagnostic
            if endChanged then r.WithEnd(mkPos endLine (1 + lastLength)) else r

    static member NewlineifyErrorString(message) = NewlineifyErrorString(message)

    static member NormalizeErrorString(text) = NormalizeErrorString(text)
    
    static member Create(severity, message, number, range, ?numberPrefix, ?subcategory) =
        let subcategory = defaultArg subcategory BuildPhaseSubcategory.TypeCheck
        let numberPrefix = defaultArg numberPrefix "FS"
        FSharpDiagnostic(range, severity, message, subcategory, number, numberPrefix)

/// Use to reset error and warning handlers            
[<Sealed>]
type DiagnosticsScope()  = 
    let mutable diags = [] 
    let unwindBP = UseBuildPhase BuildPhase.TypeCheck
    let unwindEL =        
        UseDiagnosticsLogger 
            { new DiagnosticsLogger("DiagnosticsScope") with 

                member _.DiagnosticSink(diagnostic, severity) = 
                    let diagnostic = FSharpDiagnostic.CreateFromException(diagnostic, severity, range.Zero, false)
                    diags <- diagnostic :: diags

                member _.ErrorCount = diags.Length }
        
    member _.Errors = diags |> List.filter (fun error -> error.Severity = FSharpDiagnosticSeverity.Error)

    member _.Diagnostics = diags

    member x.TryGetFirstErrorText() =
        match x.Errors with 
        | error :: _ -> Some error.Message
        | [] -> None
    
    interface IDisposable with
        member _.Dispose() = 
            unwindEL.Dispose()
            unwindBP.Dispose()

    /// Used at entry points to FSharp.Compiler.Service (service.fsi) which manipulate symbols and
    /// perform other operations which might expose us to either bona-fide F# error messages such 
    /// "missing assembly" (for incomplete assembly reference sets), or, if there is a compiler bug, 
    /// may hit internal compiler failures.
    ///
    /// In some calling cases, we get a chance to report the error as part of user text. For example
    /// if there is a "missing assembly" error while formatting the text of the description of an
    /// autocomplete, then the error message is shown in replacement of the text (rather than crashing Visual
    /// Studio, or swallowing the exception completely)
    static member Protect<'a> (m: range) (f: unit->'a) (err: string->'a): 'a = 
        use diagnosticsScope = new DiagnosticsScope()
        let res = 
            try 
                Some (f())
            with e -> 
                // Here we only call errorRecovery to save the error message for later use by TryGetFirstErrorText.
                try 
                    errorRecovery e m
                with _ -> 
                    ()
                None
        match res with 
        | Some res -> res
        | None -> 
            match diagnosticsScope.TryGetFirstErrorText() with 
            | Some text -> err text
            | None -> err ""

/// A diagnostics logger that capture diagnostics, filtering them according to warning levels etc.
type internal CompilationDiagnosticLogger (debugName: string, options: FSharpDiagnosticOptions) = 
    inherit DiagnosticsLogger("CompilationDiagnosticLogger("+debugName+")")
            
    let mutable errorCount = 0
    let diagnostics = ResizeArray<_>()

    override _.DiagnosticSink(diagnostic, severity) = 
        if ReportDiagnosticAsError options (diagnostic, severity) then
            diagnostics.Add(diagnostic, FSharpDiagnosticSeverity.Error)
            errorCount <- errorCount + 1
        elif ReportDiagnosticAsWarning options (diagnostic, severity) then
            diagnostics.Add(diagnostic, FSharpDiagnosticSeverity.Warning)
        elif ReportDiagnosticAsInfo options (diagnostic, severity) then
            diagnostics.Add(diagnostic, severity)
    
    override _.ErrorCount = errorCount

    member _.GetDiagnostics() = diagnostics.ToArray()

module DiagnosticHelpers =                            

    let ReportDiagnostic (options: FSharpDiagnosticOptions, allErrors, mainInputFileName, fileInfo, diagnostic, severity, suggestNames) = 
        [ let severity = 
               if ReportDiagnosticAsError options (diagnostic, severity) then
                   FSharpDiagnosticSeverity.Error
               else
                   severity

          if severity = FSharpDiagnosticSeverity.Error ||
             ReportDiagnosticAsWarning options (diagnostic, severity) ||
             ReportDiagnosticAsInfo options (diagnostic, severity) then 

            // We use the first line of the file as a fallbackRange for reporting unexpected errors.
            // Not ideal, but it's hard to see what else to do.
            let fallbackRange = rangeN mainInputFileName 1
            let diagnostic = FSharpDiagnostic.CreateFromExceptionAndAdjustEof (diagnostic, severity, fallbackRange, fileInfo, suggestNames)
            let fileName = diagnostic.Range.FileName
            if allErrors || fileName = mainInputFileName || fileName = TcGlobals.DummyFileNameForRangesWithoutASpecificLocation then
                yield diagnostic ]

    let CreateDiagnostics (options, allErrors, mainInputFileName, diagnostics, suggestNames) = 
        let fileInfo = (Int32.MaxValue, Int32.MaxValue)
        [| for diagnostic, severity in diagnostics do 
              yield! ReportDiagnostic (options, allErrors, mainInputFileName, fileInfo, diagnostic, severity, suggestNames) |]
