// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.LanguageService

open System
open System.Collections.Generic
open System.Runtime.InteropServices

open Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.CodeAnalysis
open Microsoft.VisualStudio
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.LanguageServices.Implementation.LanguageService
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem
open Microsoft.VisualStudio.LanguageServices.Implementation.DebuggerIntelliSense
open Microsoft.VisualStudio.LanguageServices.Implementation
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop

// Workaround to access non-public settings persistence type.
// GetService( ) with this will work as long as the GUID matches the real type.
[<Guid(FSharpCommonConstants.svsSettingsPersistenceManagerGuidString)>]
type internal SVsSettingsPersistenceManager = class end

[<Guid(FSharpCommonConstants.languageServiceGuidString)>]

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
type internal FSharpLanguageService(package : FSharpPackage) = 
    inherit AbstractLanguageService<FSharpPackage, FSharpLanguageService, FSharpProjectSite>(package)

    override this.ContentTypeName = FSharpCommonConstants.FSharpContentTypeName
    override this.LanguageName = FSharpCommonConstants.FSharpLanguageName
    override this.RoslynLanguageName = FSharpCommonConstants.FSharpLanguageName

    override this.LanguageServiceId = new Guid(FSharpCommonConstants.languageServiceGuidString)
    override this.DebuggerLanguageId = DebuggerEnvironment.GetLanguageID()

    override this.CreateContext(_,_,_,_,_) = raise(System.NotImplementedException())

    override this.SetupNewTextView(view) =
        base.SetupNewTextView(view)
        let workspace = this.Package.ComponentModel.GetService<VisualStudioWorkspaceImpl>();

        // Ensure that we have a project in the workspace for this document.
        let (_, buffer) = view.GetBuffer()
        let filename = VsTextLines.GetFilename buffer
        let result = VsRunningDocumentTable.FindDocumentWithoutLocking(package.RunningDocumentTable,filename)
        match result with
        | Some (hier, _) ->
            match hier with
            | :? IProvideProjectSite as siteProvider ->
                let site = siteProvider.GetProjectSite()
                let projectFileName = site.ProjectFileName()
                let projectId = workspace.ProjectTracker.GetOrCreateProjectIdForPath(projectFileName, projectFileName)
                if obj.ReferenceEquals(workspace.ProjectTracker.GetProject(projectId), null) then
                    let projectSite = new FSharpProjectSite(hier, this.SystemServiceProvider, workspace, projectFileName);
                    projectSite.Initialize(hier, site)                    
            | _ -> ()
        | _ -> ()

and 
    [<ProvideLanguageService(FSharpCommonConstants.languageServiceGuidString, FSharpCommonConstants.FSharpContentTypeName, 1)>]
    [<Guid(FSharpCommonConstants.packageGuidString)>]
    internal FSharpPackage() = 
    inherit AbstractPackage<FSharpPackage, FSharpLanguageService, FSharpProjectSite>()
    
    override this.RoslynLanguageName = FSharpCommonConstants.FSharpLanguageName

    override this.Initialize() = 
        base.Initialize()
        this.EstablishDefaultSettingsIfMissing()

    override this.CreateWorkspace() = this.ComponentModel.GetService<VisualStudioWorkspaceImpl>()
    
    override this.CreateLanguageService() = new FSharpLanguageService(this)

    override this.CreateEditorFactories() = Seq.empty<IVsEditorFactory>

    override this.RegisterMiscellaneousFilesWorkspaceInformation(_) = ()
    
    /// ISettingsManager only implemented for VS 14.0+
    /// In case custom VS profile settings for F# are not applied, explicitly set them here.
    /// e.g. 'keep tabs' is the text editor default, but F# requires 'insert spaces'.
    /// We specify our customizations in the General profile for VS, but we have found that in some cases,
    /// those customizations are incorrectly ignored. So we take action if the setting has no current custom value.
    member private this.EstablishDefaultSettingsIfMissing() =
        #if !VS_VERSION_DEV12

        let fsharpSpecificProfileSettings = [|
            "TextEditor.F#.Insert Tabs", box false
            "TextEditor.F#.Brace Completion", box true
            "TextEditor.F#.Make URLs Hot", box false
            "TextEditor.F#.Indent Style", box 1u |]

        match this.GetService(typeof<SVsSettingsPersistenceManager>) with
        | :? Microsoft.VisualStudio.Settings.ISettingsManager as settingsManager ->
            for settingName,defaultValue in fsharpSpecificProfileSettings do
                match settingsManager.TryGetValue(settingName) with
                | Microsoft.VisualStudio.Settings.GetValueResult.Missing, _ ->
                    settingsManager.SetValueAsync(settingName, defaultValue, false) |> ignore
                | _ -> ()
        | _ -> ()

        #endif