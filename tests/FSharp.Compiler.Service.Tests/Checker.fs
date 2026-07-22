namespace FSharp.Compiler.Service.Tests

open System
open System.Text.RegularExpressions
open FSharp.Compiler.CodeAnalysis
open FSharp.Compiler.EditorServices
open FSharp.Compiler.Text
open FSharp.Compiler.Text.Range
open FSharp.Compiler.Tokenization
open FSharp.Test.Assert

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
    type private Marker =
        { Text: string
          Position: pos }

    type private SourceMarkers =
        { Caret: Marker option
          SelectionStart: Marker option
          SelectionEnd: Marker option
          Id: int option }

    let private markers = ["caret"; "selstart"; "selend"]

    let getLines (source: string) =
        source.Split([|"\r\n"; "\n"|], StringSplitOptions.None)

    let private stripMarkers (markedSource: string) =
        let names = String.concat "|" markers
        Regex.Replace(markedSource, $@"\{{({names})\d*\}}", "")

    let rec private extractMarkersOnLine (markersAcc: (int option * Marker) list) (line, lineText: string) =
        let markersOnLine =
            markers
            |> List.choose (fun (marker: string) ->
                let regexMatch = Regex.Match(lineText, $@"\{{{marker}(\d*)\}}")
                if regexMatch.Success then Some(marker, regexMatch) else None
            )

        if markersOnLine.IsEmpty then
            markersAcc
        else
            let marker, regexMatch = markersOnLine |> List.minBy (fun (_, m) -> m.Index)

            let column =
                match marker with
                | "caret" -> regexMatch.Index - 1
                | _ -> regexMatch.Index

            let markerPos = Position.mkPos (line + 1) column

            let id =
                match regexMatch.Groups.[1].Value with
                | "" -> None
                | value -> Some(int value)

            if markersAcc |> List.exists (fun (markerId, m) -> m.Text = marker && markerId = id) then
                failwith $"Duplicate marker: {regexMatch.Value}"

            let markersAcc = (id, { Text = marker; Position = markerPos }) :: markersAcc
            let lineText = lineText.Replace(regexMatch.Value, "")

            extractMarkersOnLine markersAcc (line, lineText)

    let private extractSourceMarkers (markedSource: string) : string * SourceMarkers list =
        let sourceMarkers =
            getLines markedSource
            |> Seq.indexed
            |> Seq.fold extractMarkersOnLine []
            |> List.groupBy fst
            |> List.map (fun (id, group) ->
                let markers = group |> List.map snd
                let tryFind text = markers |> List.tryFind (fun m -> m.Text = text)

                { Id = id
                  Caret = tryFind "caret"
                  SelectionStart = tryFind "selstart"
                  SelectionEnd = tryFind "selend" })

        stripMarkers markedSource, sourceMarkers

    let private toSourceContext (source: string) (sourceMarkers: SourceMarkers) : SourceContext =
        let reportError message =
            let prefix =
                match sourceMarkers with
                | { Id = Some id } -> $"{id}: "
                | _ -> ""

            failwith (prefix + message)

        let caretPos, selectedRange =
            match sourceMarkers.Caret, sourceMarkers.SelectionStart, sourceMarkers.SelectionEnd with
            | Some caret, None, None ->
                caret.Position, None

            | Some caret, Some selStart, Some selEnd ->
                let selectedRange = mkRange "Test.fsx" selStart.Position selEnd.Position
                caret.Position, Some selectedRange

            | None, Some selStart, Some selEnd ->
                let selectedRange = mkRange "Test.fsx" selStart.Position selEnd.Position
                let caretPos = Position.mkPos selEnd.Position.Line (selEnd.Position.Column - 1)
                caretPos, Some selectedRange

            | _, None, Some _ -> reportError "Missing selected range start"
            | _, Some _, None -> reportError "Missing selected range end"
            | None, None, None -> reportError "Missing caret marker"

        let lines = getLines source
        let lineText = Array.get lines (caretPos.Line - 1)

        { Source = source; CaretPos = caretPos; LineText = lineText; SelectedRange = selectedRange }

    let fromMarkedSource (markedSource: string) : SourceContext =
        let source, sourceMarkers = extractSourceMarkers markedSource
        let sourceMarkers = sourceMarkers |> List.exactlyOne
        sourceMarkers.Id |> shouldBe None

        toSourceContext source sourceMarkers

    let fromOrderedMarkedSource (orderedMarkedSource: string) : SourceContext list =
        let source, sourceMarkers = extractSourceMarkers orderedMarkedSource
        sourceMarkers |> List.iter (fun markers -> markers.Id.IsSome |> shouldBeTrue)

        sourceMarkers
        |> List.sortBy _.Id
        |> List.map (toSourceContext source)

    let toMarkedSource (context: SourceContext) : string =
        let skipCaretMarker =
            match context.SelectedRange with
            | Some range -> context.CaretPos.Line = range.End.Line && context.CaretPos.Column = range.End.Column - 1
            | None -> false

        let insertions =
            [ if not skipCaretMarker then
                  context.CaretPos.Line, context.CaretPos.Column + 1, "{caret}"

              match context.SelectedRange with
              | Some range ->
                  range.Start.Line, range.Start.Column, "{selstart}"
                  range.End.Line, range.End.Column, "{selend}"
              | None -> () ]

        let lines = getLines context.Source

        for line, column, marker in insertions |> List.sortByDescending (fun (line, column, _) -> line, column) do
            lines[line - 1] <- lines[line - 1].Insert(column, marker)

        String.concat "\n" lines

    let extractOrderedMarkedSources (markedSource: string) : string list =
        fromOrderedMarkedSource markedSource
        |> List.map toMarkedSource


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

        member this.GetMethodOverloads(context: ResolveContext, names: string list) =
            this.GetMethodsAsSymbols(context.Pos.Line, context.Pos.Column, context.LineText, names)

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

    let getCompletionInfoWithCompilerAndCompletionOptions (compilerOptions: string array) (completionOptions: FSharpCodeCompletionOptions) (markedSource: string) =
        let context = getCompletionContext markedSource
        let parseResults, checkResults = getParseAndCheckResultsWithOptions compilerOptions context.Source
        checkResults.GetCodeCompletionSuggestions(context, parseResults, completionOptions)

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

    let getMethodOverloads names (markedSource: string) =
        let context, checkResults = getCheckedResolveContext markedSource
        checkResults.GetMethodOverloads(context, names)
