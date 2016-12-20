// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

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

open Microsoft.VisualStudio.FSharp
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

module internal BlockStructure =
    
    /// Find the length of the shortest whitespace indentation in the textblock used for the outlining
    let findIndent (text:string) =
        let countLeadingWhitespace (str:string) =
            let rec loop acc =
                if acc >= str.Length then acc
                elif not (Char.IsWhiteSpace str.[acc]) then acc
                else loop (acc+1)
            loop 0

        let lines = String.getLines text

        // To find the smallest indentation, an empty line can't serve as the seed
        let rec tryFindStartingLine idx  =
            if idx >= lines.Length then None  // return None if all the lines are blank
            elif String.IsNullOrWhiteSpace lines.[idx] then tryFindStartingLine (idx+1)
            else Some idx // found suitable starting line

        match tryFindStartingLine 0 with
        | None -> 0
        | Some startIndex ->
            if Array.isEmpty lines then 0 else
            let minIndent =
                let seed = countLeadingWhitespace lines.[startIndex]
                (seed, lines.[startIndex..])
                ||> Array.fold (fun indent line ->
                    if String.IsNullOrWhiteSpace line then indent // skip over empty lines, we don't want them skewing the min
                    else countLeadingWhitespace line |> min indent)
            minIndent

    // drills down into the snapshot text to find the first non whitespace line
    // to display as the text inside the collapse box preceding the `...`
    let getHintText (sourceText:SourceText) =

        let firstLineNum = sourceText.Lines.First().LineNumber
        let rec loop acc =
            if acc >= sourceText.Lines.Count + firstLineNum then "" else
            let text =  
                    sourceText.Lines.[acc].Text.ToString() 
                //if acc = firstLineNum then
                //    let colstart = sourceText.Lines.First().
                //    textshot.LineText(acc).Substring(colstart).Trim ()
                //else 
                //    textshot.LineText(acc).Trim ()
            if String.IsNullOrWhiteSpace text then loop (acc+1) else text
        loop firstLineNum



    let scopeToBlockType = function
    | Scope.Open -> BlockTypes.Imports
    | Scope.Namespace
    | Scope.Module -> BlockTypes.Namespace 
    | Scope.Record
    | Scope.Interface
    | Scope.TypeExtension
    | Scope.SimpleType
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
    | Scope.YieldOrReturn
    | Scope.YieldOrReturnBang
    | Scope.TryWith -> BlockTypes.Expression
    | Scope.Do -> BlockTypes.Statement
    | Scope.While
    | Scope.For -> BlockTypes.Loop
    | Scope.HashDirective -> BlockTypes.PreprocessorRegion
    | Scope.Comment
    | Scope.XmlDocComment -> BlockTypes.Comment

    //let createTagSpan (sourceText:SourceText) (scopedSpan: ScopeRange) =
    //    let textSpan = CommonRoslynHelpers.FSharpRangeToTextSpan(sourceText,scopedSpan.Range)

    //    let scope, collapse = scopedSpan.Scope, scopedSpan.Collapse
    //    //let snapshot = snapshotSpan. Snapshot
    //    let firstLine = sourceText.Lines.First()
    //    let mutable lastLine = sourceText.Lines.Last()

    //    let hintTextSpan = TextSpan()


        //let collapseText, collapseSpan =
        //        /// Determine the text that will be displayed in the collapse box and the contents of the hint tooltip
        //    let inline mkOutliningPair (token:string) (md:int) (collapse:Collapse) =
        //        match collapse, firstLine.Text.ToString().IndexOf token with // Type extension where `with` is on a lower line
        //        | Collapse.Same, -1 -> ((getHintText sourceText) + "...", snapshotSpan)
        //        | _ (* Collapse.Below *) , -1 ->  ("...", snapshotSpan)
        //        | Collapse.Same, idx  ->
        //            let modSpan = VS.Snapshot.mkSpan (firstLine.Start + idx + token.Length + md) snapshotSpan.End
        //            ((getHintText modSpan) + "...", modSpan)
        //        | _ (*Collapse.Below*), idx ->
        //            let modSpan = VS.Snapshot.mkSpan (firstLine.Start + idx + token.Length + md) snapshotSpan.End
        //            ( "...", modSpan)

        //    let (|OutliningPair|_|) (collapse:Collapse) (_:Scope) =
        //        match collapse with
        //        | Collapse.Same -> Some ((getHintText snapshotSpan) + "...", snapshotSpan)
        //        | _ (*Collapse.Below*) -> Some ("...", snapshotSpan)

        //    let lineText = firstLine.GetText()

        //    let inline pairAlts str1 str2 =
        //        if lineText.Contains str1 then mkOutliningPair str1 0 collapse else mkOutliningPair str2 0 collapse

        //    let (|StartsWith|_|) (token:string) (sspan:SnapshotSpan) =
        //        if token = "{" then // quick terminate for `{` start, this case must follow all access qualified matches in pattern match below
        //            let modspan = (sspan.ModBoth 1 -1)
        //            Some ((getHintText modspan) + "...", modspan) else
        //        let startText = lineText.SubstringSafe sspan.StartColumn
        //        if startText.StartsWith token then
        //            match sspan.PositionOf "{" with
        //            | bl,_ when bl>sspan.StartLineNum -> Some ("{...}",(sspan.ModStart (token.Length)))
        //            | bl,bc when bl=sspan.StartLineNum ->
        //                let modSpan = (sspan.ModStart (bc-sspan.StartColumn)).ModBoth 1 -1
        //                Some (getHintText modSpan+"...",modSpan)
        //            | _ -> None
        //        else None

        //    match scope with
        //    | Scope.Type
        //    | Scope.Module
        //    | Scope.Member
        //    | Scope.LetOrUse
        //    | Scope.LetOrUseBang -> mkOutliningPair "=" 0 collapse
        //    | Scope.ObjExpr
        //    | Scope.Interface
        //    | Scope.TypeExtension -> mkOutliningPair "with" 0 collapse
        //    | Scope.Match ->
        //        let idx = lineText.IndexOf "->"
        //        if idx = -1 then  mkOutliningPair "|" -1 collapse else  // special case to collapse compound guards
        //        let substr = lineText.SubstringSafe (idx+2)
        //        if substr = String.Empty || String.IsNullOrWhiteSpace substr then
        //            mkOutliningPair "->" 0 Collapse.Below
        //        else
        //            mkOutliningPair "->" 0 Collapse.Same
        //    | Scope.YieldOrReturn -> pairAlts "yield" "return"
        //    | Scope.YieldOrReturnBang -> pairAlts "yield!" "return!"
        //    | Scope.Lambda ->  mkOutliningPair "->" 0 collapse
        //    | Scope.IfThenElse -> mkOutliningPair "if" 0 collapse
        //    | Scope.RecordDefn ->
        //        match snapshotSpan with
        //        | StartsWith "private"  pair -> pair
        //        | StartsWith "public"   pair -> pair
        //        | StartsWith "internal" pair -> pair
        //        | StartsWith "{"  pair -> pair
        //        | _ -> ("...", snapshotSpan) // should never be reached due to AP
        //    | OutliningPair collapse pair -> pair
        //    | _ -> ("...", snapshotSpan) // should never be reached due to AP
        //collapseText, collapseSpan


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
                    Some <| (BlockSpan(scopeToBlockType scopeRange.Scope, true, textSpan,hintSpan):BlockSpan)
            | _, _ -> None
        )
        

