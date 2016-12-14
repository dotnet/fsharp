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
        | Scope.Tuple
        | Scope.Attribute
        | Scope.Interface
        | Scope.TypeExtension
        | Scope.UnionCase
        | Scope.EnumCase
        | Scope.SimpleType
        | Scope.RecordDefn
        | Scope.UnionDefn
        | Scope.Type -> BlockTypes.Type
        | Scope.Member -> BlockTypes.Member
        | Scope.LetOrUse
        | Scope.Match
        | Scope.IfThenElse
        | Scope.ThenInIfThenElse
        | Scope.ElseInIfThenElse
        | Scope.MatchLambda -> BlockTypes.Conditional
        | Scope.CompExpr
        | Scope.TryInTryWith
        | Scope.WithInTryWith
        | Scope.TryFinally
        | Scope.TryInTryFinally
        | Scope.FinallyInTryFinally
        | Scope.ObjExpr
        | Scope.ArrayOrList
        | Scope.CompExprInternal
        | Scope.Quote
        | Scope.SpecialFunc
        | Scope.MatchClause
        | Scope.Lambda
        | Scope.LetOrUseBang
        | Scope.YieldOrReturn
        | Scope.YieldOrReturnBang
        | Scope.RecordField
        | Scope.TryWith -> BlockTypes.Expression
        | Scope.Do -> BlockTypes.Statement
        | Scope.While
        | Scope.For -> BlockTypes.Loop
        | Scope.HashDirective -> BlockTypes.PreprocessorRegion
        | Scope.Comment
        | Scope.XmlDocComment -> BlockTypes.Comment
        | _ -> BlockTypes.Nonstructural
        
    override __.Language = FSharpCommonConstants.FSharpLanguageName
 
    override __.GetBlockStructureAsync(document, cancellationToken) : Task<BlockStructure> =
        async {
            match projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document) with 
            | Some options ->
                let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                let! fileParseResults = checker.ParseFileInProject(document.FilePath, sourceText.ToString(), options)
                match fileParseResults.ParseTree with
                | Some parsedInput ->
                    let ranges = Structure.getOutliningRanges (sourceText.Lines |> Seq.map (fun x -> x.ToString()) |> Seq.toArray) parsedInput
                    let blockSpans =
                        ranges
                        |> Seq.map (fun range -> 
                            BlockSpan(scopeToBlockType range.Scope, true, CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText, range.Range)))
                    return BlockStructure(blockSpans.ToImmutableArray())
                | None -> return BlockStructure(ImmutableArray<_>.Empty)
            | None -> return BlockStructure(ImmutableArray<_>.Empty)
        } |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)