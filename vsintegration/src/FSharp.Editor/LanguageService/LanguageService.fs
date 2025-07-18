// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

open System
open System.ComponentModel.Design
open System.Runtime.InteropServices
open System.Threading
open System.IO
open System.Collections.Immutable
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Options
open FSharp.Compiler
open FSharp.Compiler.CodeAnalysis
open FSharp.NativeInterop
open Microsoft.VisualStudio
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.LanguageServices.Implementation.LanguageService
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.Text.Outlining
open Microsoft.CodeAnalysis.ExternalAccess.FSharp
open Microsoft.CodeAnalysis.Host.Mef
open Microsoft.VisualStudio.FSharp.Editor.Telemetry
open CancellableTasks
open FSharp.Compiler.Text

#nowarn "9" // NativePtr.toNativeInt
#nowarn "57" // Experimental stuff

type internal RoamingProfileStorageLocation(keyName: string) =
    inherit OptionStorageLocation()

    member _.GetKeyNameForLanguage(languageName: string) =
        let unsubstitutedKeyName = keyName

        match languageName with
        | null -> unsubstitutedKeyName
        | _ ->
            let substituteLanguageName =
                if languageName = FSharpConstants.FSharpLanguageName then
                    "FSharp"
                else
                    languageName

            unsubstitutedKeyName.Replace("%LANGUAGE%", substituteLanguageName)

