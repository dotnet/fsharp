#nowarn "40"

namespace Microsoft.VisualStudio.FSharp.LanguageService

open Internal.Utilities.Collections
open Microsoft.FSharp.Compiler.SourceCodeServices
open System
open System.Text
open System.IO
open System.Collections.Generic
open System.Collections
open System.Configuration
open System.Diagnostics
open System.Globalization
open System.Threading
open System.ComponentModel.Design
open System.Runtime.InteropServices
open Microsoft.VisualStudio
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop 
open Microsoft.VisualStudio.Text.Editor
open Microsoft.VisualStudio.Text.Formatting
open Microsoft.VisualStudio.TextManager.Interop 
open Microsoft.VisualStudio.OLE.Interop
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics 
open Microsoft.FSharp.Compiler.Lib
open Internal.Utilities.Debug
open Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.VisualStudio.Editor
open Microsoft.VisualStudio.Text

#nowarn "45" // This method will be made public in the underlying IL because it may implement an interface or override a method

module internal Source =
    type DelegatingSource(  recolorizeWholeFile:unit->unit,
                            recolorizeLine:int->unit,
                            currentFileName:unit -> string,
                            isClosed:unit->bool,
                            fileChangeEx:IVsFileChangeEx) =         
            
        let projectSite : IProjectSite option ref = ref None
        let dependencyFileChangeCallback : (ProjectSiteRebuildCallbackSignature*DependencyFileChangeCallbackSignature) option ref = ref None

        let mutable isDisposed = false
        let mutable hasCalledSetDependencyFilesAtLeastOnce = false            
        let lastDependencies = new Dictionary<string,uint32>()  // filename, cookie
        let fileChangeFlags = _VSFILECHANGEFLAGS.VSFILECHG_Add ||| 
                              // _VSFILECHANGEFLAGS.VSFILECHG_Del ||| // don't listen for deletes - if a file (such as a 'Clean'ed project reference) is deleted, just keep using stale info
                              _VSFILECHANGEFLAGS.VSFILECHG_Time
        let fileChangeFlags : uint32 = uint32 fileChangeFlags
        
        let mutable isDirty = true
        let mutable changeCount = 0
        let mutable dirtyTime = 0
        
        let IncrementWithWrap(v:int) =
            if v = Int32.MaxValue then 0 else v + 1    
        
        interface IdealSource with 
            member source.RecolorizeWholeFile() = recolorizeWholeFile() 
            member source.RecolorizeLine line = recolorizeLine line
            
            member source.ChangeCount
                with get() = changeCount
                and set(value) = changeCount <- value
            member source.DirtyTime
                with get() = dirtyTime
                and set(value) = dirtyTime <- value   
            member source.RecordChangeToView() = 
                isDirty <- true
                dirtyTime <- System.Environment.TickCount; // NOTE: If called fast enough, it is possible for dirtyTime to have the same value as it had before.
                changeCount <- IncrementWithWrap(changeCount)
            member source.RecordViewRefreshed() = 
                isDirty <- false
            member source.NeedsVisualRefresh 
                with get() = isDirty
            member source.IsClosed 
                with get() = isClosed()
            member source.ProjectSite
                with get() = !projectSite // REVIEW: Could get this from IVsTextBuffer->IVsHierarchy->IProjectSite
                and set(value) = projectSite := value
            /// returns true if the set of dependency files we're watching changes, false otherwise (except that it always returns false on the first call to this method on this object)
            member source.SetDependencyFiles(files) = 
                let changeEvents = source :> IVsFileChangeEvents
                // Note that adding a new dependency for a file and then removing the old one (for that same file) risks missing notifications, 
                // As a result, we compute the diffs, and only act on the diff.
                
                // figure out dependencies that are no longer needed
                let cookiesToRemove = ref []
                let filesToRemove = ref []
                let newFilesSet = new System.Collections.Generic.HashSet<string>(files)  // may do lots of .Contains() calls, so do this to avoid O(N^2)
                for key in lastDependencies.Keys do
                    if not( newFilesSet.Contains(key) ) then
                        filesToRemove := key :: !filesToRemove
                        cookiesToRemove := lastDependencies.[key] :: !cookiesToRemove
                // remove from local dictionary
                Trace.PrintLine("ChangeEvents", (fun () -> sprintf "IdealSource() will stop watching %A" filesToRemove))
                for key in !filesToRemove do
                    lastDependencies.Remove(key) |> ignore
                    
                // figure out new dependencies that must be added
                let filesToAdd = ref []
                for file in files do
                    if not( lastDependencies.ContainsKey(file) ) then
                        filesToAdd := file :: !filesToAdd
                Trace.PrintLine("ChangeEvents", (fun () -> sprintf "IdealSource() will start watching %A" filesToAdd))

                let r = hasCalledSetDependencyFilesAtLeastOnce
                hasCalledSetDependencyFilesAtLeastOnce <- true
                
                match !filesToAdd, !cookiesToRemove with
                | [], [] -> false // nothing changed
                | _ ->
                    // talk to IVsFileChangeEx to update set of notifications we want
                    match fileChangeEx with
                    |   null -> ()
                    |   _ ->
                            UIThread.RunSync(fun () ->
                                if not isDisposed then
                                    for file in !filesToAdd do
                                        if not (lastDependencies.ContainsKey file) then 
                                            try
                                                let cookie = Com.ThrowOnFailure1(fileChangeEx.AdviseFileChange(file, fileChangeFlags, changeEvents))
                                                lastDependencies.Add(file, cookie)
                                            with _ -> ()  // can throw, e.g. when file/directory to watch does not exist; this is safe to ignore
                                    for cookie in !cookiesToRemove do
                                        Com.ThrowOnFailure0(fileChangeEx.UnadviseFileChange(cookie))
                            )
                    r
                        
            member source.SetDependencyFileChangeCallback(projectSiteCallback,sourceFileCallback) = 
                dependencyFileChangeCallback := Some(projectSiteCallback,sourceFileCallback)
                           
        // Hook file change events                
        interface IVsFileChangeEvents with 
            member changes.FilesChanged(_count : uint32, files: string [], changeFlags : uint32 []) = 
                match !dependencyFileChangeCallback with
                | None -> failwith "expected a dependencyFileChangeCallback"
                | Some(projectRetypecheckCallback, fileChangeCallback) ->
                    let changeFlags : DependencyChangeCode seq = changeFlags |> Seq.map int |> Seq.map enum
                    let zip : (string*DependencyChangeCode) seq = Seq.zip files changeFlags
                    let zip : (string * DependencyChangeCode) list = Seq.toList zip
                    let LogChangeEvent msg (file,flag) = 
                        Trace.PrintLine("ChangeEvents", (fun () -> sprintf " IdealSource saw change (%A) in file %s %s " flag file msg))
                    let source = changes :> IdealSource
                    let dcc = DependencyChangeCode.FileChanged ||| DependencyChangeCode.TimeChanged
                    match source.ProjectSite with
                    | Some(projectSite) -> 
                        if Trace.ShouldLog("ChangeEvents") then 
                            zip |> List.iter (LogChangeEvent "and is sending to callback")
                        if Seq.forall (fun cf -> cf &&& (~~~ dcc) = DependencyChangeCode.NoChange) changeFlags then
                            fileChangeCallback(currentFileName())
                        else
                            projectRetypecheckCallback(projectSite)                               
                    | None ->
                        if Trace.ShouldLog("ChangeEvents") then 
                            zip |> List.iter (LogChangeEvent "and is NOT sending to callback because there is no project site")
                    0
            member changes.DirectoryChanged(_directory: string) = 0
            
        interface IDisposable with
            member disp.Dispose() =         
                UIThread.MustBeCalledFromUIThread()
                isDisposed <- true
                Trace.PrintLine("ChangeEvents", (fun () -> sprintf "An IdealSource() is being disposed, and thus removing its file watchers"))
                match fileChangeEx with
                |   null -> ()
                |   _ ->
                    for KeyValue(_,cookie) in lastDependencies do
                        fileChangeEx.UnadviseFileChange(cookie) |> ignore
                lastDependencies.Clear()

    /// This is the ideal implementation of the Source concept abstracted from MLS.  
    let internal CreateDelegatingSource
                   (recolorizeWholeFile:unit->unit, 
                    recolorizeLine:int->unit, 
                    filename:string,
                    isClosed:unit->bool,
                    fileChangeEx:IVsFileChangeEx
                    ) = new DelegatingSource(recolorizeWholeFile,recolorizeLine,(fun () -> filename), isClosed,fileChangeEx) :> IdealSource

    [<AllowNullLiteralAttribute>]
    type VSFontsAndColorsHelper private(fontFamily, pointSize, excludedCodeForegroundColorBrush, backgroundBrush) =
        static let Compute(site:System.IServiceProvider) =
            UIThread.MustBeCalledFromUIThread()
            let mutable guidStatementCompletionFC = new Guid("C1614BB1-734F-4a31-BD42-5AE6275E16D2") // GUID_StatementCompletionFC
            let vsFontAndColorStorage = site.GetService(typeof<SVsFontAndColorStorage>) :?> IVsFontAndColorStorage
            let mutable guidTextEditorFontCategory = new Guid("A27B4E24-A735-4d1d-B8E7-9716E1E3D8E0") // Guid for the code editor font and color category. GUID_TextEditorFC
            vsFontAndColorStorage.OpenCategory(&guidTextEditorFontCategory, (uint32 __FCSTORAGEFLAGS.FCSF_LOADDEFAULTS) ||| (uint32 __FCSTORAGEFLAGS.FCSF_NOAUTOCOLORS) ||| (uint32 __FCSTORAGEFLAGS.FCSF_READONLY)) |> ignore
            let itemInfo : ColorableItemInfo[] = Array.zeroCreate 1  
            vsFontAndColorStorage.GetItem("Excluded Code", itemInfo) |> ignore
