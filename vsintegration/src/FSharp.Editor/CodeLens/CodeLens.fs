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

type CodeLensGeneralTag(width, topSpace, baseline, textHeight, bottomSpace, affinity, tag:obj, providerTag:obj) =
    inherit SpaceNegotiatingAdornmentTag(width, topSpace, baseline, textHeight, bottomSpace, affinity, tag, providerTag)

/// Class which provides support for general code lens
/// Use the methods <code>AddCodeLens</code< and <code> RemoveCodeLens</code>
type CodeLensGeneralTagger (buffer:ITextBuffer) as self =
    // Custom simple tagger logic

    /// Saves the ui context to switch context for ui related work.
    let uiContext = SynchronizationContext.Current

    /// The tags changed event to notify if the data for the tags has changed.
    let tagsChangedEvent = new Event<EventHandler<SnapshotSpanEventArgs>,SnapshotSpanEventArgs>()

    // Tracks the created ui elements per TrackingSpan
    let uiElements = Dictionary<_,_>()
    /// Caches the current used trackingSpans per line. One line can contain multiple trackingSpans
    let mutable trackingSpans = Dictionary<_, Generic.List<_>>()
    /// Text view for accessing the adornment layer.
    let mutable view: IWpfTextView option = None
    /// The code lens layer for adding and removing adornments.
    let mutable codeLensLayer: IAdornmentLayer option = None
    /// Tracks the recent first + last visible line numbers for adornment layout logic.
    let mutable recentFirstVsblLineNmbr, recentLastVsblLineNmbr = 0, 0
    /// Tracks the adornments on the layer.
    let mutable addedAdornments = HashSet()
    /// Cancellation token source for the layout changed event. Needed to abort previous async-work.
    let mutable layoutChangedCts = new CancellationTokenSource()

    let mutable currentBufferSnapshot = null

    /// Helper method which returns the start line number of a tracking span
    let getTrackingSpanStartLine (snapshot:ITextSnapshot) (trackingSpan:ITrackingSpan) =
        snapshot.GetLineNumberFromPosition(trackingSpan.GetStartPoint(snapshot).Position)

    /// Helper method which returns the start line number of a tracking span
    let tryGetTSpanStartLine (snapshot:ITextSnapshot) (trackingSpan:ITrackingSpan) =
        let pos = trackingSpan.GetStartPoint(snapshot).Position
        if snapshot.Length - 1 < pos then None
        else pos |> snapshot.GetLineNumberFromPosition |> Some

    /// Helper method which iterates over the tracking spans for the given line number.
    /// It checks whether the line of the tracking span changed and if so it'll move it around in the dictionary.
    //let updateTrackingSpans lineNumber =
    //    if lineNumber |> trackingSpans.ContainsKey then
    //        let currentTrackingSpans = Generic.List(trackingSpans.[lineNumber])
    //        let snapshot = buffer.CurrentSnapshot
    //        for trackingSpan in currentTrackingSpans do
    //            let newLine = getTrackingSpanStartLine snapshot trackingSpan
    //            if newLine <> lineNumber then
    //                trackingSpans.[lineNumber].Remove(trackingSpan) |> ignore
    //                trackingSpans.[newLine].Add(trackingSpan)
    
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
                        if newLine |> trackingSpans.ContainsKey |> not then
                            trackingSpans.[newLine] <- Generic.List()
                        trackingSpans.[newLine].Add(trackingSpan)
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
            match view with 
            | None -> ()
            | Some view ->
                let firstLine = view.TextViewLines.FirstVisibleLine
                view.DisplayTextLineContainingBufferPosition (firstLine.Start, 0., ViewRelativePosition.Top)
                self.RelayoutRequested.Enqueue(())
         with e -> logErrorf "Error in code lens provider: %A" e

    /// Here all layout methods for the adornments is done.
    /// Adornments which aren't in the current visible spans anymore are hidden,
    /// others which intersect the current visible span are made visible and afterwards async layouted.
    let handleLayoutChanged (e:TextViewLayoutChangedEventArgs) =
        try
            currentBufferSnapshot <- e.NewSnapshot
            let recentVisibleLineNumbers = Set [recentFirstVsblLineNmbr .. recentLastVsblLineNmbr]
            let firstVisibleLineNumber, lastVisibleLineNumber =
                match view with
                | None -> 
                    logWarning "View is none!"
                    0, 0
                | Some view ->
                    let first, last = 
                        view.TextViewLines.FirstVisibleLine, 
                        view.TextViewLines.LastVisibleLine
                    let buffer = e.NewSnapshot
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
                let snapshot = buffer.CurrentSnapshot
                let view = view.Value
                for lineNumber in nonVisibleLineNumbers do
                    if lineNumber > 0 && lineNumber < snapshot.LineCount then
                        try
                            let line = 
                                let l = e.NewSnapshot.GetLineFromLineNumber(lineNumber)
                                view.GetTextViewLineContainingBufferPosition(l.Start)
                            applyFuncOnLineStackPanels line (fun ui ->
                                ui.Visibility <- Visibility.Hidden
                            )
                        with e -> logErrorf "Error in non visible lines iteration %A" e
                for lineNumber in newVisibleLineNumbers do
                    try
                        let line = 
                                let l = e.NewSnapshot.GetLineFromLineNumber(lineNumber)
                                view.GetTextViewLineContainingBufferPosition(l.Start)
                        applyFuncOnLineStackPanels line (fun ui ->
                            ui.Visibility <- Visibility.Visible
                            layoutUIElementOnLine view line.Extent ui
                        )
                     with e -> logErrorf "Error in new visible lines iteration %A" e
            if self.RelayoutRequested.Count > 0 then
                let view = view.Value
                self.RelayoutRequested.Dequeue() |> ignore
                let buffer = buffer.CurrentSnapshot
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
                let! view = view
                let! layer = codeLensLayer
                // Now relayout the existing adornments
                if nonVisibleLineNumbers.Count > 0 || newVisibleLineNumbers.Count > 0 then
                    let buffer = buffer.CurrentSnapshot
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
    do buffer.ChangedHighPriority.Add handleBufferChanged
    
    /// <summary>
    /// Enqueing an unit signals to the tagger that all visible code lens must be layouted again,
    /// to respect single line changes.
    /// </summary>
    member val RelayoutRequested : Queue = Queue() with get, set
    
    /// Public non-thread-safe method to add code lens for a given tracking span.
    /// Returns an UIElement which can be used to add Ui elements and to remove the code lens later.
    member __.AddCodeLens (trackingSpan:ITrackingSpan) =
        if trackingSpan.TextBuffer <> buffer then failwith "TrackingSpan text buffer does not equal with CodeLens text buffer"
        let stackPanel = addTrackingSpan trackingSpan
        self.RelayoutRequested.Enqueue(())
        stackPanel :> UIElement
    
    /// Public non-thread-safe method to remove code lens for a given tracking span.
    /// Returns nothing.
    member __.RemoveCodeLens (trackingSpan:ITrackingSpan) =
        let stackPanel = uiElements.[trackingSpan]
        stackPanel.Children.Clear()
        uiElements.Remove trackingSpan |> ignore
        match codeLensLayer with 
        | Some layer ->
            try
                layer.RemoveAdornment(stackPanel) 
                true
            with e -> 
                logExceptionWithContext(e, "Removing code lens")
                false
        | None -> 
            logWarningf "Adornment for tracking span %A does not exist!" trackingSpan
            false
    
    member __.AddUiElementToCodeLens (trackingSpan:ITrackingSpan) (uiElement:UIElement)=
        let stackPanel = uiElements.[trackingSpan]
        stackPanel.Children.Add(uiElement) |> ignore
        tagsChangedEvent.Trigger(self, SnapshotSpanEventArgs(trackingSpan.GetSpan(buffer.CurrentSnapshot))) // Need to refresh the tag.
    
    member __.AddUiElementToCodeLensOnce (trackingSpan:ITrackingSpan) (uiElement:UIElement)=
        let stackPanel = uiElements.[trackingSpan]
        if uiElement |> stackPanel.Children.Contains |> not then
            self.AddUiElementToCodeLens trackingSpan uiElement

    member __.RemoveUiElementFromCodeLens (trackingSpan:ITrackingSpan) (uiElement:UIElement) =
        let stackPanel = uiElements.[trackingSpan]
        stackPanel.Children.Remove(uiElement) |> ignore
        tagsChangedEvent.Trigger(self, SnapshotSpanEventArgs(trackingSpan.GetSpan(buffer.CurrentSnapshot))) // Need to refresh the tag.
    
    /// This method shouldn't exist at all... 
    /// However we still need it.
    member __.SetView value = 
        view <- Some value
        codeLensLayer <- Some (value.GetAdornmentLayer "CodeLens")
        value.LayoutChanged.Add handleLayoutChanged
        
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
            
