namespace FSharp.Compiler.Service.Tests

open System
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Tokenization

type SourceContext =
    { Source: string
      LineText: string
      CaretPos: pos
      SelectedRange: range option }

type ResolveContext =
    { SourceContext: SourceContext
      Pos: pos
      Names: string list
      SelectedRange: range option }

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
    let private markers = ["{caret}"; "{selstart}"; "{selend}"]

    let getLines (source: string) =
        source.Split([|"\r\n"; "\n"|], StringSplitOptions.None)

    let rec private extractMarkersOnLine markersAcc (line, lineText: string) =
        let markersOnLine =
            markers
            |> List.choose (fun (marker: string) ->
                match lineText.IndexOf(marker) with
                | -1 -> None
                | column -> Some(marker, column)
            )

        if markersOnLine.IsEmpty then
            markersAcc
        else
            let marker, column = List.minBy snd markersOnLine

            let markerPos =
                let column =
                    match marker with
                    | "{caret}" -> column - 1
                    | _ -> column

                Position.mkPos (line + 1) column

            if markersAcc |> List.map fst |> List.contains marker then
                failwith $"Duplicate marker: {marker}"

            let markersAcc = (marker, markerPos) :: markersAcc
            let lineText = lineText.Replace(marker, "")

            extractMarkersOnLine markersAcc (line, lineText)

    let fromMarkedSource (markedSource: string) : SourceContext =
        let markerPositions =
            getLines markedSource
            |> Seq.indexed
            |> Seq.fold extractMarkersOnLine []

        let source =
            markerPositions
            |> List.map fst
            |> List.fold (fun (source: string) marker -> source.Replace(marker, "")) markedSource

        let markerPositions = markerPositions |> dict

        let tryGetPos marker =
            match markerPositions.TryGetValue(marker) with
            | true, pos -> Some pos
            | _ -> None

        let caretPos, selectedRange =
            match tryGetPos "{caret}", tryGetPos "{selstart}", tryGetPos "{selend}" with
            | Some caretPos, None, None ->
                caretPos, None

            | Some caretPos, Some startPos, Some endPos ->
                let selectedRange = mkRange "Test.fsx" startPos endPos
                caretPos, Some selectedRange

            | None, Some startPos, Some endPos ->
                let selectedRange = mkRange "Test.fsx" startPos endPos
                let caretPos = Position.mkPos endPos.Line (endPos.Column - 1)
                caretPos, Some selectedRange

            | _, None, Some _ -> failwith "Missing selected range start"
            | _, Some _, None -> failwith "Missing selected range end"

            | None, None, None -> failwith "Missing caret marker"

        let lines = getLines source
        let lineText = Array.get lines (caretPos.Line - 1)

        { Source = source; CaretPos = caretPos; LineText = lineText; SelectedRange = selectedRange }


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

        member this.GetCodeCompletionSuggestions(context: CodeCompletionContext, parseResults: FSharpParseFileResults, options: FSharpCodeCompletionOptions) =
            this.GetDeclarationListInfo(Some parseResults, context.Pos.Line, context.LineText, context.PartialIdentifier, options = options)

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
        { SourceContext = context; Pos = pos; Names = names; SelectedRange = context.SelectedRange }

    let getCompletionContext (markedSource: string) =
        let context = SourceContext.fromMarkedSource markedSource
        let plid = QuickParse.GetPartialLongNameEx(context.LineText, context.CaretPos.Column)
        let names = plid.QualifyingIdents @ [plid.PartialIdent]
        { SourceContext = context; Pos = context.CaretPos; PartialIdentifier = plid }

    let getParseResultsWithContext (markedSource: string) =
        let context = SourceContext.fromMarkedSource markedSource
        let parseResults = getParseFileResults "Test.fsx" context.Source
        context, parseResults

    let getCheckedResolveContext (markedSource: string) =
        let context = getResolveContext markedSource
        let _, checkResults = getParseAndCheckResults context.Source
        context, checkResults

    let getCompletionInfoWithOptions (options: FSharpCodeCompletionOptions) (markedSource: string) =
        let context = getCompletionContext markedSource
        let parseResults, checkResults = getParseAndCheckResults context.Source
        checkResults.GetCodeCompletionSuggestions(context, parseResults, options)

    let getCompletionInfo markedSource =
        getCompletionInfoWithOptions FSharpCodeCompletionOptions.Default markedSource

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
