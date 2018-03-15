// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System
open System.Windows.Controls
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.Text.Formatting
open System.Threading
open System.Windows
open System.Collections.Generic

open Microsoft.VisualStudio.FSharp.Editor.Logging

[<AbstractClass>]
type CodeLensDisplayService (view : IWpfTextView, buffer : ITextBuffer, layerName) as self =

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

    member val CodeLensLayer = view.GetAdornmentLayer layerName

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
    member __.GetTrackingSpanStartLine (snapshot:ITextSnapshot) (trackingSpan:ITrackingSpan) =
        snapshot.GetLineNumberFromPosition(trackingSpan.GetStartPoint(snapshot).Position)

    /// Helper method which returns the start line number of a tracking span
    member __.TryGetTSpanStartLine (snapshot:ITextSnapshot) (trackingSpan:ITrackingSpan) =
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

    member __.CreateDefaultStackPanel () = 
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
                logErrorf "Added a tracking span twice, this is not allowed and will result in invalid values! %A" (trackingSpan.GetText snapshot)
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
         with e -> logErrorf "Error in line lens provider: %A" e
    
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
                logExceptionWithContext(e, "Removing line lens")
        else
            logWarningf "No ui element is attached to this tracking span!"
        let lineNumber = 
            (trackingSpan.GetStartPoint self.CurrentBufferSnapshot).Position 
            |> self.CurrentBufferSnapshot.GetLineNumberFromPosition
        if self.TrackingSpans.ContainsKey lineNumber then
            if self.TrackingSpans.[lineNumber].Remove trackingSpan |> not then
                logWarningf "No tracking span is accociated with this line number %d!" lineNumber
            if self.TrackingSpans.[lineNumber].Count = 0 then
                self.TrackingSpans.Remove lineNumber |> ignore
        else
            logWarningf "No tracking span is accociated with this line number %d!" lineNumber

    abstract member AddUiElementToCodeLens : ITrackingSpan * UIElement -> unit
    default self.AddUiElementToCodeLens (trackingSpan:ITrackingSpan, uiElement:UIElement) =
        let Grid = self.UiElements.[trackingSpan]
        Grid.Children.Add uiElement |> ignore

    abstract member AddUiElementToCodeLensOnce : ITrackingSpan * UIElement -> unit
    default self.AddUiElementToCodeLensOnce (trackingSpan:ITrackingSpan, uiElement:UIElement)=
        let Grid = self.UiElements.[trackingSpan]
        if uiElement |> Grid.Children.Contains |> not then
            self.AddUiElementToCodeLens (trackingSpan, uiElement)

    abstract member RemoveUiElementFromCodeLens : ITrackingSpan * UIElement -> unit
    default self.RemoveUiElementFromCodeLens (trackingSpan:ITrackingSpan, uiElement:UIElement) =
        let Grid = self.UiElements.[trackingSpan]
        Grid.Children.Remove(uiElement) |> ignore
    
     member self.HandleLayoutChanged (e:TextViewLayoutChangedEventArgs) =
        try
            let buffer = e.NewSnapshot
            let recentVisibleLineNumbers = Set [self.RecentLastVsblLineNmbr .. self.RecentLastVsblLineNmbr]
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
                        with e -> logErrorf "Error in non visible lines iteration %A" e
                for lineNumber in newVisibleLineNumbers do
                    try
                        let line = 
                                (buffer.GetLineFromLineNumber lineNumber).Start
                                |> view.GetTextViewLineContainingBufferPosition
                        applyFuncOnLineStackPanels line (fun ui ->
                            ui.Visibility <- Visibility.Visible
                            self.LayoutUIElementOnLine view line ui
                        )
                     with e -> logErrorf "Error in new visible lines iteration %A" e
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
            // We can cancel existing stuff because the algorithm supports abortion without any data loss
            self.LayoutChangedCts.Cancel()
            self.LayoutChangedCts.Dispose()
            self.LayoutChangedCts <- new CancellationTokenSource()

            self.AsyncCustomLayoutOperation visibleLineNumbers buffer
            |> RoslynHelpers.StartAsyncSafe self.LayoutChangedCts.Token "HandleLayoutChanged"
        with e -> logExceptionWithContext (e, "Layout changed")

    abstract LayoutUIElementOnLine : IWpfTextView -> ITextViewLine -> Grid -> unit

    abstract AsyncCustomLayoutOperation : int Set -> ITextSnapshot -> unit Async