type CodeLensTag(width, topSpace, baseline, textHeight, bottomSpace, affinity, tag:obj, providerTag:obj) =
    inherit SpaceNegotiatingAdornmentTag(width, topSpace, baseline, textHeight, bottomSpace, affinity, tag, providerTag)

type internal CodeLens(taggedText, computed, fullTypeSignature, uiElement) =
      member val TaggedText: Async<(ResizeArray<Layout.TaggedText> * QuickInfoNavigation) option> = taggedText
      member val Computed: bool = computed with get, set
      member val FullTypeSignature: string = fullTypeSignature 
      member val UiElement: UIElement = uiElement with get, set

//type internal CodeLens =
//    { TaggedText: Async<(ResizeArray<Layout.TaggedText> * QuickInfoNavigation) option>
//      mutable Computed: bool 
//      mutable FullTypeSignature: string
//      mutable UiElement: UIElement }

type internal FSharpCodeLensTagger
    (
        workspace: Workspace, 
        documentId: Lazy<DocumentId>,
        buffer: ITextBuffer, 
        checker: FSharpChecker,
        projectInfoManager: FSharpProjectOptionsManager,
        typeMap: Lazy<ClassificationTypeMap>,
        gotoDefinitionService: FSharpGoToDefinitionService
     ) as self =
    inherit CodeLensGeneralTagger(buffer)

    //static let candidate = "abcdefghijklmnopqrstuvwxyz->ABCDEFGHIJKLMNOPQRSTUVWXYZ"
    
    //static let MeasureString candidate (textBox:TextBlock)=
    //    let formattedText = 
    //        FormattedText(candidate, CultureInfo.CurrentUICulture, FlowDirection.LeftToRight,
    //            Typeface(textBox.FontFamily, textBox.FontStyle, textBox.FontWeight, textBox.FontStretch),
    //            textBox.FontSize, Brushes.Black)
    //    Size(formattedText.Width, formattedText.Height)

    //static let MeasureTextBlock textBlock =
    //    MeasureString candidate textBlock
    

    //static let GetTextBlockSize = MeasureTextBlock (TextBlock())

    let visit pos parseTree = 
        AstTraversal.Traverse(pos, parseTree, { new AstTraversal.AstVisitorBase<_>() with 
            member this.VisitExpr(_path, traverseSynExpr, defaultTraverse, expr) =
                defaultTraverse(expr)
            
            override this.VisitInheritSynMemberDefn (_, _, _, _, range) = Some range

            override this.VisitTypeAbbrev( _, range) = Some range

            override this.VisitLetOrUse(binding, range) = Some range

            override this.VisitComponentInfo componentInfo = Some componentInfo.Range
        })

    let formatMap = lazy typeMap.Value.ClassificationFormatMapService.GetClassificationFormatMap "tooltip"
    //let visibleAdornments = ConcurrentDictionary()

    let mutable lastResults = Dictionary<string, ITrackingSpan * CodeLens>()
    let mutable firstTimeChecked = false
    let mutable bufferChangedCts = new CancellationTokenSource()
    let uiContext = SynchronizationContext.Current

    let FSharpRangeToSpan (bufferSnapshot:ITextSnapshot) (range:range) =
        try
            let startLine, endLine = 
                bufferSnapshot.GetLineFromLineNumber(range.StartLine - 1),
                bufferSnapshot.GetLineFromLineNumber(range.EndLine - 1)
            let startPosition = startLine.Start.Add range.StartColumn
            let endPosition = endLine.Start.Add range.EndColumn
            Span(startPosition.Position, endPosition.Position - startPosition.Position) |> Some
        with e -> 
            logErrorf "Error: %A" e
            None

    let layoutTagToFormatting (layoutTag: LayoutTag) =
        layoutTag
        |> RoslynHelpers.roslynTag
        |> ClassificationTags.GetClassificationTypeName
        |> typeMap.Value.GetClassificationType
        |> formatMap.Value.GetTextProperties   

    let createTextBox (lens:CodeLens) =
        asyncMaybe {
            let! taggedText, navigation = lens.TaggedText
            let textBox = new TextBlock(Width = 500., Background = Brushes.Transparent, Opacity = 0.5, TextTrimming = TextTrimming.WordEllipsis)
            DependencyObjectExtensions.SetDefaultTextProperties(textBox, formatMap.Value)
            for text in taggedText do
                let run = Documents.Run text.Text
                DependencyObjectExtensions.SetTextProperties (run, layoutTagToFormatting text.Tag)
                let inl =
                    match text with
                    | :? Layout.NavigableTaggedText as nav when navigation.IsTargetValid nav.Range ->
                        let h = Documents.Hyperlink(run, ToolTip = nav.Range.FileName)
                        h.Click.Add (fun _ -> navigation.NavigateTo nav.Range)
                        h :> Documents.Inline
                    | _ -> run :> _
                textBox.Inlines.Add inl
            textBox.Opacity <- 0.5
            lens.Computed <- true
            lens.UiElement <- textBox
        } |> Async.Ignore

    let executeCodeLenseAsync () =  
        asyncMaybe {
            do! Async.Sleep 800 |> liftAsync
            logInfof "Rechecking code due to buffer edit!"
            let! document = workspace.CurrentSolution.GetDocument(documentId.Value) |> Option.ofObj
            let! options = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document)
            let! _, parsedInput, checkFileResults = checker.ParseAndCheckDocument(document, options, true, "CodeLens")
            logInfof "Getting uses of all symbols!"
            let! symbolUses = checkFileResults.GetAllUsesOfAllSymbolsInFile() |> liftAsync
            let textSnapshot = buffer.CurrentSnapshot
            logInfof "Updating due to buffer edit!"
            
            logInfo "HellowWorld"
            // Clear existing data and cache flags
            // For adornments which are invalid due to signature changes (the tag is still valid though)
            let outdatedAdornments = Generic.List()
            // The results which are left.
            let oldResults = Dictionary(lastResults)

            let newResults = Dictionary()
            // Symbols which cache wasn't found yet
            let unattachedSymbols = Generic.List()
            // Tags which are new or need to be updated due to changes.
            let tagsToUpdate = Dictionary()
            let continuedAdornmentTags = Dictionary()
            let codeLensToAdd = Generic.List()

            let useResults (displayContext: FSharpDisplayContext, func: FSharpMemberOrFunctionOrValue) =
                async {
                    try
                        let textSnapshot = buffer.CurrentSnapshot
                        let lineNumber = Line.toZ func.DeclarationLocation.StartLine
                        //logInfof "Computing cache for line %A with content %A" lineNumber lineStr
                        if (lineNumber >= 0 || lineNumber < textSnapshot.LineCount) && 
                            not func.IsPropertyGetterMethod && 
                            not func.IsPropertySetterMethod then
                
                            match func.FullTypeSafe with
                            | Some ty ->
                                let! displayEnv = checkFileResults.GetDisplayEnvForPos(func.DeclarationLocation.Start)
                            
                                let displayContext =
                                    match displayEnv with
                                    | Some denv -> FSharpDisplayContext(fun _ -> denv)
                                    | None -> displayContext
                                 
                                let typeLayout = ty.FormatLayout(displayContext)
                                let taggedText = ResizeArray()
                                Layout.renderL (Layout.taggedTextListR taggedText.Add) typeLayout |> ignore
                                let navigation = QuickInfoNavigation(gotoDefinitionService, document, func.DeclarationLocation)
                                // Because the data is available notify that this line should be updated, displaying the results
                                return Some (taggedText, navigation)
                            | None -> 
                                logWarningf "Couldn't acquire CodeLens data for function %A" func
                                return None
                        else return None
                    with e -> 
                        logErrorf "Error in lazy code lens computation. %A" e
                        return None
                }

            for symbolUse in symbolUses do 
                if symbolUse.IsFromDefinition then
                    match symbolUse.Symbol with
                    | :? FSharpEntity as entity ->
                        for func in entity.MembersFunctionsAndValues do
                            // Regardles of whether we are in a async maybe, we don't want to abort the whole process due to a single empty option.
                            let! declarationRange = visit func.DeclarationLocation.Start parsedInput
                            let! declarationSpan = FSharpRangeToSpan textSnapshot declarationRange
                            let funcID = func.FullName
                            let fullDeclarationText = (textSnapshot.GetText declarationSpan).Replace(func.CompiledName, funcID)
                            let fullTypeSignature = func.FullType.ToString()
                            // Try to re-use the last results
                            if lastResults.ContainsKey(fullDeclarationText) then
                                // Make sure that the results are usable
                                let lastTrackingSpan, codeLens = lastResults.[fullDeclarationText]
                                if codeLens.FullTypeSignature = fullTypeSignature then
                                    // The results can be reused because the signature is the same
                                    if codeLens.Computed then
                                        // Just re-use the old results, changed nothing
                                        newResults.[fullDeclarationText] <- (lastTrackingSpan, codeLens)
                                        continuedAdornmentTags.[codeLens] <- ()
                                        // logInfof "Declaration %A can be reused. IdentityTag %A" fullDeclarationText codeLens
                                        oldResults.Remove(fullDeclarationText) |> ignore // Just tracking this
                                    else
                                        // The old results aren't computed at all, because the line might have changed create new results
                                        tagsToUpdate.[lastTrackingSpan] <- 
                                            (fullDeclarationText, 
                                                CodeLens( Async.cache (useResults (symbolUse.DisplayContext, func)),
                                                    false,
                                                    fullTypeSignature,
                                                    null))
                                        oldResults.Remove(fullDeclarationText) |> ignore
                                else
                                    // The signature is invalid so save the invalid data to remove it later (if those is valid)
                                    if codeLens.Computed && codeLens.UiElement |> isNull |> not then
                                        // Track the old element for removal
                                        outdatedAdornments.Add codeLens.UiElement
                                        // Push back the new results
                                        tagsToUpdate.[lastTrackingSpan] <- (fullDeclarationText,
                                                CodeLens( Async.cache (useResults (symbolUse.DisplayContext, func)),
                                                    false,
                                                    fullTypeSignature,
                                                    null ))
                                        oldResults.Remove(fullDeclarationText) |> ignore
                            else
                                // The symbol might be completely new or has slightly changed. 
                                // We need to track this and iterate over the left entries to ensure that there isn't anything
                                unattachedSymbols.Add((symbolUse, func, fullDeclarationText, fullTypeSignature))
                    | _ -> ()
            
            // In best case this works quite `covfefe` fine because often enough we change only a small part of the file and not the complete.
            for unattachedSymbol in unattachedSymbols do
                let symbolUse, func, fullDeclarationText, fullTypeSignature = unattachedSymbol
                let test (v:KeyValuePair<_, _>) =
                    let _, (codeLens:CodeLens) = v.Value
                    codeLens.FullTypeSignature = fullTypeSignature
                match oldResults |> Seq.tryFind test with
                | Some res ->
                    let (trackingSpan : ITrackingSpan), (codeLens : CodeLens) = res.Value
                    if codeLens.Computed && (isNull codeLens.UiElement |> not) then
                        newResults.[fullDeclarationText] <- (trackingSpan, codeLens)
                        tagsToUpdate.[trackingSpan] <- (fullDeclarationText, codeLens)
                        continuedAdornmentTags.[codeLens] <- ()
                    else
                        // The tag might be still valid but it hasn't been computed yet so create fresh results
                        tagsToUpdate.[trackingSpan] <- (fullDeclarationText,
                            CodeLens(
                                Async.cache (useResults (symbolUse.DisplayContext, func)),
                                false,
                                fullTypeSignature,
                                null))
                    let key = res.Key
                    oldResults.Remove(key) |> ignore // no need to check this entry again
                | None ->
                    // This function hasn't got any cache and so it's completely new.
                    // So create completely new results
                    // And finally add a tag for this.
                    let res = 
                        CodeLens(
                            Async.cache (useResults (symbolUse.DisplayContext, func)),
                            false,
                            fullTypeSignature,
                            null)
                    try
                        let declarationSpan = textSnapshot.GetLineFromLineNumber(func.DeclarationLocation.StartLine - 1).Extent.Span
                        let trackingSpan = 
                            textSnapshot.CreateTrackingSpan(declarationSpan, SpanTrackingMode.EdgeInclusive)
                        codeLensToAdd.Add (trackingSpan, res)
                        newResults.[fullDeclarationText] <- (trackingSpan, res)
                    with e -> logExceptionWithContext (e, "Code Lens tracking tag span creation")
                ()

            //for tagToUpdate in tagsToUpdate do
            //    do! removeTag(tagToUpdate.Key)
            //    let fullDeclarationText, tag = tagToUpdate.Value
            //    let! newTag = addTag (tagToUpdate.Key.Span) (CodeLensTag(0., GetTextBlockSize.Height, 0., 0., 0., PositionAffinity.Predecessor, tag, self))
            //    newResults.[fullDeclarationText] <- newTag
            lastResults <- newResults
            do! Async.SwitchToContext uiContext |> liftAsync
            
            for value in codeLensToAdd do
                let trackingSpan, codeLens = value
                let stackPanel = self.AddCodeLens trackingSpan
                // logInfof "Trackingspan %A is being added." trackingSpan 
                
                stackPanel.IsVisibleChanged
                |> Event.filter (fun eventArgs -> eventArgs.NewValue :?> bool)
                |> Event.add (fun _ ->
                    if codeLens.Computed |> not then
                        async {
                            do! Async.SwitchToContext uiContext
                            do! createTextBox codeLens
                            let uiElement = codeLens.UiElement
                            self.AddUiElementToCodeLensOnce trackingSpan uiElement
                            // logInfo "Adding text box!"
                        } |> RoslynHelpers.StartAsyncSafe CancellationToken.None
                    )

            for oldResult in oldResults do
                let trackingSpan, _ = oldResult.Value
                // logInfof "removing trackingSpan %A" trackingSpan
                self.RemoveCodeLens trackingSpan |> ignore

            logInfof "Finished updating code lens."
            
            if not firstTimeChecked then
                firstTimeChecked <- true
        } |> Async.Ignore
    
    do  
        begin
            buffer.Changed.AddHandler(fun _ e -> (self.BufferChanged e))
            async {
              let mutable numberOfFails = 0
              while not firstTimeChecked && numberOfFails < 10 do
                  try
                      do! executeCodeLenseAsync()
                      do! Async.Sleep(1000)
                  with
                  | e -> logErrorf "Code Lens startup failed with: %A" e
                         numberOfFails <- numberOfFails + 1
           } |> Async.Start
        end

    member __.BufferChanged ___ =
        bufferChangedCts.Cancel() // Stop all ongoing async workflow. 
        bufferChangedCts.Dispose()
        bufferChangedCts <- new CancellationTokenSource()
        executeCodeLenseAsync () |> Async.Ignore |> RoslynHelpers.StartAsyncSafe bufferChangedCts.Token

