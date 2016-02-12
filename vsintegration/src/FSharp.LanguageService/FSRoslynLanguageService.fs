// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.LanguageService

open System
open System.Collections.Generic
open System.Runtime.InteropServices

open Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.SolutionCrawler
open Microsoft.VisualStudio
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.LanguageServices.Implementation.LanguageService
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem
open Microsoft.VisualStudio.LanguageServices.Implementation
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop

[<Guid(FSRoslynCommonConstants.languageServiceGuid)>]
type FSRoslynLanguageService(package : FSRoslynPackage) = 
    inherit AbstractLanguageService<FSRoslynPackage, FSRoslynLanguageService, FSRoslynProject>(package)

    override this.ContentTypeName = FSRoslynCommonConstants.FSharpContentType
    override this.LanguageName = FSRoslynCommonConstants.FSharpLanguageName
    override this.RoslynLanguageName = FSRoslynCommonConstants.FSharpLanguageName

    override this.LanguageServiceId = new Guid(FSRoslynCommonConstants.languageServiceGuid)
    override this.DebuggerLanguageId = DebuggerEnvironment.GetLanguageID()

    override this.CreateContext(_,_,_,_,_) = raise(System.NotImplementedException())

    override this.SetupNewTextView(view) =
        base.SetupNewTextView(view)
        let workspace = this.Package.ComponentModel.GetService<VisualStudioWorkspaceImpl>();
        let sp = new ServiceProvider(this.SystemServiceProvider.GetService())

        // Ensure that we have a project in the workspace for this document.
        let (_, buffer) = view.GetBuffer()
        let filename = VsTextLines.GetFilename buffer
        let result = VsRunningDocumentTable.FindDocumentWithoutLocking(sp.RunningDocumentTable,filename)
        match result with
        | Some (hier, _) ->
            match hier with
            | :? IProvideProjectSite as siteProvider ->
                let site = siteProvider.GetProjectSite()

                let projectId = workspace.ProjectTracker.GetOrCreateProjectIdForPath(site.ProjectFileName(), site.ProjectFileName())
                if obj.ReferenceEquals(workspace.ProjectTracker.GetProject(projectId), null) then
                    let project = new FSRoslynProject(hier, this.SystemServiceProvider, workspace, site.ProjectFileName());
                    project.Initialize(hier, site)                    
            | _ -> ()
        | _ -> ()

and [<Guid(FSRoslynCommonConstants.editorFactoryGuid)>]
    FSharpEditorFactory(package : FSRoslynPackage) =
    inherit AbstractEditorFactory(package)

    override this.ContentTypeName = FSRoslynCommonConstants.FSharpContentType
    override this.GetFormattedTextChanges(_, _, _, _) = System.Collections.Generic.List<Text.TextChange>() :> System.Collections.Generic.IList<Text.TextChange>

and [<Guid(FSRoslynCommonConstants.packageGuid)>]
    FSRoslynPackage() = 
    inherit AbstractPackage<FSRoslynPackage, FSRoslynLanguageService, FSRoslynProject>()
    
    override this.RoslynLanguageName = FSRoslynCommonConstants.FSharpLanguageName

    override this.Initialize() = 
        if LanguageServiceUtils.shouldEnableRoslynLanguageService then
            base.Initialize()

    override this.CreateWorkspace() = this.ComponentModel.GetService<VisualStudioWorkspaceImpl>()
    
    override this.CreateLanguageService() = 
        let language = new FSRoslynLanguageService(this)
        language

    override this.CreateEditorFactories() = 
        //let factory = FSharpEditorFactory(this) :> IVsEditorFactory
        [] :> IEnumerable<IVsEditorFactory>

    override this.RegisterMiscellaneousFilesWorkspaceInformation(_) = ()
