// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System
open System.Windows.Controls
open Microsoft.VisualStudio.FSharp.Editor.Logging
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.Text.Formatting
open System.Threading
open System.Windows
open System.Collections.Generic

type CodeLensDisplayService (view : IWpfTextView, buffer : ITextBuffer) as self =

    // Add buffer changed event handler
    do (
        buffer.Changed.Add self.HandleBufferChanged
        view.LayoutChanged.Add self.HandleLayoutChanged
       )
       
    /// <summary>
    /// Enqueing an unit signals to the tagger that all visible line lens must be layouted again,
    /// to respect single line changes.
    /// </summary>
    member val RelayoutRequested : Queue<_> = Queue() with get

    member val WpfView = view

    member val TextBuffer = buffer

    /// Saves the ui context to switch context for ui related work.
    member val UiContext = SynchronizationContext.Current

    // Tracks the created ui elements per TrackingSpan
    member val UiElements = Dictionary<_,Grid>()

    member val TrackingSpanUiParent = HashSet()

    member val UiElementNeighbour = Dictionary()

    /// Caches the current used TrackingSpans per line. One line can contain multiple trackingSpans
    member val TrackingSpans = Dictionary<int, ResizeArray<_>>()

    /// Text view for accessing the adornment layer.
    member val View: IWpfTextView = view

    member val CodeLensLayer = view.GetAdornmentLayer "LineLens"

    /// Tracks the recent first + last visible line numbers for adornment layout logic.
    member val RecentFirstVsblLineNmbr = 0 with get, set

    member val RecentLastVsblLineNmbr = 0 with get, set

    /// Tracks the adornments on the layer.
    member val AddedAdornments = HashSet()

    /// Cancellation token source for the layout changed event. Needed to abort previous async-work.
    member val LayoutChangedCts = new CancellationTokenSource() with get, set

    /// Tracks the last used buffer snapshot, should be preferred used in combination with mutex.
    member val CurrentBufferSnapshot = null with get, set

    /// Helper method which returns the start line number of a tracking span
    member _.GetTrackingSpanStartLine (snapshot:ITextSnapshot) (trackingSpan:ITrackingSpan) =
        snapshot.GetLineNumberFromPosition(trackingSpan.GetStartPoint(snapshot).Position)

    /// Helper method which returns the start line number of a tracking span
    member _.TryGetTSpanStartLine (snapshot:ITextSnapshot) (trackingSpan:ITrackingSpan) =
        let pos = trackingSpan.GetStartPoint(snapshot).Position
        if snapshot.Length - 1 < pos then None
        else pos |> snapshot.GetLineNumberFromPosition |> Some

    member self.UpdateTrackingSpansFast (snapshot:ITextSnapshot) lineNumber =
        if lineNumber |> self.TrackingSpans.ContainsKey then
            let currentTrackingSpans = self.TrackingSpans.[lineNumber] |> ResizeArray // We need a copy because we modify the list.
            for trackingSpan in currentTrackingSpans do
                let newLineOption = self.TryGetTSpanStartLine snapshot trackingSpan
                match newLineOption with 
                | None -> ()
                | Some newLine ->
                    if newLine <> lineNumber then
                        // We're on a new line and need to check whether we're currently in another grid 
                        // (because somehow there were multiple trackingSpans per line).
                        if self.TrackingSpanUiParent.Contains trackingSpan then
                            self.TrackingSpanUiParent.Remove trackingSpan |> ignore
                            self.UiElementNeighbour.Remove self.UiElements.[trackingSpan] |> ignore
                        // remove our entry in the line cache dictionary
                        self.TrackingSpans.[lineNumber].Remove(trackingSpan) |> ignore
                        // if the cache entry for the old line is now empty remove it completely
                        if self.TrackingSpans.[lineNumber].Count = 0 then
                            self.TrackingSpans.Remove lineNumber |> ignore
                        // now re-register our tracking span in the cache dict.
                        // Check whether the new line has no existing entry to add a fresh one.
                        // If there is already one we put our grid into the grid of the first entry of the line.
                        if newLine |> self.TrackingSpans.ContainsKey |> not then
                            self.TrackingSpans.[newLine] <- ResizeArray()
                        else
                            let neighbour = 
                                self.UiElements.[self.TrackingSpans.[newLine] |> Seq.last] // This fails if a tracking span has no ui element!
                            self.UiElementNeighbour.[self.UiElements.[trackingSpan]] <- neighbour
                            self.TrackingSpanUiParent.Add trackingSpan |> ignore
                        // And finally add us to the cache again.
                        self.TrackingSpans.[newLine].Add(trackingSpan)
                        // Be sure that the uiElement of the trackingSpan is viRecentLastVsblLineNmbr the visible line space.
                        if newLine < self.RecentFirstVsblLineNmbr || newLine > self.RecentLastVsblLineNmbr then
                            if self.UiElements.ContainsKey trackingSpan then 
                                let mutable element = self.UiElements.[trackingSpan]
                                element.Visibility <- Visibility.Hidden

    member _.CreateDefaultStackPanel () = 
        let grid = Grid(Visibility = Visibility.Hidden)
        Canvas.SetLeft(grid, 0.)
        Canvas.SetTop(grid, 0.)
        grid

    /// Helper methods which invokes every action which is needed for new trackingSpans
    member self.AddTrackingSpan (trackingSpan:ITrackingSpan)=
        let snapshot = buffer.CurrentSnapshot
        let startLineNumber = snapshot.GetLineNumberFromPosition(trackingSpan.GetStartPoint(snapshot).Position)
        let uiElement = 
            if self.UiElements.ContainsKey trackingSpan then