#if FX_ATLEAST_45
            let fgColorInfo = itemInfo.[0].crForeground 
            let winFormColor = System.Drawing.ColorTranslator.FromOle(int fgColorInfo)
            let color = System.Windows.Media.Color.FromArgb(winFormColor.A, winFormColor.R, winFormColor.G, winFormColor.B)
            let excludedCodeForegroundColorBrush = new System.Windows.Media.SolidColorBrush(color)
#else
            let color = System.Windows.Media.Colors.Blue
            let excludedCodeForegroundColorBrush = new System.Windows.Media.SolidColorBrush(color)
#endif
            vsFontAndColorStorage.GetItem("Plain Text", itemInfo) |> ignore
#if FX_ATLEAST_45
            let bgColorInfo = itemInfo.[0].crBackground
            let winFormColor = System.Drawing.ColorTranslator.FromOle(int bgColorInfo)
            let color = System.Windows.Media.Color.FromArgb(winFormColor.A, winFormColor.R, winFormColor.G, winFormColor.B)
            let backgroundBrush = new System.Windows.Media.SolidColorBrush(color)
#else
            let color = System.Windows.Media.Colors.White
            let backgroundBrush = new System.Windows.Media.SolidColorBrush(color)
#endif
            vsFontAndColorStorage.CloseCategory() |> ignore
            vsFontAndColorStorage.OpenCategory(&guidStatementCompletionFC, (uint32 __FCSTORAGEFLAGS.FCSF_LOADDEFAULTS) ||| (uint32 __FCSTORAGEFLAGS.FCSF_NOAUTOCOLORS) ||| (uint32 __FCSTORAGEFLAGS.FCSF_READONLY)) |> ignore
            let fontInfo : FontInfo[] = Array.zeroCreate 1
            vsFontAndColorStorage.GetFont(null, fontInfo) |> ignore
            let fontFamily = fontInfo.[0].bstrFaceName 
            let pointSize = fontInfo.[0].wPointSize 
            vsFontAndColorStorage.CloseCategory() |> ignore
            fontFamily, pointSize, excludedCodeForegroundColorBrush, backgroundBrush
        static let mutable theInstance = null  // only ever updated on UI thread
        static let WM_SYSCOLORCHANGE : uint32 = 0x0015u
        static let WM_THEMECHANGED : uint32 = 0x031Au
        member private this.getFontFamilyAndPointSizeAndExcludedCodeForegroundColorBrushAndBackgroundBrush() =
            fontFamily, pointSize, excludedCodeForegroundColorBrush, backgroundBrush
        static member GetFontFamilyAndPointSizeAndExcludedCodeForegroundColorBrushAndBackgroundBrush(site:System.IServiceProvider) =
            if theInstance=null then
                theInstance <- new VSFontsAndColorsHelper(Compute(site))
                let vsShell = site.GetService(typeof<SVsShell>) :?> IVsShell
                let mutable k = 0u
                vsShell.AdviseBroadcastMessages({new IVsBroadcastMessageEvents with
                                                    member this.OnBroadcastMessage(msg, _wParam, _lParam) =
                                                        if msg = WM_THEMECHANGED || msg = WM_SYSCOLORCHANGE then
                                                            theInstance <- new VSFontsAndColorsHelper(Compute(site))
                                                        NativeMethods.S_OK
                                                 }, &k) |> ignore
            theInstance.getFontFamilyAndPointSizeAndExcludedCodeForegroundColorBrushAndBackgroundBrush()
           
    type FSharpIntelliSenseToAppearAdornment(view : IWpfTextView, cursorPoint : SnapshotPoint, site : System.IServiceProvider) as this =
        let fontFamily, pointSize, excludedCodeForegroundColorBrush, backgroundBrush = VSFontsAndColorsHelper.GetFontFamilyAndPointSizeAndExcludedCodeForegroundColorBrushAndBackgroundBrush(site)
        // TODO: We should really create our own adornment layer.  It is possible (unlikely) that pre-existing layers may be re-ordered, or that
        // code 'owning' the layer will choose to clear all adornments, for example.  But creating a new adornment layer can only be done via MEF-export, and
        // as of yet, we have not done any MEF-exporting in the language service.  So for now, use the existing VisibleWhitespace layer, and incur some risk, just to 
        // unblock the feature.
        let layer = view.GetAdornmentLayer("VisibleWhitespace") 
        let tag = "FSharpIntelliSenseToAppearAdornment"
        let pointSize = float pointSize * 96.0 / 72.0  // need to convert from pt to px
        do
            // draw it now
            for line in view.TextViewLines do
                if line.VisibilityState = VisibilityState.FullyVisible then
                    this.CreateVisuals(line)

        member this.CreateVisuals(line : ITextViewLine) =
            let textViewLines = view.TextViewLines
            let cursorPoint = cursorPoint.TranslateTo(line.Snapshot, PointTrackingMode.Positive)
            if line.ContainsBufferPosition(cursorPoint) then
                let i = cursorPoint.Position
                if view.TextSnapshot.Length <> 0 then  // if there are no characters in the buffer, then there is no character to text-relative adorn
                    let span = 
                        // The cursor is just after e.g. the '.' they just pressed, so get character just before it...
                        if i-1 < 0 then
                            // ... unless we are at the very start of the buffer and there is no prior character
                            new SnapshotSpan(view.TextSnapshot, Span.FromBounds(i, i+1))
                        else
                            new SnapshotSpan(view.TextSnapshot, Span.FromBounds(i-1, i))
                    let g = textViewLines.GetMarkerGeometry(span)
                    let tb = new System.Windows.Controls.TextBlock(Text=Strings.GetString "IntelliSenseLoading", FontFamily=System.Windows.Media.FontFamily(fontFamily), FontSize=pointSize)
                    tb.Foreground <- excludedCodeForegroundColorBrush
                    let sp = new System.Windows.Controls.StackPanel(Orientation=System.Windows.Controls.Orientation.Horizontal)
                    System.Windows.Documents.TextElement.SetForeground(sp, excludedCodeForegroundColorBrush.GetAsFrozen() :?> System.Windows.Media.Brush)
                    sp.Margin <- System.Windows.Thickness(3.0)
