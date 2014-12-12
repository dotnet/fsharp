// Copyright (c) Microsoft Open Technologies, Inc.  All Rights Reserved.  Licensed under the Apache License, Version 2.0.  See License.txt in the project root for license information.

(* Project handling. *)
namespace Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.VisualStudio
open Microsoft.VisualStudio.TextManager.Interop
open Microsoft.VisualStudio.Shell
open Microsoft.VisualStudio.Shell.Interop
open Microsoft.VisualStudio.FSharp.LanguageService
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.FSharp.Compiler.AbstractIL.Diagnostics 
open Internal.Utilities.Collections
open Internal.Utilities.Debug
open System
open System.IO
open System.Diagnostics
open System.Collections.Generic

module internal ProjectSiteOptions =    

    /// Roundtrip CheckOptions when we know them so we don't have to recreate.
    type IHaveCheckOptions = 
        abstract OriginalCheckOptions : unit -> CheckOptions
        
    /// Create project options for this project site.
    let Create(projectSite:IProjectSite,filename)= 
        use t = Trace.Call("ProjectSite","ProjectSiteOptions::Create", fun _->sprintf " projectSite=%s flags=%A" (projectSite.DescriptionOfProject()) (projectSite.CompilerFlags()))
        
        match projectSite with
        | :? IHaveCheckOptions as hco -> hco.OriginalCheckOptions()
        | _ -> 
            {ProjectFileName = projectSite.ProjectFileName()
             ProjectFileNames = projectSite.SourceFilesOnDisk()
             ProjectOptions = projectSite.CompilerFlags()
             IsIncompleteTypeCheckEnvironment = not (projectSite.IsTypeResolutionValid)
             UseScriptResolutionRules = SourceFile.MustBeSingleFileProject(filename)
             LoadTime = projectSite.LoadTime
             UnresolvedReferences = None }      
         
    /// Convert from CheckOptions into IProjectSite.         
    let ToProjectSite(filename:string, checkOptions : CheckOptions)= 
        { new IProjectSite with
            override this.SourceFilesOnDisk() = checkOptions.ProjectFileNames
            override this.DescriptionOfProject() = sprintf "Script Closure at Root %s" filename
            override this.CompilerFlags() = checkOptions.ProjectOptions
            override this.ProjectFileName() = checkOptions.ProjectFileName
            override this.ErrorListTaskProvider() = None
            override this.ErrorListTaskReporter() = None
            override this.AdviseProjectSiteChanges(_,_) = ()
            override this.AdviseProjectSiteCleaned(_,_) = ()
            override this.IsTypeResolutionValid = not(checkOptions.IsIncompleteTypeCheckEnvironment)
            override this.TargetFrameworkMoniker = ""
            override this.LoadTime = checkOptions.LoadTime
          interface IHaveCheckOptions with
            override this.OriginalCheckOptions() = checkOptions
        } 
        
        
/// An orphan file project is a .fs, .ml, .fsi, .mli that is not associated with a .fsproj.
/// By design, these are never going to typecheck because there is no affiliated references.
/// We show many squiggles in this case because they're not particularly informational. 
module OrphanFileProjectSite =         
    // CompilerFlags() gets called a lot, so pre-compute what we can
    let compilerFlags = 
        let flags = ["--noframework";"--warn:3"]
        let defaultReferences = CompilerEnvironment.DefaultReferencesForOrphanSources 
                                |> List.map(fun r->sprintf "-r:%s.dll" r)
        (flags @ defaultReferences) |> List.toArray
    let public Create(sourceFile,enableStandaloneFileIntellisense) = 
        let projectFileName = Path.Combine(System.IO.Path.GetDirectoryName(sourceFile),"orphan.fsproj")
        { new IProjectSite with
            override this.SourceFilesOnDisk() = [|sourceFile|]
            override this.DescriptionOfProject() = "Orphan File Project"
            override this.CompilerFlags() = compilerFlags
            override this.ProjectFileName() = projectFileName                
            override this.ErrorListTaskProvider() = None
            override this.ErrorListTaskReporter() = None
            override this.AdviseProjectSiteChanges(_,_) = ()
            override this.AdviseProjectSiteCleaned(_,_) = ()
            override this.IsTypeResolutionValid = not enableStandaloneFileIntellisense
            override this.TargetFrameworkMoniker = ""
            override this.LoadTime = new System.DateTime(2000,1,1)  // any constant time is fine, orphan files do not interact with reloading based on update time
        }
    