[<Export(typeof<IWpfTextViewCreationListener>)>]
[<Export(typeof<ITaggerProvider>)>]
[<TagType(typeof<CodeLensTag>)>]
[<ContentType(FSharpConstants.FSharpContentTypeName)>]
[<TextViewRole(PredefinedTextViewRoles.Document)>]
type internal CodeLensProvider  
    [<ImportingConstructor>]
    (
        textDocumentFactory: ITextDocumentFactoryService,
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: FSharpProjectOptionsManager,
        typeMap: Lazy<ClassificationTypeMap>,
        gotoDefinitionService: FSharpGoToDefinitionService
    ) as __ =

    let taggers = ResizeArray()
    
    let componentModel = Package.GetGlobalService(typeof<ComponentModelHost.SComponentModel>) :?> ComponentModelHost.IComponentModel
    let workspace = componentModel.GetService<VisualStudioWorkspace>()

    /// Returns an provider for the textView if already one has been created. Else create one.
    let getSuitableAdornmentProvider (buffer) =
        let res = taggers |> Seq.tryFind(fun (view, _) -> view = buffer)
        match res with
        | Some (_, res) -> res
        | None ->
            let documentId = 
                lazy (
                    match textDocumentFactory.TryGetTextDocument(buffer) with
                    | true, textDocument ->
                         Seq.tryHead (workspace.CurrentSolution.GetDocumentIdsWithFilePath(textDocument.FilePath))
                    | _ -> None
                    |> Option.get
                )


            let tagger = FSharpCodeLensTagger(workspace, documentId, buffer, checkerProvider.Checker, projectInfoManager, typeMap, gotoDefinitionService)
            taggers.Add((buffer, tagger))
            tagger
    [<Export(typeof<AdornmentLayerDefinition>); Name("CodeLens");
      Order(Before = PredefinedAdornmentLayers.Text);
      TextViewRole(PredefinedTextViewRoles.Document)>]

    member val CodeLensAdornmentLayerDefinition : AdornmentLayerDefinition = null with get, set

    interface IWpfTextViewCreationListener with
        
        override __.TextViewCreated view =
            let tagger = getSuitableAdornmentProvider view.TextBuffer
            tagger.SetView view
            // The view has been initialized. Notify that we can now theoretically display CodeLens
            // Temporarily removed, eventually needed again!  
            ()
             

    interface ITaggerProvider with
        override __.CreateTagger(buffer) = box (getSuitableAdornmentProvider buffer) :?> _
