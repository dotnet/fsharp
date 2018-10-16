// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

namespace rec Microsoft.VisualStudio.FSharp.Editor

#nowarn "40"

open System
open System.Collections.Concurrent
open System.Collections.Generic
open System.Collections.Immutable
open System.ComponentModel.Composition
open System.ComponentModel.Design
open System.Diagnostics
open System.IO
open System.Linq
open System.Runtime.CompilerServices
open System.Runtime.InteropServices
open System.Threading
open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Diagnostics
open Microsoft.CodeAnalysis.Completion
open Microsoft.CodeAnalysis.Options
open Microsoft.FSharp.Compiler.CompileOps
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.VisualStudio
open Microsoft.VisualStudio.Editor
open Microsoft.VisualStudio.FSharp.Editor
open Microsoft.VisualStudio.FSharp.Editor.SiteProvider
open Microsoft.VisualStudio.TextManager.Interop
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.LanguageServices.Implementation.LanguageService
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem
open Microsoft.VisualStudio.LanguageServices.Implementation.TaskList
open Microsoft.VisualStudio.LanguageServices.ProjectSystem
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Events
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.ComponentModelHost
open Microsoft.VisualStudio.Text.Outlining
open FSharp.NativeInterop

#nowarn "9" // NativePtr.toNativeInt

// Used to expose FSharpChecker/ProjectInfo manager to diagnostic providers
// Diagnostic providers can be executed in environment that does not use MEF so they can rely only
// on services exposed by the workspace
type internal FSharpCheckerWorkspaceService =
    inherit Microsoft.CodeAnalysis.Host.IWorkspaceService
    abstract Checker: FSharpChecker
    abstract FSharpProjectOptionsManager: FSharpProjectOptionsManager

type internal RoamingProfileStorageLocation(keyName: string) =
    inherit OptionStorageLocation()
    
    member __.GetKeyNameForLanguage(languageName: string) =
        let unsubstitutedKeyName = keyName
        match languageName with
        | null -> unsubstitutedKeyName
        | _ ->
            let substituteLanguageName = if languageName = FSharpConstants.FSharpLanguageName then "FSharp" else languageName
            unsubstitutedKeyName.Replace("%LANGUAGE%", substituteLanguageName)
 
[<Composition.Shared>]
[<Microsoft.CodeAnalysis.Host.Mef.ExportWorkspaceServiceFactory(typeof<FSharpCheckerWorkspaceService>, Microsoft.CodeAnalysis.Host.Mef.ServiceLayer.Default)>]
type internal FSharpCheckerWorkspaceServiceFactory
    [<Composition.ImportingConstructor>]
    (
        checkerProvider: FSharpCheckerProvider,
        projectInfoManager: FSharpProjectOptionsManager
    ) =
    interface Microsoft.CodeAnalysis.Host.Mef.IWorkspaceServiceFactory with
        member this.CreateService(_workspaceServices) =
            upcast { new FSharpCheckerWorkspaceService with
                member this.Checker = checkerProvider.Checker
                member this.FSharpProjectOptionsManager = projectInfoManager }

[<Microsoft.CodeAnalysis.Host.Mef.ExportWorkspaceServiceFactory(typeof<EditorOptions>, Microsoft.CodeAnalysis.Host.Mef.ServiceLayer.Default)>]
type internal FSharpSettingsFactory
    [<Composition.ImportingConstructor>] (settings: EditorOptions) =
    interface Microsoft.CodeAnalysis.Host.Mef.IWorkspaceServiceFactory with
        member this.CreateService(_) = upcast settings

[<Guid(FSharpConstants.packageGuidString)>]
[<ProvideOptionPage(typeof<Microsoft.VisualStudio.FSharp.Interactive.FsiPropertyPage>,
                    "F# Tools", "F# Interactive",   // category/sub-category on Tools>Options...
                    6000s,      6001s,              // resource id for localisation of the above
                    true)>]                         // true = supports automation
[<ProvideKeyBindingTable("{dee22b65-9761-4a26-8fb2-759b971d6dfc}", 6001s)>] // <-- resource ID for localised name
[<ProvideToolWindow(typeof<Microsoft.VisualStudio.FSharp.Interactive.FsiToolWindow>, 
                    // The following should place the ToolWindow with the OutputWindow by default.
                    Orientation=ToolWindowOrientation.Bottom,
                    Style=VsDockStyle.Tabbed,
                    PositionX = 0,
                    PositionY = 0,
                    Width = 360,
                    Height = 120,
                    Window="34E76E81-EE4A-11D0-AE2E-00A0C90FFFC3")>]
