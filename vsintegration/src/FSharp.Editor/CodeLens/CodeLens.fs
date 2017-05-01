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
open Microsoft.VisualStudio.FSharp.Editor.Logging

type CodeLensTag(width, topSpace, baseline, textHeight, bottomSpace, affinity, tag:obj, providerTag:obj) =
    inherit SpaceNegotiatingAdornmentTag(width, topSpace, baseline, textHeight, bottomSpace, affinity, tag, providerTag)

type internal CodeLens =
    { TaggedText: Async<(ResizeArray<Layout.TaggedText> * QuickInfoNavigation) option> 
      Shown: bool }

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
    let codeLensResults = ConcurrentDictionary<string, CodeLens>()
    let codeLensUIElementCache = ConcurrentDictionary()
    let visibleLines = ConcurrentDictionary()
    let mutable firstTimeChecked = false
    let mutable bufferChangedCts = new CancellationTokenSource()
    let mutable layoutChangedCts = new CancellationTokenSource()
    let mutable view: IWpfTextView option = None
    let mutable codeLensLayer: IAdornmentLayer option = None
    
    let layoutTagToFormatting (layoutTag: LayoutTag) =
        layoutTag
        |> RoslynHelpers.roslynTag
        |> ClassificationTags.GetClassificationTypeName
        |> typeMap.Value.GetClassificationType
        |> formatMap.Value.GetTextProperties   

    let executeCodeLenseAsync () =  
        let uiContext = SynchronizationContext.Current
        asyncMaybe {
            let! view = view
            do! Async.Sleep 500 |> liftAsync
            logInfof "Rechecking code due to buffer edit!"
            let! document = workspace.CurrentSolution.GetDocument(documentId.Value) |> Option.ofObj
            let! options = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document)
            let! _, _, checkFileResults = checker.ParseAndCheckDocument(document, options, allowStaleResults = true)
            logInfof "Getting uses of all symbols!"
            let! symbolUses = checkFileResults.GetAllUsesOfAllSymbolsInFile() |> liftAsync
            let textSnapshot = view.TextSnapshot.TextBuffer.CurrentSnapshot
            logInfof "Updating code lens due to buffer edit!"

            // Clear existing data and cache flags
            codeLensResults.Clear()

            let useResults (displayContext: FSharpDisplayContext, func: FSharpMemberOrFunctionOrValue, lineStr: string) =
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
                            
                            // The line might be visible but doesn't displaying the results yet, force it being redrawn.
                            visibleLines.TryRemove lineNumber |> ignore
                            let line = textSnapshot.GetLineFromLineNumber(lineNumber)
                            // This is only used with a cached async so flag the data as ready to use
                            codeLensResults.[lineStr] <- { codeLensResults.[lineStr] with Shown = true }
                            // Because the data is available notify that this line should be updated, displaying the results
                            tagsChanged.Trigger(self, SnapshotSpanEventArgs(SnapshotSpan(line.Start, line.End)))
                            return Some (taggedText, navigation)
                        | None -> return None
                    else return None
                }

            for symbolUse in symbolUses do
                if symbolUse.IsFromDefinition then
                    match symbolUse.Symbol with
                    | :? FSharpEntity as entity ->
                        for func in entity.MembersFunctionsAndValues do
                            let lineNumber = Line.toZ func.DeclarationLocation.StartLine
                            let text = textSnapshot.GetLineFromLineNumber(int lineNumber).GetText()
                            
                            codeLensResults.[text] <- 
                                { TaggedText = Async.cache (useResults (symbolUse.DisplayContext, func, text))
                                  Shown = false }
                    | _ -> ()

            do! Async.SwitchToContext uiContext |> liftAsync
            tagsChanged.Trigger(self, SnapshotSpanEventArgs(SnapshotSpan(view.TextViewLines.FirstVisibleLine.Start, view.TextViewLines.LastVisibleLine.End)))
            logInfof "Finished updating code lens."
            
            if not firstTimeChecked then
                firstTimeChecked <- true
        } |> Async.Ignore
    
    do async {
          while not firstTimeChecked do
              do! executeCodeLenseAsync()
              do! Async.Sleep(1000)
       } |> Async.Start
    
    /// Creates the code lens ui elements for the specified line
    /// TODO, add caching to the UI elements
    let createCodeLensUIElementByLine (line: ITextViewLine) =
        let text = line.Extent.GetText()
        asyncMaybe {
            if codeLensUIElementCache.ContainsKey(text) then
                // Use existing UI element which is proved to be safe to use
                return codeLensUIElementCache.[text]
            else
                let! lens = codeLensResults.TryFind(text) // Check whether this line has code lens
                // No delay because it's already computed
                let! taggedText, navigation = lens.TaggedText
                let! view = view
                // Get the real offset so that the code lens are placed correctly
                let offset = 
                    [0..line.Length - 1] |> Seq.tryFind (fun i -> not (Char.IsWhiteSpace (line.Start.Add(i).GetChar())))
                    |> Option.defaultValue 0

                let realStart = line.Start.Add(offset)
                let span = SnapshotSpan(line.Snapshot, Span.FromBounds(int realStart, int line.End))
                let geometry = view.TextViewLines.GetMarkerGeometry(span)
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
                return textBox
        } |> Async.map (fun ui ->
               match ui with
               | Some ui -> codeLensUIElementCache.TryAdd(text, ui) |> ignore
               | None -> 
                   let unusedElement = null
                   codeLensUIElementCache.TryRemove(text, &unusedElement) |> ignore
               ui)

    do buffer.Changed.AddHandler(fun _ e -> (self.BufferChanged e))

    member __.BufferChanged ___ =
        bufferChangedCts.Cancel() // Stop all ongoing async workflow. 
        bufferChangedCts.Dispose()
        bufferChangedCts <- new CancellationTokenSource()
        executeCodeLenseAsync () |> Async.Ignore |> RoslynHelpers.StartAsyncSafe bufferChangedCts.Token
            
    member __.SetView value = 
        view <- Some value
        codeLensLayer <- Some (value.GetAdornmentLayer "CodeLens")

    member __.TriggerTagsChanged args = tagsChanged.Trigger(self, args)

    member this.LayoutChanged (e:TextViewLayoutChangedEventArgs) =
        let uiContext = SynchronizationContext.Current
        layoutChangedCts.Cancel()
        layoutChangedCts.Dispose()
        layoutChangedCts <- new CancellationTokenSource()

        asyncMaybe {
            //if firstTimeChecked then
                do! Async.SwitchToContext uiContext |> liftAsync
                let! view = view
                let! layer = codeLensLayer
                let buffer = view.TextBuffer.CurrentSnapshot
                
                for line in e.NewOrReformattedLines do
                    if visibleLines.ContainsKey (buffer.GetLineNumberFromPosition(line.Start.Position)) then
                        if not (Seq.isEmpty (line.GetAdornmentTags this)) then
                            let! res = createCodeLensUIElementByLine line |> liftAsync
                            match res with
                            | Some textBox -> layer.AddAdornment(line.Extent, this, textBox) |> ignore
                            | None -> ()

                // Wait before we add it and check whether the lines are still valid (so visible)
                // This is important because we don't want lines being display instantly which causes worse performance.
                do! Async.Sleep(500) |> liftAsync
                // We need a new snapshot, it could be already outdated
                let buffer = view.TextBuffer.CurrentSnapshot
                
                let firstLine, lastLine =
                    let firstLine, lastLine=  (view.TextViewLines.FirstVisibleLine, view.TextViewLines.LastVisibleLine)
                    buffer.GetLineNumberFromPosition(firstLine.Start.Position),
                    buffer.GetLineNumberFromPosition(lastLine.Start.Position)

                let lineAlreadyProcessed pos = not (visibleLines.ContainsKey (buffer.GetLineNumberFromPosition(pos)))
                let realNewLines = e.NewOrReformattedLines |> Seq.filter (fun line -> lineAlreadyProcessed line.Start.Position)
                
                for line in realNewLines do
                    if line.IsValid then
                        if not (Seq.isEmpty (line.GetAdornmentTags this)) then
                            let da = DoubleAnimation(From = Nullable 0., To = Nullable 0.5, Duration = Duration(TimeSpan.FromSeconds 0.8))
                            let! res = createCodeLensUIElementByLine line |> liftAsync
                            match res with
                            | Some textBox ->
                                layer.AddAdornment(line.Extent, this, textBox) |> ignore
                                textBox.BeginAnimation(UIElement.OpacityProperty, da)
                            | None -> ()
                
                for line in firstLine..lastLine do
                    visibleLines.[line] <- ()
        }
        |> Async.Ignore
        |> RoslynHelpers.StartAsyncSafe layoutChangedCts.Token
    
    member __.Tags = ConcurrentDictionary()

    interface ITagger<CodeLensTag> with
        [<CLIEvent>]
        member __.TagsChanged = tagsChanged.Publish

        override this.GetTags spans =
            seq {
                let translatedSpans = 
                    spans 
                    |> Seq.map (fun span -> span.TranslateTo(buffer.CurrentSnapshot, SpanTrackingMode.EdgeExclusive))
                    |> NormalizedSnapshotSpanCollection
                
                for tagSpan in translatedSpans do
                    let line = buffer.CurrentSnapshot.GetLineFromPosition(tagSpan.Start.Position)
                    if codeLensResults.ContainsKey(line.GetText()) then
                        yield TagSpan(tagSpan, CodeLensTag(0., 15., 0., 0., 0., PositionAffinity.Predecessor, this, this)) :> ITagSpan<CodeLensTag>
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

            let provider = CodeLensTagger(workspace, documentId, buffer, checkerProvider.Checker, projectInfoManager, typeMap, gotoDefinitionService)
            taggers.Add((buffer, provider))
            provider

    [<Export(typeof<AdornmentLayerDefinition>)>]
    [<Name "CodeLens">]
    [<Order(Before = PredefinedAdornmentLayers.Text)>]
    [<TextViewRole(PredefinedTextViewRoles.Document)>]
    member val CodeLensAdornmentLayerDefinition : AdornmentLayerDefinition = null with get, set

    interface IWpfTextViewCreationListener with
        override __.TextViewCreated view =
            let tagger = getSuitableAdornmentProvider view.TextBuffer
            tagger.SetView view
            view.LayoutChanged.AddHandler(fun _ e -> tagger.LayoutChanged e)
            // The view has been initialized. Notify that we can now theoretically display CodeLens
            tagger.TriggerTagsChanged (SnapshotSpanEventArgs(SnapshotSpan(view.TextViewLines.FirstVisibleLine.Start, view.TextViewLines.LastVisibleLine.End)))
            ()

    interface ITaggerProvider with 
        override __.CreateTagger(buffer) =
            let tagger = getSuitableAdornmentProvider buffer
            box (tagger) :?> _