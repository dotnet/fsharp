// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Diagnostics

[<DiagnosticAnalyzer(FSharpCommonConstants.FSharpLanguageName)>]
type internal TrailingSemicolonDiagnosticAnalyzer() =
    inherit DocumentDiagnosticAnalyzer()
    
    static member DiagnosticId = "TrailingSemicolon"

    override __.SupportedDiagnostics = 
        [DiagnosticDescriptor(TrailingSemicolonDiagnosticAnalyzer.DiagnosticId, "Remove trailing semicolon", "", "", DiagnosticSeverity.Info, true, "", null)].ToImmutableArray()

    override this.AnalyzeSyntaxAsync(document: Document, cancellationToken: CancellationToken) =
        async {
            let! sourceText = document.GetTextAsync() |> Async.AwaitTask
            return
                (sourceText.Lines
                 |> Seq.choose (fun line ->
                     let lineStr = line.ToString()
                     let trimmedLineStr = lineStr.TrimEnd()
                     match trimmedLineStr.LastIndexOf ';' with
                     | -1 -> None
                     | semicolonIndex when semicolonIndex = trimmedLineStr.Length - 1 ->
                         let id = "TrailingSemicolon"
                         let emptyString = LocalizableString.op_Implicit ""
                         let description = LocalizableString.op_Implicit "Trailing semicolon."
                         let severity = DiagnosticSeverity.Info
                         let descriptor = DiagnosticDescriptor(id, emptyString, description, "", severity, true, emptyString, "", null)
                            
                         let linePositionSpan = 
                            LinePositionSpan(
                                LinePosition(line.LineNumber, semicolonIndex),
                                LinePosition(line.LineNumber, semicolonIndex + 1))

                         let textSpan = sourceText.Lines.GetTextSpan(linePositionSpan)
                         let location = Location.Create(document.FilePath, textSpan, linePositionSpan)
                         Some(Diagnostic.Create(descriptor, location))
                      | _ -> None)
                ).ToImmutableArray()
        } |> CommonRoslynHelpers.StartAsyncAsTask cancellationToken

    override this.AnalyzeSemanticsAsync(_, _) = Task.FromResult(ImmutableArray<Diagnostic>.Empty)

    interface IBuiltInAnalyzer with
        member __.OpenFileOnly _ = true
        member __.GetAnalyzerCategory() = DiagnosticAnalyzerCategory.SemanticDocumentAnalysis