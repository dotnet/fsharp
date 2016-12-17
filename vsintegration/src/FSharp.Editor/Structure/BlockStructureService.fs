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
open Microsoft.CodeAnalysis.Structure

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Classification
open Microsoft.VisualStudio.Text.Tagging
open Microsoft.VisualStudio.Text.Formatting
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Parser
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.SourceCodeServices.Structure
open System.Windows.Documents

[<ExportLanguageServiceFactory(typeof<BlockStructureService>, FSharpCommonConstants.FSharpLanguageName); Shared>]
type internal FSharpBlockStructureServiceFactory [<ImportingConstructor>](checkerProvider: FSharpCheckerProvider, projectInfoManager: ProjectInfoManager) =
    interface ILanguageServiceFactory with
        member __.CreateLanguageService(_languageServices) =
            upcast FSharpBlockStructureService(checkerProvider.Checker, projectInfoManager)
 
type internal FSharpBlockStructureService(checker: FSharpChecker, projectInfoManager: ProjectInfoManager) =
    inherit BlockStructureService()
    let scopeToBlockType = function
        | Scope.Open -> BlockTypes.Imports
        | Scope.Namespace
        | Scope.Module -> BlockTypes.Namespace 
        | Scope.Record
        | Scope.Interface
        | Scope.TypeExtension
        | Scope.SimpleType
        | Scope.RecordDefn
        | Scope.UnionDefn
        | Scope.Type -> BlockTypes.Type
        | Scope.Member -> BlockTypes.Member
        | Scope.LetOrUse
        | Scope.Match
        | Scope.MatchLambda
        | Scope.IfThenElse-> BlockTypes.Conditional
        | Scope.CompExpr
        | Scope.TryFinally
        | Scope.ObjExpr
        | Scope.ArrayOrList
        | Scope.CompExprInternal
        | Scope.Quote
        | Scope.SpecialFunc
        | Scope.Lambda
        | Scope.LetOrUseBang
        | Scope.YieldOrReturn
        | Scope.YieldOrReturnBang
        | Scope.TryWith -> BlockTypes.Expression
        | Scope.Do -> BlockTypes.Statement
        | Scope.While
        | Scope.For -> BlockTypes.Loop
        | Scope.HashDirective -> BlockTypes.PreprocessorRegion
        | Scope.Comment
        | Scope.XmlDocComment -> BlockTypes.Comment
        
    override __.Language = FSharpCommonConstants.FSharpLanguageName
 
    override __.GetBlockStructureAsync(document, cancellationToken) : Task<BlockStructure> =
        async {
            match projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document) with 
            | Some options ->
                let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                let! fileParseResults = checker.ParseFileInProject(document.FilePath, sourceText.ToString(), options)
                match fileParseResults.ParseTree with
                | Some parsedInput ->
                    let blockSpans =
                        Structure.getOutliningRanges (sourceText.Lines |> Seq.map (fun x -> x.ToString()) |> Seq.toArray) parsedInput
                        |> Seq.distinctBy (fun x -> x.Range.StartLine)
                        |> Seq.choose (fun range -> 
                            CommonRoslynHelpers.TryFSharpRangeToTextSpan(sourceText, range.Range)
                            |> Option.map (fun span -> BlockSpan(scopeToBlockType range.Scope, true, span)))
                    return BlockStructure(blockSpans.ToImmutableArray())
                | None -> return BlockStructure(ImmutableArray<_>.Empty)
            | None -> return BlockStructure(ImmutableArray<_>.Empty)
        } |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)