open BlockStructure

 
type internal FSharpBlockStructureService(checker: FSharpChecker, projectInfoManager: ProjectInfoManager) =
    inherit BlockStructureService()
        
    override __.Language = FSharpCommonConstants.FSharpLanguageName
 
    override __.GetBlockStructureAsync(document, cancellationToken) : Task<BlockStructure> =
        async {
            match projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document) with 
            | Some options ->
                let! sourceText = document.GetTextAsync(cancellationToken) |> Async.AwaitTask
                let! fileParseResults = checker.ParseFileInProject(document.FilePath, sourceText.ToString(), options)
                match fileParseResults.ParseTree with
                | Some parsedInput ->
                    let blockSpans = createBlockSpans sourceText parsedInput
                    return BlockStructure(blockSpans.ToImmutableArray())
                | None -> return BlockStructure(ImmutableArray<_>.Empty)
            | None -> return BlockStructure(ImmutableArray<_>.Empty)
        } |> CommonRoslynHelpers.StartAsyncAsTask(cancellationToken)


[<ExportLanguageServiceFactory(typeof<BlockStructureService>, FSharpCommonConstants.FSharpLanguageName); Shared>]
type internal FSharpBlockStructureServiceFactory [<ImportingConstructor>](checkerProvider: FSharpCheckerProvider, projectInfoManager: ProjectInfoManager) =
    interface ILanguageServiceFactory with
        member __.CreateLanguageService(_languageServices) =
            upcast FSharpBlockStructureService(checkerProvider.Checker, projectInfoManager)