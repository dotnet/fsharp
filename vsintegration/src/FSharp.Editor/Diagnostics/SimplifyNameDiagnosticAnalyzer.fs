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

type private TextVersionHash = int

// TODO Turn it on when user settings dialog is ready to switch it on and off.
// [<DiagnosticAnalyzer(FSharpCommonConstants.FSharpLanguageName)>]
type internal SimplifyNameDiagnosticAnalyzer() =
    inherit DocumentDiagnosticAnalyzer()
    
    let getProjectInfoManager (document: Document) = document.Project.Solution.Workspace.Services.GetService<FSharpCheckerWorkspaceService>().ProjectInfoManager
    let getChecker (document: Document) = document.Project.Solution.Workspace.Services.GetService<FSharpCheckerWorkspaceService>().Checker
    let getPlidLength (plid: string list) = (plid |> List.sumBy String.length) + plid.Length
    static let cache = ConditionalWeakTable<DocumentId, TextVersionHash * ImmutableArray<Diagnostic>>()
    static let guard = new SemaphoreSlim(1)

    static let Descriptor = 
        DiagnosticDescriptor(
            id = IDEDiagnosticIds.SimplifyNamesDiagnosticId, 
            title = SR.SimplifyName.Value, 
            messageFormat = SR.NameCanBeSimplified.Value, 
            category = DiagnosticCategory.Style, 
            defaultSeverity = DiagnosticSeverity.Hidden, 
            isEnabledByDefault = true, 
            customTags = DiagnosticCustomTags.Unnecessary)

    static member LongIdentPropertyKey = "FullName"
    
    override __.SupportedDiagnostics = ImmutableArray.Create Descriptor
    override this.AnalyzeSyntaxAsync(_, _) = Task.FromResult ImmutableArray<Diagnostic>.Empty

    override this.AnalyzeSemanticsAsync(document: Document, cancellationToken: CancellationToken) =
        asyncMaybe {
            let! options = getProjectInfoManager(document).TryGetOptionsForEditingDocumentOrProject(document)
            let! textVersion = document.GetTextVersionAsync(cancellationToken)
            let textVersionHash = textVersion.GetHashCode()
            let! _ = guard.WaitAsync(cancellationToken) |> Async.AwaitTask |> liftAsync
            try
                match cache.TryGetValue document.Id with
                | true, (oldTextVersionHash, diagnostics) when oldTextVersionHash = textVersionHash -> return diagnostics
                | _ ->
                    let! sourceText = document.GetTextAsync()
                    let checker = getChecker document
                    let! _, _, checkResults = checker.ParseAndCheckDocument(document, options, sourceText = sourceText, allowStaleResults = true)
                    let! symbolUses = checkResults.GetAllUsesOfAllSymbolsInFile() |> liftAsync
                    let mutable result = ResizeArray()
                    let symbolUses =
                        symbolUses
                        |> Array.Parallel.map (fun symbolUse ->
                            let lineStr = sourceText.Lines.[Line.toZ symbolUse.RangeAlternate.StartLine].ToString()
                            // for `System.DateTime.Now` it returns ([|"System"; "DateTime"|], "Now")
                            let plid, name = QuickParse.GetPartialLongNameEx(lineStr, symbolUse.RangeAlternate.EndColumn - 1)
                            // `symbolUse.RangeAlternate.Start` does not point to the start of plid, it points to start of `name`,
                            // so we have to calculate plid's start ourselves.
                            let plidStartCol = symbolUse.RangeAlternate.EndColumn - name.Length - (getPlidLength plid)
                            symbolUse, plid, plidStartCol, name)
                        |> Array.filter (fun (_, plid, _, _) -> not (List.isEmpty plid))
                        |> Array.groupBy (fun (symbolUse, _, plidStartCol, _) -> symbolUse.RangeAlternate.StartLine, plidStartCol)
                        |> Array.map (fun (_, xs) -> xs |> Array.maxBy (fun (symbolUse, _, _, _) -> symbolUse.RangeAlternate.EndColumn))
                    
                    for symbolUse, plid, plidStartCol, name in symbolUses do
                        if not symbolUse.IsFromDefinition then
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
                                loop (List.rev plid) []
                               
                            let! necessaryPlid = getNecessaryPlid plid |> liftAsync
                                
                            match necessaryPlid with
                            | necessaryPlid when necessaryPlid = plid -> ()
                            | necessaryPlid ->
                                let r = symbolUse.RangeAlternate
                                let necessaryPlidStartCol = r.EndColumn - name.Length - (getPlidLength necessaryPlid)
                    
                                let unnecessaryRange = 
                                    Range.mkRange r.FileName (Range.mkPos r.StartLine plidStartCol) (Range.mkPos r.EndLine necessaryPlidStartCol)
                                
                                let relativeName = (String.concat "." plid) + "." + name
                                result.Add(
                                    Diagnostic.Create(
                                       Descriptor,
                                       CommonRoslynHelpers.RangeToLocation(unnecessaryRange, sourceText, document.FilePath),
                                       properties = (dict [SimplifyNameDiagnosticAnalyzer.LongIdentPropertyKey, relativeName]).ToImmutableDictionary()))
                    
                    let diagnostics = result.ToImmutableArray()
                    cache.Remove(document.Id) |> ignore
                    cache.Add(document.Id, (textVersionHash, diagnostics))
                    return diagnostics
            finally guard.Release() |> ignore
        } 
        |> Async.map (Option.defaultValue ImmutableArray.Empty)
        |> CommonRoslynHelpers.StartAsyncAsTask cancellationToken

    interface IBuiltInAnalyzer with
        member __.OpenFileOnly _ = true
        member __.GetAnalyzerCategory() = DiagnosticAnalyzerCategory.SemanticDocumentAnalysis