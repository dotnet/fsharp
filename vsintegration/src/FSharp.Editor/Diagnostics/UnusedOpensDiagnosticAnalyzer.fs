// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable
open System.Collections.Generic
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Diagnostics
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.VisualStudio.FSharp.LanguageService

module private UnusedOpens =
    open Microsoft.CodeAnalysis.Text

    let visitModulesAndNamespaces modulesOrNss =
        [ for moduleOrNs in modulesOrNss do
            let SynModuleOrNamespace(decls = decls) = moduleOrNs

            for decl in decls do
                match decl with
                | SynModuleDecl.Open(longIdentWithDots, range) -> 
                    yield (longIdentWithDots.Lid |> List.map(fun l -> l.idText) |> String.concat "."), range
                | _ -> () ]

    let getOpenStatements = function
        | ParsedInput.ImplFile implFile ->
            let (ParsedImplFileInput(_, _, _, _, _, modules, _)) = implFile in visitModulesAndNamespaces modules
        | _ -> []

    let getAutoOpenAccessPath (ent:FSharpEntity) =
        // Some.Namespace+AutoOpenedModule+Entity

        // HACK: I can't see a way to get the EnclosingEntity of an Entity
        // Some.Namespace + Some.Namespace.AutoOpenedModule are both valid
        ent.TryFullName |> Option.bind(fun _ ->
            if (not ent.IsNamespace) && ent.QualifiedName.Contains "+" then 
                Some ent.QualifiedName.[0..ent.QualifiedName.IndexOf "+" - 1]
            else
                None)

    let entityNamespace (entOpt:FSharpEntity option) =
        match entOpt with
        | Some ent ->
            if ent.IsFSharpModule then
                [Some ent.QualifiedName; Some ent.LogicalName; Some ent.AccessPath]
            else
                [ent.Namespace; Some ent.AccessPath; getAutoOpenAccessPath ent]
        | None -> []

    let symbolIsFullyQualified (sourceText: SourceText) (sym: FSharpSymbolUse) (fullName: string option) =
        match fullName with
        | Some fullName ->
            sourceText.ToString(CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, sym.RangeAlternate)) = fullName
        | None -> true

    let getUnusedOpens (sourceText: SourceText) (parsedInput: ParsedInput) (symbolUses: FSharpSymbolUse[]) =

        let getPartNamespace (sym:FSharpSymbolUse) (fullName:string option) =
            // given a symbol range such as `Text.ISegment` and a full name
            // of `MonoDevelop.Core.Text.ISegment`, return `MonoDevelop.Core`
            fullName |> Option.bind(fun fullName ->
                let length = sym.RangeAlternate.EndColumn - sym.RangeAlternate.StartColumn
                let lengthDiff = fullName.Length - length - 2
                Some fullName.[0..lengthDiff])

        let getPossibleNamespaces (sym: FSharpSymbolUse) =
            let isQualified = symbolIsFullyQualified sourceText sym
            match sym with
            | SymbolUse.Entity ent when not (isQualified ent.TryFullName) ->
                getPartNamespace sym ent.TryFullName :: entityNamespace (Some ent)
            | SymbolUse.Field f when not (isQualified (Some f.FullName)) -> 
                getPartNamespace sym (Some f.FullName) :: entityNamespace (Some f.DeclaringEntity)
            | SymbolUse.MemberFunctionOrValue mfv when not (isQualified (Some mfv.FullName)) -> 
                getPartNamespace sym (Some mfv.FullName) :: entityNamespace (Some mfv.EnclosingEntity)
            | SymbolUse.Operator op when not (isQualified (Some op.FullName)) ->
                getPartNamespace sym (Some op.FullName) :: entityNamespace op.EnclosingEntitySafe
            | SymbolUse.ActivePattern ap when not (isQualified (Some ap.FullName)) ->
                getPartNamespace sym (Some ap.FullName) :: entityNamespace ap.EnclosingEntitySafe
            | SymbolUse.ActivePatternCase apc when not (isQualified (Some apc.FullName)) ->
                getPartNamespace sym (Some apc.FullName) :: entityNamespace apc.Group.EnclosingEntity
            | SymbolUse.UnionCase uc when not (isQualified (Some uc.FullName)) ->
                getPartNamespace sym (Some uc.FullName) :: entityNamespace (Some uc.ReturnType.TypeDefinition)
            | SymbolUse.Parameter p when not (isQualified (Some p.FullName)) ->
                getPartNamespace sym (Some p.FullName) :: entityNamespace (Some p.Type.TypeDefinition)
            | _ -> [None]

        let namespacesInUse =
            symbolUses
            |> Seq.filter (fun (s: FSharpSymbolUse) -> not s.IsFromDefinition)
            |> Seq.collect getPossibleNamespaces
            |> Seq.choose id
            |> Set.ofSeq

        let filter list: (string * Range.range) list =
            let rec filterInner acc list (seenNamespaces: Set<string>) = 
                let notUsed namespc =
                    not (namespacesInUse.Contains namespc) || seenNamespaces.Contains namespc

                match list with 
                | (namespc, range)::xs when notUsed namespc -> 
                    filterInner ((namespc, range)::acc) xs (seenNamespaces.Add namespc)
                | (namespc, _)::xs ->
                    filterInner acc xs (seenNamespaces.Add namespc)
                | [] -> acc |> List.rev
            filterInner [] list Set.empty

        parsedInput
        |> getOpenStatements
        |> filter
        |> List.map snd

