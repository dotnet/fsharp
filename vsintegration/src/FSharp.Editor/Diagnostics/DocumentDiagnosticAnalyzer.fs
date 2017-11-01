// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Immutable
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Diagnostics
open Microsoft.CodeAnalysis.Text

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.SourceCodeServices


[<RequireQualifiedAccess>]
type internal DiagnosticsType =
    | Syntax
    | Semantic

[<DiagnosticAnalyzer(FSharpConstants.FSharpLanguageName)>]
type internal FSharpDocumentDiagnosticAnalyzer() =
    inherit DocumentDiagnosticAnalyzer()

    static let userOpName = "DocumentDiagnosticAnalyzer"
    let getChecker(document: Document) =
        document.Project.Solution.Workspace.Services.GetService<FSharpCheckerWorkspaceService>().Checker

    let getProjectInfoManager(document: Document) =
        document.Project.Solution.Workspace.Services.GetService<FSharpCheckerWorkspaceService>().FSharpProjectOptionsManager
    
    static let errorInfoEqualityComparer =
        { new IEqualityComparer<FSharpErrorInfo> with 
            member __.Equals (x, y) =
                x.FileName = y.FileName &&
                x.StartLineAlternate = y.StartLineAlternate &&
                x.EndLineAlternate = y.EndLineAlternate &&
                x.StartColumn = y.StartColumn &&
                x.EndColumn = y.EndColumn &&
                x.Severity = y.Severity &&
                x.Message = y.Message &&
                x.Subcategory = y.Subcategory &&
                x.ErrorNumber = y.ErrorNumber
            member __.GetHashCode x =
                let mutable hash = 17
                hash <- hash * 23 + x.StartLineAlternate.GetHashCode()
                hash <- hash * 23 + x.EndLineAlternate.GetHashCode()
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
            let! parseResults = checker.ParseFile(filePath, sourceText.ToString(), parsingOptions, userOpName=userOpName) 
            let! errors = 
                async {
                    match diagnosticType with
                    | DiagnosticsType.Semantic ->
                        let! checkResultsAnswer = checker.CheckFileInProject(parseResults, filePath, textVersionHash, sourceText.ToString(), options, userOpName=userOpName) 
                        match checkResultsAnswer with
                        | FSharpCheckFileAnswer.Aborted -> return [||]
                        | FSharpCheckFileAnswer.Succeeded results ->
                            // In order to eleminate duplicates, we should not return parse errors here because they are returned by `AnalyzeSyntaxAsync` method.
                            let allErrors = HashSet(results.Errors, errorInfoEqualityComparer)
                            allErrors.ExceptWith(parseResults.Errors)
                            return Seq.toArray allErrors
                    | DiagnosticsType.Syntax ->
                        return parseResults.Errors
                }
            
            let results = 
                HashSet(errors, errorInfoEqualityComparer)
                |> Seq.choose(fun error ->
                    if error.StartLineAlternate = 0 || error.EndLineAlternate = 0 then
                        // F# error line numbers are one-based. Compiler returns 0 for global errors (reported by ProjectDiagnosticAnalyzer)
                        None
                    else
                        // Roslyn line numbers are zero-based
                        let linePositionSpan = LinePositionSpan(LinePosition(error.StartLineAlternate - 1, error.StartColumn), LinePosition(error.EndLineAlternate - 1, error.EndColumn))
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

    override this.SupportedDiagnostics = RoslynHelpers.SupportedDiagnostics()

    override this.AnalyzeSyntaxAsync(document: Document, cancellationToken: CancellationToken): Task<ImmutableArray<Diagnostic>> =
        let projectInfoManager = getProjectInfoManager document
        asyncMaybe {
            let! parsingOptions, projectOptions = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document)
            let! sourceText = document.GetTextAsync(cancellationToken)
            let! textVersion = document.GetTextVersionAsync(cancellationToken)
            return! 
                FSharpDocumentDiagnosticAnalyzer.GetDiagnostics(getChecker document, document.FilePath, sourceText, textVersion.GetHashCode(), parsingOptions, projectOptions, DiagnosticsType.Syntax)
                |> liftAsync
        } 
        |> Async.map (Option.defaultValue ImmutableArray<Diagnostic>.Empty)
        |> RoslynHelpers.StartAsyncAsTask cancellationToken

    override this.AnalyzeSemanticsAsync(document: Document, cancellationToken: CancellationToken): Task<ImmutableArray<Diagnostic>> =
        let projectInfoManager = getProjectInfoManager document
        asyncMaybe {
            let! parsingOptions, _, projectOptions = projectInfoManager.TryGetOptionsForDocumentOrProject(document) 
            let! sourceText = document.GetTextAsync(cancellationToken)
            let! textVersion = document.GetTextVersionAsync(cancellationToken)
            return! 
                FSharpDocumentDiagnosticAnalyzer.GetDiagnostics(getChecker document, document.FilePath, sourceText, textVersion.GetHashCode(), parsingOptions, projectOptions, DiagnosticsType.Semantic)
                |> liftAsync
        }
        |> Async.map (Option.defaultValue ImmutableArray<Diagnostic>.Empty)
        |> RoslynHelpers.StartAsyncAsTask cancellationToken

    interface IBuiltInAnalyzer with
        member __.GetAnalyzerCategory() : DiagnosticAnalyzerCategory = DiagnosticAnalyzerCategory.SemanticDocumentAnalysis
        member __.OpenFileOnly _ = true

