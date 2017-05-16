// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Diagnostics
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Ast
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices
open Symbols


module private UnusedOpens =
    

    let rec visitSynModuleOrNamespaceDecls (parent: Ast.LongIdent) decls : (Set<string> * range) list =
        [ for decl in decls do
            match decl with
            | SynModuleDecl.Open(LongIdentWithDots.LongIdentWithDots(id = longId), range) ->
                yield
                    set [ yield (longId |> List.map(fun l -> l.idText) |> String.concat ".")
                          // `open N.M` can open N.M module from parent module as well, if it's non empty
                          if not (List.isEmpty parent) then
                            yield (parent @ longId |> List.map(fun l -> l.idText) |> String.concat ".") ], range
            | SynModuleDecl.NestedModule(SynComponentInfo.ComponentInfo(longId = longId),_, decls,_,_) ->
                yield! visitSynModuleOrNamespaceDecls longId decls
            | _ -> () ]

    let getOpenStatements = function
        | ParsedInput.ImplFile (ParsedImplFileInput(modules = modules)) ->
            [ for md in modules do
                let SynModuleOrNamespace(longId = longId; decls = decls) = md
                yield! visitSynModuleOrNamespaceDecls longId decls ]
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

    let entityNamespace (entOpt: FSharpEntity option) =
        match entOpt with
        | Some ent ->
            if ent.IsFSharpModule then
                [ yield Some ent.QualifiedName
                  yield Some ent.LogicalName
                  yield Some ent.AccessPath
                  yield Some ent.FullName
                  yield Some ent.DisplayName
                  yield ent.TryGetFullDisplayName()
                  if ent.HasFSharpModuleSuffix then
                    yield Some (ent.AccessPath + "." + ent.DisplayName)]
            else
                [ yield ent.Namespace
                  yield Some ent.AccessPath
                  yield getAutoOpenAccessPath ent
                  for path in ent.AllCompilationPaths do
                    yield Some path 
                ]
        | None -> []

    let symbolIsFullyQualified (sourceText: SourceText) (sym: FSharpSymbolUse) (fullName: string) =
        match RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, sym.RangeAlternate) with
        | Some span // check that the symbol hasn't provided an invalid span
            when sourceText.Length < span.Start 
              || sourceText.Length < span.End -> false
        | Some span -> sourceText.ToString span = fullName
        | None -> false

    let getUnusedOpens (sourceText: SourceText) (parsedInput: ParsedInput) (symbolUses: FSharpSymbolUse[]) =

        let getPartNamespace (symbolUse: FSharpSymbolUse) (fullName: string) =
            // given a symbol range such as `Text.ISegment` and a full name of `MonoDevelop.Core.Text.ISegment`, return `MonoDevelop.Core`
            let length = symbolUse.RangeAlternate.EndColumn - symbolUse.RangeAlternate.StartColumn
            let lengthDiff = fullName.Length - length - 2
            if lengthDiff <= 0 || lengthDiff > fullName.Length - 1 then None
            else Some fullName.[0..lengthDiff]

        let getPossibleNamespaces (symbolUse: FSharpSymbolUse) : string list =
            let isQualified = symbolIsFullyQualified sourceText symbolUse
            maybe {
                let! fullNames, declaringEntity =
                    match symbolUse with
                    | SymbolUse.Entity (ent, cleanFullNames) when not (cleanFullNames |> List.exists isQualified) ->
                        Some (cleanFullNames, Some ent)
                    | SymbolUse.Field f when not (isQualified f.FullName) -> 
                        Some ([f.FullName], Some f.DeclaringEntity)
                    | SymbolUse.MemberFunctionOrValue mfv when not (isQualified mfv.FullName) -> 
                        Some ([mfv.FullName], mfv.EnclosingEntitySafe)
                    | SymbolUse.Operator op when not (isQualified op.FullName) ->
                        Some ([op.FullName], op.EnclosingEntitySafe)
                    | SymbolUse.ActivePattern ap when not (isQualified ap.FullName) ->
                        Some ([ap.FullName], ap.EnclosingEntitySafe)
                    | SymbolUse.ActivePatternCase apc when not (isQualified apc.FullName) ->
                        Some ([apc.FullName], apc.Group.EnclosingEntity)
                    | SymbolUse.UnionCase uc when not (isQualified uc.FullName) ->
                        Some ([uc.FullName], Some uc.ReturnType.TypeDefinition)
                    | SymbolUse.Parameter p when not (isQualified p.FullName) && p.Type.HasTypeDefinition ->
                        Some ([p.FullName], Some p.Type.TypeDefinition)
                    | _ -> None

                return
                    [ for name in fullNames do
                        yield getPartNamespace symbolUse name
                      yield! entityNamespace declaringEntity ]
            } |> Option.toList |> List.concat |> List.choose id

        let namespacesInUse =
            symbolUses
            |> Seq.filter (fun (s: FSharpSymbolUse) -> not s.IsFromDefinition)
            |> Seq.collect getPossibleNamespaces
            |> Set.ofSeq

        let filter list: (Set<string> * range) list =
            let rec filterInner acc list (seenNamespaces: Set<string>) = 
                let notUsed ns = not (namespacesInUse.Contains ns) || seenNamespaces.Contains ns
                match list with 
                | (ns, range) :: xs when ns |> Set.forall notUsed -> 
                    filterInner ((ns, range) :: acc) xs (seenNamespaces |> Set.union ns)
                | (ns, _) :: xs ->
                    filterInner acc xs (seenNamespaces |> Set.union ns)
                | [] -> List.rev acc
            filterInner [] list Set.empty

        let openStatements = getOpenStatements parsedInput
        openStatements |> filter |> List.map snd

