// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor


open System
open System.Windows.Controls
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.Text.Formatting
open System.Windows
open System.Collections.Generic

open Microsoft.VisualStudio.FSharp.Editor.Logging

type internal LineLensDisplayService (view, buffer) =
    inherit CodeLensDisplayService(view, buffer, "LineLens")
    
    /// Layouts all stack panels on the line
    override self.layoutUIElementOnLine _ (line:ITextViewLine) (ui:Grid) =
        let left, top = 
            match self.uiElementNeighbour.TryGetValue ui with
            | true, parent -> 
                let left = Canvas.GetLeft parent
                let top = Canvas.GetTop parent
                let width = parent.ActualWidth
                left + width, top
            | _ -> 
                try
                    let bounds = line.GetCharacterBounds(line.Start)
                    line.TextRight + 5.0, bounds.Top - 1.
                with e -> 
                    logExceptionWithContext (e, "Error in layout ui element on line")
                    Canvas.GetLeft ui, Canvas.GetTop ui
        Canvas.SetLeft(ui, left)
        Canvas.SetTop(ui, top)

    override self.asyncCustomLayoutOperation visibleLineNumbers buffer =
        asyncMaybe {
            // Suspend 5 ms, instantly applying the layout to the adornment elements isn't needed 
            // and would consume too much performance
            do! Async.Sleep(5) |> liftAsync // Skip at least one frames
            do! Async.SwitchToContext self.uiContext |> liftAsync
            let layer = self.codeLensLayer
            do! Async.Sleep(495) |> liftAsync
            try
                for visibleLineNumber in visibleLineNumbers do
                    if self.trackingSpans.ContainsKey visibleLineNumber then
                        self.trackingSpans.[visibleLineNumber] 
                        |> Seq.map (fun trackingSpan ->
                                let success, res = self.uiElements.TryGetValue trackingSpan
                                if success then 
                                    res 
                                else null
                            )
                        |> Seq.filter (fun ui -> not(isNull ui) && not(self.addedAdornments.Contains ui))
                        |> Seq.iter(fun grid ->
                                layer.AddAdornment(AdornmentPositioningBehavior.OwnerControlled, Nullable(), 
                                    self, grid, AdornmentRemovedCallback(fun _ _ -> self.addedAdornments.Remove grid |> ignore)) |> ignore
                                self.addedAdornments.Add grid |> ignore
                                let line = 
                                    let l = buffer.GetLineFromLineNumber visibleLineNumber
                                    view.GetTextViewLineContainingBufferPosition l.Start
                                self.layoutUIElementOnLine view line grid
                            )
            with e -> logExceptionWithContext (e, "LayoutChanged, processing new visible lines")
        } |> Async.Ignore