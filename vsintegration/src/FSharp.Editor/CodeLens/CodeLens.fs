// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System
open System.Windows.Controls
open System.Windows.Media
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.Text.Formatting
open System.ComponentModel.Composition
open Microsoft.VisualStudio.Utilities
open Microsoft.CodeAnalysis
open System.Threading
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio
open Microsoft.VisualStudio.LanguageServices
open System.Windows
open System.Collections.Generic
open Microsoft.FSharp.Compiler.Range
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.Ast
open Microsoft.CodeAnalysis.Editor.Shared.Extensions
open Microsoft.CodeAnalysis.Editor.Shared.Utilities
open Microsoft.CodeAnalysis.Classification
open Internal.Utilities.StructuredFormat
open Microsoft.VisualStudio.Text.Tagging
open System.Collections.Concurrent
open System.Collections
open System.Windows.Media.Animation
open System.Globalization

open Microsoft.VisualStudio.FSharp.Editor.Logging
open Microsoft.CodeAnalysis.Text
open System
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.Text

[<AbstractClass>]
type CodeLensDisplayService (view : IWpfTextView, buffer : ITextBuffer) =

    /// <summary>
    /// Enqueing an unit signals to the tagger that all visible line lens must be layouted again,
    /// to respect single line changes.
    /// </summary>
    member val RelayoutRequested : Queue = Queue() with get

    member __.WpfView = view

    member __.TextBuffer = buffer
    
    /// Public non-thread-safe method to add line lens for a given tracking span.
    /// Returns an UIElement which can be used to add Ui elements and to remove the line lens later.
    abstract AddCodeLens : ITrackingSpan -> UIElement
    
    /// Public non-thread-safe method to remove line lens for a given tracking span.
    /// Returns whether the operation succeeded
    abstract RemoveCodeLens : ITrackingSpan -> unit

    abstract AddUiElementToCodeLens : ITrackingSpan -> UIElement -> unit
    
    abstract AddUiElementToCodeLensOnce : ITrackingSpan -> UIElement -> unit

    abstract RemoveUiElementFromCodeLens : ITrackingSpan -> UIElement -> unit

type CodeLensGeneralTag(width, topSpace, baseline, textHeight, bottomSpace, affinity, tag:obj, providerTag:obj) =
    inherit SpaceNegotiatingAdornmentTag(width, topSpace, baseline, textHeight, bottomSpace, affinity, tag, providerTag)

