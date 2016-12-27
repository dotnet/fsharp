// Copyright (c) Microsoft Corporation.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

(* Project handling. *)
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
    abstract OriginalCheckOptions : unit -> FSharpProjectOptions
        
/// Convert from FSharpProjectOptions into IProjectSite.         
type private ProjectSiteOfScriptFile(filename:string, checkOptions : FSharpProjectOptions) = 
    interface IProjectSite with
        override this.SourceFilesOnDisk() = checkOptions.ProjectFileNames
        override this.DescriptionOfProject() = sprintf "Script Closure at Root %s" filename
        override this.CompilerFlags() = checkOptions.OtherOptions
        override this.ProjectFileName() = checkOptions.ProjectFileName
        override this.ErrorListTaskProvider() = None
        override this.ErrorListTaskReporter() = None
        override this.AdviseProjectSiteChanges(_,_) = ()
        override this.AdviseProjectSiteCleaned(_,_) = ()
        override this.AdviseProjectSiteClosed(_,_) = ()
        override this.IsIncompleteTypeCheckEnvironment = checkOptions.IsIncompleteTypeCheckEnvironment
        override this.TargetFrameworkMoniker = ""
        override this.ProjectGuid = ""
        override this.LoadTime = checkOptions.LoadTime
        override this.ProjectProvider = None

    interface IHaveCheckOptions with
        override this.OriginalCheckOptions() = checkOptions
        
        
/// An orphan file project is a .fs, .ml, .fsi, .mli that is not associated with a .fsproj.
/// By design, these are never going to typecheck because there is no affiliated references.
/// We show many squiggles in this case because they're not particularly informational. 
type private ProjectSiteOfSingleFile(sourceFile) =         
    // CompilerFlags() gets called a lot, so pre-compute what we can
    static let compilerFlags = 
        let flags = ["--noframework";"--warn:3"]
        let assumeDotNetFramework = true
        let defaultReferences = CompilerEnvironment.DefaultReferencesForOrphanSources(assumeDotNetFramework)
                                |> List.map(fun r->sprintf "-r:%s.dll" r)
        (flags @ defaultReferences) |> List.toArray

    let projectFileName = sourceFile + ".orphan.fsproj"

    interface IProjectSite with
        override this.SourceFilesOnDisk() = [|sourceFile|]
        override this.DescriptionOfProject() = "Orphan File Project"
        override this.CompilerFlags() = compilerFlags
        override this.ProjectFileName() = projectFileName                
        override this.ErrorListTaskProvider() = None
        override this.ErrorListTaskReporter() = None
        override this.AdviseProjectSiteChanges(_,_) = ()
        override this.AdviseProjectSiteCleaned(_,_) = ()
        override this.AdviseProjectSiteClosed(_,_) = ()
        override this.IsIncompleteTypeCheckEnvironment = true
        override this.TargetFrameworkMoniker = ""
        override this.ProjectGuid = ""
        override this.LoadTime = new DateTime(2000,1,1)  // any constant time is fine, orphan files do not interact with reloading based on update time
        override this.ProjectProvider = None
    
