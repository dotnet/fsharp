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
    static member CreateFromException(exn, severity, fallbackRange: range, suggestNames: bool) =
        let m = match GetRangeOfDiagnostic exn with Some m -> m | None -> fallbackRange 
        let msg = bufs (fun buf -> OutputPhasedDiagnostic buf exn false suggestNames)
        let errorNum = GetDiagnosticNumber exn
        FSharpDiagnostic(m, severity, msg, exn.Subcategory(), errorNum, "FS")

    /// Decompose a warning or error into parts: position, severity, message, error number
    static member CreateFromExceptionAndAdjustEof(exn, severity, fallbackRange: range, (linesCount: int, lastLength: int), suggestNames: bool) =
        let r = FSharpDiagnostic.CreateFromException(exn, severity, fallbackRange, suggestNames)

        // Adjust to make sure that errors reported at Eof are shown at the linesCount
        let startline, schange = min (Line.toZ r.Range.StartLine, false) (linesCount, true)
        let endline, echange = min (Line.toZ r.Range.EndLine, false)  (linesCount, true)
        
        if not (schange || echange) then r
        else
            let r = if schange then r.WithStart(mkPos startline lastLength) else r
            if echange then r.WithEnd(mkPos endline (1 + lastLength)) else r

    static member NewlineifyErrorString(message) = NewlineifyErrorString(message)

    static member NormalizeErrorString(text) = NormalizeErrorString(text)
    
    static member Create(severity: FSharpDiagnosticSeverity, message: string, number: int, range: range, ?numberPrefix: string, ?subcategory: string) =
        let subcategory = defaultArg subcategory BuildPhaseSubcategory.TypeCheck
        let numberPrefix = defaultArg numberPrefix "FS"
        FSharpDiagnostic(range, severity, message, subcategory, number, numberPrefix)

/// Use to reset error and warning handlers            
[<Sealed>]
type DiagnosticsScope()  = 
    let mutable diags = [] 
    let mutable firstError = None
    let unwindBP = PushThreadBuildPhaseUntilUnwind BuildPhase.TypeCheck
    let unwindEL =        
        PushDiagnosticsLoggerPhaseUntilUnwind (fun _oldLogger -> 
            { new DiagnosticsLogger("DiagnosticsScope") with 
                member x.DiagnosticSink(exn, severity) = 
                      let err = FSharpDiagnostic.CreateFromException(exn, severity, range.Zero, false)
                      diags <- err :: diags
                      if severity = FSharpDiagnosticSeverity.Error && firstError.IsNone then 
                          firstError <- Some err.Message
                member x.ErrorCount = diags.Length })
        
    member _.Errors = diags |> List.filter (fun error -> error.Severity = FSharpDiagnosticSeverity.Error)

    member _.Diagnostics = diags

    member x.TryGetFirstErrorText() =
        match x.Errors with 
        | error :: _ -> Some error.Message
        | [] -> None
    
    interface IDisposable with
        member _.Dispose() = 
            unwindEL.Dispose() (* unwind pushes when DiagnosticsScope disposes *)
            unwindBP.Dispose()

    member _.FirstError with get() = firstError and set v = firstError <- v
    
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
        use errorScope = new DiagnosticsScope()
        let res = 
            try 
                Some (f())
            with e -> 
                // Here we only call errorRecovery to save the error message for later use by TryGetFirstErrorText.
                try 
                    errorRecovery e m
                with _ -> 
                    // If error recovery fails, then we have an internal compiler error. In this case, we show the whole stack
                    // in the extra message, should the extra message be used.
                    errorScope.FirstError <- Some (e.ToString())
                None
        match res with 
        | Some res -> res
        | None -> 
            match errorScope.TryGetFirstErrorText() with 
            | Some text -> err text
            | None -> err ""

/// An error logger that capture errors, filtering them according to warning levels etc.
type internal CompilationDiagnosticLogger (debugName: string, options: FSharpDiagnosticOptions) = 
    inherit DiagnosticsLogger("CompilationDiagnosticLogger("+debugName+")")
            
    let mutable errorCount = 0
    let diagnostics = ResizeArray<_>()

    override _.DiagnosticSink(err, severity) = 
        if ReportDiagnosticAsError options (err, severity) then
            diagnostics.Add(err, FSharpDiagnosticSeverity.Error)
            errorCount <- errorCount + 1
        elif ReportDiagnosticAsWarning options (err, severity) then
            diagnostics.Add(err, FSharpDiagnosticSeverity.Warning)
        elif ReportDiagnosticAsInfo options (err, severity) then
            diagnostics.Add(err, severity)
    override x.ErrorCount = errorCount

    member x.GetDiagnostics() = diagnostics.ToArray()

module DiagnosticHelpers =                            

    let ReportDiagnostic (options: FSharpDiagnosticOptions, allErrors, mainInputFileName, fileInfo, (exn, severity), suggestNames) = 
        [ let severity = 
               if ReportDiagnosticAsError options (exn, severity) then FSharpDiagnosticSeverity.Error
               else severity
          if (severity = FSharpDiagnosticSeverity.Error || ReportDiagnosticAsWarning options (exn, severity)  || ReportDiagnosticAsInfo options (exn, severity)) then 
            let oneError exn =
                [ // We use the first line of the file as a fallbackRange for reporting unexpected errors.
                  // Not ideal, but it's hard to see what else to do.
                  let fallbackRange = rangeN mainInputFileName 1
                  let ei = FSharpDiagnostic.CreateFromExceptionAndAdjustEof (exn, severity, fallbackRange, fileInfo, suggestNames)
                  let fileName = ei.Range.FileName
                  if allErrors || fileName = mainInputFileName || fileName = TcGlobals.DummyFileNameForRangesWithoutASpecificLocation then
                      yield ei ]

            let mainError, relatedErrors = SplitRelatedDiagnostics exn 
            yield! oneError mainError
            for e in relatedErrors do 
                yield! oneError e ]

    let CreateDiagnostics (options, allErrors, mainInputFileName, errors, suggestNames) = 
        let fileInfo = (Int32.MaxValue, Int32.MaxValue)
        [| for exn, severity in errors do 
              yield! ReportDiagnostic (options, allErrors, mainInputFileName, fileInfo, (exn, severity), suggestNames) |]
