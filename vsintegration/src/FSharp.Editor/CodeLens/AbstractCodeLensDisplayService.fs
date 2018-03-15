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
        buffer.Changed.Add self.handleBufferChanged
        view.LayoutChanged.Add self.handleLayoutChanged
       )
    
    /// <summary>
    /// Enqueing an unit signals to the tagger that all visible line lens must be layouted again,
    /// to respect single line changes.
    /// </summary>
    member val RelayoutRequested : Queue<_> = Queue() with get

    member val WpfView = view

    member val TextBuffer = buffer

    /// Saves the ui context to switch context for ui related work.
    member val uiContext = SynchronizationContext.Current

    // Tracks the created ui elements per TrackingSpan
    member val uiElements = Dictionary<_,Grid>()

    member val trackingSpanUiParent = HashSet()

    member val uiElementNeighbour = Dictionary()

    /// Caches the current used trackingSpans per line. One line can contain multiple trackingSpans
    member val trackingSpans = Dictionary<int, ResizeArray<_>>()
    /// Text view for accessing the adornment layer.
    member val view: IWpfTextView = view
    /// The code lens layer for adding and removing adornments.
    member val codeLensLayer = view.GetAdornmentLayer layerName
    /// Tracks the recent first + last visible line numbers for adornment layout logic.
    member val recentFirstVsblLineNmbr = 0 with get, set

    member val recentLastVsblLineNmbr = 0 with get, set
    /// Tracks the adornments on the layer.
    member val addedAdornments = HashSet()
    /// Cancellation token source for the layout changed event. Needed to abort previous async-work.
    member val layoutChangedCts = new CancellationTokenSource() with get, set

    /// Tracks the last used buffer snapshot, should be preferred used in combination with mutex.
    member val currentBufferSnapshot = null with get, set

    /// Helper method which returns the start line number of a tracking span
    member __.getTrackingSpanStartLine (snapshot:ITextSnapshot) (trackingSpan:ITrackingSpan) =
        snapshot.GetLineNumberFromPosition(trackingSpan.GetStartPoint(snapshot).Position)

    /// Helper method which returns the start line number of a tracking span
    member __.tryGetTSpanStartLine (snapshot:ITextSnapshot) (trackingSpan:ITrackingSpan) =
        let pos = trackingSpan.GetStartPoint(snapshot).Position
        if snapshot.Length - 1 < pos then None
        else pos |> snapshot.GetLineNumberFromPosition |> Some
    
    member self.updateTrackingSpansFast (snapshot:ITextSnapshot) lineNumber =
        if lineNumber |> self.trackingSpans.ContainsKey then
            let currentTrackingSpans = self.trackingSpans.[lineNumber] |> ResizeArray // We need a copy because we modify the list.
            for trackingSpan in currentTrackingSpans do
                let newLineOption = self.tryGetTSpanStartLine snapshot trackingSpan
                match newLineOption with 
                | None -> ()
                | Some newLine ->
                    if newLine <> lineNumber then
                        // We're on a new line and need to check whether we're currently in another grid 
                        // (because somehow there were multiple trackingSpans per line).
                        if self.trackingSpanUiParent.Contains trackingSpan then
                            self.trackingSpanUiParent.Remove trackingSpan |> ignore
                            self.uiElementNeighbour.Remove self.uiElements.[trackingSpan] |> ignore
                        // remove our entry in the line cache dictionary
                        self.trackingSpans.[lineNumber].Remove(trackingSpan) |> ignore
                        // if the cache entry for the old line is now empty remove it completely
                        if self.trackingSpans.[lineNumber].Count = 0 then
                            self.trackingSpans.Remove lineNumber |> ignore
                        // now re-register our tracking span in the cache dict.
                        // Check whether the new line has no existing entry to add a fresh one.
                        // If there is already one we put our grid into the grid of the first entry of the line.
                        if newLine |> self.trackingSpans.ContainsKey |> not then
                            self.trackingSpans.[newLine] <- ResizeArray()
                        else
                            let neighbour = 
                                self.uiElements.[self.trackingSpans.[newLine] |> Seq.last] // This fails if a tracking span has no ui element!
                            self.uiElementNeighbour.[self.uiElements.[trackingSpan]] <- neighbour
                            self.trackingSpanUiParent.Add trackingSpan |> ignore
                        // And finally add us to the cache again.
                        self.trackingSpans.[newLine].Add(trackingSpan)
                        // Be sure that the uiElement of the trackingSpan is visible if the new line is in the visible line space.
                        if newLine < self.recentFirstVsblLineNmbr || newLine > self.recentLastVsblLineNmbr then
                            if self.uiElements.ContainsKey trackingSpan then 
                                let mutable element = self.uiElements.[trackingSpan]
                                element.Visibility <- Visibility.Hidden

    member __.createDefaultStackPanel () = 
        let grid = Grid(Visibility = Visibility.Hidden)
        Canvas.SetLeft(grid, 0.)
        Canvas.SetTop(grid, 0.)
        grid

    /// Helper methods which invokes every action which is needed for new trackingSpans
    member self.addTrackingSpan (trackingSpan:ITrackingSpan)=
        let snapshot = buffer.CurrentSnapshot
        let startLineNumber = snapshot.GetLineNumberFromPosition(trackingSpan.GetStartPoint(snapshot).Position)
        let uiElement = 
            if self.uiElements.ContainsKey trackingSpan then
                logErrorf "Added a tracking span twice, this is not allowed and will result in invalid values! %A" (trackingSpan.GetText snapshot)
                self.uiElements.[trackingSpan]
            else
                let defaultStackPanel = self.createDefaultStackPanel()
                self.uiElements.[trackingSpan] <- defaultStackPanel
                defaultStackPanel
        if self.trackingSpans.ContainsKey startLineNumber then
            self.trackingSpans.[startLineNumber].Add trackingSpan
            let neighbour = 
                self.uiElements.[self.trackingSpans.[startLineNumber] |> Seq.last] // This fails if a tracking span has no ui element!
            self.uiElementNeighbour.[uiElement] <- neighbour
            self.trackingSpanUiParent.Add trackingSpan |> ignore
        else
            self.trackingSpans.[startLineNumber] <- ResizeArray()
            self.trackingSpans.[startLineNumber].Add trackingSpan
        uiElement
        

    member self.handleBufferChanged(e:TextContentChangedEventArgs) =
        try
            let oldSnapshot = e.Before
            let snapshot = e.After
            self.currentBufferSnapshot <- snapshot
            for line in oldSnapshot.Lines do
                let lineNumber = line.LineNumber
                self.updateTrackingSpansFast snapshot lineNumber
            let firstLine = view.TextViewLines.FirstVisibleLine
            view.DisplayTextLineContainingBufferPosition (firstLine.Start, 0., ViewRelativePosition.Top)
            self.RelayoutRequested.Enqueue(())
         with e -> logErrorf "Error in line lens provider: %A" e
    
    /// Public non-thread-safe method to add line lens for a given tracking span.
    /// Returns an UIElement which can be used to add Ui elements and to remove the line lens later.

    member self.AddCodeLens (trackingSpan:ITrackingSpan) =
        if trackingSpan.TextBuffer <> buffer then failwith "TrackingSpan text buffer does not equal with CodeLens text buffer"
        let Grid = self.addTrackingSpan trackingSpan
        self.RelayoutRequested.Enqueue(())
        Grid :> UIElement
    
    /// Public non-thread-safe method to remove line lens for a given tracking span.
    member self.RemoveCodeLens (trackingSpan:ITrackingSpan) =
        if self.uiElements.ContainsKey trackingSpan then
            let Grid = self.uiElements.[trackingSpan]
            Grid.Children.Clear()
            self.uiElements.Remove trackingSpan |> ignore
            try
                self.codeLensLayer.RemoveAdornment(Grid) 
            with e -> 
                logExceptionWithContext(e, "Removing line lens")
        else
            logWarningf "No ui element is attached to this tracking span!"
        let lineNumber = 
            (trackingSpan.GetStartPoint self.currentBufferSnapshot).Position 
            |> self.currentBufferSnapshot.GetLineNumberFromPosition
        if self.trackingSpans.ContainsKey lineNumber then
            if self.trackingSpans.[lineNumber].Remove trackingSpan |> not then
                logWarningf "No tracking span is accociated with this line number %d!" lineNumber
            if self.trackingSpans.[lineNumber].Count = 0 then
                self.trackingSpans.Remove lineNumber |> ignore
        else
            logWarningf "No tracking span is accociated with this line number %d!" lineNumber

    abstract member AddUiElementToCodeLens : ITrackingSpan * UIElement -> unit
    default self.AddUiElementToCodeLens (trackingSpan:ITrackingSpan, uiElement:UIElement) =
        let Grid = self.uiElements.[trackingSpan]
        Grid.Children.Add uiElement |> ignore

    abstract member AddUiElementToCodeLensOnce : ITrackingSpan * UIElement -> unit
    default self.AddUiElementToCodeLensOnce (trackingSpan:ITrackingSpan, uiElement:UIElement)=
        let Grid = self.uiElements.[trackingSpan]
        if uiElement |> Grid.Children.Contains |> not then
            self.AddUiElementToCodeLens (trackingSpan, uiElement)

    abstract member RemoveUiElementFromCodeLens : ITrackingSpan * UIElement -> unit
    default self.RemoveUiElementFromCodeLens (trackingSpan:ITrackingSpan, uiElement:UIElement) =
        let Grid = self.uiElements.[trackingSpan]
        Grid.Children.Remove(uiElement) |> ignore
    
     member self.handleLayoutChanged (e:TextViewLayoutChangedEventArgs) =
        try
            let buffer = e.NewSnapshot
            let recentVisibleLineNumbers = Set [self.recentFirstVsblLineNmbr .. self.recentLastVsblLineNmbr]
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
                if (self.trackingSpans.ContainsKey lineNumber) && (self.trackingSpans.[lineNumber]) |> (Seq.isEmpty >> not) then
                    for trackingSpan in self.trackingSpans.[lineNumber] do
                        let success, ui = self.uiElements.TryGetValue trackingSpan
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
                            self.layoutUIElementOnLine view line ui
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
                        self.layoutUIElementOnLine view line ui
                    )
            // Save the new first and last visible lines for tracking
            self.recentFirstVsblLineNmbr <- firstVisibleLineNumber
            self.recentLastVsblLineNmbr <- lastVisibleLineNumber
            // We can cancel existing stuff because the algorithm supports abortion without any data loss
            self.layoutChangedCts.Cancel()
            self.layoutChangedCts.Dispose()
            self.layoutChangedCts <- new CancellationTokenSource()

            self.asyncCustomLayoutOperation visibleLineNumbers buffer
            |> RoslynHelpers.StartAsyncSafe self.layoutChangedCts.Token
        with e -> logExceptionWithContext (e, "Layout changed")

    abstract layoutUIElementOnLine : IWpfTextView -> ITextViewLine -> Grid -> unit

    abstract asyncCustomLayoutOperation : int Set -> ITextSnapshot -> unit Async



