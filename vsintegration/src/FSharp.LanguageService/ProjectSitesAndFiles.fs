// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Project information handling.
//
// For the old F# project system, in a running Visual Studio, the **authoritative** view 
// of the project information is maintained by Project.fs in FSharp.ProjectSystem. This 
// information is conveyed to the rest of the implementation via an IProjectSite interface.
//
// For most purposes, an IProjectSite has to provide three main things
//   - the source files`
//   - the compilation options
//   - the assembly references. 
// Project.fs collects the first two from MSBuild. For the third - assembly references - it looks
// through the nodes of the hierarchy for the F# project. There seems to be an essentially duplicated
// version of this code in this file.
//
// In our LanguageService.fs, FSharpProjectOptionsManager uses this IProjectSite information to incrementally maintain
// a corresponding F# CompilerService FSharpProjectOptions value. 
//
// In our LanguageService.fs, we also use this IProjectSite information to maintain a
// corresponding Roslyn project in the workspace. This is done by SetupProjectFile and SyncProject
// making various calls to add/remove methods such as
//    projectContext.AddSourceFile
//    projectContext.RemoveSourceFile
//    projectContext.AddMetadataReference
//    projectContext.RemoveMetadataReference
//    project.AddProjectReference
// 
// The new F# project system supplies the project information using a Roslyn project in the workspace.
// This means a lot of the stuff above is irrelevant in that case, apart from where FSharpProjectOptionsManager
// incrementally maintains a corresponding F# CompilerService FSharpProjectOptions value. 

module internal rec Microsoft.VisualStudio.FSharp.LanguageService.SiteProvider

open System
open System.Collections.Concurrent
open System.ComponentModel.Composition
open System.IO
open System.Diagnostics
open Microsoft.VisualStudio
open Microsoft.VisualStudio.TextManager.Interop
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.FSharp.Compiler.SourceCodeServices

open Microsoft.CodeAnalysis
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem
open Microsoft.VisualStudio.LanguageServices.Implementation.TaskList
open VSLangProj
open System.ComponentModel.Composition.Primitives
open Microsoft.VisualStudio.Shell
open System.Collections.Immutable


/// An additional interface that an IProjectSite object can implement to indicate it has an FSharpProjectOptions 
/// already available, so we don't have to recreate it
type private IHaveCheckOptions = 
    abstract OriginalCheckOptions : unit -> string[] * FSharpProjectOptions

let projectDisplayNameOf projectFileName =
    if String.IsNullOrWhiteSpace projectFileName then projectFileName
    else Path.GetFileNameWithoutExtension projectFileName

