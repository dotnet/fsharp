namespace FSharp.Compiler.Service.Tests

open System
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text
open FSharp.Compiler.Tokenization

type SourceContext =
    { Source: string
      LineText: string
      CaretPos: pos }

type ResolveContext =
    { SourceContext: SourceContext
      Pos: pos
      Names: string list }

    member this.Source = this.SourceContext.Source
    member this.LineText = this.SourceContext.LineText

type CodeCompletionContext =
    { SourceContext: SourceContext
      Pos: pos
      PartialIdentifier: PartialLongName }

    member this.Source = this.SourceContext.Source
    member this.LineText = this.SourceContext.LineText

[<RequireQualifiedAccess>]
module SourceContext =
    let fromMarkedSource (markedSource: string) : SourceContext =
        let lines = markedSource.Split([|"\r\n"; "\n"|], StringSplitOptions.None)
        let line = lines |> Seq.findIndex _.Contains("{caret}")
        let lineText = lines[line]
        let column = lineText.IndexOf("{caret}")

        let source = markedSource.Replace("{caret}", "")
        let pos = Position.mkPos (line + 1) (column - 1)
        let lineText = lineText.Replace("{caret}", "")
        { Source = source; CaretPos = pos; LineText = lineText }


[<AutoOpen>]
module CheckResultsExtensions =
    type FSharpCheckFileResults with
        member this.GetSymbolUses(context: ResolveContext) =
            this.GetSymbolUsesAtLocation(context.Pos.Line, context.Pos.Column, context.LineText, context.Names)

        member this.GetSymbolUse(context: ResolveContext) =
            this.GetSymbolUses(context) |> List.exactlyOne
    
        member this.GetTooltip(context: ResolveContext) =
            this.GetToolTip(context.Pos.Line, context.Pos.Column, context.LineText, context.Names, FSharpTokenTag.Identifier)

        member this.GetTooltip(context: ResolveContext, width) =
            this.GetToolTip(context.Pos.Line, context.Pos.Column, context.LineText, context.Names, FSharpTokenTag.Identifier, width)

        member this.GetCodeCompletionSuggestions(context: CodeCompletionContext, parseResults: FSharpParseFileResults) =
            this.GetDeclarationListInfo(Some parseResults, context.Pos.Line, context.LineText, context.PartialIdentifier)

[<RequireQualifiedAccess>]
module Checker =
    let getResolveContext (markedSource: string) =
        let context = SourceContext.fromMarkedSource markedSource
        let pos =
            match QuickParse.GetCompleteIdentifierIsland false context.LineText context.CaretPos.Column with
            | Some(_, column, _) -> Position.mkPos context.CaretPos.Line column
            | _ -> context.CaretPos

        let plid = QuickParse.GetPartialLongNameEx(context.LineText, pos.Column - 1)
        let names = plid.QualifyingIdents @ [plid.PartialIdent]
        { SourceContext = context; Pos = pos; Names = names }

    let getCompletionContext (markedSource: string) =
        let context = SourceContext.fromMarkedSource markedSource
        let plid = QuickParse.GetPartialLongNameEx(context.LineText, context.CaretPos.Column)
        let names = plid.QualifyingIdents @ [plid.PartialIdent]
        { SourceContext = context; Pos = context.CaretPos; PartialIdentifier = plid }

    let getCheckedResolveContext (markedSource: string) =
        let context = getResolveContext markedSource
        let _, checkResults = getParseAndCheckResults context.Source
        context, checkResults

    let getCompletionInfo (markedSource: string) =
        let context = getCompletionContext markedSource
        let parseResults, checkResults = getParseAndCheckResults context.Source
        checkResults.GetCodeCompletionSuggestions(context, parseResults)

    let getSymbolUses (markedSource: string) =
        let context, checkResults = getCheckedResolveContext markedSource
        checkResults.GetSymbolUses(context)

    let getSymbolUse (markedSource: string) =
        let symbolUses = getSymbolUses markedSource
        symbolUses |> List.exactlyOne

    let getTooltipWithOptions (options: string array) (markedSource: string) =
        let context = getResolveContext markedSource
        let _, checkResults = getParseAndCheckResultsWithOptions options context.Source
        checkResults.GetToolTip(context.Pos.Line, context.Pos.Column, context.LineText, context.Names, FSharpTokenTag.Identifier)

    let getTooltip (markedSource: string) =
        getTooltipWithOptions [||] markedSource
