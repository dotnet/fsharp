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
open FSharp.Compiler
open FSharp.Compiler.CodeAnalysis

open Microsoft.CodeAnalysis
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem
open Microsoft.VisualStudio.LanguageServices.Implementation.TaskList
open Microsoft.VisualStudio.FSharp.LanguageService

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
type private ProjectSiteOfScriptFile(fileName:string, referencedProjectFileNames, checkOptions: FSharpProjectOptions) = 
    interface IProjectSite with
        override this.Description = sprintf "Script Closure at Root %s" fileName
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

    override x.ToString() = sprintf "ProjectSiteOfScriptFile(%s)" fileName

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

    static let rec referencedProvideProjectSites(projectSite:IProjectSite, serviceProvider:System.IServiceProvider) =
        let getReferencesForSolutionService (solutionService:IVsSolution) =
            [|
                match referencedProjects projectSite, None with

                | (Some references), _ ->
                    for p in references do
                        match solutionService.GetProjectOfUniqueName(p.UniqueName) with
                        | VSConstants.S_OK, (:? IProvideProjectSite as ps) ->
                            yield p.FileName,  (fullOutputAssemblyPath p) |> Option.defaultValue "", ps
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

    static let rec referencedProjectsOf(enableInMemoryCrossProjectReferences, tryGetOptionsForReferencedProject, projectSite, serviceProvider, useUniqueStamp) =
        [| for (projectFileName, outputPath, projectSiteProvider) in referencedProvideProjectSites (projectSite, serviceProvider) do
               let referencedProjectOptions =
                   // Lookup may not succeed if the project has not been established yet
                   // In this case we go and compute the options recursively.
                   match tryGetOptionsForReferencedProject projectFileName with 
                   | None -> getProjectOptionsForProjectSite (enableInMemoryCrossProjectReferences, tryGetOptionsForReferencedProject, projectSiteProvider.GetProjectSite(), serviceProvider, projectFileName, useUniqueStamp) |> snd
                   | Some options -> options
               yield projectFileName, FSharpReferencedProject.CreateFSharp(outputPath, referencedProjectOptions) |]

    and getProjectOptionsForProjectSite(enableInMemoryCrossProjectReferences, tryGetOptionsForReferencedProject, projectSite, serviceProvider, fileName, useUniqueStamp) =
        let referencedProjectFileNames, referencedProjectOptions = 
            if enableInMemoryCrossProjectReferences then
                referencedProjectsOf(enableInMemoryCrossProjectReferences, tryGetOptionsForReferencedProject, projectSite, serviceProvider, useUniqueStamp)
                |> Array.unzip
            else [| |], [| |]
        let option =
            {
                ProjectFileName = projectSite.ProjectFileName
                ProjectId = None
                SourceFiles = projectSite.CompilationSourceFiles
                OtherOptions = projectSite.CompilationOptions
                ReferencedProjects = referencedProjectOptions
                IsIncompleteTypeCheckEnvironment = projectSite.IsIncompleteTypeCheckEnvironment
                UseScriptResolutionRules = CompilerEnvironment.MustBeSingleFileProject fileName
                LoadTime = projectSite.LoadTime
                UnresolvedReferences = None
                OriginalLoadReferences = []
                Stamp = if useUniqueStamp then (stamp <- stamp + 1L; Some stamp) else None 
            }
        referencedProjectFileNames, option

    /// Construct a project site for a single file. May be a single file project (for scripts) or an orphan project site (for everything else).
    static member ProjectSiteOfSingleFile(fileName:string) : IProjectSite = 
        if CompilerEnvironment.MustBeSingleFileProject(fileName) then 
            Debug.Assert(false, ".fsx or .fsscript should have been treated as implicit project")
            failwith ".fsx or .fsscript should have been treated as implicit project"
        new ProjectSiteOfSingleFile(fileName) :> IProjectSite

    member art.SetSource_DEPRECATED(buffer:IVsTextLines, source:IFSharpSource_DEPRECATED) : unit =
        let mutable guid = sourceUserDataGuid
        (buffer :?> IVsUserData).SetData(&guid, source) |> ErrorHandler.ThrowOnFailure |> ignore

    /// Create project options for this project site.
    static member GetProjectOptionsForProjectSite(enableInMemoryCrossProjectReferences, tryGetOptionsForReferencedProject, projectSite:IProjectSite, serviceProvider, fileName, useUniqueStamp) =
        match projectSite with
        | :? IHaveCheckOptions as hco -> hco.OriginalCheckOptions()
        | _ -> getProjectOptionsForProjectSite(enableInMemoryCrossProjectReferences, tryGetOptionsForReferencedProject, projectSite, serviceProvider, fileName, useUniqueStamp)

    /// Create project site for these project options
    static member CreateProjectSiteForScript (fileName, referencedProjectFileNames, checkOptions) = 
        ProjectSiteOfScriptFile (fileName, referencedProjectFileNames, checkOptions) :> IProjectSite

    member art.TryGetSourceOfFile_DEPRECATED(rdt:IVsRunningDocumentTable, fileName:string) : IFSharpSource_DEPRECATED option =
        match VsRunningDocumentTable.FindDocumentWithoutLocking(rdt,fileName) with 
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


    member art.GetDefinesForFile_DEPRECATED(rdt:IVsRunningDocumentTable, fileName : string, checker:FSharpChecker) =
        // The only caller of this function calls it each time it needs to colorize a line, so this call must execute very fast.  
        if CompilerEnvironment.MustBeSingleFileProject(fileName) then
            let parsingOptions = { FSharpParsingOptions.Default with IsInteractive = true}
            CompilerEnvironment.GetConditionalDefinesForEditing parsingOptions
        else 
            let siteOpt = 
                match VsRunningDocumentTable.FindDocumentWithoutLocking(rdt,fileName) with 
                | Some(hier,_) -> tryGetProjectSite(hier) 
                | None -> None

            let site = 
               match siteOpt with
               | Some site -> site
               | None -> ProjectSitesAndFiles.ProjectSiteOfSingleFile(fileName)

            let parsingOptions,_ = checker.GetParsingOptionsFromCommandLineArgs(site.CompilationOptions |> Array.toList)
            CompilerEnvironment.GetConditionalDefinesForEditing parsingOptions

    member art.TryFindOwningProject_DEPRECATED(rdt:IVsRunningDocumentTable, fileName) = 
        if CompilerEnvironment.MustBeSingleFileProject(fileName) then None
        else
            match VsRunningDocumentTable.FindDocumentWithoutLocking(rdt,fileName) with 
            | Some(hier, _textLines) ->
                match tryGetProjectSite(hier) with
                | Some(site) -> 
                    if site.CompilationSourceFiles |> Array.exists (fun src -> StringComparer.OrdinalIgnoreCase.Equals(src,fileName)) then
                        Some site
                    else
                        None
                | None -> None
            | None -> None
                        

    member art.FindOwningProject_DEPRECATED(rdt:IVsRunningDocumentTable, fileName) = 
        match art.TryFindOwningProject_DEPRECATED(rdt, fileName) with
        | Some site -> site
        | None -> ProjectSitesAndFiles.ProjectSiteOfSingleFile(fileName)
