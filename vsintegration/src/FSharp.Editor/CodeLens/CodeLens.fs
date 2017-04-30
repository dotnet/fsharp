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
        let mutable codeLensLines = ConcurrentDictionary()

        do assert (documentId <> null)

        let mutable cancellationTokenSourceBufferChanged = new CancellationTokenSource()
        let mutable cancellationTokenBufferChanged = cancellationTokenSourceBufferChanged.Token
    
        let layoutTagToFormatting (layoutTag: LayoutTag) =
            layoutTag
            |> RoslynHelpers.roslynTag
            |> ClassificationTags.GetClassificationTypeName
            |> typeMap.Value.GetClassificationType
            |> formatMap.Value.GetTextProperties   
         
        let executeCodeLenseAsync (__:TextContentChangedEventArgs) =  
            //let uiContext = SynchronizationContext.Current
            asyncMaybe {
                try 
                    Logging.Logging.logInfof "Rechecking code due to buffer edit!"
                    let! document = workspace.CurrentSolution.GetDocument(documentId.Value) |> Option.ofObj
                    let! options = projectInfoManager.TryGetOptionsForEditingDocumentOrProject(document)
                    let! _, _, checkFileResults = checker.ParseAndCheckDocument(document, options, allowStaleResults = true)
                    let! symbolUses = checkFileResults.GetAllUsesOfAllSymbolsInFile() |> liftAsync
                    
                    let view = self.WpfTextView.Value;
                    let textSnapshot = view.TextSnapshot.TextBuffer.CurrentSnapshot
                    Logging.Logging.logInfof "Updating code lens due to buffer edit!"
                    codeLensLines.Clear();
                    //let results = Concurrent.ConcurrentDictionary<_,_>()
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
                                    codeLensLines.[int lineNumber] <- (taggedText, navigation)
                                    let line = textSnapshot.GetLineFromLineNumber(lineNumber)
                                    tagsChanged.Trigger(self, SnapshotSpanEventArgs(SnapshotSpan(line.Start, line.End)))
                                | None -> ()
                        }

                    for symbolUse in symbolUses do
                        if symbolUse.IsFromDefinition then
                            match symbolUse.Symbol with
                            | :? FSharpEntity as entity ->
                                for func in entity.MembersFunctionsAndValues do
                                    do useResults (symbolUse.DisplayContext, func) |> Async.Start
                            | _ -> ()
                    Logging.Logging.logInfof "Extraction complete, replacing old code lens data!"
                    //do! Async.SwitchToContext uiContext |> liftAsync
                    //let handler = tagsChanged;
                    //handler.Trigger(self, SnapshotSpanEventArgs(SnapshotSpan(view.TextViewLines.FirstVisibleLine.Start, view.TextViewLines.LastVisibleLine.End)))
                    Logging.Logging.logInfof "Finished updating code lens." |> ignore
                with
                | ex -> Logging.Logging.logErrorf "Error occured: %A" ex
                        
            }
       
        let createCodeLensUIElementByLine (line:ITextViewLine) =
            let view = self.WpfTextView.Value
            let offset = 
                [0..line.Length - 1] |> Seq.tryFind (fun i -> not (Char.IsWhiteSpace (line.Start.Add(i).GetChar())))
                |> Option.defaultValue 0
            let realStart = line.Start.Add(offset)
            let span = SnapshotSpan(line.Snapshot, Span.FromBounds(int realStart, int line.End))
            let geometry = view.TextViewLines.GetMarkerGeometry(span)
            let taggedText, navigation = codeLensLines.[buffer.CurrentSnapshot.GetLineNumberFromPosition(line.Start.Position)]
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
            textBox


        do buffer.Changed.AddHandler(fun _ e -> (self.BufferChanged e))
        member __.BufferChanged e =
            cancellationTokenSourceBufferChanged.Cancel() // Stop all ongoing async workflow. 
            cancellationTokenSourceBufferChanged.Dispose()
            cancellationTokenSourceBufferChanged <- new CancellationTokenSource()
            cancellationTokenBufferChanged <- cancellationTokenSourceBufferChanged.Token
            executeCodeLenseAsync e |> Async.Ignore |> RoslynHelpers.StartAsyncSafe cancellationTokenBufferChanged
                
        member val WpfTextView : Lazy<IWpfTextView> = Lazy<IWpfTextView>() with get,set 

        member val CodeLensLayer = Lazy<IAdornmentLayer>() with get, set
        
        member t.T = ""

        member __.LayoutChanged (e:TextViewLayoutChangedEventArgs) =
            let uiContext = SynchronizationContext.Current

            //cancellationTokenSourceLayoutChanged.Cancel()
            //cancellationTokenSourceLayoutChanged.Dispose()
            //cancellationTokenSourceLayoutChanged <- new CancellationTokenSource()
            async{
                
                try
                    do! Async.Sleep(1000) //Wait before we add it and check whether the lines are still valid (so visible) if not, nothing will happen
                    do! Async.SwitchToContext uiContext
                    let view = self.WpfTextView.Value
                    let visibleSnapshot = SnapshotSpan(view.TextViewLines.FirstVisibleLine.Start, view.TextViewLines.LastVisibleLine.End)
                    for line in e.NewOrReformattedLines do
                        if line.IsValid && visibleSnapshot.Contains(line.Extent) then
                            let tags = line.GetAdornmentTags self
                            let tagOption = tags |> Seq.tryHead
                            match tagOption with
                            | None -> ()
                            | Some tag ->
                                try
                                    if not self.CodeLensLayer.IsValueCreated then
                                        self.CodeLensLayer.Force() |> ignore
                                    let layer = self.CodeLensLayer.Value
                                    let da = new DoubleAnimation(From = Nullable 0., To = Nullable 0.5, Duration = new Duration(TimeSpan.FromSeconds(2.)))
                                    let textBox = createCodeLensUIElementByLine line
                                    layer.AddAdornment(line.Extent, tag, textBox) |> ignore
                                    do textBox.BeginAnimation(UIElement.OpacityProperty, da)
                                with
                                | e -> Logging.Logging.logErrorf "Error occured: %A" e
                                ()
                with
                | e -> Logging.Logging.logErrorf "Error occured: %A" e
                ()
            } |> Async.Start
            ()
        
        member __.Tags = new Concurrent.ConcurrentDictionary<_, _>()

        interface ITagger<CodeLensTag> with
            [<CLIEvent>]
            member this.TagsChanged = tagsChanged.Publish

            override __.GetTags spans =
                    seq{
                        let translatedSpans = 
                            new NormalizedSnapshotSpanCollection(
                                spans 
                                |> Seq.map (fun span -> span.TranslateTo(buffer.CurrentSnapshot, SpanTrackingMode.EdgeExclusive)))
                        for tagSpan in translatedSpans do
                            let lineNumber = buffer.CurrentSnapshot.GetLineNumberFromPosition(tagSpan.Start.Position)
                            let codeLens = codeLensLines
                            if codeLens.ContainsKey(lineNumber) then
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
            tagger.CodeLensLayer <- Lazy<_>.CreateFromValue(view.GetAdornmentLayer "CodeLens")
            view.LayoutChanged.AddHandler(fun _ e -> tagger.LayoutChanged e)
            view.DisplayTextLineContainingBufferPosition(view.TextViewLines.FirstVisibleLine.Start, 0., ViewRelativePosition.Top)
            ()

    interface ITaggerProvider with
        override __.CreateTagger(buffer) =
            let tagger = getSuitableAdornmentProvider buffer
            box (tagger) :?> _