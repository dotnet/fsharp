namespace FSharp.Tools.Visualizer.WPF

open System
open System.Collections
open System.Collections.Generic
open System.Collections.Concurrent
open System.Collections.Immutable
open Microsoft.CodeAnalysis.Text
open ICSharpCode.AvalonEdit
open ICSharpCode.AvalonEdit.Highlighting
open ICSharpCode.AvalonEdit.Rendering
open System.Windows.Media

type HighlightSpanKind = 
    | Underline = 0
    | Foreground = 1
    | Background = 2

[<Struct>]
type HighlightSpan = HighlightSpan of span: TextSpan * color: Drawing.Color * kind: HighlightSpanKind

type private FSharpDocumentColorizingTransformer (editor: FSharpTextEditor) =
    inherit DocumentColorizingTransformer ()

    let lookup = Dictionary<int, ResizeArray<HighlightSpan>> ()

    member __.GetTextSpanColors lineNumber =
        let textSpanColors =
            match lookup.TryGetValue lineNumber with
            | true, textSpanColors -> textSpanColors
            | _ ->
                let textSpanColors = ResizeArray ()
                lookup.[lineNumber] <- textSpanColors
                textSpanColors
        textSpanColors

    override this.ColorizeLine line =
        let textSpanColors = this.GetTextSpanColors line.LineNumber
        for i = 0 to textSpanColors.Count - 1 do
            let (HighlightSpan (span, color, kind)) = textSpanColors.[i]
            if line.Length = 0 || span.Length = 0 then
                ()
            elif span.Start < line.Offset || span.End > line.EndOffset then
                ()
            else
                let brush = System.Windows.Media.SolidColorBrush (Color.FromRgb (color.R, color.G, color.B))
                this.ChangeLinePart (span.Start, span.End, fun element ->
                    match kind with
                    | HighlightSpanKind.Underline ->
                        element.TextRunProperties.SetTextDecorations(Windows.TextDecorations.Underline)
                    | HighlightSpanKind.Foreground ->
                        element.TextRunProperties.SetForegroundBrush brush
                    | HighlightSpanKind.Background ->
                        element.TextRunProperties.SetBackgroundBrush brush
                    | _ ->
                        ()
                )

    member __.ClearAllTextSpanColors () = lookup.Clear ()

and FSharpTextEditor () as this =
    inherit TextEditor ()

    let sourceTextChanged = Event<SourceText> ()
    let mutable sourceText = SourceText.From String.Empty
    let mutable prevTextChanges = [||]

    let defaultColor = HighlightingColor (Name = "Default")
    let symbolColor = HighlightingColor (Name = "Symbol")

    let foregroundColorizer = FSharpDocumentColorizingTransformer this

    do
        let brush = SimpleHighlightingBrush (Windows.Media.Colors.Red)
        symbolColor.Background <- brush
        this.ShowLineNumbers <- true
        this.Options.ConvertTabsToSpaces <- true
        this.FontFamily <- Windows.Media.FontFamily ("Consolas")
        this.Foreground <- Windows.Media.SolidColorBrush (Windows.Media.Color.FromRgb(220uy, 220uy, 220uy)) 
        this.Background <- Windows.Media.SolidColorBrush (Windows.Media.Color.FromRgb(30uy, 30uy, 30uy))
        this.FontSize <- 14.
        this.BorderThickness <- Windows.Thickness(0.)
        this.AllowDrop <- false
        this.TextArea.TextView.LineTransformers.Add foregroundColorizer

        let queueTextChanges = Queue<TextChange>()
        this.Document.Changing.Add (fun args ->
            if args.InsertionLength > 0 then
                TextChange (TextSpan (args.Offset, 0), args.InsertedText.Text)
                |> queueTextChanges.Enqueue
            elif args.RemovalLength > 0 then
                TextChange (TextSpan (args.Offset, args.RemovalLength), String.Empty)
                |> queueTextChanges.Enqueue
        )
        this.Document.TextChanged.Add (fun _ ->
            let textChanges = queueTextChanges.ToArray()
            prevTextChanges <- textChanges
            queueTextChanges.Clear ()
            sourceText <- (sourceText, textChanges) ||> Array.fold (fun text textChange -> text.WithChanges textChange)
            sourceTextChanged.Trigger sourceText    
        )
        
    member __.SourceText = sourceText

    member __.SourceTextChanged = sourceTextChanged.Publish

    member __.GetTextSpanColors lineNumber = foregroundColorizer.GetTextSpanColors lineNumber

    member __.ClearAllTextSpanColors () = foregroundColorizer.ClearAllTextSpanColors ()

    member this.Redraw () =
        this.TextArea.TextView.Redraw ()

    interface IHighlightingDefinition with

        member this.GetNamedColor(name: string): HighlightingColor = 
            match name with
            | "Symbol" -> symbolColor
            | _ -> defaultColor

        member this.GetNamedRuleSet(name: string): HighlightingRuleSet = 
            HighlightingRuleSet ()

        member this.MainRuleSet: HighlightingRuleSet = HighlightingRuleSet ()

        member this.Name: string = "FSharpTextEditor"

        member this.NamedHighlightingColors: IEnumerable<HighlightingColor> = 
            [
                defaultColor
                symbolColor
            ]
            |> Seq.ofList

        member this.Properties: IDictionary<string,string> = 
            Dictionary () :> IDictionary<_, _>