[<Composition.Shared>]
[<ExportWorkspaceServiceFactory(typeof<IFSharpWorkspaceService>, ServiceLayer.Default)>]
type internal FSharpWorkspaceServiceFactory [<Composition.ImportingConstructor>] (metadataAsSourceService: FSharpMetadataAsSourceService) =

    // We have a lock just in case if multi-threads try to create a new IFSharpWorkspaceService -
    //     but we only want to have a single instance of the FSharpChecker regardless if there are multiple instances of IFSharpWorkspaceService.
    //     In VS, we only ever have a single IFSharpWorkspaceService, but for testing we may have multiple; we still only want a
    //     single FSharpChecker instance shared across them.
    static let gate = obj ()

    // We only ever want to have a single FSharpChecker.
    static let mutable checkerSingleton = None

    interface IWorkspaceServiceFactory with
        member _.CreateService(workspaceServices) =

            let workspace = workspaceServices.Workspace

            let tryGetMetadataSnapshot (path, timeStamp) =
                match workspace with
                | :? VisualStudioWorkspace as workspace ->
                    try
                        let md =
                            LanguageServices.FSharpVisualStudioWorkspaceExtensions.GetMetadata(workspace, path, timeStamp)

                        let amd = (md :?> AssemblyMetadata)
                        let mmd = amd.GetModules().[0]
                        let mmr = mmd.GetMetadataReader()

                        // "lifetime is timed to Metadata you got from the GetMetadata(...). As long as you hold it strongly, raw
                        // memory we got from metadata reader will be alive. Once you are done, just let everything go and
                        // let finalizer handle resource rather than calling Dispose from Metadata directly. It is shared metadata.
                        // You shouldn't dispose it directly."

                        let objToHold = box md

                        // We don't expect any ilread WeakByteFile to be created when working in Visual Studio
                        // Debug.Assert((FSharp.Compiler.AbstractIL.ILBinaryReader.GetStatistics().weakByteFileCount = 0), "Expected weakByteFileCount to be zero when using F# in Visual Studio. Was there a problem reading a .NET binary?")

                        Some(objToHold, NativePtr.toNativeInt mmr.MetadataPointer, mmr.MetadataLength)
                    with ex ->
                        // We catch all and let the backup routines in the F# compiler find the error
                        Assert.Exception(ex)
                        None
                | _ -> None

            let getSource filename =
                async {
                    let! ct = Async.CancellationToken

                    match workspace.CurrentSolution.TryGetDocumentFromPath filename with
                    | ValueSome document ->
                        let! text = document.GetTextAsync(ct) |> Async.AwaitTask
                        return Some(text.ToFSharpSourceText())
                    | ValueNone -> return None
                }

            lock gate (fun () ->
                match checkerSingleton with
                | Some _ -> ()
                | _ ->
                    let checker =
                        lazy
                            let editorOptions = workspace.Services.GetService<EditorOptions>()

                            let enableParallelReferenceResolution =
                                editorOptions.LanguageServicePerformance.EnableParallelReferenceResolution

                            let enableLiveBuffers = editorOptions.Advanced.IsUseLiveBuffersEnabled

                            let enableInMemoryCrossProjectReferences =
                                editorOptions.LanguageServicePerformance.EnableInMemoryCrossProjectReferences

                            let enableFastFindReferences =
                                editorOptions.LanguageServicePerformance.EnableFastFindReferencesAndRename

                            let isInlineParameterNameHintsEnabled =
                                editorOptions.Advanced.IsInlineParameterNameHintsEnabled

                            let isInlineTypeHintsEnabled = editorOptions.Advanced.IsInlineTypeHintsEnabled

                            let isInlineReturnTypeHintsEnabled =
                                editorOptions.Advanced.IsInlineReturnTypeHintsEnabled

                            let enablePartialTypeChecking =
                                editorOptions.LanguageServicePerformance.EnablePartialTypeChecking

                            // Default should be false
                            let keepAllBackgroundResolutions =
                                editorOptions.LanguageServicePerformance.KeepAllBackgroundResolutions

                            // Default should be false
                            let keepAllBackgroundSymbolUses =
                                editorOptions.LanguageServicePerformance.KeepAllBackgroundSymbolUses

                            // Default should be true
                            let enableBackgroundItemKeyStoreAndSemanticClassification =
                                editorOptions.LanguageServicePerformance.EnableBackgroundItemKeyStoreAndSemanticClassification

                            let useTransparentCompiler = editorOptions.Advanced.UseTransparentCompiler

                            // Default is false here
                            let solutionCrawler = editorOptions.Advanced.SolutionBackgroundAnalysis

                            use _eventDuration =
                                TelemetryReporter.ReportSingleEventWithDuration(
                                    TelemetryEvents.LanguageServiceStarted,
                                    [|
                                        nameof enableLiveBuffers, enableLiveBuffers
                                        nameof enableParallelReferenceResolution, enableParallelReferenceResolution
                                        nameof enableInMemoryCrossProjectReferences, enableInMemoryCrossProjectReferences
                                        nameof enableFastFindReferences, enableFastFindReferences
                                        nameof isInlineParameterNameHintsEnabled, isInlineParameterNameHintsEnabled
                                        nameof isInlineTypeHintsEnabled, isInlineTypeHintsEnabled
                                        nameof isInlineReturnTypeHintsEnabled, isInlineReturnTypeHintsEnabled
                                        nameof enablePartialTypeChecking, enablePartialTypeChecking
                                        nameof keepAllBackgroundResolutions, keepAllBackgroundResolutions
                                        nameof keepAllBackgroundSymbolUses, keepAllBackgroundSymbolUses
                                        nameof enableBackgroundItemKeyStoreAndSemanticClassification,
                                        enableBackgroundItemKeyStoreAndSemanticClassification
                                        "captureIdentifiersWhenParsing", enableFastFindReferences
                                        nameof useTransparentCompiler, useTransparentCompiler
                                        nameof solutionCrawler, solutionCrawler
                                    |],
                                    TelemetryThrottlingStrategy.NoThrottling
                                )

                            let checker =
                                FSharpChecker.Create(
                                    projectCacheSize = 5000, // We do not care how big the cache is. VS will actually tell FCS to clear caches, so this is fine.
                                    keepAllBackgroundResolutions = keepAllBackgroundResolutions,
                                    legacyReferenceResolver = LegacyMSBuildReferenceResolver.getResolver (),
                                    tryGetMetadataSnapshot = tryGetMetadataSnapshot,
                                    keepAllBackgroundSymbolUses = keepAllBackgroundSymbolUses,
                                    enableBackgroundItemKeyStoreAndSemanticClassification =
                                        enableBackgroundItemKeyStoreAndSemanticClassification,
                                    enablePartialTypeChecking = enablePartialTypeChecking,
                                    parallelReferenceResolution = enableParallelReferenceResolution,
                                    captureIdentifiersWhenParsing = enableFastFindReferences,
                                    documentSource =
                                        (if enableLiveBuffers then
                                             (DocumentSource.Custom(fun filename ->
                                                 async {
                                                     match! getSource filename with
                                                     | Some source -> return Some(source :> ISourceText)
                                                     | None -> return None
                                                 }))
                                         else
                                             DocumentSource.FileSystem),
                                    useTransparentCompiler = useTransparentCompiler
                                )

                            if enableLiveBuffers && not useTransparentCompiler then
                                workspace.WorkspaceChanged.Add(fun args ->
                                    if args.DocumentId <> null then
                                        cancellableTask {
                                            let document = args.NewSolution.GetDocument(args.DocumentId)

                                            let! _, _, _, options =
                                                document.GetFSharpCompilationOptionsAsync(nameof (workspace.WorkspaceChanged))

                                            do! checker.NotifyFileChanged(document.FilePath, options)
                                        }
                                        |> CancellableTask.startAsTask CancellationToken.None
                                        |> ignore)

                            checker

                    checkerSingleton <- Some checker)

            let optionsManager =
                lazy
                    match checkerSingleton with
                    | Some checker -> FSharpProjectOptionsManager(checker.Value, workspaceServices.Workspace)
                    | _ -> failwith "Checker not set."

            { new IFSharpWorkspaceService with
                member _.Checker =
                    match checkerSingleton with
                    | Some checker -> checker.Value
                    | _ -> failwith "Checker not set."

                member _.FSharpProjectOptionsManager = optionsManager.Value
                member _.MetadataAsSource = metadataAsSourceService
            }
            :> _

