// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

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
open Microsoft.CodeAnalysis.Editor.Shared.Extensions
open Microsoft.CodeAnalysis.Editor.Shared.Utilities
open Microsoft.CodeAnalysis.Classification
open Internal.Utilities.StructuredFormat
open Microsoft.VisualStudio.Text.Tagging
open System.Collections.Concurrent
open System.Collections
open System.Windows.Media.Animation
  
type CodeLensTag(width, topSpace, baseline, textHeight, bottomSpace, affinity, tag:obj, providerTag:obj) =
    inherit SpaceNegotiatingAdornmentTag(width, topSpace, baseline, textHeight, bottomSpace, affinity, tag, providerTag)

type internal CodeLensTagger  
    (
        workspace: Workspace, 
        documentId: Lazy<DocumentId>,
        buffer: ITextBuffer, 
        checker: FSharpChecker,
        projectInfoManager: ProjectInfoManager,
        typeMap: Lazy<ClassificationTypeMap>,
        gotoDefinitionService: FSharpGoToDefinitionService
     ) as self =
        let tagsChanged = new Event<EventHandler<SnapshotSpanEventArgs>,SnapshotSpanEventArgs>()

        let formatMap = lazy typeMap.Value.ClassificationFormatMapService.GetClassificationFormatMap "tooltip"
        let mutable codeLensResults = ConcurrentDictionary()
        let mutable codeLensCacheAvailable = Generic.HashSet()
        let mutable codeLensUIElementCache = ConcurrentDictionary()
        let mutable firstTimeChecked = false

        do assert (documentId <> null)

        let mutable cancellationTokenSourceBufferChanged = new CancellationTokenSource()
        let mutable cancellationTokenBufferChanged = cancellationTokenSourceBufferChanged.Token
        
        let mutable cancellationTokenSourceLayoutChanged = new CancellationTokenSource()
    
        let layoutTagToFormatting (layoutTag: LayoutTag) =
            layoutTag
            |> RoslynHelpers.roslynTag
            |> ClassificationTags.GetClassificationTypeName
            |> typeMap.Value.GetClassificationType
            |> formatMap.Value.GetTextProperties   
        
        let mutable visibleLines = Set.empty

        let executeCodeLenseAsync () =  
            let uiContext = SynchronizationContext.Current
            asyncMaybe {
                try 
                    Async.Sleep(2000) |> Async.RunSynchronously
                    Logging.Logging.logInfof "Rechecking code due to buffer edit!"
                    let! document = workspace.CurrentSolution.GetDocument(documentId.Value) |> Option.ofObj
                    let! options = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document)
                    let! _, _, checkFileResults = checker.ParseAndCheckDocument(document, options, allowStaleResults = true)
                    Logging.Logging.logInfof "Getting uses of all symbols!"
                    let! symbolUses = checkFileResults.GetAllUsesOfAllSymbolsInFile() |> liftAsync
                    
                    let view = self.WpfTextView.Value;
                    let textSnapshot = view.TextSnapshot.TextBuffer.CurrentSnapshot
                    Logging.Logging.logInfof "Updating code lens due to buffer edit!"

                    //Clear existing data and cache flags
                    codeLensResults.Clear()
                    codeLensCacheAvailable.Clear()

                    let useResults (displayContext: FSharpDisplayContext, func: FSharpMemberOrFunctionOrValue) =
                        async {
                            let lineNumber = Line.toZ func.DeclarationLocation.StartLine
                        
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
                                    
                                    if visibleLines.Contains lineNumber then
                                        // The line might be visible but doesn't displaying the results yet, force it being redrawn.
                                        visibleLines <- visibleLines.Remove(lineNumber)
                                    
                                    let line = textSnapshot.GetLineFromLineNumber(lineNumber)
                                    // This is only used with a cached async so flag the data as ready to use
                                    codeLensCacheAvailable.Add(line.Extent.GetText()) |> ignore 
                                    // Because the data is available notify that this line should be updated, displaying the results
                                    tagsChanged.Trigger(self, SnapshotSpanEventArgs(SnapshotSpan(line.Start, line.End)))
                                    return Some taggedText, Some navigation
                                | None -> return None, None
                            else return None, None
                        }

                    for symbolUse in symbolUses do
                        if symbolUse.IsFromDefinition then
                            match symbolUse.Symbol with
                            | :? FSharpEntity as entity ->
                                for func in entity.MembersFunctionsAndValues do
                                    let lineNumber = Line.toZ func.DeclarationLocation.StartLine
                                    let text = textSnapshot.GetLineFromLineNumber(int lineNumber).GetText()
                                    codeLensResults.[text] <- Async.cache (useResults (symbolUse.DisplayContext, func))
                            | _ -> ()
                    do! Async.SwitchToContext uiContext |> liftAsync
                    let handler = tagsChanged;
                    handler.Trigger(self, SnapshotSpanEventArgs(SnapshotSpan(view.TextViewLines.FirstVisibleLine.Start, view.TextViewLines.LastVisibleLine.End)))
                    Logging.Logging.logInfof "Finished updating code lens." |> ignore
                    if not firstTimeChecked then
                        firstTimeChecked <- true
                with
                | ex -> Logging.Logging.logErrorf "Error occured: %A" ex
                        
            }
        
        do asyncMaybe{
            while not firstTimeChecked do
                do! executeCodeLenseAsync()
                Async.Sleep(1000) |> Async.RunSynchronously
           } |> Async.Ignore |> Async.Start
        
        /// Creates the code lens ui elements for the specified line
        /// TODO, add caching to the UI elements
        let createCodeLensUIElementByLine (line:ITextViewLine) =
            let text = line.Extent.GetText()
            if codeLensResults.ContainsKey(text) then // Check whether this line has code lens
                if codeLensCacheAvailable.Contains(text) then  // Also check whether cache is available
                    if codeLensUIElementCache.ContainsKey(text) then
                        // Use existing UI element which is proved to be safe to use
                        Some codeLensUIElementCache.[text]
                    else
                        // No delay because it's already computed
                        let taggedTextOption, navigationOption = codeLensResults.[text] |> Async.RunSynchronously 
                    
                        let view = self.WpfTextView.Value
                        // Get the real offset so that the code lens are placed correctly
                        let offset = 
                            [0..line.Length - 1] |> Seq.tryFind (fun i -> not (Char.IsWhiteSpace (line.Start.Add(i).GetChar())))
                            |> Option.defaultValue 0
                        let realStart = line.Start.Add(offset)
                        let span = SnapshotSpan(line.Snapshot, Span.FromBounds(int realStart, int line.End))
                        let geometry = view.TextViewLines.GetMarkerGeometry(span)

                        // It could be that the options are empty so we need to check the results
                        match taggedTextOption, navigationOption with
                        | Some taggedText, Some navigation ->
                            // We have valid data, create our UI element
                            let textBox = TextBlock(Width = view.ViewportWidth, Background = Brushes.Transparent, Opacity = 0.5, TextTrimming = TextTrimming.WordEllipsis)
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
                            Canvas.SetLeft(textBox, geometry.Bounds.Left)
                            Canvas.SetTop(textBox, geometry.Bounds.Top - 15.)
                            textBox.Opacity <- 0.5
                            codeLensUIElementCache.TryAdd(text, textBox) |> ignore
                            Some textBox
                        | _, _ -> None // There aren't any valid results with the current data, skip
                else
                    // We start asynchrounus cache computation so that the UI isn't blocked
                    codeLensResults.[text] |> Async.Ignore |> Async.Start
                    // It's likely that if the cache isn't computed yet, that the current content
                    // of the UI element cache is wrong, so remove it.
                    let unusedElement = null
                    codeLensUIElementCache.TryRemove(text, &unusedElement) |> ignore
                    None //The cache wasn't available, skip
            else
                None // There aren't any code lens available for this line, skip

        do buffer.Changed.AddHandler(fun _ e -> (self.BufferChanged e))
        member __.BufferChanged ___ =
            cancellationTokenSourceBufferChanged.Cancel() // Stop all ongoing async workflow. 
            cancellationTokenSourceBufferChanged.Dispose()
            cancellationTokenSourceBufferChanged <- new CancellationTokenSource()
            cancellationTokenBufferChanged <- cancellationTokenSourceBufferChanged.Token
            executeCodeLenseAsync () |> Async.Ignore |> RoslynHelpers.StartAsyncSafe cancellationTokenBufferChanged
                
        member val WpfTextView : Lazy<IWpfTextView> = Lazy<IWpfTextView>() with get,set 

        member val CodeLensLayer = Lazy<IAdornmentLayer>() with get, set

        member __.TriggerTagsChanged args = tagsChanged.Trigger(self, args)

        member __.LayoutChanged (e:TextViewLayoutChangedEventArgs) =
            let uiContext = SynchronizationContext.Current

            cancellationTokenSourceLayoutChanged.Cancel()
            cancellationTokenSourceLayoutChanged.Dispose()
            cancellationTokenSourceLayoutChanged <- new CancellationTokenSource()
            let asyncStuff = 
                async{
                    if firstTimeChecked then
                        try // We always need to catch exceptions here because we're working in a critical section due to multi threading
                            let handleAlreadyVisible = 
                                async {
                                    do! Async.SwitchToContext uiContext
                                    let view = self.WpfTextView.Value
                                    let buffer = view.TextBuffer.CurrentSnapshot
                                    for line in e.NewOrReformattedLines do
                                        if Set.contains (buffer.GetLineNumberFromPosition(line.Start.Position)) visibleLines then
                                            let tags = line.GetAdornmentTags self
                                            let tagOption = tags |> Seq.tryHead
                                            match tagOption with
                                            | None -> ()
                                            | Some __ ->
                                                let layer = self.CodeLensLayer.Value
                                                let res = createCodeLensUIElementByLine line
                                                match res with
                                                | Some textBox ->
                                                    layer.AddAdornment(line.Extent, self, textBox) |> ignore
                                                | None -> ()
                                }
                            Async.Start (handleAlreadyVisible, cancellationTokenSourceLayoutChanged.Token)
                            // Wait before we add it and check whether the lines are still valid (so visible)
                            // This is important because we don't want lines being display instantly which causes worse performance.
                            do! Async.Sleep(500) 
                            do! Async.SwitchToContext uiContext
                            let view = self.WpfTextView.Value
                            // We need a new snapshot, it could be already outdated
                            let buffer = view.TextBuffer.CurrentSnapshot
                            let firstLine, lastLine =
                                let firstLine, lastLine=  (view.TextViewLines.FirstVisibleLine, view.TextViewLines.LastVisibleLine)
                                buffer.GetLineNumberFromPosition(firstLine.Start.Position),
                                buffer.GetLineNumberFromPosition(lastLine.Start.Position)
                            let lineAlreadyProcessed pos = not(Set.contains (buffer.GetLineNumberFromPosition(pos)) visibleLines)
                            let realNewLines = e.NewOrReformattedLines |> Seq.filter (fun line -> lineAlreadyProcessed line.Start.Position)
                            for line in realNewLines do
                                if line.IsValid then
                                    let tags = line.GetAdornmentTags self
                                    let tagOption = tags |> Seq.tryHead
                                    match tagOption with
                                    | None -> ()
                                    | Some __ ->
                                        try
                                            let layer = self.CodeLensLayer.Value
                                            let da = new DoubleAnimation(From = Nullable 0., To = Nullable 0.5, Duration = new Duration(TimeSpan.FromSeconds(0.8)))
                                            let res = createCodeLensUIElementByLine line
                                            match res with
                                            | Some textBox ->
                                                layer.AddAdornment(line.Extent, self, textBox) |> ignore
                                                textBox.BeginAnimation(UIElement.OpacityProperty, da)
                                            | None -> ()
                                        with
                                        | e -> Logging.Logging.logErrorf "Error occured: %A" e
                                        ()
                            visibleLines <- Set [ firstLine .. lastLine]
                        with
                        | e -> Logging.Logging.logErrorf "Error occured: %A" e
                        ()
                }
            Async.Start (asyncStuff , cancellationTokenSourceLayoutChanged.Token)
            ()
        
        member __.Tags = new Concurrent.ConcurrentDictionary<_, _>()

        interface ITagger<CodeLensTag> with
            [<CLIEvent>]
            member __.TagsChanged = tagsChanged.Publish

            override __.GetTags spans =
                    seq{
                        let translatedSpans = 
                            new NormalizedSnapshotSpanCollection(
                                spans 
                                |> Seq.map (fun span -> span.TranslateTo(buffer.CurrentSnapshot, SpanTrackingMode.EdgeExclusive)))
                        for tagSpan in translatedSpans do
                            let line = buffer.CurrentSnapshot.GetLineFromPosition(tagSpan.Start.Position)
                            let codeLens = codeLensResults
                            if codeLens.ContainsKey(line.GetText()) then
                                let res = TagSpan(tagSpan, CodeLensTag(0., 15., 0., 0., 0., PositionAffinity.Predecessor, self, self)):> ITagSpan<CodeLensTag>
                                yield res
                       }


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
        projectInfoManager: ProjectInfoManager,
        typeMap: Lazy<ClassificationTypeMap>,
        gotoDefinitionService: FSharpGoToDefinitionService
    ) as __ =
    let CodeLens = ResizeArray<_ * _>()
    let componentModel = Package.GetGlobalService(typeof<ComponentModelHost.SComponentModel>) :?> ComponentModelHost.IComponentModel
    let workspace = componentModel.GetService<VisualStudioWorkspace>()

    /// Returns an provider for the textView if already one has been created. Else create one.
    let getSuitableAdornmentProvider (buffer) =
        let res = CodeLens |> Seq.tryFind(fun (view, _) -> view = buffer)
        match res with
        | Some (_, res) -> res
        | None ->
            let documentId = 
                lazy(
                    match textDocumentFactory.TryGetTextDocument(buffer) with
                    | true, textDocument ->
                         Seq.tryHead (workspace.CurrentSolution.GetDocumentIdsWithFilePath(textDocument.FilePath))
                    | _ -> None
                    |> Option.get
                )

            let provider = CodeLensTagger(workspace, documentId, buffer, checkerProvider.Checker, projectInfoManager, typeMap, gotoDefinitionService)
            CodeLens.Add((buffer, provider))
            provider

    [<Export(typeof<AdornmentLayerDefinition>); Name("CodeLens");
      Order(Before = PredefinedAdornmentLayers.Text);
      TextViewRole(PredefinedTextViewRoles.Document)>]
    member val CodeLensAdornmentLayerDefinition : AdornmentLayerDefinition = null with get, set

    interface IWpfTextViewCreationListener with
        override __.TextViewCreated view =
            let tagger = getSuitableAdornmentProvider view.TextBuffer
            tagger.WpfTextView <- Lazy<_>.CreateFromValue view
            tagger.WpfTextView.Force() |> ignore
            tagger.CodeLensLayer <- Lazy<_>.CreateFromValue(view.GetAdornmentLayer "CodeLens")
            tagger.CodeLensLayer.Force() |> ignore
            view.LayoutChanged.AddHandler(fun _ e -> tagger.LayoutChanged e)
            // The view has been initialized. Notify that we can now theoretically display CodeLens
            tagger.TriggerTagsChanged (SnapshotSpanEventArgs(SnapshotSpan(view.TextViewLines.FirstVisibleLine.Start, view.TextViewLines.LastVisibleLine.End)))
            ()

    interface ITaggerProvider with 
        override __.CreateTagger(buffer) =
            let tagger = getSuitableAdornmentProvider buffer
            box (tagger) :?> _

module Test = 
     let a = "a"

//module Test = 
//    let t = ""


//    type Cherry (s, a, b) =
//        let s = s
//        let b = b
//        let a = a

//    let s = ""