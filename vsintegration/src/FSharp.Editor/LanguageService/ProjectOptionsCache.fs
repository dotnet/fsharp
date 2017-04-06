namespace Microsoft.VisualStudio.FSharp.Editor

open System
open System.Collections.Concurrent
open Microsoft.CodeAnalysis
open Microsoft.FSharp.Compiler.SourceCodeServices


module internal ProjectOptionsCache =
    open Microsoft.VisualStudio.Shell.Interop
    open Microsoft.VisualStudio

    let toFSharpProjectOptions (workspace: Workspace) (projectTable:ConcurrentDictionary<ProjectId, FSharpProjectOptions>) (project:Project): FSharpProjectOptions =
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
                OtherOptions = project.ParseOptions. PreprocessorSymbolNames |> Array.ofSeq
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



open ProjectOptionsCache

type internal ProjectOptionsCache (checker:FSharpChecker, workspace: Workspace) =
   
    // A table of information about projects, excluding single-file projects.  
    let projectTable = ConcurrentDictionary<ProjectId, FSharpProjectOptions>()

    // A table of information about single-file projects.  Currently we only need the load time of each such file, plus
    // the original options for editing
    let singleFileProjectTable = ConcurrentDictionary<ProjectId, DateTime * FSharpProjectOptions>()

    let toFSharpProjectOptions = ProjectOptionsCache.toFSharpProjectOptions workspace projectTable


    member __.TryGetOptions (projectId:ProjectId) = tryGet projectId projectTable : FSharpProjectOptions option

    member __.ProjectTable = projectTable 

    member this.AddSingleFileProject (projectId, timeStampAndOptions) = async {
        singleFileProjectTable.TryAdd (projectId, timeStampAndOptions) |> ignore
    }


    member __.AddProject (project:Project) = async {
        if projectTable.ContainsKey project.Id then return () else 
            let options = toFSharpProjectOptions  project
            projectTable.TryAdd (project.Id, options) |> ignore
        }
    

    member self.AddProject (projectId:ProjectId) = async {
        match workspace.CurrentSolution.TryGetProject projectId with
        | None -> return () 
        | Some project -> do! self.AddProject project
    }


    member this.RemoveSingleFileProject projectId = async {
        singleFileProjectTable.TryRemove projectId |> ignore
    }

    member __.SingleFileProjectTable = singleFileProjectTable
    ///// Get the exact options for a single-file script
    //member __.ComputeSingleFileOptions (fileName, loadTime, fileContents, workspace: Workspace) = async {
    //    let extraProjectInfo = Some (box workspace)
    //    if SourceFile.MustBeSingleFileProject fileName then 
    //    //if isScriptFile fileName then 
    //        let! _options, _diagnostics = checker.GetProjectOptionsFromScript (fileName, fileContents, loadTime, [| |], ?extraProjectInfo=extraProjectInfo) 
    //        //ProjectOptionsCache.fsxProjectOptions fileName (Some workspace)
    //        return getProjectOptionsForProjectSite ( fileName, loadTime,extraProjectInfo)
    //    else
    //        //ProjectOptionsCache.singleFileProjectOptions fileName
    //        return getProjectOptionsForProjectSite ( fileName, loadTime, extraProjectInfo)
    //}

    member self.UpdateProject (project:Project) = async {
        self.TryGetOptions project.Id |> Option.iter checker.InvalidateConfiguration
        let options = toFSharpProjectOptions project
        if projectTable.ContainsKey project.Id then
            projectTable.[project.Id] <- options
        else projectTable.TryAdd (project.Id,options) |> ignore
    }


    member self.UpdateProject (projectId:ProjectId) = async {
        match workspace.CurrentSolution.TryGetProject projectId with
        | None -> do! self.AddProject projectId
        | Some project -> do! self.UpdateProject project
    }


    member self.RemoveProject (project:Project) = async {
        match tryGet project.Id projectTable with
        | None -> return ()
        | Some projectOptions -> 
            checker.InvalidateConfiguration projectOptions
            projectTable.TryRemove project.Id |> ignore
            for project in project.GetDependentProjects() do
                do! self.UpdateProject project
    }


    member self.RemoveProject (projectId:ProjectId) = async {
        match workspace.CurrentSolution.TryGetProject projectId with
        | None -> return () 
        | Some project -> do! self.RemoveProject project
    }


    /// Get compilation defines relevant for syntax processing.  
    /// Quicker then TryGetOptionsForDocumentOrProject as it doesn't need to recompute the exact project 
    /// options for a script.
    member this.GetCompilationDefinesForEditingDocument (document: Document) = 
        let projectOptionsOpt = this.TryGetOptions document.Project.Id
        let otherOptions = defaultArg (projectOptionsOpt |> Option.map (fun options -> options.OtherOptions |> Array.toList)) [] 
        CompilerEnvironment.GetCompilationDefinesForEditing (document.Name, otherOptions)


    ///// Get the exact options for a document or project
    //member self.TryGetOptionsForDocumentOrProject(document: Document) = asyncMaybe { 
    //    let projectId = document.Project.Id
    //    // The options for a single-file script project are re-requested each time the file is analyzed.  This is because the
    //    // single-file project may contain #load and #r references which are changing as the user edits, and we may need to re-analyze
    //    // to determine the latest settings.  FCS keeps a cache to help ensure these are up-to-date.
    //    match tryGet projectId singleFileProjectTable with
    //    | Some (loadTime,_) ->
    //        let fileName = document.FilePath
    //        let! cancellationToken = Async.CancellationToken |> liftAsync
    //        let! sourceText = document.GetTextAsync(cancellationToken)
    //        let! options = self.ComputeSingleFileOptions( fileName, loadTime,sourceText.ToString(), document.Project.Solution.Workspace)
    //        singleFileProjectTable.[projectId] <- (loadTime, options)
    //        return options
    //    | None ->
    //        match self.TryGetOptions projectId with
    //        | Some options ->
    //            return options
    //        | None ->
    //            return! None
    //}


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

