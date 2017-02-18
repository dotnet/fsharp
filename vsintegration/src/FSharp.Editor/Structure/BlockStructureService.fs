// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Composition
open System.Collections.Immutable
open System.Threading
open System.Threading.Tasks
open System.Runtime.CompilerServices

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Editor
open Microsoft.CodeAnalysis.Host
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Options
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Structure

open Microsoft.VisualStudio.FSharp
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Tagging
open Microsoft.VisualStudio.Shell

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Parser
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler.SourceCodeServices
open System.Windows.Documents
open Microsoft.VisualStudio.FSharp.Editor.Structure

module internal BlockStructure =
    let scopeToBlockType = function
    | Scope.Open -> BlockTypes.Imports
    | Scope.Namespace
    | Scope.Module -> BlockTypes.Namespace 
    | Scope.Record
    | Scope.Interface
    | Scope.TypeExtension
    | Scope.RecordDefn
    | Scope.CompExpr
    | Scope.ObjExpr
    | Scope.UnionDefn
    | Scope.Attribute
    | Scope.Type -> BlockTypes.Type
    | Scope.New
    | Scope.RecordField
    | Scope.Member -> BlockTypes.Member
    | Scope.LetOrUse
    | Scope.Match
    | Scope.MatchClause
    | Scope.EnumCase
    | Scope.UnionCase
    | Scope.MatchLambda
    | Scope.ThenInIfThenElse
    | Scope.ElseInIfThenElse
    | Scope.TryWith
    | Scope.TryInTryWith
    | Scope.WithInTryWith
    | Scope.TryFinally
    | Scope.TryInTryFinally
    | Scope.FinallyInTryFinally
    | Scope.IfThenElse-> BlockTypes.Conditional
    | Scope.Tuple
    | Scope.ArrayOrList
    | Scope.CompExprInternal
    | Scope.Quote
    | Scope.SpecialFunc
    | Scope.Lambda
    | Scope.LetOrUseBang
    | Scope.Val
    | Scope.YieldOrReturn
    | Scope.YieldOrReturnBang
    | Scope.TryWith -> BlockTypes.Expression
    | Scope.Do -> BlockTypes.Statement
    | Scope.While
    | Scope.For -> BlockTypes.Loop
    | Scope.HashDirective -> BlockTypes.PreprocessorRegion
    | Scope.Comment
    | Scope.XmlDocComment -> BlockTypes.Comment

    let createBlockSpans (sourceText:SourceText) (parsedInput:Ast.ParsedInput) =
        let linetext = sourceText.Lines |> Seq.map (fun x -> x.ToString()) |> Seq.toArray
        
        Structure.getOutliningRanges linetext parsedInput
        |> Seq.distinctBy (fun x -> x.Range.StartLine)
        |> Seq.choose (fun scopeRange -> 
            // the range of text to collapse
            let textSpan = CommonRoslynHelpers.TryFSharpRangeToTextSpan(sourceText, scopeRange.CollapseRange)
            // the range of the entire expression
            let hintSpan = CommonRoslynHelpers.TryFSharpRangeToTextSpan(sourceText, scopeRange.Range)
            match textSpan,hintSpan with
            | Some textSpan, Some hintSpan ->
                let line = sourceText.Lines.GetLineFromPosition  textSpan.Start
                let bannerText =
                    match Option.ofNullable (line.Span.Intersection textSpan) with
                    | Some span -> sourceText.GetSubText(span).ToString()+"..."
                    | None -> "..."

                Some <| (BlockSpan(scopeToBlockType scopeRange.Scope, true, textSpan,hintSpan,bannerText):BlockSpan)
            | _, _ -> None
        )

open BlockStructure
 
type internal FSharpBlockStructureService(checker: FSharpChecker, projectInfoManager: ProjectInfoManager) =
    inherit BlockStructureService()
        
    override __.Language = FSharpCommonConstants.FSharpLanguageName
 
    override __.GetBlockStructureAsync(document, cancellationToken) : Task<BlockStructure> =
        Logging.Logging.logInfof "=> BlockStructureService.GetBlockStructureAsync\n%s" Environment.StackTrace
        asyncMaybe {
            use! __ = Async.OnCancel(fun () -> Logging.Logging.logInfof "CANCELLED BlockStructureService.GetBlockStructureAsync\n%s" Environment.StackTrace) |> liftAsync
            let! options = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document)
            let! sourceText = document.GetTextAsync(cancellationToken)
            let! parsedInput = checker.ParseDocument(document, options, sourceText)
            let blockSpans = createBlockSpans sourceText parsedInput
            return blockSpans.ToImmutableArray()
        } 
        |> Async.map (Option.defaultValue ImmutableArray<_>.Empty)
        |> Async.map BlockStructure
        |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)

[<ExportLanguageServiceFactory(typeof<BlockStructureService>, FSharpCommonConstants.FSharpLanguageName); Shared>]
type internal FSharpBlockStructureServiceFactory [<ImportingConstructor>](checkerProvider: FSharpCheckerProvider, projectInfoManager: ProjectInfoManager) =
    interface ILanguageServiceFactory with
        member __.CreateLanguageService(_languageServices) =
            upcast FSharpBlockStructureService(checkerProvider.Checker, projectInfoManager)