// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.LanguageService

open System
open System.IO
open System.Collections.Generic
open System.Configuration
open System.Globalization
open System.Runtime.InteropServices
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop 
open Microsoft.VisualStudio.TextManager.Interop 
open Microsoft.VisualStudio.Text
open Microsoft.VisualStudio.OLE.Interop
open Microsoft.FSharp.Compiler
open Microsoft.FSharp.Compiler.SourceCodeServices

// This is the list of known owners of callbacks that may register themselves.
// Consumers not in this list should use a GUID our some other strongly unique key string
module internal KnownAdviseProjectSiteChangesCallbackOwners = 
    let LanguageService = "F# Language Service"

module internal FSharpCommonConstants =
    [<Literal>]
    let languageServiceGuidString = "BC6DD5A5-D4D6-4dab-A00D-A51242DBAF1B"


module internal FSharpConstants = 
    [<Literal>]
    let fsharpLanguageName = "F#"

    // These are the IDs from fslangservice.dll
    [<Literal>]
    let packageGuidString               = "871D2A70-12A2-4e42-9440-425DD92A4116"
    [<Literal>]
    let languageServiceGuidString       = FSharpCommonConstants.languageServiceGuidString

    // These are the IDs from the Python sample:
    let intellisenseProviderGuidString  = "8b1807ea-d222-4765-afa8-c092d480e451"
        
    // These are the entries from fslangservice.dll
    let PLKMinEdition                   = "standard"
    let PLKCompanyName                  = "Microsoft" // "Microsoft Corporation"
    let PLKProductName                  = "f#" // "Visual Studio Integration of FSharp Language Service"
    let PLKProductVersion               = "1.0"
    let PLKResourceID                   = 1s        
    let enableLanguageService = "fsharp-language-service-enabled"


                
/// This class defines capabilities of the language service. 
/// CodeSense = true\false, for example
type internal FSharpLanguagePreferences(site, langSvc, name) = 
    inherit LanguagePreferences(site, langSvc, name) 

// Container class that delays loading of FSharp.Compiler.dll compiler types until they're actually needed
type internal FSharpCheckerContainer(checker) =
    member this.FSharpChecker = checker

