// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Generic
open System.Collections.Immutable
open System.Diagnostics
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Diagnostics
open Microsoft.FSharp.Compiler.SourceCodeServices

[<DiagnosticAnalyzer(FSharpConstants.FSharpLanguageName)>]
type internal UnusedDeclarationsAnalyzer() =
    inherit DocumentDiagnosticAnalyzer()
    
    static let userOpName = "UnusedDeclarationsAnalyzer"
    let getProjectInfoManager (document: Document) = document.Project.Solution.Workspace.Services.GetService<FSharpCheckerWorkspaceService>().FSharpProjectOptionsManager
    let getChecker (document: Document) = document.Project.Solution.Workspace.Services.GetService<FSharpCheckerWorkspaceService>().Checker
    let [<Literal>] DescriptorId = "FS1182"
    
    let Descriptor = 
        DiagnosticDescriptor(
            id = DescriptorId,
            title = SR.TheValueIsUnused(),
            messageFormat = SR.TheValueIsUnused(),
            category = DiagnosticCategory.Style,
            defaultSeverity = DiagnosticSeverity.Hidden,
            isEnabledByDefault = true,
            customTags = DiagnosticCustomTags.Unnecessary)
    
    let isPotentiallyUnusedDeclaration (symbol: FSharpSymbol) : bool =
        match symbol with
        // Determining that a record, DU or module is used anywhere requires inspecting all their enclosed entities (fields, cases and func / vals)
        // for usages, which is too expensive to do. Hence we never gray them out.
        | :? FSharpEntity as e when e.IsFSharpRecord || e.IsFSharpUnion || e.IsInterface || e.IsFSharpModule || e.IsClass || e.IsNamespace -> false
        // FCS returns inconsistent results for override members; we're skipping these symbols.
        | :? FSharpMemberOrFunctionOrValue as f when 
                f.IsOverrideOrExplicitInterfaceImplementation ||
                f.IsBaseValue ||
                f.IsConstructor -> false
        // Usage of DU case parameters does not give any meaningful feedback; we never gray them out.
        | :? FSharpParameter -> false
        | _ -> true

    let getUnusedDeclarationRanges (symbolsUses: FSharpSymbolUse[]) (isScript: bool) =
        let definitions =
            symbolsUses
            |> Array.filter (fun su -> 
                su.IsFromDefinition && 
                su.Symbol.DeclarationLocation.IsSome && 
                (isScript || su.IsPrivateToFile) && 
                not (su.Symbol.DisplayName.StartsWith "_") &&
                isPotentiallyUnusedDeclaration su.Symbol)

        let usages =
            let usages = 
                symbolsUses
                |> Array.filter (fun su -> not su.IsFromDefinition)
                |> Array.choose (fun su -> su.Symbol.DeclarationLocation)
            HashSet(usages)

        let unusedRanges =
            definitions
            |> Array.map (fun defSu -> defSu, usages.Contains defSu.Symbol.DeclarationLocation.Value)
            |> Array.groupBy (fun (defSu, _) -> defSu.RangeAlternate)
            |> Array.filter (fun (_, defSus) -> defSus |> Array.forall (fun (_, isUsed) -> not isUsed))
            |> Array.map (fun (m, _) -> m)

        //#if DEBUG
        //let formatRange (x: Microsoft.FSharp.Compiler.Range.range) = sprintf "(%d, %d) - (%d, %d)" x.StartLine x.StartColumn x.EndLine x.EndColumn

        //symbolsUses
        //|> Array.map (fun su -> sprintf "%s, %s, is definition = %b, Symbol (def range = %A)" 
        //                                (formatRange su.RangeAlternate) su.Symbol.DisplayName su.IsFromDefinition
        //                                (su.Symbol.DeclarationLocation |> Option.map formatRange))
        //|> Logging.Logging.logInfof "SymbolUses:\n%+A"
        //
        //definitions
        //|> Seq.map (fun su -> sprintf "su range = %s, symbol range = %A, symbol name = %s" 
        //                              (formatRange su.RangeAlternate) (su.Symbol.DeclarationLocation |> Option.map formatRange) su.Symbol.DisplayName)
        //|> Logging.Logging.logInfof "Definitions:\n%A"
        //
        //usages
        //|> Seq.map formatRange
        //|> Seq.toArray
        //|> Logging.Logging.logInfof "Used ranges:\n%A"
        //
        //unusedRanges
        //|> Array.map formatRange
        //|> Logging.Logging.logInfof "Unused ranges: %A"
        //#endif
        unusedRanges

    override __.SupportedDiagnostics = ImmutableArray.Create Descriptor
    
    override __.AnalyzeSyntaxAsync(_, _) = Task.FromResult ImmutableArray<Diagnostic>.Empty

    override __.AnalyzeSemanticsAsync(document, cancellationToken) =
        asyncMaybe {
            do Trace.TraceInformation("{0:n3} (start) UnusedDeclarationsAnalyzer", DateTime.Now.TimeOfDay.TotalSeconds)
            do! Async.Sleep DefaultTuning.UnusedDeclarationsAnalyzerInitialDelay |> liftAsync // be less intrusive, give other work priority most of the time
            match getProjectInfoManager(document).TryGetOptionsForEditingDocumentOrProject(document) with
            | Some (_parsingOptions, projectOptions) ->
                let! sourceText = document.GetTextAsync()
                let checker = getChecker document
                let! _, _, checkResults = checker.ParseAndCheckDocument(document, projectOptions, sourceText = sourceText, allowStaleResults = true, userOpName = userOpName)
                let! allSymbolUsesInFile = checkResults.GetAllUsesOfAllSymbolsInFile() |> liftAsync
                let unusedRanges = getUnusedDeclarationRanges allSymbolUsesInFile (isScriptFile document.FilePath)
                return
                    unusedRanges
                    |> Seq.map (fun m -> Diagnostic.Create(Descriptor, RoslynHelpers.RangeToLocation(m, sourceText, document.FilePath)))
                    |> Seq.toImmutableArray
            | None -> return ImmutableArray.Empty
        }
        |> Async.map (Option.defaultValue ImmutableArray.Empty)
        |> RoslynHelpers.StartAsyncAsTask cancellationToken

    interface IBuiltInAnalyzer with
        member __.OpenFileOnly _ = true
        member __.GetAnalyzerCategory() = DiagnosticAnalyzerCategory.SemanticDocumentAnalysis