namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Concurrent
open Microsoft.CodeAnalysis
open Microsoft.FSharp.Compiler.SourceCodeServices
open Microsoft.VisualStudio.LanguageServices.Implementation.LanguageService
open Microsoft.VisualStudio.LanguageServices.Implementation.ProjectSystem

module internal ProjectOptionsCache =


    let createFSharpProjectOptionsEnvDTE (projectTable:ConcurrentDictionary<ProjectId, FSharpProjectOptions>)  (workspace: VisualStudioWorkspaceImpl) (project:EnvDTE.Project): FSharpProjectOptions =
        let loadTime = System.DateTime.Now
        let rec generate (project:EnvDTE.Project) : FSharpProjectOptions =
            let getProjectRefs (project:EnvDTE.Project): (string * FSharpProjectOptions)[] =
                project.GetReferencedProjects()
                |> Seq.map (fun pref -> 
                    let projectId = workspace.ProjectTracker.GetOrCreateProjectIdForPath (pref.FullName, pref.Name)
                    if projectTable.ContainsKey projectId  then
                        (pref.GetOutputPath(), projectTable.[projectId])
                    else
                        let fsinfo = generate pref
                        projectTable.TryAdd (projectId , fsinfo) |> ignore
                        (pref.GetOutputPath(), fsinfo)
                    )
                |> Array.ofSeq
            
            {   ProjectFileName = project.FullName
                ProjectFileNames = project.GetFiles () |> Seq.filter SourceFile.IsCompilable |> Array.ofSeq
                // TODO - This does not include all necessary compiler flags
                // if possible all of the the requisite compiler flags should be acquired from
                // the CodeAnalysis Project and converted into the FSC desired form
                OtherOptions = project.GetReferencePaths () |> List.map (fun path -> sprintf " -r:%s" path) |> Array.ofList
                ReferencedProjects =  getProjectRefs project
                IsIncompleteTypeCheckEnvironment = false
                UseScriptResolutionRules = false
                LoadTime = loadTime
                UnresolvedReferences = None
                OriginalLoadReferences = []
                ExtraProjectInfo = Some (box (workspace:>Workspace))
            }
        let fsprojOptions = generate project
        fsprojOptions

 
    let createFSharpProjectOptions (projectTable:ConcurrentDictionary<ProjectId, FSharpProjectOptions>)  (workspace: Workspace) (project:Project): FSharpProjectOptions =
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
                ProjectFileNames = project.Documents |> Seq.map (fun doc -> doc.FilePath) |> Seq.filter SourceFile.IsCompilable  |> Array.ofSeq
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
                ExtraProjectInfo = Some (box workspace)
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

    let createFSharpProjectOptions = ProjectOptionsCache.createFSharpProjectOptions projectTable
    let createFSharpProjectOptionsEnvDTE = ProjectOptionsCache.createFSharpProjectOptionsEnvDTE projectTable

    member __.TryGetOptions (projectId:ProjectId) = tryGet projectId projectTable : FSharpProjectOptions option

    member __.ProjectTable = projectTable 

    member this.AddSingleFileProject (projectId, timeStampAndOptions) = 
        singleFileProjectTable.TryAdd (projectId, timeStampAndOptions) |> ignore    

    member this.RemoveSingleFileProject projectId = 
        singleFileProjectTable.TryRemove projectId |> ignore
    
    member __.SingleFileProjectTable = singleFileProjectTable

    member __.AddProject (project:Project) = 
        if isNull project then () else
        if projectTable.ContainsKey project.Id then () else 
        let options = createFSharpProjectOptions  project.Solution.Workspace project
        projectTable.TryAdd (project.Id, options) |> ignore


    member __.AddProject (project:EnvDTE.Project, workspace:VisualStudioWorkspaceImpl)  = 
        if isNull project then () else
        let projectId = workspace.ProjectTracker.GetOrCreateProjectIdForPath (project.FullName, project.Name)
        if projectTable.ContainsKey projectId then () else 
        let options = createFSharpProjectOptionsEnvDTE  workspace project
        projectTable.TryAdd (projectId, options) |> ignore


    member self.UpdateProject (project:EnvDTE.Project, workspace:VisualStudioWorkspaceImpl)  = 
        if isNull project then () else
        let projectId = workspace.ProjectTracker.GetOrCreateProjectIdForPath (project.FullName, project.Name)
        if projectTable.ContainsKey projectId then 
            self.TryGetOptions projectId |> Option.iter checker.InvalidateConfiguration
            let options = createFSharpProjectOptionsEnvDTE  workspace project
            projectTable.[projectId] <- options
        else 
            self.AddProject (project, workspace)


    member self.UpdateProject (project:Project) = 
        if isNull project then () else
        if projectTable.ContainsKey project.Id then
            self.TryGetOptions project.Id |> Option.iter checker.InvalidateConfiguration
            let options = createFSharpProjectOptions project.Solution.Workspace project
            projectTable.[project.Id] <- options
        else 
            self.AddProject project
            


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
    member this.GetCompilationDefinesForEditingDocument (document:Document) = 
        let projectOptionsOpt = this.TryGetOptions document.Project.Id
        let otherOptions = defaultArg (projectOptionsOpt |> Option.map (fun options -> options.OtherOptions |> Array.toList)) [] 
        CompilerEnvironment.GetCompilationDefinesForEditing (document.Name, otherOptions)


    /// Get the options for a document or project relevant for syntax processing.
    /// Quicker then TryGetOptionsForDocumentOrProject as it doesn't need to recompute the exact project options for a script.
    member self.TryGetOptionsForEditingDocumentOrProject (document:Document) = 
        let projectId = document.Project.Id
        let originalOptions = 
            tryGet projectId  singleFileProjectTable |> Option.map (fun (_loadTime, originalOptions) -> originalOptions)
        Option.orElse (self.TryGetOptions projectId) originalOptions  


    member __.Clear () =
        singleFileProjectTable.Clear ()
        projectTable.Clear ()