#if FX_ATLEAST_45
                    let sa = new Microsoft.Internal.VisualStudio.PlatformUI.SpinAnimationControl(Width=pointSize, Height=pointSize, IsSpinning=true)
                    sp.Children.Add(sa) |> ignore
#endif
                    sp.Children.Add(new System.Windows.Controls.Canvas(Width=4.0)) |> ignore // spacing between
                    sp.Children.Add(tb) |> ignore
                    let border = new System.Windows.Controls.Border()
                    border.BorderBrush <- System.Windows.SystemColors.ActiveBorderBrush 
                    border.BorderThickness <- System.Windows.Thickness(1.0)
                    border.Background <- backgroundBrush
                    border.Child <- sp
                    System.Windows.Controls.Canvas.SetLeft(border, g.Bounds.Right)
                    System.Windows.Controls.Canvas.SetTop(border, g.Bounds.Bottom + 3.0)
                    layer.AddAdornment(AdornmentPositioningBehavior.TextRelative, System.Nullable(span), tag, border, null) |> ignore

        member this.RemoveSelf() =
            layer.RemoveAdornmentsByTag(tag)

    // Can't untangle from MLS Source. The purpose of this class is to 
    // satisfy the requirement of inheriting from Source. 
#if DEBUG
    [<System.Diagnostics.DebuggerDisplay("SourceOverIdealSource({OriginalFilename})")>]
