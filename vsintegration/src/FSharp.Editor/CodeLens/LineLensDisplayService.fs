// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor


open System
open System.Windows.Controls
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.Text.Formatting

open Microsoft.VisualStudio.FSharp.Editor.Logging

type internal LineLensDisplayService (view, buffer) =
    inherit CodeLensDisplayService(view, buffer, "LineLens")
    
    /// Layouts all stack panels on the line
    override self.LayoutUIElementOnLine _ (line:ITextViewLine) (ui:Grid) =
        let left, top = 
            try
                let bounds = line.GetCharacterBounds(line.Start)
                line.TextRight + 5.0, bounds.Top - 1.
            with e ->
#if DEBUG
                logExceptionWithContext (e, "Error in layout ui element on line")
#else
                ignore e
#endif
                Canvas.GetLeft ui, Canvas.GetTop ui
        Canvas.SetLeft(ui, left)
        Canvas.SetTop(ui, top)

    override self.AsyncCustomLayoutOperation visibleLineNumbers buffer =
        asyncMaybe {
            // Suspend 5 ms, instantly applying the layout to the adornment elements isn't needed 
            // and would consume too much performance
            do! Async.Sleep(5) |> liftAsync // Skip at least one frames
            do! Async.SwitchToContext self.UiContext |> liftAsync
            let layer = self.CodeLensLayer
            do! Async.Sleep(495) |> liftAsync
            try
                for visibleLineNumber in visibleLineNumbers do
                    if self.TrackingSpans.ContainsKey visibleLineNumber then
                        self.TrackingSpans.[visibleLineNumber] 
                        |> Seq.map (fun trackingSpan ->
                                let success, res = self.UiElements.TryGetValue trackingSpan
                                if success then 
                                    res 
                                else null
                            )
                        |> Seq.filter (fun ui -> not(isNull ui) && not(self.AddedAdornments.Contains ui))
                        |> Seq.iter(fun grid ->
                                layer.AddAdornment(AdornmentPositioningBehavior.OwnerControlled, Nullable(), 
                                    self, grid, AdornmentRemovedCallback(fun _ _ -> self.AddedAdornments.Remove grid |> ignore)) |> ignore
                                self.AddedAdornments.Add grid |> ignore
                                let line = 
                                    let l = buffer.GetLineFromLineNumber visibleLineNumber
                                    view.GetTextViewLineContainingBufferPosition l.Start
                                self.LayoutUIElementOnLine view line grid
                            )
            with e ->
#if DEBUG
                logExceptionWithContext (e, "LayoutChanged, processing new visible lines")
#else
                ignore e
#endif
        } |> Async.Ignore