// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Concurrent
open System.Collections.Generic
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks
open System.Linq
open System.Runtime.CompilerServices
open System.Windows
open System.Windows.Controls
open System.Windows.Media

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Completion
open Microsoft.CodeAnalysis.Classification
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Editor.Shared.Utilities
open Microsoft.CodeAnalysis.Formatting
open Microsoft.CodeAnalysis.Host
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Options
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.CodeFixes
open Microsoft.CodeAnalysis.CodeActions

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Classification
open Microsoft.VisualStudio.Text.Tagging
open Microsoft.VisualStudio.Text.Formatting
open Microsoft.VisualStudio.Shell.Interop

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Parser
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.SourceCodeServices.Structure
open System.Windows.Documents

[<ExportCodeFixProvider(FSharpCommonConstants.FSharpLanguageName, Name = "AddOpen"); Shared>]
type internal FSharpAddOpenCodeFixProvider
    [<ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider, 
        projectInfoManager: ProjectInfoManager,
        assemblyContentProvider: AssemblyContentProvider
    ) =
    inherit CodeFixProvider()

    let checker = checkerProvider.Checker

    override __.FixableDiagnosticIds = ["FS0039"].ToImmutableArray()

    override __.RegisterCodeFixesAsync context : Task =
        async {
            match projectInfoManager.TryGetOptionsForEditingDocumentOrProject context.Document with 
            | Some options ->
                let! sourceText = context.Document.GetTextAsync(context.CancellationToken) |> Async.AwaitTask
                let! textVersion = context.Document.GetTextVersionAsync(context.CancellationToken) |> Async.AwaitTask
                let! parseResults, checkFileAnswer = checker.ParseAndCheckFileInProject(context.Document.FilePath, textVersion.GetHashCode(), sourceText.ToString(), options)
                match parseResults.ParseTree, checkFileAnswer with
                | None, _
                | _, FSharpCheckFileAnswer.Aborted -> ()
                | Some parsedInput, FSharpCheckFileAnswer.Succeeded(checkFileResults) ->
                    let entities = assemblyContentProvider.GetAllEntitiesInProjectAndReferencedAssemblies checkFileResults
                    match ParsedInput.getEntityKind parsedInput pos with // convert context.Span to Range !
                    | None -> ()
                    | Some entityKind ->
                        let isAttribute = entityKind = EntityKind.Attribute
                        
                        let entities =
                            entities |> List.filter (fun e ->
                                match entityKind, e.Kind with
                                | EntityKind.Attribute, EntityKind.Attribute 
                                | EntityKind.Type, (EntityKind.Type | EntityKind.Attribute)
                                | EntityKind.FunctionOrValue _, _ -> true 
                                | EntityKind.Attribute, _
                                | _, EntityKind.Module _
                                | EntityKind.Module _, _
                                | EntityKind.Type, _ -> false)
                        
                        let entities = 
                            entities
                            |> List.map (fun e -> 
                                 [ yield e.TopRequireQualifiedAccessParent, e.AutoOpenParent, e.Namespace, e.CleanedIdents
                                   if isAttribute then
                                       let lastIdent = e.CleanedIdents.[e.CleanedIdents.Length - 1]
                                       if e.Kind = EntityKind.Attribute && lastIdent.EndsWith "Attribute" then
                                           yield 
                                               e.TopRequireQualifiedAccessParent, 
                                               e.AutoOpenParent,
                                               e.Namespace,
                                               e.CleanedIdents 
                                               |> Array.replace (e.CleanedIdents.Length - 1) (lastIdent.Substring(0, lastIdent.Length - 9)) ])
                            |> List.concat
                        
                        let! idents = UntypedAstUtils.getLongIdentAt parseTree (Range.mkPos pos.Line sym.RightColumn)
                        let createEntity = ParsedInput.tryFindInsertionContext pos.Line parseTree idents
                        return entities |> Seq.map createEntity |> Seq.concat |> Seq.toList |> getSuggestions newWord 
            | None -> ()

            context.RegisterCodeFix(
                CodeAction.Create(
                    "open System.Collections.Generic",
                    fun (cancellationToken: CancellationToken) -> 
                        async {
                            return context.Document
                        } |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)
                    ), context.Diagnostics)
        } |> CommonRoslynHelpers.StartAsyncUnitAsTask(context.CancellationToken)
 