#endif
    type internal SourceOverIdealSource(service:LanguageService, textLines, colorizer, filechange:IVsFileChangeEx) = 
        inherit SourceImpl(service, textLines, colorizer)
        
        let mutable lastCommentSpan = new TextSpan()
        let mutable idealSource : IdealSource option = None
        let mutable filechange = filechange
        let mutable textLines = textLines

#if DEBUG        
        let originalFilename = VsTextLines.GetFilename textLines
#endif
        let mutable fileName = VsTextLines.GetFilename textLines


        override source.NormalizeErrorString(message) = Microsoft.FSharp.Compiler.ErrorLogger.NormalizeErrorString message
        override source.NewlineifyErrorString(message) = Microsoft.FSharp.Compiler.ErrorLogger.NewlineifyErrorString message

        override source.GetExpressionAtPosition(line, col) = 
            let upi = source.GetParseTree()
            match Microsoft.FSharp.Compiler.SourceCodeServices.UntypedParseInfoImpl.TryFindExpressionIslandInPosition(line, col, upi.ParseTree) with
            | Some islandToEvaluate -> islandToEvaluate
            | None -> null

        member source.GetParseTree() : UntypedParseInfo =
            // get our hands on lss.Parser (InteractiveChecker)
            let ic = service.GetInteractiveChecker() :?> Microsoft.FSharp.Compiler.SourceCodeServices.InteractiveChecker 
            let flags = 
                [|
                    match (source :> IdealSource).ProjectSite with
                    | Some pi -> 
                        yield! pi.CompilerFlags () |> Array.filter(fun flag -> flag.StartsWith("--define:"))
                    | None -> ()
                    yield "--noframework"

                |]
            // get a sync parse of the file
            let co = 
                { ProjectFileName = fileName + ".dummy.fsproj"
                  ProjectFileNames = [| fileName |]
                  ProjectOptions = flags
                  IsIncompleteTypeCheckEnvironment = true
                  UseScriptResolutionRules = false
                  LoadTime = new System.DateTime(2000,1,1)   // dummy data, just enough to get a parse
                  UnresolvedReferences = None }

            ic.UntypedParse(fileName, source.GetText(), co)

        member source.IdealSource =
            match idealSource with
            | Some(x) -> x
            | None -> 
                let recolorizeWholeFile() = 
                    if source.ColorState<>null && textLines<>null then      // textlines is used by GetLineCount()
                            source.ColorState.ReColorizeLines(0, source.GetLineCount() - 1) |> ignore

                let recolorizeLine(line:int) = 
                    if source.ColorState<>null && textLines<>null && line >= 0 && line < source.GetLineCount() then      // textlines is used by GetLineCount()
                            source.ColorState.ReColorizeLines(line, line) |> ignore

                let isClosed() = source.IsClosed
                let currentFileName() = VsTextLines.GetFilename textLines
                idealSource <- Some(new DelegatingSource(recolorizeWholeFile,recolorizeLine,currentFileName,isClosed,filechange) :> IdealSource)
                source.IdealSource
        
        override source.GetCommentFormat() = 
            let mutable info = new CommentInfo()
            info.BlockEnd<-"(*"
            info.BlockStart<-"*)"
            info.UseLineComments<-true
            info.LineStart <- "//"
            info
            
        override source.GetTaskProvider() =
            match source.IdealSource.ProjectSite with
            | Some ps ->
                match ps.ErrorListTaskProvider() with
                | Some etp -> etp
                | _ -> base.GetTaskProvider()
            | _ -> base.GetTaskProvider()
            
        override source.GetTaskReporter() =
            match source.IdealSource.ProjectSite with
            | Some(ps) ->
                match ps.ErrorListTaskReporter() with
                | Some(etr) -> etr
                | _ -> base.GetTaskReporter()
            | _ -> base.GetTaskReporter()       

        member val FSharpIntelliSenseToAppearAdornment : FSharpIntelliSenseToAppearAdornment option = None with get, set
        member val CancellationTokenSource : CancellationTokenSource = null with get, set

        member source.ResetFSharpIntelliSenseToAppearAdornment() =
            UIThread.MustBeCalledFromUIThread()
            if source.CancellationTokenSource <> null && not source.CancellationTokenSource.IsCancellationRequested then
                source.CancellationTokenSource.Cancel()
            match source.FSharpIntelliSenseToAppearAdornment with
            | None -> ()
            | Some a -> 
                a.RemoveSelf()
                source.FSharpIntelliSenseToAppearAdornment <- None

        member source.AttachFSharpIntelliSenseToAppearAdornment(wpfTextView, cursorPoint, completionWasExplicitlyRequested) =
            UIThread.MustBeCalledFromUIThread()
            source.ResetFSharpIntelliSenseToAppearAdornment() 
            let cts = new CancellationTokenSource()
            source.CancellationTokenSource <- cts
            let timeUntilPopup =
                if not completionWasExplicitlyRequested then 
                    2000 // we could always tweak this number, intent is long enough to never show up with everyday 'fast' IntelliSense on '.', but short enough so you discover that waiting may be useful
                else 
                    500 // they explicitly pressed Ctrl-J or Ctrl-space, so we should apply the 'loading...' adornment more aggressively
            let cancelableTask =
                async {
                    do! Async.Sleep(timeUntilPopup)  
                    UIThread.MustBeCalledFromUIThread()
                    if source.FSharpIntelliSenseToAppearAdornment.IsNone then
                        source.FSharpIntelliSenseToAppearAdornment <- Some <| new FSharpIntelliSenseToAppearAdornment(wpfTextView, cursorPoint, service.Site)
                        // reset if they move cursor, (ideally would only reset if moved out of 'applicative span', but for now, any movement cancels)
                        let rec caretSubscription : IDisposable = wpfTextView.Caret.PositionChanged.Subscribe(fun _ea -> source.ResetFSharpIntelliSenseToAppearAdornment(); caretSubscription.Dispose())
                        // if the user types new chars right before the caret, the editor does not fire a Caret.PositionChanged event, but LayoutChanged will fire, so also hook that
                        let rec layoutSubscription : IDisposable = wpfTextView.LayoutChanged.Subscribe(fun _ea -> source.ResetFSharpIntelliSenseToAppearAdornment(); layoutSubscription.Dispose())
                        ()
                    else
                        Debug.Assert(false, "why was FSharpIntelliSenseToAppearAdornment already set?")
                }
            Async.StartImmediate(Async.TryCancelled(cancelableTask, ignore), source.CancellationTokenSource.Token)

        override source.HandleLostFocus() =
            source.ResetFSharpIntelliSenseToAppearAdornment()

        // entrypoint for autocompletion
        override source.Completion(textView, info, reason, requireFreshResults) =
            let line = ref 0
            let idx = ref 0
            NativeMethods.ThrowOnFailure(textView.GetCaretPos(line, idx)) |> ignore
            if requireFreshResults <> RequireFreshResults.Yes then
                // if it was Yes, then we are in second-chance intellisense and we already started the task for the first-chance
                let wpfTextView = SourceImpl.GetWpfTextViewFromVsTextView(textView)
                let ss = wpfTextView.TextSnapshot
                let tsLine = ss.GetLineFromLineNumber(!line)
                let lineLen = tsLine.End.Position - tsLine.Start.Position 
                let toAdd = min lineLen !idx   // if we are in virtual whitespace after EOF, is illegal to get a snapshot point there
                let cursorPoint = ss.GetLineFromLineNumber(!line).Start.Add(toAdd)
                let completionWasExplicitlyRequested = 
                    (reason=BackgroundRequestReason.DisplayMemberList) // Ctrl-J
                    || (reason=BackgroundRequestReason.CompleteWord) // Ctrl-space
                    // Note: BackgroundRequestReason.MemberSelect is when they pressed '.' and it auto-popped up
                source.AttachFSharpIntelliSenseToAppearAdornment(wpfTextView, cursorPoint, completionWasExplicitlyRequested)
            let reason =
                if reason = BackgroundRequestReason.CompleteWord then
                    let upi = source.GetParseTree()
                    let isBetweenDotAndIdent =
                        match Microsoft.FSharp.Compiler.SourceCodeServices.UntypedParseInfoImpl.TryFindExpressionASTLeftOfDotLeftOfCursor(!line, !idx, upi.ParseTree) with
                        | Some(_,isBetweenDotAndIdent) -> isBetweenDotAndIdent
                        | None -> false
                    if isBetweenDotAndIdent then
                        BackgroundRequestReason.DisplayMemberList  // be like C#; convert a Ctrl-space to a Ctrl-J if the cursor is here:   class.$bar()
                    else
                        reason
                else 
                    reason
            source.BeginBackgroundRequest(
                !line, !idx, info, reason, textView, requireFreshResults, 
                new BackgroundRequestResultHandler(source.HandleCompletionResponse)) |> ignore

        member source.HandleCompletionResponse(req) =
            match req with
            | null -> source.ResetFSharpIntelliSenseToAppearAdornment()
            | _ when req.View = null || req.ResultScope = null -> source.ResetFSharpIntelliSenseToAppearAdornment()
            | _ when (req.Timestamp <> source.ChangeCount) -> source.ResetFSharpIntelliSenseToAppearAdornment()
            | _ ->
                  source.HandleResponseHelper(req)
                  let reason = req.Reason
                  if reason = BackgroundRequestReason.MemberSelectAndHighlightBraces then
                      source.HandleMatchBracesResponse(req)
                  async {
                      let! decls = req.ResultScope.GetDeclarations(req.Snapshot, req.Line, req.Col, reason)
                      do! Async.SwitchToContext UIThread.TheSynchronizationContext
                      if (decls = null || decls.IsEmpty()) && req.Timestamp <> req.ResultTimestamp then
                          // Second chance intellisense: we didn't get any result and the basis typecheck was stale. We need to retrigger the completion.
                          source.Completion(req.View, req.TokenInfo, req.Reason, RequireFreshResults.Yes)
                      else
                          match decls with
                          | null -> ()
                          | _ ->
                              let completeWord = (reason = BackgroundRequestReason.CompleteWord)
                              let line,idx =
                                  let mutable line = 0
                                  let mutable idx = 0
                                  req.View.GetCaretPos(&line, &idx) |> NativeMethods.ThrowOnFailure |> ignore
                                  (line,idx)
                              if decls.GetCount("") > 0 && 
                                  (source.Service.Preferences.AutoListMembers || completeWord || reason = BackgroundRequestReason.DisplayMemberList) &&
                                  line = req.Line && idx = req.Col // ensure user has not chaged cursor location (note: ideally, we would allow typing if still in 'applicative span' for completion)
                                  then
                                  source.CompletionSet.Init(req.View, decls, completeWord)
                          source.ResetFSharpIntelliSenseToAppearAdornment()
                  } |> Async.StartImmediate

        member source.PreFixupSpan(origSpan : TextSpan) =            
            let mutable span = new TextSpan(iEndIndex = origSpan.iEndIndex, iEndLine = origSpan.iEndLine, iStartIndex = origSpan.iStartIndex, iStartLine = origSpan.iStartLine)
            // if at start of next line, treat like end of previous line
            if span.iEndIndex = 0 && not(span.iEndLine = span.iStartLine) then
                span.iEndLine <- span.iEndLine - 1
            span
            
        member source.PostFixupSpan(origSpan : TextSpan) =            
            let mutable span = new TextSpan(iEndIndex = origSpan.iEndIndex, iEndLine = origSpan.iEndLine, iStartIndex = origSpan.iStartIndex, iStartLine = origSpan.iStartLine)
            // move highlight to start & end of line
            span.iStartIndex <- 0
            span.iEndIndex <- source.GetLine(span.iEndLine).Length
            span
        
        override source.CommentLines(origSpan, lineComment) =
            let adapter = source.getEditorAdapter()
            use edit = adapter.GetDataBuffer(textLines).CreateEdit(EditOptions.DefaultMinimalChange, Unchecked.defaultof<Nullable<int>>, None)
            let mutable span = source.PreFixupSpan(origSpan)
            for i = span.iStartLine to span.iEndLine do
                let line = edit.Snapshot.GetLineFromLineNumber(i)
                source.SetText(line, 0, 0, lineComment, edit)
            edit.Apply() |> ignore

            lastCommentSpan <- source.PostFixupSpan(span)
            lastCommentSpan

        override source.UncommentLines(origSpan, lineComment) =
            let adapter = source.getEditorAdapter()
            use edit = adapter.GetDataBuffer(textLines).CreateEdit(EditOptions.DefaultMinimalChange, Unchecked.defaultof<Nullable<int>>, None)
            let mutable span = source.PreFixupSpan(origSpan)
            let len = lineComment.Length
            for i = span.iStartLine to span.iEndLine do
                let startIndex = source.ScanToNonWhitespaceChar(i)
                let curLine = source.GetLine(i)
                if curLine.Length >= startIndex + len && curLine.Substring(startIndex, len) = lineComment then
                    let line = edit.Snapshot.GetLineFromLineNumber(i)
                    source.SetText(line, startIndex, len, "", edit)
            edit.Apply() |> ignore
            source.PostFixupSpan(span)

        override source.CommentSpan(span) =
            base.CommentSpan(span) |> ignore
            lastCommentSpan
            
        override source.RecordChangeToView() = source.IdealSource.RecordChangeToView()
        override source.RecordViewRefreshed() = source.IdealSource.RecordViewRefreshed()
        override source.NeedsVisualRefresh 
            with get() = source.IdealSource.NeedsVisualRefresh
            
        override source.ChangeCount
            with get() = source.IdealSource.ChangeCount
            and set(value) = source.IdealSource.ChangeCount <- value                
            
        override source.DirtyTime
            with get() = source.IdealSource.DirtyTime
            and set(value) = source.IdealSource.DirtyTime <- value                
                            
        override source.Dispose() =
            try 
                base.Dispose()       
            finally
                match idealSource with
                | None -> ()
                | Some(is) -> 
                    ((box is):?>IDisposable).Dispose()       
                    filechange<-null
                    idealSource<-None
                    textLines<-null

        override source.OnUserDataChange(riidKey, _vtNewValue) =
            let newfileName = VsTextLines.GetFilename textLines
            if not (String.Equals(fileName, newfileName, StringComparison.InvariantCultureIgnoreCase)) then
                // the filename of the text buffer is changing, could be changing e.g. .fsx to .fs or vice versa
                fileName <- newfileName
                (source :> IdealSource).RecolorizeWholeFile()

