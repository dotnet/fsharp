// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor


open System
open System.Collections.Generic
open System.Threading
open System.Windows
open System.Windows.Controls
open System.Windows.Media
open System.Windows.Media.Animation

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Classification
open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Editor.Shared.Extensions

open FSharp.Compiler.Symbols
open FSharp.Compiler.Syntax
open FSharp.Compiler.Text

open Microsoft.VisualStudio.FSharp.Editor.Logging
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.Text.Classification

open Microsoft.CodeAnalysis.ExternalAccess.FSharp.Editor.Shared.Utilities

type internal Lens(taggedText, computed, funcID, uiElement) =
    member val TaggedText: Async<(ResizeArray<TaggedText> * FSharpNavigation) option> = taggedText
    member val Computed: bool = computed with get, set
    member val FuncID: string = funcID
    member val UiElement: UIElement = uiElement with get, set

type internal LensService
    (
        serviceProvider: IServiceProvider,
        workspace: Workspace, 
        documentId: DocumentId,
        buffer: ITextBuffer, 
        metadataAsSource: FSharpMetadataAsSourceService,
        classificationFormatMapService: IClassificationFormatMapService,
        typeMap: Lazy<FSharpClassificationTypeMap>,
        lensDisplayService : LensDisplayService,
        settings: EditorOptions
    ) as self =

    let visit pos parseTree = 
        SyntaxTraversal.Traverse(pos, parseTree, { new SyntaxVisitorBase<_>() with 
            member _.VisitExpr(_path, traverseSynExpr, defaultTraverse, expr) =
                defaultTraverse(expr)
            
            override _.VisitInheritSynMemberDefn (_, _, _, _, _, range) = Some range

            override _.VisitTypeAbbrev(_, _, range) = Some range

            override _.VisitLetOrUse(_, _, _, bindings, range) =
                match bindings |> Seq.tryFind (fun b -> b.RangeOfHeadPattern.StartLine = pos.Line) with
                | Some entry ->
                    Some entry.RangeOfBindingWithRhs
                | None ->
                    // We choose to use the default range because
                    // it wasn't possible to find the complete range
                    // including implementation code.
                    Some range

            override _.VisitBinding (_, fn, binding) =
                Some binding.RangeOfBindingWithRhs
        })

    let formatMap = lazy classificationFormatMapService.GetClassificationFormatMap "tooltip"

    let mutable lastResults = Dictionary<string, ITrackingSpan * Lens>()
    let mutable firstTimeChecked = false
    let mutable bufferChangedCts = new CancellationTokenSource()
    let uiContext = SynchronizationContext.Current

    let layoutTagToFormatting (layoutTag: TextTag) =
        layoutTag
        |> RoslynHelpers.roslynTag
        |> FSharpClassificationTags.GetClassificationTypeName
        |> typeMap.Value.GetClassificationType
        |> formatMap.Value.GetTextProperties   

    let createTextBox (lens:Lens) =
        async {
            do! Async.SwitchToContext uiContext
            let! res = lens.TaggedText
            match res with
            | Some (taggedText, navigation) ->
#if DEBUG
                logInfof "Tagged text %A" taggedText
#endif
                let textBlock = new TextBlock(Background = Brushes.AliceBlue, Opacity = 0.0, TextTrimming = TextTrimming.None)
                FSharpDependencyObjectExtensions.SetDefaultTextProperties(textBlock, formatMap.Value)

                let prefix = Documents.Run settings.Lens.Prefix
                prefix.Foreground <- SolidColorBrush(Color.FromRgb(153uy, 153uy, 153uy))
                textBlock.Inlines.Add prefix

                for text in taggedText do

                    let coloredProperties = layoutTagToFormatting text.Tag
                    let actualProperties =
                        if settings.Lens.UseColors
                        then
                            // If color is gray (R=G=B), change to correct gray color.
                            // Otherwise, use the provided color.
                            match coloredProperties.ForegroundBrush with
                            | :? SolidColorBrush as b ->
                                let c = b.Color
                                if c.R = c.G && c.R = c.B
                                then coloredProperties.SetForeground(Color.FromRgb(153uy, 153uy, 153uy))
                                else coloredProperties
                            | _ -> coloredProperties
                        else
                            coloredProperties.SetForeground(Color.FromRgb(153uy, 153uy, 153uy))

                    let run = Documents.Run text.Text
                    FSharpDependencyObjectExtensions.SetTextProperties (run, actualProperties)

                    let inl =
                        match text with
                        | :? NavigableTaggedText as nav when navigation.IsTargetValid nav.Range ->
                            let h = Documents.Hyperlink(run, ToolTip = nav.Range.FileName)
                            h.Click.Add (fun _ -> 
                                navigation.NavigateTo(nav.Range, CancellationToken.None))
                            h :> Documents.Inline
                        | _ -> run :> _
                    FSharpDependencyObjectExtensions.SetTextProperties (inl, actualProperties)
                    textBlock.Inlines.Add inl
            

                textBlock.Measure(Size(Double.PositiveInfinity, Double.PositiveInfinity))
                lens.Computed <- true
                lens.UiElement <- textBlock
                return true
            | _ -> 
                return false
        }  

    let executeLensAsync () =  
        asyncMaybe {
            do! Async.Sleep 800 |> liftAsync
#if DEBUG
            logInfof "Rechecking code due to buffer edit!"
#endif
            let! document = workspace.CurrentSolution.GetDocument documentId |> Option.ofObj
            let! parseFileResults, checkFileResults = document.GetFSharpParseAndCheckResultsAsync(nameof(FSharpUseMutationWhenValueIsMutableFixProvider)) |> liftAsync
            let parsedInput = parseFileResults.ParseTree
#if DEBUG
            logInfof "Getting uses of all symbols!"
#endif
            let! ct = Async.CancellationToken |> liftAsync
            let symbolUses = checkFileResults.GetAllUsesOfAllSymbolsInFile(ct)
            let textSnapshot = buffer.CurrentSnapshot
#if DEBUG
            logInfof "Updating due to buffer edit!"
#endif
            // Clear existing data and cache flags
            // The results which are left.
            let oldResults = Dictionary(lastResults)

            let newResults = Dictionary()
            // Symbols which cache wasn't found yet
            let unattachedSymbols = ResizeArray()
            // Tags which are new or need to be updated due to changes.
            let tagsToUpdate = Dictionary()
            let lensToAdd = ResizeArray()

            let useResults (displayContext: FSharpDisplayContext, func: FSharpMemberOrFunctionOrValue, realPosition: range) =
                async {
                    try
                        let textSnapshot = buffer.CurrentSnapshot
                        let lineNumber = Line.toZ func.DeclarationLocation.StartLine
                        if (lineNumber >= 0 || lineNumber < textSnapshot.LineCount) then
                            match func.FullTypeSafe with
                            | Some _ ->
                                let maybeContext = checkFileResults.GetDisplayContextForPos(func.DeclarationLocation.Start)
                            
                                let displayContext = Option.defaultValue displayContext maybeContext
                                let typeLayout = func.FormatLayout displayContext
                                let taggedText = ResizeArray()        
                                typeLayout |> Seq.iter taggedText.Add
                                let statusBar = StatusBar(serviceProvider.GetService<SVsStatusbar, IVsStatusbar>()) 
                                let navigation = FSharpNavigation(statusBar, metadataAsSource, document, realPosition)
                                // Because the data is available notify that this line should be updated, displaying the results
                                return Some (taggedText, navigation)
                            | None -> 
#if DEBUG
                                logWarningf "Couldn't acquire Lens data for function %A" func
#endif
                                return None
                        else return None
                    with e ->
#if DEBUG
                        logErrorf "Error in lazy lens computation. %A" e
#else
                        ignore e
#endif
                        return None
                }
                
            let inline setNewResultsAndWarnIfOverriden fullDeclarationText value = 
#if DEBUG
                if newResults.ContainsKey fullDeclarationText then
                    logWarningf "New results already contains: %A" fullDeclarationText
#endif
                newResults.[fullDeclarationText] <- value
                
            for symbolUse in symbolUses do
                if symbolUse.IsFromDefinition then
                    match symbolUse.Symbol with
                    | :? FSharpMemberOrFunctionOrValue as func when func.IsModuleValueOrMember || func.IsProperty ->
                        let funcID = func.LogicalName + (func.FullType.ToString() |> hash |> string)
                        // Try to re-use the last results
                        if lastResults.ContainsKey funcID then
                            // Make sure that the results are usable
                            let inline setNewResultsAndWarnIfOverridenLocal value = setNewResultsAndWarnIfOverriden funcID value
                            let lastTrackingSpan, lens as lastResult = lastResults.[funcID]
                            if lens.FuncID = funcID then
                                setNewResultsAndWarnIfOverridenLocal lastResult
                                oldResults.Remove funcID |> ignore
                            else
                                let declarationLine, range = 
                                    match visit func.DeclarationLocation.Start parsedInput with
                                    | Some range -> range.StartLine - 1, range
                                    | _ -> func.DeclarationLocation.StartLine - 1, func.DeclarationLocation
                                // Track the old element for removal
                                let declarationSpan = 
                                    let line = textSnapshot.GetLineFromLineNumber declarationLine
                                    let offset = line.GetText() |> Seq.findIndex (Char.IsWhiteSpace >> not)
                                    SnapshotSpan(line.Start.Add offset, line.End).Span
                                let newTrackingSpan = 
                                    textSnapshot.CreateTrackingSpan(declarationSpan, SpanTrackingMode.EdgeExclusive)
                                // Push back the new results
                                let res =
                                        Lens( Async.cache (useResults (symbolUse.DisplayContext, func, range)),
                                            false,
                                            funcID,
                                            null)
                                // The old results aren't computed at all, because the line might have changed create new results
                                tagsToUpdate.[lastTrackingSpan] <- (newTrackingSpan, funcID, res)
                                setNewResultsAndWarnIfOverridenLocal (newTrackingSpan, res)
                                        
                                oldResults.Remove funcID |> ignore
                        else
                            // The symbol might be completely new or has slightly changed. 
                            // We need to track this and iterate over the left entries to ensure that there isn't anything
                            unattachedSymbols.Add(symbolUse, func, funcID)
                    | _ -> ()
            
            // In best case this works quite `covfefe` fine because often enough we change only a small part of the file and not the complete.
            for unattachedSymbol in unattachedSymbols do
                let symbolUse, func, funcID = unattachedSymbol
                let declarationLine, range = 
                    match visit func.DeclarationLocation.Start parsedInput with
                    | Some range -> range.StartLine - 1, range
                    | _ -> func.DeclarationLocation.StartLine - 1, func.DeclarationLocation
                    
                let test (v:KeyValuePair<_, _>) =
                    let _, (lens:Lens) = v.Value
                    lens.FuncID = funcID
                match oldResults |> Seq.tryFind test with
                | Some res ->
                    let (trackingSpan : ITrackingSpan), (lens : Lens) = res.Value
                    let declarationSpan = 
                        let line = textSnapshot.GetLineFromLineNumber declarationLine
                        let offset = line.GetText() |> Seq.findIndex (Char.IsWhiteSpace >> not)
                        SnapshotSpan(line.Start.Add offset, line.End).Span
                    let newTrackingSpan = 
                        textSnapshot.CreateTrackingSpan(declarationSpan, SpanTrackingMode.EdgeExclusive)
                    if lens.Computed && (isNull lens.UiElement |> not) then
                        newResults.[funcID] <- (newTrackingSpan, lens)
                        tagsToUpdate.[trackingSpan] <- (newTrackingSpan, funcID, lens)
                    else
                        let res = 
                            Lens(
                                Async.cache (useResults (symbolUse.DisplayContext, func, range)),
                                false,
                                funcID,
                                null)
                        // The tag might be still valid but it hasn't been computed yet so create fresh results
                        tagsToUpdate.[trackingSpan] <- (newTrackingSpan, funcID, res)
                        newResults.[funcID] <- (newTrackingSpan, res)
                    let key = res.Key
                    oldResults.Remove key |> ignore // no need to check this entry again
                | None ->
                    // This function hasn't got any cache and so it's completely new.
                    // So create completely new results
                    // And finally add a tag for this.
                    let res = 
                        Lens(
                            Async.cache (useResults (symbolUse.DisplayContext, func, range)),
                            false,
                            funcID,
                            null)
                    try
                        let declarationSpan = 
                            let line = textSnapshot.GetLineFromLineNumber declarationLine
                            let offset = line.GetText() |> Seq.findIndex (Char.IsWhiteSpace >> not)
                            SnapshotSpan(line.Start.Add offset, line.End).Span
                        let trackingSpan = 
                            textSnapshot.CreateTrackingSpan(declarationSpan, SpanTrackingMode.EdgeExclusive)
                        lensToAdd.Add (trackingSpan, res)
                        newResults.[funcID] <- (trackingSpan, res)
                    with e ->
#if DEBUG
                        logExceptionWithContext (e, "Lens tracking tag span creation")
#endif
                        ignore e
                ()
            lastResults <- newResults
            do! Async.SwitchToContext uiContext |> liftAsync
            let createLensUIElement (lens:Lens) trackingSpan _ =
                if lens.Computed |> not then
                    async {
                        let! res = createTextBox lens
                        if res then
                            do! Async.SwitchToContext uiContext
#if DEBUG
                            logInfof "Adding ui element for %A" (lens.TaggedText)
#endif
                            let uiElement = lens.UiElement
                            let animation = 
                                DoubleAnimation(
                                    To = Nullable 0.8,
                                    Duration = (TimeSpan.FromMilliseconds 800. |> Duration.op_Implicit),
                                    EasingFunction = QuadraticEase()
                                    )
                            let sb = Storyboard()
                            Storyboard.SetTarget(sb, uiElement)
                            Storyboard.SetTargetProperty(sb, PropertyPath Control.OpacityProperty)
                            sb.Children.Add animation
                            lensDisplayService.AddUiElementToLensOnce (trackingSpan, uiElement)
                            lensDisplayService.RelayoutRequested.Enqueue ()
                            sb.Begin()
                        else
#if DEBUG
                            logWarningf "Couldn't retrieve lens information for %A" lens.FuncID
#endif
                            ()
                    } |> (RoslynHelpers.StartAsyncSafe CancellationToken.None) "UIElement creation"

            for value in tagsToUpdate do
                let trackingSpan, (newTrackingSpan, _, lens) = value.Key, value.Value
                lensDisplayService.RemoveLens trackingSpan |> ignore
                let Grid = lensDisplayService.AddLens newTrackingSpan
                if lens.Computed && (isNull lens.UiElement |> not) then
                    let uiElement = lens.UiElement
                    lensDisplayService.AddUiElementToLensOnce (newTrackingSpan, uiElement)
                else
                    Grid.IsVisibleChanged
                    |> Event.filter (fun eventArgs -> eventArgs.NewValue :?> bool)
                    |> Event.add (createLensUIElement lens newTrackingSpan)

            for value in lensToAdd do
                let trackingSpan, lens = value
                let Grid = lensDisplayService.AddLens trackingSpan
#if DEBUG
                logInfof "Trackingspan %A is being added." trackingSpan
#endif
                
                Grid.IsVisibleChanged
                |> Event.filter (fun eventArgs -> eventArgs.NewValue :?> bool)
                |> Event.add (createLensUIElement lens trackingSpan)

            for oldResult in oldResults do
                let trackingSpan, _ = oldResult.Value
                lensDisplayService.RemoveLens trackingSpan |> ignore
#if DEBUG
            logInfof "Finished updating lens."
#endif
            
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
                      do! executeLensAsync()
                      do! Async.Sleep(1000)
                  with
                  | e ->
#if DEBUG
                      logErrorf "Lens startup failed with: %A" e
#else
                      ignore e
#endif
                      numberOfFails <- numberOfFails + 1
           } |> Async.Start
        end

    member _.BufferChanged ___ =
        bufferChangedCts.Cancel() // Stop all ongoing async workflow. 
        bufferChangedCts.Dispose()
        bufferChangedCts <- new CancellationTokenSource()
        executeLensAsync () |> Async.Ignore |> RoslynHelpers.StartAsyncSafe bufferChangedCts.Token "Buffer Changed"
