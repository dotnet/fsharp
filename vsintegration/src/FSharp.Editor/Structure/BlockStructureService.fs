// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition
open System.Collections.Immutable
open System.Threading.Tasks

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.CodeAnalysis.Text
open Microsoft.CodeAnalysis.Structure

open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.SourceCodeServices.Structure

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
        | Scope.MatchBang
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

    let isAutoCollapsible = function
        | Scope.New
        | Scope.Attribute
        | Scope.Member
        | Scope.LetOrUse
        | Scope.EnumCase
        | Scope.UnionCase
        | Scope.SpecialFunc
        | Scope.HashDirective
        | Scope.Comment
        | Scope.Open
        | Scope.XmlDocComment -> true

        | Scope.Namespace
        | Scope.Module
        | Scope.Record
        | Scope.Interface
        | Scope.TypeExtension
        | Scope.RecordDefn
        | Scope.CompExpr
        | Scope.ObjExpr
        | Scope.UnionDefn
        | Scope.Type
        | Scope.RecordField
        | Scope.Match
        | Scope.MatchClause
        | Scope.MatchLambda
        | Scope.MatchBang
        | Scope.ThenInIfThenElse
        | Scope.ElseInIfThenElse
        | Scope.TryWith
        | Scope.TryInTryWith
        | Scope.WithInTryWith
        | Scope.TryFinally
        | Scope.TryInTryFinally
        | Scope.FinallyInTryFinally
        | Scope.IfThenElse
        | Scope.Tuple
        | Scope.ArrayOrList
        | Scope.CompExprInternal
        | Scope.Quote
        | Scope.Lambda
        | Scope.LetOrUseBang
        | Scope.Val
        | Scope.YieldOrReturn
        | Scope.YieldOrReturnBang
        | Scope.TryWith
        | Scope.Do
        | Scope.While
        | Scope.For -> false

    let createBlockSpans isBlockStructureEnabled (sourceText:SourceText) (parsedInput:Ast.ParsedInput) =
        let linetext = sourceText.Lines |> Seq.map (fun x -> x.ToString()) |> Seq.toArray
        
        Structure.getOutliningRanges linetext parsedInput
        |> Seq.distinctBy (fun x -> x.Range.StartLine)
        |> Seq.choose (fun scopeRange -> 
            // the range of text to collapse
            let textSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, scopeRange.CollapseRange)
            // the range of the entire expression
            let hintSpan = RoslynHelpers.TryFSharpRangeToTextSpan(sourceText, scopeRange.Range)
            match textSpan,hintSpan with
            | Some textSpan, Some hintSpan ->
                let line = sourceText.Lines.GetLineFromPosition textSpan.Start
                let bannerText =
                    match Option.ofNullable (line.Span.Intersection textSpan) with
                    | Some span -> sourceText.GetSubText(span).ToString()+"..."
                    | None -> "..."
                let blockType = if isBlockStructureEnabled then scopeToBlockType scopeRange.Scope else BlockTypes.Nonstructural
                Some (BlockSpan(blockType, true, textSpan, hintSpan, bannerText, autoCollapse = isAutoCollapsible scopeRange.Scope))
            | _, _ -> None
        )

open BlockStructure
 
type internal FSharpBlockStructureService(checker: FSharpChecker, projectInfoManager: FSharpProjectOptionsManager) =
    inherit BlockStructureService()
        
    static let userOpName = "BlockStructure"

    override __.Language = FSharpConstants.FSharpLanguageName
 
    override __.GetBlockStructureAsync(document, cancellationToken) : Task<BlockStructure> =
        asyncMaybe {
            let! parsingOptions, _options = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document)
            let! sourceText = document.GetTextAsync(cancellationToken)
            let! parsedInput = checker.ParseDocument(document, parsingOptions, sourceText, userOpName)
            return createBlockSpans document.FSharpOptions.Advanced.IsBlockStructureEnabled sourceText parsedInput |> Seq.toImmutableArray
        } 
        |> Async.map (Option.defaultValue ImmutableArray<_>.Empty)
        |> Async.map BlockStructure
        |> RoslynHelpers.StartAsyncAsTask(cancellationToken)

[<ExportLanguageServiceFactory(typeof<BlockStructureService>, FSharpConstants.FSharpLanguageName); Shared>]
type internal FSharpBlockStructureServiceFactory [<ImportingConstructor>](checkerProvider: FSharpCheckerProvider, projectInfoManager: FSharpProjectOptionsManager) =
    interface ILanguageServiceFactory with
        member __.CreateLanguageService(_languageServices) =
            upcast FSharpBlockStructureService(checkerProvider.Checker, projectInfoManager)