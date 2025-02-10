// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

//----------------------------------------------------------------------------
// Open up the compiler as an incremental service for parsing, 
// type checking and intellisense-like environment-reporting.
//--------------------------------------------------------------------------

namespace FSharp.Compiler.Diagnostics

open System

open FSharp.Compiler.AttributeChecking
open FSharp.Compiler.CheckExpressions
open FSharp.Compiler.ConstraintSolver
open FSharp.Compiler.SignatureConformance
open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax
open FSharp.Compiler.TypedTree
open FSharp.Compiler.TypedTreeBasics
open FSharp.Compiler.TypedTreeOps
open Internal.Utilities.Library

open FSharp.Core.Printf
open FSharp.Compiler
open FSharp.Compiler.CompilerDiagnostics
open FSharp.Compiler.Diagnostics
open FSharp.Compiler.DiagnosticsLogger
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Position
open FSharp.Compiler.Text.Range

module ExtendedData =
    [<RequireQualifiedAccess; Experimental("This FCS API is experimental and subject to change.")>]
    type DiagnosticContextInfo =
        | NoContext
        | IfExpression
        | OmittedElseBranch
        | ElseBranchResult
        | RecordFields
        | TupleInRecordFields
        | CollectionElement
        | ReturnInComputationExpression
        | YieldInComputationExpression
        | RuntimeTypeTest
        | DowncastUsedInsteadOfUpcast
        | FollowingPatternMatchClause
        | PatternMatchGuard
        | SequenceExpression
        with
        static member From(contextInfo: ContextInfo) =
            match contextInfo with
            | ContextInfo.NoContext -> NoContext
            | ContextInfo.IfExpression _ -> IfExpression
            | ContextInfo.OmittedElseBranch _ -> OmittedElseBranch
            | ContextInfo.ElseBranchResult _ -> ElseBranchResult
            | ContextInfo.RecordFields -> RecordFields
            | ContextInfo.TupleInRecordFields -> TupleInRecordFields
            | ContextInfo.CollectionElement _ -> CollectionElement
            | ContextInfo.ReturnInComputationExpression -> ReturnInComputationExpression
            | ContextInfo.YieldInComputationExpression -> YieldInComputationExpression
            | ContextInfo.RuntimeTypeTest _ -> RuntimeTypeTest
            | ContextInfo.DowncastUsedInsteadOfUpcast _ -> DowncastUsedInsteadOfUpcast
            | ContextInfo.FollowingPatternMatchClause _ -> FollowingPatternMatchClause
            | ContextInfo.PatternMatchGuard _ -> PatternMatchGuard
            | ContextInfo.SequenceExpression _ -> SequenceExpression

    [<Interface; Experimental("This FCS API is experimental and subject to change.")>]
    type IFSharpDiagnosticExtendedData = interface end

    /// Additional data for diagnostics about obsolete attributes.
    [<Class; Experimental("This FCS API is experimental and subject to change.")>]
    type ObsoleteDiagnosticExtendedData
        internal (diagnosticId: string option, urlFormat: string option) =
        interface IFSharpDiagnosticExtendedData
        /// Represents the DiagnosticId of the diagnostic
        member this.DiagnosticId: string option = diagnosticId

        /// Represents the URL format of the diagnostic
        member this.UrlFormat: string option = urlFormat

    /// Additional data for diagnostics about experimental attributes.
    [<Class; Experimental("This FCS API is experimental and subject to change.")>]
    type ExperimentalExtendedData
        internal (diagnosticId: string option, urlFormat: string option) =
        interface IFSharpDiagnosticExtendedData
        /// Represents the DiagnosticId of the diagnostic
        member this.DiagnosticId: string option = diagnosticId

        /// Represents the URL format of the diagnostic
        member this.UrlFormat: string option = urlFormat
    
    [<Experimental("This FCS API is experimental and subject to change.")>]
    type TypeMismatchDiagnosticExtendedData
        internal (symbolEnv: SymbolEnv, dispEnv: DisplayEnv, expectedType: TType, actualType: TType, context: DiagnosticContextInfo) =
        interface IFSharpDiagnosticExtendedData

        member x.ExpectedType = FSharpType(symbolEnv, expectedType)
        member x.ActualType = FSharpType(symbolEnv, actualType)
        member x.ContextInfo = context
        member x.DisplayContext = FSharpDisplayContext(fun _ -> dispEnv)

    [<Experimental("This FCS API is experimental and subject to change.")>]
    type ExpressionIsAFunctionExtendedData
        internal (symbolEnv: SymbolEnv, actualType: TType) =
        interface IFSharpDiagnosticExtendedData

        member x.ActualType = FSharpType(symbolEnv, actualType)

    [<Experimental("This FCS API is experimental and subject to change.")>]
    type FieldNotContainedDiagnosticExtendedData
        internal (symbolEnv: SymbolEnv, implTycon: Tycon, sigTycon: Tycon, signatureField: RecdField, implementationField: RecdField) =
        interface IFSharpDiagnosticExtendedData
        member x.SignatureField = FSharpField(symbolEnv, RecdFieldRef.RecdFieldRef(mkLocalTyconRef sigTycon, signatureField.Id.idText))
        member x.ImplementationField = FSharpField(symbolEnv, RecdFieldRef.RecdFieldRef(mkLocalTyconRef implTycon, implementationField.Id.idText))

    [<Experimental("This FCS API is experimental and subject to change.")>]
    type ValueNotContainedDiagnosticExtendedData
        internal (symbolEnv: SymbolEnv, signatureValue: Val, implValue: Val) =
        interface IFSharpDiagnosticExtendedData
        member x.SignatureValue = FSharpMemberOrFunctionOrValue(symbolEnv, mkLocalValRef signatureValue)
        member x.ImplementationValue = FSharpMemberOrFunctionOrValue(symbolEnv, mkLocalValRef implValue)

    [<Experimental("This FCS API is experimental and subject to change.")>]
    type ArgumentsInSigAndImplMismatchExtendedData
        internal(sigArg: Ident, implArg: Ident) =
        interface IFSharpDiagnosticExtendedData
        member x.SignatureName = sigArg.idText
        member x.ImplementationName = implArg.idText
        member x.SignatureRange = sigArg.idRange
        member x.ImplementationRange = implArg.idRange
        
    [<Class; Experimental("This FCS API is experimental and subject to change.")>]
    type DefinitionsInSigAndImplNotCompatibleAbbreviationsDifferExtendedData
        internal(signatureType: Tycon, implementationType: Tycon) =
        interface IFSharpDiagnosticExtendedData
        member x.SignatureRange: range = signatureType.Range
        member x.ImplementationRange: range = implementationType.Range

