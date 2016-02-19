// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

namespace Microsoft.VisualStudio.FSharp.Editor

open System.Composition

open Microsoft.CodeAnalysis
open Microsoft.CodeAnalysis.Host
open Microsoft.CodeAnalysis.Host.Mef

open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem

// Currently the CompilationOptions type in Roslyn is sealed and there's no way to set the compilation options for a project.
// There's no property bag on a project either. So this service is a means to get the host project for a given Roslyn project
// so that extra F# specific information can be stored on the host project.
// Note that the FSharpProject is available only through the VS Workspace although we might call this service from projects of 
// some other workspace like the PreviewWorkspace.
type internal IHostProjectService =
    inherit IWorkspaceService

    abstract member GetHostProject : id : ProjectId -> FSharpProjectSite

[<ExportWorkspaceServiceFactory(typeof<IHostProjectService>, ServiceLayer.Default); Shared>]
type internal FSharpProjectSiteService [<ImportingConstructor>] (vsWorkspace : VisualStudioWorkspaceImpl) =
    interface IWorkspaceServiceFactory with
        member this.CreateService(_) = upcast this

    interface IHostProjectService with
        member this.GetHostProject(id:ProjectId) =
            downcast vsWorkspace.GetHostProject(id)