/// LanguageService state. 
type internal FSharpLanguageServiceTestable() as this =
    static let colorizerGuid = new Guid("{A2976312-7D71-4BB4-A5F8-66A08EBF46C8}") // Guid for colorizwed user data on IVsTextBuffer
    let mutable checkerContainerOpt : FSharpCheckerContainer option = None
    let mutable artifacts : ProjectSitesAndFiles option = None
    let mutable serviceProvider : System.IServiceProvider option = None
    let mutable preferences : LanguagePreferences option = None
    let mutable documentationBuilder : IDocumentationBuilder option = None
    let mutable sourceFactory : (IVsTextLines -> IFSharpSource) option = None
    let mutable dirtyForTypeCheckFiles : Set<string> = Set.empty
    let mutable isInitialized = false
    let mutable unhooked = false
    let getColorizer (view:IVsTextView) = 
        let buffer = Com.ThrowOnFailure1(view.GetBuffer())
        this.GetColorizer(buffer)                         

    let bgRequests = new FSharpLanguageServiceBackgroundRequests(getColorizer,(fun () -> this.FSharpChecker),(fun () -> this.ProjectSitesAndFiles),(fun () -> this.ServiceProvider),(fun () -> this.DocumentationBuilder))
    
    member this.FSharpChecker = 
        if this.Unhooked then raise Error.UseOfUnhookedLanguageServiceState
        if not this.IsInitialized then raise Error.UseOfUninitializedLanguageServiceState
        checkerContainerOpt.Value.FSharpChecker

    member this.ServiceProvider = 
        if this.Unhooked then raise Error.UseOfUnhookedLanguageServiceState
        if not this.IsInitialized then raise Error.UseOfUninitializedLanguageServiceState
        serviceProvider.Value 

    member this.ProjectSitesAndFiles = 
        if not this.IsInitialized then raise Error.UseOfUninitializedLanguageServiceState
        artifacts.Value
        
    member this.Preferences = 
        if this.Unhooked then raise Error.UseOfUnhookedLanguageServiceState
        if not this.IsInitialized then raise Error.UseOfUninitializedLanguageServiceState
        preferences.Value
        
    member this.SourceFactory = 
        if this.Unhooked then raise Error.UseOfUnhookedLanguageServiceState
        if not this.IsInitialized then raise Error.UseOfUninitializedLanguageServiceState
        sourceFactory.Value
        
    member this.IsInitialized = isInitialized
    member this.Unhooked = unhooked
    member this.DocumentationBuilder = documentationBuilder.Value
    
    /// Handle late intialization pieces
    member this.Initialize (sp, dp, prefs, sourceFact) = 
        if this.Unhooked then raise Error.UseOfUnhookedLanguageServiceState        
        artifacts <- Some (ProjectSitesAndFiles())
        let checker = FSharpChecker.Create()
        checker.BeforeBackgroundFileCheck.Add (fun filename -> UIThread.Run(fun () -> this.NotifyFileTypeCheckStateIsDirty(filename)))
        checkerContainerOpt <- Some (FSharpCheckerContainer checker)
        serviceProvider <- Some sp
        isInitialized <- true
        unhooked <- false
        documentationBuilder <- Some dp
        preferences <- Some prefs
        sourceFactory <- Some sourceFact

    
    member this.NotifyFileTypeCheckStateIsDirty(filename) = 
        dirtyForTypeCheckFiles <- dirtyForTypeCheckFiles.Add filename

    /// Clear all language service caches and finalize all transient references to compiler objects
    member this.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients() = 
        if this.Unhooked then raise Error.UseOfUnhookedLanguageServiceState
        if this.IsInitialized then
            this.FSharpChecker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()

    /// Unhook the object. These are the held resources that need to be disposed.
    member this.Unhook() =
        if this.Unhooked then raise Error.UseOfUnhookedLanguageServiceState
        if this.IsInitialized then
            // Dispose the preferences.
            if this.Preferences <> null then this.Preferences.Dispose()
            // Stop the background compile.
            // here we refer to checkerContainerOpt directly to avoid triggering its creation
            match checkerContainerOpt with
            | Some container -> 
                let checker = container.FSharpChecker
                checker.StopBackgroundCompile()
                checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
            | None -> ()
            
            checkerContainerOpt <- None 
            artifacts <- None
            preferences <- None
            documentationBuilder <- None
            unhooked <- true
            sourceFactory <- None
            serviceProvider <- None
    
    /// Respond to project settings changes
    member this.OnProjectSettingsChanged(site:IProjectSite) = 
        // The project may have changed its references.  These would be represented as 'dependency files' of each source file.  Each source file will eventually start listening
        // for changes to those dependencies, at which point we'll get OnDependencyFileCreateOrDelete notifications.  Until then, though, we just 'make a note' that this project is out of date.
        bgRequests.AddOutOfDateProjectFileName(site.ProjectFileName()) 
        for filename in site.SourceFilesOnDisk() do
            let rdt = this.ServiceProvider.RunningDocumentTable
            match this.ProjectSitesAndFiles.TryGetSourceOfFile(rdt,filename) with
            | Some source -> 
                source.RecolorizeWholeFile()
                source.RecordChangeToView()
            | None -> ()

    /// Respond to project being cleaned/rebuilt (any live type providers in the project should be refreshed)
    member this.OnProjectCleaned(projectSite:IProjectSite) = 
        let checkOptions = ProjectSitesAndFiles.GetProjectOptionsForProjectSite(projectSite, "")
        this.FSharpChecker.NotifyProjectCleaned(checkOptions)

    member this.OnActiveViewChanged(textView) =
        bgRequests.OnActiveViewChanged(textView)

    member this.BackgroundRequests = bgRequests
    
    /// Unittestable complement to LanguageServce.CreateSource
    member this.CreateSource(buffer:IVsTextLines) : IFSharpSource =
    
        // Each time a source is created, also verify that the IProjectSite has been initialized to listen to changes to the project.
        // We can't listen to OnProjectLoaded because the language service is not guaranteed to be loaded when this is called.
        let filename = VsTextLines.GetFilename buffer
        let rdt = this.ServiceProvider.RunningDocumentTable
        let result = VsRunningDocumentTable.FindDocumentWithoutLocking(rdt,filename)
        match result with 
        | Some(hier,_) -> 
            match hier with 
            | :? IProvideProjectSite as siteProvider ->
                let site = siteProvider.GetProjectSite()
                site.AdviseProjectSiteChanges(KnownAdviseProjectSiteChangesCallbackOwners.LanguageService, 
                                              new AdviseProjectSiteChanges(fun () -> this.OnProjectSettingsChanged(site))) 
                site.AdviseProjectSiteCleaned(KnownAdviseProjectSiteChangesCallbackOwners.LanguageService, 
                                              new AdviseProjectSiteChanges(fun () -> this.OnProjectCleaned(site))) 
            | _ -> 
                // This can happen when the file is in a solution folder or in, say, a C# project.
                ()
        | _ ->
            // This can happen when renaming a file from a different language service into .fs or fsx.
            // This naturally won't have an associated project.
            ()
        
        // Create the source and register file change callbacks there.       
        let source = this.SourceFactory(buffer)
        this.ProjectSitesAndFiles.SetSource(buffer, source)
        source
    
    // For each change in dependency files, notify the language service of the change and propagate the update
    interface IDependencyFileChangeNotify with
        member this.DependencyFileCreated projectSite = 
            // Invalidate the configuration if we notice any add for any DependencyFiles 
            let checkOptions = ProjectSitesAndFiles.GetProjectOptionsForProjectSite(projectSite, "")
            this.FSharpChecker.InvalidateConfiguration(checkOptions)

        member this.DependencyFileChanged (filename) = 
            this.NotifyFileTypeCheckStateIsDirty filename


    /// Do OnIdle processing for the whole language service. dirtyForTypeCheckFiles can be set by events 
    /// raised on the background compilation thread.  
    member this.OnIdle() = 
        for file in dirtyForTypeCheckFiles do
            let rdt = this.ServiceProvider.RunningDocumentTable
            match this.ProjectSitesAndFiles.TryGetSourceOfFile(rdt, file) with
            | Some source -> source.RecordChangeToView()
            | None ->  ()
        dirtyForTypeCheckFiles <- Set.empty
        

    /// Remove a colorizer.
    member this.CloseColorizer(colorizer:FSharpColorizer) = 
        let buffer = colorizer.Buffer
        let mutable guid = colorizerGuid
        (buffer :?> IVsUserData).SetData(&guid, null) |> ErrorHandler.ThrowOnFailure |> ignore

    /// Get a colorizer for a particular buffer.
    member this.GetColorizer(buffer:IVsTextLines) : FSharpColorizer = 
        let mutable guid = colorizerGuid
        let mutable colorizerObj = null
        
        (buffer :?> IVsUserData).GetData(&guid, &colorizerObj) |> ignore
        match colorizerObj with
        | null ->
            let scanner = 
                new FSharpScanner(fun source ->
                    // Note: in theory, the next few lines do not need to be recomputed every line.  Instead we could just cache the tokenizer
                    // and only update it when e.g. the project system notifies us there is an important change (e.g. a file rename, etc).
                    // In practice we have been there, and always screwed up some non-unit-tested/testable corner-cases.
                    // So this is not ideal from a perf perspective, but it is easy to reason about the correctness.
                    let filename = VsTextLines.GetFilename buffer
                    let rdt = this.ServiceProvider.RunningDocumentTable
                    let defines = this.ProjectSitesAndFiles.GetDefinesForFile(rdt, filename)
                    let sourceTokenizer = FSharpSourceTokenizer(defines,filename)
                    sourceTokenizer.CreateLineTokenizer(source))

            let colorizer = new FSharpColorizer(this.CloseColorizer, buffer, scanner) 
            (buffer :?> IVsUserData).SetData(&guid, colorizer) |> ErrorHandler.ThrowOnFailure |> ignore
            colorizer
        | _ -> colorizerObj :?> FSharpColorizer
    
    /// Block until the background compile finishes.
    //
    // This is for unit testing only
    member this.WaitForBackgroundCompile() =
        this.FSharpChecker.WaitForBackgroundCompile()            