/// A value and a function to recompute/refresh the value.  The function is passed a flag indicating if a refresh is happening.
type Refreshable<'T> = 'T * (bool -> 'T)

/// Convert from FSharpProjectOptions into IProjectSite.
type private ProjectSiteOfScriptFile(filename:string, referencedProjectFileNames, checkOptions: FSharpProjectOptions) = 
    interface IProjectSite with
        override this.Description = sprintf "Script Closure at Root %s" filename
        override this.CompilationSourceFiles = checkOptions.SourceFiles
        override this.CompilationOptions = checkOptions.OtherOptions
        override this.CompilationReferences =
             checkOptions.OtherOptions 
             |> Array.choose (fun flag -> if flag.StartsWith("-r:") then Some flag.[3..] else None) 
        override this.CompilationBinOutputPath = None
        override this.ProjectFileName = checkOptions.ProjectFileName
        override this.BuildErrorReporter with get() = None and set _ = ()
        override this.AdviseProjectSiteChanges(_,_) = ()
        override this.AdviseProjectSiteCleaned(_,_) = ()
        override this.AdviseProjectSiteClosed(_,_) = ()
        override this.IsIncompleteTypeCheckEnvironment = checkOptions.IsIncompleteTypeCheckEnvironment
        override this.TargetFrameworkMoniker = ""
        override this.ProjectGuid = ""
        override this.LoadTime = checkOptions.LoadTime
        override this.ProjectProvider = None

    interface IHaveCheckOptions with
        override this.OriginalCheckOptions() = (referencedProjectFileNames, checkOptions)

    override x.ToString() = sprintf "ProjectSiteOfScriptFile(%s)" filename

/// An orphan file project is a .fs, .ml, .fsi, .mli that is not associated with a .fsproj.
/// By design, these are never going to typecheck because there is no affiliated references.
/// We show many squiggles in this case because they're not particularly informational. 
type private ProjectSiteOfSingleFile(sourceFile) =
    // CompilerFlags() gets called a lot, so pre-compute what we can
    static let compilerFlags = 
        let flags = ["--noframework";"--warn:3"]
        let assumeDotNetFramework = true
        let defaultReferences = 
                [ for r in CompilerEnvironment.DefaultReferencesForOrphanSources(assumeDotNetFramework) do 
                    yield sprintf "-r:%s%s" r (if r.EndsWith(".dll",StringComparison.OrdinalIgnoreCase) then "" else ".dll") ]
        (flags @ defaultReferences)
        |> List.toArray 
        |> Array.choose (fun flag -> if flag.StartsWith("-r:") then Some flag.[3..] elif flag.StartsWith("--reference:") then Some flag.[12..] else None)

    let projectFileName = sourceFile + ".orphan.fsproj"

    interface IProjectSite with
        override this.Description = projectFileName
        override this.CompilationSourceFiles = [|sourceFile|]
        override this.CompilationOptions = compilerFlags
        override this.CompilationReferences = compilerFlags
        override this.CompilationBinOutputPath = None
        override this.ProjectFileName = projectFileName
        override this.BuildErrorReporter with get() = None and set _v = ()
        override this.AdviseProjectSiteChanges(_,_) = ()
        override this.AdviseProjectSiteCleaned(_,_) = ()
        override this.AdviseProjectSiteClosed(_,_) = ()
        override this.IsIncompleteTypeCheckEnvironment = true
        override this.TargetFrameworkMoniker = ""
        override this.ProjectGuid = ""
        override this.LoadTime = new DateTime(2000,1,1)  // any constant time is fine, orphan files do not interact with reloading based on update time
        override this.ProjectProvider = None

    override x.ToString() = sprintf "ProjectSiteOfSingleFile(%s)" sourceFile

/// Manage Storage of FSharpProjectOptions the options for a project
type internal FSharpProjectOptionsTable () =

    // A table of information about projects, excluding single-file projects.
    let projectTable = ConcurrentDictionary<ProjectId, Refreshable<ProjectId[] * FSharpParsingOptions * IProjectSite option * FSharpProjectOptions>>()
    let commandLineOptions = new ConcurrentDictionary<ProjectId, string[]*string[]*string[]>()

    /// Re-fetch all of the options for everything that references projectId
    let refreshInfoForProjectsThatReferenceThisProject (projectId:ProjectId) =
        for KeyValue(otherProjectId, ((referencedProjectIds, _parsingOptions, _site, _options), refresh)) in projectTable.ToArray() do
           for referencedProjectId in referencedProjectIds do
              if referencedProjectId = projectId then 
                  projectTable.[otherProjectId] <- (refresh true, refresh)

    /// Add or update a project in the project table
    member __.AddOrUpdateProject(projectId:ProjectId, refresh) =
        projectTable.[projectId] <- (refresh false, refresh)
        refreshInfoForProjectsThatReferenceThisProject(projectId)

    /// Clear a project from the project table
    member this.ClearInfoForProject(projectId:ProjectId) =
        projectTable.TryRemove(projectId) |> ignore
        refreshInfoForProjectsThatReferenceThisProject projectId

    /// Get the options for a project
    member this.TryGetOptionsForProject(projectId:ProjectId) =
        match projectTable.TryGetValue(projectId) with
        | true, ((_referencedProjects, parsingOptions, site, projectOptions), _) -> Some (parsingOptions, site, projectOptions)
        | _ -> None

    /// Given a projectId return the most recent set of command line options for it
    member __.GetCommandLineOptionsWithProjectId(projectId:ProjectId) =
        match commandLineOptions.TryGetValue projectId with
        | true, (sources, references, options) -> sources, references, options
        | _ -> [||], [||], [||]

    /// Store the command line options for a projectId
    member this.SetOptionsWithProjectId(projectId:ProjectId, sourcePaths:string[], referencePaths:string[], options:string[]) =
        commandLineOptions.[projectId] <- (sourcePaths, referencePaths, options)


let internal provideProjectSiteProvider(workspace:VisualStudioWorkspaceImpl, project:Project, serviceProvider:System.IServiceProvider, projectOptionsTable:FSharpProjectOptionsTable option) =
    let hier = workspace.GetHierarchy(project.Id)
    let getCommandLineOptionsWithProjectId (projectId) =
        match projectOptionsTable with
        | Some (options) -> options.GetCommandLineOptionsWithProjectId(projectId) 
        | None -> [||], [||], [||]
    {
        new IProvideProjectSite with
            member x.GetProjectSite() =
                let fst (a, _, _) = a
                let snd (_, b, _) = b
                let mutable errorReporter = 
                    let reporter = ProjectExternalErrorReporter(project.Id, "FS", serviceProvider)
                    Some(reporter:> IVsLanguageServiceBuildErrorReporter2)

                {
                    new IProjectSite with
                        member __.Description = project.Name
                        member __.CompilationSourceFiles = getCommandLineOptionsWithProjectId(project.Id) |> fst
                        member __.CompilationOptions =
                            let _,references,options = getCommandLineOptionsWithProjectId(project.Id)
                            Array.concat [options; references |> Array.map(fun r -> "-r:" + r)]
                        member __.CompilationReferences = getCommandLineOptionsWithProjectId(project.Id) |> snd
                        member site.CompilationBinOutputPath = site.CompilationOptions |> Array.tryPick (fun s -> if s.StartsWith("-o:") then Some s.[3..] else None)
                        member __.ProjectFileName = project.FilePath
                        member __.AdviseProjectSiteChanges(_,_) = ()
                        member __.AdviseProjectSiteCleaned(_,_) = ()
                        member __.AdviseProjectSiteClosed(_,_) = ()
                        member __.IsIncompleteTypeCheckEnvironment = false
                        member __.TargetFrameworkMoniker = ""
                        member __.ProjectGuid =  project.Id.Id.ToString()
                        member __.LoadTime = System.DateTime.Now
                        member __.ProjectProvider = Some (x)
                        member __.BuildErrorReporter with get () = errorReporter and set (v) = errorReporter <- v
                }
        interface IVsHierarchy with
            member __.SetSite(psp)                                    = hier.SetSite(psp)
            member __.GetSite(psp)                                    = hier.GetSite(ref psp)
            member __.QueryClose(pfCanClose)                          = hier.QueryClose(ref pfCanClose)
            member __.Close()                                         = hier.Close()
            member __.GetGuidProperty(itemid, propid, pguid)          = hier.GetGuidProperty(itemid, propid, ref pguid)
            member __.SetGuidProperty(itemid, propid, rguid)          = hier.SetGuidProperty(itemid, propid, ref rguid)
            member __.GetProperty(itemid, propid, pvar)               = hier.GetProperty(itemid, propid, ref pvar) 
            member __.SetProperty(itemid, propid, var)                = hier.SetProperty(itemid, propid, var)
            member __.GetNestedHierarchy(itemid, iidHierarchyNested, ppHierarchyNested, pitemidNested) = 
                                                                        hier.GetNestedHierarchy(itemid, ref iidHierarchyNested, 
                                                                                                ref ppHierarchyNested, ref pitemidNested)
            member __.GetCanonicalName(itemid, pbstrName)             = hier.GetCanonicalName(itemid, ref pbstrName)
            member __.ParseCanonicalName(pszName, pitemid)            = hier.ParseCanonicalName(pszName, ref pitemid)
            member __.Unused0()                                       = hier.Unused0()
            member __.AdviseHierarchyEvents(pEventSink, pdwCookie)    = hier.AdviseHierarchyEvents(pEventSink, ref pdwCookie)
            member __.UnadviseHierarchyEvents(dwCookie)               = hier.UnadviseHierarchyEvents(dwCookie)
            member __.Unused1()                                       = hier.Unused1()
            member __.Unused2()                                       = hier.Unused2()
            member __.Unused3()                                       = hier.Unused3()
            member __.Unused4()                                       = hier.Unused4()
    }

/// Information about projects, open files and other active artifacts in visual studio.
/// Keeps track of the relationship between IVsTextLines buffers, IFSharpSource_DEPRECATED objects, IProjectSite objects and FSharpProjectOptions
[<Sealed>]
type internal ProjectSitesAndFiles() =

    static let sourceUserDataGuid = new Guid("{55F834FD-B950-4C61-BBAA-0511ABAF4AE2}") // Guid for source user data on text buffer
    static let mutable stamp = 0L
    static let tryGetProjectSite(hierarchy:IVsHierarchy) =
        match hierarchy with
        | :? IProvideProjectSite as siteFactory -> Some(siteFactory.GetProjectSite())
        | _ -> None

    static let fullOutputAssemblyPath (p:EnvDTE.Project) =
        let getProperty tag =
            try Some (p.Properties.[tag].Value.ToString()) with _ -> None
        getProperty "FullPath"
        |> Option.bind (fun fullPath ->
            (try Some (p.ConfigurationManager.ActiveConfiguration.Properties.["OutputPath"].Value.ToString()) with _ -> None)
            |> Option.bind (fun outputPath -> 
                getProperty "OutputFileName"
                |> Option.map (fun outputFileName -> Path.Combine(fullPath, outputPath, outputFileName))))
        |> Option.bind (fun path -> try Some (Path.GetFullPath path) with _ -> None)

    static let referencedProjects (projectSite:IProjectSite) =
        match projectSite.ProjectProvider with
        | None -> None
        | Some (:? IVsHierarchy as hier) ->
            match hier.GetProperty(VSConstants.VSITEMID_ROOT, int __VSHPROPID.VSHPROPID_ExtObject) with
            | VSConstants.S_OK, (:? EnvDTE.Project as p) when not (isNull p) ->
                Some ((p.Object :?> VSLangProj.VSProject).References
                        |> Seq.cast<VSLangProj.Reference>
                        |> Seq.choose (fun r ->
                            Option.ofObj r
                            |> Option.bind (fun r -> try Option.ofObj r.SourceProject with _ -> None)) )
            | _ -> None
        | Some _ -> None

    static let rec referencedProvideProjectSites(projectSite:IProjectSite, serviceProvider:System.IServiceProvider, extraProjectInfo:obj option, projectOptionsTable:FSharpProjectOptionsTable option) =
        let getReferencesForSolutionService (solutionService:IVsSolution) =
            [|
                match referencedProjects projectSite, extraProjectInfo with
                | None, Some (:? VisualStudioWorkspaceImpl as workspace) when not (isNull workspace.CurrentSolution)->
                    let path = projectSite.ProjectFileName
                    if not (String.IsNullOrWhiteSpace(path)) then
                        let projectId = workspace.ProjectTracker.GetOrCreateProjectIdForPath(path, projectDisplayNameOf path)
                        let project = workspace.CurrentSolution.GetProject(projectId)
                        if not (isNull project) then
                            for reference in project.ProjectReferences do
                                let project = workspace.CurrentSolution.GetProject(reference.ProjectId)
                                if not (isNull project) && project.Language = LanguageServiceConstants.FSharpLanguageName then
                                    let siteProvider = provideProjectSiteProvider (workspace, project, serviceProvider, projectOptionsTable)
                                    let referenceProject = workspace.ProjectTracker.GetProject(reference.ProjectId)
                                    let outputPath = referenceProject.BinOutputPath
                                    yield Some projectId, project.FilePath, outputPath, siteProvider

                | (Some references), _ ->
                    for p in references do
                        match solutionService.GetProjectOfUniqueName(p.UniqueName) with
                        | VSConstants.S_OK, (:? IProvideProjectSite as ps) ->
                            yield None, p.FileName,  (fullOutputAssemblyPath p) |> Option.defaultValue "", ps
                        | _ -> ()
                | None, _ -> ()
            |]
        let solutionService = try Some (serviceProvider.GetService(typeof<SVsSolution>) :?> IVsSolution) with _ -> None
        seq { match solutionService with
              | Some solutionService ->
                  for reference in getReferencesForSolutionService solutionService do
                    yield reference
              | None -> ()
            }

    static let rec referencedProjectsOf(enableInMemoryCrossProjectReferences, tryGetOptionsForReferencedProject, projectSite, serviceProvider, extraProjectInfo, projectOptionsTable, useUniqueStamp) =
        [| for (projectId, projectFileName, outputPath, projectSiteProvider) in referencedProvideProjectSites (projectSite, serviceProvider, extraProjectInfo, projectOptionsTable) do
               let referencedProjectOptions =
                   // Lookup may not succeed if the project has not been established yet
                   // In this case we go and compute the options recursively.
                   match tryGetOptionsForReferencedProject projectFileName with 
                   | None -> getProjectOptionsForProjectSite (enableInMemoryCrossProjectReferences, tryGetOptionsForReferencedProject, projectSiteProvider.GetProjectSite(), serviceProvider,  projectId, projectFileName, extraProjectInfo, projectOptionsTable, useUniqueStamp) |> snd
                   | Some options -> options
               yield projectFileName, (outputPath, referencedProjectOptions) |]

    and getProjectOptionsForProjectSite(enableInMemoryCrossProjectReferences, tryGetOptionsForReferencedProject, projectSite, serviceProvider, projectId, fileName, extraProjectInfo, projectOptionsTable,  useUniqueStamp) =
        let referencedProjectFileNames, referencedProjectOptions = 
            if enableInMemoryCrossProjectReferences then
                referencedProjectsOf(enableInMemoryCrossProjectReferences, tryGetOptionsForReferencedProject, projectSite, serviceProvider, extraProjectInfo, projectOptionsTable, useUniqueStamp)
                |> Array.unzip
            else [| |], [| |]
        let option =
            let newOption () = {
                ProjectFileName = projectSite.ProjectFileName
                SourceFiles = projectSite.CompilationSourceFiles
                OtherOptions = projectSite.CompilationOptions
                ReferencedProjects = referencedProjectOptions
                IsIncompleteTypeCheckEnvironment = projectSite.IsIncompleteTypeCheckEnvironment
                UseScriptResolutionRules = SourceFile.MustBeSingleFileProject fileName
                LoadTime = projectSite.LoadTime
                UnresolvedReferences = None
                OriginalLoadReferences = []
                ExtraProjectInfo=extraProjectInfo 
                Stamp = if useUniqueStamp then (stamp <- stamp + 1L; Some stamp) else None 
            }
            match projectId, projectOptionsTable with
            | Some id, Some optionsTable ->
                // Get options from cache
                match optionsTable.TryGetOptionsForProject(id) with
                | Some (_parsingOptions, _site, projectOptions) ->
                    if projectSite.CompilationSourceFiles <> projectOptions.SourceFiles ||
                       projectSite.CompilationOptions <> projectOptions.OtherOptions ||
                       referencedProjectOptions <> projectOptions.ReferencedProjects then
                            newOption()
                    else
                            projectOptions
                | _ ->  newOption()
            | _ -> newOption()
        referencedProjectFileNames, option

    /// Construct a project site for a single file. May be a single file project (for scripts) or an orphan project site (for everything else).
    static member ProjectSiteOfSingleFile(filename:string) : IProjectSite = 
        if SourceFile.MustBeSingleFileProject(filename) then 
            Debug.Assert(false, ".fsx or .fsscript should have been treated as implicit project")
            failwith ".fsx or .fsscript should have been treated as implicit project"
        new ProjectSiteOfSingleFile(filename) :> IProjectSite

    static member GetReferencedProjectSites(projectSite:IProjectSite, serviceProvider:System.IServiceProvider, extraProjectInfo, projectOptions) =
        referencedProvideProjectSites (projectSite, serviceProvider, extraProjectInfo, projectOptions)
        |> Seq.map (fun (_, _, _, ps) -> ps.GetProjectSite())
        |> Seq.toArray

    member art.SetSource_DEPRECATED(buffer:IVsTextLines, source:IFSharpSource_DEPRECATED) : unit =
        let mutable guid = sourceUserDataGuid
        (buffer :?> IVsUserData).SetData(&guid, source) |> ErrorHandler.ThrowOnFailure |> ignore

    /// Create project options for this project site.
    static member GetProjectOptionsForProjectSite(enableInMemoryCrossProjectReferences, tryGetOptionsForReferencedProject, projectSite:IProjectSite, serviceProvider, projectId, filename, extraProjectInfo, projectOptionsTable, useUniqueStamp) =
        match projectSite with
        | :? IHaveCheckOptions as hco -> hco.OriginalCheckOptions()
        | _ -> getProjectOptionsForProjectSite(enableInMemoryCrossProjectReferences, tryGetOptionsForReferencedProject, projectSite, serviceProvider, projectId, filename, extraProjectInfo, projectOptionsTable, useUniqueStamp)

    /// Create project site for these project options
    static member CreateProjectSiteForScript (filename, referencedProjectFileNames, checkOptions) = 
        ProjectSiteOfScriptFile (filename, referencedProjectFileNames, checkOptions) :> IProjectSite

    member art.TryGetSourceOfFile_DEPRECATED(rdt:IVsRunningDocumentTable, filename:string) : IFSharpSource_DEPRECATED option =
        match VsRunningDocumentTable.FindDocumentWithoutLocking(rdt,filename) with 
        | Some(_hier, textLines) ->
            match textLines with
            | null -> None
            | _ ->
                let mutable guid = sourceUserDataGuid
                let mutable result = null
                (textLines :?> IVsUserData).GetData(&guid, &result) |> ignore
                match result with
                |   null -> None
                |   source -> Some(source :?> IFSharpSource_DEPRECATED)
                
        | None -> None                


    member art.GetDefinesForFile_DEPRECATED(rdt:IVsRunningDocumentTable, filename : string, checker:FSharpChecker) =
        // The only caller of this function calls it each time it needs to colorize a line, so this call must execute very fast.  
        if SourceFile.MustBeSingleFileProject(filename) then 
            CompilerEnvironment.GetCompilationDefinesForEditing(filename,FSharpParsingOptions.Default)
        else 
            let siteOpt = 
                match VsRunningDocumentTable.FindDocumentWithoutLocking(rdt,filename) with 
                | Some(hier,_) -> tryGetProjectSite(hier) 
                | None -> None

            let site = 
               match siteOpt with
               | Some site -> site
               | None -> ProjectSitesAndFiles.ProjectSiteOfSingleFile(filename)

            let parsingOptions,_ = checker.GetParsingOptionsFromCommandLineArgs(site.CompilationOptions |> Array.toList)
            CompilerEnvironment.GetCompilationDefinesForEditing(filename,parsingOptions)

    member art.TryFindOwningProject_DEPRECATED(rdt:IVsRunningDocumentTable, filename) = 
        if SourceFile.MustBeSingleFileProject(filename) then None
        else
            match VsRunningDocumentTable.FindDocumentWithoutLocking(rdt,filename) with 
            | Some(hier, _textLines) ->
                match tryGetProjectSite(hier) with
                | Some(site) -> 
                    if site.CompilationSourceFiles |> Array.exists (fun src -> StringComparer.OrdinalIgnoreCase.Equals(src,filename)) then
                        Some site
                    else
                        None
                | None -> None
            | None -> None
                        

    member art.FindOwningProject_DEPRECATED(rdt:IVsRunningDocumentTable, filename) = 
        match art.TryFindOwningProject_DEPRECATED(rdt, filename) with
        | Some site -> site
        | None -> ProjectSitesAndFiles.ProjectSiteOfSingleFile(filename)