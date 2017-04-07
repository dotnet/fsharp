namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Concurrent
open Microsoft.CodeAnalysis
open Microsoft.FSharp.Compiler.SourceCodeServices


module internal ProjectOptionsCache =
 
    let toFSharpProjectOptions (projectTable:ConcurrentDictionary<ProjectId, FSharpProjectOptions>)  (workspace: Workspace) (project:Project): FSharpProjectOptions =
        let loadTime = System.DateTime.Now
        let rec generate (project:Project) : FSharpProjectOptions =
            let getProjectRefs (project:Project): (string * FSharpProjectOptions)[] =
                project.ProjectReferences
                |> Seq.choose (fun pref -> workspace.CurrentSolution.TryGetProject pref.ProjectId)
                |> Seq.map (fun proj ->
                    if projectTable.ContainsKey proj.Id then
                        (proj.OutputFilePath, projectTable.[proj.Id])
                    else
                        let fsinfo = generate proj
                        projectTable.TryAdd (proj.Id, fsinfo) |> ignore
                        (proj.OutputFilePath, fsinfo)
                    )
                |> Array.ofSeq
            
            {   ProjectFileName = project.FilePath
                ProjectFileNames = project.Documents |> Seq.map (fun doc -> doc.FilePath) |> Array.ofSeq
                // TODO - This does not include all necessary compiler flags
                // if possible all of the the requisite compiler flags should be acquired from
                // the CodeAnalysis Project and converted into the FSC desired form
                OtherOptions = [||] // project.ParseOptions. PreprocessorSymbolNames |> Array.ofSeq
                ReferencedProjects =  getProjectRefs project
                IsIncompleteTypeCheckEnvironment = false
                UseScriptResolutionRules = false
                LoadTime = loadTime
                UnresolvedReferences = None
                OriginalLoadReferences = []
                ExtraProjectInfo = Some (workspace :> _)
            }
        let fsprojOptions = generate project
        fsprojOptions


    let singleFileProjectOptions (sourceFile:string) =
        let compilerFlags = 
            let flags = ["--noframework";"--warn:3"]
            let assumeDotNetFramework = true
            let defaultReferences = 
                CompilerEnvironment.DefaultReferencesForOrphanSources assumeDotNetFramework
                |> List.map (fun r->sprintf "-r:%s.dll" r)
            (flags @ defaultReferences) |> List.toArray
        let projectFileName = sourceFile + ".orphan.fsproj"
        {   ProjectFileName = projectFileName
            ProjectFileNames = [|sourceFile|]
            OtherOptions = compilerFlags
            ReferencedProjects =  [||]
            IsIncompleteTypeCheckEnvironment = true
            UseScriptResolutionRules = true
            LoadTime = new DateTime(2000,1,1)  // any constant time is fine, orphan files do not interact with reloading based on update time
            UnresolvedReferences = None
            OriginalLoadReferences = []
            ExtraProjectInfo = None
        }


    let fsxProjectOptions (sourceFile:string) (workspace:Workspace option) =
        let loadTime = System.DateTime.Now
        let workspace = workspace |> Option.map(fun w -> w :> obj)
        let compilerFlags = 
            let flags = ["--noframework";"--warn:3"]
            let assumeDotNetFramework = true
            let defaultReferences = 
                CompilerEnvironment.DefaultReferencesForOrphanSources(assumeDotNetFramework)
                |> List.map (fun r->sprintf "-r:%s.dll" r)
            (flags @ defaultReferences) |> List.toArray
        let projectFileName = sourceFile + ".orphan.fsproj"
        {   ProjectFileName = projectFileName
            ProjectFileNames = [|sourceFile|]
            OtherOptions = compilerFlags
            ReferencedProjects =  [||]
            IsIncompleteTypeCheckEnvironment = true
            UseScriptResolutionRules = true
            LoadTime = loadTime
            UnresolvedReferences = None
            OriginalLoadReferences = []
            ExtraProjectInfo = workspace
        }


type internal ProjectOptionsCache (checker:FSharpChecker) =
   
    // A table of information about projects, excluding single-file projects.  
    let projectTable = ConcurrentDictionary<ProjectId, FSharpProjectOptions>()

    // A table of information about single-file projects.  Currently we only need the load time of each such file, plus
    // the original options for editing
    let singleFileProjectTable = ConcurrentDictionary<ProjectId, DateTime * FSharpProjectOptions>()

    let toFSharpProjectOptions = ProjectOptionsCache.toFSharpProjectOptions projectTable

    member __.TryGetOptions (projectId:ProjectId) = tryGet projectId projectTable : FSharpProjectOptions option

    member __.ProjectTable = projectTable 

    member this.AddSingleFileProject (projectId, timeStampAndOptions) = 
        singleFileProjectTable.TryAdd (projectId, timeStampAndOptions) |> ignore    


    member __.AddProject (project:Project) = 
        if projectTable.ContainsKey project.Id then () else 
        let options = toFSharpProjectOptions  project.Solution.Workspace project
        projectTable.TryAdd (project.Id, options) |> ignore


    member this.RemoveSingleFileProject projectId = 
        singleFileProjectTable.TryRemove projectId |> ignore
    

    member __.SingleFileProjectTable = singleFileProjectTable

    member self.UpdateProject (project:Project) = 
        self.TryGetOptions project.Id |> Option.iter checker.InvalidateConfiguration
        let options = toFSharpProjectOptions project.Solution.Workspace project
        if projectTable.ContainsKey project.Id then
            projectTable.[project.Id] <- options
        else projectTable.TryAdd (project.Id,options) |> ignore


    member self.RemoveProject (project:Project) = 
        match tryGet project.Id projectTable with
        | None -> ()
        | Some projectOptions -> 
            checker.InvalidateConfiguration projectOptions
            projectTable.TryRemove project.Id |> ignore
            for project in project.GetDependentProjects() do
                self.UpdateProject project


    /// Get compilation defines relevant for syntax processing.  
    /// Quicker then TryGetOptionsForDocumentOrProject as it doesn't need to recompute the exact project 
    /// options for a script.
    member this.GetCompilationDefinesForEditingDocument (document: Document) = 
        let projectOptionsOpt = this.TryGetOptions document.Project.Id
        let otherOptions = defaultArg (projectOptionsOpt |> Option.map (fun options -> options.OtherOptions |> Array.toList)) [] 
        CompilerEnvironment.GetCompilationDefinesForEditing (document.Name, otherOptions)


    /// Get the options for a document or project relevant for syntax processing.
    /// Quicker then TryGetOptionsForDocumentOrProject as it doesn't need to recompute the exact project options for a script.
    member self.TryGetOptionsForEditingDocumentOrProject(document: Document) = 
        let projectId = document.Project.Id
        let originalOptions = 
            tryGet projectId  singleFileProjectTable |> Option.map (fun (_loadTime, originalOptions) -> originalOptions)
        Option.orElse (self.TryGetOptions projectId) originalOptions  


    member __.Clear () =
        singleFileProjectTable.Clear ()
        projectTable.Clear ()