[<DiagnosticAnalyzer(FSharpConstants.FSharpLanguageName)>]
type internal UnusedOpensDiagnosticAnalyzer() =
    inherit DocumentDiagnosticAnalyzer()
    
    let getProjectInfoManager (document: Document) = document.Project.Solution.Workspace.Services.GetService<FSharpCheckerWorkspaceService>().ProjectInfoManager
    let getChecker (document: Document) = document.Project.Solution.Workspace.Services.GetService<FSharpCheckerWorkspaceService>().Checker

    static let Descriptor = 
        DiagnosticDescriptor(
            id = IDEDiagnosticIds.RemoveUnnecessaryImportsDiagnosticId, 
            title = SR.RemoveUnusedOpens.Value, 
            messageFormat = SR.UnusedOpens.Value, 
            category = DiagnosticCategory.Style, 
            defaultSeverity = DiagnosticSeverity.Hidden, 
            isEnabledByDefault = true, 
            customTags = DiagnosticCustomTags.Unnecessary)

    override __.SupportedDiagnostics = ImmutableArray.Create Descriptor
    override this.AnalyzeSyntaxAsync(_, _) = Task.FromResult ImmutableArray<Diagnostic>.Empty

    static member GetUnusedOpenRanges(document: Document, options, checker: FSharpChecker) =
        asyncMaybe {
            do! Option.guard Settings.CodeFixes.UnusedOpens
            let! sourceText = document.GetTextAsync()
            let! _, parsedInput, checkResults = checker.ParseAndCheckDocument(document, options, sourceText = sourceText, allowStaleResults = true)
            let! symbolUses = checkResults.GetAllUsesOfAllSymbolsInFile() |> liftAsync
            return UnusedOpens.getUnusedOpens sourceText parsedInput symbolUses
        } 

    override this.AnalyzeSemanticsAsync(document: Document, cancellationToken: CancellationToken) =
        asyncMaybe {
            let! options = getProjectInfoManager(document).TryGetOptionsForEditingDocumentOrProject(document)
            let! sourceText = document.GetTextAsync()
            let checker = getChecker document
            let! unusedOpens = UnusedOpensDiagnosticAnalyzer.GetUnusedOpenRanges(document, options, checker)
            
            return 
                unusedOpens
                |> List.map (fun m ->
                      Diagnostic.Create(
                         Descriptor,
                         RoslynHelpers.RangeToLocation(m, sourceText, document.FilePath)))
                |> Seq.toImmutableArray
        } 
        |> Async.map (Option.defaultValue ImmutableArray.Empty)
        |> RoslynHelpers.StartAsyncAsTask cancellationToken

    interface IBuiltInAnalyzer with
        member __.OpenFileOnly _ = true
        member __.GetAnalyzerCategory() = DiagnosticAnalyzerCategory.SemanticDocumentAnalysis