#if DEBUG        
        override source.OriginalFilename = originalFilename
#endif

        // Just forward to IdealSource  
        interface IdealSource with
            member source.RecolorizeWholeFile() = source.IdealSource.RecolorizeWholeFile() 
            member source.RecolorizeLine line = source.IdealSource.RecolorizeLine line
            member source.RecordChangeToView() = source.IdealSource.RecordChangeToView()
            member source.RecordViewRefreshed() = source.IdealSource.RecordViewRefreshed()
            member source.NeedsVisualRefresh 
                with get() = source.IdealSource.NeedsVisualRefresh
            member source.IsClosed 
                with get() = source.IdealSource.IsClosed
            member source.ProjectSite
                with get() = source.IdealSource.ProjectSite
                and set(value) = source.IdealSource.ProjectSite <- value
            member source.ChangeCount
                with get() = source.IdealSource.ChangeCount
                and set(value) = source.IdealSource.ChangeCount <- value                
            member source.DirtyTime
                with get() = source.IdealSource.DirtyTime
                and set(value) = source.IdealSource.DirtyTime <- value                
            member source.SetDependencyFiles(files) = 
                source.IdealSource.SetDependencyFiles(files)
            member source.SetDependencyFileChangeCallback(projectCallback,fileCallback) = 
                source.IdealSource.SetDependencyFileChangeCallback(projectCallback,fileCallback)
                
        // Hook file change events                
        interface IVsFileChangeEvents with 
            member changes.FilesChanged(_count : uint32, _files: string [], _changeFlags : uint32 []) = 0
            member changes.DirectoryChanged(_directory: string) = 0


                
    let internal CreateSource(service:LanguageService,
                              textLines:IVsTextLines,
                              colorizer:Colorizer,
                              filechange:IVsFileChangeEx) : IdealSource =
        new SourceOverIdealSource(service,textLines,colorizer,filechange) :> IdealSource    
                