[<ProvideLanguageEditorOptionPage(typeof<OptionsUI.IntelliSenseOptionPage>, "F#", null, "IntelliSense", "6008")>]
[<ProvideLanguageEditorOptionPage(typeof<OptionsUI.QuickInfoOptionPage>, "F#", null, "QuickInfo", "6009")>]
[<ProvideLanguageEditorOptionPage(typeof<OptionsUI.CodeFixesOptionPage>, "F#", null, "Code Fixes", "6010")>]
[<ProvideLanguageEditorOptionPage(typeof<OptionsUI.LanguageServicePerformanceOptionPage>, "F#", null, "Performance", "6011")>]
[<ProvideLanguageEditorOptionPage(typeof<OptionsUI.AdvancedSettingsOptionPage>, "F#", null, "Advanced", "6012")>]
[<ProvideLanguageEditorOptionPage(typeof<OptionsUI.CodeLensOptionPage>, "F#", null, "CodeLens", "6013")>]
[<ProvideLanguageEditorOptionPage(typeof<OptionsUI.FormattingOptionPage>, "F#", null, "Formatting", "6014")>]
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

    let mutable vfsiToolWindow = Unchecked.defaultof<Microsoft.VisualStudio.FSharp.Interactive.FsiToolWindow>
    let GetToolWindowAsITestVFSI() =
        if vfsiToolWindow = Unchecked.defaultof<_> then
            vfsiToolWindow <- this.FindToolWindow(typeof<Microsoft.VisualStudio.FSharp.Interactive.FsiToolWindow>, 0, true) :?> Microsoft.VisualStudio.FSharp.Interactive.FsiToolWindow
        vfsiToolWindow :> Microsoft.VisualStudio.FSharp.Interactive.ITestVFSI

    // FSI-LINKAGE-POINT: unsited init
    do Microsoft.VisualStudio.FSharp.Interactive.Hooks.fsiConsoleWindowPackageCtorUnsited (this :> Package)

    override this.Initialize() =
        base.Initialize()

        // FSI-LINKAGE-POINT: sited init
        let commandService = this.GetService(typeof<IMenuCommandService>) :?> OleMenuCommandService // FSI-LINKAGE-POINT
        Microsoft.VisualStudio.FSharp.Interactive.Hooks.fsiConsoleWindowPackageInitalizeSited (this :> Package) commandService
        // FSI-LINKAGE-POINT: private method GetDialogPage forces fsi options to be loaded
        let _fsiPropertyPage = this.GetDialogPage(typeof<Microsoft.VisualStudio.FSharp.Interactive.FsiPropertyPage>)

        ()

    override this.RoslynLanguageName = FSharpConstants.FSharpLanguageName
    override this.CreateWorkspace() = this.ComponentModel.GetService<VisualStudioWorkspaceImpl>()
    override this.CreateLanguageService() = FSharpLanguageService(this, this.GetService(typeof<SVsSolution>) :?> IVsSolution)
    override this.CreateEditorFactories() = seq { yield FSharpEditorFactory(this) :> IVsEditorFactory }
    override this.RegisterMiscellaneousFilesWorkspaceInformation(_) = ()

    interface Microsoft.VisualStudio.FSharp.Interactive.ITestVFSI with
        member this.SendTextInteraction(s:string) =
            GetToolWindowAsITestVFSI().SendTextInteraction(s)
        member this.GetMostRecentLines(n:int) : string[] =
            GetToolWindowAsITestVFSI().GetMostRecentLines(n)

