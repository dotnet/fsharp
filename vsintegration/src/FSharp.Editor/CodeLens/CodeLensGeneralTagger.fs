// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System
open System.Windows.Controls
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.Text.Formatting
open System.Windows
open System.Collections.Generic
open Microsoft.VisualStudio.Text.Tagging

open Microsoft.VisualStudio.FSharp.Editor.Logging

type CodeLensGeneralTag(width, topSpace, baseline, textHeight, bottomSpace, affinity, tag:obj, providerTag:obj) =
    inherit SpaceNegotiatingAdornmentTag(width, topSpace, baseline, textHeight, bottomSpace, affinity, tag, providerTag)

/// Class which provides support for general code lens
/// Use the methods <code>AddCodeLens</code> and <code> RemoveCodeLens</code>
type CodeLensGeneralTagger (view, buffer) as self =
    inherit CodeLensDisplayService(view, buffer, "CodeLens")

    /// The tags changed event to notify if the data for the tags has changed.
    let tagsChangedEvent = new Event<EventHandler<SnapshotSpanEventArgs>,SnapshotSpanEventArgs>()
    
    /// Layouts all stack panels on the line
    override self.LayoutUIElementOnLine (view:IWpfTextView) (line:ITextViewLine) (ui:Grid) =
        let left, top = 
            match self.UiElementNeighbour.TryGetValue ui with
            | true, parent -> 
                let left = Canvas.GetLeft parent
                let top = Canvas.GetTop parent
                let width = parent.ActualWidth
                logInfof "Width of parent: %.4f" width
                left + width, top
            | _ ->
                try
                    // Get the real offset so that the code lens are placed respectively to their content
                    let offset =
                        [0..line.Length - 1] |> Seq.tryFind (fun i -> not (Char.IsWhiteSpace (line.Start.Add(i).GetChar())))
                        |> Option.defaultValue 0

                    let realStart = line.Start.Add(offset)
                    let g = view.TextViewLines.GetCharacterBounds(realStart)
                    // WORKAROUND VS BUG, left cannot be zero if the offset is creater than zero!
                    // Calling the method twice fixes this bug and ensures that all values are correct.
                    // Okay not really :( Must be replaced later with an own calculation depending on editor font settings!
                    if 7 * offset > int g.Left then
                        logErrorf "Incorrect return from geometry measure"
                        Canvas.GetLeft ui, g.Top
                    else 
                        g.Left, g.Top
                with e -> 
                    logExceptionWithContext (e, "Error in layout ui element on line")
                    Canvas.GetLeft ui, Canvas.GetTop ui
        Canvas.SetLeft(ui, left)
        Canvas.SetTop(ui, top)

    override self.AsyncCustomLayoutOperation _ _ =
        asyncMaybe {
                // Suspend 16 ms, instantly applying the layout to the adornment elements isn't needed 
                // and would consume too much performance
                do! Async.Sleep(16) |> liftAsync // Skip at least one frames
                do! Async.SwitchToContext self.UiContext |> liftAsync
                let layer = self.CodeLensLayer

                do! Async.Sleep(495) |> liftAsync

                // WORKAROUND FOR VS BUG
                // The layout changed event may not provide us all real changed lines so
                // we take care of this on our own.
                let visibleSpan =
                    let first, last = 
                        view.TextViewLines.FirstVisibleLine, 
                        view.TextViewLines.LastVisibleLine
                    SnapshotSpan(first.Start, last.End)
                let customVisibleLines = view.TextViewLines.GetTextViewLinesIntersectingSpan visibleSpan
                let isLineVisible (line:ITextViewLine) = line.IsValid
                let linesToProcess = customVisibleLines |> Seq.filter isLineVisible

                for line in linesToProcess do
                    try
                        match line.GetAdornmentTags self |> Seq.tryHead with
                        | Some (:? seq<Grid> as stackPanels) ->
                            for stackPanel in stackPanels do
                                if stackPanel |> self.AddedAdornments.Contains |> not then
                                    layer.AddAdornment(AdornmentPositioningBehavior.OwnerControlled, Nullable(), 
                                        self, stackPanel, AdornmentRemovedCallback(fun _ _ -> ())) |> ignore
                                    self.AddedAdornments.Add stackPanel |> ignore
                        | _ -> ()
                    with e -> logExceptionWithContext (e, "LayoutChanged, processing new visible lines")
            } |> Async.Ignore
    
    override self.AddUiElementToCodeLens (trackingSpan:ITrackingSpan, uiElement:UIElement)=
        base.AddUiElementToCodeLens (trackingSpan, uiElement) // We do the same as the base call execpt that we need to notify that the tag needs to be refreshed.
        tagsChangedEvent.Trigger(self, SnapshotSpanEventArgs(trackingSpan.GetSpan(buffer.CurrentSnapshot)))

    override self.RemoveUiElementFromCodeLens (trackingSpan:ITrackingSpan, uiElement:UIElement) =
        base.RemoveUiElementFromCodeLens (trackingSpan, uiElement)
        tagsChangedEvent.Trigger(self, SnapshotSpanEventArgs(trackingSpan.GetSpan(buffer.CurrentSnapshot))) // Need to refresh the tag.

    interface ITagger<CodeLensGeneralTag> with
        [<CLIEvent>]
        override __.TagsChanged = tagsChangedEvent.Publish

        /// Returns the tags which reserve the correct space for adornments
        /// Notice, it's asumed that the data in the collection is valid.
        override __.GetTags spans =
            try
                seq {
                    for span in spans do
                        let snapshot = span.Snapshot
                        let lineNumber = 
                            try
                                snapshot.GetLineNumberFromPosition(span.Start.Position)
                            with e -> logExceptionWithContext (e, "line number tagging"); 0
                        if self.TrackingSpans.ContainsKey(lineNumber) && self.TrackingSpans.[lineNumber] |> Seq.isEmpty |> not then
                            
                            let tagSpan = snapshot.GetLineFromLineNumber(lineNumber).Extent
                            let stackPanels = 
                                self.TrackingSpans.[lineNumber] 
                                |> Seq.map (fun trackingSpan ->
                                        let success, res = self.UiElements.TryGetValue trackingSpan
                                        if success then res else null
                                    )
                                |> Seq.filter (isNull >> not)
                            let span = 
                                try 
                                    tagSpan.TranslateTo(span.Snapshot, SpanTrackingMode.EdgeExclusive)
                                with e -> logExceptionWithContext (e, "tag span translation"); tagSpan
                            let sizes = 
                                try
                                    stackPanels |> Seq.map (fun ui -> 
                                        ui.Measure(Size(10000., 10000.))
                                        ui.DesiredSize )
                                with e -> logExceptionWithContext (e, "internal tagging"); Seq.empty
                            let height = 
                                try
                                    sizes 
                                    |> Seq.map (fun size -> size.Height) 
                                    |> Seq.sortDescending 
                                    |> Seq.tryHead
                                    |> Option.defaultValue 0.
                                with e -> logExceptionWithContext (e, "height tagging"); 0.
                            
                            yield TagSpan(span, CodeLensGeneralTag(0., height, 0., 0., 0., PositionAffinity.Predecessor, stackPanels, self)) :> ITagSpan<CodeLensGeneralTag>
                }
            with e -> 
                logErrorf "Error in code lens get tags %A" e
                Seq.empty


