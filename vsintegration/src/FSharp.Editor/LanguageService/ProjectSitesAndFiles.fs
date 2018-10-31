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


