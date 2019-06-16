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
open ICSharpCode.AvalonEdit.CodeCompletion
open System.Windows.Media

type HighlightSpanKind = 
    | Underline = 0
    | Foreground = 1
    | Background = 2

[<Struct>]
type HighlightSpan = HighlightSpan of span: TextSpan * color: Drawing.Color * kind: HighlightSpanKind

// https://github.com/icsharpcode/AvalonEdit/issues/28
type FSharpCompletionData (text: string) =

    member __.Image = null

    member __.Text = text

    // you can technically show a fancy UI element here
    member __.Content = text :> obj

    member __.Description = text :> obj

    member __.Complete (textArea: Editing.TextArea, completionSegment: Document.ISegment, _: EventArgs) : unit =
        textArea.Document.Replace(completionSegment, text)

    member __.Priority = 0.

    interface ICompletionData with

        member this.Image = this.Image

        member this.Text = this.Text

        // you can technically show a fancy UI element here
        member this.Content = this.Content

        member this.Description = this.Description

        member this.Complete (textArea, completionSegment, args) = this.Complete (textArea, completionSegment, args)

        member __.Priority = 0.

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

    let completionTriggered = Event<SourceText * int> ()
    let sourceTextChanged = Event<SourceText * int * bool> ()
    let mutable sourceText = SourceText.From String.Empty
    let mutable prevTextChanges = [||]
    let mutable completionWindow: CompletionWindow = null

    let foregroundColorizer = FSharpDocumentColorizingTransformer this

    do
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
            TextChange (TextSpan (args.Offset, args.RemovalLength), args.InsertedText.Text)
            |> queueTextChanges.Enqueue
        )

        let mutable willCompletionTrigger = false

        this.TextArea.Document.TextChanged.Add (fun _ ->
            let textChanges = queueTextChanges.ToArray()
            prevTextChanges <- textChanges
            queueTextChanges.Clear ()
            sourceText <- (sourceText, textChanges) ||> Array.fold (fun text textChange -> text.WithChanges textChange)
            sourceTextChanged.Trigger (sourceText, this.CaretOffset, willCompletionTrigger)
        )

        this.TextArea.TextEntering.Add (fun args ->
            if args.Text = " " || args.Text = "." then
                willCompletionTrigger <- true

            if args.Text.Length > 0 && completionWindow <> null then
                if not (Char.IsLetterOrDigit (args.Text.[0])) then
                    completionWindow.CompletionList.RequestInsertion args
        )
        this.TextArea.TextEntered.Add (fun args ->
            if willCompletionTrigger then
                completionWindow <- CompletionWindow (this.TextArea)
                completionWindow.Closed.Add (fun _ ->
                    completionWindow <- null
                )
                willCompletionTrigger <- false
        )

    member this.ShowCompletions (data: FSharpCompletionData seq) =
        if completionWindow <> null && not (Seq.isEmpty data) then
            if not completionWindow.IsActive then
                data
                |> Seq.iter completionWindow.CompletionList.CompletionData.Add
                completionWindow.Show ()
        
    member __.SourceText = sourceText

    member __.SourceTextChanged = sourceTextChanged.Publish

    member __.CompletionTriggered = completionTriggered.Publish

    member __.GetTextSpanColors lineNumber = foregroundColorizer.GetTextSpanColors lineNumber

    member __.ClearAllTextSpanColors () = foregroundColorizer.ClearAllTextSpanColors ()

    member this.Redraw () =
        this.TextArea.TextView.Redraw ()
