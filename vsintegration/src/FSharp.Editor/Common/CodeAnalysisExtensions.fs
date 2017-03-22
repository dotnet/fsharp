[<AutoOpen>]
module internal Microsoft.VisualStudio.FSharp.Editor.CodeAnalysisExtensions

open Microsoft.CodeAnalysis
open Microsoft.FSharp.Compiler.Range

type Project with

    /// The list all projects within the same solution that reference this project.
    member this.GetDependentProjects () =
        this.Solution.GetProjectDependencyGraph().GetProjectsThatDirectlyDependOnThisProject(this.Id)
        |> Seq.map this.Solution.GetProject

type Solution with 

    /// Try to get a document inside the solution using the document's name
    member self.TryGetDocumentNamed docName =
        self.Projects |> Seq.tryPick (fun proj ->
            proj.Documents |> Seq.tryFind (fun doc -> doc.Name = docName))

    /// Try to get a project inside the solution using the project's id
    member self.TryGetProject (projId:ProjectId) =
        if self.ContainsProject projId then Some (self.GetProject projId) else None


    member self.GetDependentProjects (projectId:ProjectId) =
        self.GetProject(projectId).GetDependentProjects()


    /// Try to find the document assoicated with the F# range in the solution
    /// If provided a projectId prefer a copy of the document from that project
    member self.TryGetDocumentFromFSharpRange (range:range,?projectId:ProjectId) =
        /// Retrieve the DocumentId from the workspace for the range's file
        let filePath = System.IO.Path.GetFullPathSafe range.FileName
        let candidates = self.GetDocumentIdsWithFilePath filePath
        match candidates.Length with 
        | 0 -> None 
        | 1 -> Some (self.GetDocument candidates.[0])
        | _ -> 
            match projectId with 
            | Some projectId -> 
                let r = candidates |> Seq.tryFind (fun docId -> 
                        (self.GetDependentProjects docId.ProjectId 
                        |> Seq.tryFind (fun proj -> proj.Id = projectId))
                        |> Option.isSome) 
                if r.IsSome then Some (self.GetDocument r.Value)
                else Some (self.GetDocument candidates.[0])
            | None -> Some (self.GetDocument candidates.[0])