#if DEBUG
                logErrorf "Added a tracking span twice, this is not allowed and will result in invalid values! %A" (trackingSpan.GetText snapshot)
#endif
                self.UiElements.[trackingSpan]
            else
                let defaultStackPanel = self.CreateDefaultStackPanel()
                self.UiElements.[trackingSpan] <- defaultStackPanel
                defaultStackPanel
        if self.TrackingSpans.ContainsKey startLineNumber then
            self.TrackingSpans.[startLineNumber].Add trackingSpan
            let neighbour = 
                self.UiElements.[self.TrackingSpans.[startLineNumber] |> Seq.last] // This fails if a tracking span has no ui element!
            self.UiElementNeighbour.[uiElement] <- neighbour
            self.TrackingSpanUiParent.Add trackingSpan |> ignore
        else
            self.TrackingSpans.[startLineNumber] <- ResizeArray()
            self.TrackingSpans.[startLineNumber].Add trackingSpan
        uiElement
        

    member self.HandleBufferChanged(e:TextContentChangedEventArgs) =
        try
            let oldSnapshot = e.Before
            let snapshot = e.After
            self.CurrentBufferSnapshot <- snapshot
            for line in oldSnapshot.Lines do
                let lineNumber = line.LineNumber
                self.UpdateTrackingSpansFast snapshot lineNumber
            let firstLine = view.TextViewLines.FirstVisibleLine
            view.DisplayTextLineContainingBufferPosition (firstLine.Start, 0., ViewRelativePosition.Top)
            self.RelayoutRequested.Enqueue(())
         with e ->
#if DEBUG
            logErrorf "Error in line lens provider: %A" e
#else
            ignore e
#endif

    /// Public non-thread-safe method to add line lens for a given tracking span.
    /// Returns an UIElement which can be used to add Ui elements and to remove the line lens later.
    member self.AddCodeLens (trackingSpan:ITrackingSpan) =
        if trackingSpan.TextBuffer <> buffer then failwith "TrackingSpan text buffer does not equal with CodeLens text buffer"
        let Grid = self.AddTrackingSpan trackingSpan
        self.RelayoutRequested.Enqueue(())
        Grid :> UIElement
    
    /// Public non-thread-safe method to remove line lens for a given tracking span.
    member self.RemoveCodeLens (trackingSpan:ITrackingSpan) =
        if self.UiElements.ContainsKey trackingSpan then
            let Grid = self.UiElements.[trackingSpan]
            Grid.Children.Clear()
            self.UiElements.Remove trackingSpan |> ignore
            try
                self.CodeLensLayer.RemoveAdornment(Grid) 
            with e ->
#if DEBUG
                logExceptionWithContext(e, "Removing line lens")
#else
                ignore e
#endif
#if DEBUG
        else
            logWarningf "No ui element is attached to this tracking span!"
#endif
        let lineNumber = 
            (trackingSpan.GetStartPoint self.CurrentBufferSnapshot).Position 
            |> self.CurrentBufferSnapshot.GetLineNumberFromPosition
        if self.TrackingSpans.ContainsKey lineNumber then
#if DEBUG
            if self.TrackingSpans.[lineNumber].Remove trackingSpan |> not then
                logWarningf "No tracking span is accociated with this line number %d!" lineNumber
#endif
            if self.TrackingSpans.[lineNumber].Count = 0 then
                self.TrackingSpans.Remove lineNumber |> ignore
#if DEBUG
        else
            logWarningf "No tracking span is accociated with this line number %d!" lineNumber
