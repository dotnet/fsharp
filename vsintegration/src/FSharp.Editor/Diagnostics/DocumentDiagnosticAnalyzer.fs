﻿// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Diagnostics

open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.Diagnostics

[<RequireQualifiedAccess>]
type internal DiagnosticsType =
    | Syntax
    | Semantic

[<Export(typeof<IFSharpDocumentDiagnosticAnalyzer>)>]
type internal FSharpDocumentDiagnosticAnalyzer
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider, 
        projectInfoManager: FSharpProjectOptionsManager
    ) =

    static let userOpName = "DocumentDiagnosticAnalyzer"

    static let errorInfoEqualityComparer =
        { new IEqualityComparer<FSharpDiagnostic> with 
            member _.Equals (x, y) =
                x.FileName = y.FileName &&
                x.StartLine = y.StartLine &&
                x.EndLine = y.EndLine &&
                x.StartColumn = y.StartColumn &&
                x.EndColumn = y.EndColumn &&
                x.Severity = y.Severity &&
                x.Message = y.Message &&
                x.Subcategory = y.Subcategory &&
                x.ErrorNumber = y.ErrorNumber
            member _.GetHashCode x =
                let mutable hash = 17
                hash <- hash * 23 + x.StartLine.GetHashCode()
                hash <- hash * 23 + x.EndLine.GetHashCode()
                hash <- hash * 23 + x.StartColumn.GetHashCode()
                hash <- hash * 23 + x.EndColumn.GetHashCode()
                hash <- hash * 23 + x.Severity.GetHashCode()
                hash <- hash * 23 + x.Message.GetHashCode()
                hash <- hash * 23 + x.Subcategory.GetHashCode()
                hash <- hash * 23 + x.ErrorNumber.GetHashCode()
                hash 
        }

    static member GetDiagnostics(checker: FSharpChecker, filePath: string, sourceText: SourceText, textVersionHash: int, parsingOptions: FSharpParsingOptions, options: FSharpProjectOptions, diagnosticType: DiagnosticsType) = 
        async {
            let fsSourceText = sourceText.ToFSharpSourceText()
            let! parseResults = checker.ParseFile(filePath, fsSourceText, parsingOptions, userOpName=userOpName) 
            let! errors = 
                async {
                    match diagnosticType with
                    | DiagnosticsType.Semantic ->
                        let! checkResultsAnswer = checker.CheckFileInProject(parseResults, filePath, textVersionHash, fsSourceText, options, userOpName=userOpName) 
                        match checkResultsAnswer with
                        | FSharpCheckFileAnswer.Aborted -> return [||]
                        | FSharpCheckFileAnswer.Succeeded results ->
                            // In order to eleminate duplicates, we should not return parse errors here because they are returned by `AnalyzeSyntaxAsync` method.
                            let allErrors = HashSet(results.Diagnostics, errorInfoEqualityComparer)
                            allErrors.ExceptWith(parseResults.Diagnostics)
                            return Seq.toArray allErrors
                    | DiagnosticsType.Syntax ->
                        return parseResults.Diagnostics
                }
            
            let results = 
                HashSet(errors, errorInfoEqualityComparer)
                |> Seq.choose(fun error ->
                    if error.StartLine = 0 || error.EndLine = 0 then
                        // F# error line numbers are one-based. Compiler returns 0 for global errors (reported by ProjectDiagnosticAnalyzer)
                        None
                    else
                        // Roslyn line numbers are zero-based
                        let linePositionSpan = LinePositionSpan(LinePosition(error.StartLine - 1, error.StartColumn), LinePosition(error.EndLine - 1, error.EndColumn))
                        let textSpan = sourceText.Lines.GetTextSpan(linePositionSpan)
                        
                        // F# compiler report errors at end of file if parsing fails. It should be corrected to match Roslyn boundaries
                        let correctedTextSpan =
                            if textSpan.End <= sourceText.Length then 
                                textSpan 
                            else 
                                let start =
                                    min textSpan.Start (sourceText.Length - 1)
                                    |> max 0

                                TextSpan.FromBounds(start, sourceText.Length)
                        
                        let location = Location.Create(filePath, correctedTextSpan , linePositionSpan)
                        Some(RoslynHelpers.ConvertError(error, location)))
                |> Seq.toImmutableArray
            return results
        }

    interface IFSharpDocumentDiagnosticAnalyzer with

        member this.AnalyzeSyntaxAsync(document: Document, cancellationToken: CancellationToken): Task<ImmutableArray<Diagnostic>> =
            asyncMaybe {
                let! parsingOptions, projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document, cancellationToken, userOpName)
                let! sourceText = document.GetTextAsync(cancellationToken)
                let! textVersion = document.GetTextVersionAsync(cancellationToken)
                return! 
                    FSharpDocumentDiagnosticAnalyzer.GetDiagnostics(checkerProvider.Checker, document.FilePath, sourceText, textVersion.GetHashCode(), parsingOptions, projectOptions, DiagnosticsType.Syntax)
                    |> liftAsync
            } 
            |> Async.map (Option.defaultValue ImmutableArray<Diagnostic>.Empty)
            |> RoslynHelpers.StartAsyncAsTask cancellationToken

        member this.AnalyzeSemanticsAsync(document: Document, cancellationToken: CancellationToken): Task<ImmutableArray<Diagnostic>> =
            asyncMaybe {
                let! parsingOptions, _, projectOptions = projectInfoManager.TryGetOptionsForDocumentOrProject(document, cancellationToken, userOpName) 
                let! sourceText = document.GetTextAsync(cancellationToken)
                let! textVersion = document.GetTextVersionAsync(cancellationToken)
                if document.Project.Name <> FSharpConstants.FSharpMiscellaneousFilesName || isScriptFile document.FilePath then
                    return! 
                        FSharpDocumentDiagnosticAnalyzer.GetDiagnostics(checkerProvider.Checker, document.FilePath, sourceText, textVersion.GetHashCode(), parsingOptions, projectOptions, DiagnosticsType.Semantic)
                        |> liftAsync
                else
                    return ImmutableArray<Diagnostic>.Empty
            }
            |> Async.map (Option.defaultValue ImmutableArray<Diagnostic>.Empty)
            |> RoslynHelpers.StartAsyncAsTask cancellationToken