[<Sealed>]
type private FSharpSolutionEvents(projectManager: FSharpProjectOptionsManager, metadataAsSource: FSharpMetadataAsSourceService) =

    interface IVsSolutionEvents with

        member _.OnAfterCloseSolution(_) =
            projectManager.Checker.ClearLanguageServiceRootCachesAndCollectAndFinalizeAllTransients()
            metadataAsSource.ClearGeneratedFiles()
            projectManager.ClearAllCaches()
            VSConstants.S_OK

        member _.OnAfterLoadProject(_, _) = VSConstants.E_NOTIMPL

        member _.OnAfterOpenProject(_, _) = VSConstants.E_NOTIMPL

        member _.OnAfterOpenSolution(_, _) = VSConstants.E_NOTIMPL

        member _.OnBeforeCloseProject(_, _) = VSConstants.E_NOTIMPL

        member _.OnBeforeCloseSolution(_) = VSConstants.E_NOTIMPL

        member _.OnBeforeUnloadProject(_, _) = VSConstants.E_NOTIMPL

        member _.OnQueryCloseProject(_, _, _) = VSConstants.E_NOTIMPL

        member _.OnQueryCloseSolution(_, _) = VSConstants.E_NOTIMPL

        member _.OnQueryUnloadProject(_, _) = VSConstants.E_NOTIMPL

[<ExportWorkspaceServiceFactory(typeof<EditorOptions>, ServiceLayer.Default)>]
type internal FSharpSettingsFactory [<Composition.ImportingConstructor>] (settings: EditorOptions) =
    interface Host.Mef.IWorkspaceServiceFactory with
        member _.CreateService(_) = upcast settings

[<Guid(FSharpConstants.packageGuidString)>]
[<ProvideOptionPage(typeof<FSharp.Interactive.FsiPropertyPage>, "F# Tools", "F# Interactive", 6000s, 6001s, true)>] // true = supports automation

[<ProvideKeyBindingTable("{dee22b65-9761-4a26-8fb2-759b971d6dfc}", 6001s)>] // <-- resource ID for localised name

[<ProvideToolWindow(typeof<FSharp.Interactive.FsiToolWindow>,
                    Orientation = ToolWindowOrientation.Bottom,
                    Style = VsDockStyle.Tabbed,
                    PositionX = 0,
                    PositionY = 0,
                    Width = 360,
                    Height = 120,
                    Window = "34E76E81-EE4A-11D0-AE2E-00A0C90FFFC3")
// The following should place the ToolWindow with the OutputWindow by default.
>]
[<ProvideLanguageEditorOptionPage(typeof<OptionsUI.IntelliSenseOptionPage>, "F#", null, "IntelliSense", "6008", "IntelliSensePageKeywords")>]
[<ProvideLanguageEditorOptionPage(typeof<OptionsUI.QuickInfoOptionPage>, "F#", null, "QuickInfo", "6009", "QuickInfoPageKeywords")>]
[<ProvideLanguageEditorOptionPage(typeof<OptionsUI.CodeFixesOptionPage>, "F#", null, "Code Fixes", "6010", "CodeFixesPageKeywords")>]
[<ProvideLanguageEditorOptionPage(typeof<OptionsUI.LanguageServicePerformanceOptionPage>,
                                  "F#",
                                  null,
                                  "Performance",
                                  "6011",
                                  "PerformancePageKeywords")>]