#endif

    member self.AddUiElementToCodeLens (trackingSpan:ITrackingSpan, uiElement:UIElement) =
        let Grid = self.UiElements.[trackingSpan]
        Grid.Children.Add uiElement |> ignore

    member self.AddUiElementToCodeLensOnce (trackingSpan:ITrackingSpan, uiElement:UIElement)=
        let Grid = self.UiElements.[trackingSpan]
        if uiElement |> Grid.Children.Contains |> not then
            self.AddUiElementToCodeLens (trackingSpan, uiElement)

    member self.RemoveUiElementFromCodeLens (trackingSpan:ITrackingSpan, uiElement:UIElement) =
        let Grid = self.UiElements.[trackingSpan]
        Grid.Children.Remove(uiElement) |> ignore
    
    member self.HandleLayoutChanged (e:TextViewLayoutChangedEventArgs) =
        try
            // We can cancel existing stuff because the algorithm supports abortion without any data loss
            self.LayoutChangedCts.Cancel()
            self.LayoutChangedCts.Dispose()
            self.LayoutChangedCts <- new CancellationTokenSource()
            let buffer = e.NewSnapshot
            let recentVisibleLineNumbers = Set [self.RecentFirstVsblLineNmbr .. self.RecentLastVsblLineNmbr]
            let firstVisibleLineNumber, lastVisibleLineNumber =
                let first, last = 
                    view.TextViewLines.FirstVisibleLine, 
                    view.TextViewLines.LastVisibleLine
                buffer.GetLineNumberFromPosition(first.Start.Position),
                buffer.GetLineNumberFromPosition(last.Start.Position)
            let visibleLineNumbers = Set [firstVisibleLineNumber .. lastVisibleLineNumber]
            let nonVisibleLineNumbers = Set.difference recentVisibleLineNumbers visibleLineNumbers
            let newVisibleLineNumbers = Set.difference visibleLineNumbers recentVisibleLineNumbers
        
            let applyFuncOnLineStackPanels (line:IWpfTextViewLine) (func:Grid -> unit) =
                let lineNumber = line.Snapshot.GetLineNumberFromPosition(line.Start.Position)
                if (self.TrackingSpans.ContainsKey lineNumber) && (self.TrackingSpans.[lineNumber]) |> (Seq.isEmpty >> not) then
                    for trackingSpan in self.TrackingSpans.[lineNumber] do
                        let success, ui = self.UiElements.TryGetValue trackingSpan
                        if success then 
                            func ui

            if nonVisibleLineNumbers.Count > 0 || newVisibleLineNumbers.Count > 0 then
                for lineNumber in nonVisibleLineNumbers do
                    if lineNumber > 0 && lineNumber < buffer.LineCount then
                        try
                            let line = 
                                (buffer.GetLineFromLineNumber lineNumber).Start
                                |> view.GetTextViewLineContainingBufferPosition
                            applyFuncOnLineStackPanels line (fun ui ->
                                ui.Visibility <- Visibility.Hidden
                            )
                        with e ->
#if DEBUG
                            logErrorf "Error in non visible lines iteration %A" e
#else
                            ignore e
#endif
                for lineNumber in newVisibleLineNumbers do
                    try
                        let line = 
                                (buffer.GetLineFromLineNumber lineNumber).Start
                                |> view.GetTextViewLineContainingBufferPosition
                        applyFuncOnLineStackPanels line (fun ui ->
                            ui.Visibility <- Visibility.Visible
                            self.LayoutUIElementOnLine view line ui
                        )
                     with e ->
#if DEBUG
                        logErrorf "Error in new visible lines iteration %A" e
#else
                        ignore e
#endif
            if not e.VerticalTranslation && e.NewViewState.ViewportHeight <> e.OldViewState.ViewportHeight then
                self.RelayoutRequested.Enqueue() // Unfortunately zooming requires a relayout too, to ensure that no weird layout happens due to unkown reasons.
            if self.RelayoutRequested.Count > 0 then
                self.RelayoutRequested.Dequeue() |> ignore
                for lineNumber in visibleLineNumbers do
                    let line = 
                        (buffer.GetLineFromLineNumber lineNumber).Start
                        |> view.GetTextViewLineContainingBufferPosition
                    applyFuncOnLineStackPanels line (fun ui ->
                        ui.Visibility <- Visibility.Visible
                        self.LayoutUIElementOnLine view line ui
                    )
            // Save the new first and last visible lines for tracking
            self.RecentFirstVsblLineNmbr <- firstVisibleLineNumber
            self.RecentLastVsblLineNmbr <- lastVisibleLineNumber

            self.AsyncCustomLayoutOperation visibleLineNumbers buffer
            |> RoslynHelpers.StartAsyncSafe self.LayoutChangedCts.Token "HandleLayoutChanged"
        with e ->
#if DEBUG
            logExceptionWithContext (e, "Layout changed")
#else
            ignore e
#endif

    /// Layouts all stack panels on the line
    member self.LayoutUIElementOnLine _ (line:ITextViewLine) (ui:Grid) =
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
    
    member self.AsyncCustomLayoutOperation visibleLineNumbers buffer =
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