open ExtendedData

type FSharpDiagnostic(m: range, severity: FSharpDiagnosticSeverity, message: string, subcategory: string, errorNum: int, numberPrefix: string, extendedData: IFSharpDiagnosticExtendedData option) =
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

    [<Experimental("This FCS API is experimental and subject to change.")>]
    member _.ExtendedData = extendedData

    member _.WithStart newStart =
        let m = mkFileIndexRange m.FileIndex newStart m.End
        FSharpDiagnostic(m, severity, message, subcategory, errorNum, numberPrefix, extendedData)

    member _.WithEnd newEnd =
        let m = mkFileIndexRange m.FileIndex m.Start newEnd
        FSharpDiagnostic(m, severity, message, subcategory, errorNum, numberPrefix, extendedData)

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
    static member CreateFromException(diagnostic: PhasedDiagnostic, severity, fallbackRange: range, suggestNames: bool, flatErrors: bool, symbolEnv: SymbolEnv option) =
        let m = match diagnostic.Range with Some m -> m | None -> fallbackRange
        let extendedData: IFSharpDiagnosticExtendedData option =
            match symbolEnv with
            | None -> None
            | Some symbolEnv ->

            match diagnostic.Exception with
            | ErrorFromAddingTypeEquation(_, displayEnv, expectedType, actualType, ConstraintSolverTupleDiffLengths(contextInfo = contextInfo), _)
            | ErrorFromAddingTypeEquation(_, displayEnv, expectedType, actualType, ConstraintSolverTypesNotInEqualityRelation(_, _, _, _, _, contextInfo), _)
            | ErrorsFromAddingSubsumptionConstraint(_, displayEnv, expectedType, actualType, _, contextInfo, _) ->
               let context = DiagnosticContextInfo.From(contextInfo)
               Some(TypeMismatchDiagnosticExtendedData(symbolEnv, displayEnv, expectedType, actualType, context))

            | ErrorFromAddingTypeEquation(_, displayEnv, expectedType, actualType, _, _)->
               Some(TypeMismatchDiagnosticExtendedData(symbolEnv, displayEnv, expectedType, actualType, DiagnosticContextInfo.NoContext))

            | FunctionValueUnexpected(_, actualType, _) ->
                Some(ExpressionIsAFunctionExtendedData(symbolEnv, actualType))

            | FieldNotContained(_,_, _, implEntity, sigEntity, impl, sign, _) ->
                Some(FieldNotContainedDiagnosticExtendedData(symbolEnv, implEntity, sigEntity, sign, impl))

            | ValueNotContained(_,_, _, _, implValue, sigValue, _) ->
                Some(ValueNotContainedDiagnosticExtendedData(symbolEnv, sigValue, implValue))

            | ArgumentsInSigAndImplMismatch(sigArg, implArg) ->
                Some(ArgumentsInSigAndImplMismatchExtendedData(sigArg, implArg))

            | DefinitionsInSigAndImplNotCompatibleAbbreviationsDiffer(implTycon = implTycon; sigTycon = sigTycon) ->
                Some(DefinitionsInSigAndImplNotCompatibleAbbreviationsDifferExtendedData(sigTycon, implTycon))

            | ObsoleteDiagnostic(diagnosticId= diagnosticId; urlFormat= urlFormat) ->
                Some(ObsoleteDiagnosticExtendedData(diagnosticId, urlFormat))
                
            | Experimental(diagnosticId= diagnosticId; urlFormat= urlFormat) ->
                Some(ExperimentalExtendedData(diagnosticId, urlFormat))
            | _ -> None

        let msg =
             match diagnostic.Exception.Data["CachedFormatCore"] with
             | :? string as message -> message
             | _ -> diagnostic.FormatCore(flatErrors, suggestNames)

        let errorNum = diagnostic.Number
        FSharpDiagnostic(m, severity, msg, diagnostic.Subcategory(), errorNum, "FS", extendedData)

    /// Decompose a warning or error into parts: position, severity, message, error number
    static member CreateFromExceptionAndAdjustEof(diagnostic, severity, fallbackRange: range, (linesCount: int, lastLength: int), suggestNames: bool, flatErrors: bool, symbolEnv: SymbolEnv option) =
        let diagnostic = FSharpDiagnostic.CreateFromException(diagnostic, severity, fallbackRange, suggestNames, flatErrors, symbolEnv)

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
        FSharpDiagnostic(range, severity, message, subcategory, number, numberPrefix, None)