/// Class which provides support for general code lens
/// Use the methods <code>AddCodeLens</code< and <code> RemoveCodeLens</code>
type CodeLensGeneralTagger (view, buffer) as self =
    inherit CodeLensDisplayService(view, buffer)

    /// Saves the ui context to switch context for ui related work.
    let uiContext = SynchronizationContext.Current

    /// The tags changed event to notify if the data for the tags has changed.
    let tagsChangedEvent = new Event<EventHandler<SnapshotSpanEventArgs>,SnapshotSpanEventArgs>()

    // Tracks the created ui elements per TrackingSpan
    let uiElements = Dictionary<_,StackPanel>()
    /// Caches the current used trackingSpans per line. One line can contain multiple trackingSpans
    let mutable trackingSpans = Dictionary<_, Generic.List<_>>()
    /// Text view for accessing the adornment layer.
    let mutable view: IWpfTextView = view
    /// The code lens layer for adding and removing adornments.
    let mutable codeLensLayer: IAdornmentLayer = view.GetAdornmentLayer "CodeLens"
    /// Tracks the recent first + last visible line numbers for adornment layout logic.
    let mutable recentFirstVsblLineNmbr, recentLastVsblLineNmbr = 0, 0
    /// Tracks the adornments on the layer.
    let mutable addedAdornments = HashSet()
    /// Cancellation token source for the layout changed event. Needed to abort previous async-work.
    let mutable layoutChangedCts = new CancellationTokenSource()

    /// Tracks the last used buffer snapshot, should be preferred used in combination with mutex.
    let mutable currentBufferSnapshot = null

    /// Helper method which returns the start line number of a tracking span
    let getTrackingSpanStartLine (snapshot:ITextSnapshot) (trackingSpan:ITrackingSpan) =
        snapshot.GetLineNumberFromPosition(trackingSpan.GetStartPoint(snapshot).Position)

    /// Helper method which returns the start line number of a tracking span
    let tryGetTSpanStartLine (snapshot:ITextSnapshot) (trackingSpan:ITrackingSpan) =
        let pos = trackingSpan.GetStartPoint(snapshot).Position
        if snapshot.Length - 1 < pos then None
        else pos |> snapshot.GetLineNumberFromPosition |> Some
    
    let updateTrackingSpansFast (snapshot:ITextSnapshot) lineNumber =
        if lineNumber |> trackingSpans.ContainsKey then
            let currentTrackingSpans = Generic.List(trackingSpans.[lineNumber])
            for trackingSpan in currentTrackingSpans do
                let newLineOption = tryGetTSpanStartLine snapshot trackingSpan
                match newLineOption with 
                | None -> ()
                | Some newLine ->
                    if newLine <> lineNumber then
                        trackingSpans.[lineNumber].Remove(trackingSpan) |> ignore
                        if trackingSpans.[lineNumber].Count = 0 then
                            trackingSpans.Remove lineNumber |> ignore
                        if newLine |> trackingSpans.ContainsKey |> not then
                            trackingSpans.[newLine] <- Generic.List()
                        trackingSpans.[newLine].Add(trackingSpan)
                        if newLine < recentFirstVsblLineNmbr || newLine > recentLastVsblLineNmbr then
                            if uiElements.ContainsKey trackingSpan then 
                                let mutable element = uiElements.[trackingSpan]
                                element.Visibility <- Visibility.Hidden
                        // tagsChangedEvent.Trigger(self, SnapshotSpanEventArgs(trackingSpan.GetSpan snapshot)) // This results in super annoying blinking

    let createDefaultStackPanel () = 
        let uiElement = new StackPanel()
        uiElement.Visibility <- Visibility.Hidden
        uiElement

    /// Helper methods which invokes every action which is needed for new trackingSpans
    let addTrackingSpan trackingSpan =
        let snapshot = buffer.CurrentSnapshot
        let startLineNumber = getTrackingSpanStartLine snapshot trackingSpan
        if trackingSpans.ContainsKey startLineNumber then
            trackingSpans.[startLineNumber].Add trackingSpan
        else
            trackingSpans.[startLineNumber] <- Generic.List()
            trackingSpans.[startLineNumber].Add trackingSpan
        let defaultStackPanel = createDefaultStackPanel ()
        uiElements.[trackingSpan] <- defaultStackPanel
        tagsChangedEvent.Trigger(self, trackingSpan.GetSpan snapshot |> SnapshotSpanEventArgs)
        defaultStackPanel
    
    /// Layouts all stack panels on the line
    let layoutUIElementOnLine (view:IWpfTextView) (line:SnapshotSpan) (ui:UIElement) =
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
                Canvas.SetTop(ui, g.Top)
                logErrorf "Incorrect return from geometry measure"
            else
                Canvas.SetLeft(ui, g.Left)
                Canvas.SetTop(ui, g.Top)
        with e -> logExceptionWithContext (e, "Error in layout ui element on line")

    // We update all content of our cache system with this method here
    let handleBufferChanged(e:TextContentChangedEventArgs) =
        try
            let oldSnapshot = e.Before
            let snapshot = e.After
            currentBufferSnapshot <- snapshot
            for line in oldSnapshot.Lines do
                let lineNumber = line.LineNumber
                updateTrackingSpansFast snapshot lineNumber
            //for change in e.Changes do
            //    if change.LineCountDelta > 0 then
            //        let span = change.OldSpan
            //        let startLineNumber, endLineNumber = 
            //            snapshot.GetLineNumberFromPosition span.Start,
            //            snapshot.GetLineNumberFromPosition span.End
            //        for lineNumber in startLineNumber .. endLineNumber do updateTrackingSpansFast snapshot lineNumber
            let firstLine = view.TextViewLines.FirstVisibleLine
            view.DisplayTextLineContainingBufferPosition (firstLine.Start, 0., ViewRelativePosition.Top)
            self.RelayoutRequested.Enqueue(())
         with e -> logErrorf "Error in code lens provider: %A" e

    /// Here all layout methods for the adornments is done.
    /// Adornments which aren't in the current visible spans anymore are hidden,
    /// others which intersect the current visible span are made visible and afterwards async layouted.
    let handleLayoutChanged (e:TextViewLayoutChangedEventArgs) =
        try
            let buffer = e.NewSnapshot
            let recentVisibleLineNumbers = Set [recentFirstVsblLineNmbr .. recentLastVsblLineNmbr]
            let firstVisibleLineNumber, lastVisibleLineNumber =
                let first, last = 
                    view.TextViewLines.FirstVisibleLine, 
                    view.TextViewLines.LastVisibleLine
                buffer.GetLineNumberFromPosition(first.Start.Position),
                buffer.GetLineNumberFromPosition(last.Start.Position)
            let visibleLineNumbers = Set [firstVisibleLineNumber .. lastVisibleLineNumber]
            let nonVisibleLineNumbers = Set.difference recentVisibleLineNumbers visibleLineNumbers
            let newVisibleLineNumbers = Set.difference visibleLineNumbers recentVisibleLineNumbers
        
            let applyFuncOnLineStackPanels (line:IWpfTextViewLine) (func:StackPanel -> unit) =
                let lineNumber = line.Snapshot.GetLineNumberFromPosition(line.Start.Position)
                if trackingSpans.ContainsKey(lineNumber) && trackingSpans.[lineNumber] |> (Seq.isEmpty >> not) then
                    let stackPanels = 
                        trackingSpans.[lineNumber] 
                        |> Seq.map (fun trackingSpan ->
                                let success, res = uiElements.TryGetValue trackingSpan
                                if success then res else null
                            )
                    stackPanels |> Seq.iter (fun ui -> if ui <> null then func ui)

            // logInfof "nonVisibleLineNumbers = %A\newVisibleLineNumbers %A" nonVisibleLineNumbers newVisibleLineNumbers
            if nonVisibleLineNumbers.Count > 0 || newVisibleLineNumbers.Count > 0 then
                for lineNumber in nonVisibleLineNumbers do
                    if lineNumber > 0 && lineNumber < buffer.LineCount then
                        try
                            let line = 
                                let l = buffer.GetLineFromLineNumber(lineNumber)
                                view.GetTextViewLineContainingBufferPosition(l.Start)
                            applyFuncOnLineStackPanels line (fun ui ->
                                ui.Visibility <- Visibility.Hidden
                            )
                        with e -> logErrorf "Error in non visible lines iteration %A" e
                for lineNumber in newVisibleLineNumbers do
                    try
                        let line = 
                                let l = buffer.GetLineFromLineNumber(lineNumber)
                                view.GetTextViewLineContainingBufferPosition(l.Start)
                        applyFuncOnLineStackPanels line (fun ui ->
                            ui.Visibility <- Visibility.Visible
                            layoutUIElementOnLine view line.Extent ui
                        )
                     with e -> logErrorf "Error in new visible lines iteration %A" e
            if self.RelayoutRequested.Count > 0 then
                self.RelayoutRequested.Dequeue() |> ignore
                for lineNumber in visibleLineNumbers do
                    let line = 
                        let l = buffer.GetLineFromLineNumber(lineNumber)
                        view.GetTextViewLineContainingBufferPosition(l.Start)
                    applyFuncOnLineStackPanels line (fun ui ->
                        ui.Visibility <- Visibility.Visible
                        layoutUIElementOnLine view line.Extent ui
                    )
            // Save the new first and last visible lines for tracking
            recentFirstVsblLineNmbr <- firstVisibleLineNumber
            recentLastVsblLineNmbr <- lastVisibleLineNumber
            // We can cancel existing stuff because the algorithm supports abortion without any data loss
            layoutChangedCts.Cancel()
            layoutChangedCts.Dispose()
            layoutChangedCts <- new CancellationTokenSource()
            asyncMaybe {
                // Suspend 16 ms, instantly applying the layout to the adornment elements isn't needed 
                // and would consume too much performance
                do! Async.Sleep(16) |> liftAsync // Skip at least one frames
                do! Async.SwitchToContext uiContext |> liftAsync
                let layer = codeLensLayer
                // Now relayout the existing adornments
                if nonVisibleLineNumbers.Count > 0 || newVisibleLineNumbers.Count > 0 then
                    for lineNumber in visibleLineNumbers do
                        let line = 
                            let l = buffer.GetLineFromLineNumber(lineNumber)
                            view.GetTextViewLineContainingBufferPosition(l.Start)
                        applyFuncOnLineStackPanels line (fun ui ->
                            ui.Visibility <- Visibility.Visible
                            layoutUIElementOnLine view line.Extent ui
                        )
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
                        | Some (:? seq<StackPanel> as stackPanels) ->
                            for stackPanel in stackPanels do
                                if stackPanel |> addedAdornments.Contains |> not then
                                    layer.AddAdornment(AdornmentPositioningBehavior.ViewportRelative, Nullable(), 
                                        self, stackPanel, AdornmentRemovedCallback(fun _ _ -> ())) |> ignore
                                    addedAdornments.Add stackPanel |> ignore
                        | _ -> ()
                    with e -> logExceptionWithContext (e, "LayoutChanged, processing new visible lines")
            } |> Async.Ignore |> RoslynHelpers.StartAsyncSafe layoutChangedCts.Token
        with e -> logExceptionWithContext (e, "Layout changed")
        ()

    // Add buffer changed event handler
    do (
        buffer.Changed.Add handleBufferChanged
        codeLensLayer <- view.GetAdornmentLayer "CodeLens"
        view.LayoutChanged.Add handleLayoutChanged
       )
    
    /// Public non-thread-safe method to add code lens for a given tracking span.
    /// Returns an UIElement which can be used to add Ui elements and to remove the code lens later.
    override __.AddCodeLens (trackingSpan:ITrackingSpan) =
        if trackingSpan.TextBuffer <> buffer then failwith "TrackingSpan text buffer does not equal with CodeLens text buffer"
        let stackPanel = addTrackingSpan trackingSpan
        self.RelayoutRequested.Enqueue(())
        stackPanel :> UIElement
    
    /// Public non-thread-safe method to remove code lens for a given tracking span.
    /// Returns whether the operation succeeded
    override __.RemoveCodeLens (trackingSpan:ITrackingSpan) =
        if uiElements.ContainsKey trackingSpan then
            let stackPanel = uiElements.[trackingSpan]
            stackPanel.Children.Clear()
            uiElements.Remove trackingSpan |> ignore
            try
                codeLensLayer.RemoveAdornment(stackPanel) 
            with e -> 
                logExceptionWithContext(e, "Removing code lens")
        else
            logWarningf "No ui element is attached to this tracking span!"
        let lineNumber = 
            (trackingSpan.GetStartPoint currentBufferSnapshot).Position 
            |> currentBufferSnapshot.GetLineNumberFromPosition
        if trackingSpans.ContainsKey lineNumber then
            trackingSpans.[lineNumber].Remove trackingSpan |> ignore
            if trackingSpans.[lineNumber].Count = 0 then
                trackingSpans.Remove lineNumber |> ignore
        else
            logWarningf "No tracking span is accociated with this line number %d!" lineNumber
    
    override __.AddUiElementToCodeLens (trackingSpan:ITrackingSpan) (uiElement:UIElement)=
        let stackPanel = uiElements.[trackingSpan]
        stackPanel.Children.Add(uiElement) |> ignore
        tagsChangedEvent.Trigger(self, SnapshotSpanEventArgs(trackingSpan.GetSpan(buffer.CurrentSnapshot))) // Need to refresh the tag.
    
    override __.AddUiElementToCodeLensOnce (trackingSpan:ITrackingSpan) (uiElement:UIElement)=
        let stackPanel = uiElements.[trackingSpan]
        if uiElement |> stackPanel.Children.Contains |> not then
            self.AddUiElementToCodeLens trackingSpan uiElement

    override __.RemoveUiElementFromCodeLens (trackingSpan:ITrackingSpan) (uiElement:UIElement) =
        let stackPanel = uiElements.[trackingSpan]
        stackPanel.Children.Remove(uiElement) |> ignore
        tagsChangedEvent.Trigger(self, SnapshotSpanEventArgs(trackingSpan.GetSpan(buffer.CurrentSnapshot))) // Need to refresh the tag.

    /// <summary>
    /// This is being called after the tagger has processed a buffer changed event. Please DON'T add new code lens within a buffer changed event. 
    /// Subscribe to this event to ensure that all data is updated.
    /// </summary>
    //[<CLIEvent>]
    //override __.BufferChanged = bufferChanged.Publish

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
                        if trackingSpans.ContainsKey(lineNumber) && trackingSpans.[lineNumber] |> Seq.isEmpty |> not then
                            
                            let tagSpan = snapshot.GetLineFromLineNumber(lineNumber).Extent
                            let stackPanels = 
                                trackingSpans.[lineNumber] 
                                |> Seq.map (fun trackingSpan ->
                                        let success, res = uiElements.TryGetValue trackingSpan
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
                            // logInfof "adding tag, height %A, visible %A" height (stackPanels |> Seq.head).Visibility
                            
                            yield TagSpan(span, CodeLensGeneralTag(0., height, 0., 0., 0., PositionAffinity.Predecessor, stackPanels, self)) :> ITagSpan<CodeLensGeneralTag>
                }
            with e -> 
                logErrorf "Error in code lens get tags %A" e
                Seq.empty

type internal CodeLens(taggedText, computed, fullTypeSignature, uiElement) =
      member val TaggedText: Async<(ResizeArray<Layout.TaggedText> * QuickInfoNavigation) option> = taggedText
      member val Computed: bool = computed with get, set
      member val FullTypeSignature: string = fullTypeSignature 
      member val UiElement: UIElement = uiElement with get, set