[<DiagnosticAnalyzer(FSharpCommonConstants.FSharpLanguageName)>]
type internal UnusedOpensDiagnosticAnalyzer() =
    inherit DocumentDiagnosticAnalyzer()
    
    let getProjectInfoManager (document: Document) = document.Project.Solution.Workspace.Services.GetService<FSharpCheckerWorkspaceService>().ProjectInfoManager
    let getChecker (document: Document) = document.Project.Solution.Workspace.Services.GetService<FSharpCheckerWorkspaceService>().Checker

    static let Descriptor = 
        DiagnosticDescriptor(
            IDEDiagnosticIds.RemoveUnnecessaryImportsDiagnosticId, 
            SR.RemoveUnusedOpens.Value, 
            SR.UnusedOpens.Value, 
            DiagnosticCategory.Style, 
            DiagnosticSeverity.Hidden, 
            true, 
            "", 
            "", 
            DiagnosticCustomTags.Unnecessary)

    override __.SupportedDiagnostics = ImmutableArray.Create Descriptor
    override this.AnalyzeSyntaxAsync(_, _) = Task.FromResult ImmutableArray<Diagnostic>.Empty

    override this.AnalyzeSemanticsAsync(document: Document, cancellationToken: CancellationToken) =
        asyncMaybe {
            let! options = getProjectInfoManager(document).TryGetOptionsForEditingDocumentOrProject(document)
            let! sourceText = document.GetTextAsync()
            let checker = getChecker document
            let! _, parsedInput, checkResults = checker.ParseAndCheckDocument(document, options, sourceText = sourceText, allowStaleResults = true)
            let! symbolUses = checkResults.GetAllUsesOfAllSymbolsInFile() |> liftAsync
            let unusedOpens = UnusedOpens.getUnusedOpens sourceText parsedInput symbolUses
            
            return 
                (unusedOpens
                 |> List.map (fun m ->
                      Diagnostic.Create(
                         Descriptor,
                         CommonRoslynHelpers.RangeToLocation(m, sourceText, document.FilePath)))
                ).ToImmutableArray()
        } 
        |> Async.map (Option.defaultValue ImmutableArray.Empty)
        |> CommonRoslynHelpers.StartAsyncAsTask cancellationToken

    interface IBuiltInAnalyzer with
        member __.OpenFileOnly _ = true
        member __.GetAnalyzerCategory() = DiagnosticAnalyzerCategory.SemanticDocumentAnalysis