/// Use to reset error and warning handlers            
[<Sealed>]
type DiagnosticsScope(flatErrors: bool)  = 
    let mutable diags = [] 
    let unwindBP = UseBuildPhase BuildPhase.TypeCheck
    let unwindEL =        
        UseDiagnosticsLogger 
            { new DiagnosticsLogger("DiagnosticsScope") with 

                member _.DiagnosticSink(diagnostic, severity) = 
                    let diagnostic = FSharpDiagnostic.CreateFromException(diagnostic, severity, range.Zero, false, flatErrors, None)
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
        use diagnosticsScope = new DiagnosticsScope(false)
        let res = 
            try 
                Some (f())
            with e -> 
                // Here we only call errorRecovery to save the error message for later use by TryGetFirstErrorText.
                try 
                    errorRecovery e m
                with RecoverableException _ -> 
                    ()
                None
        match res with 
        | Some res -> res
        | None -> 
            match diagnosticsScope.TryGetFirstErrorText() with 
            | Some text -> err text
            | None -> err ""

/// A diagnostics logger that capture diagnostics, filtering them according to warning levels etc.
type internal CompilationDiagnosticLogger (debugName: string, options: FSharpDiagnosticOptions, ?preprocess: (PhasedDiagnostic -> PhasedDiagnostic)) =
    inherit DiagnosticsLogger("CompilationDiagnosticLogger("+debugName+")")
            
    let mutable errorCount = 0
    let diagnostics = ResizeArray<_>()

    override _.DiagnosticSink(diagnostic, severity) = 
        let diagnostic =
            match preprocess with
            | Some f -> f diagnostic
            | None -> diagnostic

        match diagnostic.AdjustSeverity(options, severity) with
        | FSharpDiagnosticSeverity.Error ->
            diagnostics.Add(diagnostic, FSharpDiagnosticSeverity.Error)
            errorCount <- errorCount + 1
        | FSharpDiagnosticSeverity.Hidden -> ()
        | sev -> diagnostics.Add(diagnostic, sev)

    override _.ErrorCount = errorCount

    member _.GetDiagnostics() = diagnostics.ToArray()

module DiagnosticHelpers =                            

    let ReportDiagnostic (options: FSharpDiagnosticOptions, allErrors, mainInputFileName, fileInfo, diagnostic: PhasedDiagnostic, severity, suggestNames, flatErrors, symbolEnv) =
        match diagnostic.AdjustSeverity(options, severity) with
        | FSharpDiagnosticSeverity.Hidden -> []
        | adjustedSeverity ->

            // We use the first line of the file as a fallbackRange for reporting unexpected errors.
            // Not ideal, but it's hard to see what else to do.
            let fallbackRange = rangeN mainInputFileName 1
            let diagnostic = FSharpDiagnostic.CreateFromExceptionAndAdjustEof (diagnostic, adjustedSeverity, fallbackRange, fileInfo, suggestNames, flatErrors, symbolEnv)
            let fileName = diagnostic.Range.FileName
            if allErrors || fileName = mainInputFileName || fileName = TcGlobals.DummyFileNameForRangesWithoutASpecificLocation then
                [diagnostic]
            else []

    let CreateDiagnostics (options, allErrors, mainInputFileName, diagnostics, suggestNames, flatErrors, symbolEnv) =
        let fileInfo = (Int32.MaxValue, Int32.MaxValue)
        [| for diagnostic, severity in diagnostics do 
              yield! ReportDiagnostic (options, allErrors, mainInputFileName, fileInfo, diagnostic, severity, suggestNames, flatErrors, symbolEnv) |]
