// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Diagnostics
open Microsoft.FSharp.Compiler.SourceCodeServices

type private LineCheckSum = ImmutableArray<byte>

[<DiagnosticAnalyzer(FSharpCommonConstants.FSharpLanguageName)>]
type internal TrailingSemicolonDiagnosticAnalyzer() =
    inherit DocumentDiagnosticAnalyzer()
    
    let cacheByDocumentId = ConditionalWeakTable<DocumentId, ResizeArray<(LineCheckSum * Location option) option>>()
    
    let getLexer(document: Document) = 
        document.Project.Solution.Workspace.Services.GetService<FSharpCheckerWorkspaceService>().Lexer

    let getProjectInfoManager(document: Document) =
        document.Project.Solution.Workspace.Services.GetService<FSharpCheckerWorkspaceService>().ProjectInfoManager

    static member DiagnosticId = "TrailingSemicolon"

    override __.SupportedDiagnostics = 
        [DiagnosticDescriptor(TrailingSemicolonDiagnosticAnalyzer.DiagnosticId, "Remove trailing semicolon", "", "", DiagnosticSeverity.Info, true, "", null)].ToImmutableArray()

    override this.AnalyzeSyntaxAsync(document: Document, cancellationToken: CancellationToken) =
        async {
            match getProjectInfoManager(document).TryGetOptionsForEditingDocumentOrProject(document) with 
            | Some options ->
                let! sourceText = document.GetTextAsync() |> Async.AwaitTask
                let lines = sourceText.Lines
                let defines = CompilerEnvironment.GetCompilationDefinesForEditing(document.FilePath, Seq.toList options.OtherOptions)
                let lineDatas = getLexer(document).GetSourceLineDatas(document.Id, sourceText, 0, sourceText.Lines.Count - 1, Some document.FilePath, defines, cancellationToken)
                let cache =
                    match cacheByDocumentId.TryGetValue document.Id with
                    | true, x -> 
                        if x.Count < lineDatas.Count then
                            x.Capacity <- lineDatas.Capacity
                            for __ in 1..lineDatas.Count - x.Count do
                                x.Add None
                        elif x.Count > lineDatas.Count then
                            for i in 1..x.Count - lineDatas.Count do
                                x.RemoveAt(x.Count - i)
                        else ()
                        x
                    | _ -> 
                        let cache = ResizeArray(lineDatas.Count)
                        for __ in 1..lineDatas.Count do cache.Add None
                        cache
                
                let getTrailingSemicolonIndex (line: string) : int =
                    let rec loop (index: int) =
                        if index < 0 then -1 
                        elif line.[index] = ';' then index
                        elif Char.IsWhiteSpace(line.[index]) then loop (index - 1)
                        else -1
                    loop (line.Length - 1)

                let locations =
                    lineDatas
                    |> Seq.mapi (fun lineNumber lineData ->
                        match cache.[lineNumber] with
                        | Some (oldCheckSum, oldLocation) when oldCheckSum = lineData.CheckSum -> oldLocation
                        | _ ->
                            let location =
                                match getTrailingSemicolonIndex (lines.[lineNumber].ToString()) with
                                | -1 -> None
                                | lastSemicolonIndex ->
                                    let linePositionSpan = 
                                        LinePositionSpan(
                                            LinePosition(lineNumber, lastSemicolonIndex), 
                                            LinePosition(lineNumber, lastSemicolonIndex + 1))
                                    let textSpan = sourceText.Lines.GetTextSpan(linePositionSpan)
                                    let location = Location.Create(document.FilePath, textSpan, linePositionSpan)
                                    Some location
                            cache.[lineNumber] <- Some (lineData.CheckSum, location)
                            location)
                     |> Seq.choose id

                cacheByDocumentId.Remove(document.Id) |> ignore
                cacheByDocumentId.Add(document.Id, cache)

                return
                    (locations
                     |> Seq.map (fun location ->
                         let id = "TrailingSemicolon"
                         let emptyString = LocalizableString.op_Implicit ""
                         let description = LocalizableString.op_Implicit "Trailing semicolon."
                         let severity = DiagnosticSeverity.Info
                         let descriptor = DiagnosticDescriptor(id, emptyString, description, "", severity, true, emptyString, "", null)
                         Diagnostic.Create(descriptor, location))
                ).ToImmutableArray()
            | None -> return ImmutableArray<_>.Empty
        } |> CommonRoslynHelpers.StartAsyncAsTask cancellationToken

    override this.AnalyzeSemanticsAsync(_, _) = Task.FromResult(ImmutableArray<Diagnostic>.Empty)

    interface IBuiltInAnalyzer with
        member __.OpenFileOnly _ = true
        member __.GetAnalyzerCategory() = DiagnosticAnalyzerCategory.SemanticDocumentAnalysis