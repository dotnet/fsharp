// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

module internal rec Microsoft.VisualStudio.FSharp.Editor.SiteProvider

open System
open System.IO
open System.Collections.Concurrent
open System.Diagnostics

open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.CodeAnalysis
open Microsoft.VisualStudio
open Microsoft.VisualStudio.LanguageServices
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem
open Microsoft.VisualStudio.LanguageServices.Implementation.TaskList
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.TextManager.Interop

open VSLangProj

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
        override __.Description = sprintf "Script Closure at Root %s" filename
        override __.CompilationSourceFiles = checkOptions.SourceFiles
        override __.CompilationOptions = checkOptions.OtherOptions
        override __.CompilationReferences =
             checkOptions.OtherOptions 
             |> Array.choose (fun flag -> if flag.StartsWith("-r:") then Some flag.[3..] else None) 
        override __.CompilationBinOutputPath = None
        override __.ProjectFileName = checkOptions.ProjectFileName
        override __.BuildErrorReporter with get() = None and set _ = ()
        override __.AdviseProjectSiteChanges(_,_) = ()
        override __.AdviseProjectSiteCleaned(_,_) = ()
        override __.AdviseProjectSiteClosed(_,_) = ()
        override __.IsIncompleteTypeCheckEnvironment = checkOptions.IsIncompleteTypeCheckEnvironment
        override __.TargetFrameworkMoniker = ""
        override __.ProjectGuid = ""
        override __.LoadTime = checkOptions.LoadTime
        override __.ProjectProvider = None

    interface IHaveCheckOptions with
        override __.OriginalCheckOptions() = (referencedProjectFileNames, checkOptions)

    override __.ToString() = sprintf "ProjectSiteOfScriptFile(%s)" filename

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
        override __.Description = projectFileName
        override __.CompilationSourceFiles = [|sourceFile|]
        override __.CompilationOptions = compilerFlags
        override __.CompilationReferences = compilerFlags
        override __.CompilationBinOutputPath = None
        override __.ProjectFileName = projectFileName
        override __.BuildErrorReporter with get() = None and set _v = ()
        override __.AdviseProjectSiteChanges(_,_) = ()
        override __.AdviseProjectSiteCleaned(_,_) = ()
        override __.AdviseProjectSiteClosed(_,_) = ()
        override __.IsIncompleteTypeCheckEnvironment = true
        override __.TargetFrameworkMoniker = ""
        override __.ProjectGuid = ""
        override __.LoadTime = new DateTime(2000,1,1)  // any constant time is fine, orphan files do not interact with reloading based on update time
        override __.ProjectProvider = None

    override __.ToString() = sprintf "ProjectSiteOfSingleFile(%s)" sourceFile

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
    member __.ClearInfoForProject(projectId:ProjectId) =
        projectTable.TryRemove(projectId) |> ignore
        refreshInfoForProjectsThatReferenceThisProject projectId

    /// Get the options for a project
    member __.TryGetOptionsForProject(projectId:ProjectId) =
        match projectTable.TryGetValue(projectId) with
        | true, ((_referencedProjects, parsingOptions, site, projectOptions), _) -> Some (parsingOptions, site, projectOptions)
        | _ -> None

    /// Given a projectId return the most recent set of command line options for it
    member __.GetCommandLineOptionsWithProjectId(projectId:ProjectId) =
        match commandLineOptions.TryGetValue projectId with
        | true, (sources, references, options) -> sources, references, options
        | _ -> [||], [||], [||]

    /// Store the command line options for a projectId
    member __.SetOptionsWithProjectId(projectId:ProjectId, sourcePaths:string[], referencePaths:string[], options:string[]) =
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
            member __.SetSite(psp) = hier.SetSite(psp)
            member __.GetSite(psp) = hier.GetSite(ref psp)
            member __.QueryClose(pfCanClose)= hier.QueryClose(ref pfCanClose)
            member __.Close() = hier.Close()
            member __.GetGuidProperty(itemid, propid, pguid) = hier.GetGuidProperty(itemid, propid, ref pguid)
            member __.SetGuidProperty(itemid, propid, rguid) = hier.SetGuidProperty(itemid, propid, ref rguid)
            member __.GetProperty(itemid, propid, pvar) = hier.GetProperty(itemid, propid, ref pvar) 
            member __.SetProperty(itemid, propid, var)  = hier.SetProperty(itemid, propid, var)
            member __.GetNestedHierarchy(itemid, iidHierarchyNested, ppHierarchyNested, pitemidNested) = 
                hier.GetNestedHierarchy(itemid, ref iidHierarchyNested, ref ppHierarchyNested, ref pitemidNested)
            member __.GetCanonicalName(itemid, pbstrName) = hier.GetCanonicalName(itemid, ref pbstrName)
            member __.ParseCanonicalName(pszName, pitemid) = hier.ParseCanonicalName(pszName, ref pitemid)
            member __.Unused0() = hier.Unused0()
            member __.AdviseHierarchyEvents(pEventSink, pdwCookie) = hier.AdviseHierarchyEvents(pEventSink, ref pdwCookie)
            member __.UnadviseHierarchyEvents(dwCookie) = hier.UnadviseHierarchyEvents(dwCookie)
            member __.Unused1() = hier.Unused1()
            member __.Unused2() = hier.Unused2()
            member __.Unused3() = hier.Unused3()
            member __.Unused4() = hier.Unused4()
    }