/// Information about projects, open files and other active artifacts in visual studio
[<Sealed>]
type internal Artifacts() =
    static let sourceUserDataGuid = new Guid("{55F834FD-B950-4C61-BBAA-0511ABAF4AE2}") // Guid for source user data on text buffer
    
    member art.TryGetProjectSite(hierarchy:IVsHierarchy) =
        match hierarchy with
        | :? IProvideProjectSite as siteFactory -> 
            Some(siteFactory.GetProjectSite())
        | _ -> None

    /// Construct a project site for a single file. May be a single file project (for scripts) or an orphan project site (for everything else).
    static member ProjectSiteOfSingleFile(filename:string,enableStandaloneFileIntellisense) : IProjectSite = 
        if SourceFile.MustBeSingleFileProject(filename) then 
            System.Diagnostics.Debug.Assert(false, ".fsx or .fsscript should have been treated as implicit project")
            failwith ".fsx or .fsscript should have been treated as implicit project"
        else OrphanFileProjectSite.Create(filename,enableStandaloneFileIntellisense)                                          
    
    member art.SetSource(buffer:IVsTextLines, source:IdealSource) : unit =
        let mutable guid = sourceUserDataGuid
        (buffer :?> IVsUserData).SetData(&guid, source) |> ErrorHandler.ThrowOnFailure |> ignore

    member art.UnsetSource(buffer:IVsTextLines) : unit =
        let mutable guid = sourceUserDataGuid
        (buffer :?> IVsUserData).SetData(&guid, null) |> ErrorHandler.ThrowOnFailure |> ignore
        
    /// Given a filename get the corresponding Source
    member art.GetSourceOfFilename(rdt:IVsRunningDocumentTable, filename:string) : IdealSource option =
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
                |   source -> Some(source :?> IdealSource)
                
        | None -> None                


    /// Get the list of Defines for a given buffer
    member art.GetDefinesForFile(rdt:IVsRunningDocumentTable, filename : string, enableStandaloneFileIntellisense) =
        // The only caller of this function calls it each time it needs to colorize a line, so this call must execute very fast.  
        if SourceFile.MustBeSingleFileProject(filename) then 
            CompilerEnvironment.GetCompilationDefinesForEditing(filename,[])
        else 
            let StandaloneSite() =  Artifacts.ProjectSiteOfSingleFile(filename,enableStandaloneFileIntellisense)
            let site = 
                match VsRunningDocumentTable.FindDocumentWithoutLocking(rdt,filename) with 
                | Some(hier,_) -> 
                    match art.TryGetProjectSite(hier) with 
                    | Some(site)->site
                    | None -> StandaloneSite()
                | None -> StandaloneSite()
            CompilerEnvironment.GetCompilationDefinesForEditing(filename,site.CompilerFlags() |> Array.toList)

    member art.TryFindOwningProject(rdt:IVsRunningDocumentTable, filename) = 
        if SourceFile.MustBeSingleFileProject(filename) then None
        else
            match VsRunningDocumentTable.FindDocumentWithoutLocking(rdt,filename) with 
            | Some(hier, _textLines) ->
                match art.TryGetProjectSite(hier) with
                | Some(site) -> 
#if DEBUG
                    site.SourceFilesOnDisk() |> Seq.iter (fun src -> 
                        Debug.Assert(System.IO.Path.GetFullPath(src) = src, "SourceFilesOnDisk reported a filename that was not in canonical format")
                    )
#endif
                    if site.SourceFilesOnDisk() |> Array.exists (fun src -> System.StringComparer.OrdinalIgnoreCase.Equals(src,filename)) then
                        Some site
                    else
                        None
                | None -> None
            | None -> None
                        
    /// Find the project that "owns" this filename.  That is,
    ///  - if the file is associated with an F# IVsHierarchy in the RDT, and
    ///  - the .fsproj has this file in its list of <Compile> items,
    /// then the project is considered the 'owner'.  Otherwise a 'single file project' is returned.
    member art.FindOwningProject(rdt:IVsRunningDocumentTable, filename,enableStandaloneFileIntellisense) = 
        let SiteOfSingleFile() = Artifacts.ProjectSiteOfSingleFile(filename,enableStandaloneFileIntellisense)        
        match art.TryFindOwningProject(rdt, filename) with
        | Some site -> site
        | None -> SiteOfSingleFile()