module internal VsConstants =
    let guidStdEditor = new Guid("9ADF33D0-8AAD-11D0-B606-00A0C922E851")
    let guidCodeCloneProvider = new Guid("38fd587e-d4b7-4030-9a95-806ff0d5c2c6")

    let cmdidGotoDecl = 936u // "Go To Declaration"
    let cmdidGotoRef = 1107u // "Go To Reference"
    
    let IDM_VS_EDITOR_CSCD_OUTLINING_MENU = 773u // "Outlining"
    let ECMD_OUTLN_HIDE_SELECTION = 128u // "Hide Selection" - 
    let ECMD_OUTLN_TOGGLE_CURRENT = 129u // "Toggle Outlining Expansion" - 
    let ECMD_OUTLN_TOGGLE_ALL = 130u // "Toggle All Outlining"
    let ECMD_OUTLN_STOP_HIDING_ALL = 131u // "Stop Outlining"
    let ECMD_OUTLN_STOP_HIDING_CURRENT = 132u // "Stop Hiding Current"

type private QueryStatusResult =
    | NOTSUPPORTED = 0
    | SUPPORTED = 1
    | ENABLED = 2
    | LATCHED = 4
    | NINCHED = 8
    | INVISIBLE = 16

type internal FSharpViewFilter(mgr:CodeWindowManager,view:IVsTextView) =
    inherit ViewFilter(mgr,view)

    override this.Dispose() = base.Dispose()

    member this.IsSupportedCommand(guidCmdGroup:byref<Guid>,cmd:uint32) =
        if guidCmdGroup = VsMenus.guidStandardCommandSet97 && (cmd = VsConstants.cmdidGotoDecl || cmd = VsConstants.cmdidGotoRef) then false
        elif guidCmdGroup = VsConstants.guidCodeCloneProvider then false // disable commands for CodeClone package
        else
            // These are all the menu options in the "Outlining" cascading menu. We need to disable all the individual
            // items to disable the cascading menu. QueryCommandStatus does not get called for the cascading menu itself.
            assert((guidCmdGroup = VsConstants.guidStdEditor && cmd = VsConstants.IDM_VS_EDITOR_CSCD_OUTLINING_MENU) = false)
            if guidCmdGroup = VsMenus.guidStandardCommandSet2K && (cmd = VsConstants.ECMD_OUTLN_HIDE_SELECTION ||
                                                                   cmd = VsConstants.ECMD_OUTLN_TOGGLE_CURRENT ||
                                                                   cmd = VsConstants.ECMD_OUTLN_TOGGLE_ALL ||
                                                                   cmd = VsConstants.ECMD_OUTLN_STOP_HIDING_ALL ||
                                                                   cmd = VsConstants.ECMD_OUTLN_STOP_HIDING_CURRENT) then false
            else true

    override this.QueryCommandStatus(guidCmdGroup:byref<Guid>,cmd:uint32) =        
        if this.IsSupportedCommand(&guidCmdGroup,cmd) then
            base.QueryCommandStatus(&guidCmdGroup,cmd)
        else
            // Hide the menu item. Just returning QueryStatusResult.NOTSUPPORTED does not work
            QueryStatusResult.INVISIBLE ||| QueryStatusResult.SUPPORTED |> int

        
