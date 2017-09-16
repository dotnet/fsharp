// Copyright (c) Microsoft Corporation.  All Rights Reserved.  See License.txt in the project root for license information.

// Project information handling.
//
// For the old F# project system, in a running Visual Studio, the **authoritative** view 
// of the project information is maintained by Project.fs in FSharp.ProjectSystem. This 
// information is conveyed to the rest of the implementation via an IProjectSite interface.
//
// For most purposes, an IProjectSite has to provide three main things
//   - the source files
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

namespace Microsoft.VisualStudio.FSharp.LanguageService

open System
open System.IO
open System.Diagnostics
open System.Runtime.InteropServices
open Microsoft.VisualStudio
open Microsoft.VisualStudio.TextManager.Interop
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.FSharp.Compiler.SourceCodeServices

/// An additional interface that an IProjectSite object can implement to indicate it has an FSharpProjectOptions 
/// already available, so we don't have to recreate it
type private IHaveCheckOptions = 
    abstract OriginalCheckOptions : unit -> string[] * FSharpProjectOptions
        
/// Convert from FSharpProjectOptions into IProjectSite.
type private ProjectSiteOfScriptFile(filename:string, referencedProjectFileNames, checkOptions : FSharpProjectOptions) = 
    interface IProjectSite with
        override this.SourceFilesOnDisk() = checkOptions.SourceFiles
        override this.DescriptionOfProject() = sprintf "Script Closure at Root %s" filename
        override this.CompilerFlags() = checkOptions.OtherOptions
        override this.ProjectFileName() = checkOptions.ProjectFileName
        override this.BuildErrorReporter with get() = None and set _v = ()
        override this.AdviseProjectSiteChanges(_,_) = ()
        override this.AdviseProjectSiteCleaned(_,_) = ()
        override this.AdviseProjectSiteClosed(_,_) = ()
        override this.IsIncompleteTypeCheckEnvironment = checkOptions.IsIncompleteTypeCheckEnvironment
        override this.TargetFrameworkMoniker = ""
        override this.ProjectGuid = ""
        override this.LoadTime = checkOptions.LoadTime
        override this.ProjectProvider = None
        override this.AssemblyReferences() = [||]

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
        (flags @ defaultReferences) |> List.toArray

    let projectFileName = sourceFile + ".orphan.fsproj"

    interface IProjectSite with
        override this.SourceFilesOnDisk() = [|sourceFile|]
        override this.DescriptionOfProject() = "Orphan File Project"
        override this.CompilerFlags() = compilerFlags
        override this.ProjectFileName() = projectFileName                
        override this.BuildErrorReporter with get() = None and set _v = ()
        override this.AdviseProjectSiteChanges(_,_) = ()
        override this.AdviseProjectSiteCleaned(_,_) = ()
        override this.AdviseProjectSiteClosed(_,_) = ()
        override this.IsIncompleteTypeCheckEnvironment = true
        override this.TargetFrameworkMoniker = ""
        override this.ProjectGuid = ""
        override this.LoadTime = new DateTime(2000,1,1)  // any constant time is fine, orphan files do not interact with reloading based on update time
        override this.ProjectProvider = None
        override this.AssemblyReferences() = [||]
        
    override x.ToString() = sprintf "ProjectSiteOfSingleFile(%s)" sourceFile
    