[<ProvideLanguageEditorOptionPage(typeof<OptionsUI.AdvancedSettingsOptionPage>, "F#", null, "Advanced", "6012", "AdvancedPageKeywords")>]
[<ProvideLanguageEditorOptionPage(typeof<OptionsUI.FormattingOptionPage>, "F#", null, "Formatting", "6014", "FormattingPageKeywords")>]
[<ProvideFSharpVersionRegistration(FSharpConstants.projectPackageGuidString, "Microsoft Visual F#")>]
// 64 represents a hex number. It needs to be greater than 37 so the TextMate editor will not be chosen as higher priority.
[<ProvideEditorExtension(typeof<FSharpEditorFactory>, ".fs", 64)>]
[<ProvideEditorExtension(typeof<FSharpEditorFactory>, ".fsi", 64)>]
[<ProvideEditorExtension(typeof<FSharpEditorFactory>, ".fsscript", 64)>]
[<ProvideEditorExtension(typeof<FSharpEditorFactory>, ".fsx", 64)>]
[<ProvideEditorExtension(typeof<FSharpEditorFactory>, ".ml", 64)>]
[<ProvideEditorExtension(typeof<FSharpEditorFactory>, ".mli", 64)>]
[<ProvideEditorFactory(typeof<FSharpEditorFactory>, 101s, CommonPhysicalViewAttributes = Constants.FSharpEditorFactoryPhysicalViewAttributes)>]
[<ProvideLanguageExtension(typeof<FSharpLanguageService>, ".fs")>]
[<ProvideLanguageExtension(typeof<FSharpLanguageService>, ".fsi")>]
[<ProvideLanguageExtension(typeof<FSharpLanguageService>, ".fsx")>]
[<ProvideLanguageExtension(typeof<FSharpLanguageService>, ".fsscript")>]
[<ProvideLanguageExtension(typeof<FSharpLanguageService>, ".ml")>]
[<ProvideLanguageExtension(typeof<FSharpLanguageService>, ".mli")>]
[<ProvideBraceCompletion(FSharpConstants.FSharpLanguageName)>]
[<ProvideLanguageService(languageService = typeof<FSharpLanguageService>,
                         strLanguageName = FSharpConstants.FSharpLanguageName,
                         languageResourceID = 100,
                         MatchBraces = true,
                         MatchBracesAtCaret = true,
                         ShowCompletion = true,
                         ShowMatchingBrace = true,
                         ShowSmartIndent = true,
                         EnableAsyncCompletion = true,
                         QuickInfo = true,
                         DefaultToInsertSpaces = true,
                         CodeSense = true,
                         DefaultToNonHotURLs = true,
                         RequestStockColors = true,
                         EnableCommenting = true,
                         CodeSenseDelay = 100,
                         ShowDropDownOptions = true)>]
type internal FSharpPackage() as this =
    inherit AbstractPackage<FSharpPackage, FSharpLanguageService>()

    let mutable vfsiToolWindow = Unchecked.defaultof<FSharp.Interactive.FsiToolWindow>

    let GetToolWindowAsITestVFSI () =
        if vfsiToolWindow = Unchecked.defaultof<_> then
            vfsiToolWindow <- this.FindToolWindow(typeof<FSharp.Interactive.FsiToolWindow>, 0, true) :?> FSharp.Interactive.FsiToolWindow

        vfsiToolWindow :> FSharp.Interactive.ITestVFSI

    let mutable solutionEventsOpt = None

    // FSI-LINKAGE-POINT: unsited init
    do FSharp.Interactive.Hooks.fsiConsoleWindowPackageCtorUnsited (this :> Package)

#if DEBUG
    do Logging.FSharpServiceTelemetry.logCacheMetricsToOutput ()

    let flushTelemetry = Logging.FSharpServiceTelemetry.otelExport ()

    override this.Dispose(disposing: bool) =
        base.Dispose(disposing: bool)

        if disposing then
            flushTelemetry ()