[<Guid(FSharpConstants.languageServiceGuidString)>]

[<ProvideLanguageExtension(typeof<FSharpLanguageService>, ".fs")>]
[<ProvideLanguageExtension(typeof<FSharpLanguageService>, ".fsi")>]
[<ProvideLanguageExtension(typeof<FSharpLanguageService>, ".fsx")>]
[<ProvideLanguageExtension(typeof<FSharpLanguageService>, ".fsscript")>]
[<ProvideLanguageExtension(typeof<FSharpLanguageService>, ".ml")>]
[<ProvideLanguageExtension(typeof<FSharpLanguageService>, ".mli")>]

[<ProvideEditorExtension("4EB7CCB7-4336-4FFD-B12B-396E9FD079A9", ".fs", 97)>]
[<ProvideEditorExtension("4EB7CCB7-4336-4FFD-B12B-396E9FD079A9", ".fsi", 97)>]
[<ProvideEditorExtension("4EB7CCB7-4336-4FFD-B12B-396E9FD079A9", ".fsx", 97)>]
[<ProvideEditorExtension("4EB7CCB7-4336-4FFD-B12B-396E9FD079A9", ".fsscript", 97)>]
[<ProvideEditorExtension("4EB7CCB7-4336-4FFD-B12B-396E9FD079A9", ".ml", 97)>]
[<ProvideEditorExtension("4EB7CCB7-4336-4FFD-B12B-396E9FD079A9", ".mli", 97)>]

