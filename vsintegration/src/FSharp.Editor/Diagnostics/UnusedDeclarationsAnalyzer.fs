// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Immutable
open System.Threading.Tasks
open System.Collections.Generic

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Diagnostics
open Microsoft.FSharp.Compiler.SourceCodeServices

[<DiagnosticAnalyzer(FSharpConstants.FSharpLanguageName)>]
type internal UnusedDeclarationsAnalyzer() =
    inherit DocumentDiagnosticAnalyzer()
    
    let getProjectInfoManager (document: Document) = document.Project.Solution.Workspace.Services.GetService<FSharpCheckerWorkspaceService>().ProjectInfoManager
    let getChecker (document: Document) = document.Project.Solution.Workspace.Services.GetService<FSharpCheckerWorkspaceService>().Checker
    let [<Literal>] DescriptorId = "FS1182"
    
    let Descriptor = 
        DiagnosticDescriptor(
            id = DescriptorId,
            title = SR.TheValueIsUnused.Value,
            messageFormat = SR.TheValueIsUnused.Value,
            category = DiagnosticCategory.Style,
            defaultSeverity = DiagnosticSeverity.Hidden,
            isEnabledByDefault = true,
            customTags = DiagnosticCustomTags.Unnecessary)
    
    let symbolUseComparer =
        { new IEqualityComparer<FSharpSymbolUse> with
            member __.Equals (x, y) = x.Symbol.IsEffectivelySameAs y.Symbol
            member __.GetHashCode x = x.Symbol.GetHashCode() }

    let countSymbolsUses (symbolsUses: FSharpSymbolUse[]) =
        let result = Dictionary<FSharpSymbolUse, int>(symbolUseComparer)
    
        for symbolUse in symbolsUses do
            match result.TryGetValue symbolUse with
            | true, count -> result.[symbolUse] <- count + 1
            | _ -> result.[symbolUse] <- 1
        result

    let getSingleDeclarations (symbolsUses: FSharpSymbolUse[]) (isScript: bool) =
        let declarations =
            countSymbolsUses symbolsUses
            |> Seq.choose (fun (KeyValue(symbolUse, count)) ->
                match symbolUse.Symbol with
                // Determining that a record, DU or module is used anywhere requires inspecting all their enclosed entities (fields, cases and func / vals)
                // for usages, which is too expensive to do. Hence we never gray them out.
                | :? FSharpEntity as e when e.IsFSharpRecord || e.IsFSharpUnion || e.IsInterface || e.IsFSharpModule || e.IsClass -> None
                // FCS returns inconsistent results for override members; we're skipping these symbols.
                | :? FSharpMemberOrFunctionOrValue as f when 
                        f.IsOverrideOrExplicitInterfaceImplementation ||
                        f.IsConstructorThisValue ||
                        f.IsBaseValue ||
                        f.IsConstructor -> None
                // Usage of DU case parameters does not give any meaningful feedback; we never gray them out.
                | :? FSharpParameter when symbolUse.IsFromDefinition -> None
                | _ when count = 1 && symbolUse.IsFromDefinition && (isScript || symbolUse.IsPrivateToFile) -> Some symbolUse
                | _ -> None)
        HashSet(declarations, symbolUseComparer)

    override __.SupportedDiagnostics = ImmutableArray.Create Descriptor
    
    override this.AnalyzeSyntaxAsync(_, _) = Task.FromResult ImmutableArray<Diagnostic>.Empty

    override this.AnalyzeSemanticsAsync(document, cancellationToken) =
        asyncMaybe {
            match getProjectInfoManager(document).TryGetOptionsForEditingDocumentOrProject(document) with
            | Some options ->
                let! sourceText = document.GetTextAsync()
                let checker = getChecker document
                let! _, _, checkResults = checker.ParseAndCheckDocument(document, options, sourceText = sourceText, allowStaleResults = true)
                let! allSymbolUsesInFile = checkResults.GetAllUsesOfAllSymbolsInFile() |> liftAsync
                let unusedDeclarations = getSingleDeclarations allSymbolUsesInFile (isScriptFile document.FilePath)
                return 
                    unusedDeclarations
                    |> Seq.filter (fun symbolUse -> not (symbolUse.Symbol.DisplayName.StartsWith "_"))
                    |> Seq.map (fun symbolUse -> Diagnostic.Create(Descriptor, RoslynHelpers.RangeToLocation(symbolUse.RangeAlternate, sourceText, document.FilePath)))
                    |> Seq.toImmutableArray
            | None -> return ImmutableArray.Empty
        }
        |> Async.map (Option.defaultValue ImmutableArray.Empty)
        |> RoslynHelpers.StartAsyncAsTask cancellationToken

    interface IBuiltInAnalyzer with
        member __.OpenFileOnly _ = true
        member __.GetAnalyzerCategory() = DiagnosticAnalyzerCategory.SemanticDocumentAnalysis