#endif

    override this.RegisterInitializeAsyncWork(packageRegistrationTasks: PackageLoadTasks) : unit =
        base.RegisterInitializeAsyncWork(packageRegistrationTasks)

        packageRegistrationTasks.AddTask(
            true,
            (fun _tasks cancellationToken ->
                foregroundCancellableTask {
                    let! commandService = this.GetServiceAsync(typeof<IMenuCommandService>)
                    let commandService = commandService :?> OleMenuCommandService

                    // Switch to UI thread
                    do! this.JoinableTaskFactory.SwitchToMainThreadAsync()

                    // FSI-LINKAGE-POINT: sited init
                    FSharp.Interactive.Hooks.fsiConsoleWindowPackageInitializeSited (this :> Package) commandService

                    // FSI-LINKAGE-POINT: private method GetDialogPage forces fsi options to be loaded
                    let _fsiPropertyPage =
                        this.GetDialogPage(typeof<FSharp.Interactive.FsiPropertyPage>)

                    let workspace = this.ComponentModel.GetService<VisualStudioWorkspace>()

                    let _ =
                        this.ComponentModel.DefaultExportProvider.GetExport<HackCpsCommandLineChanges>()

                    let optionsManager =
                        workspace.Services.GetService<IFSharpWorkspaceService>().FSharpProjectOptionsManager

                    let metadataAsSource =
                        this.ComponentModel.DefaultExportProvider.GetExport<FSharpMetadataAsSourceService>().Value

                    let! solution = this.GetServiceAsync(typeof<SVsSolution>)
                    let solution = solution :?> IVsSolution

                    let solutionEvents = FSharpSolutionEvents(optionsManager, metadataAsSource)

                    let! rdt = this.GetServiceAsync(typeof<SVsRunningDocumentTable>)
                    let rdt = rdt :?> IVsRunningDocumentTable

                    solutionEventsOpt <- Some(solutionEvents)
                    solution.AdviseSolutionEvents(solutionEvents) |> ignore

                    let projectContextFactory =
                        this.ComponentModel.GetService<FSharpWorkspaceProjectContextFactory>()

                    let miscFilesWorkspace =
                        this.ComponentModel.GetService<MiscellaneousFilesWorkspace>()

                    do
                        SingleFileWorkspaceMap(FSharpMiscellaneousFileService(workspace, miscFilesWorkspace, projectContextFactory), rdt)
                        |> ignore

                    do
                        LegacyProjectWorkspaceMap(solution, optionsManager, projectContextFactory)
                        |> ignore

                }
                |> CancellableTask.startAsTask cancellationToken)
        )

    override _.RoslynLanguageName = FSharpConstants.FSharpLanguageName
    (*override this.CreateWorkspace() = this.ComponentModel.GetService<VisualStudioWorkspaceImpl>() *)
    override this.CreateLanguageService() = FSharpLanguageService(this)

    override this.CreateEditorFactories() =
        seq { yield FSharpEditorFactory(this) :> IVsEditorFactory }

    override _.RegisterMiscellaneousFilesWorkspaceInformation(miscFilesWorkspace) =
        miscFilesWorkspace.RegisterLanguage(Guid(FSharpConstants.languageServiceGuidString), FSharpConstants.FSharpLanguageName, ".fsx")

    interface FSharp.Interactive.ITestVFSI with
        member _.SendTextInteraction(s: string) =
            GetToolWindowAsITestVFSI().SendTextInteraction(s)

        member _.GetMostRecentLines(n: int) : string[] =
            GetToolWindowAsITestVFSI().GetMostRecentLines(n)