type internal FSharpLanguageService() as fls = 
    inherit LanguageService() 
    
    let ls = FSharpLanguageServiceTestable()
    let mutable rdtCookie = VSConstants.VSCOOKIE_NIL
    
    let thisAssembly = typeof<FSharpLanguageService>.Assembly 
    let resources = lazy (new System.Resources.ResourceManager("VSPackage", thisAssembly))
    let GetString(name:string) = resources.Force().GetString(name, CultureInfo.CurrentUICulture)

    let navigation = FSharpNavigationController()

    let formatFilterList = lazy(
                                    let fsFile = GetString("FSharpFile")
                                    let fsInterfaceFile = GetString("FSharpInterfaceFile")
                                    let fsxFile = GetString("FSXFile")
                                    let fsScriptFile = GetString("FSharpScriptFile")
                                    let result = sprintf "%s|*.fs\n%s|*.fsi\n%s|*.fsx\n%s|*.fsscript"
                                                             fsFile fsInterfaceFile fsxFile fsScriptFile
                                    result)
    
    // This array contains the definition of the colorable items provided by this
    // language service.
    let colorableItems = [|
            // See e.g. the TokenColor type defined in Scanner.cs.  Position 0 is implicit and always means "Plain Text".
            // The next 5 items in this list MUST be these default items in this order:
            new FSharpColorableItem("Keyword",              lazy (GetString("Keyword")),             COLORINDEX.CI_BLUE,         COLORINDEX.CI_USERTEXT_BK)
            new FSharpColorableItem("Comment",              lazy (GetString("Comment")),             COLORINDEX.CI_DARKGREEN,    COLORINDEX.CI_USERTEXT_BK)
            new FSharpColorableItem("Identifier",           lazy (GetString("Identifier")),          COLORINDEX.CI_USERTEXT_FG,  COLORINDEX.CI_USERTEXT_BK)
            new FSharpColorableItem("String",               lazy (GetString("String")),              COLORINDEX.CI_MAROON,       COLORINDEX.CI_USERTEXT_BK)
            new FSharpColorableItem("Number",               lazy (GetString("Number")),              COLORINDEX.CI_USERTEXT_FG,  COLORINDEX.CI_USERTEXT_BK)
            // User-defined color classes go here:
            new FSharpColorableItem("Excluded Code",        lazy (GetString("ExcludedCode")),         COLORINDEX.CI_DARKGRAY,     COLORINDEX.CI_USERTEXT_BK)
            new FSharpColorableItem("Preprocessor Keyword", lazy (GetString("PreprocessorKeyword")),  COLORINDEX.CI_BLUE,         COLORINDEX.CI_USERTEXT_BK)
            new FSharpColorableItem("Operator",             lazy (GetString("Operator")),             COLORINDEX.CI_USERTEXT_FG,  COLORINDEX.CI_USERTEXT_BK)
        |]

    let sourceFactory(buffer:IVsTextLines) = 
        let vsFileWatch = fls.GetService(typeof<SVsFileChangeEx >) :?> IVsFileChangeEx
        Source.CreateSource(fls, buffer, fls.GetColorizer(buffer), vsFileWatch, ls :> IDependencyFileChangeNotify, (fun () -> ls.FSharpChecker))

    let refreshUI() =
        let uiShell = fls.Site.GetService<SVsUIShell,IVsUIShell>()
        if (uiShell <> null) then
            uiShell.UpdateCommandUI(0) |> ignore
            

    /// Initialize the language service
    override fls.Initialize() =
        if not ls.IsInitialized then 
            let preferences = new FSharpLanguagePreferences(fls.Site, (typeof<FSharpLanguageService>).GUID, FSharpConstants.fsharpLanguageName)
            preferences.Init() // Reads preferences from the VS registry.
            preferences.MaxErrorMessages <- 1000
            
            let serviceProvider = fls.Site 
            let docBuilder = XmlDocumentation.CreateDocumentationBuilder(serviceProvider.XmlService, serviceProvider.DTE)
            
            ls.Initialize(serviceProvider, docBuilder, preferences, sourceFactory)

            rdtCookie <- (Com.ThrowOnFailure1 (serviceProvider.RunningDocumentTable.AdviseRunningDocTableEvents (fls:>IVsRunningDocTableEvents)))

                          
    override fls.Dispose() =
        try
            try
                if rdtCookie <> VSConstants.VSCOOKIE_NIL then 
                    let rdt = fls.Site.RunningDocumentTable
                    Com.ThrowOnFailure0 (rdt.UnadviseRunningDocTableEvents rdtCookie)
            finally
                if ls.IsInitialized then 
                    ls.Unhook()
        finally 
            base.Dispose()

    member fls.Core = ls

    // -----------------------------------------------------------------------
    // Implement IVsLanguageInfo 

    interface IVsLanguageInfo with
        /// Allows a language to add adornments to a code editor.
        member this.GetCodeWindowManager(codeWindow, mgr) =
            fls.Initialize();
            let mutable buffer = null;
            NativeMethods.ThrowOnFailure(codeWindow.GetBuffer(&buffer)) |> ignore;
            // see if we already have a Source object.
            let source = 
                match this.GetSource(buffer) with 
                | null -> 
                    let s = ls.CreateSource(buffer) :?> ISource 
                    this.sources.Add(s) |> ignore;  
                    s
                | s -> s

            mgr <- this.CreateCodeWindowManager(codeWindow, source);
            NativeMethods.S_OK;

        /// Returns the colorizer.
        member this.GetColorizer(buffer, result) =
            result <- this.GetColorizer(buffer)
            NativeMethods.S_OK

        /// Returns the file extensions belonging to this language.
        member this.GetFileExtensions(extensions) =
            extensions <- ""
            NativeMethods.S_OK

        member this.GetLanguageName(name) =
            name <- FSharpConstants.fsharpLanguageName
            NativeMethods.S_OK

    // -----------------------------------------------------------------------
    // Implement LanguageService base methods from the C# code

    override fls.OnActiveViewChanged(textView) =
        base.OnActiveViewChanged(textView)
        ls.OnActiveViewChanged(textView)
        if navigation.EnableNavBar || navigation.EnableRegions then
           match fls.GetSource(textView) with
           | null -> ()
           | source -> ls.BackgroundRequests.TriggerParseFile(textView, source) |> ignore


    // Provides the index to the filter matching the extension of the file passed in.
    override fls.CurFileExtensionFormat(filename) = 
        // These indexes match the "GetFormatFilterList" function
        match Path.GetExtension(filename) with
        | ".fs" -> 0
        | ".ml" -> 1
        | ".fsi" -> 2
        | ".mli" -> 3
        | ".fsx" -> 4
        | ".fsscript" -> 5
        | _ -> -1

    /// Provides the list of available extensions for Save As.
    /// The following default filter string is automatically added by Visual Studio:
    /// "All Files (*.*)\n*.*\nText Files (*.txt)\n*.txt\n"
    override fls.GetFormatFilterList() = 
        formatFilterList.Value 

    // This seems to be called by codeWinMan.OnNewView(textView) to install a ViewFilter on the TextView.    
    override this.CreateViewFilter(mgr:CodeWindowManager,newView:IVsTextView) = new FSharpViewFilter(mgr,newView) :> ViewFilter

    override fls.GetLanguagePreferences() = ls.Preferences

    override fls.CreateBackgroundRequest(line, col, info, sourceText, snapshot, methodTipMiscellany, fname, reason, view, sink, source, timestamp, synchronous) =
        ls.BackgroundRequests.CreateBackgroundRequest(line, col, info, sourceText, snapshot, methodTipMiscellany, fname,reason, view,sink, source, timestamp, synchronous) :> BackgroundRequest                                                
        
    /// Handle an incoming request to analyze a file.
    ///
    /// Executed either on the UI thread (for req.IsSynchronous) or the background request thread.
    override fls.ExecuteBackgroundRequest(req:BackgroundRequest) = 
        ls.BackgroundRequests.ExecuteBackgroundRequest(req :?> FSharpBackgroundRequest, req.Source :?> IFSharpSource)

    // Check if we can shortcut executing the background request and just fill in the latest
    // cached scope for the active view from this.service.RecentFullTypeCheckResults.
    //
    // THIS MUST ONLY RETURN TRUE IF ---> ExecuteBackgroundRequest is equivalent to fetching a recent,
    // perhaps out-of-date scope.
    override fls.IsRecentScopeSufficientForBackgroundRequest(reason) = 
        ls.BackgroundRequests.IsRecentScopeSufficientForBackgroundRequest(reason)
      
    // This is called on the UI thread after fresh full typecheck results are available
    override fls.OnParseFileOrCheckFileComplete( req:BackgroundRequest) =
        if (req <> null && req.Source <> null && not req.Source.IsClosed) then 
            fls.SetUserContextDirty(req.FileName)
            refreshUI()
            ls.BackgroundRequests.OnParseFileOrCheckFileComplete(req, navigation.EnableRegions, fls.LastActiveTextView)

    override fls.GetColorizer(buffer) = 
        fls.Initialize()
        ls.GetColorizer(buffer) :> Colorizer

            
    override fls.CreateDropDownHelper(_view) =
        if navigation.EnableNavBar then 
            (new FSharpNavigationBars(fls, fun () -> ls.BackgroundRequests.NavigationBarAndRegionInfo)) :> TypeAndMemberDropdownBars
        else null

    /// Do OnIdle processing    
    override fls.OnIdle(periodic, mgr : IOleComponentManager) =
        try
            let r = base.OnIdle(periodic, mgr)
            ls.OnIdle()
            r
        with e-> 
            Assert.Exception(e)
            reraise()

    // -----------------------------------------------------------------------
    // Implement IVsLanguageDebugInfo 

    /// This is used to return the expression evaluator language to the debugger
    interface IVsLanguageDebugInfo with 
        // Returns the corresponding debugger back-end "language ID".
        member fls.GetLanguageID(_buffer,_line, _col, langId) =
            langId <- DebuggerEnvironment.GetLanguageID()
            VSConstants.S_OK

        // Deprecated. Do not use.
        member fls.GetLocationOfName(_name, pbstrMkDoc, _spans) =
            pbstrMkDoc <- null
            NativeMethods.E_NOTIMPL

        // Generates a name for the given location in the file.
        member fls.GetNameOfLocation(_buffer, _line, _col, name, lineOffset) =
            name <- null;
            lineOffset <- 0;
            NativeMethods.S_OK;

        // Generates proximity expressions, used to populate the "Autos" window
        member fls.GetProximityExpressions(_buffer, _line, _col, _cLines, ppEnum) =
            ppEnum <- null
            NativeMethods.S_FALSE

        // Returns whether the location contains code that is mapped to another document, for example, client-side script code.
        member fls.IsMappedLocation(_buffer, _line, _col) =
            NativeMethods.S_FALSE

        // Disambiguates the given name, providing non-ambiguous names for all entities that "match" the name.
        member fls.ResolveName(_name, _flags, ppNames) =
            ppNames <- null
            NativeMethods.E_NOTIMPL

        // Validates the given position as a place to set a breakpoint.
        member fls.ValidateBreakpointLocation(buffer:IVsTextBuffer, line, col, pCodeSpan:TextSpan[]) =
          let result = 
            if (pCodeSpan <> null) && (pCodeSpan.Length > 0) && (buffer :? IVsTextLines) then
                let syncOk = 
                    let view = fls.LastActiveTextView 
                    view <> null &&
                    let source = fls.GetSource(view) 
                    source <> null &&
                    ls.BackgroundRequests.TrySynchronizeParseFileInformation(view, source, millisecondsTimeout = 100)
                let lineText = VsTextLines.LineText (buffer :?> IVsTextLines) line
                let firstNonWhitespace = lineText.Length - (lineText.TrimStart [| ' '; '\t' |]).Length 
                let lastNonWhitespace = (lineText.TrimEnd [| ' '; '\t' |]).Length 
                // If the column is before the range of text then zap it to the text
                // If the column is after the range of text then zap it to the _start_ of the text (like C#)
                let attempt1, haveScope  = 
                    let col = if col > lastNonWhitespace || col < firstNonWhitespace then firstNonWhitespace else col
                    match ls.BackgroundRequests.ParseFileResults with
                    | Some parseResults -> 
                        match parseResults.ValidateBreakpointLocation(Range.Pos.fromZ line col) with
                        | Some bpl -> Some (TextSpanOfRange bpl), true
                        | None -> None, true
                    | None ->   
                        None, false
                match attempt1 with 
                | Some r -> Some r
                | None -> 
                    if syncOk || haveScope then 
                        None
                    else 
                        // If we didn't sync OK AND we don't even have an ParseFileResults then just accept the whole line.
                        // This is unfortunate but necessary.
                        Some (TextSpan(iStartLine = line, iStartIndex = firstNonWhitespace, iEndLine = line, iEndIndex = lastNonWhitespace))
            else 
                None
                 
          match result with 
          | Some span -> 
            pCodeSpan.[0] <- span
            VSConstants.S_OK
          | None -> 
            VSConstants.S_FALSE                
                        
    // -----------------------------------------------------------------------
    // Implement IVsProvideColorableItems 

    interface IVsProvideColorableItems with

        member x.GetItemCount(count: int byref) =
            count <- colorableItems.Length
            VSConstants.S_OK

        member  x.GetColorableItem(index, item: IVsColorableItem byref) =
            if (index < 1) || (index > colorableItems.Length) then 
                VSConstants.E_INVALIDARG
            else
                item <- colorableItems.[index - 1]
                VSConstants.S_OK

    /// Respond to changes to documents in the Running Document Table.
    interface IVsRunningDocTableEvents with
        member this.OnAfterAttributeChange(_docCookie, _grfAttribs) = VSConstants.S_OK
        member this.OnAfterDocumentWindowHide(_docCookie, _frame) = VSConstants.S_OK
        member this.OnAfterFirstDocumentLock(_docCookie,_dwRDTLockType,_dwReadLocks,_dwEditLocks) = VSConstants.S_OK
        member this.OnAfterSave(_docCookie) = VSConstants.S_OK
        member this.OnBeforeDocumentWindowShow(_docCookie,_isFirstShow,_frame) = VSConstants.S_OK
        member this.OnBeforeLastDocumentUnlock(docCookie,_dwRDTLockType,dwReadLocksRemaining,dwEditLocksRemaining) =
            let rdt = this.Site.RunningDocumentTable
            let (_, _, _, _, file, _, _, unkdoc) = rdt.GetDocumentInfo docCookie // see here http://msdn.microsoft.com/en-us/library/microsoft.visualstudio.shell.interop.ivsrunningdocumenttable.getdocumentinfo(VS.80).aspx for info on the `GetDocumentInfo` results we're ignoring
            try
                if int dwReadLocksRemaining = 0 && int dwEditLocksRemaining = 0 then // check that this is there are no other read / edit locks
                    if SourceFile.IsCompilable file then
                        if IntPtr.Zero<>unkdoc then
                            match Marshal.GetObjectForIUnknown(unkdoc) with
                            | :? IVsTextLines as tl ->
                                ls.ProjectSitesAndFiles.UnsetSource(tl)
                            | _ -> ()
            finally 
                if IntPtr.Zero <> unkdoc then Marshal.Release(unkdoc)|>ignore
            VSConstants.S_OK