/// Information about projects, open files and other active artifacts in visual studio.
/// Keeps track of the relationship between IVsTextLines buffers, IFSharpSource_DEPRECATED objects, IProjectSite objects and FSharpProjectOptions
[<Sealed>]
type internal ProjectSitesAndFiles() =
    static let sourceUserDataGuid = new Guid("{55F834FD-B950-4C61-BBAA-0511ABAF4AE2}") // Guid for source user data on text buffer
    
    static let mutable stamp = 0L
    static let tryGetProjectSite(hierarchy:IVsHierarchy) =
        match hierarchy with
        | :? IProvideProjectSite as siteFactory -> 
            Some(siteFactory.GetProjectSite())
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
        | None -> Seq.empty
        | Some (:? IVsHierarchy as hier) ->                                
            match hier.GetProperty(VSConstants.VSITEMID_ROOT, int __VSHPROPID.VSHPROPID_ExtObject) with
            | VSConstants.S_OK, (:? EnvDTE.Project as p) ->
                (p.Object :?> VSLangProj.VSProject).References
                |> Seq.cast<VSLangProj.Reference>
                |> Seq.choose (fun r ->
                    Option.ofObj r
                    |> Option.bind (fun r -> try Option.ofObj r.SourceProject with _ -> None))            
            | _ -> Seq.empty
        | Some _ -> Seq.empty

    static let rec referencedProvideProjectSites (projectSite:IProjectSite, serviceProvider:System.IServiceProvider) =
        seq { 
            let solutionService = try Some (serviceProvider.GetService(typeof<SVsSolution>) :?> IVsSolution) with _ -> None
            match solutionService with
            | Some solutionService ->
                for p in referencedProjects projectSite do
                    match solutionService.GetProjectOfUniqueName(p.UniqueName) with
                    | VSConstants.S_OK, (:? IProvideProjectSite as ps) ->
                        yield (p, ps)
                    | _ -> ()
            | None -> ()
        }
                    
    static let rec referencedProjectsOf (enableInMemoryCrossProjectReferences, tryGetOptionsForReferencedProject, projectSite:IProjectSite, extraProjectInfo, serviceProvider:System.IServiceProvider, useUniqueStamp) =
        [| for (p,ps) in referencedProvideProjectSites (projectSite, serviceProvider) do
              match fullOutputAssemblyPath p with 
              | None -> ()
              | Some path ->
                  let referencedProjectOptions = 
                      // Lookup may not succeed if the project has not been established yet
                      // In this case we go and compute the options recursively.
                      match tryGetOptionsForReferencedProject p.FileName with 
                      | None -> getProjectOptionsForProjectSite (enableInMemoryCrossProjectReferences, tryGetOptionsForReferencedProject, ps.GetProjectSite(), p.FileName, extraProjectInfo, serviceProvider, useUniqueStamp) |> snd
                      | Some options -> options
                  yield (p.FileName, (path, referencedProjectOptions)) |]

    and getProjectOptionsForProjectSite(enableInMemoryCrossProjectReferences, tryGetOptionsForReferencedProject, projectSite:IProjectSite, fileName, extraProjectInfo, serviceProvider, useUniqueStamp) =            
        let referencedProjectFileNames, referencedProjectOptions = 
            if enableInMemoryCrossProjectReferences then
                referencedProjectsOf(enableInMemoryCrossProjectReferences, tryGetOptionsForReferencedProject, projectSite, extraProjectInfo, serviceProvider, useUniqueStamp) 
                |> Array.unzip
            else [| |], [| |]

        let options = 
            {ProjectFileName = projectSite.ProjectFileName()
             SourceFiles = projectSite.SourceFilesOnDisk()
             OtherOptions = projectSite.CompilerFlags()
             ReferencedProjects = referencedProjectOptions
             IsIncompleteTypeCheckEnvironment = projectSite.IsIncompleteTypeCheckEnvironment
             UseScriptResolutionRules = SourceFile.MustBeSingleFileProject fileName
             LoadTime = projectSite.LoadTime
             UnresolvedReferences = None
             OriginalLoadReferences = []
             ExtraProjectInfo=extraProjectInfo 
             Stamp = (if useUniqueStamp then (stamp <- stamp + 1L; Some stamp) else None) }   
        referencedProjectFileNames, options

    /// Construct a project site for a single file. May be a single file project (for scripts) or an orphan project site (for everything else).
    static member ProjectSiteOfSingleFile(filename:string) : IProjectSite = 
        if SourceFile.MustBeSingleFileProject(filename) then 
            Debug.Assert(false, ".fsx or .fsscript should have been treated as implicit project")
            failwith ".fsx or .fsscript should have been treated as implicit project"

        new ProjectSiteOfSingleFile(filename) :> IProjectSite
    
    member art.SetSource(buffer:IVsTextLines, source:IFSharpSource) : unit =
        let mutable guid = sourceUserDataGuid
        (buffer :?> IVsUserData).SetData(&guid, source) |> ErrorHandler.ThrowOnFailure |> ignore


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


    member art.GetDefinesForFile_DEPRECATED(rdt:IVsRunningDocumentTable, filename : string) =
        // The only caller of this function calls it each time it needs to colorize a line, so this call must execute very fast.  
        if SourceFile.MustBeSingleFileProject(filename) then 
            CompilerEnvironment.GetCompilationDefinesForEditing(filename,[])
        else 
            let siteOpt = 
                match VsRunningDocumentTable.FindDocumentWithoutLocking(rdt,filename) with 
                | Some(hier,_) -> tryGetProjectSite(hier) 
                | None -> None

            let site = 
               match siteOpt with
               | Some site -> site
               | None -> ProjectSitesAndFiles.ProjectSiteOfSingleFile(filename)

            CompilerEnvironment.GetCompilationDefinesForEditing(filename,site.CompilerFlags() |> Array.toList)


    member art.TryFindOwningProject_DEPRECATED(rdt:IVsRunningDocumentTable, filename) = 
        if SourceFile.MustBeSingleFileProject(filename) then None
        else
            match VsRunningDocumentTable.FindDocumentWithoutLocking(rdt,filename) with 
            | Some(hier, _textLines) ->
                match tryGetProjectSite(hier) with
                | Some(site) -> 
#if DEBUG
                    for src in site.SourceFilesOnDisk() do 
                        Debug.Assert(Path.GetFullPath(src) = src, "SourceFilesOnDisk reported a filename that was not in canonical format")
#endif
                    if site.SourceFilesOnDisk() |> Array.exists (fun src -> StringComparer.OrdinalIgnoreCase.Equals(src,filename)) then
                        Some site
                    else
                        None
                | None -> None
            | None -> None
                        

    member art.FindOwningProject_DEPRECATED(rdt:IVsRunningDocumentTable, filename) = 
        match art.TryFindOwningProject_DEPRECATED(rdt, filename) with
        | Some site -> site
        | None -> ProjectSitesAndFiles.ProjectSiteOfSingleFile(filename)      