[<Guid(FSharpConstants.languageServiceGuidString)>]
type internal FSharpLanguageService(package : FSharpPackage, solution: IVsSolution) =
    inherit AbstractLanguageService<FSharpPackage, FSharpLanguageService>(package)

    let projectInfoManager = package.ComponentModel.DefaultExportProvider.GetExport<FSharpProjectOptionsManager>().Value

    let projectDisplayNameOf projectFileName =
        if String.IsNullOrWhiteSpace projectFileName then projectFileName
        else Path.GetFileNameWithoutExtension projectFileName

    let singleFileProjects = ConcurrentDictionary<_, IWorkspaceProjectContext>()

    let tryRemoveSingleFileProject projectId =
        match singleFileProjects.TryRemove(projectId) with
        | true, project ->
            projectInfoManager.ClearInfoForSingleFileProject(projectId)
            project.Dispose()
        | _ -> ()

    let tryGetOrCreateProjectId (workspace: VisualStudioWorkspaceImpl) (projectFileName: string) =
        let projectDisplayName = projectDisplayNameOf projectFileName
        Some (workspace.ProjectTracker.GetOrCreateProjectIdForPath(projectFileName, projectDisplayName))

    let mutable legacyProjectWorkspaceMap = Unchecked.defaultof<LegacyProjectWorkspaceMap>

    override this.Initialize() = 
        base.Initialize()

        this.Workspace.Options <- this.Workspace.Options.WithChangedOption(Completion.CompletionOptions.BlockForCompletionItems, FSharpConstants.FSharpLanguageName, false)
        this.Workspace.Options <- this.Workspace.Options.WithChangedOption(Shared.Options.ServiceFeatureOnOffOptions.ClosedFileDiagnostic, FSharpConstants.FSharpLanguageName, Nullable false)

        this.Workspace.DocumentClosed.Add <| fun args -> tryRemoveSingleFileProject args.Document.Project.Id

        legacyProjectWorkspaceMap <- LegacyProjectWorkspaceMap(this.Workspace, solution, projectInfoManager, package.ComponentModel.GetService<IWorkspaceProjectContextFactory>(), this.SystemServiceProvider)
        legacyProjectWorkspaceMap.Initialize()

        let theme = package.ComponentModel.DefaultExportProvider.GetExport<ISetThemeColors>().Value
        theme.SetColors()

    member this.SetupStandAloneFile(fileName: string, fileContents: string, workspace: VisualStudioWorkspaceImpl, hier: IVsHierarchy) =
        let loadTime = DateTime.Now
        let projectFileName = fileName
        let projectDisplayName = projectDisplayNameOf projectFileName

        let mutable projectId = workspace.ProjectTracker.GetOrCreateProjectIdForPath(projectFileName, projectDisplayName)

        if isNull (workspace.ProjectTracker.GetProject projectId) then
            let projectContextFactory = package.ComponentModel.GetService<IWorkspaceProjectContextFactory>();

            let projectContext = projectContextFactory.CreateProjectContext(FSharpConstants.FSharpLanguageName, projectDisplayName, projectFileName, projectId.Id, hier, null)
            
            projectId <- workspace.ProjectTracker.GetOrCreateProjectIdForPath(projectFileName, projectDisplayName)

            projectContext.AddSourceFile(fileName)
            
            singleFileProjects.[projectId] <- projectContext

        let _referencedProjectFileNames, parsingOptions, projectOptions = projectInfoManager.ComputeSingleFileOptions (tryGetOrCreateProjectId workspace, fileName, loadTime, fileContents) |> Async.RunSynchronously
        projectInfoManager.AddOrUpdateSingleFileProject(projectId, (loadTime, parsingOptions, projectOptions))

    override this.ContentTypeName = FSharpConstants.FSharpContentTypeName
    override this.LanguageName = FSharpConstants.FSharpLanguageName
    override this.RoslynLanguageName = FSharpConstants.FSharpLanguageName

    override this.LanguageServiceId = new Guid(FSharpConstants.languageServiceGuidString)
    override this.DebuggerLanguageId = DebuggerEnvironment.GetLanguageID()

    override this.CreateContext(_,_,_,_,_) = raise(System.NotImplementedException())

    override this.SetupNewTextView(textView) =
        base.SetupNewTextView(textView)

        let textViewAdapter = package.ComponentModel.GetService<IVsEditorAdaptersFactoryService>()

        // Toggles outlining (or code folding) based on settings
        let outliningManagerService = this.Package.ComponentModel.GetService<IOutliningManagerService>()
        let wpfTextView = this.EditorAdaptersFactoryService.GetWpfTextView(textView)
        let outliningManager = outliningManagerService.GetOutliningManager(wpfTextView)
        if not (isNull outliningManager) then
            let settings = this.Workspace.Services.GetService<EditorOptions>()
            outliningManager.Enabled <- settings.Advanced.IsOutliningEnabled

        match textView.GetBuffer() with
        | (VSConstants.S_OK, textLines) ->
            let filename = VsTextLines.GetFilename textLines

            match VsRunningDocumentTable.FindDocumentWithoutLocking(package.RunningDocumentTable,filename) with
            | Some (hier, _) ->


                // Check if the file is in a CPS project or not.
                // CPS projects don't implement IProvideProjectSite and IVSProjectHierarchy
                // Simple explanation:
                //    Legacy projects have IVSHierarchy and IProjectSite
                //    CPS Projects, out-of-project file and script files don't

                match hier with
                | :? IProvideProjectSite as _siteProvider when not (IsScript(filename)) ->

                    // This is the path for .fs/.fsi files in legacy projects
                    ()
                | h when not (isNull h) && not (IsScript(filename)) ->
                    let docId = this.Workspace.CurrentSolution.GetDocumentIdsWithFilePath(filename).FirstOrDefault()
                    match docId with
                    | null ->
                        if not (h.IsCapabilityMatch("CPS")) then

                            // This is the path when opening out-of-project .fs/.fsi files in CPS projects

                            let fileContents = VsTextLines.GetFileContents(textLines, textViewAdapter)
                            this.SetupStandAloneFile(filename, fileContents, this.Workspace, hier)
                    | _ -> ()
                | _ ->

                    // This is the path for both in-project and out-of-project .fsx files

                    let fileContents = VsTextLines.GetFileContents(textLines, textViewAdapter)
                    this.SetupStandAloneFile(filename, fileContents, this.Workspace, hier)

            | _ -> ()
        | _ -> ()