/// Information about projects, open files and other active artifacts in visual studio.
/// Keeps track of the relationship between IVsTextLines buffers, IFSharpSource objects, IProjectSite objects and FSharpProjectOptions
[<Sealed>]
type internal ProjectSitesAndFiles() =
    static let sourceUserDataGuid = new Guid("{55F834FD-B950-4C61-BBAA-0511ABAF4AE2}") // Guid for source user data on text buffer
    
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
        let solutionService = 
            try Some (serviceProvider.GetService(typeof<SVsSolution>) :?> IVsSolution) with _ -> None
        match solutionService with
        | Some solutionService ->
            referencedProjects projectSite
            |> Seq.choose (fun p ->
                match solutionService.GetProjectOfUniqueName(p.UniqueName) with
                | VSConstants.S_OK, (:? IProvideProjectSite as ps) ->
                    Some (p, ps)
                | _ -> None)
        | None -> Seq.empty
                    
    static let rec referencedProjectsOf (projectSite:IProjectSite, fileName, extraProjectInfo, serviceProvider:System.IServiceProvider) =
        referencedProvideProjectSites (projectSite, serviceProvider)
        |> Seq.choose (fun (p, ps) ->            
            fullOutputAssemblyPath p
            |> Option.map (fun path ->
                path, getProjectOptionsForProjectSite (ps.GetProjectSite(), fileName, extraProjectInfo, serviceProvider))
            )
        |> Seq.toArray

    and getProjectOptionsForProjectSite(projectSite:IProjectSite, fileName, extraProjectInfo, serviceProvider) =            
           {ProjectFileName = projectSite.ProjectFileName()
            ProjectFileNames = projectSite.SourceFilesOnDisk()
            OtherOptions = projectSite.CompilerFlags()
            ReferencedProjects = referencedProjectsOf(projectSite, fileName, extraProjectInfo, serviceProvider)
            IsIncompleteTypeCheckEnvironment = projectSite.IsIncompleteTypeCheckEnvironment
            UseScriptResolutionRules = SourceFile.MustBeSingleFileProject fileName
            LoadTime = projectSite.LoadTime
            UnresolvedReferences = None
            OriginalLoadReferences = []
            ExtraProjectInfo=extraProjectInfo }   

    /// Construct a project site for a single file. May be a single file project (for scripts) or an orphan project site (for everything else).
    static member ProjectSiteOfSingleFile(filename:string) : IProjectSite = 
        if SourceFile.MustBeSingleFileProject(filename) then 
            Debug.Assert(false, ".fsx or .fsscript should have been treated as implicit project")
            failwith ".fsx or .fsscript should have been treated as implicit project"

        new ProjectSiteOfSingleFile(filename) :> IProjectSite
    
    member art.SetSource(buffer:IVsTextLines, source:IFSharpSource) : unit =
        let mutable guid = sourceUserDataGuid
        (buffer :?> IVsUserData).SetData(&guid, source) |> ErrorHandler.ThrowOnFailure |> ignore

    member art.UnsetSource(buffer:IVsTextLines) : unit =
        let mutable guid = sourceUserDataGuid
        (buffer :?> IVsUserData).SetData(&guid, null) |> ErrorHandler.ThrowOnFailure |> ignore
        
    /// Given a filename get the corresponding Source
    member art.TryGetSourceOfFile(rdt:IVsRunningDocumentTable, filename:string) : IFSharpSource option =
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
                |   source -> Some(source :?> IFSharpSource)
                
        | None -> None                

    /// Get the list of Defines for a given buffer
    member art.GetDefinesForFile(rdt:IVsRunningDocumentTable, filename : string) =
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

    member art.TryFindOwningProject(rdt:IVsRunningDocumentTable, filename) = 
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
                        
    /// Find the project that "owns" this filename.  That is,
    ///  - if the file is associated with an F# IVsHierarchy in the RDT, and
    ///  - the .fsproj has this file in its list of <Compile> items,
    /// then the project is considered the 'owner'.  Otherwise a 'single file project' is returned.
    member art.FindOwningProject(rdt:IVsRunningDocumentTable, filename) = 
        match art.TryFindOwningProject(rdt, filename) with
        | Some site -> site
        | None -> ProjectSitesAndFiles.ProjectSiteOfSingleFile(filename)      
        
    static member GetReferencedProjectSites(projectSite:IProjectSite, serviceProvider:System.IServiceProvider) =
        referencedProvideProjectSites (projectSite, serviceProvider)
        |> Seq.map (fun (_, ps) -> ps.GetProjectSite())
        |> Seq.toArray

    /// Create project options for this project site.
    static member GetProjectOptionsForProjectSite(projectSite:IProjectSite,filename,extraProjectInfo,serviceProvider:System.IServiceProvider) =
        match projectSite with
        | :? IHaveCheckOptions as hco -> hco.OriginalCheckOptions()
        | _ ->             
            getProjectOptionsForProjectSite(projectSite, filename, extraProjectInfo, serviceProvider)
         
    /// Create project site for these project options
    static member CreateProjectSiteForScript (filename, checkOptions) = ProjectSiteOfScriptFile (filename, checkOptions) :> IProjectSite