[<Guid(FSharpConstants.languageServiceGuidString)>]
type internal FSharpLanguageService(package: FSharpPackage) =
    inherit AbstractLanguageService<FSharpPackage, FSharpLanguageService>(package)

    member _.Initialize() =
        let exportProvider = package.ComponentModel.DefaultExportProvider
        let globalOptions = exportProvider.GetExport<FSharpGlobalOptions>().Value

        let workspace = package.ComponentModel.GetService<VisualStudioWorkspace>()

        let solutionAnalysis =
            workspace.Services.GetService<EditorOptions>().Advanced.SolutionBackgroundAnalysis

        globalOptions.SetBackgroundAnalysisScope(openFilesOnly = not solutionAnalysis)

        globalOptions.BlockForCompletionItems <- false

        let theme = exportProvider.GetExport<ISetThemeColors>().Value
        theme.SetColors()

    override _.ContentTypeName = FSharpConstants.FSharpContentTypeName
    override _.LanguageName = FSharpConstants.FSharpLanguageName
    override _.RoslynLanguageName = FSharpConstants.FSharpLanguageName

    override _.LanguageServiceId = new Guid(FSharpConstants.languageServiceGuidString)
    override _.DebuggerLanguageId = CompilerEnvironment.GetDebuggerLanguageID()

    override _.CreateContext(_, _, _, _, _) = raise (NotImplementedException())

    override this.SetupNewTextView(textView) =
        base.SetupNewTextView(textView)

        // Toggles outlining (or code folding) based on settings
        let outliningManagerService =
            this.Package.ComponentModel.GetService<IOutliningManagerService>()

        let wpfTextView = this.EditorAdaptersFactoryService.GetWpfTextView(textView)
        let outliningManager = outliningManagerService.GetOutliningManager(wpfTextView)

        if not (isNull outliningManager) then
            let settings = this.Workspace.Services.GetService<EditorOptions>()
            outliningManager.Enabled <- settings.Advanced.IsOutliningEnabled

[<Composition.Shared>]
[<ComponentModel.Composition.Export(typeof<HackCpsCommandLineChanges>)>]
type internal HackCpsCommandLineChanges
    [<ComponentModel.Composition.ImportingConstructor>]
    ([<ComponentModel.Composition.Import(typeof<VisualStudioWorkspace>)>] workspace: VisualStudioWorkspace) =

    static let projectDisplayNameOf projectFileName =
        if String.IsNullOrWhiteSpace projectFileName then
            projectFileName
        else
            Path.GetFileNameWithoutExtension projectFileName

    /// This handles commandline change notifications from the Dotnet Project-system
    /// Prior to VS 15.7 path contained path to project file, post 15.7 contains target binpath
    /// binpath is more accurate because a project file can have multiple in memory projects based on configuration
    [<ComponentModel.Composition.Export>]
    member _.HandleCommandLineChanges
        (
            path: string,
            sources: ImmutableArray<CommandLineSourceFile>,
            references: ImmutableArray<CommandLineReference>,
            options: ImmutableArray<string>
        ) =
        use _logBlock =
            Logger.LogBlock(LogEditorFunctionId.LanguageService_HandleCommandLineArgs)

        let projectId =
            match
                Microsoft.CodeAnalysis.ExternalAccess.FSharp.LanguageServices.FSharpVisualStudioWorkspaceExtensions.TryGetProjectIdByBinPath(
                    workspace,
                    path
                )
            with
            | true, projectId -> projectId
            | false, _ ->
                LanguageServices.FSharpVisualStudioWorkspaceExtensions.GetOrCreateProjectIdForPath(
                    workspace,
                    path,
                    projectDisplayNameOf path
                )

        let path =
            Microsoft.CodeAnalysis.ExternalAccess.FSharp.LanguageServices.FSharpVisualStudioWorkspaceExtensions.GetProjectFilePath(
                workspace,
                projectId
            )

        let getFullPath p =
            let p' =
                if Path.IsPathRooted(p) || path = null then
                    p
                else
                    Path.Combine(Path.GetDirectoryName(path), p)

            Path.GetFullPathSafe(p')

        let sourcePaths = sources |> Seq.map (fun s -> getFullPath s.Path) |> Seq.toArray

        // Due to an issue in project system, when we close and reopen solution, it sends the CommandLineChanges twice for every project.
        // First time it sends a correct path, sources, references and options.
        // Second time it sends a correct path, empty sources, empty references and empty options, and we rewrite our cache, and fail to colourize the document later.
        // As a workaround, until we have a fix from PS or will move to Roslyn as a source of truth, we will not overwrite the cache in case of empty lists.

        if not (sources.IsEmpty && references.IsEmpty && options.IsEmpty) then
            let workspaceService =
                workspace.Services.GetRequiredService<IFSharpWorkspaceService>()

            workspaceService.FSharpProjectOptionsManager.SetCommandLineOptions(projectId, sourcePaths, options)
