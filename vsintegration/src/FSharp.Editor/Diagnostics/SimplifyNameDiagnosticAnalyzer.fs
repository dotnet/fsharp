// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Diagnostics
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.VisualStudio.FSharp.LanguageService

type private LineHash = int

[<DiagnosticAnalyzer(FSharpCommonConstants.FSharpLanguageName)>]
type internal SimplifyNameDiagnosticAnalyzer() =
    inherit DocumentDiagnosticAnalyzer()
    
    let getProjectInfoManager (document: Document) = document.Project.Solution.Workspace.Services.GetService<FSharpCheckerWorkspaceService>().ProjectInfoManager
    let getChecker (document: Document) = document.Project.Solution.Workspace.Services.GetService<FSharpCheckerWorkspaceService>().Checker
    let getPlidLength (plid: string list) = (plid |> List.sumBy String.length) + plid.Length

    static let Descriptor = 
        DiagnosticDescriptor(IDEDiagnosticIds.SimplifyNamesDiagnosticId, SR.SimplifyName.Value, "", "", DiagnosticSeverity.Hidden, true, "", "", DiagnosticCustomTags.Unnecessary)

    static member LongIdentPropertyKey = "FullName"
    
    override __.SupportedDiagnostics = ImmutableArray.Create Descriptor

    override this.AnalyzeSyntaxAsync(document: Document, cancellationToken: CancellationToken) =
        asyncMaybe {
            match getProjectInfoManager(document).TryGetOptionsForEditingDocumentOrProject(document) with 
            | Some options ->
                let! sourceText = document.GetTextAsync()
                let checker = getChecker document
                let! _, checkResults = checker.ParseAndCheckDocument(document, options, sourceText)
                let! symbolUses = checkResults.GetAllUsesOfAllSymbolsInFile() |> liftAsync
                let mutable result = ResizeArray()
                
                for symbolUse in symbolUses do
                    if not symbolUse.IsFromDefinition then
                        let lineStr = sourceText.Lines.[Line.toZ symbolUse.RangeAlternate.StartLine].ToString()
                        // for `System.DateTime.Now` it returns ([|"System"; "DateTime"|], "Now")
                        let plid, name = QuickParse.GetPartialLongNameEx(lineStr, symbolUse.RangeAlternate.EndColumn - 1) 
                        let posAtStartOfName =
                            let r = symbolUse.RangeAlternate
                            if r.StartLine = r.EndLine then Range.mkPos r.StartLine (r.EndColumn - name.Length)
                            else r.Start

                        let getNecessaryPlid (plid: string list) : Async<string list> =
                            let rec loop (rest: string list) (current: string list) =
                                async {
                                    match rest with
                                    | [] -> return current
                                    | headIdent :: restPlid ->
                                        let! res = checkResults.IsRelativeNameResolvable(posAtStartOfName, current, symbolUse.Symbol.Item) 
                                        if res then return current
                                        else return! loop restPlid (headIdent :: current)
                                }
                            loop (List.rev plid) [] |> Async.map List.rev
                           
                        let! necessaryPlid = getNecessaryPlid plid |> liftAsync
                            
                        match necessaryPlid with
                        | necessaryPlid when necessaryPlid = plid -> ()
                        | necessaryPlid ->
                            let r = symbolUse.RangeAlternate
                            // `symbolUse.RangeAlternate.Start` does not point to the start of plid, it points to start of `name`,
                            // so we have to calculate plid's start ourselves.
                            let plidStartCol = r.EndColumn - name.Length - (getPlidLength plid)
                            let necessaryPlidStartCol = r.EndColumn - name.Length - (getPlidLength necessaryPlid)

                            let unnecessaryRange = 
                                Range.mkRange r.FileName (Range.mkPos r.StartLine plidStartCol) (Range.mkPos r.EndLine necessaryPlidStartCol)
                            
                            let relativeName = (String.concat "." plid) + "." + name
                            result.Add(symbolUse.RangeAlternate.EndColumn, unnecessaryRange, relativeName)
                return
                    (result
                     |> Seq.groupBy (fun (_, r, _) -> r.Start)
                     |> Seq.map (fun (_, relativeNames) ->
                          let maxEndCol = relativeNames |> Seq.map (fun (endCol, _, _) -> endCol) |> Seq.max
                          
                          let unnecessaryRange, relativeName =
                              relativeNames 
                              |> Seq.filter (fun (pos, _, _) -> pos = maxEndCol)
                              |> Seq.maxBy (fun (_, _, name) -> name.Length)
                              |> fun (_, r, name) -> r, name
                         
                          Diagnostic.Create(
                               Descriptor,
                               CommonRoslynHelpers.RangeToLocation(unnecessaryRange, sourceText, document.FilePath),
                               properties = (dict [SimplifyNameDiagnosticAnalyzer.LongIdentPropertyKey, relativeName]).ToImmutableDictionary()))
                    ).ToImmutableArray()
            
            | None -> return ImmutableArray.Empty
        } 
        |> Async.map (Option.defaultValue ImmutableArray.Empty)
        |> CommonRoslynHelpers.StartAsyncAsTask cancellationToken

    override this.AnalyzeSemanticsAsync(_, _) = Task.FromResult ImmutableArray<Diagnostic>.Empty

    interface IBuiltInAnalyzer with
        member __.OpenFileOnly _ = true
        member __.GetAnalyzerCategory() = DiagnosticAnalyzerCategory.SemanticDocumentAnalysis